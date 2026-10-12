using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.HRM.HRMViewModel;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class SalarySummaryReportObjectDataSource : IDisposable
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SalarySummaryReportVM> GetSalarySummaryReport(string branch)
        {
            int.TryParse(branch, out int branchId);

            var placementQuery =
                from p in db.Placements
                join e in db.Person_Employee on p.EmployeeId equals e.EmployeeId
                where e.IsActive && p.isCurrent
                select new
                {
                    p.EmployeeId,
                    p.EmployementType,
                    p.Salary,
                    e.BranchId
                };

            if (branchId > 0)
                placementQuery = placementQuery.Where(x => x.BranchId == branchId);

            // EMPLOYEE + SALARY GROUPING
            var salaryGrouped = placementQuery
                .GroupBy(x => x.EmployementType)
                .Select(g => new
                {
                    Status = g.Key,
                    EmployeeCount = g.Select(x => x.EmployeeId).Distinct().Count(),
                    GrossSalary = g.Sum(x => x.Salary)
                });

            //  POSITION & RESPONSIBILITY ALLOWANCE
            var allowanceQuery =
                from a in db.AllowancePerEmployees
                join p in db.Placements on a.EmployeeId equals p.EmployeeId
                where !a.Pend && p.isCurrent
                select new
                {
                    p.EmployementType,
                    a.EmployeeId,
                    a.AllowanceDefination.AllowanceType,
                    a.AllowanceDefination.Amount
                };

            if (branchId > 0)
                allowanceQuery = allowanceQuery.Where(x =>
                    db.Person_Employee
                        .Where(e => e.EmployeeId == x.EmployeeId)
                        .Select(e => e.BranchId)
                        .FirstOrDefault() == branchId);

            var allowanceGrouped = allowanceQuery
                .GroupBy(x => x.EmployementType)
                .Select(g => new
                {
                    Status = g.Key,
                    PositionAllowance = g
                        .Where(x => x.AllowanceType == AllowanceType.POSITION)
                        .Sum(x => (decimal?)x.Amount) ?? 0,

                    ResponsibilityAllowance = g
                        .Where(x => x.AllowanceType == AllowanceType.RESPONSIBILITY)
                        .Sum(x => (decimal?)x.Amount) ?? 0
                });

            // OVERTIME
            var overtimeQuery =
                from o in db.OverTimes
                join p in db.Placements on o.EmployeeId equals p.EmployeeId
                where p.isCurrent
                select new
                {
                    p.EmployementType,
                    OT = o.HolidayHours + o.NightHours + o.NormalHours + o.RestdayHours
                };

            var overtimeGrouped = overtimeQuery
                .GroupBy(x => x.EmployementType)
                .Select(g => new
                {
                    Status = g.Key,
                    OT = g.Sum(x => (decimal?)x.OT) ?? 0
                });

            // MERGE ALL IN MEMORY (SMALL DATASET)
            var salaryData = salaryGrouped.ToList();
            var allowanceData = allowanceGrouped.ToList();
            var overtimeData = overtimeGrouped.ToList();

            var statuses = new[]
            {
                EmployeeStatus.PERMANENT,
                EmployeeStatus.CONTRACT,
                EmployeeStatus.PROBATION

            };

            var result = new List<SalarySummaryReportVM>();

            foreach (var status in statuses)
            {
                var salary = salaryData.FirstOrDefault(x => x.Status == status);
                var allowance = allowanceData.FirstOrDefault(x => x.Status == status);
                var ot = overtimeData.FirstOrDefault(x => x.Status == status);

                result.Add(new SalarySummaryReportVM
                {
                    Status = status.ToString(),
                    NumberOfEmployee = salary?.EmployeeCount ?? 0,
                    SumOfGrossSalary = salary?.GrossSalary ?? 0,
                    PositionAllowance = allowance?.PositionAllowance ?? 0,
                    ResponsibilityAllowance = allowance?.ResponsibilityAllowance ?? 0,
                    OT = ot?.OT ?? 0
                });
            }

            //  DAILY LABOUR (SEPARATE)
            var today = DateTime.Now;

            //var dailyLabour = (
            //    from gl in db.GLPeriods
            //    where gl.DateFrom < today && today < gl.DateTo
            //    join dl in db.DailyLaborers on gl.GLPeriodId equals dl.GlPeriodId
            //    join emp in db.DailyLaborerEmployees on dl.DailyLaborerId equals emp.DailyLaborerId
            //    select emp
            //).ToList();

            result.Add(new SalarySummaryReportVM
            {
                Status = "Daily Labour",
                NumberOfEmployee = 0,
                SumOfGrossSalary =  0,
                PositionAllowance = 0,
                ResponsibilityAllowance = 0,
                OT =  0
            });

            return result.OrderBy(x => x.Status).ToList();
        }
        //public List<SalarySummaryReportVM> GetSalarySummaryReport(string branch)
        //{
        //    int.TryParse(branch, out int branchId);


        //    // Base Query (adjust table names according to your schema)
        //    var query =
        //        from p in db.Placements
        //        join e in db.Person_Employee on p.EmployeeId equals e.EmployeeId
        //        where e.IsActive &&p.isCurrent
        //        select new { p, e };

        //    if (branchId > 0)
        //        query = query.Where(x => x.e.BranchId == branchId);



        //    var data = new List<SalarySummaryReportVM>();

        //    var allowancePerEmployees = db.AllowancePerEmployees.Where(x => !x.Pend).ToList();
        //    // Ensure all statuses exist even if no data
        //    var statuses = new List<string>
        //    {
        //        "Permanent",
        //        "Contract",
        //        "Daily Labour",
        //        "Probation"
        //    };

        //    foreach (var status in statuses)
        //    {
        //        switch (status)
        //        {
        //            case "Permanent":
        //                var permanentEmpIds = query.Where(p => p.p.EmployementType == EmployeeStatus.PERMANENT).Select(p => p.p.EmployeeId).Distinct();
        //                var possitionAllowanceIds = db.AllowanceDefinations.Where(x => x.AllowanceType == AllowanceType.POSITION).Select(x => x.AllowanceDefinationId).Distinct();
        //                var responsiblityAllowanceIds = db.AllowanceDefinations.Where(x => x.AllowanceType == AllowanceType.RESPONSIBILITY).Select(x => x.AllowanceDefinationId).Distinct();
        //                var employeeOT = db.OverTimes.Where(x => permanentEmpIds.Contains(x.EmployeeId));
        //                data.Add(new SalarySummaryReportVM
        //                {
        //                    Status = status,
        //                    NumberOfEmployee = permanentEmpIds.Count(),
        //                    SumOfGrossSalary = query.Where(p => p.p.EmployementType == EmployeeStatus.PERMANENT).Sum(p => p.p.Salary),
        //                    PositionAllowance = allowancePerEmployees.Where(x => possitionAllowanceIds.Contains(x.AllowanceDefinationId) && permanentEmpIds.Contains(x.EmployeeId)).Sum(x => x.AllowanceDefination.Amount),
        //                    ResponsibilityAllowance = allowancePerEmployees.Where(x => responsiblityAllowanceIds.Contains(x.AllowanceDefinationId) && permanentEmpIds.Contains(x.EmployeeId)).Sum(x => x.AllowanceDefination.Amount),
        //                    OT = employeeOT.Any() ? employeeOT.Sum(x => x.HolidayHours + x.NightHours + x.NormalHours + x.RestdayHours):0
        //                });
        //                break;
        //            case "Contract":
        //                var contractEmpIds = query.Where(p => p.p.EmployementType == EmployeeStatus.CONTRACT).Select(p => p.p.EmployeeId).Distinct();
        //                var contractpossitionAllowanceIds = db.AllowanceDefinations.Where(x => x.AllowanceType == AllowanceType.POSITION).Select(x => x.AllowanceDefinationId).Distinct();
        //                var contractresponsiblityAllowanceIds = db.AllowanceDefinations.Where(x => x.AllowanceType == AllowanceType.RESPONSIBILITY).Select(x => x.AllowanceDefinationId).Distinct();
        //                var contractemployeeOT = db.OverTimes.Where(x => contractEmpIds.Contains(x.EmployeeId));
        //                data.Add(new SalarySummaryReportVM
        //                {
        //                    Status = status,
        //                    NumberOfEmployee = contractEmpIds.Count(),
        //                    SumOfGrossSalary = query.Where(p => p.p.EmployementType == EmployeeStatus.PERMANENT).Sum(p => p.p.Salary),
        //                    PositionAllowance = allowancePerEmployees.Where(x => contractpossitionAllowanceIds.Contains(x.AllowanceDefinationId) && contractEmpIds.Contains(x.EmployeeId)).Sum(x => x.AllowanceDefination.Amount),
        //                    ResponsibilityAllowance = allowancePerEmployees.Where(x => contractresponsiblityAllowanceIds.Contains(x.AllowanceDefinationId) && contractEmpIds.Contains(x.EmployeeId)).Sum(x => x.AllowanceDefination.Amount),
        //                    OT = contractemployeeOT.Any() ? contractemployeeOT.Sum(x => x.HolidayHours + x.NightHours + x.NormalHours + x.RestdayHours):0
        //                });
        //                break;
        //            case "Probation":
        //                var ProbationEmpIds = query.Where(p => p.p.EmployementType == EmployeeStatus.PROBATION).Select(p => p.p.EmployeeId).Distinct();
        //                var ProbationpossitionAllowanceIds = db.AllowanceDefinations.Where(x => x.AllowanceType == AllowanceType.POSITION).Select(x => x.AllowanceDefinationId).Distinct();
        //                var ProbationresponsiblityAllowanceIds = db.AllowanceDefinations.Where(x => x.AllowanceType == AllowanceType.RESPONSIBILITY).Select(x => x.AllowanceDefinationId).Distinct();
        //                var ProbationemployeeOT = db.OverTimes.Where(x => ProbationEmpIds.Contains(x.EmployeeId));
        //                data.Add(new SalarySummaryReportVM
        //                {
        //                    Status = status,
        //                    NumberOfEmployee = ProbationEmpIds.Count(),
        //                    SumOfGrossSalary = query.Where(p => p.p.EmployementType == EmployeeStatus.PERMANENT).Sum(p => p.p.Salary),
        //                    PositionAllowance = allowancePerEmployees.Where(x => ProbationpossitionAllowanceIds.Contains(x.AllowanceDefinationId) && ProbationEmpIds.Contains(x.EmployeeId)).Sum(x => x.AllowanceDefination.Amount),
        //                    ResponsibilityAllowance = allowancePerEmployees.Where(x => ProbationresponsiblityAllowanceIds.Contains(x.AllowanceDefinationId) && ProbationEmpIds.Contains(x.EmployeeId)).Sum(x => x.AllowanceDefination.Amount),
        //                    OT = ProbationemployeeOT.Any() ? ProbationemployeeOT.Sum(x => x.HolidayHours + x.NightHours + x.NormalHours + x.RestdayHours):0
        //                });
        //                break;
        //            case "Daily Labour":
        //               var  periodIds = db.GLPeriods.Where(x => x.DateFrom < DateTime.Now && DateTime.Now < x.DateTo).Select(x => x.GLPeriodId);

        //                var dailyLaborIds = db.DailyLaborers.Where(d => periodIds.Contains(d.DailyLaborerId)).Select(x => x.DailyLaborerId);
        //                var dailyLabourEmployee = db.DailyLaborerEmployees.Where(x => dailyLaborIds.Contains(x.DailyLaborerId));
        //                data.Add(new SalarySummaryReportVM
        //                {
        //                    Status = status,
        //                    NumberOfEmployee = dailyLabourEmployee.Count(),
        //                    SumOfGrossSalary =(decimal) dailyLabourEmployee.Sum(x=>x.TotalWage),
        //                    PositionAllowance = 0,
        //                    ResponsibilityAllowance = 0,
        //                    OT = dailyLabourEmployee.Any() ? (decimal)dailyLabourEmployee.Sum(x => x.DayOTWage + x.NightOTWage + x.RestTimeOTWage + x.HolidayOTWage):0
        //                });
        //                break;

        //            default:

        //                    data.Add(new SalarySummaryReportVM
        //                    {
        //                        Status = status,
        //                        NumberOfEmployee = 0,
        //                        SumOfGrossSalary = 0,
        //                        PositionAllowance = 0,
        //                        ResponsibilityAllowance = 0,
        //                        OT = 0
        //                    });

        //        }

        //    }

        //    return data.OrderBy(x => x.Status).ToList();
        //}

        public void Dispose()
        {
            db.Dispose();
        }
    }
}

