using System.Data;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class VacancyTrackingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Lookup"] = db.Lookups.ToList();
            ViewData["Branches"] = db.Branch.ToList();
          //  ViewData["Location"] = db.BranchLocations.ToList();
            ViewData["Department"] = db.MainCompanyStructures.ToList();
            return View();
        }

        public ActionResult VaccancyPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Vacancy> vaccancyplans = db.VaccancyPlans.Where(x=>x.IsActive);
            DataSourceResult result = vaccancyplans.ToDataSourceResult(request, vacancy => new {
                VacancyId = vacancy.VacancyId,
                VaccancyCode = vacancy.VaccancyCode,
                RecruitmentPlanId = vacancy.RecruitmentPlanId,
                From = vacancy.From,
                FromVacAm = vacancy.FromVacAm,
                To = vacancy.To,
                ToVacAm = vacancy.ToVacAm,
                JobTitle = vacancy.JobTitle,
                RecruSource = vacancy.RecruSource,
                BranchId = vacancy.BranchId,
                Remark = vacancy.Remark,
                VacancyType = vacancy.VacancyType,
                Check = vacancy.Check,
                Department = vacancy.Department,
                IsActive = vacancy.IsActive,
                NoOfVacant = vacancy.NoOfVacant,
                WrittenExamSchedule = vacancy.WrittenExamSchedule,
                InterviewSchedule = vacancy.InterviewSchedule,
                Location = vacancy.Location,
               // CurrentStatusTracking = vacancy.CurrentStatusTracking,
                Salary = vacancy.Salary,
                ExperienceLevel = vacancy.ExperienceLevel,
                TotalYearOfExperience = vacancy.TotalYearOfExperience,
                JobRequirement = vacancy.JobRequirement,
                Responsibility = vacancy.Responsibility,
                EthioJobsCode = vacancy.EthioJobsCode,
                Language = vacancy.Language,
                Gender = vacancy.Gender,
                OrgBranchName = vacancy.OrgBranchName,
                DateCreated = vacancy.DateCreated,
                LastModified = vacancy.LastModified,
                CreatedBy = vacancy.CreatedBy,
                ModifiedBy = vacancy.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VaccancyPlans_Update([DataSourceRequest]DataSourceRequest request, Vacancy vacancy)
        {
            if (ModelState.IsValid)
            {
                var entity = new Vacancy
                {
                    VacancyId = vacancy.VacancyId,
                    VaccancyCode = vacancy.VaccancyCode,
                    RecruitmentPlanId = vacancy.RecruitmentPlanId,
                    From = vacancy.From,
                    FromVacAm = vacancy.FromVacAm,
                    To = vacancy.To,
                    ToVacAm = vacancy.ToVacAm,
                    JobTitle = vacancy.JobTitle,
                    RecruSource = vacancy.RecruSource,
                    BranchId = vacancy.BranchId,
                    Remark = vacancy.Remark,
                    VacancyType = vacancy.VacancyType,
                    Check = vacancy.Check,
                    Department = vacancy.Department,
                    IsActive = vacancy.IsActive,
                    NoOfVacant = vacancy.NoOfVacant,
                    WrittenExamSchedule = vacancy.WrittenExamSchedule,
                    InterviewSchedule = vacancy.InterviewSchedule,
                    Location = vacancy.Location,
                   // CurrentStatusTracking = vacancy.CurrentStatusTracking,
                    Salary = vacancy.Salary,
                    ExperienceLevel = vacancy.ExperienceLevel,
                    TotalYearOfExperience = vacancy.TotalYearOfExperience,
                    JobRequirement = vacancy.JobRequirement,
                    Responsibility = vacancy.Responsibility,
                    EthioJobsCode = vacancy.EthioJobsCode,
                    Language = vacancy.Language,
                    Gender = vacancy.Gender,
                    OrgBranchName = vacancy.OrgBranchName,
                    DateCreated = vacancy.DateCreated,
                    LastModified = vacancy.LastModified,
                    CreatedBy = vacancy.CreatedBy,
                    ModifiedBy = vacancy.ModifiedBy
                };

                db.VaccancyPlans.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { vacancy }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
