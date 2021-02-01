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
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    [Authorize(Roles = Payroll_Admin + "," + Payroll_Process + "," + Payroll_View + "," + Payroll_Distribute + "," + Payroll_Close + "," + Payroll_Transfer)]
    public partial class PayrollProcessController : BaseController
    {

        public ActionResult Index()
        {

            lookups = new ListStack<Lookup>();
            lookups = db.Lookups.ToList();
            GetPeriod();
            GetBranch();
            PrepareEmployeeAccount();

            return View();


        }
        public ActionResult GetBranch()
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var branches = db.Branch.Where(x => mybraches.Contains(x.BranchId.ToString()));
            ViewData["branches"] = branches;
            return Json(branches.Select(p => new { BranchId = p.BranchId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }
        private void SetForAllChild()
        {
            var branchCostCenters = db.BranchCostCenter.ToList();
            foreach (var branchCostCenter in branchCostCenters)
            {

                var mainStructures =
                    db.MainCompanyStructures.Where(x => x.Parent == branchCostCenter.OrgStructureId).ToList();
                foreach (var m in mainStructures)
                {
                    var check =
                        branchCostCenters.Where(
                            x => x.OrgStructureId == m.OrgStructureId && x.BranchId == branchCostCenter.BranchId)
                            .FirstOrDefault();
                    if (check == null)
                    {

                        var entity = new BranchCostCenter
                        {
                            BranchId = branchCostCenter.BranchId,
                            Values = branchCostCenter.Values,
                            OrgStructureId = m.OrgStructureId,
                            Remark = "System"
                        };
                        db = new ExceedDbContext();
                        db.BranchCostCenter.Add(entity);
                        db.SaveChanges();
                        branchCostCenter.BranchCostCenterId = entity.BranchCostCenterId;
                    }
                }
            }

        }
        private string BranchCode(int id)
        {
            var placment = db.Placements.Where(x => x.EmployeeId == id && x.isCurrent).FirstOrDefault();
            if (placment != null)
            {
                var costcenter = db.BranchCostCenter.Where(x => x.BranchId == placment.BranchId && x.OrgStructureId == placment.OrgStructureId).FirstOrDefault();
                if (costcenter != null)
                {
                    return costcenter.Values;
                }
            }
            return null;
        }
        private void PrepareEmployeeAccount()
        {
            SetForAllChild();
            HRMInputs inputs = new HRMInputs();
            var list = inputs.GetEmployees(User.Identity.Name).Where(x => x.IsActive).ToList();
            ExceedDbContext dbAc = new ExceedDbContext();
            foreach (var e in list)
            {
                string costCenter = BranchCode(e.PersonId);
                if (costCenter != null)
                {
                    var payrollEmployeeAccount = dbAc.PayrollEmployeeAccounts.Where(x => x.EmployeeId == e.PersonId).FirstOrDefault();
                    if (payrollEmployeeAccount != null)
                    {
                        if (payrollEmployeeAccount.CostCenterAccountCode != costCenter)
                        {
                            payrollEmployeeAccount.EmployeeId = e.PersonId;
                            payrollEmployeeAccount.CostCenterAccountCode = costCenter;
                            payrollEmployeeAccount.LocationAccCode = payrollEmployeeAccount.LocationAccCode;
                            payrollEmployeeAccount.ModifiedBy = User.Identity.Name;
                            payrollEmployeeAccount.LastModified = DateTime.Now;
                            payrollEmployeeAccount.OrgBranchName =
                                UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                            dbAc.Entry(payrollEmployeeAccount).State = EntityState.Modified;
                            dbAc.SaveChanges();
                        }
                    }

                    else
                    {
                        var entityy = new PayrollEmployeeAccount
                        {
                            DateCreated = DateTime.Now,
                            CreatedBy = User.Identity.Name,
                            EmployeeId = e.PersonId,
                            CostCenterAccountCode = costCenter,
                            LocationAccCode = null,
                            SubAccCode = null,
                            OrgBranchName =
                                       UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            Remark = null
                        };

                        db.PayrollEmployeeAccounts.Add(entityy);
                        db.SaveChanges();
                        //payrollEmployeeAccount.PayrollEmployeeAccountId = entityy.PayrollEmployeeAccountId;
                    }
                }
            }
        }
        [Authorize(Roles = Payroll_View)]
        public ActionResult GetProcessingClosedPeriods([DataSourceRequest]DataSourceRequest request)
        {
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            var closedPeriods = db.GLPeriods;
            List<PeriodStatusViewModel> periodStatusViewModels = new List<PeriodStatusViewModel>();
            PeriodStatusViewModel periodStatusViewModel = new PeriodStatusViewModel();
            if (closedPeriods.Any())
            {
                var unclosedPeriodsStatus = db.PeriodStatuses.Where(x => (x.Status == PeriodStatusList.CLOSED || x.Status == PeriodStatusList.PROCESSING || x.Status == PeriodStatusList.TRANSFERRED) && (mybraches.Contains(x.BranchId.ToString()))).ToList();
                if (unclosedPeriodsStatus.Any())
                {
                    foreach (PeriodStatus p in unclosedPeriodsStatus)
                    {
                        periodStatusViewModel = new PeriodStatusViewModel();
                        var check = closedPeriods.Where(c => c.GLPeriodId == p.GLPeriodId).FirstOrDefault();
                        if (check != null)
                        {
                            periodStatusViewModel.PeriodName = check.Name;
                            periodStatusViewModel.GLPeriodId = check.GLPeriodId;
                            periodStatusViewModel.StartDate = check.DateFrom;
                            periodStatusViewModel.EndDate = check.DateTo;
                            periodStatusViewModel.Status = p.Status;
                            periodStatusViewModel.BranchId = p.BranchId;
                            periodStatusViewModels.Add(periodStatusViewModel);
                        }
                    }
                }
            }
            DataSourceResult result = periodStatusViewModels.ToDataSourceResult(request, pe => new
            {
                PeriodName = pe.PeriodName,
                GLPeriodId = pe.GLPeriodId,
                StartDate = pe.StartDate,
                EndDate = pe.EndDate,
                Status = pe.Status,
                BranchId = pe.BranchId
            });

            return Json(result);
        }

        public ActionResult GetPeriod()
        {

            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            var lists = from p in periodList
                        join g in db.GLPeriods
                            on p.Name equals g.Name
                        select (new
                        {
                            g
                        });
            ViewData["PeriodList"] = lists;
            //List<PayrollPeriod> newPeriodList = new List<PayrollPeriod>();


            //    if (periodList.Any())
            //    {
            //        foreach (var p in periodList)
            //        {

            //            if (p.Status ==  FiscalYearStatus.Open )
            //            {

            //                newPeriodList.Add(p);
            //            }
            //        }
            //    }

            ViewData["PeriodList"] = lists;// != null ? newPeriodList.ToList() : new List<PayrollPeriod>();

            return Json(lists.Select(p => new { GLPeriodId = p.g.GLPeriodId, Name = p.g.Name }), JsonRequestBehavior.AllowGet);
        }
        [Authorize(Roles = Payroll_Process)]
        [HttpPost]
        public ActionResult ProcessPayroll(string period, int branchid, string all = "")//, bool jumploan=false)
        {

            string message = "";
            var identifications = db.Identifications.Where(x => x.Type == IdentificationType.PENSION_NO).ToList();
            var Providentifications = db.Identifications.Where(x => x.Type == IdentificationType.PROVIDENT_FUND).ToList();
            var allBranches = db.Branch.Select(x => x.BranchId).ToList();
            if (all == "")
            {
                allBranches = db.Branch.Where(x => x.BranchId == branchid).Select(x => x.BranchId).ToList();
            }
            try
            {
                if (period != "")
                {
                    bool jumploan = false;

                    PayrollProcessAdditions additiondata;
                    if (!String.IsNullOrEmpty(period))
                    {
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "payroll process", "Success", User.Identity.Name, $"started payroll process for period {period} and branch {branchid} ", Modules.Payroll);
                        #endregion

                        foreach (var b in allBranches)
                        {
                            branchid = b;
                            PayrollProcessViewModel payroldata;
                            restDayRate = 0;
                            NormalRate = 0;
                            NightRate = 0;
                            HolidayRate = 0;
                            oTCoefficient = 0;
                            CompanyContribution = 0;
                            EmployeeContribution = 0;
                            ProvCompanyContribution = 0;
                            ProvEmployeeContribution = 0;
                            RetirementAge = 0;
                            var activeEmployees = GetEmployees(branchid).ToList();
                            if (!activeEmployees.Any() && all == "")
                            {
                                message = "The Process is aborted!! There is no any employee under this branch!!";

                                // RedirectToAction("Index");
                                return new JavaScriptResult() { Script = "alert('" + message + "');" };
                            }
                            var periodObj =
                                db.GLPeriods.Where(
                                    x => x.GLPeriodId.ToString() == period)
                                    .FirstOrDefault();
                            if (periodObj != null)
                            {
                                period = periodObj.Name;

                                #region check  period status

                                var PeriodStatus =
                                    db.PeriodStatuses.Where(
                                        x => x.GLPeriodId == periodObj.GLPeriodId && x.BranchId == branchid)
                                        .FirstOrDefault();
                                if (PeriodStatus != null)
                                {
                                    if (PeriodStatus.IsLocked && PeriodStatus.Status == PeriodStatusList.PROCESSING)
                                    {
                                        message = periodObj.Name +
                                                  " is in Processing status, please wait the untill the process is end.";

                                        return new JavaScriptResult() { Script = "alert('" + message + "');" };
                                    }
                                    if (PeriodStatus.Status == PeriodStatusList.PROCESSING)
                                    {

                                        message = periodObj.Name +
                                                  " is in PROCESSING status,please reverse it before processing again.";

                                        // RedirectToAction("Index");
                                        return new JavaScriptResult() { Script = "alert('" + message + "');" };
                                        //return View("Error");
                                    }
                                    if (PeriodStatus.Status == PeriodStatusList.CLOSED)
                                    {
                                        message = periodObj.Name + " is in CLOSED status,cannot be processed.";
                                        // RedirectToAction("Index");
                                        return new JavaScriptResult() { Script = "alert('" + message + "');" };
                                    }
                                }

                                #endregion

                                #region check previous period status

                                var unclosedPeriods =
                                    db.PayrollPeriods.Where(
                                        x => x.DateFrom < periodObj.DateFrom && x.Status == FiscalYearStatus.Open);

                                if (unclosedPeriods.Any())
                                {


                                    foreach (PayrollPeriod p in unclosedPeriods)
                                    {
                                        db = new ExceedDbContext();
                                        var gl = db.GLPeriods.Where(x => x.Name == p.Name).FirstOrDefault();
                                        if (gl != null)
                                        {

                                            var unclosedPeriodsStatus =
                                                db.PeriodStatuses.Where(
                                                    x =>
                                                        x.Status != PeriodStatusList.CLOSED &&
                                                        x.Status != PeriodStatusList.TRANSFERRED)
                                                    .Where(x => x.GLPeriodId == gl.GLPeriodId && x.BranchId == branchid)
                                                    .ToList();

                                            if (unclosedPeriodsStatus.Any())
                                            {

                                                ViewBag.Note =
                                                    "This period cannot be processed before closing previous periods:";
                                                message = ViewBag.Note + p.Name + ",";
                                                //  RedirectToAction("Index");
                                                return new JavaScriptResult() { Script = "alert('" + message + "');" };
                                            }
                                        }

                                    }
                                }

                                #endregion

                            }


                            #region OTRatesSetting

                            if (!GetOTRateSetting())
                            {
                                message = "Over time setting is not configured properly.";
                                //  RedirectToAction("Index");
                                return new JavaScriptResult() { Script = "alert('" + message + "');" };
                            }
                            if (oTCoefficient == 0)
                            {
                                message = "Over time Cofficent setting is not configured properly.";
                                //  RedirectToAction("Index");
                                return new JavaScriptResult() { Script = "alert('" + message + "');" };
                            }

                            #endregion ;


                            //commented for one time

                            #region CheckEmployeeAccount

                            var accounts = db.PayrollEmployeeAccounts.ToList();
                            if (accounts.Count == 0)
                            {
                                message = "Please Set cost cneter accounts for all employees.";

                                return new JavaScriptResult() { Script = "alert('" + message + "');" };
                            }
                            bool isset = false;
                            message = "Please Set cost cneter accounts for employee ";
                            foreach (var e in activeEmployees)
                            {
                                var checkAcc = accounts.Where(x => x.EmployeeId == e.PersonId).FirstOrDefault();

                                if (checkAcc == null)
                                {
                                    isset = true;
                                    message = message + e.FullName + ",";

                                }
                            }

                            #endregion

                            if (!GetPensionSetting())
                            {
                                message = "Pension setting is not configured.";
                                //  RedirectToAction("Index");
                                return new JavaScriptResult() { Script = "alert('" + message + "');" };
                            }
                            GetProvidentSetting();
                            if (isset)
                            {
                                return new JavaScriptResult() { Script = "alert('" + message + "');" };
                            }
                            message = "";

                            #region update period status

                            var findPeriod =
                                db.GLPeriods.Where(x => x.Name == period)
                                    .FirstOrDefault();
                            if (findPeriod != null)
                            {
                                var unclosedPeriodsStatus =
                                    db.PeriodStatuses.Where(
                                        x => x.GLPeriodId == findPeriod.GLPeriodId && x.BranchId == branchid)
                                        .FirstOrDefault();
                                if (unclosedPeriodsStatus != null)
                                {
                                    unclosedPeriodsStatus.Status = PeriodStatusList.PROCESSING;
                                    unclosedPeriodsStatus.IsLocked = true;
                                    db.Entry(unclosedPeriodsStatus).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }

                            #endregion

                            #region Allowance Treashhold


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

                            #endregion

                            // int maxempassignment = 0;
                            //new load payroall configurations
                            LoadPayrollSettings();
                            foreach (EmployeeIdView employee in activeEmployees)
                            {
                                int modelid = SaveViewModel(findPeriod.GLPeriodId.ToString(), employee.PersonId, branchid);
                                if (modelid != 0)
                                {

                                    payroldata = new PayrollProcessViewModel();
                                    payroldata.UnpaidLeaveDays = GetemployeeUnpaidLeaveDays(employee.PersonId, findPeriod.GLPeriodId);

                                    var assignment = db.Placements.Where(x => x.EmployeeId == employee.PersonId && x.isCurrent)
                                            .FirstOrDefault();
                                    if (assignment != null)
                                    {
                                        if (assignment.IsPend)//added
                                            continue;
                                        payroldata.BasicSalary = assignment.Salary;
                                    }
                                    else
                                    {
                                        payroldata.BasicSalary = 0;

                                    }
                                    #region Check placment Date 
                                    decimal WorkingdateDiff = 0;
                                    bool checkNewEmployeeState = false;

                                    if (employee.AssignmentType == AssignmentType.NEW)
                                    {
                                        if (findPeriod.DateTo < employee.PlacmentDate)
                                        {
                                        }
                                        else
                                        {
                                            if (findPeriod.DateTo > employee.PlacmentDate)
                                            {
                                                WorkingdateDiff = (decimal)(findPeriod.DateTo - employee.PlacmentDate).TotalDays;//+1;                                               
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
                                    payroldata.CreatedOn = DateTime.Now.Date;
                                    payroldata.CreatedBy = User.Identity.Name;
                                    payroldata.EmployeeId = employee.PersonId;
                                    payroldata.FullName = employee.FullName;
                                    payroldata.PeriodName = period;
                                    payroldata.GLPeriodId = findPeriod.GLPeriodId;
                                    payroldata.BranchId = branchid;
                                    var employeeAccouunt = db.BankReferences.Where(x =>
                                               x.Reference == employee.PersonId.ToString() &&
                                               x.Type == BankAccountType.PAYROLL &&
                                               x.ReferenceType == "Employee").FirstOrDefault();
                                    if (employeeAccouunt != null)
                                    {
                                        payroldata.BankAccount = employeeAccouunt.AccountNumber;
                                    }

                                    #region absence


                                    var absentdays =
                                        db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == employee.PersonId)
                                            .Where(x => x.GLPeriodId == findPeriod.GLPeriodId).ToList();
                                    if (absentdays.Count() != 0)
                                        payroldata.DaysAbsent = absentdays.Select(x => x.Days).FirstOrDefault();
                                    else
                                        payroldata.DaysAbsent = 0;



                                    #endregion

                                    #region set cost center

                                    var costcenter =
                                        db.PayrollEmployeeAccounts.Where(x => x.EmployeeId == employee.PersonId)
                                            .FirstOrDefault();
                                    if (costcenter != null)
                                    {
                                        payroldata.CostCenterAccountCode = costcenter.CostCenterAccountCode;
                                        var description =
                                            db.GLChartOfAccountAccounts.Where(
                                                x => x.Values == costcenter.CostCenterAccountCode).FirstOrDefault();
                                        if (description != null)
                                        {
                                            payroldata.CostCenterAccountDesc = description.Description;
                                        }
                                        payroldata.OrgBranchAccountCode = costcenter.LocationAccCode;
                                        payroldata.LocationCode = costcenter.LocationAccCode;
                                    }
                                    else
                                    {
                                        payroldata.CostCenterAccountCode = "";
                                        payroldata.CostCenterAccountDesc = "";
                                        payroldata.OrgBranchAccountCode = costcenter.LocationAccCode;
                                        payroldata.LocationCode = costcenter.LocationAccCode;
                                    }

                                    #endregion

                                    #region basic salary



                                    #region Basic Salary Additions

                                    db = new ExceedDbContext();
                                    additiondata = new PayrollProcessAdditions();
                                    additiondata.CreatedOn = DateTime.Now.Date;
                                    additiondata.CreatedBy = User.Identity.Name;
                                    //additiondata.EmployeeId = payroldata.EmployeeId;
                                    additiondata.Name = "Basic Salary";
                                    additiondata.PayrollProcessViewModelId = modelid;
                                    additiondata.Amount = payroldata.BasicSalary;

                                    db.PayrollProcessAdditions.Add(additiondata);
                                    db.SaveChanges();

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
                                        yy = (ot.RestdayHours * restDayRate) + (ot.NormalHours * NormalRate) +
                                             (ot.NightHours * NightRate) +
                                             (ot.HolidayHours * HolidayRate);
                                        totalOT = totalOT + (xx * yy);
                                    }
                                    payroldata.TotalOvertime = totalOT;

                                    #region OT Additions

                                    db = new ExceedDbContext();
                                    additiondata = new PayrollProcessAdditions();
                                    additiondata.CreatedOn = DateTime.Now.Date;
                                    additiondata.CreatedBy = User.Identity.Name;
                                    //additiondata.EmployeeId = employee.PersonId;
                                    additiondata.Name = "Over Time";
                                    additiondata.PayrollProcessViewModelId = modelid;
                                    additiondata.Amount = payroldata.TotalOvertime;

                                    db.PayrollProcessAdditions.Add(additiondata);
                                    db.SaveChanges();

                                    #endregion

                                    #endregion


                                    #endregion

                                    //get allowances

                                    #region allowances

                                    var allowances =
                                        db.AllowancePerEmployees.Where(
                                            x => x.EmployeeId == employee.PersonId && x.Pend == false).ToList();
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
                                        allowancemodel = db.AllowanceDefinations.Where(
                                            x => x.AllowanceDefinationId == allowance.AllowanceDefinationId)
                                            .FirstOrDefault();
                                        decimal allowancemodelAmount = 0;
                                        allowancemodelAmount = allowancemodel.Amount;
                                        allowancedata = new PayrollProcessAllowances();
                                        allowancedata.CreatedOn = DateTime.Now.Date;
                                        allowancedata.CreatedBy = User.Identity.Name;
                                        //allowancedata.EmployeeId = employee.PersonId;
                                        allowancedata.Name = allowancemodel.AllowanceName;
                                        allowancedata.PayrollProcessViewModelId = modelid;
                                        allowancedata.AllowanceType = allowancemodel.AllowanceType;

                                        var rule =
                                            allowanceTypeRule.Where(a => a.AllowanceType == allowancemodel.AllowanceType)
                                                .FirstOrDefault();

                                        if (rule == null)
                                        {
                                            message = "Please Set Allowance Rule For " +
                                                      allowancemodel.AllowanceType.ToString();
                                            // RedirectToAction("Index");
                                            return new JavaScriptResult() { Script = "alert('" + message + "');" };
                                        }
                                        if (allowancemodel.AmtPercent == FixedContPecntType.PERCENT)
                                        {
                                            allowancemodelAmount = payroldata.BasicSalary * (allowancemodelAmount / 100);
                                        }
                                        if (allowancemodel.AllowanceType == AllowanceType.PROFESSIONAL)
                                        {
                                            //decimal totalSickleave = 0;
                                            //var absentdayss =
                                            //    db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == employee.PersonId)
                                            //        .Where(x => x.GLPeriodId == findPeriod.GLPeriodId)
                                            //        .ToList();
                                            //if (absentdays.Count() != 0)
                                            //{
                                            //    totalSickleave += absentdayss.Select(x => x.Days).FirstOrDefault();
                                            //}
                                            //var checkFiscalYear =
                                            //    db.GLPeriods.Where(x => x.GLPeriodId == findPeriod.GLPeriodId)
                                            //        .FirstOrDefault();
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
                                            //allowancemodelAmount = allowancemodelAmount -
                                            //                       (allowancemodelAmount / 30) * totalSickleave;

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

                                            totalSickleave = GetEmployeeSickLeave(employee.PersonId, findPeriod.GLPeriodId, 30);
                                            totalSickleaveAndabsent = totalSickleave + totalAbsent;

                                            allowancemodelAmount = allowancemodelAmount -
                                                                   (allowancemodelAmount / 30) * totalSickleaveAndabsent;

                                        }
                                        if (allowancemodel.AllowanceType == AllowanceType.POSITION)
                                        {

                                            decimal totalDelagatin = 0;

                                            var checkDelegation = db.Delegations.Where(
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
                                                        double dateDiff =
                                                            (checkDelegation.FromDateTime - delegationEndDate)
                                                                .TotalDays;
                                                        if (dateDiff >= 30)
                                                        {
                                                            totalDelagatin = Convert.ToDecimal(dateDiff);
                                                        }
                                                    }
                                                    else if (delegationEndDate > findPeriod.DateTo &&
                                                             checkDelegation.FromDateTime > findPeriod.DateFrom)
                                                    {
                                                        double dateDiff =
                                                            (findPeriod.DateTo - checkDelegation.FromDateTime).TotalDays;
                                                        if (dateDiff >= 30)
                                                        {
                                                            totalDelagatin = Convert.ToDecimal(dateDiff);
                                                        }
                                                    }
                                                    else if (findPeriod.DateTo < checkDelegation.FromDateTime)
                                                    {
                                                        double dateDiff =
                                                            (findPeriod.DateTo - checkDelegation.FromDateTime).TotalDays;
                                                        if (dateDiff >= 30)
                                                        {
                                                            totalDelagatin = Convert.ToDecimal(dateDiff);
                                                        }
                                                    }
                                                }
                                            }
                                            allowancemodelAmount = allowancemodelAmount -
                                                                   (allowancemodelAmount / 30) * totalDelagatin;

                                        }

                                        if (allowancemodel.AllowanceType == AllowanceType.POSITION)
                                        {
                                            decimal totalDelagatin = 0;

                                            var checkDelegation = db.Delegations.Where(
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
                                                        double dateDiff =
                                                            (checkDelegation.FromDateTime - delegationEndDate)
                                                                .TotalDays;
                                                        if (dateDiff >= 30)
                                                        {
                                                            totalDelagatin = Convert.ToDecimal(dateDiff);
                                                        }
                                                    }
                                                    else if (delegationEndDate > findPeriod.DateTo &&
                                                             checkDelegation.FromDateTime > findPeriod.DateFrom)
                                                    {
                                                        double dateDiff =
                                                            (findPeriod.DateTo - checkDelegation.FromDateTime).TotalDays;
                                                        if (dateDiff >= 30)
                                                        {
                                                            totalDelagatin = Convert.ToDecimal(dateDiff);
                                                        }
                                                    }
                                                    else if (findPeriod.DateTo < checkDelegation.FromDateTime)
                                                    {
                                                        double dateDiff =
                                                            (findPeriod.DateTo - checkDelegation.FromDateTime).TotalDays;
                                                        if (dateDiff >= 30)
                                                        {
                                                            totalDelagatin = Convert.ToDecimal(dateDiff);
                                                        }
                                                    }
                                                }
                                                allowancemodelAmount = allowancemodelAmount +
                                                                       (allowancemodelAmount / 30) * totalDelagatin;

                                            }
                                        }

                                        Threshold = ((decimal)(rule.Percentage / 100) * payroldata.BasicSalary);

                                        #region maximum non taxable rule

                                        if (rule.TaxTrasholdAmount == 0 && rule.Percentage == 0)
                                        {
                                            if (settingvalue == "NON_TAXABLE")
                                            {
                                                allowancedata.NonTaxable = Convert.ToDecimal(allowancemodelAmount);
                                                allowancedata.Taxable = 0;
                                                allowancedata.Total = Convert.ToDecimal(allowancemodelAmount);
                                                totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                                totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                           allowancedata.NonTaxable;
                                                totalAllowance = totalAllowance + allowancedata.Total;

                                                payroldata.NonTaxableAllowance = Convert.ToDecimal(allowancemodelAmount);
                                                payroldata.TaxableAllowance = 0;
                                                payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                            payroldata.TaxableAllowance;
                                            }
                                            else
                                            {
                                                if (allowancemodelAmount > Threshold)
                                                {


                                                    if (allowancemodelAmount > rule.TaxTrasholdAmount &&
                                                        rule.TaxTrasholdAmount > 0)
                                                    {
                                                        allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                        allowancedata.Taxable = Convert.ToDecimal(allowancemodelAmount) -
                                                                                Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                        allowancedata.Total = Convert.ToDecimal(allowancemodelAmount);
                                                        totalTaxableAllowance = totalTaxableAllowance +
                                                                                allowancedata.Taxable;
                                                        totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                                   allowancedata.NonTaxable;
                                                        totalAllowance = totalAllowance + allowancedata.Total;

                                                        payroldata.NonTaxableAllowance =
                                                            Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                        payroldata.TaxableAllowance =
                                                            Convert.ToDecimal(allowancemodelAmount) -
                                                            Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                        payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                                    payroldata.TaxableAllowance;
                                                    }

                                                    else
                                                    {
                                                        allowancedata.NonTaxable = Threshold;
                                                        allowancedata.Taxable = Convert.ToDecimal(allowancemodelAmount) -
                                                                                Threshold;
                                                        allowancedata.Total = Convert.ToDecimal(allowancemodelAmount);
                                                        totalTaxableAllowance = totalTaxableAllowance +
                                                                                allowancedata.Taxable;
                                                        totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                                   allowancedata.NonTaxable;
                                                        totalAllowance = totalAllowance + allowancedata.Total;

                                                        payroldata.NonTaxableAllowance = Threshold;
                                                        payroldata.TaxableAllowance =
                                                            Convert.ToDecimal(allowancemodelAmount) -
                                                            Threshold;
                                                        payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                                    payroldata.NonTaxableAllowance;
                                                    }
                                                }
                                                else
                                                {
                                                    if (allowancemodelAmount > rule.TaxTrasholdAmount &&
                                                        rule.TaxTrasholdAmount > 0)
                                                    {
                                                        allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                        allowancedata.Taxable = Convert.ToDecimal(allowancemodelAmount) -
                                                                                Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                        allowancedata.Total = Convert.ToDecimal(allowancemodelAmount);
                                                        totalTaxableAllowance = totalTaxableAllowance +
                                                                                allowancedata.Taxable;
                                                        totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                                   allowancedata.NonTaxable;
                                                        totalAllowance = totalAllowance + allowancedata.Total;

                                                        payroldata.NonTaxableAllowance =
                                                            Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                        payroldata.TaxableAllowance =
                                                            Convert.ToDecimal(allowancemodelAmount) -
                                                            Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                        payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                                    payroldata.TaxableAllowance;
                                                    }
                                                    else
                                                    {
                                                        allowancedata.NonTaxable = Convert.ToDecimal(allowancemodelAmount);
                                                        allowancedata.Taxable = 0;
                                                        allowancedata.Total = Convert.ToDecimal(allowancemodelAmount);
                                                        totalTaxableAllowance = totalTaxableAllowance +
                                                                                allowancedata.Taxable;
                                                        totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                                   allowancedata.NonTaxable;
                                                        totalAllowance = totalAllowance + allowancedata.Total;

                                                        payroldata.NonTaxableAllowance =
                                                            Convert.ToDecimal(allowancemodelAmount);
                                                        payroldata.TaxableAllowance = 0;
                                                        payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                                    payroldata.NonTaxableAllowance;
                                                    }

                                                }
                                            }
                                        }
                                        else
                                        {
                                            if (allowancemodelAmount > Threshold)
                                            {


                                                if (allowancemodelAmount > rule.TaxTrasholdAmount &&
                                                    rule.TaxTrasholdAmount > 0)
                                                {
                                                    allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                    allowancedata.Taxable = Convert.ToDecimal(allowancemodelAmount) -
                                                                            Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                    allowancedata.Total = Convert.ToDecimal(allowancemodelAmount);
                                                    totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                                    totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                               allowancedata.NonTaxable;
                                                    totalAllowance = totalAllowance + allowancedata.Total;

                                                    payroldata.NonTaxableAllowance =
                                                        Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                    payroldata.TaxableAllowance = Convert.ToDecimal(allowancemodelAmount) -
                                                                                  Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                    payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                                payroldata.TaxableAllowance;
                                                }

                                                else
                                                {
                                                    allowancedata.NonTaxable = Threshold;
                                                    allowancedata.Taxable = Convert.ToDecimal(allowancemodelAmount) -
                                                                            Threshold;
                                                    allowancedata.Total = Convert.ToDecimal(allowancemodelAmount);
                                                    totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                                    totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                               allowancedata.NonTaxable;
                                                    totalAllowance = totalAllowance + allowancedata.Total;

                                                    payroldata.NonTaxableAllowance = Threshold;
                                                    payroldata.TaxableAllowance = Convert.ToDecimal(allowancemodelAmount) -
                                                                                  Threshold;
                                                    payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                                payroldata.NonTaxableAllowance;
                                                }
                                            }
                                            else
                                            {
                                                if (allowancemodelAmount > rule.TaxTrasholdAmount &&
                                                    rule.TaxTrasholdAmount > 0)
                                                {
                                                    allowancedata.NonTaxable = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                    allowancedata.Taxable = Convert.ToDecimal(allowancemodelAmount) -
                                                                            Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                    allowancedata.Total = Convert.ToDecimal(allowancemodelAmount);
                                                    totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                                    totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                               allowancedata.NonTaxable;
                                                    totalAllowance = totalAllowance + allowancedata.Total;

                                                    payroldata.NonTaxableAllowance =
                                                        Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                    payroldata.TaxableAllowance = Convert.ToDecimal(allowancemodelAmount) -
                                                                                  Convert.ToDecimal(rule.TaxTrasholdAmount);
                                                    payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                                payroldata.TaxableAllowance;
                                                }
                                                else
                                                {
                                                    allowancedata.NonTaxable = Convert.ToDecimal(allowancemodelAmount);
                                                    allowancedata.Taxable = 0;
                                                    allowancedata.Total = Convert.ToDecimal(allowancemodelAmount);
                                                    totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                                                    totalNonTaxableAllowance = totalNonTaxableAllowance +
                                                                               allowancedata.NonTaxable;
                                                    totalAllowance = totalAllowance + allowancedata.Total;

                                                    payroldata.NonTaxableAllowance = Convert.ToDecimal(allowancemodelAmount);
                                                    payroldata.TaxableAllowance = 0;
                                                    payroldata.TotalAllowance = payroldata.NonTaxableAllowance +
                                                                                payroldata.NonTaxableAllowance;
                                                }

                                            }

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
                                        if (allowancemodel.AllowanceType == AllowanceType.PROJECT)
                                        {
                                            payroldata.TaxableProjectAllowance += payroldata.TaxableAllowance;
                                            payroldata.NonTaxableProjectAllowance += payroldata.NonTaxableAllowance;
                                            payroldata.TotalProjectAllowance += payroldata.TaxableAllowance +
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

                                        allowancedata.PayrollProcessViewModelId = modelid;
                                        db.PayrollProcessAllowances.Add(allowancedata);
                                        db.SaveChanges();

                                        #region Allowance Additions

                                        db = new ExceedDbContext();
                                        additiondata = new PayrollProcessAdditions();
                                        additiondata.CreatedOn = DateTime.Now.Date;
                                        additiondata.CreatedBy = User.Identity.Name;
                                        //additiondata.EmployeeId = employee.PersonId;
                                        additiondata.Name = allowancedata.Name;
                                        //additiondata.Name = period;
                                        additiondata.Amount = allowancedata.Taxable + allowancedata.NonTaxable;
                                        additiondata.PayrollProcessViewModelId = modelid;
                                        db.PayrollProcessAdditions.Add(additiondata);
                                        db.SaveChanges();
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
                                        additiondata.PayrollProcessViewModelId = modelid;
                                        additiondata.Name =
                                            lookups.Where(l => l.LookupId == addition.LookupId).FirstOrDefault().Description;
                                        //additiondata.Name = period;
                                        additiondata.AdditionId = addition.LookupId;
                                        additiondata.Amount = addition.Amount;
                                        totalAdditions = totalAdditions + additiondata.Amount;
                                        db.PayrollProcessAdditions.Add(additiondata);
                                        db.SaveChanges();

                                        if (addition.AdditionTaxType == AdditionTaxType.TAXABLE)
                                        {
                                            payroldata.TotalTaxAdditions += totalAdditions;
                                        }
                                        else
                                        {
                                            payroldata.TotalNonTaxAdditions += totalAdditions;
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
                                        deductiondata.PayrollProcessViewModelId = modelid;
                                        deductiondata.Index = 2;
                                        deductiondata.Amount = deduction.Amount;
                                        totalDeductions = totalDeductions + deduction.Amount;
                                        db.PayrollProcessDeductions.Add(deductiondata);
                                        db.SaveChanges();
                                    }
                                    //get unpaid leave deductions

                                    if (payroldata.UnpaidLeaveDays > 0 && DeductUnpaidLeave)
                                    {
                                        deductiondata = new PayrollProcessDeductions();
                                        deductiondata.CreatedOn = DateTime.Now.Date;
                                        deductiondata.CreatedBy = User.Identity.Name;
                                        //deductiondata.EmployeeId = employee.PersonId;
                                        deductiondata.Name = "Leave Without Pay Deduction";
                                        deductiondata.PayrollProcessViewModelId = modelid;
                                        deductiondata.Index = 3;
                                        deductiondata.Amount = ((payroldata.BasicSalary * payroldata.UnpaidLeaveDays) / 30);
                                        //can be divided by total working days
                                        //totalDeductions = totalDeductions + deductiondata.Amount;

                                        db.PayrollProcessDeductions.Add(deductiondata);
                                        db.SaveChanges();
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
                                        deductiondata.PayrollProcessViewModelId = modelid;
                                        deductiondata.Index = 4;
                                        deductiondata.Amount = ((payroldata.BasicSalary * payroldata.DaysAbsent) / 30);
                                        //can be divided by total working days
                                        //totalDeductions = totalDeductions + deductiondata.Amount;

                                        db.PayrollProcessDeductions.Add(deductiondata);
                                        db.SaveChanges();

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
                                        db.EmpPayrollLongTermDeductions.Where(x => x.EmployeeId == employee.PersonId)
                                            .ToList();
                                    decimal totalLongtermDeductions = 0;
                                    PayrollProcessLongTermDeductions longtermdata;
                                    foreach (EmpPayrollLongTermDeduction longterm in longtermDeductions)
                                    {
                                        if (longterm.Balance > 0)
                                        {
                                            longtermdata = new PayrollProcessLongTermDeductions();
                                            longtermdata.CreatedOn = DateTime.Now.Date;
                                            longtermdata.CreatedBy = User.Identity.Name;
                                            longtermdata.EmpPayrollLongTermDeductionId =
                                                longterm.EmpPayrollLongTermDeductionId;
                                            //longtermdata.EmployeeId = employee.PersonId;
                                            longtermdata.Name = longterm.DeductionType;
                                            longtermdata.PayrollProcessViewModelId = modelid;

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
                                            db.PayrollProcessLongTermDeductions.Add(longtermdata);
                                            db.SaveChanges();

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
                                            db.PayrollProcessDeductions.Add(deductiondata);
                                            db.SaveChanges();

                                            #endregion
                                        }
                                    }
                                    payroldata.TotalLongTermDeductions = totalLongtermDeductions;

                                    #endregion

                                    #region get Penality


                                    var Allpenalities =
                                        db.DisciplineMeasuerByMoney.Where(x => x.EmployeeId == employee.PersonId).ToList();
                                    decimal totalpenalityAmt = 0;
                                    List<DisciplineMeasuerByMoney> penalities = new List<DisciplineMeasuerByMoney>();
                                    foreach (var p in Allpenalities)
                                    {
                                        if (p.GLPeriod.DateFrom.Date >= findPeriod.DateFrom.Date &&
                                            p.GLPeriod.DateTo.Date <= findPeriod.DateTo.Date)
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
                                            PenalityData.LastModified = DateTime.Now.Date;
                                            PenalityData.ModifiedBy = User.Identity.Name;
                                            PenalityData.DisciplineMeasuerByMoneyId = longterm.DisciplineMeasuerByMoneyId;
                                            PenalityData.EmployeeId = longterm.EmployeeId;
                                            //PenalityData.GLPeriodId = longterm.GLPeriodId;                             

                                            if (longterm.Balance <= longterm.MonthlyAmount)
                                            {
                                                PenalityData.Amount = longterm.Balance;
                                                Penality.OutstandingAmount = 0;
                                            }
                                            else
                                            {
                                                PenalityData.Amount = longterm.MonthlyAmount;
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
                                            // db.DisciplineMeasuerByMoney.Attach(longterm);
                                            db.Entry(longterm).State = EntityState.Modified;
                                            db.SaveChanges();

                                            #region Discipline Deduction

                                            db = new ExceedDbContext();
                                            deductiondata = new PayrollProcessDeductions();
                                            deductiondata.CreatedOn = DateTime.Now.Date;
                                            deductiondata.CreatedBy = User.Identity.Name;
                                            deductiondata.PayrollProcessViewModelId = modelid;
                                            deductiondata.Name = "Discipline Penality";
                                            deductiondata.Remark = "Discipline";
                                            deductiondata.OutstandingAmount = Penality.OutstandingAmount;
                                            deductiondata.Amount = longterm.Amount;
                                            deductiondata.Index = 11;
                                            db.PayrollProcessDeductions.Add(deductiondata);
                                            db.SaveChanges();

                                            #endregion
                                        }
                                    }
                                    payroldata.TotalPenality = totalpenalityAmt;

                                    #endregion

                                    #region get loans

                                    ExceedDbContext dbl = new ExceedDbContext();
                                    //get loans
                                    var loans =
                                        dbl.Loans.Where(x => x.EmployeeId == employee.PersonId && x.IsActive).ToList();
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
                                                loandata.PayrollProcessViewModelId = modelid;
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
                                                dbl.Entry(loan).State = EntityState.Modified;
                                                dbl.PayrollProcessLoans.Add(loandata);
                                                dbl.SaveChanges();

                                                #region LoanDeduction

                                                db = new ExceedDbContext();
                                                deductiondata = new PayrollProcessDeductions();
                                                deductiondata.CreatedOn = DateTime.Now.Date;
                                                deductiondata.CreatedBy = User.Identity.Name;
                                                //deductiondata.EmployeeId = employee.PersonId;
                                                deductiondata.Name = "Loan";
                                                deductiondata.OutstandingAmount = loandata.Outstanding;
                                                deductiondata.Amount = loandata.Amount;
                                                deductiondata.PayrollProcessViewModelId = modelid;
                                                deductiondata.Index = 6;
                                                db.PayrollProcessDeductions.Add(deductiondata);
                                                db.SaveChanges();

                                                #endregion
                                            }
                                        }
                                    }
                                    payroldata.TotalLoan = totalLoans;

                                    #endregion

                                    #region get advances

                                    ExceedDbContext dba = new ExceedDbContext();
                                    //get advances
                                    var advances =
                                        dba.Advances.Where(x => x.EmployeeId == employee.PersonId && x.IsActive).ToList();
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
                                                advancedata.PayrollProcessViewModelId = modelid;
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
                                                dba.Entry(advance).State = EntityState.Modified;
                                                dba.PayrollProcessAdvances.Add(advancedata);
                                                dba.SaveChanges();

                                                #region Advance Deduction

                                                db = new ExceedDbContext();
                                                deductiondata = new PayrollProcessDeductions();
                                                deductiondata.CreatedOn = DateTime.Now.Date;
                                                deductiondata.CreatedBy = User.Identity.Name;
                                                deductiondata.OutstandingAmount = advancedata.Outstanding;
                                                deductiondata.Name = advancedata.Name;
                                                deductiondata.PayrollProcessViewModelId = modelid;
                                                deductiondata.Amount = advancedata.Amount;
                                                deductiondata.Index = 7;
                                                db.PayrollProcessDeductions.Add(deductiondata);
                                                db.SaveChanges();

                                                #endregion
                                            }
                                        }
                                    }
                                    payroldata.TotalAdvance = totalAdvances;

                                    #endregion

                                    #region get Sport Fee

                                    //get credits
                                    var sports =
                                        db.Associations.Where(x => x.EmployeeId == employee.PersonId)
                                            .Where(x => x.IsMemeber == true)
                                            .Where(x => x.Type == AssocationType.SPORT_FEE)
                                            .ToList();
                                    decimal Totalsport = 0;
                                    decimal OneSport = 0;
                                    foreach (Association credit in sports)
                                    {
                                        OneSport = 0;
                                        if (credit.AmtPercnet == FixedContPecntType.PERCENT)
                                        {
                                            if (credit.NetGross == FixedContAmtType.FROM_GROSS)
                                            {
                                                Totalsport = Totalsport + credit.AdditionalAmount +
                                                             (payroldata.BasicSalary * credit.MonthlyAmount / 100);
                                                OneSport = credit.AdditionalAmount +
                                                           (payroldata.BasicSalary * credit.MonthlyAmount / 100);
                                            }
                                            if (credit.NetGross == FixedContAmtType.FROM_NET)
                                            {
                                                decimal Pension = payroldata.BasicSalary *
                                                                  Convert.ToDecimal((EmployeeContribution / 100));
                                                decimal beforeIncome =
                                                    (decimal)XAPI.IncomeTax.GetIncomeTax((float)(payroldata.BasicSalary));
                                                Totalsport = Totalsport + credit.AdditionalAmount +
                                                             ((payroldata.BasicSalary - Pension - beforeIncome) *
                                                              (credit.MonthlyAmount / 100));
                                                OneSport = credit.AdditionalAmount +
                                                           ((payroldata.BasicSalary - Pension - beforeIncome) *
                                                            (credit.MonthlyAmount / 100));
                                            }
                                        }
                                        else
                                        {
                                            Totalsport = Totalsport + credit.Amount;
                                            OneSport = credit.Amount;
                                        }


                                        #region Sport Fee

                                        db = new ExceedDbContext();
                                        deductiondata = new PayrollProcessDeductions();
                                        deductiondata.CreatedOn = DateTime.Now.Date;
                                        deductiondata.CreatedBy = User.Identity.Name;
                                        //deductiondata.EmployeeId = employee.PersonId;
                                        deductiondata.Name = credit.Type.ToString();
                                        //deductiondata.Name = period;
                                        deductiondata.Amount = OneSport; //credit.AdditionalAmount +
                                                                         //(payroldata.BasicSalary * credit.MonthlyAmount / 100);
                                        deductiondata.Index = 12;
                                        deductiondata.PayrollProcessViewModelId = modelid;
                                        db.PayrollProcessDeductions.Add(deductiondata);
                                        db.SaveChanges();

                                        #endregion
                                    }
                                    payroldata.SportFee = Totalsport;

                                    #endregion

                                    #region get regular contribution

                                    var contributins = db.ContributionTypes;
                                    decimal RedCross = 0;
                                    ExceedDbContext dbr = new ExceedDbContext();
                                    //get contribution
                                    var contributions =
                                        dbr.RegularContributions.Where(x => x.EmployeeId == employee.PersonId)
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
                                                decimal beforeIncome = (decimal)XAPI.IncomeTax.GetIncomeTax((float)payroldata.BasicSalary);
                                                regularcontributionAmount = contribution.AdditionalAmount + ((payroldata.BasicSalary - Pension - beforeIncome) * (contribution.Amount / 100));

                                            }
                                            else
                                            {
                                                regularcontributionAmount = contribution.AdditionalAmount + (payroldata.BasicSalary * contribution.Amount / 100);
                                            }
                                            #endregion
                                            regularcontributiondata.Name = onecontr.Description;
                                            regularcontributiondata.PayrollProcessViewModelId = modelid;
                                            regularcontributiondata.CostCenterAccountCode = payroldata.CostCenterAccountCode;
                                            regularcontributiondata.OrgBranchName = payroldata.OrgBranchAccountCode;
                                            regularcontributiondata.Amount = regularcontributionAmount;//new 
                                            //regularcontributiondata.Amount = contribution.AdditionalAmount +
                                            //                                 (payroldata.BasicSalary * contribution.Amount / 100);
                                            totalContributions = totalContributions + regularcontributiondata.Amount;
                                            dbr.PayrollProcessRegularContribution.Add(regularcontributiondata);
                                            dbr.SaveChanges();
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
                                            deductiondata.PayrollProcessViewModelId = modelid;
                                            db.PayrollProcessDeductions.Add(deductiondata);
                                            db.SaveChanges();

                                            #endregion
                                        }
                                    }
                                    payroldata.TotalRegularContribution = totalContributions;

                                    #endregion

                                    #region get fixed contribution

                                    ExceedDbContext fxdb = new ExceedDbContext();


                                    var fixedcontributions =
                                        fxdb.FixedContributions.Where(x => x.EmployeeId == employee.PersonId && x.IsActive)
                                            .ToList();
                                    decimal totalFixedcontribution = 0;

                                    decimal OneFixedcontribution = 0;
                                    PayrollProcessFixedContribution fixedcontributiondata;
                                    foreach (FixedContribution fixedcontribution in fixedcontributions)
                                    {
                                        decimal Amount = 0;
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

                                            fixedcontributiondata = new PayrollProcessFixedContribution();
                                            fixedcontributiondata.CreatedOn = DateTime.Now.Date;
                                            fixedcontributiondata.CreatedBy = User.Identity.Name;
                                            fixedcontributiondata.FixedContributionId =
                                                fixedcontribution.FixedContributionId;

                                            var onecontr =
                                                contributins.Where(
                                                    l => l.ContributionTypeId == fixedcontribution.ContributionTypeId)
                                                    .FirstOrDefault();
                                            if (onecontr != null)
                                            {
                                                fixedcontributiondata.Name = onecontr.Description;
                                                fixedcontributiondata.PayrollProcessViewModelId = modelid;
                                                fixedcontributiondata.CostCenterAccountCode =
                                                    payroldata.CostCenterAccountCode;
                                                fixedcontributiondata.OrgBranchName = payroldata.OrgBranchAccountCode;
                                                if (fixedcontribution.Balance <= OneFixedcontribution)
                                                {
                                                    fixedcontributiondata.Amount = fixedcontribution.Balance;
                                                    fixedcontributiondata.Outstanding = 0;
                                                }
                                                else
                                                {
                                                    fixedcontributiondata.Amount = OneFixedcontribution;
                                                    fixedcontributiondata.Outstanding = fixedcontribution.Balance -
                                                                                        OneFixedcontribution;
                                                }
                                                totalFixedcontribution = totalFixedcontribution +
                                                                         fixedcontributiondata.Amount;

                                                if (fixedcontribution.Balance <= OneFixedcontribution)
                                                {
                                                    fixedcontribution.Balance = 0;
                                                }
                                                else
                                                {
                                                    fixedcontribution.Balance = fixedcontribution.Balance -
                                                                                OneFixedcontribution;
                                                }
                                                fxdb.Entry(fixedcontribution).State = EntityState.Modified;
                                                fxdb.PayrollProcessFixedContribution.Add(fixedcontributiondata);
                                                fxdb.SaveChanges();

                                                #region Fixed Contribution Deduction

                                                db = new ExceedDbContext();
                                                deductiondata = new PayrollProcessDeductions();
                                                deductiondata.CreatedOn = DateTime.Now.Date;
                                                deductiondata.CreatedBy = User.Identity.Name;
                                                deductiondata.PayrollProcessViewModelId = modelid;
                                                deductiondata.Name = fixedcontributiondata.Name;
                                                deductiondata.OutstandingAmount = fixedcontributiondata.Outstanding;
                                                deductiondata.Amount = fixedcontributiondata.Amount;
                                                deductiondata.Index = 8;
                                                db.PayrollProcessDeductions.Add(deductiondata);
                                                db.SaveChanges();

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
                                    decimal OneCredits = 0;
                                    foreach (Association credit in credits)
                                    {
                                        OneCredits = 0;
                                        if (credit.AmtPercnet == FixedContPecntType.PERCENT)
                                        {
                                            if (credit.NetGross == FixedContAmtType.FROM_GROSS)
                                            {
                                                totalCredits = totalCredits + credit.AdditionalAmount +
                                                               (payroldata.BasicSalary * credit.MonthlyAmount / 100);
                                                OneCredits = credit.AdditionalAmount +
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
                                                OneCredits = credit.AdditionalAmount +
                                                             ((payroldata.BasicSalary - Pension - beforeIncome) *
                                                              (credit.MonthlyAmount / 100));
                                            }
                                        }
                                        else
                                        {
                                            totalCredits = totalCredits + credit.Amount;
                                            OneCredits = credit.Amount;
                                        }


                                        #region Assocation Contribution Deduction

                                        db = new ExceedDbContext();
                                        deductiondata = new PayrollProcessDeductions();
                                        deductiondata.CreatedOn = DateTime.Now.Date;
                                        deductiondata.CreatedBy = User.Identity.Name;
                                        //deductiondata.EmployeeId = employee.PersonId;
                                        deductiondata.Name = credit.Type.ToString();
                                        //deductiondata.Name = period;

                                        deductiondata.Amount = OneCredits; //credit.AdditionalAmount +
                                                                           //(payroldata.BasicSalary*credit.MonthlyAmount/100);
                                        deductiondata.Index = 10;
                                        deductiondata.PayrollProcessViewModelId = modelid;
                                        db.PayrollProcessDeductions.Add(deductiondata);
                                        db.SaveChanges();

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
                                    decimal OneLabour = 0;
                                    foreach (Association labour in labourunions)
                                    {
                                        OneLabour = 0;
                                        if (labour.AmtPercnet == FixedContPecntType.PERCENT)
                                        {
                                            if (labour.NetGross == FixedContAmtType.FROM_GROSS)
                                            {
                                                totalLabour = totalLabour + labour.AdditionalAmount +
                                                              (payroldata.BasicSalary * labour.MonthlyAmount / 100);
                                                OneLabour = labour.AdditionalAmount +
                                                            (payroldata.BasicSalary * labour.MonthlyAmount / 100);
                                            }
                                            if (labour.NetGross == FixedContAmtType.FROM_NET)
                                            {
                                                decimal Pension = payroldata.BasicSalary *
                                                                  Convert.ToDecimal((EmployeeContribution / 100));
                                                decimal beforeIncome =
                                                    (decimal)XAPI.IncomeTax.GetIncomeTax((float)(payroldata.BasicSalary));
                                                totalLabour = totalLabour + labour.AdditionalAmount +
                                                              ((payroldata.BasicSalary - Pension - beforeIncome) *
                                                               (labour.MonthlyAmount / 100));
                                                OneLabour = labour.AdditionalAmount +
                                                            ((payroldata.BasicSalary - Pension - beforeIncome) *
                                                             (labour.MonthlyAmount / 100));
                                            }
                                        }
                                        else
                                        {
                                            totalLabour = totalLabour + labour.Amount;
                                            OneLabour = labour.Amount;
                                        }


                                        #region Assocation Contribution Deduction

                                        db = new ExceedDbContext();
                                        deductiondata = new PayrollProcessDeductions();
                                        deductiondata.CreatedOn = DateTime.Now.Date;
                                        deductiondata.CreatedBy = User.Identity.Name;
                                        //deductiondata.EmployeeId = employee.PersonId;
                                        deductiondata.Name = labour.Type.ToString();
                                        deductiondata.PayrollProcessViewModelId = modelid;
                                        //deductiondata.Name = period;
                                        deductiondata.Amount = OneLabour; // labour.AdditionalAmount +
                                                                          // (payroldata.BasicSalary*labour.MonthlyAmount/100);
                                        deductiondata.Index = 10;
                                        db.PayrollProcessDeductions.Add(deductiondata);
                                        db.SaveChanges();

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

                                    payroldata.GrossEarning = payroldata.GrossNonTaxableEarning +
                                                              payroldata.GrossTaxableEarning;

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

                                    #region time span difference 

                                    #endregion
                                    var timeSpan = findPeriod.DateFrom - employee.DateOfBirth;
                                    var yearDiff = timeSpan.TotalDays / 365.25;
                                    employee.Age = (int)yearDiff;
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
                                    //else

                                    //{
                                    //    var checkExistance =
                                    //       db.ProvidentFunds.Where(x => x.EmployeeId == payroldata.EmployeeId).FirstOrDefault();
                                    //    if (checkExistance != null)
                                    //    {
                                    //        #region get Provident Fund

                                    //        if (checkExistance.IsMemeber)
                                    //        {

                                    //            var idnumber =
                                    //                Providentifications.Where(
                                    //                    x =>
                                    //                        x.Reference == payroldata.EmployeeId.ToString() &&
                                    //                        x.ReferenceType == "Employee").FirstOrDefault();
                                    //            if (idnumber != null)
                                    //            {
                                    //                payroldata.ProvidentNumber = idnumber.IdNumber;
                                    //            }
                                    //            payroldata.ProvidentFromEmployee = payroldata.BasicSalary *
                                    //                                               ProvEmployeeContribution /
                                    //                                               100;
                                    //            payroldata.ProvidentFromCompany = payroldata.BasicSalary * ProvCompanyContribution /
                                    //                                              100;
                                    //            payroldata.TotalProvident = payroldata.ProvidentFromCompany +
                                    //                                        payroldata.ProvidentFromEmployee;
                                    //        }
                                    //        else
                                    //        {
                                    //            #region get  pension

                                    //            ////get pension

                                    //            var idnumber =
                                    //                identifications.Where(
                                    //                    x =>
                                    //                        x.Reference == payroldata.EmployeeId.ToString() &&
                                    //                        x.ReferenceType == "Employee").FirstOrDefault();
                                    //            if (idnumber != null)
                                    //            {
                                    //                payroldata.PensionNumber = idnumber.IdNumber;
                                    //            }
                                    //            payroldata.PensionFromEmployee = payroldata.BasicSalary * EmployeeContribution / 100;
                                    //            payroldata.PensionFromCompany = payroldata.BasicSalary * CompanyContribution / 100;
                                    //            payroldata.TotalPension = payroldata.PensionFromCompany +
                                    //                                      payroldata.PensionFromEmployee;

                                    //            #endregion
                                    //        }

                                    //        #endregion
                                    //    }
                                    //    else
                                    //    {

                                    //        #region get  pension

                                    //        ////get pension

                                    //        var idnumber =
                                    //            identifications.Where(
                                    //                x =>
                                    //                    x.Reference == payroldata.EmployeeId.ToString() &&
                                    //                    x.ReferenceType == "Employee").FirstOrDefault();
                                    //        if (idnumber != null)
                                    //        {
                                    //            payroldata.PensionNumber = idnumber.IdNumber;
                                    //        }
                                    //        payroldata.PensionFromEmployee = payroldata.BasicSalary * EmployeeContribution / 100;
                                    //        payroldata.PensionFromCompany = payroldata.BasicSalary * CompanyContribution / 100;
                                    //        payroldata.TotalPension = payroldata.PensionFromCompany +
                                    //                                  payroldata.PensionFromEmployee;

                                    //        #endregion
                                    //    }
                                    //}

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
                                                            + payroldata.AbsentDeduction
                                        ;

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
                                    deductiondata.PayrollProcessViewModelId = modelid;
                                    db.PayrollProcessDeductions.Add(deductiondata);
                                    db.SaveChanges();

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
                                    ;
                                    deductiondata.Index = 1;
                                    deductiondata.PayrollProcessViewModelId = modelid;
                                    db.PayrollProcessDeductions.Add(deductiondata);
                                    db.SaveChanges();

                                    #endregion

                                    //#region Provident Contribution Deduction

                                    //db = new ExceedDbContext();
                                    //deductiondata = new PayrollProcessDeductions();
                                    //deductiondata.CreatedOn = DateTime.Now.Date;
                                    //deductiondata.CreatedBy = User.Identity.Name;
                                    ////deductiondata.EmployeeId = employee.PersonId;
                                    //deductiondata.Name = "Provident";
                                    ////deductiondata.Name = period;
                                    //deductiondata.Amount = payroldata.ProvidentFromEmployee;
                                    //deductiondata.Index = 133;
                                    //deductiondata.PayrollProcessViewModelId = modelid;
                                    //db.PayrollProcessDeductions.Add(deductiondata);
                                    //db.SaveChanges();

                                    //#endregion

                                    payroldata.Type = employee.Status;

                                }
                                else
                                {
                                    message = "The Process is aborted ";
                                    return new JavaScriptResult() { Script = "alert('" + message + "');" };
                                }


                            }
                            #region update period status

                            var findPeriodd =
                                db.GLPeriods.Where(x => x.Name == period)
                                    .FirstOrDefault();
                            if (findPeriodd != null)
                            {
                                var unclosedPeriodsStatus =
                                    db.PeriodStatuses.Where(x => x.GLPeriodId == findPeriodd.GLPeriodId && x.BranchId == branchid).FirstOrDefault();
                                if (unclosedPeriodsStatus != null)
                                {
                                    unclosedPeriodsStatus.Status = PeriodStatusList.PROCESSING;
                                    unclosedPeriodsStatus.IsLocked = false;
                                    db.Entry(unclosedPeriodsStatus).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }

                            #endregion
                            var PeriodStatusLists = db.PeriodStatuses.Where(x => x.Status == PeriodStatusList.NEW).ToList();
                            GetPeriod();
                        }
                    }


                }
                else
                {


                    return new JavaScriptResult() { Script = "alert('" + "Please Select Period First!!" + "');" };
                }


            }
            catch (Exception e)
            {
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll process", "Success", User.Identity.Name, $"failed payroll process for period {period} and branch {branchid} with error {e.Message}", Modules.Payroll);
                #endregion
                Logger.Error("Messsage: " + e.Message + "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
                message = "The Process is aborted";
                return new JavaScriptResult() { Script = "alert('" + message + "');" };
                // throw;
            }
            // RedirectToAction("Index");
            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "payroll process", "Success", User.Identity.Name, $"completed payroll process for period {period} and branch {branchid} ", Modules.Payroll);
            #endregion
            message = "Successfully Processed!!";
            return new JavaScriptResult() { Script = "alert('" + message + "');" };

        }

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
            catch (Exception e)
            {
                Logger.Error("Messsage: " + e.Message + "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
                return false;
            }
            return false;
        }
        private bool GetPensionSetting()
        {
            var Pensions = db.Configurations.Where(x => x.Reference == "Pension").ToList();

            try
            {
                if (Pensions.Any())
                {
                    var Employ = Pensions.Where(o => o.Attribute == PensionAttribute.EMPLOYEE_CONTRIBUTION.ToString())
                        .FirstOrDefault();
                    if (Employ != null)
                    {
                        EmployeeContribution =
                            Convert.ToDecimal(Employ.CurrentValue);
                    }
                    var Comp = Pensions.Where(o => o.Attribute == PensionAttribute.COMPANY_CONTRIBUTION.ToString())
                      .FirstOrDefault();
                    if (Comp != null)
                    {
                        CompanyContribution =
                            Convert.ToDecimal(Comp.CurrentValue);
                    }
                    var Retire = Pensions.Where(o => o.Attribute == PensionAttribute.RETIREMENT_AGE.ToString())
                        .FirstOrDefault();
                    if (Retire != null)
                    {
                        RetirementAge =
                            Convert.ToDecimal(Retire.CurrentValue);
                    }
                    var probation = Pensions.Where(o => o.Attribute == PensionAttribute.PROBATION_DAY_LENGTH.ToString())
                         .FirstOrDefault();
                    if (probation != null)
                    {
                        ProbationDateLenghth = Convert.ToInt32(probation.CurrentValue);
                    }

                    ApplyProbationSetting = true;
                    var ProbationSetting = db.Configurations.Where(o => o.Reference == "APPLY_PROBATION_DAY_LENGTH")
                        .FirstOrDefault();
                    if (ProbationSetting != null)
                    {
                        ApplyProbationSetting = Convert.ToBoolean(ProbationSetting.Attribute);
                    }

                    return true;
                }

            }
            catch (Exception e)
            {
                Logger.Error("Messsage: " + e.Message + "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
                return false;
            }
            return false;
        }
        private bool GetProvidentSetting()
        {

            var ProvidentFund = db.Configurations.Where(x => x.Reference == "PROVIDENT_FUND").ToList();
            try
            {
                if (ProvidentFund.Any())
                {
                    var Employ = ProvidentFund.Where(o => o.Attribute == ProvidentAttribute.PV_EMPLOYEE_CONTRIBUTION.ToString())
                        .FirstOrDefault();
                    if (Employ != null)
                    {
                        ProvEmployeeContribution =
                            Convert.ToDecimal(Employ.CurrentValue);
                    }
                    var Comp = ProvidentFund.Where(o => o.Attribute == ProvidentAttribute.PV_COMPANY_CONTRIBUTION.ToString())
                      .FirstOrDefault();
                    if (Comp != null)
                    {
                        ProvCompanyContribution =
                            Convert.ToDecimal(Comp.CurrentValue);
                    }


                    return true;
                }

            }
            catch (Exception e)
            {
                Logger.Error("Messsage: " + e.Message + "\n StackTrace: " + e.StackTrace + "\n TargetSite: " + e.TargetSite);
                return false;
            }
            return false;
        }
        [Authorize(Roles = Payroll_Process)]
        public ActionResult Reverse(int? id, int branchid)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "payroll process", "Success", User.Identity.Name, $"started payroll reverse for period {id} and branch {branchid} ", Modules.Payroll);
            #endregion
            var Period = db.GLPeriods.Find(id);
            PeriodStatus PeriodStatus = db.PeriodStatuses.Where(p => p.GLPeriodId == id && p.BranchId == branchid).FirstOrDefault();
            if (PeriodStatus.IsLocked && PeriodStatus.Status == PeriodStatusList.PROCESSING)
            {

                return new JavaScriptResult()
                {
                    Script = "alert('" + Period.Name +
                        " is in Processing status, please wait the untill the process is end." + "');"
                };
            }
            if (PeriodStatus.Status == PeriodStatusList.CLOSED)
            {
                string messages = "You cannot reverse closed Period!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            if (PeriodStatus.IsLocked && PeriodStatus.Status == PeriodStatusList.REVERSING)
            {

                return new JavaScriptResult()
                {
                    Script = "alert('" + Period.Name +
                             " is in Reversing status, please wait the untill the process is completed." + "');"
                };
            }
            if (Period == null)
            {
                GetProcessingClosedPeriods(new DataSourceRequest());
                RedirectToAction("Index");
                return HttpNotFound();
            }
            var processViewList = db.PayrollProcessViewModel.Where(x => x.GLPeriodId == id && x.BranchId == branchid).ToList();
            #region update period status

            PeriodStatus.Status = PeriodStatusList.REVERSING;
            PeriodStatus.IsLocked = true;
            db.Entry(PeriodStatus).State = EntityState.Modified;
            db.SaveChanges();
            #endregion
            #region reverse long term deduction

            foreach (var p in processViewList)
            {
                var longtermDeductions =
                    db.PayrollProcessLongTermDeductions.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                EmpPayrollLongTermDeduction longtermdata;
                foreach (PayrollProcessLongTermDeductions longterm in longtermDeductions)
                {
                    longtermdata = db.EmpPayrollLongTermDeductions.Where(x => x.EmployeeId == p.EmployeeId)
                        .Where(x => x.EmpPayrollLongTermDeductionId == longterm.EmpPayrollLongTermDeductionId).FirstOrDefault();
                    if (longtermdata != null)
                    {
                        longtermdata.Balance = longtermdata.Balance + longterm.Amount;
                        db.Entry(longtermdata).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            #endregion
            #region reverse loans

            foreach (var p in processViewList)
            {
                var loans = db.PayrollProcessLoans.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                Loan loandata;
                foreach (PayrollProcessLoans loan in loans)
                {

                    loandata = db.Loans.Where(x => x.EmployeeId == p.EmployeeId)
                        .Where(x => x.LoanId == loan.LoanId).FirstOrDefault();
                    if (loandata != null)
                    {

                        loandata.Balance = loandata.Balance + loan.Amount;
                        loandata.GLPeriodId = p.GLPeriodId;
                        loandata.EmployeeId = p.EmployeeId;
                        db.Entry(loandata).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            #endregion
            #region reverse advances

            foreach (var p in processViewList)
            {
                var advances = db.PayrollProcessAdvances.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                Advance advancedata;
                foreach (PayrollProcessAdvances advance in advances)
                {
                    advancedata = new Advance();
                    advancedata = db.Advances.Where(x => x.EmployeeId == p.EmployeeId)
                        .Where(x => x.AdvanceId == advance.AdvanceId).FirstOrDefault();
                    if (advancedata != null)
                    {
                        // db = new ExceedDbContext();
                        advancedata.Balance = advancedata.Balance + advance.Amount;
                        db.Entry(advancedata).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }

            #endregion
            #region Reverse fixed contribution
            ExceedDbContext dbfix = new ExceedDbContext();
            foreach (var p in processViewList)
            {
                dbfix = new ExceedDbContext();
                var fixedcontributions =
                    dbfix.PayrollProcessFixedContribution.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                FixedContribution fixedcontributiondata;
                foreach (PayrollProcessFixedContribution fixedcontribution in fixedcontributions)
                {

                    var contribution =
                       db.ContributionTypes.Where(l => l.Description == fixedcontribution.Name).FirstOrDefault();
                    if (contribution != null)
                    {
                        var contributionid = contribution.ContributionTypeId;
                        fixedcontributiondata = dbfix.FixedContributions.Where(x => x.EmployeeId == p.EmployeeId && x.IsActive)
                            .Where(x => x.FixedContributionId == fixedcontribution.FixedContributionId).FirstOrDefault();
                        if (fixedcontributiondata != null)
                        {

                            fixedcontributiondata.Balance = fixedcontributiondata.Balance + fixedcontribution.Amount;
                            dbfix.Entry(fixedcontributiondata).State = EntityState.Modified;
                            dbfix.SaveChanges();
                        }
                    }
                }
            }

            #endregion
            #region Reverse Penality
            ExceedDbContext dbpen = new ExceedDbContext();
            foreach (var p in processViewList)
            {
                dbpen = new ExceedDbContext();
                var penalities =
                    dbpen.PayrollProcessDeductions.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId && x.Remark ==
                        "Discipline").ToList();
                DisciplineMeasuerByMoney PenalityData;
                foreach (PayrollProcessDeductions contribution in penalities)
                {


                    PenalityData = dbpen.DisciplineMeasuerByMoney.Where(x => x.EmployeeId == p.EmployeeId && x.DisciplinaryCaseId.ToString() == contribution.Name)
                       .FirstOrDefault();
                    if (PenalityData != null)
                    {

                        PenalityData.Balance = PenalityData.Balance + contribution.Amount;
                        dbpen.Entry(PenalityData).State = EntityState.Modified;
                        dbpen.SaveChanges();
                    }
                }
            }

            #endregion
            #region remove sub data

            foreach (var p in processViewList)
            {
                var record1 = db.PayrollProcessAllowances.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                db.PayrollProcessAllowances.RemoveRange(record1);
            }
            foreach (var p in processViewList)
            {
                var record2 = db.PayrollProcessAdditions.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                db.PayrollProcessAdditions.RemoveRange(record2);
            }
            foreach (var p in processViewList)
            {
                var record3 = db.PayrollProcessDeductions.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                db.PayrollProcessDeductions.RemoveRange(record3);

                var record4 = db.PayrollProcessLongTermDeductions.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                db.PayrollProcessLongTermDeductions.RemoveRange(record4);

                var record5 = db.PayrollProcessLoans.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                db.PayrollProcessLoans.RemoveRange(record5);

                var record6 = db.PayrollProcessFixedContribution.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                db.PayrollProcessFixedContribution.RemoveRange(record6);

                var record7 = db.PayrollProcessRegularContribution.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                db.PayrollProcessRegularContribution.RemoveRange(record7);

                var record8 = db.PayrollProcessAdvances.Where(x => x.PayrollProcessViewModelId == p.PayrollProcessViewModelId).ToList();
                db.PayrollProcessAdvances.RemoveRange(record8);
            }
            db.SaveChanges();
            #endregion
            #region remove distribution data
            var record9 = db.PayrollDistributions.Where(x => x.GLPeriodId == Period.GLPeriodId).ToList();
            db.PayrollDistributions.RemoveRange(record9);
            db.SaveChanges();
            #endregion
            #region update period status

            PeriodStatus.Status = PeriodStatusList.NEW;
            PeriodStatus.IsLocked = false;
            db.Entry(PeriodStatus).State = EntityState.Modified;
            db.SaveChanges();
            #endregion
            #region remove main data


            db.PayrollProcessViewModel.RemoveRange(processViewList);
            db.SaveChanges();
            #endregion
            GetProcessingClosedPeriods(new DataSourceRequest());
            //RedirectToAction("Index");
            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "payroll process", "Success", User.Identity.Name, $"completed payroll reverse for period {id} and branch {branchid} ", Modules.Payroll);
            #endregion
            string message = "Successfully Reversed!!";
            return new JavaScriptResult() { Script = "alert('" + message + "');" };
        }

        [Authorize(Roles = Payroll_Close)]
        public ActionResult Close(int? id, int branchId)
        {
            string message = "";
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            #region check if debit and credit are equal
            decimal TDebit = 0;
            decimal TCredit = 0;
            var distribution = db.PayrollDistributions.ToList();
            if (distribution.Any())
            {
                TDebit = distribution.Where(x => x.GLPeriodId == id).Sum(x => x.Debit);
                TCredit = distribution.Where(x => x.GLPeriodId == id).Sum(x => x.Credit);
            }
            else
            {
                message = "You should to distribute the process first!!";
                return new JavaScriptResult() { Script = "alert('" + message + "');" };
            }


            if (Math.Round(TDebit, 2) != Math.Round(TCredit, 2))
            {
                message = "Cannot close this Process.Total debit and total credit is not balanced.";
                return new JavaScriptResult() { Script = "alert('" + message + "');" };
            }
            #endregion
            var Period = db.GLPeriods.Find(id);

            if (Period != null)
            {
                var glperiod = db.GLPeriods.Where(x => x.Name == Period.Name).FirstOrDefault();
                if (glperiod != null)
                {
                    id = glperiod.GLPeriodId;
                }


            }
            PeriodStatus PeriodStatus = db.PeriodStatuses.Where(p => p.GLPeriodId == id && p.BranchId == branchId).FirstOrDefault();
            if (PeriodStatus == null)
            {
                return HttpNotFound();
            }
            #region update period status
            PeriodStatus.Status = PeriodStatusList.CLOSED;
            db.Entry(PeriodStatus).State = EntityState.Modified;
            db.SaveChanges();
            #endregion

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "payroll process", "Success", User.Identity.Name, $"completed payroll close for period {id} and branch {branchId} ", Modules.Payroll);
            #endregion

            message = "Successfully Closed!!";
            //  RedirectToAction("Index");
            return new JavaScriptResult() { Script = "alert('" + message + "');" };

        }

        private string distributed = "true";
        [Authorize(Roles = Payroll_Distribute)]
        public string Distribution(int id, int branchid)
        {
            string PeiodName = "";
            string debit = "";
            string credit = "";
            if (id == 0)
            {
                distributed = "false";
                return distributed;
            }
            var Period = db.GLPeriods.Find(id);


            if (Period != null)
            {
                var glperiod = db.GLPeriods.Where(x => x.Name == Period.Name).FirstOrDefault();
                if (glperiod != null)
                {
                    id = glperiod.GLPeriodId;
                }


            }
            if (Period == null)
            {
                distributed = "false";
                return distributed;
            }
            var PeriodStatus = db.PeriodStatuses.Where(x => x.GLPeriodId == id && x.BranchId == branchid).FirstOrDefault();
            string message = "";
            if (PeriodStatus.Status != PeriodStatusList.CLOSED && PeriodStatus.Status != PeriodStatusList.TRANSFERRED)
            {
                if (id != 0)
                {
                    UpdateDistributionModified(branchid, id);
                }
            }
            PayrollDistributionController.PeriodId = id;
            PayrollDistributionController.BranchId = branchid;
            List<PayrollDistribution> PayrollDistributions = db.PayrollDistributions.Where(p => p.GLPeriodId == id && p.BranchId == branchid).ToList();
            var period = db.GLPeriods.Where(x => x.GLPeriodId == id).FirstOrDefault();




            if (period != null)
            {
                var glperiod = db.GLPeriods.Where(x => x.Name == Period.Name).FirstOrDefault();
                if (glperiod != null)
                {
                    id = glperiod.GLPeriodId;
                }
                PeiodName = period.Name;

            }


            debit = Convert.ToDecimal(PayrollDistributions.Where(x => x.GLPeriodId == id).Sum(x => x.Debit)).ToString("#,##0.00");
            credit = Convert.ToDecimal(PayrollDistributions.Where(x => x.GLPeriodId == id).Sum(x => x.Credit)).ToString("#,##0.00");

            return distributed;
            // return View("OpenDistribution",);
        }
        ////////////////////////////Fianance/////////////////////////////////////////////////////////

        private ActionResult UpdateDistributionModified(int branchId, int id = 0)
        {
            var record = db.GLPeriods.Find(id);
            decimal amount = 0;
            string message = "";
            var payrollAccounts = db.PayrollAccounts.ToList();
            var chartOfAccounts = db.GLChartOfAccountAccounts.ToList();
            PayrollDistribution distribution = new PayrollDistribution();
            List<PayrollDistribution> payrollDistributions = new List<PayrollDistribution>();
            List<PayrollDistribution> splittedDistributions = new List<PayrollDistribution>();

            PayrollDistributionController.PeriodId = id;
            PayrollDistributionController.BranchId = branchId;
            var employeeAccounts = db.PayrollEmployeeAccounts.ToList();
            string defaultBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            var regularDistributions = db.PayrollProcessRegularContribution.ToList();
            var fisedDistributions = db.PayrollProcessFixedContribution.ToList();
            var additionLists = db.PayrollProcessAdditions.ToList();
            if (record != null)
            {
                var glperiod = db.GLPeriods.Where(x => x.Name == record.Name).FirstOrDefault();
                if (glperiod != null)
                {
                    id = glperiod.GLPeriodId;
                    //remove exiting distribution
                    var oldrecord =
                        db.PayrollDistributions.Where(
                            x => x.GLPeriodId == record.GLPeriodId && x.BranchId == branchId && x.isMannual == false)
                            .ToList();
                    db.PayrollDistributions.RemoveRange(oldrecord);
                    db.SaveChanges();


                    var payrolldatas =
                        db.PayrollProcessViewModel.Where(x => x.PeriodName == record.Name && x.BranchId == branchId)
                            .ToList();

                    string account = "";


                    foreach (var accounts in payrollAccounts)
                    {
                        AccountHelper accountHelper = new AccountHelper();
                        db = new ExceedDbContext();
                        //payeDistributions = db.PayrollDistributions.Where(x => x.GLPeriodId==id).ToList();
                        if (accounts.Account == null)
                        {
                            message = "Please Set Account For " + accounts.Description;
                            // RedirectToAction("Index");
                            distributed = "false";
                            return new JavaScriptResult() { Script = "alert('" + message + "');" };
                        }
                        string accountMain = accountHelper.GetNaturalAccountSegment(accounts.Account);
                        string branch = accountHelper.GetBranchSegment(accounts.Account);
                        string costCneter = accountHelper.GetCostCenterSegment(accounts.Account);
                        string SubAccCode = accountHelper.GetSubAccountSegment(accounts.Account);
                        var empAccounts = employeeAccounts.Where(x => x.CostCenterAccountCode == costCneter);


                        distribution = new PayrollDistribution();
                        distribution.CostCenterCode = costCneter;
                        distribution.LocationCode = branch;
                        distribution.OrgBranchName = defaultBranch;
                        distribution.GLPeriodId = id;
                        distribution.BranchId = branchId;

                        if (accountHelper.IsDynamicCoA())
                        {
                            distribution.AccountCode = accounts.Account;
                        }
                        else
                        {
                            distribution.AccountCode = accountHelper.RearrangeAccountSequence(accountMain, branch, costCneter, SubAccCode);
                        }
                        distribution.AccountDesc = accounts.Description;
                        var payrolldatass = new List<PayrollProcessViewModel>();
                        if (!accounts.IsPerCostCenter && !accounts.IsPerEmployee)
                        {
                            payrolldatass = payrolldatas;
                        }

                        payrolldatass = payrolldatas;
                        //.Where(x => x.OrgBranchAccountCode == branch && x.CostCenterAccountCode == costCneter).ToList();

                        var costcnetrlist = payrolldatas.Select(x => x.CostCenterAccountCode).Distinct();
                        var branchAccountList = payrolldatas.Select(x => x.LocationCode).Distinct();


                        switch (accounts.Description)
                        {
                            #region "BASIC_SALARY_PERMANENT"

                            case "BASIC_SALARY_PERMANENT":
                                if (accounts.IsPerEmployee)
                                {
                                    payrolldatass =
                                        payrolldatas.Where(x => x.Type == EmployeeStatus.PERMANENT).ToList();
                                    foreach (var p in payrolldatass)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;

                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter, emp.SubAccCode);

                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.BasicSalary - p.AbsentDeduction;//-added
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {

                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    payrolldatass =
                                        payrolldatas.Where(x => x.Type == EmployeeStatus.PERMANENT).ToList();
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {

                                            var p =
                                                payrolldatass.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() &&
                                                        x.LocationCode == b.ToString());
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName = defaultBranch;

                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;

                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                        SubAccCode);
                                                }
                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = b;
                                                distribution.Debit = p.Sum(x => x.BasicSalary) - p.Sum(x => x.AbsentDeduction);//-added

                                                distribution.Credit = 0;
                                                distribution.DateCreated = DateTime.Now;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) && checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }



                                            }


                                        }
                                    }
                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    payrolldatass =
                                        payrolldatas.Where(x => x.Type == EmployeeStatus.PERMANENT).ToList();
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p = payrolldatass.Where(x => x.CostCenterAccountCode == c.ToString());
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c.ToString(),
                                                    SubAccCode);
                                            }
                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.Sum(x => x.BasicSalary) - p.Sum(x => x.AbsentDeduction);//-added
                                            ;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }



                                        }


                                    }
                                }
                                else if (accounts.IsPerBranch)
                                {
                                    payrolldatass =
                                        payrolldatas.Where(x => x.Type == EmployeeStatus.PERMANENT).ToList();
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatass.Where(x => x.LocationCode == c.ToString());
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                                                                SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Debit = p.Sum(x => x.BasicSalary) - p.Sum(x => x.AbsentDeduction);//-added
                                            ;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }



                                        }


                                    }
                                }
                                else
                                {
                                    distribution.Debit =
                                        payrolldatass.Where(x => x.Type == EmployeeStatus.PERMANENT)
                                            .Sum(x => x.BasicSalary);
                                    distribution.Credit = 0;
                                }

                                break;

                            #endregion

                            #region "BASIC_SALARY_CONRACT"

                            case "BASIC_SALARY_CONRACT":
                                if (accounts.IsPerEmployee)
                                {
                                    payrolldatass =
                                        payrolldatas.Where(x => x.Type != EmployeeStatus.PERMANENT).ToList();
                                    foreach (var p in payrolldatass)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();

                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                   emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.BasicSalary - p.AbsentDeduction;//-added
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    payrolldatass =
                                        payrolldatas.Where(x => x.Type != EmployeeStatus.PERMANENT).ToList();
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatass.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b);
                                            if (p != null)
                                            {

                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                  SubAccCode);
                                                }

                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = b;
                                                distribution.Debit = p.Sum(x => x.BasicSalary) - p.Sum(x => x.AbsentDeduction);//-added
                                                ;
                                                distribution.Credit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }



                                            }


                                        }
                                    }
                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    payrolldatass =
                                        payrolldatas.Where(x => x.Type != EmployeeStatus.PERMANENT).ToList();
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p = payrolldatass.Where(x => x.CostCenterAccountCode == c.ToString());
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c.ToString(),
                                                 SubAccCode);
                                            }

                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.Sum(x => x.BasicSalary) - p.Sum(x => x.AbsentDeduction);//-added
                                            ;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }



                                        }


                                    }
                                }
                                else if (accounts.IsPerBranch)
                                {
                                    payrolldatass =
                                        payrolldatas.Where(x => x.Type != EmployeeStatus.PERMANENT).ToList();
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatass.Where(x => x.LocationCode == c.ToString());
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }

                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Debit = p.Sum(x => x.BasicSalary) - p.Sum(x => x.AbsentDeduction);//-added
                                            ;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }



                                        }


                                    }
                                }
                                else
                                {
                                    distribution.Debit =
                                        payrolldatass.Where(x => x.Type != EmployeeStatus.PERMANENT)
                                            .Sum(x => x.BasicSalary);
                                    distribution.Credit = 0;
                                }

                                break;

                            #endregion

                            #region  "OVER_TIME"

                            case "OVER_TIME":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter, emp.SubAccCode);
                                            }

                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.TotalOvertime;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }

                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = b;
                                                distribution.Debit = p.Sum(x => x.TotalOvertime);
                                                distribution.Credit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }


                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c, SubAccCode);
                                            }

                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.Sum(x => x.TotalOvertime);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);

                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter, SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Debit = p.Sum(x => x.TotalOvertime);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Debit = payrolldatass.Sum(x => x.TotalOvertime);
                                    distribution.Credit = 0;
                                }

                                break;

                            #endregion

                            #region "INCOME_TAX"

                            case "INCOME_TAX":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.IncomeTax;
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = b;
                                                distribution.Credit = p.Sum(x => x.IncomeTax);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }


                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.Sum(x => x.IncomeTax);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Credit = p.Sum(x => x.IncomeTax);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = payrolldatass.Sum(x => x.IncomeTax);
                                    distribution.Debit = 0;
                                }

                                break;

                            #endregion

                            #region "PENSION_COMPANY"

                            case "PENSION_COMPANY":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = 0;
                                            distribution.Credit = p.PensionFromCompany;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = b;
                                                distribution.Debit = 0;
                                                distribution.Credit = p.Sum(x => x.PensionFromCompany);
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }


                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c, SubAccCode);
                                            }
                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Debit = 0;
                                            distribution.Credit = p.Sum(x => x.PensionFromCompany);
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                                                                  SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Debit = 0;
                                            distribution.Credit = p.Sum(x => x.PensionFromCompany);
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Debit = 0;
                                    distribution.Credit = payrolldatass.Sum(x => x.PensionFromCompany);
                                }

                                break;

                            #endregion

                            #region "PROVIDENT_COMPANY"

                            case "PROVIDENT_COMPANY":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                  emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = 0;
                                            distribution.Credit = p.ProvidentFromCompany;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = b;
                                                distribution.Debit = 0;
                                                distribution.Credit = p.Sum(x => x.ProvidentFromCompany);
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }


                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }

                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Debit = 0;
                                            distribution.Credit = p.Sum(x => x.ProvidentFromCompany);
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                  SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Debit = 0;
                                            distribution.Credit = p.Sum(x => x.ProvidentFromCompany);
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Debit = 0;
                                    distribution.Credit = payrolldatass.Sum(x => x.ProvidentFromCompany);
                                }

                                break;

                            #endregion

                            #region "LOAN"

                            case "LOAN":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.TotalLoan;
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = branch;
                                                distribution.Credit = p.Sum(x => x.TotalLoan);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }

                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.Sum(x => x.TotalLoan);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Credit = p.Sum(x => x.TotalLoan);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = payrolldatass.Sum(x => x.TotalLoan);
                                    distribution.Debit = 0;
                                }
                                break;

                            #endregion

                            #region "ADVANCE"

                            case "ADVANCE":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.TotalAdvance;
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = b;
                                                distribution.Credit = p.Sum(x => x.TotalAdvance);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c, SubAccCode);
                                            }
                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.Sum(x => x.TotalAdvance);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter, SubAccCode);

                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Credit = p.Sum(x => x.TotalAdvance);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = payrolldatass.Sum(x => x.TotalAdvance);
                                    distribution.Debit = 0;
                                }

                                break;

                            #endregion

                            #region "NET_PAY"

                            case "NET_PAY":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.NetEarning;
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = b;
                                                distribution.Credit = p.Sum(x => x.NetEarning);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }


                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.Sum(x => x.NetEarning);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Credit = p.Sum(x => x.NetEarning);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = payrolldatass.Sum(x => x.NetEarning);
                                    distribution.Debit = 0;
                                }

                                break;

                            #endregion

                            #region "PENSION_PAYABLE"

                            case "PENSION_PAYABLE":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.PensionFromEmployee;
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = b;
                                                distribution.Credit = p.Sum(x => x.PensionFromEmployee);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c, SubAccCode);
                                            }
                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.Sum(x => x.PensionFromEmployee);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Credit = p.Sum(x => x.PensionFromEmployee);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);

                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = payrolldatass.Sum(x => x.PensionFromEmployee);
                                    distribution.Debit = 0;
                                }

                                break;


                            #endregion

                            #region  "PROVIDENT_PAYABLE"

                            case "PROVIDENT_PAYABLE":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.ProvidentFromEmployee;
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = b;
                                                distribution.Credit = p.Sum(x => x.ProvidentFromEmployee);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.Sum(x => x.ProvidentFromEmployee);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Credit = p.Sum(x => x.ProvidentFromEmployee);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = payrolldatass.Sum(x => x.ProvidentFromEmployee);
                                    distribution.Debit = 0;
                                }

                                break;

                            #endregion

                            #region "ASSOCIATION"

                            case "ASSOCIATION":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.AssocationContribution;
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c.ToString();
                                                distribution.LocationCode = b;
                                                distribution.Credit = p.Sum(x => x.AssocationContribution);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }


                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c.ToString();
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.Sum(x => x.AssocationContribution);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c.ToString();
                                            distribution.Credit = p.Sum(x => x.AssocationContribution);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = payrolldatass.Sum(x => x.AssocationContribution);
                                    distribution.Debit = 0;
                                }

                                break;

                            #endregion

                            #region  "LABOR_UNION"

                            case "LABOR_UNION":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.LabourUnionContribution;
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Credit = p.Sum(x => x.LabourUnionContribution);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.Sum(x => x.LabourUnionContribution);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Credit = p.Sum(x => x.LabourUnionContribution);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = payrolldatass.Sum(x => x.LabourUnionContribution);
                                    distribution.Debit = 0;
                                }

                                break;

                            #endregion

                            #region "TRANSPORT"

                            case "TRANSPORT":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.TotalTransportAllowance;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Debit = p.Sum(x => x.TotalTransportAllowance);
                                                distribution.Credit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.Sum(x => x.TotalTransportAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Debit = p.Sum(x => x.TotalTransportAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Debit = payrolldatass.Sum(x => x.TotalTransportAllowance);
                                    distribution.Credit = 0;
                                }


                                break;

                            #endregion

                            #region "TELEPHONE"

                            case "TELEPHONE":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.TotalTelephoneAllowance;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Debit = p.Sum(x => x.TotalTelephoneAllowance);
                                                distribution.Credit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }


                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.Sum(x => x.TotalTelephoneAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Debit = p.Sum(x => x.TotalTelephoneAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Debit = payrolldatass.Sum(x => x.TotalTelephoneAllowance);
                                    distribution.Credit = 0;
                                }

                                break;

                            #endregion

                            #region "HOME"

                            case "HOME":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                                                                  emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.TotalHouseAllowance;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Debit = p.Sum(x => x.TotalHouseAllowance);
                                                distribution.Credit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }
                                        }
                                    }


                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.Sum(x => x.TotalHouseAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Debit = p.Sum(x => x.TotalHouseAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Debit = payrolldatass.Sum(x => x.TotalHouseAllowance);
                                    distribution.Credit = 0;
                                }


                                break;

                            #endregion

                            #region "RESPONSIBILITY"

                            case "RESPONSIBILITY":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.TotalResponsiblityAllowance;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Debit = p.Sum(x => x.TotalResponsiblityAllowance);
                                                distribution.Credit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.Sum(x => x.TotalResponsiblityAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Debit = p.Sum(x => x.TotalResponsiblityAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Debit = payrolldatass.Sum(x => x.TotalResponsiblityAllowance);
                                    distribution.Credit = 0;
                                }

                                break;

                            #endregion

                            #region "POSITION"

                            case "POSITION":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.TotalPositionAllowance;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Debit = p.Sum(x => x.TotalPositionAllowance);
                                                distribution.Credit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.Sum(x => x.TotalPositionAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Debit = p.Sum(x => x.TotalPositionAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Debit = payrolldatass.Sum(x => x.TotalPositionAllowance);
                                    distribution.Credit = 0;
                                }

                                break;

                            #endregion

                            #region "PROFESSIONAL"

                            case "PROFESSIONAL":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.TotalProfessionalAllowance;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Debit = p.Sum(x => x.TotalProfessionalAllowance);
                                                distribution.Credit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.Sum(x => x.TotalProfessionalAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Debit = p.Sum(x => x.TotalProfessionalAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Debit = payrolldatass.Sum(x => x.TotalProfessionalAllowance);
                                    distribution.Credit = 0;
                                }

                                break;

                            #endregion

                            #region "TOP_UP"

                            case "TOP_UP":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);

                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.TotalTopUpAllowance;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Debit = p.Sum(x => x.TotalTopUpAllowance);
                                                distribution.Credit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);

                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.Sum(x => x.TotalTopUpAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);

                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Debit = p.Sum(x => x.TotalTopUpAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Debit = payrolldatass.Sum(x => x.TotalTopUpAllowance);
                                    distribution.Credit = 0;
                                }

                                break;

                            #endregion
                            #region "DESERT"

                            case "DESERT":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.TotalDesertAllowance;
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);

                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Debit = p.Sum(x => x.TotalDesertAllowance);
                                                distribution.Credit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);

                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Debit = p.Sum(x => x.TotalDesertAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);

                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Debit = p.Sum(x => x.TotalDesertAllowance);
                                            distribution.Credit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Debit = payrolldatass.Sum(x => x.TotalDesertAllowance);
                                    distribution.Credit = 0;
                                }

                                break;

                            #endregion
                            #region "PENSION_EXPENSE"

                            case "PENSION_EXPENSE":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);

                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = 0;
                                            distribution.Debit = p.PensionFromCompany;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);

                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Credit = 0;
                                                distribution.Debit = p.Sum(x => x.PensionFromCompany);
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);

                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = 0;
                                            distribution.Debit = p.Sum(x => x.PensionFromCompany);
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);

                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Credit = 0;
                                            distribution.Debit = p.Sum(x => x.PensionFromCompany);
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = 0;
                                    distribution.Debit = payrolldatass.Sum(x => x.PensionFromCompany);
                                }

                                break;

                            #endregion

                            #region "PROVIDENT_EXPENSE"

                            case "PROVIDENT_EXPENSE":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);

                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = 0;
                                            distribution.Debit = p.ProvidentFromCompany;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Credit = 0;
                                                distribution.Debit = p.Sum(x => x.ProvidentFromCompany);
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = 0;
                                            distribution.Debit = p.Sum(x => x.ProvidentFromCompany);
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Credit = 0;
                                            distribution.Debit = p.Sum(x => x.ProvidentFromCompany);
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = 0;
                                    distribution.Debit = payrolldatass.Sum(x => x.ProvidentFromCompany);
                                }

                                break;

                            #endregion

                            #region "EMPLOYEE_PENALITY"

                            case "EMPLOYEE_PENALITY":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.TotalPenality;
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {

                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Credit = p.Sum(x => x.TotalPenality);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.Sum(x => x.TotalPenality);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);

                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Credit = p.Sum(x => x.TotalPenality);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = payrolldatass.Sum(x => x.TotalPenality);
                                    distribution.Debit = 0;
                                }

                                break;

                            #endregion

                            #region "SPORT_FEE"

                            case "SPORT_FEE":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);

                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.SportFee;
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Credit = p.Sum(x => x.SportFee);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.Sum(x => x.SportFee);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Credit = p.Sum(x => x.SportFee);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = payrolldatass.Sum(x => x.SportFee);
                                    distribution.Debit = 0;
                                }

                                break;

                            #endregion

                            #region "OTHER"

                            case "OTHER":
                                if (accounts.IsPerEmployee)
                                {
                                    foreach (var p in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == p.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.AbsentDeduction;
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var p =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() && x.LocationCode == b)
                                                    .ToList();
                                            if (p != null)
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);

                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Credit = p.Sum(x => x.AbsentDeduction);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExe =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExe == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }

                                        }
                                    }

                                }
                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);

                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = p.Sum(x => x.AbsentDeduction);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var p =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();
                                        if (p != null)
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Credit = p.Sum(x => x.AbsentDeduction);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExe =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExe == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                    }


                                }
                                else
                                {
                                    distribution.Credit = payrolldatass.Sum(x => x.AbsentDeduction);
                                    distribution.Debit = 0;
                                }

                                break;

                            #endregion

                            #region "Default"

                            default:


                                if (accounts.IsPerEmployee)
                                {


                                    foreach (var v in payrolldatas)
                                    {
                                        var emp =
                                            employeeAccounts.Where(x => x.EmployeeId == v.EmployeeId).FirstOrDefault();
                                        if (emp != null)
                                        {
                                            #region Regular Contribution
                                            var regular = regularDistributions.Where(
                                                x =>
                                                    x.PayrollProcessViewModelId == v.PayrollProcessViewModelId &&
                                                    accounts.Description == x.Name).ToList();
                                            if (regular.Any())
                                            {


                                                AccountHelper myAccountHelper = new AccountHelper();

                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName = defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                                }
                                                distribution.CostCenterCode = costCneter;
                                                distribution.LocationCode = branch;
                                                distribution.Credit = regular.Sum(x => x.Amount);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExer =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExer == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }
                                            }
                                            #endregion
                                            #region Fixed COntribution


                                            var fixeds =
                                                fisedDistributions.Where(
                                                    x =>
                                                        x.PayrollProcessViewModelId == v.PayrollProcessViewModelId &&
                                                        accounts.Description == x.Name).ToList();
                                            if (fixeds.Any())
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName = defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);

                                                }
                                                distribution.CostCenterCode = costCneter;
                                                distribution.LocationCode = branch;
                                                distribution.Credit = fixeds.Sum(x => x.Amount);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExef =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExef == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }
                                            #endregion
                                            #region Addition Type


                                            var additions = additionLists.Where(x => x.PayrollProcessViewModelId == v.PayrollProcessViewModelId &&
                                                         accounts.Description == x.Name).ToList();
                                            if (additions.Any())
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName = defaultBranch;
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetSubAccountSequenceIndex(), emp.SubAccCode);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, costCneter,
                                                      emp.SubAccCode);
                                                }
                                                distribution.CostCenterCode = costCneter;
                                                distribution.LocationCode = branch;
                                                distribution.Credit = 0;
                                                distribution.Debit = additions.Sum(x => x.Amount);
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExef =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExef == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }
                                            #endregion

                                        }

                                    }

                                }
                                else if (accounts.IsPerCostCenter && accounts.IsPerBranch)
                                {
                                    foreach (var b in branchAccountList)
                                    {
                                        foreach (var c in costcnetrlist)
                                        {
                                            var all =
                                                payrolldatas.Where(
                                                    x =>
                                                        x.CostCenterAccountCode == c.ToString() &&
                                                        x.LocationCode == b)
                                                    .ToList();


                                            #region Regural Contribution
                                            var regulars = from r in regularDistributions
                                                           join p in all
                                                               on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                           where r.Name == accounts.Description
                                                           select (new { r.Amount });




                                            if (regulars.Any())
                                            {



                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    UserInformationHelper.GetUserDefaultBranch(
                                                        User.Identity.Name);
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = branch;
                                                distribution.Credit = regulars.Sum(x => x.Amount);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExer =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExer == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }
                                            }

                                            #endregion

                                            #region Fixed 
                                            var fixedss = from r in fisedDistributions
                                                          join p in all
                                                              on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                          where r.Name == accounts.Description
                                                          select (new { r.Amount });
                                            if (fixedss.Any())
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    UserInformationHelper.GetUserDefaultBranch(
                                                        User.Identity.Name);
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Credit = fixedss.Sum(x => x.Amount);
                                                distribution.Debit = 0;
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExef =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExef == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }
                                            #endregion

                                            #region Addition 
                                            var additions = from r in additionLists
                                                            join p in all
                                                                on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                            where r.Name == accounts.Description
                                                            select (new { r.Amount });
                                            if (additions.Any())
                                            {
                                                AccountHelper myAccountHelper = new AccountHelper();
                                                distribution = new PayrollDistribution();
                                                distribution.OrgBranchName =
                                                    UserInformationHelper.GetUserDefaultBranch(
                                                        User.Identity.Name);
                                                distribution.GLPeriodId = id;
                                                distribution.BranchId = branchId;
                                                distribution.AccountDesc = accounts.Description;
                                                if (accountHelper.IsDynamicCoA())
                                                {
                                                    string branchreplaced = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), b);
                                                    distribution.AccountCode = dynamicAccount.ReplaceSegment(branchreplaced, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                                }
                                                else
                                                {
                                                    distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, b, c.ToString(),
                                                      SubAccCode);
                                                }
                                                distribution.CostCenterCode = c;
                                                distribution.LocationCode = b;
                                                distribution.Credit = 0;
                                                distribution.Debit = additions.Sum(x => x.Amount);
                                                distribution.DateCreated = DateTime.Now.Date;
                                                distribution.LastModified = null;
                                                distribution.CreatedBy = User.Identity.Name;
                                                distribution.ModifiedBy = null;
                                                var checkExef =
                                                    payrollDistributions.Where(
                                                        x =>
                                                            x.AccountDesc == distribution.AccountDesc &&
                                                            x.Debit == distribution.Debit &&
                                                            x.Credit == distribution.Credit &&
                                                            x.AccountCode == distribution.AccountCode)
                                                        .FirstOrDefault();

                                                if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                    checkExef == null)
                                                {
                                                    //db.PayrollDistributions.Add(distribution);
                                                    //db.SaveChanges();
                                                    payrollDistributions.Add(distribution);
                                                }

                                            }
                                            #endregion
                                        }


                                    }
                                }




                                else if (accounts.IsPerCostCenter)
                                {
                                    foreach (var c in costcnetrlist)
                                    {
                                        var all =
                                            payrolldatas.Where(x => x.CostCenterAccountCode == c.ToString()).ToList();

                                        var regulars = from r in regularDistributions
                                                       join p in all
                                                           on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                       where r.Name == accounts.Description
                                                       select (new { r.Amount });
                                        #region Regular


                                        if (regulars.Any())
                                        {



                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = regulars.Sum(x => x.Amount);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExer =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExer == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }
                                        }
                                        #endregion


                                        #region FixedS
                                        var fixedss = from r in fisedDistributions
                                                      join p in all
                                                          on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                      where r.Name == accounts.Description
                                                      select (new { r.Amount });
                                        if (fixedss.Any())
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = fixedss.Sum(x => x.Amount);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExef =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExef == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }
                                        #endregion

                                        #region Addition
                                        var additions = from r in additionLists
                                                        join p in all
                                                            on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                        where r.Name == accounts.Description
                                                        select (new { r.Amount });
                                        if (additions.Any())
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetCostCenterAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, branch, c,
                                                      SubAccCode);

                                            }
                                            distribution.CostCenterCode = c;
                                            distribution.LocationCode = branch;
                                            distribution.Credit = 0;
                                            distribution.Debit = additions.Sum(x => x.Amount);
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExef =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExef == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }
                                        #endregion


                                    }

                                }
                                else if (accounts.IsPerBranch)
                                {
                                    foreach (var c in branchAccountList)
                                    {
                                        var all = payrolldatas.Where(x => x.LocationCode == c.ToString()).ToList();
                                        #region Regular
                                        var regulars = from r in regularDistributions
                                                       join p in all
                                                           on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                       where r.Name == accounts.Description
                                                       select (new { r.Amount });


                                        if (regulars.Any())
                                        {



                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c.ToString(), costCneter,
                                                      SubAccCode);

                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Credit = regulars.Sum(x => x.Amount);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExer =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExer == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }
                                        }
                                        #endregion

                                        #region Fixed
                                        var fixedss = from r in fisedDistributions
                                                      join p in all
                                                          on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                      where r.Name == accounts.Description
                                                      select (new { r.Amount });

                                        if (fixedss.Any())
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c, costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Credit = fixedss.Sum(x => x.Amount);
                                            distribution.Debit = 0;
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExef =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExef == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                        #endregion

                                        #region Additions
                                        var additions = from r in additionLists
                                                        join p in all
                                                            on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                        where r.Name == accounts.Description
                                                        select (new { r.Amount });

                                        if (additions.Any())
                                        {
                                            AccountHelper myAccountHelper = new AccountHelper();
                                            distribution = new PayrollDistribution();
                                            distribution.OrgBranchName = defaultBranch;
                                            distribution.GLPeriodId = id;
                                            distribution.BranchId = branchId;
                                            distribution.AccountDesc = accounts.Description;
                                            if (accountHelper.IsDynamicCoA())
                                            {
                                                distribution.AccountCode = dynamicAccount.ReplaceSegment(accounts.Account, dynamicAccount.GetBranchAccountSequenceIndex(), c);
                                            }
                                            else
                                            {
                                                distribution.AccountCode = myAccountHelper.RearrangeAccountSequence(accountMain, c, costCneter,
                                                      SubAccCode);
                                            }
                                            distribution.CostCenterCode = costCneter;
                                            distribution.LocationCode = c;
                                            distribution.Credit = 0;
                                            distribution.Debit = additions.Sum(x => x.Amount);
                                            distribution.DateCreated = DateTime.Now.Date;
                                            distribution.LastModified = null;
                                            distribution.CreatedBy = User.Identity.Name;
                                            distribution.ModifiedBy = null;
                                            var checkExef =
                                                payrollDistributions.Where(
                                                    x =>
                                                        x.AccountDesc == distribution.AccountDesc &&
                                                        x.Debit == distribution.Debit &&
                                                        x.Credit == distribution.Credit &&
                                                        x.AccountCode == distribution.AccountCode).FirstOrDefault();

                                            if ((distribution.Debit != 0 || distribution.Credit != 0) &&
                                                checkExef == null)
                                            {
                                                //db.PayrollDistributions.Add(distribution);
                                                //db.SaveChanges();
                                                payrollDistributions.Add(distribution);
                                            }

                                        }

                                        #endregion
                                    }

                                }
                                else
                                {

                                    #region Regular
                                    var regulars = from r in regularDistributions
                                                   join p in payrolldatas
                                                       on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                   where r.Name == accounts.Description
                                                   select (new { r.Amount });


                                    if (regulars.Any())
                                    {

                                        distribution.Credit += regulars.Sum(x => x.Amount);
                                        distribution.Debit = 0;
                                    }
                                    #endregion
                                    #region Fixed
                                    var fixedss = from r in fisedDistributions
                                                  join p in payrolldatas
                                                      on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                  where r.Name == accounts.Description
                                                  select (new { r.Amount });

                                    if (fixedss.Any())
                                    {

                                        distribution.Credit += fixedss.Sum(x => x.Amount);
                                        distribution.Debit = 0;
                                    }
                                    #endregion
                                    #region Additions
                                    var additions = from r in additionLists
                                                    join p in payrolldatas
                                                        on r.PayrollProcessViewModelId equals p.PayrollProcessViewModelId
                                                    where r.Name == accounts.Description
                                                    select (new { r.Amount });

                                    if (additions.Any())
                                    {

                                        distribution.Credit = 0;
                                        distribution.Debit += additions.Sum(x => x.Amount);
                                    }
                                    #endregion

                                }

                                #endregion

                                break;


                        }
                        distribution.GLPeriodId = id;
                        distribution.DateCreated = DateTime.Now.Date;
                        distribution.LastModified = null;
                        distribution.CreatedBy = User.Identity.Name;
                        distribution.ModifiedBy = null;
                        var checkExeem =
                            payrollDistributions.Where(
                                x =>
                                    x.AccountDesc == distribution.AccountDesc && x.Debit == distribution.Debit &&
                                    x.Credit == distribution.Credit && x.AccountCode == distribution.AccountCode)
                                .FirstOrDefault();

                        if ((distribution.Debit != 0 || distribution.Credit != 0) && checkExeem == null)
                        {
                            //db.PayrollDistributions.Add(distribution);
                            //db.SaveChanges();
                            payrollDistributions.Add(distribution);
                        }

                    }

                    //split cost
                    if (accountHelper.IsDynamicCoA())
                    {
                        foreach (var dist in payrollDistributions)
                        {
                            string costcenter = dist.AccountCode.Split('-')[dynamicAccount.GetCostCenterAccountSequenceIndex()];
                            var splitRules = db.PayrollCostAllocationSetting.Where(x => x.CostCenter == costcenter).ToList();
                            if (splitRules.Any())
                            {
                                foreach (var rule in splitRules)
                                {
                                    string newAccountCode =
                                        dynamicAccount.ReplaceSegment(dist.AccountCode, 3, rule.ProductAccount);
                                    var newDist = new PayrollDistribution
                                    {

                                        Credit = dist.Credit,
                                        Debit = dist.Debit * rule.Percentage / 100,
                                        AccountCode = newAccountCode,
                                        AccountDesc = dist.AccountDesc,
                                        BranchId = dist.BranchId,
                                        GLPeriodId = id,
                                        CostCenterCode = dist.CostCenterCode,
                                        Description = dist.Description,
                                        DateCreated = DateTime.Now,
                                        CreatedBy = User.Identity.Name
                                    };
                                    splittedDistributions.Add(newDist);

                                }
                            }
                            else
                            {
                                splittedDistributions.Add(dist);
                            }
                        }
                    }
                    else
                    {
                        splittedDistributions.AddRange(payrollDistributions);
                    }

                    db.PayrollDistributions.AddRange(splittedDistributions);
                    db.SaveChanges();

                    distributed = "true";
                    return RedirectToAction("Index");
                }
            }


            return RedirectToAction("Index");


        }
        public ActionResult PayrollDistributions_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<PayrollDistribution> PayrollDistributions = db.PayrollDistributions.Where(p => p.GLPeriodId == PayrollDistributionController.PeriodId && PayrollDistributionController.BranchId == p.BranchId).AsQueryable().ToList();
            var period = db.GLPeriods.Where(x => x.GLPeriodId == PayrollDistributionController.PeriodId).FirstOrDefault();


            if (period != null)
            {
                var glperiod = db.GLPeriods.Where(x => x.Name == period.Name).FirstOrDefault();
                if (glperiod != null)
                {
                    ViewBag.Period = glperiod.Name;
                }
                else
                {
                    ViewBag.Period = "";
                }

            }
            else
            {
                ViewBag.Period = "";
            }
            string debit = Convert.ToDecimal(PayrollDistributions.Where(x => x.GLPeriodId == PayrollDistributionController.PeriodId && PayrollDistributionController.BranchId == x.BranchId).Sum(x => x.Debit)).ToString("#,##0.00");
            string credit = Convert.ToDecimal(PayrollDistributions.Where(x => x.GLPeriodId == PayrollDistributionController.PeriodId && PayrollDistributionController.BranchId == x.BranchId).Sum(x => x.Credit)).ToString("#,##0.00");
            IQueryable<PayrollDistribution> payrolldistributions = db.PayrollDistributions;

            List<PayrollDistribution> distributions = new List<PayrollDistribution>();
            PayrollDistribution distribution = new PayrollDistribution();
            if (period != null) distribution.GLPeriodId = period.GLPeriodId;
            distribution.AccountCode = "";
            distribution.AccountDesc = "";
            distribution.Debit = Convert.ToDecimal(debit);
            distribution.Credit = Convert.ToDecimal(credit);
            distributions.Add(distribution);
            DataSourceResult result = PayrollDistributions.ToDataSourceResult(request, payrollDistribution => new
            {
                //PayrollDistributionID = payrollDistribution.PayrollDistributionID,
                //  CreatedOn = payrollDistribution.CreatedOn,
                // CreatedBy = payrollDistribution.CreatedBy,
                //  Period = payrollDistribution.Period,
                AccountCode = payrollDistribution.AccountCode,
                AccountDesc = payrollDistribution.AccountDesc,
                Description = accountFullName(payrollDistribution.AccountCode),
                Debit = payrollDistribution.Debit,
                Credit = payrollDistribution.Credit,
                OrgBranchName = payrollDistribution.OrgBranchName,
                CostCenterCode = payrollDistribution.CostCenterCode,
                GLPeriodId = payrollDistribution.GLPeriodId,
                BranchId = payrollDistribution.BranchId,
                PayrollDistributionId = payrollDistribution.PayrollDistributionId,
                isMannual = payrollDistribution.isMannual,
                // Status = payrollDistribution.Status
                //isSendForApproval = payrollDistribution.isSendForApproval,
                //isOnlineApproved = payrollDistribution.isOnlineApproved,
                //OnlineApprovedBy = payrollDistribution.OnlineApprovedBy,
                //OnlineApprovedTime = payrollDistribution.OnlineApprovedTime,
                //isVoid = payrollDistribution.isVoid,
                //VoidBy = payrollDistribution.VoidBy,
                //VoidTime = payrollDistribution.VoidTime
            });

            return Json(result);
        }
        private string accountFullName(string code)
        {
            AccountHelper accountHelper = new AccountHelper();
            string name = "";
            //string accountMain = code.Split('-')[0].Replace('\t', ' ').Trim();
            //string branch = code.Split('-')[1].Replace('\t', ' ').Trim();
            //string costCneter = code.Split('-')[2].Replace('\t', ' ').Trim();
            //string SubAccCode = code.Split('-')[3].Replace('\t', ' ').Trim();
            var account = accountHelper.GetAccountDesc(code);
            //if (account !=null)
            //{
            //    name=account.Description ;
            //}
            // var location = db.GLChartOfAccountLocations.Where(x => x.Values == branch).FirstOrDefault();
            //if (location !=null)
            //{
            //    name=name +"-"+ location.Description ;
            //}
            //  var cost = db.GLChartOfAccountCostCenters.Where(x => x.Values == costCneter).FirstOrDefault();
            //if (cost !=null)
            //{
            //    name=name +"-"+ cost.Description ;
            //}
            //  var sub = db.GLChartOfAccountSubAccounts.Where(x => x.Values == SubAccCode).FirstOrDefault();
            //if (sub !=null)
            //{
            //    name=name +"-"+ sub.Description ;
            //}
            return account;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayrollDistributions_Create([DataSourceRequest]DataSourceRequest request, PayrollDistribution payrollDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new PayrollDistribution
                {
                    AccountCode = payrollDistribution.AccountCode,
                    AccountDesc = payrollDistribution.AccountDesc,
                    Debit = payrollDistribution.Debit,
                    Credit = payrollDistribution.Credit,
                    OrgBranchName = payrollDistribution.OrgBranchName,
                    CostCenterCode = payrollDistribution.CostCenterCode,
                    GLPeriodId = Periodid,
                    BranchId = Branchid,
                    isMannual = true,
                };

                db.PayrollDistributions.Add(entity);
                db.SaveChanges();
                payrollDistribution.PayrollDistributionId = entity.PayrollDistributionId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll process", "Success", User.Identity.Name, $"added payroll distribution for period {entity.GLPeriodId} and branch {entity.GLPeriodId} account {entity.AccountCode} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { payrollDistribution }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PayrollDistributions_Update([DataSourceRequest]DataSourceRequest request, PayrollDistribution payrollDistribution)
        {
            if (ModelState.IsValid)
            {
                var entity = new PayrollDistribution
                {
                    AccountCode = payrollDistribution.AccountCode,
                    AccountDesc = payrollDistribution.AccountDesc,
                    Debit = payrollDistribution.Debit,
                    Credit = payrollDistribution.Credit,
                    OrgBranchName = payrollDistribution.OrgBranchName,
                    CostCenterCode = payrollDistribution.CostCenterCode,
                    PayrollDistributionId = payrollDistribution.PayrollDistributionId,
                    GLPeriodId = payrollDistribution.GLPeriodId,
                    BranchId = payrollDistribution.BranchId,
                    isMannual = payrollDistribution.isMannual,
                };

                db.PayrollDistributions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll process", "Success", User.Identity.Name, $"updated payroll distribution for period {entity.GLPeriodId} and branch {entity.GLPeriodId} account {entity.AccountCode} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { payrollDistribution }.ToDataSourceResult(request, ModelState));
        }
        public static int Branchid { get; set; }
        public static int Periodid { get; set; }
        public string CheckStatus(int id, int branchid)
        {
            string result = "";
            Branchid = branchid;
            var payrollPeriod = db.GLPeriods.Where(x => x.GLPeriodId == id).FirstOrDefault();
            if (payrollPeriod != null)
            {
                var glperiod = db.GLPeriods.Where(x => x.Name == payrollPeriod.Name).FirstOrDefault();
                if (glperiod != null)
                {
                    id = glperiod.GLPeriodId;
                }


            }

            Periodid = id;
            var periodstat = db.PeriodStatuses.Where(x => x.GLPeriodId == id && x.BranchId == branchid).FirstOrDefault();
            if (periodstat != null)
            {
                if (periodstat.Status == PeriodStatusList.CLOSED)
                {
                    result = "c";
                }
                else if (periodstat.Status == PeriodStatusList.TRANSFERRED)
                {
                    result = "t";
                }
                else if (periodstat.Status == PeriodStatusList.NEW)
                {
                    result = "n";
                }
                else
                {
                    result = "p";
                }
            }
            return result;
        }
        [Authorize(Roles = Payroll_Transfer)]
        public ActionResult TransferDistribution(int branchId, int id = 0)
        {
            var record = db.GLPeriods.Find(id);
            var defaultBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

            if (record != null)
            {
                var glperiod = db.GLPeriods.Where(x => x.Name == record.Name).FirstOrDefault();
                if (glperiod != null)
                {
                    id = glperiod.GLPeriodId;
                }


            }
            var PeriodStatus = db.PeriodStatuses.Where(x => x.GLPeriodId == id && x.BranchId == branchId).FirstOrDefault();
            string message = "";
            if (PeriodStatus.Status != PeriodStatusList.CLOSED)
            {
                message = "The record cannot be transffred before closing.";
                return new JavaScriptResult() { Script = "alert('" + message + "');" };
            }
            else
            {
                //if (!record.isOnlineTransferred)
                //{
                #region check if debit and credit are equal
                decimal TDebit = 0;
                decimal TCredit = 0;
                var distribution = db.PayrollDistributions.Where(x => x.BranchId == branchId)
                    .Where(x => x.GLPeriodId == id).ToList();
                //#region check if already transffered 
                //var exist = db.GLRecordJournals
                //   .Where(x => x.Source == JournalSources.Payroll.ToString())
                //   .Where(x => x.SourceId == record.Name)
                //   .FirstOrDefault();
                //if (exist != null)
                //{
                //    var formattedData = new
                //    {
                //        Message = "The record is already transffered, it can't be retransferred.",
                //        Status = "NOT_OK"
                //    };
                //    return Json(formattedData, JsonRequestBehavior.AllowGet);
                //}
                //#endregion
                #region check segment length validation
                AccountHelper accountHelper = new AccountHelper();
                foreach (var dist in distribution)
                {
                    string errmessage = accountHelper.ValidateSegmentCombinationLength(dist.AccountCode);
                    if (!String.IsNullOrEmpty(errmessage))
                    {
                        message = errmessage;
                        return new JavaScriptResult() { Script = "alert('" + message + "');" };
                    }

                }


                #endregion
                if (distribution != null)
                {
                    TDebit = distribution.Where(x => x.GLPeriodId == id).Sum(x => x.Debit);
                    TCredit = distribution.Where(x => x.GLPeriodId == id).Sum(x => x.Credit);
                }
                if (Math.Round(TDebit, 2) != Math.Round(TCredit, 2))
                {
                    message = "Total debit and total credit is not balanced.";
                    return new JavaScriptResult() { Script = "alert('" + message + "');" };
                }
                if (TDebit == 0)
                {
                    message = "Total debit and total credit is zero.";
                    return new JavaScriptResult() { Script = "alert('" + message + "');" };
                }
                #endregion
                #region send to GL
                string glperiod = "";
                string glyear = "";
                #region get period
                var period = db.GLPeriods.Where(x => x.DateFrom <= record.DateFrom)
                                       .Where(x => x.DateTo >= record.DateTo).ToList();
                if (period.Any())
                {

                    if (period.Count() == 1)
                    {
                        var myPeriod = period.FirstOrDefault();
                        if (myPeriod != null)
                        {
                            var glperiodd = db.GLPeriods.Where(x => x.Name == record.Name).FirstOrDefault();
                            if (glperiodd != null)
                            {
                                id = glperiodd.GLPeriodId;
                                glperiod = myPeriod.Name;
                                glyear = myPeriod.GlFiscalYear.Name.ToString();
                            }


                        }


                        if (myPeriod.Status == FiscalYearStatus.Close)
                        {
                            message = "The period " + glperiod + " is closed,the record cannot be transffred to closed period.";
                            return new JavaScriptResult() { Script = "alert('" + message + "');" };
                        }
                    }
                    else if (period.Count() == 0)
                    {
                        message = "There are no periods defined that contain the Payroll period " + record.Name;
                        return new JavaScriptResult() { Script = "alert('" + message + "');" };
                    }
                    else if (period.Count() >= 1)
                    {
                        message = "There are multiple periods defined that contain the Payroll period " + record.Name;
                        return new JavaScriptResult() { Script = "alert('" + message + "');" };
                    }
                    else
                    {

                    }
                }
                else
                {
                    message = "The are no periods defined.";
                    return new JavaScriptResult() { Script = "alert('" + message + "');" };
                }
                #endregion
                if (distribution.Any())
                {
                    GlRecordJournal journal = new GlRecordJournal
                    {
                        // journal.CreatedOn =DateTime.Now.Date;
                        CreatedBy = User.Identity.Name,
                        JournalName = "Payroll",
                        Description = "Payroll Process",
                        FiscalYear = glyear,
                        PeriodName = glperiod,
                        CategoryName = "Payroll- Process",
                        SourcesName = "Payroll",
                        JournalNumber = "Payroll." + record.Name.ToString(),
                        EffectiveDate = record.DateTo.Date,
                        SourceId = record.Name,
                        Source = JournalSources.Payroll.ToString(),
                        Status = "Approved",
                        OrgBranchName = branchId.ToString(),
                        IsOnlineApproved = true,
                        OnlineApprovedBy = User.Identity.Name,
                        OnlineApprovedTime = DateTime.Now.Date
                    };
                    db.GLRecordJournals.Add(journal);
                    db.SaveChanges();
                    GLRecordJournalDetail journalDetail;
                    accountHelper = new AccountHelper();
                    foreach (var i in distribution)
                    {
                        #region debit and credit
                        journalDetail = new GLRecordJournalDetail
                        {
                            DateCreated = DateTime.Now.Date,
                            OrgBranchName = defaultBranch,
                            CreatedBy = User.Identity.Name,
                            GLRecordJournal = journal,
                            AccountCode = i.AccountCode,
                            AccountDesc = accountHelper.GetAccountDesc(i.AccountCode),
                            Debit = Math.Round(i.Debit, 4),
                            Credit = Math.Round(i.Credit, 4)
                        };
                        db.GLRecordJournalDetails.Add(journalDetail);
                        db.SaveChanges();
                        #endregion
                    }

                    #region update period status

                    if (PeriodStatus == null)
                    {
                        return HttpNotFound();
                    }

                    PeriodStatus.Status = PeriodStatusList.TRANSFERRED;
                    db.Entry(PeriodStatus).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion

                    #region split journal
                    var journaldetails = db.GLRecordJournalDetails
                        .Where(x => x.GlRecordJournalId == journal.GlRecordJournalId).ToList();
                    foreach (var detail in journaldetails)
                    {
                        AccountHelper helper = new AccountHelper();
                        helper.SplitJournal(detail.GlRecordJournalDetailId);
                    }
                    #endregion
                }
                #endregion
                //record.isOnlineTransferred = true;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();

                // }
                //else
                //{
                //    message = "You cannot transfer this transaction, it is already transffered.";
                //    return new JavaScriptResult() { Script = "alert('" + message + "');" };
                //}
            }
            //  RedirectToAction("Index");
            message = "Successfully Transfred!!";
            return new JavaScriptResult() { Script = "alert('" + message + "');" };
        }

        public ActionResult SendMail(int branchId, int id)
        {
            var company = from c in db.Companies
                          join cm in db.CompanyEmails
                              on c.MainCompanyId equals cm.MainCompanyId
                          where c.Category == "0"
                          select (new { cm.EmailAddress, cm.Password, cm.ServerType });
            var period = db.GLPeriods.Where(x => x.GLPeriodId == id).FirstOrDefault();

            if (!company.Any())
            {
                return new JavaScriptResult() { Script = "alert('" + "please set email address and password for the company!! " + "');" };
            }
            var processes =
                db.PayrollProcessViewModel.Where(x => x.BranchId == branchId && x.GLPeriodId == id).ToList();
            var address = db.Addresses.Where(x => x.Category == AddressType.OFFICE_EMAIL).ToList();
            foreach (var p in processes)
            {
                var companyMail = company.FirstOrDefault();
                var Officeemail =
                    address.Where(x => x.Reference == p.EmployeeId.ToString() && x.ReferenceType == "Employee")
                        .FirstOrDefault();
                if (Officeemail != null)
                {
                    MailMessage mail = new MailMessage();
                    SmtpClient SmtpServer = new SmtpClient(companyMail.ServerType);
                    mail.From = new MailAddress(companyMail.EmailAddress);
                    mail.To.Add(Officeemail.Value);
                    mail.Subject = "Please Find the attachment";
                    mail.Body = period.Name + " Payroll Slip";
                    string password = Encrypt.DecryptString(companyMail.Password, companyMail.EmailAddress);

                    EmployeePaySlipByEmployee report = new EmployeePaySlipByEmployee();
                    Telerik.Reporting.Processing.ReportProcessor reportProcessor = new Telerik.Reporting.Processing.ReportProcessor();

                    // set any deviceInfo settings if necessary
                    System.Collections.Hashtable deviceInfo = new System.Collections.Hashtable();
                    deviceInfo["FontEmbedding"] = "Subset";
                    deviceInfo["ViewerDirection"] = "L2R";
                    deviceInfo["DocumentTitle"] = period.Name + " Payroll Slip";
                    Telerik.Reporting.InstanceReportSource instanceReportSource = new Telerik.Reporting.InstanceReportSource();

                    instanceReportSource.ReportDocument = report;

                    instanceReportSource.Parameters.Add(new Telerik.Reporting.Parameter("period", period.Name));
                    instanceReportSource.Parameters.Add(new Telerik.Reporting.Parameter("employee", p.Employee.EmployeeCode.ToString()));

                    Telerik.Reporting.Processing.RenderingResult result = reportProcessor.RenderReport("PDF", instanceReportSource, deviceInfo);
                    //ReportProcessor reportProcessor = new ReportProcessor();
                    //RenderingResult result = reportProcessor.RenderReport("PDF", report, null);
                    MemoryStream ms = new MemoryStream(result.DocumentBytes);
                    ms.Position = 0;
                    System.Net.Mail.Attachment attachment = new System.Net.Mail.Attachment(ms, period.Name + " Payroll Slip" + ".pdf");
                    mail.Attachments.Add(attachment);
                    SmtpServer.Port = 587;
                    SmtpServer.Credentials = new System.Net.NetworkCredential(companyMail.EmailAddress, password);
                    SmtpServer.EnableSsl = true;

                    SmtpServer.Send(mail);
                }

            }
            return new JavaScriptResult() { Script = "alert('" + "Successfully Send" + "');" };

        }

    }

}