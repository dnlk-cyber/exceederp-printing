using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Web;
using AllowanceType = ExceedERP.Core.Domain.Common.AllowanceType;

namespace ExceedERP.Helpers.Finance
{


    public class PayrollHelper
    {
        ExceedDbContext db ;
        public PayrollHelper()
        {
             db = new ExceedDbContext();

        }
        public decimal GetemployeeDaysAbsent(int EmpId, int period)
        {            
            decimal days = 0;

            var absentdays = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == EmpId)
            .Where(x => x.GLPeriodId == period).ToList();
            if (absentdays.Any())
                days = absentdays.Select(x => x.Days).FirstOrDefault();
            if (days > 30)
                days = 30;
            return days;
        }
        public decimal GetemployeePaidLeaveDays(int EmpId, int period)
        {
            
            decimal days = 0;

            var periodObj = db.GLPeriods.Where(x => x.GLPeriodId == period)
                           .FirstOrDefault();

            var employeePaidLeaves = db.Leaves
                .Where(x => x.LeaveType == LeaveType.ANNUAL)
                .Where(x => x.EmployeeId == EmpId)
                .Where(x => (x.From >= periodObj.DateFrom && x.From <= periodObj.DateTo) || (x.To >= periodObj.DateFrom && x.To <= periodObj.DateTo)).ToList();

            foreach (var unpaid in employeePaidLeaves)
            {
                if (unpaid.From < periodObj.DateFrom)
                {
                    if (unpaid.To < periodObj.DateTo)
                        days = days + (decimal)(unpaid.To - periodObj.DateFrom).TotalDays + 1;
                    else
                        days = days + (decimal)(periodObj.DateTo - periodObj.DateFrom).TotalDays + 1;
                }
                else
                {
                    if (unpaid.To < periodObj.DateTo)
                        days = days + (decimal)(unpaid.To - unpaid.From).TotalDays + 1;
                    else
                        days = days + (decimal)(periodObj.DateTo - unpaid.From).TotalDays + 1;
                }

            }


            //later to consider holiday and weekend
            //later consider half days

            if (days > 30)
                days = 30;
            return days;
        }
        public decimal GetemployeeUnpaidLeaveDays(int EmpId, int period)
        {
            
            decimal days = 0;

            var periodObj = db.GLPeriods.Where(x => x.GLPeriodId == period)
                           .FirstOrDefault();

            var employeeUnpaidLeaves = db.Leaves
                .Where(x => x.LeaveType == LeaveType.Leave_With_Out_Pay)
                .Where(x => x.EmployeeId == EmpId)
                .Where(x => (x.From >= periodObj.DateFrom && x.From <= periodObj.DateTo) || (x.To >= periodObj.DateFrom && x.To <= periodObj.DateTo)).ToList();

            foreach (var unpaid in employeeUnpaidLeaves)
            {
                if (unpaid.From < periodObj.DateFrom)
                {
                    if (unpaid.To < periodObj.DateTo)
                        days = days + (decimal)(unpaid.To - periodObj.DateFrom).TotalDays;
                    else
                        days = days + (decimal)(periodObj.DateTo - periodObj.DateFrom).TotalDays;
                }
                else
                {
                    if (unpaid.To < periodObj.DateTo)
                        days = days + (decimal)(unpaid.To - unpaid.From).TotalDays;
                    else
                        days = days + (decimal)(periodObj.DateTo - unpaid.From).TotalDays;
                }

            }

            if (days > 30)
                days = 30;
            return days;
        }
        public  decimal GetDelegatorBaseSalary(int empid)
        {
            
            decimal salary = 0;
            var delegatorAssignment = db.Placements.Where(x => x.EmployeeId == empid && x.isCurrent).FirstOrDefault();
            if (delegatorAssignment != null)
            {
                var rank=db.Ranks.Find(delegatorAssignment.RankId);
                if(rank!=null)
                   salary= rank.StartingSalary;
            }
            return salary;
        }
        public  decimal GetTaxablePay(int empid,decimal desertPercentage, GLPeriod findPeriod,decimal oTCoefficient,decimal restDayRate,decimal NormalRate, decimal NightRate, decimal HolidayRate)
        {
            
            decimal taxablePayBeforCamp = 0;
            decimal salary = 0;
            var assignment =
                db.Placements.Where(x => x.EmployeeId == empid && x.isCurrent).FirstOrDefault();
            if (assignment != null)
            {
                salary = assignment.Salary;
            }

            #region Allowance Treashhold

            var allowanceTypeRule =
            db.AllowanceTypeRule.ToList();

            #endregion
            #region allowances

            string settingvalue = "NON_TAXABLE";
            var isTaxableSetting =
                db.Configurations.Where(
                    x => x.Category == "Payroll" && x.Reference == "BENEFIT_ZERO_THRESHOLD")
                    .FirstOrDefault();
            if (isTaxableSetting != null)
            {
                settingvalue = isTaxableSetting.Attribute;
            }

            List<AllowancePerEmployee> allowances =
                db.AllowancePerEmployees.Where(x => x.EmployeeId ==empid && x.Pend == false)
                    .ToList();

            
            decimal totalAllowance = 0;
            decimal totalTaxableAllowance = 0;
            decimal totalNonTaxableAllowance = 0;
            //decimal houseallowanceSalaryThreshold = 0;
            AllowanceDefination allowancemodel;
            PayrollProcessAllowances allowancedata;
            decimal Threshold = 0;
            foreach (AllowancePerEmployee allowance in allowances)
            {
                allowancemodel = new AllowanceDefination();
                allowancemodel =
                    db.AllowanceDefinations.Where(
                        x => x.AllowanceDefinationId == allowance.AllowanceDefinationId)
                        .AsNoTracking()
                        .FirstOrDefault();

                allowancedata = new PayrollProcessAllowances();

                allowancedata.Name = allowancemodel.AllowanceName;

                allowancedata.AllowanceType = allowancemodel.AllowanceType;


                var rule =
                    allowanceTypeRule.Where(a => a.AllowanceType == allowancemodel.AllowanceType)
                        .FirstOrDefault();

               
                if (allowancemodel.AmtPercent == FixedContPecntType.PERCENT)
                {
                    //contribution from camping benefit
                    if (allowancemodel.AllowanceType == AllowanceType.DESERT)
                        desertPercentage = allowancemodel.Amount;
                    //working original
                    allowancemodel.Amount = salary * (allowancemodel.Amount / 100);

                }

             
                if (allowancemodel.AllowanceType == AllowanceType.DESERT)
                {
                    #region deductable amount from desert allowance
                    //var days = db.PayrollEmployeeDesertDays.Where(x => x.EmployeeId ==empid)
                    //        .Where(x => x.GLPeriodId == findPeriod.GLPeriodId).FirstOrDefault();
                    //if (days != null)
                    //{
                    //    decimal deductableDays = 30 - days.Days;
                    //    if (deductableDays > 30)
                    //        deductableDays = 30;

                    //    decimal dedectedAllowance = (allowancemodel.Amount / 30) * deductableDays;
                    //    allowancemodel.Amount = allowancemodel.Amount - dedectedAllowance;
                    //}
                    #endregion
                }

                if (allowancemodel.AllowanceType == AllowanceType.PROFESSIONAL)
                {
                  

                }
                if (allowancemodel.AllowanceType == AllowanceType.POSITION)
                {

                    decimal totalDelagatin = 0;

                    var checkDelegation =
                          db.Delegations.Where(
                                  x =>
                                      x.FromEmployeeId == allowance.EmployeeId &&
                                      x.FromDateTime >= findPeriod.DateFrom &&
                                      x.FromDateTime <= findPeriod.DateTo)
                              .FirstOrDefault();

                    if (checkDelegation != null)
                    {
                        //if no end date ,take up to end of the period
                        DateTime delegationEndDate = checkDelegation.ToDateTime != null ? (DateTime)checkDelegation.ToDateTime : findPeriod.DateTo;

                        if (findPeriod.DateTo < checkDelegation.FromDateTime)
                        {

                        }
                        else if (delegationEndDate < findPeriod.DateFrom)
                        {

                        }
                        else
                        {
                            if (findPeriod.DateTo > delegationEndDate &&
                                checkDelegation.FromDateTime > findPeriod.DateFrom)
                            {
                                var dif =
                                    (checkDelegation.FromDateTime - delegationEndDate);
                                double dateDiff =
                                    dif.HasValue ? dif.Value.TotalDays : 0;
                                if (dateDiff >= 30)
                                {
                                    totalDelagatin = Convert.ToDecimal(dateDiff);
                                }
                            }
                            else if (delegationEndDate > findPeriod.DateTo &&
                                     checkDelegation.FromDateTime > findPeriod.DateFrom)
                            {
                                var dif =
                                    (findPeriod.DateTo - checkDelegation.FromDateTime);
                                double dateDiff =
                                    dif.HasValue ? dif.Value.TotalDays : 0;
                                if (dateDiff >= 30)
                                {
                                    totalDelagatin = Convert.ToDecimal(dateDiff);
                                }
                            }
                            else if (findPeriod.DateTo < checkDelegation.FromDateTime)
                            {
                                var dif =
                                    (findPeriod.DateTo - checkDelegation.FromDateTime);
                                double dateDiff =
                                    dif.HasValue ? dif.Value.TotalDays : 0;
                                if (dateDiff >= 30)
                                {
                                    totalDelagatin = Convert.ToDecimal(dateDiff);
                                }
                            }
                        }
                    }
                    allowancemodel.Amount = allowancemodel.Amount -
                                            (allowancemodel.Amount / 30) * totalDelagatin;

                }

                if (allowancemodel.AllowanceType == AllowanceType.POSITION)
                {
                    decimal totalDelagatin = 0;

                    var checkDelegation =
                         db.Delegations.Where(
                                 x =>
                                     x.FromEmployeeId == allowance.EmployeeId &&
                                     x.FromDateTime >= findPeriod.DateFrom &&
                                     x.FromDateTime <= findPeriod.DateTo)
                             .FirstOrDefault();

                    if (checkDelegation != null)
                    {
                        //if no end date ,take up to end of the period
                        DateTime delegationEndDate = checkDelegation.ToDateTime != null ? (DateTime)checkDelegation.ToDateTime : findPeriod.DateTo;
                        if (findPeriod.DateTo < checkDelegation.FromDateTime)
                        {

                        }
                        else if (delegationEndDate < findPeriod.DateFrom)
                        {

                        }
                        else
                        {
                            if (findPeriod.DateTo > delegationEndDate &&
                                checkDelegation.FromDateTime > findPeriod.DateFrom)
                            {
                                var dif =
                                    (checkDelegation.FromDateTime - delegationEndDate);
                                double dateDiff =
                                   dif.HasValue ? dif.Value.TotalDays : 0;
                                if (dateDiff >= 30)
                                {
                                    totalDelagatin = Convert.ToDecimal(dateDiff);
                                }
                            }
                            else if (delegationEndDate > findPeriod.DateTo &&
                                     checkDelegation.FromDateTime > findPeriod.DateFrom)
                            {
                                var dif =
                                    (findPeriod.DateTo - checkDelegation.FromDateTime);
                                double dateDiff =
                                    dif.HasValue ? dif.Value.TotalDays : 0;
                                if (dateDiff >= 30)
                                {
                                    totalDelagatin = Convert.ToDecimal(dateDiff);
                                }
                            }
                            else if (findPeriod.DateTo < checkDelegation.FromDateTime)
                            {
                                var dif =
                                    (findPeriod.DateTo - checkDelegation.FromDateTime);
                                double dateDiff =
                                    dif.HasValue ? dif.Value.TotalDays : 0;
                                if (dateDiff >= 30)
                                {
                                    totalDelagatin = Convert.ToDecimal(dateDiff);
                                }
                            }
                        }
                        allowancemodel.Amount = allowancemodel.Amount +
                                                (allowancemodel.Amount / 30) * totalDelagatin;

                    }
                }
                Threshold = ((decimal)(rule.Percentage / 100) * salary);

                #region if both thresholds are 0

                if (rule.TaxTrasholdAmount == 0 && rule.Percentage == 0)
                {
                    if (settingvalue == "NON_TAXABLE")
                    {
                        allowancedata.NonTaxable = Convert.ToDecimal(allowancemodel.Amount);
                        allowancedata.Taxable = 0;
                        allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                        totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                        totalNonTaxableAllowance = totalNonTaxableAllowance + allowancedata.NonTaxable;
                        totalAllowance = totalAllowance + allowancedata.Total;
                    }
                    // new block
                    else
                    {
                        allowancedata.NonTaxable = 0;
                        allowancedata.Taxable = allowancemodel.Amount;
                        allowancedata.Total = allowancemodel.Amount;
                        totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                        totalNonTaxableAllowance = totalNonTaxableAllowance + allowancedata.NonTaxable;
                        totalAllowance = totalAllowance + allowancedata.Total;
                    }
                   
                }
                #endregion
                #region at least  one of them is non zero

                else
                {
                    if (allowancemodel.Amount > Threshold)
                    {
                        if (allowancemodel.Amount > rule.TaxTrasholdAmount)
                        {
                            //new block
                            if (Threshold > rule.TaxTrasholdAmount)
                            {
                                allowancedata.NonTaxable = rule.TaxTrasholdAmount;
                                allowancedata.Taxable = allowancemodel.Amount - rule.TaxTrasholdAmount;
                            }
                            else
                            {
                                allowancedata.NonTaxable = Threshold;
                                allowancedata.Taxable = allowancemodel.Amount - Threshold;
                            }

                            allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                            totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                            totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                       allowancedata.NonTaxable;
                            totalAllowance = totalAllowance + allowancedata.Total;
                        }

                        else
                        {
                            allowancedata.NonTaxable = Threshold;
                            allowancedata.Taxable = Convert.ToDecimal(allowancemodel.Amount) - Threshold;
                            allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                            totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                            totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                       allowancedata.NonTaxable;
                            totalAllowance = totalAllowance + allowancedata.Total;
                        }
                    }
                    else
                    {
                        if (allowancemodel.Amount > rule.TaxTrasholdAmount)
                        {
                            allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                            allowancedata.Taxable = Convert.ToDecimal(allowancemodel.Amount) -
                                                    Convert.ToDecimal(rule.TaxTrasholdAmount);
                            allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                            totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                            totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                       allowancedata.NonTaxable;
                            totalAllowance = totalAllowance + allowancedata.Total;


                        }
                        else
                        {

                            allowancedata.NonTaxable = Convert.ToDecimal(allowancemodel.Amount);
                            allowancedata.Taxable = 0;
                            allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                            totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                            totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                       allowancedata.NonTaxable;
                            totalAllowance = totalAllowance + allowancedata.Total;

                      
                        }

                    }
                  
                }

                #endregion

              
             
               


            }



            #endregion
            #region additions
            PayrollProcessAdditions additiondata;
            //get additions
            var additions = db.Additions.Where(x => x.EmployeeId ==empid)
                .Where(x => x.GLPeriodId == findPeriod.GLPeriodId).ToList();
            decimal totalAdditions = 0;
            decimal totalTaxableAdditions = 0;
            decimal totalNonTaxableAdditions = 0;

            foreach (Addition addition in additions)
            {
                var additionType = db.Lookups.Where(l => l.LookupId == addition.LookupId).FirstOrDefault();
                additiondata = new PayrollProcessAdditions
                {
                    CreatedOn = DateTime.Now.Date,
                    Name = additionType.Description,
                    AdditionId = addition.LookupId,
                    Amount = addition.Amount
                };

                totalAdditions = totalAdditions + additiondata.Amount;

                if (additionType != null && additionType.TaxTresholdAmount > 0 && additionType.Percentage > 0)
                {
                    decimal thresholdAmount = additionType.TaxTresholdAmount;
                    decimal thresholdFromPercent = salary * (additionType.Percentage / 100);
                    if (thresholdAmount > thresholdFromPercent)
                    {
                        if (addition.Amount > thresholdFromPercent)
                        {
                            totalTaxableAdditions += additiondata.Amount - thresholdFromPercent;
                            totalNonTaxableAdditions += thresholdFromPercent;
                        }
                        else
                        {
                           totalTaxableAdditions += 0;
                            totalNonTaxableAdditions += additiondata.Amount;
                        }
                    }
                    else
                    {
                        if (addition.Amount > thresholdAmount)
                        {
                           totalTaxableAdditions += additiondata.Amount - thresholdAmount;
                            totalNonTaxableAdditions += thresholdAmount;
                        }
                        else
                        {
                           totalTaxableAdditions += 0;
                            totalNonTaxableAdditions += additiondata.Amount;
                        }
                    }
                }
                else
                {
                    if (addition.AdditionTaxType == AdditionTaxType.TAXABLE)
                    {
                       totalTaxableAdditions += additiondata.Amount;
                    }
                    else
                    {
                        totalNonTaxableAdditions += additiondata.Amount;
                    }
                }


            }
           

            #endregion
            #region OT

            var overtimes = db.OverTimes.Where(x => x.EmployeeId ==empid)
                .Where(x => x.GLPeriodId == findPeriod.GLPeriodId).ToList();
            decimal totalOT = 0;
            decimal xx = 0;
            decimal yy = 0;
            foreach (OverTime ot in overtimes)
            {
                xx = (salary / oTCoefficient);
                yy = (ot.RestdayHours * restDayRate) + (ot.NormalHours * NormalRate) + (ot.NightHours * NightRate) +
                     (ot.HolidayHours * HolidayRate);
                totalOT = totalOT + (xx * yy);
            }
            #endregion

            taxablePayBeforCamp = salary + totalTaxableAllowance + totalTaxableAdditions + totalOT;
            return taxablePayBeforCamp;
        }
        public List<EmployeeIdView> GetEmployees(int branchid, int businessUnitId, string all = "")
        {
            var employeeIdViews = new List<EmployeeIdView>();

            // Query only active and current employees with necessary fields
            var employeesQuery = from e in db.Person_Employee
                                 join p in db.Placements
                                     on e.EmployeeId equals p.EmployeeId
                                 where e.IsActive && p.isCurrent && !p.IsPend
                                 select new
                                 {
                                     e.EmployeeId,
                                     e.EmployeeCode,
                                     e.IsActive,
                                     e.FirstNameEnglish,
                                     e.MiddleNameEnglish,
                                     e.LastNameEnglish,
                                     e.DateOfBirth,
                                     e.HiredDate,
                                     e.Status,
                                     p.BranchId,
                                     p.BusinessUnitId,
                                     p.Salary,
                                     p.AssignmentType,
                                     p.AssignmentDate,
                                     e.EmployeeTypeId,
                                     p.LookupId,
                                     p.RankId
                                 };

            // Filter by branch and business unit if necessary
            if (branchid != 0)
            {
                employeesQuery = employeesQuery.Where(x => x.BranchId == branchid);
            }

            if (GlobalHelper.IsPayrollModebranchAndBusinessUnit() && businessUnitId != 0)
            {
                employeesQuery = employeesQuery.Where(x => x.BusinessUnitId == businessUnitId);
            }

            // Materialize the query into a list for iteration
            var employeeList = employeesQuery.AsNoTracking().ToList();

            var now = DateTime.Now; // Reduce DateTime.Now calls inside the loop

            // Use StringBuilder to avoid string concatenation overhead
            foreach (var emp in employeeList)
            {
                if (emp.Salary >= 0)
                {
                    var fullNameBuilder = new StringBuilder();
                    fullNameBuilder.Append(emp.FirstNameEnglish).Append(" ")
                                   .Append(emp.MiddleNameEnglish).Append(" ")
                                   .Append(emp.LastNameEnglish);

                    var employeeView = new EmployeeIdView
                    {
                        PersonId = emp.EmployeeId,
                        EmployeeCode = emp.EmployeeCode,
                        IsActive = emp.IsActive,
                        FullName = fullNameBuilder.ToString(), 
                        Age = now.Year - emp.DateOfBirth.Year, 
                        PlacmentDate = emp.AssignmentDate,
                        BasicSalary = emp.Salary,
                        HiredDate = emp.HiredDate,
                        AssignmentType = emp.AssignmentType,
                        DateOfBirth = emp.DateOfBirth,
                        IsMilitary = emp.EmployeeTypeId == 2, 
                        BusinessUnitId = emp.BusinessUnitId,
                        BranchId = emp.BranchId,
                        JobTitleId = emp.LookupId,
                        RankId = emp.RankId,
                        Status = emp.Status
                    };

                    employeeIdViews.Add(employeeView);
                }
            }

            return employeeIdViews;
        }

        //public List<EmployeeIdView> GetEmployees_old(int branchid, int businessUnitId, string all = "")
        //{

        //    List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
        //    EmployeeIdView EmployeeIdView = new EmployeeIdView();
        //    var employees = from e in db.Person_Employee
        //                    join p in db.Placements
        //                        on e.EmployeeId equals p.EmployeeId
        //                    where e.IsActive && p.isCurrent && !p.IsPend
        //                    //where e.EmployeeCode== "DCDE-00165"
        //                    select (new
        //                    {
        //                        e.EmployeeId,
        //                        e.EmployeeCode,
        //                        e.IsActive,
        //                        e.FirstNameEnglish,
        //                        e.MiddleNameEnglish,
        //                        e.LastNameEnglish,
        //                        e.DateOfBirth,
        //                        e.HiredDate,
        //                        e.Status,
        //                        p.BranchId,
        //                        p.BusinessUnitId,
        //                        p.Salary,
        //                        p.AssignmentType,
        //                        p.AssignmentDate,
        //                        e.EmployeeTypeId,
        //                        p.LookupId,
        //                        p.RankId
        //                    });
        //    var employeeList = employees.AsQueryable();
        //    if (!GlobalHelper.IsProcessAllBranchPayroll())
        //    {
        //        if (branchid != 0)
        //        {
        //            employeeList = employeeList.Where(x => x.BranchId == branchid).AsQueryable();
        //        }

        //        if (GlobalHelper.IsPayrollModebranchAndBusinessUnit() && businessUnitId != 0)
        //        {
        //            employeeList = employeeList.Where(x => x.BusinessUnitId == businessUnitId).AsQueryable();
        //        }
        //    }

        //    foreach (var emp in employeeList)
        //    {

        //        if (emp.Salary >= 0)
        //        {
        //            EmployeeIdView = new EmployeeIdView
        //            {
        //                PersonId = emp.EmployeeId,
        //                EmployeeCode = emp.EmployeeCode,
        //                IsActive = emp.IsActive,
        //                FullName = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " + emp.LastNameEnglish,
        //                Age = (DateTime.Now.Year - emp.DateOfBirth.Year),
        //                PlacmentDate = emp.AssignmentDate,
        //                BasicSalary = emp.Salary,
        //                HiredDate = emp.HiredDate,
        //                AssignmentType = emp.AssignmentType,
        //                DateOfBirth = emp.DateOfBirth,
        //                IsMilitary = emp.EmployeeTypeId == 2 ? true : false,
        //                BusinessUnitId = emp.BusinessUnitId,
        //                BranchId = emp.BranchId,
        //                JobTitleId = emp.LookupId,
        //                RankId = emp.RankId,
        //                Status = emp.Status
        //            };
        //            EmployeeIdViews.Add(EmployeeIdView);
        //        }
        //    }


        //    return EmployeeIdViews;
        //}
    }

}
