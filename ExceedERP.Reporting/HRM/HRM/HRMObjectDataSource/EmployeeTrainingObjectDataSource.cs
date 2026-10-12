using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;
namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class EmployeeTrainingObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();
        string cultureName = GlobalHelper.GetCurrentCulture();

        [DataObjectMethod(DataObjectMethodType.Select)]
        //public List<EmployeeTraining> GetEmployeeTraining(string fiscalYear, string type, string status, DateTime? from, DateTime? to, string branchId, string orgStructureId, string businessUnit)
        //{
        //    var employeeTrainingList = new List<EmployeeTraining>();
        //    var context = db;
        //    var placements = context.Placements.Where(p => p.isCurrent).AsQueryable();

        //    // Apply filters
        //    if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
        //        placements = placements.Where(x => x.BranchId == braId);

        //    if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
        //        placements = placements.Where(x => x.OrgStructureId == orgId || x.ParentOrgStructureId == orgId);

        //    if (!string.IsNullOrEmpty(businessUnit) && int.TryParse(businessUnit, out int buId))
        //        placements = placements.Where(x => x.BusinessUnitId == buId);

        //    var coursesQuery = context.CourseTrackings.AsQueryable();
        //    if (!string.IsNullOrEmpty(fiscalYear) && int.TryParse(fiscalYear, out int yearId))
        //        coursesQuery = coursesQuery.Where(c => c.GlFiscalYearId == yearId);

        //    if (!string.IsNullOrEmpty(type))
        //        coursesQuery = coursesQuery.Where(c => c.TrainingType.ToString().ToUpper() == type.ToUpper());

        //    if (from.HasValue)
        //        coursesQuery = coursesQuery.Where(c => c.From <= from.Value || c.To >= from.Value);

        //    if (to.HasValue)
        //        coursesQuery = coursesQuery.Where(c => c.To < to.Value || c.From <= to.Value);

        //    var courses = coursesQuery.ToList();
        //    var providers = context.Companies.Where(x => x.Category == "1").ToList();
        //    var jobs = context.Lookups.ToList();
        //    var fiscalYears = context.GLFiscalYears.ToList();
        //    var departments = context.MainCompanyStructures.ToList();
        //    var courseIds = courses.Select(c => c.CourseTrackingId).Distinct();
        //    // Get distinct employees with their trainings
        //    var distinctEmployees = context.Trainee
        //        .Where(t => courseIds.Contains(t.CourseTrackingId))
        //        .GroupBy(t => t.EmployeeId)
        //        .Select(g => new {
        //            EmployeeId = g.Key,
        //            Courses = g.Select(t => t.CourseTrackingId).ToList()
        //        })
        //        .ToList();

        //    foreach (var emp in distinctEmployees)
        //    {
        //        var employee = context.Person_Employee
        //            .FirstOrDefault(e => e.EmployeeId == emp.EmployeeId);
        //        if (employee == null) continue;
        //        var placement = placements.FirstOrDefault(p => p.EmployeeId == emp.EmployeeId);
        //        var depId = placement?.OrgStructureId ?? 0;
        //        var dept = departments.FirstOrDefault(d => d.OrgStructureId == depId)?.Name;
        //        // Initialize concatenated fields
        //        var courseNames = new List<string>();
        //        var providerNames = new List<string>();
        //        var fromDates = new List<string>();
        //        var toDates = new List<string>();
        //        var periods = new List<string>();
        //        var trainingTypes = new List<string>();
        //        decimal totalCost = 0;

        //        foreach (var courseId in emp.Courses)
        //        {
        //            var course = courses.FirstOrDefault(c => c.CourseTrackingId == courseId);
        //            if (course == null) continue;

        //            // Get trainee count for cost calculation
        //            var traineeCount = context.Trainee
        //                .Count(t => t.CourseTrackingId == courseId);
        //            int count = traineeCount > 0 ? traineeCount : 1;

        //            totalCost += course.Cost / count;

        //            // Add details for concatenation
        //            courseNames.Add(jobs.FirstOrDefault(j => j.LookupId == course.LookupId)?.Description);
        //            providerNames.Add(providers.FirstOrDefault(p => p.MainCompanyId == course.MainCompanyId)?.TradeName);
        //            fromDates.Add(XAPI.EthiopicDateTime.GetEthiopicDate(course.From.Day, course.From.Month, course.From.Year));
        //            toDates.Add(XAPI.EthiopicDateTime.GetEthiopicDate(course.To.Day, course.To.Month, course.To.Year));
        //            periods.Add(fiscalYears.FirstOrDefault(y => y.GlFiscalYearId == course.GlFiscalYearId)?.Name);
        //            trainingTypes.Add(course.TrainingType == TrainingType.TRAINING ? "Training" : "Education");
        //        }

        //        employeeTrainingList.Add(new EmployeeTraining
        //        {
        //            EmployeeId = employee.EmployeeCode,
        //            FullName = $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}",
        //            Sex = HrHelper.GetGender(employee.Gender),
        //            From = string.Join(", ", fromDates),
        //            To = string.Join(", ", toDates),
        //            Cost = totalCost,
        //            Period = string.Join(", ", periods.Distinct()),
        //            Departement = dept,
        //            Course = string.Join(", ", courseNames),
        //            Provider = string.Join(", ", providerNames),
        //            TrainingType = string.Join(", ", trainingTypes.Distinct())
        //        });
        //    }

        //    return employeeTrainingList;
        //}
        public List<EmployeeTraining> GetEmployeeTraining(string fiscalYear, string type, string status, DateTime? from, DateTime? to, string branchId, string orgStructureId, string businessUnit)
        {
            var employeeTrainingList = new List<EmployeeTraining>();
            var context = db; // use the already initialized db context
            var placements = context.Placements.Where(p => p.isCurrent).AsQueryable();

            // Apply filters
            if (!string.IsNullOrEmpty(branchId) && int.TryParse(branchId, out int braId))
                placements = placements.Where(x => x.BranchId == braId).AsQueryable();

            if (!string.IsNullOrEmpty(orgStructureId) && int.TryParse(orgStructureId, out int orgId))
                placements = placements.Where(x => x.OrgStructureId == orgId || x.ParentOrgStructureId == orgId).AsQueryable();
            //placements = EmployeeHelper.GetEmployees_ByDepartment_WithCaseTeam_FromPlacement(placements.ToList(), orgId).AsQueryable();

            if (int.TryParse(businessUnit, out int buId))
                placements = placements.Where(x => x.BusinessUnitId == buId);
            var coursesQuery = context.CourseTrackings.AsQueryable();
            if (!string.IsNullOrEmpty(fiscalYear) && int.TryParse(fiscalYear, out int yearId))
                coursesQuery = coursesQuery.Where(c => c.GlFiscalYearId == yearId);
            if (!string.IsNullOrEmpty(type))
                coursesQuery = coursesQuery.Where(c => c.TrainingType.ToString().ToUpper() == type.ToUpper());
            if (from.HasValue)
                coursesQuery = coursesQuery.Where(c => c.From <= from.Value || c.To >= from.Value).AsQueryable();
            if (to.HasValue)
                coursesQuery = coursesQuery.Where(c => c.To < to.Value || c.From <= to.Value).AsQueryable();
            var courses = coursesQuery.ToList();
            var providers = context.Companies.Where(x => x.Category == "1").ToList();
            var jobs = context.Lookups.ToList();
            var fiscalYears = context.GLFiscalYears.ToList();
            var departments = context.MainCompanyStructures.ToList();
            foreach (var course in courses)
            {
                DateTime cFrom = course.From.Date;
                DateTime cTo = course.To.Date;
                var trainees = context.Trainee.Where(t => t.CourseTrackingId == course.CourseTrackingId);

                // Status filter
                if (!string.IsNullOrEmpty(status) &&status == "COMPLETED")
                    trainees = trainees.Where(t => t.IsCompleteTraining);
                else if (!string.IsNullOrEmpty(status) &&status == "NOT_COMPLETED")
                    trainees = trainees.Where(t => !t.IsCompleteTraining);

                var traineeList = trainees.ToList();
                int count = traineeList.Count == 0 ? 1 : traineeList.Count;

                foreach (var trainee in traineeList)
                {
                    var employee = context.Person_Employee
                        .Where(e => e.EmployeeId == trainee.EmployeeId)
                        .FirstOrDefault();
                    if (employee != null)
                    {
                        var placement = placements.FirstOrDefault(p => p.EmployeeId == employee.EmployeeId);
                        var depId = placement?.OrgStructureId ?? 0;
                        var dept = departments.FirstOrDefault(d => d.OrgStructureId == depId);

                        employeeTrainingList.Add(new EmployeeTraining
                        {
                            EmployeeId = employee.EmployeeCode,
                            FullName = (cultureName.ToLower() == "am" || cultureName.ToLower() == "am-et")
                                        ? $"{employee.FirstName} {employee.MiddleName} {employee.LastName}"
                                        : $"{employee.FirstNameEnglish} {employee.MiddleNameEnglish} {employee.LastNameEnglish}",
                            Sex = HrHelper.GetGender(employee.Gender),
                            From = XAPI.EthiopicDateTime.GetEthiopicDate(cFrom.Day, cFrom.Month, cFrom.Year),
                            To = XAPI.EthiopicDateTime.GetEthiopicDate(cTo.Day, cTo.Month, cTo.Year),
                            Cost = course.Cost / count,
                            Period = fiscalYears.FirstOrDefault(y => y.GlFiscalYearId == course.GlFiscalYearId)?.Name,
                            Departement = dept?.Name,
                            Course = jobs.FirstOrDefault(j => j.LookupId == course.LookupId)?.Description,
                            Provider = providers.FirstOrDefault(p => p.MainCompanyId == course.MainCompanyId)?.TradeName,
                            TrainingType = course.TrainingType == TrainingType.TRAINING ? "Training" : "Education"
                        });
                    }
                }
            }

            return employeeTrainingList;
        }

        public void Dispose()
        {
            db.Dispose();
        }
    }
}