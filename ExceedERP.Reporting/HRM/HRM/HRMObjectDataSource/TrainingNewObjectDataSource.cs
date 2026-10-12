using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class TrainingNewObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TrainingVM> GetEmployeeTraining(string fiscalYear,string type, string status, DateTime? from, DateTime? to, string branchId, string orgStructureId, string businessUnit)
        {
            var trainingList = new List<TrainingVM>();
            var context = db; // use the already initialized db context
            DateTime dateFrom = from?.Date ?? DateTime.MinValue;
            DateTime dateTo = to?.Date ?? DateTime.MaxValue;

            var placements = context.Placements.Where(p => p.isCurrent).AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(branchId) &&int.TryParse(branchId, out int braId))
                placements = placements.Where(x=>x.BranchId==braId).AsQueryable();

            if (int.TryParse(orgStructureId, out int orgId))
                placements = placements.Where(x=>x.OrgStructureId== orgId||x.ParentOrgStructureId==orgId).AsQueryable();
                //placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements.ToList(), orgId).AsQueryable();

            if (!string.IsNullOrEmpty(businessUnit) &&int.TryParse(businessUnit, out int buId))
                placements = placements.Where(x => x.BusinessUnitId == buId);

            var coursesQuery = context.CourseTrackings.AsQueryable();

            if (!string.IsNullOrEmpty(type))
                coursesQuery = coursesQuery.Where(c => c.TrainingType.ToString().ToUpper() == type.ToUpper());
           
               if (!string.IsNullOrEmpty(fiscalYear)&& int.TryParse(fiscalYear, out int fiscalYearId))
                coursesQuery = coursesQuery.Where(c => c.GlFiscalYearId == fiscalYearId);
           
            var courses = coursesQuery.ToList();
            var providers = context.Companies.Where(x => x.Category == "1").ToList();
            var jobs = context.Lookups.ToList();
            var fiscalYears = context.GLFiscalYears.ToList();
            var departments = context.MainCompanyStructures.ToList();

            foreach (var course in courses)
            {
                DateTime cFrom = course.From.Date;
                DateTime cTo = course.To.Date;

                if (cFrom < dateFrom || cTo > dateTo)
                    continue;

                var trainees = context.Trainee.Where(t => t.CourseTrackingId == course.CourseTrackingId).DistinctByField(x=>x.EmployeeId);
                var plans = context.TrainingNeedRequests
                    .Where(t => t.assessment.LookupId == course.LookupId
                    &&t.GlFiscalYearId==course.GlFiscalYearId)
                    .DistinctByField(x=>x.EmployeeId);
                var todatePlans = context.TrainingNeedRequests
                    .Where(t => t.assessment.LookupId == course.LookupId
                    &&t.GlFiscalYearId==course.GlFiscalYearId)
                    .DistinctByField(x=>x.EmployeeId);
                
                // Status filter
                if (status == "COMPLETED")
                    trainees = trainees.Where(t => t.IsCompleteTraining);
                else if (status == "NOT_COMPLETED")
                    trainees = trainees.Where(t => !t.IsCompleteTraining);

                var traineeList = trainees.ToList();
                int count = traineeList.Count == 0 ? 1 : traineeList.Count;
                trainingList.Add(new TrainingVM
                {
                    TypeOfTraining = $"Course:{jobs.FirstOrDefault(j => j.LookupId == course.LookupId)?.Description} :Type: { course.TrainingType}",
                    Unit = "Number of people",
                    AnnualPlan = plans.Any() ? plans.Count() : 0,
                    Plan = plans.Any()?plans.Count():0,
                    Excution = count,
                    TodatePlan = plans.Any() ? plans.Count() : 0,
                    TodateExcution = count,
                    TrainingCost = count*course.Cost,
                   
                });
               
            }

            return trainingList;
        }
        public void Dispose()
        {
            db.Dispose();
        }
    }
}