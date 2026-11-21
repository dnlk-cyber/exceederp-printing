using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    internal sealed class ApplicationVacancy
    {
        public int VacancyId { get; set; }
        public string VacancyCode { get; set; }
        public bool IsActive { get; set; }
        public int BranchId { get; set; }
        public VacancyType VacancyType { get; set; }
        public int WorkUnitId { get; set; }
        public int JobTitleId { get; set; }
        public string JobTitle { get; set; }

        internal static IQueryable<ApplicationVacancy> Query(ExceedDbContext db)
        {
            return from vacancy in db.VaccancyPlans
                   where vacancy.RecruSource == RecruitmentSource.INTERNAL
                       || vacancy.RecruSource == RecruitmentSource.INTERNALPROMOTION
                       || vacancy.RecruSource == RecruitmentSource.INTERNALTRANSFER
                   join recruitmentPlan in db.RecruitmentPlans on vacancy.RecruitmentPlanId equals recruitmentPlan.RecruitmentPlanId into plans
                   from plan in plans.DefaultIfEmpty()
                   let workUnitId = (vacancy.Department ?? 0) > 0 ? vacancy.Department.Value : plan == null ? 0 : plan.OrgStructureId
                   let jobTitleId = vacancy.JobTitle > 0 ? vacancy.JobTitle : plan == null ? 0 : plan.LookupId
                   select new ApplicationVacancy
                   {
                       VacancyId = vacancy.VacancyId,
                       VacancyCode = vacancy.VaccancyCode,
                       IsActive = vacancy.IsActive,
                       BranchId = vacancy.BranchId,
                       VacancyType = vacancy.VacancyType,
                       WorkUnitId = workUnitId,
                       JobTitleId = jobTitleId,
                       JobTitle = db.Lookups.Where(x => x.LookupId == jobTitleId).Select(x => x.Description).FirstOrDefault()
                   };
        }

        internal static IQueryable<MainCompanyStructure> WorkUnits(ExceedDbContext db, ApplicationVacancy vacancy)
        {
            if (vacancy.WorkUnitId > 0)
            {
                var configured = db.MainCompanyStructures.Where(x => x.OrgStructureId == vacancy.WorkUnitId);
                if (configured.Any())
                    return configured;
            }

            var active = db.MainCompanyStructures.Where(x => x.IsActive);
            var mappings = db.JobTitleMapping.Where(x => x.IsActive && x.BranchId == vacancy.BranchId);
            var titleMappings = mappings.Where(x => x.LookupId == vacancy.JobTitleId).Select(x => x.OrgStructureId);
            var jobWorkUnits = active.Where(x => titleMappings.Contains(x.OrgStructureId));
            if (jobWorkUnits.Any())
                return jobWorkUnits;

            var branchMappings = mappings.Select(x => x.OrgStructureId);
            var branchCostCenters = db.BranchCostCenter.Where(x => x.BranchId == vacancy.BranchId).Select(x => x.OrgStructureId);
            var branchWorkUnits = active.Where(x => branchMappings.Contains(x.OrgStructureId) || branchCostCenters.Contains(x.OrgStructureId));
            return branchWorkUnits.Any() ? branchWorkUnits : active;
        }

        internal static void Apply(ExceedDbContext db, ApplicantionForm form, ModelStateDictionary modelState, int? existingVacancyId = null)
        {
            var vacancy = Query(db).SingleOrDefault(x => x.VacancyId == form.VacancyId);
            if (vacancy == null || (!vacancy.IsActive && vacancy.VacancyId != existingVacancyId))
            {
                modelState.AddModelError(nameof(form.VacancyId), "Select an active internal vacancy.");
                return;
            }

            if (string.IsNullOrWhiteSpace(vacancy.JobTitle))
            {
                modelState.AddModelError(nameof(form.VacancyId), "Set the job title on this vacancy before saving the application.");
                return;
            }

            if (!WorkUnits(db, vacancy).Any(x => x.OrgStructureId == form.OrgStructureId))
            {
                modelState.AddModelError(nameof(form.OrgStructureId), "Select a work unit for the chosen vacancy.");
                return;
            }

            form.LookupId = vacancy.JobTitleId;
            form.VacancyType = vacancy.VacancyType;
            modelState.Remove(nameof(form.LookupId));
        }
    }
}
