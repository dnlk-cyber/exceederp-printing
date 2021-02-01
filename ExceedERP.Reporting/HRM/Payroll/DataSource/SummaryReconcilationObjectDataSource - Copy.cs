using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class SummaryReconcilationObjectDataSource : List<SummaryReconcilation>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SummaryReconcilation> GetPayrollSummaryReconcilation(string branchId, string period, string user)
        {
            List<SummaryReconcilation> list = new List<SummaryReconcilation> { };
            SummaryReconcilation oneSummary;


            string periodOne = "";
            string periodTwo = "";
            int index = 0;
            var currentPeriod = db.GLPeriods.Where(x => x.Name == period).FirstOrDefault();
            if (currentPeriod != null)
            {
                var allPeriods = db.GLPeriods.Where(x => x.DateTo <= currentPeriod.DateTo && x.GLPeriodId != currentPeriod.GLPeriodId)
                    .OrderByDescending(x => x.DateTo)
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
                db.PayrollProcessViewModel.Where(x => x.PeriodName == period).ToList();
            var processOneList =
                db.PayrollProcessViewModel.Where(x => x.PeriodName == periodOne).ToList();

            var processTwoList =
                db.PayrollProcessViewModel.Where(x => x.PeriodName == periodTwo).ToList();
            if (branchId != null)
            {
                List<string> branchs = branchId.Split('-').ToList();
                processList = processList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
                processOneList = processOneList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
                processTwoList = processTwoList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
            }
            List<string> summaryNames = new List<string>
            {
                "No of employee",
                "Previous Month New Employees",
                "Previous Month Terminated Employees",
                "Current Month New Employees",
                "Current Month Terminated employees",
                "Net Payable",
                "TAX",
                "Pesnion",
                "Basic Salary",
                "Allowances",
                "OT",
                "Additions",
                "Deductions",
                "Loan",
                "Advance",
                "Sport Fee",
                "Credit-Saving",
                "Labour Union"
            };
            #region Extract Id
            var selectedCurrentMonth = processList.Select(x => x.PayrollProcessViewModelId).ToList();
            var selectedMonthOne = processOneList.Select(x => x.PayrollProcessViewModelId).ToList();
            var selectedMonthTwo = processTwoList.Select(x => x.PayrollProcessViewModelId).ToList();
            #endregion
            foreach (var s in summaryNames)
            {
                oneSummary = new SummaryReconcilation();

                if (s == "No of employee")
                {
                    oneSummary.Name = s;
                    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Count());
                    oneSummary.MonthOne = Convert.ToDecimal(processOneList.Count());
                    oneSummary.PeriodOne = periodOne;
                    oneSummary.PeriodTwo = periodTwo;
                    oneSummary.CurrentPeriod = period;
                    oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Count());

                    if (periodTwo != "")
                    {
                        oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    }
                    else
                    {
                        oneSummary.DiffOne = oneSummary.MonthOne;
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                    }
                    list.Add(oneSummary);
                }
                if (s == "Previous Month New Employees")
                {
                    oneSummary.Name = s;
                    // oneSummary.CurrentMonth = Convert.ToDecimal(processList.Count());
                    //oneSummary.MonthOne = Convert.ToDecimal(processOneList.Count());
                    //oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Count());
                    //oneSummary.DiffOne =oneSummary.MonthTwo - Math.Abs( oneSummary.MonthOne);
                    //oneSummary.DiffTwo =oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    list.Add(oneSummary);
                }
                if (s == "Previous Month Terminated Employees")
                {
                    oneSummary.Name = s;
                    //oneSummary.CurrentMonth = Convert.ToDecimal(processList.Count());
                    //oneSummary.MonthOne = Convert.ToDecimal(processOneList.Count());
                    //oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Count());
                    //oneSummary.DiffOne =oneSummary.MonthTwo - Math.Abs( oneSummary.MonthOne);
                    //oneSummary.DiffTwo =oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    list.Add(oneSummary);
                }
                if (s == "Current Month New Employees")
                {
                    oneSummary.Name = s;
                    //    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Count());
                    //oneSummary.MonthOne = Convert.ToDecimal(processOneList.Count());
                    //oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Count());
                    //oneSummary.DiffOne =oneSummary.MonthTwo - Math.Abs( oneSummary.MonthOne);
                    //oneSummary.DiffTwo =oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    list.Add(oneSummary);
                }
                if (s == "Current Month Terminated employees")
                {
                    oneSummary.Name = s;
                    //    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Count());
                    //oneSummary.MonthOne = Convert.ToDecimal(processOneList.Count());
                    //oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Count());
                    //oneSummary.DiffOne =oneSummary.MonthTwo - Math.Abs( oneSummary.MonthOne);
                    //oneSummary.DiffTwo =oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    list.Add(oneSummary);
                }
                if (s == "Net Payable")
                {
                    oneSummary.Name = s;
                    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.NetEarning));
                    oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.NetEarning));
                    oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.NetEarning));
                    if (periodTwo != "")
                    {
                        oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    }
                    else
                    {
                        oneSummary.DiffOne = oneSummary.MonthOne;
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                    }

                    oneSummary.PeriodOne = periodOne;
                    oneSummary.PeriodTwo = periodTwo;
                    oneSummary.CurrentPeriod = period;
                    list.Add(oneSummary);
                }
                if (s == "TAX")
                {
                    oneSummary.Name = s;
                    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.IncomeTax));

                    oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.IncomeTax));
                    oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.IncomeTax));
                    if (periodTwo != "")
                    {
                        oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    }
                    else
                    {
                        oneSummary.DiffOne = oneSummary.MonthOne;
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                    }
                    oneSummary.PeriodOne = periodOne;
                    oneSummary.PeriodTwo = periodTwo;
                    oneSummary.CurrentPeriod = period;
                    list.Add(oneSummary);
                }
                if (s == "Pesnion")
                {
                    oneSummary.Name = s;
                    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalPension));

                    oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalPension));
                    oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalPension));
                    if (periodTwo != "")
                    {
                        oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    }
                    else
                    {
                        oneSummary.DiffOne = oneSummary.MonthOne;
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                    }
                    oneSummary.PeriodOne = periodOne;
                    oneSummary.PeriodTwo = periodTwo;
                    oneSummary.CurrentPeriod = period;
                    list.Add(oneSummary);
                }
                if (s == "Basic Salary")
                {
                    oneSummary.Name = s;
                    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.BasicSalary));

                    oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.BasicSalary));
                    oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.BasicSalary));
                    if (periodTwo != "")
                    {
                        oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    }
                    else
                    {
                        oneSummary.DiffOne = oneSummary.MonthOne;
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                    }
                    oneSummary.PeriodOne = periodOne;
                    oneSummary.PeriodTwo = periodTwo;
                    oneSummary.CurrentPeriod = period;
                    list.Add(oneSummary);
                }
                if (s == "Allowances")
                {
                    var allowances = Enum.GetValues(typeof(AllowanceType))
                        .Cast<AllowanceType>()
                        .ToList();
                    foreach (var a in allowances)
                    {
                        oneSummary = new SummaryReconcilation();
                        switch (a)
                        {
                            case AllowanceType.TRANSPORT:
                                oneSummary.Name = "Transport Allowance";
                                oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalTransportAllowance));
                                oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalTransportAllowance));
                                oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalTransportAllowance));
                                if (periodTwo != "")
                                {
                                    oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                                }
                                else
                                {
                                    oneSummary.DiffOne = oneSummary.MonthOne;
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                                }
                                oneSummary.PeriodOne = periodOne;
                                oneSummary.PeriodTwo = periodTwo;
                                oneSummary.CurrentPeriod = period;
                                list.Add(oneSummary);
                                break;
                            case AllowanceType.HOME:
                                oneSummary.Name = "House Allowance";
                                oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalHouseAllowance));
                                oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalHouseAllowance));
                                oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalHouseAllowance));
                                if (periodTwo != "")
                                {
                                    oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                                }
                                else
                                {
                                    oneSummary.DiffOne = oneSummary.MonthOne;
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                                }
                                oneSummary.PeriodOne = periodOne;
                                oneSummary.PeriodTwo = periodTwo;
                                oneSummary.CurrentPeriod = period;
                                list.Add(oneSummary);
                                break;
                            case AllowanceType.TELEPHONE:
                                oneSummary.Name = "Telephone Allowance";
                                oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalTelephoneAllowance));
                                oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalTelephoneAllowance));
                                oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalTelephoneAllowance));
                                if (periodTwo != "")
                                {
                                    oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                                }
                                else
                                {
                                    oneSummary.DiffOne = oneSummary.MonthOne;
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                                }
                                oneSummary.PeriodOne = periodOne;
                                oneSummary.PeriodTwo = periodTwo;
                                oneSummary.CurrentPeriod = period;
                                list.Add(oneSummary);
                                break;
                            case AllowanceType.POSITION:
                                oneSummary.Name = "Postion Allowance";
                                oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalPositionAllowance));
                                oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalPositionAllowance));
                                oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalPositionAllowance));
                                if (periodTwo != "")
                                {
                                    oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                                }
                                else
                                {
                                    oneSummary.DiffOne = oneSummary.MonthOne;
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                                }
                                oneSummary.PeriodOne = periodOne;
                                oneSummary.PeriodTwo = periodTwo;
                                oneSummary.CurrentPeriod = period;
                                list.Add(oneSummary);
                                break;
                            case AllowanceType.PROFESSIONAL:
                                oneSummary.Name = "Professional Allowance";
                                oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalProfessionalAllowance));
                                oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalProfessionalAllowance));
                                oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalProfessionalAllowance));
                                if (periodTwo != "")
                                {
                                    oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                                }
                                else
                                {
                                    oneSummary.DiffOne = oneSummary.MonthOne;
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                                }
                                oneSummary.PeriodOne = periodOne;
                                oneSummary.PeriodTwo = periodTwo;
                                oneSummary.CurrentPeriod = period;
                                list.Add(oneSummary);
                                break;
                            case AllowanceType.RESPONSIBILITY:
                                oneSummary.Name = "Responsibility Allowance";
                                oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalResponsiblityAllowance));
                                oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalResponsiblityAllowance));
                                oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalResponsiblityAllowance));
                                if (periodTwo != "")
                                {
                                    oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                                }
                                else
                                {
                                    oneSummary.DiffOne = oneSummary.MonthOne;
                                    oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                                }
                                oneSummary.PeriodOne = periodOne;
                                oneSummary.PeriodTwo = periodTwo;
                                oneSummary.CurrentPeriod = period;
                                list.Add(oneSummary);
                                break;
                            case AllowanceType.PROJECT:
                                oneSummary.Name = "Project Allowance";
                                oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalProjectAllowance));
                                oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalProjectAllowance));
                                oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalProjectAllowance));
                                oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                                oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                                oneSummary.PeriodOne = periodOne;
                                oneSummary.PeriodTwo = periodTwo;
                                oneSummary.CurrentPeriod = period;
                                list.Add(oneSummary);
                                break;
                            case AllowanceType.DESERT:
                                oneSummary.Name = "Desert Allowance";
                                oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalDesertAllowance));
                                oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalDesertAllowance));
                                oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalDesertAllowance));
                                oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                                oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                                oneSummary.PeriodOne = periodOne;
                                oneSummary.PeriodTwo = periodTwo;
                                oneSummary.CurrentPeriod = period;
                                list.Add(oneSummary);
                                break;
                        }

                    }

                }
                if (s == "OT")
                {
                    oneSummary.Name = s;
                    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalOvertime));

                    oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalOvertime));
                    oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalOvertime));
                    if (periodTwo != "")
                    {
                        oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    }
                    else
                    {
                        oneSummary.DiffOne = oneSummary.MonthOne;
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                    }
                    oneSummary.PeriodOne = periodOne;
                    oneSummary.PeriodTwo = periodTwo;
                    oneSummary.CurrentPeriod = period;
                    list.Add(oneSummary);
                }
                if (s == "Additions")
                {

                    var additionTypes =
                        db.Lookups.Where(x => x.LookupType == LookUpTypes.ADDITION_TYPE).ToList();

                    var allAddition = from c in db.Lookups
                        join f in db.Additions
                            on c.LookupId equals f.LookupId
                        join p in db.PayrollProcessAdditions
                            on f.LookupId equals p.AdditionId
                        where c.LookupType == LookUpTypes.ADDITION_TYPE
                        select
                            (new { p, f });
                    var allAdditionList = allAddition.ToList();

                    foreach (var add in additionTypes)
                    {
                        var currentFixed = allAdditionList.Where(x => selectedCurrentMonth.Contains(x.p.PayrollProcessViewModelId) && x.f.LookupId == add.LookupId).ToList();
                        var monthOneFixed = allAdditionList.Where(x => selectedMonthOne.Contains(x.p.PayrollProcessViewModelId) && x.f.LookupId == add.LookupId).ToList();
                        var monthOneTwo = allAdditionList.Where(x => selectedMonthTwo.Contains(x.p.PayrollProcessViewModelId) && x.f.LookupId == add.LookupId).ToList();
                        oneSummary = new SummaryReconcilation();
                        oneSummary.Name = add.Description;
                        oneSummary.CurrentMonth = Convert.ToDecimal(currentFixed.Sum(x => x.p.Amount));
                        oneSummary.MonthOne = Convert.ToDecimal(monthOneFixed.Sum(x => x.p.Amount));
                        oneSummary.MonthTwo = Convert.ToDecimal(monthOneTwo.Sum(x => x.p.Amount));
                        if (periodTwo != "")
                        {
                            oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                            oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                        }
                        else
                        {
                            oneSummary.DiffOne = oneSummary.MonthOne;
                            oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                        }
                        oneSummary.PeriodOne = periodOne;
                        oneSummary.PeriodTwo = periodTwo;
                        oneSummary.CurrentPeriod = period;
                        if (oneSummary.CurrentMonth != 0 || oneSummary.MonthOne != 0 || oneSummary.MonthTwo != 0)
                        {
                            list.Add(oneSummary);
                        }
                    }


                }
                if (s == "Deductions")
                {

                    #region FixedContribution

                    var fixedTypes =
                        db.ContributionTypes.Where(x => x.ContributionCategory == ContributionCategory.FIXED).ToList();

                    var allfixed = from c in db.ContributionTypes
                        join f in db.FixedContributions
                            on c.ContributionTypeId equals f.ContributionTypeId
                        join p in db.PayrollProcessFixedContribution
                            on f.FixedContributionId equals p.FixedContributionId
                        where c.ContributionCategory == ContributionCategory.FIXED
                        select
                            (new { p, f });
                    var allFixedList = allfixed.ToList();

                    foreach (var fixedt in fixedTypes)
                    {
                        var currentFixed = allFixedList.Where(x => selectedCurrentMonth.Contains(x.p.PayrollProcessViewModelId) && x.f.ContributionTypeId == fixedt.ContributionTypeId).ToList();
                        var monthOneFixed = allFixedList.Where(x => selectedMonthOne.Contains(x.p.PayrollProcessViewModelId) && x.f.ContributionTypeId == fixedt.ContributionTypeId).ToList();
                        var monthOneTwo = allFixedList.Where(x => selectedMonthTwo.Contains(x.p.PayrollProcessViewModelId) && x.f.ContributionTypeId == fixedt.ContributionTypeId).ToList();
                        oneSummary = new SummaryReconcilation();
                        oneSummary.Name = fixedt.Description;
                        oneSummary.CurrentMonth = Convert.ToDecimal(currentFixed.Sum(x => x.p.Amount));
                        oneSummary.MonthOne = Convert.ToDecimal(monthOneFixed.Sum(x => x.p.Amount));
                        oneSummary.MonthTwo = Convert.ToDecimal(monthOneTwo.Sum(x => x.p.Amount));
                        if (periodTwo != "")
                        {
                            oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                            oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                        }
                        else
                        {
                            oneSummary.DiffOne = oneSummary.MonthOne;
                            oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                        }
                        oneSummary.PeriodOne = periodOne;
                        oneSummary.PeriodTwo = periodTwo;
                        oneSummary.CurrentPeriod = period;
                        if (oneSummary.CurrentMonth != 0 || oneSummary.MonthOne != 0 || oneSummary.MonthTwo != 0)
                        {
                            list.Add(oneSummary);
                        }
                    }
                    #endregion
                    #region Regular Contribution

                    var regularTypes =
                        db.ContributionTypes.Where(x => x.ContributionCategory == ContributionCategory.REGULAR).ToList();

                    var allregular = from c in db.ContributionTypes
                        join f in db.RegularContributions
                            on c.ContributionTypeId equals f.ContributionTypeId
                        join p in db.PayrollProcessRegularContribution
                            on f.RegContributionId equals p.RegContributionId
                        where c.ContributionCategory == ContributionCategory.REGULAR
                        select
                            (new { p, f });
                    var allRegularList = allregular.ToList();

                    foreach (var fixedt in regularTypes)
                    {
                        var currentFixed = allRegularList.Where(x => selectedCurrentMonth.Contains(x.p.PayrollProcessViewModelId) && x.f.ContributionTypeId == fixedt.ContributionTypeId).ToList();
                        var monthOneFixed = allRegularList.Where(x => selectedMonthOne.Contains(x.p.PayrollProcessViewModelId) && x.f.ContributionTypeId == fixedt.ContributionTypeId).ToList();
                        var monthOneTwo = allRegularList.Where(x => selectedMonthTwo.Contains(x.p.PayrollProcessViewModelId) && x.f.ContributionTypeId == fixedt.ContributionTypeId).ToList();
                        oneSummary = new SummaryReconcilation();
                        oneSummary.Name = fixedt.Description;
                        oneSummary.CurrentMonth = Convert.ToDecimal(currentFixed.Sum(x => x.p.Amount));
                        oneSummary.MonthOne = Convert.ToDecimal(monthOneFixed.Sum(x => x.p.Amount));
                        oneSummary.MonthTwo = Convert.ToDecimal(monthOneTwo.Sum(x => x.p.Amount));
                        if (periodTwo != "")
                        {
                            oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                            oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                        }
                        else
                        {
                            oneSummary.DiffOne = oneSummary.MonthOne;
                            oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                        }
                        oneSummary.PeriodOne = periodOne;
                        oneSummary.PeriodTwo = periodTwo;
                        oneSummary.CurrentPeriod = period;
                        if (oneSummary.CurrentMonth != 0 || oneSummary.MonthOne != 0 || oneSummary.MonthTwo != 0)
                        {
                            list.Add(oneSummary);
                        }
                    }
                    #endregion

                }
                if (s == "Loan")
                {
                    oneSummary.Name = s;
                    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalLoan));

                    oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalLoan));
                    oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalLoan));
                    if (periodTwo != "")
                    {
                        oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    }
                    else
                    {
                        oneSummary.DiffOne = oneSummary.MonthOne;
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                    }
                    oneSummary.PeriodOne = periodOne;
                    oneSummary.PeriodTwo = periodTwo;
                    oneSummary.CurrentPeriod = period;
                    list.Add(oneSummary);
                }
                if (s == "Advance")
                {
                    oneSummary.Name = s;
                    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.TotalAdvance));

                    oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.TotalAdvance));
                    oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.TotalAdvance));
                    if (periodTwo != "")
                    {
                        oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    }
                    else
                    {
                        oneSummary.DiffOne = oneSummary.MonthOne;
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                    }
                    oneSummary.PeriodOne = periodOne;
                    oneSummary.PeriodTwo = periodTwo;
                    oneSummary.CurrentPeriod = period;
                    list.Add(oneSummary);
                }
                if (s == "Sport Fee")
                {
                    oneSummary.Name = s;
                    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.SportFee));

                    oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.SportFee));
                    oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.SportFee));
                    if (periodTwo != "")
                    {
                        oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    }
                    else
                    {
                        oneSummary.DiffOne = oneSummary.MonthOne;
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                    }
                    oneSummary.PeriodOne = periodOne;
                    oneSummary.PeriodTwo = periodTwo;
                    oneSummary.CurrentPeriod = period;
                    list.Add(oneSummary);
                }
                if (s == "Credit-Saving")
                {
                    oneSummary.Name = s;

                    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.AssocationContribution));
                    oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.AssocationContribution));
                    oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.AssocationContribution));
                    if (periodTwo != "")
                    {
                        oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    }
                    else
                    {
                        oneSummary.DiffOne = oneSummary.MonthOne;
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                    }
                    oneSummary.PeriodOne = periodOne;
                    oneSummary.PeriodTwo = periodTwo;
                    oneSummary.CurrentPeriod = period;
                    list.Add(oneSummary);
                }
                if (s == "Labour Union")
                {
                    oneSummary.Name = s;
                    oneSummary.CurrentMonth = Convert.ToDecimal(processList.Sum(x => x.LabourUnionContribution));

                    oneSummary.MonthOne = Convert.ToDecimal(processOneList.Sum(x => x.LabourUnionContribution));
                    oneSummary.MonthTwo = Convert.ToDecimal(processTwoList.Sum(x => x.LabourUnionContribution));
                    if (periodTwo != "")
                    {
                        oneSummary.DiffOne = oneSummary.MonthTwo - Math.Abs(oneSummary.MonthOne);
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - Math.Abs(oneSummary.MonthTwo);
                    }
                    else
                    {
                        oneSummary.DiffOne = oneSummary.MonthOne;
                        oneSummary.DiffTwo = oneSummary.CurrentMonth - oneSummary.MonthOne;
                    }
                    oneSummary.PeriodOne = periodOne;
                    oneSummary.PeriodTwo = periodTwo;
                    oneSummary.CurrentPeriod = period;
                    list.Add(oneSummary);
                }
            }
            return list;


        }
    }
}