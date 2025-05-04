using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Mvc;
using System.Web.Mvc.Routing.Constraints;
using Antlr.Runtime.Misc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Finance;
using ExceedERP.Reporting.HRM.Payroll;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using Telerik.Reporting.Processing;
using System.Web;
using Kendo.Mvc.UI.Fluent;
using Microsoft.SqlServer.Server;
using ExceedERP.Core.Domain.HRM.Discipline;
using ExceedERP.Core.Domain.HRM.Allowance;
using ExceedERP.Core.Domain.HRM.Company;
using ExceedERP.Core.Domain.HRM.Payroll.Setting;


namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Admin + "," + Payroll_Process + "," + Payroll_View + "," + Payroll_Distribute + "," + Payroll_Close + "," + Payroll_Transfer)]
    public partial class PayrollProcessController : BaseController
    {
        ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Process = "Payroll_Process";
        private const string Payroll_Distribute = "Payroll_Distribute";
        private const string Payroll_Close = "Payroll_Close";
        private const string Payroll_Admin = "Payroll_Admin";
        private const string Payroll_View = "Payroll_View";
        private const string Payroll_Transfer = "Payroll_Transfer";
        private static decimal restDayRate { get; set; }
        private static decimal NormalRate { get; set; }
        private static decimal NightRate { get; set; }
        private static decimal HolidayRate { get; set; }
        private static decimal oTCoefficient { get; set; }
        static decimal EmployeeContribution { get; set; }
        private static decimal CompanyContribution { get; set; }
        static decimal ProvEmployeeContribution { get; set; }
        private static decimal ProvCompanyContribution { get; set; }
        private static decimal RetirementAge { get; set; }
        private static int ProbationDateLenghth { get; set; }
        private static bool ApplyProbationSetting { get; set; }

        private static bool DeductUnpaidLeave = false;
        private static List<Lookup> lookups { get; set; }
        //   [Authorize(Roles = Payroll + "," + Payroll_Admin)]

        AccountHelper accountHelper = new AccountHelper();
        private DynamicAccountHelper dynamicAccount = new DynamicAccountHelper();
       

        public List<EmployeeIdView> GetEmployees(int branchid, string all = "")
        {
            List<EmployeeIdView> EmployeeIdViews = new List<EmployeeIdView>();
            EmployeeIdView EmployeeIdView = new EmployeeIdView();
            var employees = from e in db.Person_Employee
                            join p in db.Placements
                                on e.EmployeeId equals p.EmployeeId
                            where e.IsActive && p.isCurrent && !p.IsPend
                            //where e.EmployeeCode== "MP-00302"
                            select (new
                            {
                                e.EmployeeId,
                                e.IsActive,
                                e.FirstNameEnglish,
                                e.MiddleNameEnglish,
                                e.LastNameEnglish,
                                e.DateOfBirth,
                                e.HiredDate,
                                e.Status,
                                p.BranchId,
                                p.Salary,
                                p.AssignmentType,
                                p.AssignmentDate
                            });
            var employeeList = employees.ToList();
            if (all == "all")
            {
                var allbranch = db.Branch.Select(x => x.BranchId).ToList();
                employeeList = employeeList.Where(x => allbranch.Contains(x.BranchId)).ToList();
            }
            else
            {
                employeeList = employeeList.Where(x => x.BranchId == branchid).ToList();
            }
            foreach (var emp in employeeList)
            {

                if (emp.Salary > 0)
                {
                    EmployeeIdView = new EmployeeIdView();
                    EmployeeIdView.PersonId = emp.EmployeeId;
                    EmployeeIdView.IsActive = emp.IsActive;
                    EmployeeIdView.FullName = emp.FirstNameEnglish + " " + emp.MiddleNameEnglish + " " +
                                              emp.LastNameEnglish;
                    EmployeeIdView.Age = (DateTime.Now.Year - emp.DateOfBirth.Year);
                    EmployeeIdView.PlacmentDate = emp.AssignmentDate;
                    EmployeeIdView.HiredDate = emp.HiredDate;
                    EmployeeIdView.AssignmentType = emp.AssignmentType;
                    EmployeeIdView.DateOfBirth = emp.DateOfBirth;
                    EmployeeIdView.Status = emp.Status;
                    EmployeeIdViews.Add(EmployeeIdView);
                }
            }


            return EmployeeIdViews;
        }
        private int SaveViewModel(string period, int EmpId, int branchid)//, bool jumploan=false)
        {
            try
            {
                var identifications = db.Identifications.Where(x => x.Type == IdentificationType.PENSION_NO).ToList();
                var Providentifications =
                    db.Identifications.Where(x => x.Type == IdentificationType.PROVIDENT_FUND).ToList();
                bool jumploan = false;

                string message = "";
                PayrollProcessAdditions additiondata;
                if (!String.IsNullOrEmpty(period))
                {

                    PayrollProcessViewModel payroldata;


                    var periodObj =
                        db.GLPeriods.Where(
                            x => x.GLPeriodId.ToString() == period)
                            .FirstOrDefault();
                    if (periodObj != null)
                    {
                        period = periodObj.Name;



                    }
                                                         
                    var activeEmployees = GetEmployees(branchid).ToList();



                    message = "";

                    #region update period status

                    var findPeriod =
                        db.GLPeriods.Where(x => x.Name == period)
                            .FirstOrDefault();


                    #endregion

                    #region Allowance Treashhold

                    var allowanceTypeRule =
                        db.AllowanceTypeRule.ToList();

                    #endregion

                    var placments = db.Placements.Where(x => x.isCurrent);
                    var FilteredactiveEmployees = activeEmployees.Where(x => x.PersonId == EmpId).ToList();
                   // int maxempassignment = 0;
                    foreach (EmployeeIdView employee in FilteredactiveEmployees)
                    {
                        string branchCode = null;

                        payroldata = new PayrollProcessViewModel();
                        payroldata.UnpaidLeaveDays = GetemployeeUnpaidLeaveDays(employee.PersonId, findPeriod.GLPeriodId);
                        payroldata.CreatedOn = DateTime.Now.Date;
                        payroldata.CreatedBy = User.Identity.Name;
                        payroldata.EmployeeId = employee.PersonId;
                        payroldata.FullName = employee.FullName;
                        payroldata.PeriodName = period;
                        payroldata.GLPeriodId = findPeriod.GLPeriodId;
                        payroldata.BranchId = branchid;
                        var timeSpan = findPeriod.DateFrom - employee.DateOfBirth;
                        var yearDiff = timeSpan.TotalDays / 365.25;
                        employee.Age = (int)yearDiff;
                        var employeeAccouunt =
                            db.BankReferences.Where(
                                x =>
                                    x.Reference == employee.PersonId.ToString() && x.Type == BankAccountType.PAYROLL &&
                                    x.ReferenceType == "Employee").FirstOrDefault();
                        if (employeeAccouunt != null)
                        {
                            payroldata.BankAccount = employeeAccouunt.AccountNumber;
                        }

                        #region absence

                        bool checkNewEmployeeState = false;
                       // decimal totalAbcent = 0;
                        var absentdays = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == employee.PersonId)
                            .Where(x => x.GLPeriodId == findPeriod.GLPeriodId).ToList();
                        if (absentdays.Any())
                            payroldata.DaysAbsent = absentdays.Select(x => x.Days).FirstOrDefault();
                        else
                            payroldata.DaysAbsent = 0;



                        #endregion

                        #region set cost center

                        var employeeAccount =
                            db.PayrollEmployeeAccounts.Where(x => x.EmployeeId == employee.PersonId)
                                .FirstOrDefault();
                        if (employeeAccount != null)
                        {
                            payroldata.CostCenterAccountCode = employeeAccount.CostCenterAccountCode;
                            payroldata.LocationCode = employeeAccount.LocationAccCode;
                            //var description =
                            //    db.GLChartOfAccountCostCenters.Where(x => x.Values == employeeAccount.CostCenterAccountCode)
                            //        .FirstOrDefault();
                            //if (description != null)
                            //{
                            //    payroldata.CostCenterAccountDesc = description.Description;
                            //}

                            payroldata.OrgBranchAccountCode = employeeAccount.LocationAccCode;
                            //payroldata.LocationCode = employeeAccount.LocationAccCode;
                        }
                        else
                        {
                            payroldata.CostCenterAccountCode = "";
                            payroldata.CostCenterAccountDesc = "";
                            payroldata.OrgBranchAccountCode = employeeAccount.LocationAccCode;
                            payroldata.LocationCode = employeeAccount.LocationAccCode;
                        }
                        var locationCode = placments.Where(x => x.EmployeeId == employee.PersonId).FirstOrDefault();
                        if (locationCode != null)
                        {
                            var branch = db.Branch.Where(x => x.BranchId == locationCode.BranchId).FirstOrDefault();
                            if (branch != null)
                            {
                                payroldata.OrgBranchAccountCode = branch.LocationAccount;
                                payroldata.LocationCode = branch.LocationAccount;
                            }
                            else
                            {
                                payroldata.OrgBranchAccountCode = "";
                                payroldata.LocationCode = "";
                            }
                        }
                        else
                        {
                            payroldata.OrgBranchAccountCode = "";
                            payroldata.LocationCode = "";
                        }

                        #endregion
                       
                        #region basic salary

                        var assignment =
                            db.Placements.Where(x => x.EmployeeId == employee.PersonId && x.isCurrent).FirstOrDefault();
                        if (assignment != null)
                        {
                            payroldata.BasicSalary = assignment.Salary;
                            //branchCode = assignment.Branch.Code;
                            var br = db.Branch.Find(assignment.BranchId);
                            if(br != null)
                            {
                                branchCode = br.Code;
                            }
                        }
                        else
                        {
                            payroldata.BasicSalary = 0;
                        }
                        #region set branch code ()
                        payroldata.LocationCode = branchCode;
                        #endregion
                        #region Check placment Date 
                        decimal WorkingdateDiff = 0;                       

                        if (employee.AssignmentType == AssignmentType.NEW)
                        {
                            if (findPeriod.DateTo < employee.PlacmentDate)
                            {
                            }
                            else
                            {
                                if (findPeriod.DateTo > employee.PlacmentDate)
                                {
                                    WorkingdateDiff = (decimal)(findPeriod.DateTo - employee.PlacmentDate).TotalDays+1;                                               
                                                                                                                     //WorkingdateDiff = (30 - employee.PlacmentDate.Day)+1;
                                    decimal actualWorkeddays = DeductUnpaidLeave ? WorkingdateDiff - payroldata.UnpaidLeaveDays : WorkingdateDiff;//added
                                    if (actualWorkeddays <= 30 && actualWorkeddays > 0)
                                    {
                                        checkNewEmployeeState = true;
                                        //int DaysAbsent = Convert.ToInt32(30 - WorkingdateDiff);
                                        payroldata.BasicSalary = (payroldata.BasicSalary / 30) * (decimal)actualWorkeddays;
                                    }
                                    else if (actualWorkeddays == 0)
                                        payroldata.BasicSalary = 0;
                                }

                            }


                        }

                        #endregion
                        #region Basic Salary Additions

                        db = new ExceedDbContext();
                        additiondata = new PayrollProcessAdditions();
                        additiondata.CreatedOn = DateTime.Now.Date;
                        additiondata.CreatedBy = User.Identity.Name;

                        additiondata.Name = "Basic Salary";

                        additiondata.Amount = payroldata.BasicSalary;



                        #endregion

                        #region OT

                        var overtimes = db.OverTimes.Where(x => x.EmployeeId == employee.PersonId)
                            .Where(x => x.GLPeriodId == findPeriod.GLPeriodId).ToList();
                        decimal totalOT = 0;
                        decimal xx = 0;
                        decimal yy = 0;
                        foreach (OverTime ot in overtimes)
                        {
                            xx = (payroldata.BasicSalary / oTCoefficient);
                            yy = (ot.RestdayHours * restDayRate) + (ot.NormalHours * NormalRate) + (ot.NightHours * NightRate) +
                                 (ot.HolidayHours * HolidayRate);
                            totalOT = totalOT + (xx * yy);
                        }
                        payroldata.TotalOvertime = totalOT;

                        #region OT Additions

                        db = new ExceedDbContext();
                        additiondata = new PayrollProcessAdditions();
                        additiondata.CreatedOn = DateTime.Now.Date;
                        additiondata.CreatedBy = User.Identity.Name;

                        additiondata.Name = "Over Time";

                        additiondata.Amount = payroldata.TotalOvertime;



                        #endregion

                        #endregion


                        #endregion

                        //get allowances

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
                        var allowances =
                            db.AllowancePerEmployees.Where(x => x.EmployeeId == employee.PersonId && x.Pend == false)
                                .ToList();
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
                            allowancemodel =
                                db.AllowanceDefinations.Where(
                                    x => x.AllowanceDefinationId == allowance.AllowanceDefinationId)
                                    .FirstOrDefault();

                            allowancedata = new PayrollProcessAllowances();
                            allowancedata.CreatedOn = DateTime.Now.Date;
                            allowancedata.CreatedBy = User.Identity.Name;

                            allowancedata.Name = allowancemodel.AllowanceName;

                            allowancedata.AllowanceType = allowancemodel.AllowanceType;


                            var rule =
                                allowanceTypeRule.Where(a => a.AllowanceType == allowancemodel.AllowanceType)
                                    .FirstOrDefault();

                            if (rule == null)
                            {
                                message = "Please Set Allowance Rule For " + allowancemodel.AllowanceType.ToString();
                                // RedirectToAction("Index");
                                return 0;
                            }
                            if (allowancemodel.AmtPercent == FixedContPecntType.PERCENT)
                            {
                                allowancemodel.Amount = payroldata.BasicSalary * (allowancemodel.Amount / 100);
                            }

                            if (allowancemodel.AllowanceType == AllowanceType.PROFESSIONAL)
                            {
                                decimal totalSickleave = 0;
                                decimal totalAbsent = 0;
                                decimal totalSickleaveAndabsent = 0;
                                var absentdayss =
                                    db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == employee.PersonId)
                                        .Where(x => x.GLPeriodId == findPeriod.GLPeriodId).ToList();
                                if (absentdays.Any())
                                {
                                    totalAbsent = absentdayss.Sum(x => x.Days);
                                }

                                totalSickleave = GetEmployeeSickLeave(employee.PersonId, findPeriod.GLPeriodId,30);
                                totalSickleaveAndabsent = totalSickleave + totalAbsent;
                                //var checkFiscalYear =
                                //    db.GLPeriods.Where(x => x.GLPeriodId == findPeriod.GLPeriodId).FirstOrDefault();
                                //if (checkFiscalYear != null)
                                //{
                                //    var cheksiclLiave =
                                //        db.Leaves.Where(
                                //            x =>
                                //                x.EmployeeId == allowance.EmployeeId &&
                                //                x.GlFiscalYearId == checkFiscalYear.GlFiscalYearId).ToList();
                                //    foreach (var l in cheksiclLiave)
                                //    {
                                //        if (l.LeaveType == LeaveType.SICK_LEAVE)
                                //        {
                                //            totalSickleave += l.Amount;
                                //        }
                                //    }
                                //}
                                allowancemodel.Amount = allowancemodel.Amount -
                                                        (allowancemodel.Amount / 30) * totalSickleaveAndabsent;

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
                                                dif.HasValue? dif.Value.TotalDays : 0;
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
                                                dif.HasValue? dif.Value.TotalDays : 0;
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
                            Threshold = ((decimal)(rule.Percentage / 100) * payroldata.BasicSalary);

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

                                    payroldata.NonTaxableAllowance = Convert.ToDecimal(allowancemodel.Amount);
                                    payroldata.TaxableAllowance = 0;
                                    payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                payroldata.TaxableAllowance;
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

                                    payroldata.NonTaxableAllowance = 0;
                                    payroldata.TaxableAllowance = allowancemodel.Amount;
                                    payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                payroldata.TaxableAllowance;
                                }
                                // old code////not useful
                                //else 
                                //{
                                //    if (allowancemodel.Amount > Threshold)
                                //    {
                                //        if (allowancemodel.Amount > rule.TaxTrasholdAmount && rule.TaxTrasholdAmount > 0)
                                //        {
                                //            allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //            allowancedata.Taxable = Convert.ToDecimal(allowancemodel.Amount) -
                                //                                    Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //            allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                                //            totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                //            totalNonTaxableAllowance = totalNonTaxableAllowance +
                                //                                       allowancedata.NonTaxable;
                                //            totalAllowance = totalAllowance + allowancedata.Total;

                                //            payroldata.NonTaxableAllowance = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //            payroldata.TaxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                //                                          Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //            payroldata.TotalAllowance = payroldata.NonTaxableAllowance +payroldata.TaxableAllowance;
                                //        }

                                //        else
                                //        {
                                //            allowancedata.NonTaxable = Threshold;
                                //            allowancedata.Taxable = Convert.ToDecimal(allowancemodel.Amount) - Threshold;
                                //            allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                                //            totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                //            totalNonTaxableAllowance = totalNonTaxableAllowance +
                                //                                       allowancedata.NonTaxable;
                                //            totalAllowance = totalAllowance + allowancedata.Total;

                                //            payroldata.NonTaxableAllowance = Threshold;
                                //            payroldata.TaxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                //                                          Threshold;
                                //            payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                //                                        payroldata.NonTaxableAllowance;


                                //        }
                                //    }
                                //    else
                                //    {
                                //        if (allowancemodel.Amount > rule.TaxTrasholdAmount && rule.TaxTrasholdAmount > 0)
                                //        {
                                //            allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //            allowancedata.Taxable = Convert.ToDecimal(allowancemodel.Amount) -
                                //                                    Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //            allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                                //            totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                //            totalNonTaxableAllowance = totalNonTaxableAllowance +
                                //                                       allowancedata.NonTaxable;
                                //            totalAllowance = totalAllowance + allowancedata.Total;

                                //            payroldata.NonTaxableAllowance = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //            payroldata.TaxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                //                                          Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //            payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                //                                        payroldata.TaxableAllowance;
                                //        }
                                //        else
                                //        {
                                //            allowancedata.NonTaxable = Convert.ToDecimal(allowancemodel.Amount);
                                //            allowancedata.Taxable = 0;
                                //            allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                                //            totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                //            totalNonTaxableAllowance = totalNonTaxableAllowance +
                                //                                       allowancedata.NonTaxable;
                                //            totalAllowance = totalAllowance + allowancedata.Total;

                                //            payroldata.NonTaxableAllowance = Convert.ToDecimal(allowancemodel.Amount);
                                //            payroldata.TaxableAllowance = 0;
                                //            payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                //                                        payroldata.NonTaxableAllowance;
                                //        }

                                //    }
                                //}
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

                                        payroldata.NonTaxableAllowance = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                        payroldata.TaxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                                                      Convert.ToDecimal(rule.TaxTrasholdAmount);
                                        payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                    payroldata.TaxableAllowance;
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

                                        payroldata.NonTaxableAllowance = Threshold;
                                        payroldata.TaxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                                                      Threshold;
                                        payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                    payroldata.NonTaxableAllowance;
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

                                        payroldata.NonTaxableAllowance = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                        payroldata.TaxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                                                      Convert.ToDecimal(rule.TaxTrasholdAmount);
                                        payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                    payroldata.TaxableAllowance;
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

                                        payroldata.NonTaxableAllowance = Convert.ToDecimal(allowancemodel.Amount);
                                        payroldata.TaxableAllowance = 0;
                                        payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                    payroldata.NonTaxableAllowance;
                                    }

                                }
                                //else
                                //{
                                //    #region maximum non taxable rule

                                //    if (allowancemodel.Amount > rule.TaxTrasholdAmount)
                                //    {
                                //        allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //        allowancedata.Taxable = Convert.ToDecimal(allowancemodel.Amount) -
                                //                                Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //        allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                                //        totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                //        totalNonTaxableAllowance = totalNonTaxableAllowance + allowancedata.NonTaxable;
                                //        totalAllowance = totalAllowance + allowancedata.Total;

                                //        payroldata.NonTaxableAllowance = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //        payroldata.TaxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                //                                      Convert.ToDecimal(rule.TaxTrasholdAmount);
                                //        payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                //                                    payroldata.TaxableAllowance;
                                //    }
                                //    else if (rule.TaxTrasholdAmount == 0 && rule.Percentage == 0)
                                //    {
                                //        allowancedata.NonTaxable = Convert.ToDecimal(allowancemodel.Amount);
                                //        allowancedata.Taxable = 0;
                                //        allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                                //        totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                //        totalNonTaxableAllowance = totalNonTaxableAllowance + allowancedata.NonTaxable;
                                //        totalAllowance = totalAllowance + allowancedata.Total;

                                //        payroldata.NonTaxableAllowance = Convert.ToDecimal(allowancemodel.Amount);
                                //        payroldata.TaxableAllowance = 0;
                                //        payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                //                                    payroldata.TaxableAllowance;
                                //    }
                                //    else
                                //    {
                                //        allowancedata.NonTaxable = Convert.ToDecimal(allowancemodel.Amount);
                                //        allowancedata.Taxable = 0;
                                //        allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                                //        totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                //        totalNonTaxableAllowance = totalNonTaxableAllowance + allowancedata.NonTaxable;
                                //        totalAllowance = totalAllowance + allowancedata.Total;

                                //        payroldata.NonTaxableAllowance = Convert.ToDecimal(allowancemodel.Amount);
                                //        payroldata.TaxableAllowance = 0;
                                //        payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                //                                    payroldata.TaxableAllowance;
                                //    }


                                //   
                                //}
                            }

                            #endregion

                            if (allowancemodel.AllowanceType == AllowanceType.HOME)
                            {
                                payroldata.TaxableHouseAllowance += payroldata.TaxableAllowance;
                                payroldata.NonTaxableHouseAllowance += payroldata.NonTaxableAllowance;
                                payroldata.TotalHouseAllowance += payroldata.TaxableAllowance +
                                                                  payroldata.NonTaxableAllowance;
                            }
                            if (allowancemodel.AllowanceType == AllowanceType.POSITION)
                            {
                                payroldata.TaxablePositionAllowance += payroldata.TaxableAllowance;
                                payroldata.NonTaxablePositionAllowance += payroldata.NonTaxableAllowance;
                                payroldata.TotalPositionAllowance += payroldata.TaxableAllowance +
                                                                     payroldata.NonTaxableAllowance;
                            }
                            if (allowancemodel.AllowanceType == AllowanceType.PROFESSIONAL)
                            {
                                payroldata.TaxableProfessionalAllowance += payroldata.TaxableAllowance;
                                payroldata.NonTaxableProfessionalAllowance += payroldata.NonTaxableAllowance;
                                payroldata.TotalProfessionalAllowance += payroldata.TaxableAllowance +
                                                                         payroldata.NonTaxableAllowance;
                            }
                            //if (allowancemodel.AllowanceType == AllowanceType.TOP_UP)
                            //{
                            //    payroldata.TaxableTopUpAllowance += payroldata.TaxableAllowance;
                            //    payroldata.NonTaxableTopUpAllowance += payroldata.NonTaxableAllowance;
                            //    payroldata.TotalTopUpAllowance += payroldata.TaxableAllowance +
                            //                                     payroldata.NonTaxableAllowance;
                            //}
                            if (allowancemodel.AllowanceType == AllowanceType.TRANSPORT)
                            {
                                payroldata.TaxableTransportAllowance += payroldata.TaxableAllowance;
                                payroldata.NonTaxableTransportAllowance += payroldata.NonTaxableAllowance;
                                payroldata.TotalTransportAllowance += payroldata.TaxableAllowance +
                                                                      payroldata.NonTaxableAllowance;
                            }
                            if (allowancemodel.AllowanceType == AllowanceType.TELEPHONE)
                            {
                                payroldata.TaxableTelephoneAllowance += payroldata.TaxableAllowance;
                                payroldata.NonTaxableTelephoneAllowance += payroldata.NonTaxableAllowance;
                                payroldata.TotalTelephoneAllowance += payroldata.TaxableAllowance +
                                                                      payroldata.NonTaxableAllowance;
                            }


                            if (allowancemodel.AllowanceType == AllowanceType.RESPONSIBILITY)
                            {
                                payroldata.TaxableResponsiblityAllowance += payroldata.TaxableAllowance;
                                payroldata.NonTaxableResponsiblityAllowance += payroldata.NonTaxableAllowance;
                                payroldata.TotalResponsiblityAllowance += payroldata.TaxableAllowance +
                                                                          payroldata.NonTaxableAllowance;
                            }
                            if (allowancemodel.AllowanceType == AllowanceType.DESERT)
                            {
                                payroldata.TaxableDesertAllowance += payroldata.TaxableAllowance;
                                payroldata.NonTaxableDesertAllowance += payroldata.NonTaxableAllowance;
                                payroldata.TotalDesertAllowance += payroldata.TaxableAllowance +
                                                                          payroldata.NonTaxableAllowance;
                            }
                            if (allowancemodel.AllowanceType == AllowanceType.PROJECT)
                            {
                                payroldata.TaxableProjectAllowance += payroldata.TaxableAllowance;
                                payroldata.NonTaxableProjectAllowance += payroldata.NonTaxableAllowance;
                                payroldata.TotalProjectAllowance += payroldata.TaxableAllowance +
                                                                          payroldata.NonTaxableAllowance;
                            }
                            #region Allowance Additions

                            db = new ExceedDbContext();
                            additiondata = new PayrollProcessAdditions();
                            additiondata.CreatedOn = DateTime.Now.Date;
                            additiondata.CreatedBy = User.Identity.Name;
                            //additiondata.EmployeeId = employee.PersonId;
                            additiondata.Name = allowancedata.Name;
                            //additiondata.Name = period;
                            additiondata.Amount = allowancedata.Taxable + allowancedata.NonTaxable;


                        }
                        payroldata.TaxableAllowance = totalTaxableAllowance;
                        payroldata.NonTaxableAllowance = totalNonTaxableAllowance;
                        payroldata.TotalAllowance = totalAllowance;

                        #endregion

                        #endregion

                        //get OT

                        #region additions

                        //get additions
                        var additions = db.Additions.Where(x => x.EmployeeId == employee.PersonId)
                            .Where(x => x.GLPeriodId == findPeriod.GLPeriodId).ToList();
                        decimal totalAdditions = 0;

                        foreach (Addition addition in additions)
                        {
                            additiondata = new PayrollProcessAdditions();
                            additiondata.CreatedOn = DateTime.Now.Date;
                            additiondata.CreatedBy = User.Identity.Name;
                            //additiondata.EmployeeId = employee.PersonId;
                            additiondata.Name =
                                lookups.Where(l => l.LookupId == addition.LookupId).FirstOrDefault().Description;
                            additiondata.AdditionId = addition.LookupId;
                            //additiondata.Name = period;
                            additiondata.Amount = addition.Amount;
                            totalAdditions = totalAdditions + additiondata.Amount;

                            if (addition.AdditionTaxType == AdditionTaxType.TAXABLE)
                            {
                                payroldata.TotalTaxAdditions += additiondata.Amount;
                            }
                            else
                            {
                                payroldata.TotalNonTaxAdditions += additiondata.Amount;
                            }


                        }
                        payroldata.TotalAdditions = totalAdditions;

                        #endregion

                        #region deductions

                        PayrollProcessDeductions deductiondata;

                        //get manual deductions
                        var deductions = db.EmpPayrollDeductions.Where(x => x.EmployeeId == employee.PersonId)
                            .Where(x => x.PeriodName == period).ToList();
                        decimal totalDeductions = 0;
                        foreach (EmpPayrollDeduction deduction in deductions)
                        {
                            deductiondata = new PayrollProcessDeductions();
                            deductiondata.CreatedOn = DateTime.Now.Date;
                            deductiondata.CreatedBy = User.Identity.Name;
                            //deductiondata.EmployeeId = employee.PersonId;
                            deductiondata.Name = deduction.DeductionType;
                            //deductiondata.Name = period;
                            deductiondata.Index = 2;
                            deductiondata.Amount = deduction.Amount;
                            totalDeductions = totalDeductions + deduction.Amount;

                        }
                        //get unpaid leave deductions
                        if (payroldata.UnpaidLeaveDays > 0 && DeductUnpaidLeave)
                        {
                            deductiondata = new PayrollProcessDeductions();
                            deductiondata.CreatedOn = DateTime.Now.Date;
                            deductiondata.CreatedBy = User.Identity.Name;
                            //deductiondata.EmployeeId = employee.PersonId;
                            deductiondata.Name = "Leave Without Pay Deduction";
                            deductiondata.Index = 3;
                            deductiondata.Amount = ((payroldata.BasicSalary * payroldata.UnpaidLeaveDays) / 30);
                            //can be divided by total working days
                           // totalDeductions = totalDeductions + deductiondata.Amount;

                            payroldata.UnpaidLeaveDeduction = deductiondata.Amount;

                        }
                        //get absent deductions
                        if (payroldata.DaysAbsent > 0 && !checkNewEmployeeState)
                        {
                            deductiondata = new PayrollProcessDeductions();
                            deductiondata.CreatedOn = DateTime.Now.Date;
                            deductiondata.CreatedBy = User.Identity.Name;
                            //deductiondata.EmployeeId = employee.PersonId;
                            deductiondata.Name = "Absent Deduction";
                            //deductiondata.Name = period;
                            deductiondata.Index = 4;
                            deductiondata.Amount = ((payroldata.BasicSalary * payroldata.DaysAbsent) / 30);
                            //can be divided by total working days
                           // totalDeductions = totalDeductions + deductiondata.Amount;
                            payroldata.AbsentDeduction = deductiondata.Amount;

                        }
                        if (checkNewEmployeeState)
                        {
                            payroldata.DaysAbsent = 30 - (decimal)WorkingdateDiff;
                        }

                         payroldata.TotalDeductions = totalDeductions;//enabled
                        //payroldata.AbsentDeduction = totalDeductions;//commented 


                        #endregion

                        #region get long term deduction

                        //get long term deduction
                        var longtermDeductions =
                            db.EmpPayrollLongTermDeductions.Where(x => x.EmployeeId == employee.PersonId).ToList();
                        decimal totalLongtermDeductions = 0;
                        PayrollProcessLongTermDeductions longtermdata;
                        foreach (EmpPayrollLongTermDeduction longterm in longtermDeductions)
                        {
                            if (longterm.Balance > 0)
                            {
                                longtermdata = new PayrollProcessLongTermDeductions();
                                longtermdata.CreatedOn = DateTime.Now.Date;
                                longtermdata.CreatedBy = User.Identity.Name;
                                longtermdata.EmpPayrollLongTermDeductionId = longterm.EmpPayrollLongTermDeductionId;
                                //longtermdata.EmployeeId = employee.PersonId;
                                longtermdata.Name = longterm.DeductionType;
                                //longtermdata.Name = period;

                                if (longterm.Balance <= longterm.MonthlyAmount)
                                {
                                    longtermdata.Amount = longterm.Balance;
                                    longtermdata.Outstanding = 0;
                                }
                                else
                                {
                                    longtermdata.Amount = longterm.MonthlyAmount;
                                    longtermdata.Outstanding = longterm.Balance - longterm.MonthlyAmount;
                                }
                                totalLongtermDeductions = totalLongtermDeductions + longtermdata.Amount;
                                //update loan info
                                if (longterm.Balance <= longterm.MonthlyAmount)
                                {
                                    longterm.Balance = 0;
                                }
                                else
                                {
                                    longterm.Balance = longterm.Balance - longterm.MonthlyAmount;
                                }
                                db.Entry(longterm).State = EntityState.Modified;

                                #region LoangTerm Deduction

                                db = new ExceedDbContext();
                                deductiondata = new PayrollProcessDeductions();
                                deductiondata.CreatedOn = DateTime.Now.Date;
                                deductiondata.CreatedBy = User.Identity.Name;
                                //deductiondata.EmployeeId = employee.PersonId;
                                deductiondata.Name = longtermdata.Name;
                                deductiondata.OutstandingAmount = longtermdata.Outstanding;
                                deductiondata.Amount = longtermdata.Amount;
                                deductiondata.Index = 5;

                                #endregion
                            }
                        }
                        payroldata.TotalLongTermDeductions = totalLongtermDeductions;

                        #endregion

                        #region get Penality

                        //get long term deduction
                        var Allpenalities =
                            db.DisciplineMeasuerByMoney.Where(x => x.EmployeeId == employee.PersonId).ToList();
                        decimal totalpenalityAmt = 0;
                        List<DisciplineMeasuerByMoney> penalities = new List<DisciplineMeasuerByMoney>();
                        foreach (var p in Allpenalities)
                        {
                            if (p.GLPeriod.DateFrom.Date >= findPeriod.DateFrom &&
                                p.GLPeriod.DateTo.Date <= findPeriod.DateTo)
                            {
                                penalities.Add(p);
                            }

                        }
                        DisciplineMeasuerByMoney PenalityData;
                        PayrollProcessDeductions Penality;
                        foreach (DisciplineMeasuerByMoney longterm in penalities)
                        {
                            Penality = new PayrollProcessDeductions();
                            if (longterm.Balance > 0)
                            {
                                PenalityData = new DisciplineMeasuerByMoney();
                                longterm.LastModified = DateTime.Now.Date;
                                longterm.ModifiedBy = User.Identity.Name;
                                PenalityData.DisciplineMeasuerByMoneyId = longterm.DisciplineMeasuerByMoneyId;
                                PenalityData.EmployeeId = longterm.EmployeeId;
                                //PenalityData.GLPeriodId = longterm.GLPeriodId;                             

                                if (longterm.Balance <= longterm.MonthlyAmount)
                                {
                                    longterm.Amount = longterm.Balance;
                                    Penality.OutstandingAmount = 0;
                                }
                                else
                                {
                                    longterm.Amount = longterm.MonthlyAmount;
                                    Penality.OutstandingAmount = longterm.Balance - longterm.MonthlyAmount;
                                }
                                totalpenalityAmt = totalpenalityAmt + longterm.Amount;
                                //update loan info
                                if (longterm.Balance <= longterm.MonthlyAmount)
                                {
                                    longterm.Balance = 0;
                                }
                                else
                                {
                                    longterm.Balance = longterm.Balance - longterm.MonthlyAmount;
                                }
                                db.Entry(longterm).State = EntityState.Modified;

                                #region Discipline Deduction

                                db = new ExceedDbContext();
                                deductiondata = new PayrollProcessDeductions();
                                deductiondata.CreatedOn = DateTime.Now.Date;
                                deductiondata.CreatedBy = User.Identity.Name;
                                deductiondata.Name = "Discipline Penality";
                                deductiondata.Remark = "Discipline";
                                deductiondata.OutstandingAmount = Penality.OutstandingAmount;
                                deductiondata.Amount = longterm.Amount;
                                deductiondata.Index = 11;

                                #endregion
                            }
                        }
                        payroldata.TotalPenality = totalpenalityAmt;

                        #endregion

                        #region get loans

                        //get loans
                        var loans = db.Loans.Where(x => x.EmployeeId == employee.PersonId && x.IsActive).ToList();
                        decimal totalLoans = 0;
                        PayrollProcessLoans loandata;
                        foreach (Loan loan in loans)
                        {
                            if (!(jumploan && !loan.JumpHoliday))
                            {
                                if (loan.Balance > 0)
                                {
                                    loandata = new PayrollProcessLoans();
                                    loandata.CreatedOn = DateTime.Now.Date;
                                    loandata.CreatedBy = User.Identity.Name;
                                    loandata.LoanId = loan.LoanId;
                                    //loandata.EmployeeId = employee.PersonId;
                                    loandata.Name = loan.Description;
                                    //loandata.Name = period;
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
                                    //  db.Entry(loan).State = EntityState.Modified;

                                    #region LoanDeduction

                                    db = new ExceedDbContext();
                                    deductiondata = new PayrollProcessDeductions();
                                    deductiondata.CreatedOn = DateTime.Now.Date;
                                    deductiondata.CreatedBy = User.Identity.Name;
                                    //deductiondata.EmployeeId = employee.PersonId;
                                    deductiondata.Name = "Loan";
                                    deductiondata.OutstandingAmount = loandata.Outstanding;
                                    deductiondata.Amount = loandata.Amount;
                                    deductiondata.Index = 6;

                                    #endregion
                                }
                            }
                        }
                        payroldata.TotalLoan = totalLoans;

                        #endregion

                        #region get advances

                        //get advances
                        var advances = db.Advances.Where(x => x.EmployeeId == employee.PersonId && x.IsActive).ToList();
                        decimal totalAdvances = 0;
                        PayrollProcessAdvances advancedata;
                        foreach (Advance advance in advances)
                        {
                            if (!(jumploan && !advance.JumpHoliday))
                            {
                                if (advance.Balance > 0)
                                {
                                    advancedata = new PayrollProcessAdvances();
                                    advancedata.CreatedOn = DateTime.Now.Date;
                                    advancedata.CreatedBy = User.Identity.Name;
                                    advancedata.AdvanceId = advance.AdvanceId;
                                    //advancedata.EmployeeId = employee.PersonId;
                                    advancedata.Name = advance.Description;
                                    //advancedata.Name = period;
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
                                    //db.Entry(advance).State = EntityState.Modified;


                                    #region Advance Deduction

                                    db = new ExceedDbContext();
                                    deductiondata = new PayrollProcessDeductions();
                                    deductiondata.CreatedOn = DateTime.Now.Date;
                                    deductiondata.CreatedBy = User.Identity.Name;
                                    //deductiondata.EmployeeId = employee.PersonId;
                                    deductiondata.Name = advancedata.Name;
                                    deductiondata.OutstandingAmount = advancedata.Outstanding;
                                    deductiondata.Amount = advancedata.Amount;
                                    deductiondata.Index = 7;

                                    #endregion
                                }
                            }
                        }
                        payroldata.TotalAdvance = totalAdvances;

                        #endregion

                        #region get regular contribution

                        var contributins = db.ContributionTypes;
                        //  decimal RedCross = 0;
                        //get contribution
                        var contributions =
                            db.RegularContributions.Where(x => x.EmployeeId == employee.PersonId)
                                .Where(x => x.IsActive == true)
                                .ToList();
                        decimal totalContributions = 0;
                        PayrollProcessRegularContribution regularcontributiondata;
                        foreach (RegularContribution contribution in contributions)
                        {
                            decimal regularcontributionAmount = 0;
                            regularcontributiondata = new PayrollProcessRegularContribution();
                            regularcontributiondata.CreatedOn = DateTime.Now.Date;
                            regularcontributiondata.CreatedBy = User.Identity.Name;
                            regularcontributiondata.RegContributionId = contribution.RegContributionId;
                            var onecontr = contributins.Where(l => l.ContributionTypeId == contribution.ContributionTypeId).FirstOrDefault();
                            if (onecontr != null)
                            {
                                #region calculate contribution amount
                                if (contribution.NetGross == FixedContAmtType.FROM_NET)
                                {
                                    decimal Pension = payroldata.BasicSalary * Convert.ToDecimal((EmployeeContribution / 100));
                                    decimal beforeIncome = (decimal) XAPI.IncomeTax.GetIncomeTax((float)payroldata.BasicSalary);
                                    regularcontributionAmount = contribution.AdditionalAmount + ((payroldata.BasicSalary - Pension - beforeIncome) * (contribution.Amount / 100));

                                }
                                else
                                {
                                    regularcontributionAmount = contribution.AdditionalAmount + (payroldata.BasicSalary * contribution.Amount / 100);
                                }
                                #endregion
                                regularcontributiondata.Name = onecontr.Description;
                                //regularcontributiondata.Name = period;
                                regularcontributiondata.CostCenterAccountCode = payroldata.CostCenterAccountCode;
                                regularcontributiondata.OrgBranchName = payroldata.OrgBranchAccountCode;
                                regularcontributiondata.Amount = regularcontributionAmount;//new 
                                //regularcontributiondata.Amount = contribution.AdditionalAmount +
                                //                                 (payroldata.BasicSalary * contribution.Amount / 100);
                                totalContributions = totalContributions + regularcontributiondata.Amount;
                                if (onecontr.Description == "Red Cross")
                                {
                                    payroldata.RedCross = payroldata.RedCross + regularcontributiondata.Amount;
                                }

                                #region Regular Contribution Deduction

                                db = new ExceedDbContext();
                                deductiondata = new PayrollProcessDeductions();
                                deductiondata.CreatedOn = DateTime.Now.Date;
                                deductiondata.CreatedBy = User.Identity.Name;
                                //deductiondata.EmployeeId = employee.PersonId;
                                deductiondata.Name = regularcontributiondata.Name;
                                //deductiondata.Name = period;
                                deductiondata.Amount = regularcontributiondata.Amount;
                                deductiondata.Index = 9;

                                #endregion
                            }
                        }
                        payroldata.TotalRegularContribution = totalContributions;

                        #endregion

                        #region get SportFee

                        //get credits
                        var sporsts =
                            db.Associations.Where(x => x.EmployeeId == employee.PersonId)
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
                                               (payroldata.BasicSalary * credit.MonthlyAmount / 100);
                                    oneSportFee = credit.AdditionalAmount +
                                                  (payroldata.BasicSalary * credit.MonthlyAmount / 100);
                                }
                                if (credit.NetGross == FixedContAmtType.FROM_NET)
                                {
                                    decimal Pension = payroldata.BasicSalary *
                                                      Convert.ToDecimal((EmployeeContribution / 100));
                                    decimal beforeIncome =
                                        (decimal)XAPI.IncomeTax.GetIncomeTax((float)payroldata.BasicSalary);
                                    SportFee = SportFee + credit.AdditionalAmount +
                                               ((payroldata.BasicSalary - Pension - beforeIncome) *
                                                (credit.MonthlyAmount / 100));
                                    oneSportFee = credit.AdditionalAmount +
                                                  ((payroldata.BasicSalary - Pension - beforeIncome) *
                                                   (credit.MonthlyAmount / 100));
                                }
                            }
                            else
                            {
                                SportFee = SportFee + credit.Amount;
                                oneSportFee = credit.Amount;
                            }


                            #region Assocation Contribution Deduction

                            db = new ExceedDbContext();
                            deductiondata = new PayrollProcessDeductions();
                            deductiondata.CreatedOn = DateTime.Now.Date;
                            deductiondata.CreatedBy = User.Identity.Name;
                            //deductiondata.EmployeeId = employee.PersonId;
                            deductiondata.Name = credit.Type.ToString();
                            //deductiondata.Name = period;
                            deductiondata.Amount = oneSportFee; //credit.AdditionalAmount +
                            //(payroldata.BasicSalary * credit.MonthlyAmount / 100);
                            deductiondata.Index = 12;

                            #endregion
                        }
                        payroldata.SportFee = SportFee;

                        #endregion

                        #region get fixed contribution

                        //get fixed contribution
                        var fixedcontributions =
                            db.FixedContributions.Where(x => x.EmployeeId == employee.PersonId && x.IsActive).ToList();
                        decimal totalFixedcontribution = 0;
                        decimal OneFixedcontribution = 0;
                        PayrollProcessFixedContribution fixedcontributiondata;
                        foreach (FixedContribution fixedcontribution in fixedcontributions)
                        {
                            if (fixedcontribution.Balance > 0)
                            {
                                fixedcontributiondata = new PayrollProcessFixedContribution();
                                fixedcontributiondata.CreatedOn = DateTime.Now.Date;
                                fixedcontributiondata.CreatedBy = User.Identity.Name;
                                fixedcontributiondata.FixedContributionId = fixedcontribution.FixedContributionId;
                                //fixedcontributiondata.EmployeeId = employee.PersonId;



                                if (fixedcontribution.Balance > 0)
                                {
                                    if (fixedcontribution.NetGross == FixedContAmtType.FROM_NET)
                                    {
                                        if (fixedcontribution.AmtPercnet == FixedContPecntType.PERCENT)
                                        {
                                            decimal Pension = payroldata.BasicSalary *
                                                              Convert.ToDecimal((EmployeeContribution / 100));
                                            decimal beforeIncome =
                                                (decimal)
                                                    XAPI.IncomeTax.GetIncomeTax((float)payroldata.BasicSalary);
                                            OneFixedcontribution = ((payroldata.BasicSalary - Pension - beforeIncome -
                                                                     payroldata.RedCross - payroldata.SportFee) *
                                                                    (fixedcontribution.AmountPercent / 100)) /
                                                                   fixedcontribution.Months;
                                        }
                                        else
                                        {

                                            OneFixedcontribution = (fixedcontribution.Amount) /
                                                                   fixedcontribution.Months;
                                        }
                                    }
                                    else
                                    {
                                        if (fixedcontribution.AmtPercnet == FixedContPecntType.PERCENT)
                                        {
                                            OneFixedcontribution = payroldata.BasicSalary *
                                                                   (fixedcontribution.AmountPercent / 100) /
                                                                   fixedcontribution.Months;
                                        }
                                        else
                                        {
                                            OneFixedcontribution = (fixedcontribution.Amount) /
                                                                   fixedcontribution.Months;
                                        }
                                    }
                                }
                                var onecontr =
                                    contributins.Where(l => l.ContributionTypeId == fixedcontribution.ContributionTypeId)
                                        .FirstOrDefault();
                                if (onecontr != null)
                                {
                                    fixedcontributiondata.Name = onecontr.Description;
                                    fixedcontributiondata.CostCenterAccountCode = payroldata.CostCenterAccountCode;
                                    fixedcontributiondata.OrgBranchName = payroldata.OrgBranchAccountCode;
                                    if (fixedcontribution.Balance <= fixedcontribution.MonthlyAmount)
                                    {
                                        fixedcontributiondata.Amount = fixedcontribution.Balance;
                                        fixedcontributiondata.Outstanding = 0;
                                    }
                                    else
                                    {
                                        fixedcontributiondata.Amount = OneFixedcontribution;
                                        ;
                                        fixedcontributiondata.Outstanding = fixedcontribution.Balance -
                                                                            OneFixedcontribution;
                                        ;
                                    }
                                    totalFixedcontribution = totalFixedcontribution + fixedcontributiondata.Amount;
                                    //update fixed contribution info
                                    if (fixedcontribution.Balance <= OneFixedcontribution)
                                    {
                                        fixedcontribution.Balance = 0;
                                    }
                                    else
                                    {
                                        fixedcontribution.Balance = fixedcontribution.Balance -
                                                                    OneFixedcontribution;
                                        ;
                                    }
                                    // db.Entry(fixedcontribution).State = EntityState.Modified;


                                    #region Fixed Contribution Deduction

                                    db = new ExceedDbContext();
                                    deductiondata = new PayrollProcessDeductions();
                                    deductiondata.CreatedOn = DateTime.Now.Date;
                                    deductiondata.CreatedBy = User.Identity.Name;
                                    //deductiondata.EmployeeId = employee.PersonId;
                                    deductiondata.Name = fixedcontributiondata.Name;
                                    deductiondata.OutstandingAmount = fixedcontributiondata.Outstanding;
                                    deductiondata.Amount = fixedcontributiondata.Amount;
                                    deductiondata.Index = 8;

                                    #endregion
                                }
                            }
                        }
                        payroldata.TotalFixedContribution = totalFixedcontribution;

                        #endregion


                        #region get credits

                        //get credits
                        var credits =
                            db.Associations.Where(x => x.EmployeeId == employee.PersonId)
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
                                                   (payroldata.BasicSalary * credit.MonthlyAmount / 100);
                                    oneCredits = credit.AdditionalAmount +
                                                 (payroldata.BasicSalary * credit.MonthlyAmount / 100);
                                }
                                if (credit.NetGross == FixedContAmtType.FROM_NET)
                                {
                                    decimal Pension = payroldata.BasicSalary *
                                                      Convert.ToDecimal((EmployeeContribution / 100));
                                    decimal beforeIncome =
                                        (decimal)XAPI.IncomeTax.GetIncomeTax((float)(payroldata.BasicSalary));
                                    totalCredits = totalCredits + credit.AdditionalAmount +
                                                   ((payroldata.BasicSalary - Pension - beforeIncome) *
                                                    (credit.MonthlyAmount / 100));
                                    oneCredits = credit.AdditionalAmount +
                                                 ((payroldata.BasicSalary - Pension - beforeIncome) *
                                                  (credit.MonthlyAmount / 100));
                                }
                            }
                            else
                            {
                                totalCredits = totalCredits + credit.Amount;
                                oneCredits = credit.Amount;
                            }

                            #region Assocation Contribution Deduction

                            db = new ExceedDbContext();
                            deductiondata = new PayrollProcessDeductions();
                            deductiondata.CreatedOn = DateTime.Now.Date;
                            deductiondata.CreatedBy = User.Identity.Name;
                            //deductiondata.EmployeeId = employee.PersonId;
                            deductiondata.Name = credit.Type.ToString();
                            //deductiondata.Name = period;
                            deductiondata.Amount = oneCredits; //credit.AdditionalAmount +
                            //(payroldata.BasicSalary*credit.MonthlyAmount/100);
                            deductiondata.Index = 10;

                            #endregion
                        }
                        payroldata.AssocationContribution = totalCredits;

                        #endregion

                        #region get labour union

                        ////get credits
                        var labourunions =
                            db.Associations.Where(x => x.EmployeeId == employee.PersonId)
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
                                                  (payroldata.BasicSalary * labour.MonthlyAmount / 100);
                                    oneLabour = labour.AdditionalAmount +
                                                (payroldata.BasicSalary * labour.MonthlyAmount / 100);
                                }
                                if (labour.NetGross == FixedContAmtType.FROM_NET)
                                {
                                    decimal Pension = payroldata.BasicSalary *
                                                      Convert.ToDecimal((EmployeeContribution / 100));
                                    decimal beforeIncome =
                                        (decimal)XAPI.IncomeTax.GetIncomeTax((float)payroldata.BasicSalary);
                                    totalLabour = totalLabour + labour.AdditionalAmount +
                                                  ((payroldata.BasicSalary - Pension - beforeIncome) *
                                                   (labour.MonthlyAmount / 100));
                                    oneLabour = labour.AdditionalAmount +
                                                ((payroldata.BasicSalary - Pension - beforeIncome) *
                                                 (labour.MonthlyAmount / 100));
                                }
                            }
                            else
                            {
                                totalLabour = totalLabour + labour.Amount;
                                oneLabour = labour.Amount;
                            }


                            #region Assocation Contribution Deduction

                            db = new ExceedDbContext();
                            deductiondata = new PayrollProcessDeductions();
                            deductiondata.CreatedOn = DateTime.Now.Date;
                            deductiondata.CreatedBy = User.Identity.Name;
                            //deductiondata.EmployeeId = employee.PersonId;
                            deductiondata.Name = labour.Type.ToString();
                            //deductiondata.Name = period;
                            deductiondata.Amount = oneLabour; //labour.AdditionalAmount +
                            // (payroldata.BasicSalary*labour.MonthlyAmount/100);
                            deductiondata.Index = 10;

                            #endregion
                        }
                        payroldata.LabourUnionContribution = totalLabour;

                        #endregion

                        #region get service charge

                        //get service charge
                        var servicecharges =
                            db.EmpPayrollServiceChargeViewModel.Where(x => x.EmployeeId == employee.PersonId)
                                .Where(x => x.PeriodName == period).ToList();
                        decimal totalServiceCharge = 0;
                        foreach (EmpPayrollServiceChargeViewModel servicecharge in servicecharges)
                        {
                            totalServiceCharge = totalServiceCharge + servicecharge.Amount;
                        }
                        payroldata.ServiceCharge = totalServiceCharge;

                        #endregion

                        payroldata.GrossNonTaxableEarning = payroldata.NonTaxableAllowance +
                                                            payroldata.TotalNonTaxAdditions;
                        payroldata.GrossTaxableEarning = payroldata.BasicSalary
                                                         + payroldata.TaxableAllowance
                                                         + payroldata.TotalOvertime
                                                         + payroldata.TotalTaxAdditions
                                                         + payroldata.ServiceCharge
                                                         - payroldata.AbsentDeduction;
                                                         

                        payroldata.GrossEarning = payroldata.GrossNonTaxableEarning + payroldata.GrossTaxableEarning;
                        payroldata.GrossDeduction = payroldata.TotalDeductions
                                                    + payroldata.TotalLongTermDeductions
                                                    + payroldata.TotalLoan
                                                    + payroldata.SportFee
                                                    + payroldata.LabourUnionContribution
                                                    + payroldata.TotalPenality
                                                    + payroldata.TotalAdvance
                                                    + payroldata.TotalFixedContribution
                                                    + payroldata.AssocationContribution
                                                    + payroldata.TotalRegularContribution
                                                    + payroldata.AbsentDeduction
                                                    + payroldata.UnpaidLeaveDeduction;//added

                        #region get Provident Fund and Pension

                        #region get contract pension rule

                        bool enableContractpension = true;
                        var payrollDisableContractpension = db.Configurations.Where(x =>
                                x.Reference ==
                                PayrollSettingCategories.DISABLE_PENSION_FOR_CONTRACT.ToString())
                            .FirstOrDefault();
                        if (payrollDisableContractpension != null)
                        {
                            if (payrollDisableContractpension.Attribute.ToUpper() == "TRUE")
                                enableContractpension = false;
                        }

                        if (employee.Status == EmployeeStatus.PERMANENT || employee.Status == EmployeeStatus.PROBATION)
                            enableContractpension = true;

                        #endregion

                        //employee.Age = findPeriod.DateFrom.Year - employee.DateOfBirth.Year;
                        if (employee.Age < RetirementAge && enableContractpension)
                        {
                            var newPlacmenet =
                                db.Placements.Where(
                                    x =>
                                        x.AssignmentType == AssignmentType.NEW && x.isCurrent &&
                                        x.EmployeeId == employee.PersonId).FirstOrDefault();
                            int dateDiff = ProbationDateLenghth + 1;
                            if (newPlacmenet != null)
                            {
                                if (newPlacmenet.AssignmentDate.Date <= findPeriod.DateFrom.Date)
                                    dateDiff = Convert.ToInt32((findPeriod.DateFrom.Date - newPlacmenet.AssignmentDate.Date).TotalDays) + 30;//30 working days
                                else if (newPlacmenet.AssignmentDate.Date > findPeriod.DateFrom.Date && newPlacmenet.AssignmentDate.Date <= findPeriod.DateTo.Date)
                                    dateDiff = 30 - newPlacmenet.AssignmentDate.Date.Day + 1;
                                else
                                {
                                    dateDiff = 0;
                                }
                            }
                            if ((dateDiff > ProbationDateLenghth) && ApplyProbationSetting)
                            {
                                var checkExistance =
                                    db.ProvidentFunds.Where(x => x.EmployeeId == payroldata.EmployeeId).FirstOrDefault();
                                if (checkExistance != null)
                                {
                                    #region get Provident Fund

                                    if (checkExistance.IsMemeber)
                                    {

                                        var idnumber =
                                            Providentifications.Where(
                                                x =>
                                                    x.Reference == payroldata.EmployeeId.ToString() &&
                                                    x.ReferenceType == "Employee").FirstOrDefault();
                                        if (idnumber != null)
                                        {
                                            payroldata.ProvidentNumber = idnumber.IdNumber;
                                        }
                                        payroldata.ProvidentFromEmployee = payroldata.BasicSalary *
                                                                           (ProvEmployeeContribution /
                                                                            100);
                                        payroldata.ProvidentFromCompany = payroldata.BasicSalary *
                                                                          (ProvCompanyContribution /
                                                                           100);
                                        payroldata.TotalProvident = payroldata.ProvidentFromCompany +
                                                                    payroldata.ProvidentFromEmployee;
                                    }
                                    else
                                    {
                                        #region get  pension

                                        ////get pension

                                        var idnumber =
                                            identifications.Where(
                                                x =>
                                                    x.Reference == payroldata.EmployeeId.ToString() &&
                                                    x.ReferenceType == "Employee").FirstOrDefault();
                                        if (idnumber != null)
                                        {
                                            payroldata.PensionNumber = idnumber.IdNumber;
                                        }
                                        payroldata.PensionFromEmployee = payroldata.BasicSalary *
                                                                         (EmployeeContribution / 100);
                                        payroldata.PensionFromCompany = payroldata.BasicSalary * (CompanyContribution / 100);
                                        payroldata.TotalPension = payroldata.PensionFromCompany +
                                                                  payroldata.PensionFromEmployee;

                                        #endregion
                                    }

                                    #endregion
                                }
                                else
                                {

                                    #region get  pension

                                    ////get pension

                                    var idnumber =
                                        identifications.Where(
                                            x =>
                                                x.Reference == payroldata.EmployeeId.ToString() &&
                                                x.ReferenceType == "Employee").FirstOrDefault();
                                    if (idnumber != null)
                                    {
                                        payroldata.PensionNumber = idnumber.IdNumber;
                                    }
                                    payroldata.PensionFromEmployee = payroldata.BasicSalary * (EmployeeContribution / 100);
                                    payroldata.PensionFromCompany = payroldata.BasicSalary * (CompanyContribution / 100);
                                    payroldata.TotalPension = payroldata.PensionFromCompany +
                                                              payroldata.PensionFromEmployee;

                                    #endregion
                                }
                            }
                            else if (!ApplyProbationSetting)
                            {
                                var checkExistance =
                                    db.ProvidentFunds.Where(x => x.EmployeeId == payroldata.EmployeeId)
                                        .FirstOrDefault();
                                if (checkExistance != null)
                                {
                                    #region get Provident Fund

                                    if (checkExistance.IsMemeber)
                                    {

                                        var idnumber =
                                            Providentifications.Where(
                                                x =>
                                                    x.Reference == payroldata.EmployeeId.ToString() &&
                                                    x.ReferenceType == "Employee").FirstOrDefault();
                                        if (idnumber != null)
                                        {
                                            payroldata.ProvidentNumber = idnumber.IdNumber;
                                        }
                                        payroldata.ProvidentFromEmployee = payroldata.BasicSalary *
                                                                           ProvEmployeeContribution /
                                                                           100;
                                        payroldata.ProvidentFromCompany = payroldata.BasicSalary *
                                                                          ProvCompanyContribution /
                                                                          100;
                                        payroldata.TotalProvident = payroldata.ProvidentFromCompany +
                                                                    payroldata.ProvidentFromEmployee;
                                    }
                                    else
                                    {
                                        #region get  pension

                                        ////get pension

                                        var idnumber =
                                            identifications.Where(
                                                x =>
                                                    x.Reference == payroldata.EmployeeId.ToString() &&
                                                    x.ReferenceType == "Employee").FirstOrDefault();
                                        if (idnumber != null)
                                        {
                                            payroldata.PensionNumber = idnumber.IdNumber;
                                        }
                                        payroldata.PensionFromEmployee = payroldata.BasicSalary *
                                                                         EmployeeContribution /
                                                                         100;
                                        payroldata.PensionFromCompany = payroldata.BasicSalary *
                                                                        CompanyContribution /
                                                                        100;
                                        payroldata.TotalPension = payroldata.PensionFromCompany +
                                                                  payroldata.PensionFromEmployee;

                                        #endregion
                                    }

                                    #endregion
                                }
                                else
                                {

                                    #region get  pension

                                    ////get pension

                                    var idnumber =
                                        identifications.Where(
                                            x =>
                                                x.Reference == payroldata.EmployeeId.ToString() &&
                                                x.ReferenceType == "Employee").FirstOrDefault();
                                    if (idnumber != null)
                                    {
                                        payroldata.PensionNumber = idnumber.IdNumber;
                                    }
                                    payroldata.PensionFromEmployee = payroldata.BasicSalary *
                                                                     (EmployeeContribution / 100);
                                    payroldata.PensionFromCompany = payroldata.BasicSalary *
                                                                    (CompanyContribution / 100);
                                    payroldata.TotalPension = payroldata.PensionFromCompany +
                                                              payroldata.PensionFromEmployee;

                                    #endregion
                                }
                            }
                        }

                        #endregion

                        #region get tax

                        //get tax
                        payroldata.IncomeTax =
                            (decimal)XAPI.IncomeTax.GetIncomeTax((float)(payroldata.GrossTaxableEarning));
                        payroldata.NetEarning = payroldata.GrossTaxableEarning
                                                - payroldata.IncomeTax
                                                + payroldata.GrossNonTaxableEarning
                                                - payroldata.PensionFromEmployee
                                                - payroldata.ProvidentFromEmployee
                                                - payroldata.GrossDeduction
                                                + payroldata.AbsentDeduction;


                        #endregion

                        #region IncomeTax Deduction

                        db = new ExceedDbContext();
                        deductiondata = new PayrollProcessDeductions();
                        deductiondata.CreatedOn = DateTime.Now.Date;
                        deductiondata.CreatedBy = User.Identity.Name;
                        //deductiondata.EmployeeId = employee.PersonId;
                        deductiondata.Name = "Income Tax";
                        //deductiondata.Name = period;
                        deductiondata.Amount = payroldata.IncomeTax;
                        deductiondata.Index = 0;

                        #endregion

                        #region Pension Contribution Deduction

                        db = new ExceedDbContext();
                        deductiondata = new PayrollProcessDeductions();
                        deductiondata.CreatedOn = DateTime.Now.Date;
                        deductiondata.CreatedBy = User.Identity.Name;
                        //deductiondata.EmployeeId = employee.PersonId;
                        deductiondata.Name = "Pension/Provident";
                        //deductiondata.Name = period;
                        deductiondata.Amount = payroldata.PensionFromEmployee + payroldata.ProvidentFromEmployee;
                        deductiondata.Index = 1;

                        #endregion



                        payroldata.Type = employee.Status;

                        db.PayrollProcessViewModel.Add(payroldata);
                        db.SaveChanges();

                    }

                }
            }

            catch (Exception e)
            {
               Logger.Error("Messsage: "+e.Message+ "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
                return 0;
                // throw;
            }
            // RedirectToAction("Index");
            var datas = db.PayrollProcessViewModel.OrderByDescending(x => x.PayrollProcessViewModelId).FirstOrDefault();
            if (datas != null)
            {
                return datas.PayrollProcessViewModelId;
            }
            else
                return 0;

        }
        
        private decimal GetemployeeUnpaidLeaveDays(int EmpId,int period)
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
                if(unpaid.From< periodObj.DateFrom)
                {
                    if(unpaid.To < periodObj.DateTo)
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

        private decimal GetEmployeeSickLeave(int EmpId, int period, int daysinperiod)
        {
            decimal days = 0;

            var periodObj = db.GLPeriods.Where(x => x.GLPeriodId == period)
                           .FirstOrDefault();

            var employeeSickLeaves = db.Leaves
                .Where(x => x.LeaveType == LeaveType.SICK_LEAVE)
                .Where(x => x.EmployeeId == EmpId)
                .Where(x => (x.From >= periodObj.DateFrom && x.From <= periodObj.DateTo) || (x.To >= periodObj.DateFrom && x.To <= periodObj.DateTo)).ToList();

            foreach (var sickLeave in employeeSickLeaves)
            {
                if (sickLeave.From < periodObj.DateFrom)
                {
                    if (sickLeave.To < periodObj.DateTo)
                        days = days + (sickLeave.To - periodObj.DateFrom).Days + 1;
                    else
                        days = days + (periodObj.DateTo - periodObj.DateFrom).Days + 1;
                }
                else
                {
                    if (sickLeave.To < periodObj.DateTo)
                        days = days + (sickLeave.To - sickLeave.From).Days + 1;
                    else
                        days = days + (periodObj.DateTo - sickLeave.From).Days + 1;
                }

            }

            if (days > daysinperiod)
                days = daysinperiod;
            return days;
        }

        private void LoadPayrollSettings()
        {
            #region get unpaid leave rule

           
            var payrollDisableContractpension = db.Configurations.Where(x =>
                    x.Reference ==
                    PayrollSettingCategories.DEDUCT_LEAVE_WITHOUT_PAYMENT.ToString())
                .FirstOrDefault();
            if (payrollDisableContractpension != null)
            {
                if (payrollDisableContractpension.Attribute.ToUpper() == "TRUE")
                    DeductUnpaidLeave = true;
            }

           

            #endregion
        }

    }

}