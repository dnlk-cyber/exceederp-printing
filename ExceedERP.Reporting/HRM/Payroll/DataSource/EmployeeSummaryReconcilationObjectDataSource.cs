using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class EmployeeSummaryReconcilationObjectDataSource : List<SummaryReconcilation>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SummaryReconcilation> GetPayrollEmployeeSummaryReconcilation(string branchId, string period)
        {
            List<SummaryReconcilation> list = new List<SummaryReconcilation> { };
            SummaryReconcilation oneSummary;


            string periodOne = "";
            string periodTwo = "";
            int periodOneId;
            int periodTwoId;
            string periodThree = "";
            int index = 0;
            var currentPeriod = db.GLPeriods.Where(x => x.Name == period).FirstOrDefault();
            if (currentPeriod != null)
            {
                var allPeriods =
                    db.GLPeriods.Where(
                            x => x.DateTo <= currentPeriod.DateTo && x.GLPeriodId != currentPeriod.GLPeriodId)
                        .OrderBy(x => x.DateTo)
                        .ToList();
                if (allPeriods.Count() > 1)
                {
                    periodOne = allPeriods[1].Name;
                    periodTwo = allPeriods[0].Name;
                }

                else if (allPeriods.Count() == 1)
                {
                    periodOne = allPeriods[0].Name;

                }

                else
                {
                    periodOne = "";
                    periodTwo = "";
                }
            }


            var processList =
                db.PayrollProcessViewModel.Where(x => x.PeriodName == period)

                    .ToList();
            if (branchId != null)
            {
                List<string> branchs = branchId.Split('-').ToList();
                processList = db.PayrollProcessViewModel.Where(x => branchs.Contains(x.BranchId.ToString()) && x.PeriodName == period)

                    .ToList();
            }
            List<string> summaryNames = new List<string>
            {
                "Previous Month New Employees",
                "Previous Month Terminated Employees",
                "Current Month New Employees",
                "Current Month Terminated Employees"
            };
            var processOneList = db.PayrollProcessViewModel.Where(x => x.PeriodName == periodOne).ToList();

            if (branchId != null)
            {
                List<string> branchs = branchId.Split('-').ToList();
                processOneList = db.PayrollProcessViewModel.Where(x => branchs.Contains(x.BranchId.ToString()) && x.PeriodName == periodOne).ToList();

            }
            var processTwoList = db.PayrollProcessViewModel.Where(x => x.PeriodName == periodTwo).ToList();
            if (branchId != null)
            {
                List<string> branchs = branchId.Split('-').ToList();
                processTwoList = db.PayrollProcessViewModel.Where(x => branchs.Contains(x.BranchId.ToString()) && x.PeriodName == periodTwo).ToList();

            }
            var processThreeList = db.PayrollProcessViewModel.Where(x => x.PeriodName == periodThree).ToList();
            if (branchId != null)
            {
                List<string> branchs = branchId.Split('-').ToList();
                processThreeList = db.PayrollProcessViewModel.Where(x => branchs.Contains(x.BranchId.ToString()) && x.PeriodName == periodThree).ToList();

            }
            foreach (var s in summaryNames)
            {
                oneSummary = new SummaryReconcilation();

                int diffrence = 0;
                if (s == "Previous Month New Employees")
                {



                    if (periodTwo == "")
                    {
                        var newPlacment = from t in db.Placements
                            join p in db.PayrollProcessViewModel
                                on t.EmployeeId equals p.EmployeeId
                            join g in db.GLPeriods
                                on p.GLPeriodId equals g.GLPeriodId
                            where t.AssignmentDate >= g.DateFrom && t.AssignmentDate <= g.DateTo && t.AssignmentType == AssignmentType.NEW && p.PeriodName == periodOne
                            select (new { t.EmployeeId });
                        if (newPlacment.ToList().Any())
                        {
                            List<int> xs = newPlacment.Select(x => x.EmployeeId).ToList();
                            diffrence = xs.Count;
                        }


                    }
                    else
                    {

                        var newPlacment = from t in db.Placements
                            join p in db.PayrollProcessViewModel
                                on t.EmployeeId equals p.EmployeeId
                            join g in db.GLPeriods
                                on p.GLPeriodId equals g.GLPeriodId
                            where t.AssignmentDate >= g.DateFrom && t.AssignmentDate <= g.DateTo && t.AssignmentType == AssignmentType.NEW && p.PeriodName == periodTwo
                            select (new { t.EmployeeId });

                        if (newPlacment.ToList().Any())
                        {
                            List<int> xs = newPlacment.Select(x => x.EmployeeId).ToList();
                            diffrence = xs.Count;
                        }


                    }
                    oneSummary.Name = s;
                    oneSummary.MonthOne = diffrence;
                    list.Add(oneSummary);
                }
                if (s == "Previous Month Terminated Employees")
                {
                    if (periodTwo == "")
                    {
                        var termination = from t in db.Terminations
                            join p in db.PayrollProcessViewModel
                                on t.EmployeeId equals p.EmployeeId
                            join g in db.GLPeriods
                                on p.GLPeriodId equals g.GLPeriodId
                            where t.TerminationDate >= g.DateFrom && t.TerminationDate <= g.DateTo && p.PeriodName == periodOne
                            select (new { t.EmployeeId });

                        var xs = termination.ToList();
                        diffrence = xs.Count;

                    }
                    else
                    {

                        var termination = from t in db.Terminations
                            join p in db.PayrollProcessViewModel
                                on t.EmployeeId equals p.EmployeeId
                            join g in db.GLPeriods
                                on p.GLPeriodId equals g.GLPeriodId
                            where t.TerminationDate >= g.DateFrom && t.TerminationDate <= g.DateTo && p.PeriodName == periodTwo
                            select (new { t.EmployeeId });

                        var xs = termination.ToList();
                        diffrence = xs.Count;


                    }

                    oneSummary.MonthOne = diffrence;
                    oneSummary.Name = s;
                    list.Add(oneSummary);
                }
                if (s == "Current Month New Employees")
                {

                    var newPlacment = from t in db.Placements
                        join p in db.PayrollProcessViewModel
                            on t.EmployeeId equals p.EmployeeId
                        join g in db.GLPeriods
                            on p.GLPeriodId equals g.GLPeriodId
                        where t.AssignmentDate >= g.DateFrom && t.AssignmentDate <= g.DateTo && t.AssignmentType == AssignmentType.NEW && p.PeriodName == period
                        select (new { t.EmployeeId });

                    var xs = newPlacment.ToList();
                    diffrence = xs.Count;
                    oneSummary.Name = s;
                    oneSummary.MonthOne = diffrence;
                    list.Add(oneSummary);
                }
                if (s == "Current Month Terminated Employees")
                {

                    var termination = from t in db.Terminations
                        join p in db.PayrollProcessViewModel
                            on t.EmployeeId equals p.EmployeeId
                        join g in db.GLPeriods
                            on p.GLPeriodId equals g.GLPeriodId
                        where t.TerminationDate >= g.DateFrom && t.TerminationDate <= g.DateTo && p.PeriodName == period
                        select (new { t.EmployeeId });

                    var xs = termination.ToList();
                    diffrence = xs.Count;


                    oneSummary.MonthOne = diffrence;
                    oneSummary.Name = s;
                    list.Add(oneSummary);
                }

            }


            return list;
        }
    }
}