using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;
using PayrollInputs = ExceedERP.Reporting.HRM.Payroll.ViewModel.PayrollInputs;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class PayrollInputObjectDataSource : List<PayrollInputs>
    {
        private ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PayrollInputs> GetPayrollInputReport(string period, string branchId, string EmpId, string user)
        {
            List<PayrollInputs> list = new List<PayrollInputs> { };
            PayrollInputs EmployeeInfo;
            Deductions deductions = new Deductions();

            GetOTRateSetting();
            var bankrefence = db.BankReferences;

            var employee = from e in db.Person_Employee
                join p in db.Placements
                    on e.EmployeeId equals p.EmployeeId
                where p.isCurrent == true && e.IsActive && p.Salary > 0
                select (new
                {
                    e.EmployeeId,
                    e.FirstNameEnglish,
                    e.MiddleNameEnglish,
                    e.LastNameEnglish,
                    e.EmployeeCode,
                    p.Salary,
                    p.BranchId
                });
            var employeeList = employee.ToList();
            if (!String.IsNullOrEmpty(EmpId))
            {
                employeeList = employee.Where(x => x.EmployeeCode == EmpId).ToList();
            }


            if (branchId != null)
            {
                List<string> branchs = branchId.Split('-').ToList();
                employeeList = employeeList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
            }
            else if (UserHelper.CheckUserRole(user, "All_Branch_Payroll_Report"))
            {
                List<string> branchs = UserHelper.GetUserBranchId(user);
                employeeList = employeeList.Where(x => branchs.Contains(x.BranchId.ToString())).ToList();
            }
            else
            {
                employeeList = null;
            }
            if (employeeList != null)
            {

                if (!String.IsNullOrEmpty(period))
                {
                    var periodId = db.GLPeriods.FirstOrDefault(x => x.Name == period);

                    foreach (var payrolldata in employeeList)
                    {
                        db = new ExceedDbContext();

                        EmployeeInfo = new PayrollInputs();
                        EmployeeInfo.WorkingDays = 30;
                        EmployeeInfo.DeductionList = new List<Deductions>();
                        EmployeeInfo.EmployeeCode = payrolldata.EmployeeCode;
                        EmployeeInfo.FullName = payrolldata.FirstNameEnglish + " " + payrolldata.MiddleNameEnglish + " " +
                                                payrolldata.LastNameEnglish;
                        EmployeeInfo.BasicSalary = payrolldata.Salary;
                        var bankCode =
                            bankrefence
                                .FirstOrDefault(
                                    x =>
                                        x.Reference == payrolldata.EmployeeId.ToString() &&
                                        x.ReferenceType == "Employee");
                        if (bankCode != null)
                        {
                            EmployeeInfo.BankAccountCode = bankCode.AccountNumber;
                        }


                        var absentdays =
                            db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == payrolldata.EmployeeId)
                                .Where(x => x.GLPeriodId == periodId.GLPeriodId).ToList();
                        if (absentdays.Any())
                            EmployeeInfo.TotalAbsent = absentdays.Select(x => x.Days).FirstOrDefault();
                        else
                            EmployeeInfo.TotalAbsent = 0;

                        #region Additions

                        var additions = db.Additions.Where(x => x.EmployeeId == payrolldata.EmployeeId)
                            .Where(x => x.GLPeriodId == periodId.GLPeriodId).ToList();
                        decimal totalAdditions = 0;

                        foreach (Addition addition in additions)
                        {

                            totalAdditions = totalAdditions + addition.Amount;

                            if (addition.AdditionTaxType == AdditionTaxType.TAXABLE)
                            {
                                EmployeeInfo.TotalTaxableAddition += totalAdditions;
                            }
                            else
                            {
                                EmployeeInfo.TotalNonTaxableAddition += totalAdditions;
                            }

                        }

                        #endregion

                        #region get fixed contribution

                        var contributins = db.ContributionTypes;
                        //get fixed contribution
                        var fixedcontributions =
                            db.FixedContributions.Where(x => x.EmployeeId == payrolldata.EmployeeId && x.IsActive)
                                .ToList();
                        decimal totalFixedcontribution = 0;

                        foreach (FixedContribution fixedcontribution in fixedcontributions)
                        {
                            deductions = new Deductions();
                            if (fixedcontribution.Balance > 0)
                            {


                                var onecontr =
                                    contributins
                                        .FirstOrDefault(
                                            l => l.ContributionTypeId == fixedcontribution.ContributionTypeId);
                                if (onecontr != null)
                                {
                                    deductions.Name = onecontr.Description;

                                    if (fixedcontribution.Balance <= fixedcontribution.MonthlyAmount)
                                    {
                                        deductions.Amount = fixedcontribution.Balance;

                                    }
                                    else
                                    {
                                        deductions.Amount = fixedcontribution.MonthlyAmount;

                                    }
                                    //totalFixedcontribution = totalFixedcontribution + deductions.Amount;

                                    //if (fixedcontribution.Balance <= fixedcontribution.MonthlyAmount)
                                    //{
                                    //    fixedcontribution.Balance = 0;
                                    //}
                                    //else
                                    //{
                                    //    fixedcontribution.Balance = fixedcontribution.Balance -
                                    //                                fixedcontribution.MonthlyAmount;
                                    //}
                                    // db.Entry(fixedcontribution).State = EntityState.Modified;

                                    EmployeeInfo.DeductionList.Add(deductions);

                                }
                            }
                        }


                        #endregion

                        #region get regular contribution

                        //get contribution
                        var contributions =
                            db.RegularContributions.Where(x => x.EmployeeId == payrolldata.EmployeeId)

                                .ToList();
                        decimal totalContributions = 0;

                        foreach (RegularContribution contribution in contributions)
                        {
                            deductions = new Deductions();

                            var onecontr =
                                contributins.Where(l => l.ContributionTypeId == contribution.ContributionTypeId)
                                    .FirstOrDefault();
                            if (onecontr != null)
                            {
                                deductions.Name = onecontr.Description;

                                //regularcontributiondata.Name = period;

                                deductions.Amount = contribution.AdditionalAmount +
                                                    (EmployeeInfo.BasicSalary * contribution.Amount / 100);


                                EmployeeInfo.DeductionList.Add(deductions);

                            }
                        }


                        #endregion

                        #region get credits

                        //get credits
                        var credits =
                            db.Associations.Where(x => x.EmployeeId == payrolldata.EmployeeId)
                                .Where(x => x.IsMemeber == true)
                                .Where(x => x.Type == AssocationType.CREDIT)
                                .ToList();
                        decimal totalCredits = 0;
                        decimal oneCredits = 0;
                        foreach (Association credit in credits)
                        {
                            oneCredits = 0;
                            if (credit.AmtPercnet == FixedContPecntType.PERCENT)
                            {
                                if (credit.NetGross == FixedContAmtType.FROM_GROSS)
                                {
                                    totalCredits = totalCredits + credit.AdditionalAmount +
                                                   (EmployeeInfo.BasicSalary * credit.MonthlyAmount / 100);
                                    oneCredits = credit.AdditionalAmount +
                                                 (EmployeeInfo.BasicSalary * credit.MonthlyAmount / 100);
                                }
                                if (credit.NetGross == FixedContAmtType.FROM_NET)
                                {
                                    decimal Pension = EmployeeInfo.BasicSalary * Convert.ToDecimal(0.07);
                                    decimal beforeIncome =
                                        (decimal)XAPI.IncomeTax.GetIncomeTax((float)(EmployeeInfo.BasicSalary));
                                    totalCredits = totalCredits + credit.AdditionalAmount +
                                                   ((EmployeeInfo.BasicSalary - Pension - beforeIncome) *
                                                    (credit.MonthlyAmount / 100));
                                    oneCredits = credit.AdditionalAmount +
                                                 ((EmployeeInfo.BasicSalary - Pension - beforeIncome) *
                                                  (credit.MonthlyAmount / 100));
                                }
                            }
                            else
                            {
                                totalCredits = totalCredits + credit.Amount;
                                oneCredits = credit.Amount;
                            }

                            #region Assocation Contribution Deduction




                            #endregion
                        }
                        EmployeeInfo.Saving = totalCredits;

                        #endregion

                        #region get SportFee

                        //get credits
                        var sporsts =
                            db.Associations.Where(x => x.EmployeeId == payrolldata.EmployeeId)
                                .Where(x => x.IsMemeber == true)
                                .Where(x => x.Type == AssocationType.SPORT_FEE)
                                .ToList();
                        decimal SportFee = 0;
                        decimal oneSportFee = 0;
                        foreach (Association credit in sporsts)
                        {
                            oneSportFee = 0;
                            if (credit.AmtPercnet == FixedContPecntType.PERCENT)
                            {
                                if (credit.NetGross == FixedContAmtType.FROM_GROSS)
                                {
                                    SportFee = SportFee + credit.AdditionalAmount +
                                               (EmployeeInfo.BasicSalary * credit.MonthlyAmount / 100);
                                    oneSportFee = credit.AdditionalAmount +
                                                  (EmployeeInfo.BasicSalary * credit.MonthlyAmount / 100);
                                }
                                if (credit.NetGross == FixedContAmtType.FROM_NET)
                                {
                                    decimal Pension = EmployeeInfo.BasicSalary * Convert.ToDecimal(0.07);
                                    decimal beforeIncome =
                                        (decimal)XAPI.IncomeTax.GetIncomeTax((float)EmployeeInfo.BasicSalary);
                                    SportFee = SportFee + credit.AdditionalAmount +
                                               ((EmployeeInfo.BasicSalary - Pension - beforeIncome) *
                                                (credit.MonthlyAmount / 100));
                                    oneSportFee = credit.AdditionalAmount +
                                                  ((EmployeeInfo.BasicSalary - Pension - beforeIncome) *
                                                   (credit.MonthlyAmount / 100));
                                }
                            }
                            else
                            {
                                SportFee = SportFee + credit.Amount;
                                oneSportFee = credit.Amount;
                            }


                            #region Assocation Contribution Deduction



                            #endregion
                        }
                        EmployeeInfo.SportFee = SportFee;

                        #endregion

                        #region get labour union

                        ////get credits
                        var labourunions =
                            db.Associations.Where(x => x.EmployeeId == payrolldata.EmployeeId)
                                .Where(x => x.IsMemeber == true)
                                .Where(x => x.Type == AssocationType.LABOUR)
                                .ToList();
                        decimal totalLabour = 0;
                        decimal oneLabour = 0;
                        foreach (Association labour in labourunions)
                        {
                            if (labour.AmtPercnet == FixedContPecntType.PERCENT)
                            {
                                if (labour.NetGross == FixedContAmtType.FROM_GROSS)
                                {
                                    totalLabour = totalLabour + labour.AdditionalAmount +
                                                  (EmployeeInfo.BasicSalary * labour.MonthlyAmount / 100);
                                    oneLabour = labour.AdditionalAmount +
                                                (EmployeeInfo.BasicSalary * labour.MonthlyAmount / 100);
                                }
                                if (labour.NetGross == FixedContAmtType.FROM_NET)
                                {
                                    decimal Pension = EmployeeInfo.BasicSalary * Convert.ToDecimal(0.07);
                                    decimal beforeIncome =
                                        (decimal)XAPI.IncomeTax.GetIncomeTax((float)EmployeeInfo.BasicSalary);
                                    totalLabour = totalLabour + labour.AdditionalAmount +
                                                  ((EmployeeInfo.BasicSalary - Pension - beforeIncome) *
                                                   (labour.MonthlyAmount / 100));
                                    oneLabour = labour.AdditionalAmount +
                                                ((EmployeeInfo.BasicSalary - Pension - beforeIncome) *
                                                 (labour.MonthlyAmount / 100));
                                }
                            }
                            else
                            {
                                totalLabour = totalLabour + labour.Amount;
                                oneLabour = labour.Amount;
                            }


                            #region Assocation Contribution Deduction



                            #endregion
                        }
                        EmployeeInfo.LabourUnion = totalLabour;

                        #endregion


                        #region get loans

                        bool jumploan = false;
                        //get loans
                        var loans = db.Loans.Where(x => x.EmployeeId == payrolldata.EmployeeId && x.IsActive).ToList();
                        decimal totalLoans = 0;
                        PayrollProcessLoans loandata;
                        foreach (Loan loan in loans)
                        {
                            loandata = new PayrollProcessLoans();
                            if (!(jumploan && !loan.JumpHoliday))
                            {
                                if (loan.Balance > 0)
                                {

                                    if (loan.Balance <= loan.MonthlyAmount)
                                    {
                                        loandata.Amount = loan.Balance;
                                        loandata.Outstanding = 0;
                                    }
                                    else
                                    {
                                        loandata.Amount = loan.MonthlyAmount;
                                        loandata.Outstanding = loan.Balance - loan.MonthlyAmount;
                                    }
                                    totalLoans = totalLoans + loandata.Amount;
                                    //update loan info
                                    if (loan.Balance <= loan.MonthlyAmount)
                                    {
                                        loan.Balance = 0;
                                    }
                                    else
                                    {
                                        loan.Balance = loan.Balance - loan.MonthlyAmount;
                                    }


                                    #region LoanDeduction





                                    #endregion
                                }
                            }
                        }

                        EmployeeInfo.Loan = totalLoans;

                        #endregion

                        #region get advances

                        //get advances
                        var advances =
                            db.Advances.Where(x => x.EmployeeId == payrolldata.EmployeeId && x.IsActive).ToList();
                        decimal totalAdvances = 0;
                        PayrollProcessAdvances advancedata;
                        foreach (Advance advance in advances)
                        {
                            if (!(jumploan && !advance.JumpHoliday))
                            {
                                if (advance.Balance > 0)
                                {
                                    advancedata = new PayrollProcessAdvances();

                                    if (advance.Balance <= advance.MonthlyAmount)
                                    {
                                        advancedata.Amount = advance.Balance;
                                        advancedata.Outstanding = 0;
                                    }
                                    else
                                    {
                                        advancedata.Amount = advance.MonthlyAmount;
                                        advancedata.Outstanding = advance.Balance - advance.MonthlyAmount;
                                    }
                                    totalAdvances = totalAdvances + advancedata.Amount;
                                    //update loan info
                                    if (advance.Balance <= advance.MonthlyAmount)
                                    {
                                        advance.Balance = 0;
                                    }
                                    else
                                    {
                                        advance.Balance = advance.Balance - advance.MonthlyAmount;
                                    }




                                }
                            }
                        }
                        EmployeeInfo.Advance = totalAdvances;

                        #endregion


                        #region allowances

                        var allowanceTypeRule =
                            db.AllowanceTypeRule.ToList();
                        string settingvalue = "NON_TAXABLE";
                        var isTaxableSetting =
                            db.Configurations.Where(
                                    x => x.Category == "Payroll" && x.Reference == "BENEFIT_ZERO_THRESHOLD")
                                .FirstOrDefault();
                        if (isTaxableSetting != null)
                        {
                            settingvalue = isTaxableSetting.Attribute;
                        }
                        var allowances =
                            db.AllowancePerEmployees.Where(
                                x => x.EmployeeId == payrolldata.EmployeeId && x.Pend == false).ToList();
                        decimal totalAllowance = 0;
                        decimal totalTaxableAllowance = 0;
                        decimal totalNonTaxableAllowance = 0;
                        decimal houseallowanceSalaryThreshold = 0;
                        AllowanceDefination allowancemodel;
                        PayrollProcessAllowances allowancedata;
                        decimal Threshold = 0;
                        foreach (AllowancePerEmployee allowance in allowances)
                        {
                            allowancemodel = new AllowanceDefination();
                            allowancedata = new PayrollProcessAllowances();
                            allowancemodel =
                                db.AllowanceDefinations.Where(
                                        x => x.AllowanceDefinationId == allowance.AllowanceDefinationId)
                                    .FirstOrDefault();



                            var rule =
                                allowanceTypeRule.Where(a => a.AllowanceType == allowancemodel.AllowanceType)
                                    .FirstOrDefault();

                            //if (rule == null)
                            //{


                            //}
                            if (allowancemodel.AmtPercent == FixedContPecntType.PERCENT)
                            {
                                allowancemodel.Amount = EmployeeInfo.BasicSalary * (allowancemodel.Amount / 100);
                            }

                            if (allowancemodel.AllowanceType == AllowanceType.PROFESSIONAL)
                            {
                                decimal totalSickleave = 0;
                                var absentdayss =
                                    db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == payrolldata.EmployeeId)
                                        .Where(x => x.GLPeriodId == periodId.GLPeriodId).ToList();
                                if (absentdays.Any())
                                {
                                    totalSickleave += absentdayss.Select(x => x.Days).FirstOrDefault();
                                }

                                var checkFiscalYear =
                                    db.GLPeriods.Where(x => x.GLPeriodId == periodId.GLPeriodId).FirstOrDefault();
                                if (checkFiscalYear != null)
                                {
                                    var cheksiclLiave =
                                        db.Leaves.Where(
                                            x =>
                                                x.EmployeeId == allowance.EmployeeId &&
                                                x.GlFiscalYearId == checkFiscalYear.GlFiscalYearId).ToList();
                                    foreach (var l in cheksiclLiave)
                                    {
                                        if (l.LeaveType == LeaveType.SICK_LEAVE)
                                        {
                                            totalSickleave += l.Amount;
                                        }
                                    }
                                }
                                allowancemodel.Amount = allowancemodel.Amount -
                                                        (allowancemodel.Amount / 30) * totalSickleave;

                            }
                            if (allowancemodel.AllowanceType == AllowanceType.POSITION)
                            {

                                decimal totalDelagatin = 0;

                                var checkDelegation =
                                     db.Delegations.Where(
                                             x =>
                                                 x.FromEmployeeId == allowance.EmployeeId &&
                                                 x.FromDateTime >= periodId.DateFrom &&
                                                 x.FromDateTime <= periodId.DateTo)
                                         .FirstOrDefault();

                                if (checkDelegation != null)
                                {
                                    //if no end date ,take up to end of the period
                                    DateTime delegationEndDate = checkDelegation.ToDateTime != null ? (DateTime)checkDelegation.ToDateTime : periodId.DateTo;

                                    if (periodId.DateTo < checkDelegation.FromDateTime)
                                    {

                                    }
                                    else if (delegationEndDate < periodId.DateFrom)
                                    {

                                    }
                                    else
                                    {
                                        if (periodId.DateTo > delegationEndDate &&
                                            checkDelegation.FromDateTime > periodId.DateFrom)
                                        {
                                            var dif =
                                                (checkDelegation.FromDateTime - delegationEndDate);
                                            double dateDiff =
                                                dif.HasValue? dif.Value.TotalDays : 0;
                                            if (dateDiff >= 30)
                                            {
                                                totalDelagatin = Convert.ToDecimal(dateDiff);
                                            }
                                        }
                                        else if (delegationEndDate > periodId.DateTo &&
                                                 checkDelegation.FromDateTime > periodId.DateFrom)
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
                                        else if (periodId.DateTo < checkDelegation.FromDateTime)
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
                                                x.FromDateTime >= periodId.DateFrom &&
                                                x.FromDateTime <= periodId.DateTo)
                                        .FirstOrDefault();

                                if (checkDelegation != null)
                                {
                                    //if no end date ,take up to end of the period
                                    DateTime delegationEndDate = checkDelegation.ToDateTime != null ? (DateTime)checkDelegation.ToDateTime : periodId.DateTo;
                                    if (periodId.DateTo < checkDelegation.FromDateTime)
                                    {

                                    }
                                    else if (delegationEndDate < periodId.DateFrom)
                                    {

                                    }
                                    else
                                    {
                                        if (periodId.DateTo > delegationEndDate &&
                                            checkDelegation.FromDateTime > periodId.DateFrom)
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
                                        else if (delegationEndDate > periodId.DateTo &&
                                                 checkDelegation.FromDateTime > periodId.DateFrom)
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
                                        else if (periodId.DateTo < checkDelegation.FromDateTime)
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
                                    }
                                    allowancemodel.Amount = allowancemodel.Amount +
                                                            (allowancemodel.Amount / 30) * totalDelagatin;

                                }
                            }
                            Threshold = ((decimal)(rule.Percentage / 100) * EmployeeInfo.BasicSalary);

                            #region maximum non taxable rule

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

                                    EmployeeInfo.TotalNonTaxableAllowance = Convert.ToDecimal(allowancemodel.Amount);
                                    EmployeeInfo.TotalTaxableAllowance = 0;

                                }
                                else
                                {
                                    if (allowancemodel.Amount > Threshold)
                                    {
                                        if (allowancemodel.Amount > rule.TaxTrasholdAmount && rule.TaxTrasholdAmount > 0)
                                        {
                                            allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                            allowancedata.Taxable = Convert.ToDecimal(allowancemodel.Amount) -
                                                                    Convert.ToDecimal(rule.TaxTrasholdAmount);
                                            allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                                            totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                            totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                       allowancedata.NonTaxable;
                                            totalAllowance = totalAllowance + allowancedata.Total;

                                            EmployeeInfo.TotalNonTaxableAllowance =
                                                Convert.ToDecimal(rule.TaxTrasholdAmount);
                                            EmployeeInfo.TotalTaxableAllowance =
                                                Convert.ToDecimal(allowancemodel.Amount) -
                                                Convert.ToDecimal(rule.TaxTrasholdAmount);
                                            //payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                            //                            payroldata.TaxableAllowance;
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

                                            EmployeeInfo.TotalNonTaxableAllowance = Threshold;
                                            EmployeeInfo.TotalTaxableAllowance =
                                                Convert.ToDecimal(allowancemodel.Amount) -
                                                Threshold;

                                        }
                                    }
                                    else
                                    {
                                        if (allowancemodel.Amount > rule.TaxTrasholdAmount && rule.TaxTrasholdAmount > 0)
                                        {
                                            allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                            allowancedata.Taxable = Convert.ToDecimal(allowancemodel.Amount) -
                                                                    Convert.ToDecimal(rule.TaxTrasholdAmount);
                                            allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                                            totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                            totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                       allowancedata.NonTaxable;
                                            totalAllowance = totalAllowance + allowancedata.Total;

                                            EmployeeInfo.TotalNonTaxableAllowance =
                                                Convert.ToDecimal(rule.TaxTrasholdAmount);
                                            EmployeeInfo.TotalTaxableAllowance =
                                                Convert.ToDecimal(allowancemodel.Amount) -
                                                Convert.ToDecimal(rule.TaxTrasholdAmount);

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

                                            EmployeeInfo.TotalNonTaxableAllowance =
                                                Convert.ToDecimal(allowancemodel.Amount);
                                            EmployeeInfo.TotalTaxableAllowance = 0;

                                        }

                                    }
                                }
                            }
                            else
                            {
                                if (allowancemodel.Amount > Threshold)
                                {
                                    if (allowancemodel.Amount > rule.TaxTrasholdAmount && rule.TaxTrasholdAmount > 0)
                                    {
                                        allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                        allowancedata.Taxable = Convert.ToDecimal(allowancemodel.Amount) -
                                                                Convert.ToDecimal(rule.TaxTrasholdAmount);
                                        allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                                        totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                        totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                   allowancedata.NonTaxable;
                                        totalAllowance = totalAllowance + allowancedata.Total;

                                        EmployeeInfo.TotalNonTaxableAllowance = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                        EmployeeInfo.TotalTaxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                                                             Convert.ToDecimal(rule.TaxTrasholdAmount);

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

                                        EmployeeInfo.TotalNonTaxableAllowance = Threshold;
                                        EmployeeInfo.TotalTaxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                                                             Threshold;

                                    }
                                }
                                else
                                {
                                    if (allowancemodel.Amount > rule.TaxTrasholdAmount && rule.TaxTrasholdAmount > 0)
                                    {
                                        allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                        allowancedata.Taxable = Convert.ToDecimal(allowancemodel.Amount) -
                                                                Convert.ToDecimal(rule.TaxTrasholdAmount);
                                        allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                                        totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                        totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                   allowancedata.NonTaxable;
                                        totalAllowance = totalAllowance + allowancedata.Total;

                                        EmployeeInfo.TotalNonTaxableAllowance = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                        EmployeeInfo.TotalTaxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                                                             Convert.ToDecimal(rule.TaxTrasholdAmount);

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

                                        EmployeeInfo.TotalNonTaxableAllowance = Convert.ToDecimal(allowancemodel.Amount);
                                        EmployeeInfo.TotalTaxableAllowance = 0;

                                    }

                                }

                            }

                            #endregion



                            //if (allowancemodel.AllowanceType == AllowanceType.HOME)
                            //{
                            //    payroldata.TaxableHouseAllowance += payroldata.TaxableAllowance;
                            //    payroldata.NonTaxableHouseAllowance += payroldata.NonTaxableAllowance;
                            //    payroldata.TotalHouseAllowance += payroldata.TaxableAllowance +
                            //                                     payroldata.NonTaxableAllowance;
                            //}
                            //if (allowancemodel.AllowanceType == AllowanceType.POSITION)
                            //{
                            //    payroldata.TaxablePositionAllowance += payroldata.TaxableAllowance;
                            //    payroldata.NonTaxablePositionAllowance += payroldata.NonTaxableAllowance;
                            //    payroldata.TotalPositionAllowance += payroldata.TaxableAllowance +
                            //                                        payroldata.NonTaxableAllowance;
                            //}
                            //if (allowancemodel.AllowanceType == AllowanceType.PROFESSIONAL)
                            //{
                            //    payroldata.TaxableProfessionalAllowance += payroldata.TaxableAllowance;
                            //    payroldata.NonTaxableProfessionalAllowance += payroldata.NonTaxableAllowance;
                            //    payroldata.TotalProfessionalAllowance += payroldata.TaxableAllowance +
                            //                                            payroldata.NonTaxableAllowance;
                            //}
                            ////if (allowancemodel.AllowanceType == AllowanceType.TOP_UP)
                            ////{
                            ////    payroldata.TaxableTopUpAllowance += payroldata.TaxableAllowance;
                            ////    payroldata.NonTaxableTopUpAllowance += payroldata.NonTaxableAllowance;
                            ////    payroldata.TotalTopUpAllowance += payroldata.TaxableAllowance +
                            ////                                     payroldata.NonTaxableAllowance;
                            ////}
                            //if (allowancemodel.AllowanceType == AllowanceType.TRANSPORT)
                            //{
                            //    payroldata.TaxableTransportAllowance += payroldata.TaxableAllowance;
                            //    payroldata.NonTaxableTransportAllowance += payroldata.NonTaxableAllowance;
                            //    payroldata.TotalTransportAllowance += payroldata.TaxableAllowance +
                            //                                         payroldata.NonTaxableAllowance;
                            //}
                            //if (allowancemodel.AllowanceType == AllowanceType.TELEPHONE)
                            //{
                            //    payroldata.TaxableTelephoneAllowance += payroldata.TaxableAllowance;
                            //    payroldata.NonTaxableTelephoneAllowance += payroldata.NonTaxableAllowance;
                            //    payroldata.TotalTelephoneAllowance += payroldata.TaxableAllowance +
                            //                                         payroldata.NonTaxableAllowance;
                            //}


                            //if (allowancemodel.AllowanceType == AllowanceType.RESPONSIBILITY)
                            //{
                            //    payroldata.TaxableResponsiblityAllowance += payroldata.TaxableAllowance;
                            //    payroldata.NonTaxableResponsiblityAllowance += payroldata.NonTaxableAllowance;
                            //    payroldata.TotalResponsiblityAllowance += payroldata.TaxableAllowance +
                            //                                         payroldata.NonTaxableAllowance;
                            //}

                            #region Allowance Additions



                        }
                        EmployeeInfo.TotalTaxableAllowance = totalTaxableAllowance;
                        EmployeeInfo.TotalNonTaxableAllowance = totalNonTaxableAllowance;


                        #endregion

                        #endregion

                        #region OT


                        var overtimes = db.OverTimes.Where(x => x.EmployeeId == payrolldata.EmployeeId)
                            .Where(x => x.GLPeriodId == periodId.GLPeriodId).ToList();
                        decimal totalOT = 0;
                        decimal xx = 0;
                        decimal yy = 0;
                        foreach (OverTime ot in overtimes)
                        {
                            xx = (EmployeeInfo.BasicSalary / oTCoefficient);
                            yy = (ot.RestdayHours * restDayRate) + (ot.NormalHours * NormalRate) +
                                 (ot.NightHours * NightRate) +
                                 (ot.HolidayHours * HolidayRate);
                            totalOT = totalOT + (xx * yy);
                        }
                        EmployeeInfo.OverTime = totalOT;

                        #endregion

                        list.Add(EmployeeInfo);
                    }
                    return list;
                }
            }
            return list;
        }

        private static decimal restDayRate { get; set; }
        private static decimal NormalRate { get; set; }
        private static decimal NightRate { get; set; }
        private static decimal HolidayRate { get; set; }
        private static decimal oTCoefficient { get; set; }
        private bool GetOTRateSetting()
        {
            var otrates = db.Configurations.Where(x => x.Reference == "OverTime").ToList();
            try
            {
                if (otrates.Any())
                {
                    Configuration config = new Configuration();
                    config = otrates.Where(o => o.Attribute == OverTimeAttribute.RESTDAY_RATE.ToString())
                        .FirstOrDefault();
                    if (config != null)
                    {
                        restDayRate =
                            Convert.ToDecimal(config.CurrentValue);
                    }

                    config = otrates.Where(o => o.Attribute == OverTimeAttribute.NORMAL_RATE.ToString())
                        .FirstOrDefault();
                    if (config != null)
                    {
                        NormalRate =
                            Convert.ToDecimal(config.CurrentValue);
                    }
                    config = otrates.Where(o => o.Attribute == OverTimeAttribute.NIGHT_RATE.ToString())
                        .FirstOrDefault();

                    if (config != null)
                    {
                        NightRate =
                            Convert.ToDecimal(config.CurrentValue);
                    }
                    config = otrates.Where(o => o.Attribute == OverTimeAttribute.HOLIDAY_RATE.ToString())
                        .FirstOrDefault();
                    if (config != null)
                    {
                        HolidayRate =
                            Convert.ToDecimal(config.CurrentValue);
                    }
                    config = otrates.Where(o => o.Attribute == OverTimeAttribute.COEFFICIENT.ToString())
                        .FirstOrDefault();
                    if (config != null)
                    {
                        oTCoefficient =
                            Convert.ToDecimal(config.CurrentValue);
                    }

                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {

                return false;
            }
            return false;
        }
    }
}