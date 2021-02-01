﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
﻿using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Finance.GL;
﻿using ExceedERP.Core.Domain.HRM;
 using ExceedERP.Core.Domain.HRM.Allowance;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.Payroll;
﻿using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Helpers.Common;
﻿using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
    public class BackPaymentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        HRMInputs inputs = new HRMInputs();
        static decimal EmployeeContribution { get; set; }
        private static decimal CompanyContribution { get; set; }
        decimal totalTaxableAllowance = 0; 
        decimal totalNonTaxableAllowance = 0;
        decimal pensionFromEmployee = 0;
        decimal pensionFromCompany = 0;
        decimal totalPension = 0;
        public ActionResult Index()
        {
            GetPeriod();
            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            return View();
        }
        public JsonResult GetPeriod()
        {
            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open);
            var listt = from p in periodList
                        join g in db.GLPeriods
                            on p.Name equals g.Name
                        select (new
                        {
                            g
                        });
            ViewData["PeriodList"] = listt.Select(x => x.g).ToList(); ;

            return Json(listt.Select(p => new { GLPeriodId = p.g.GLPeriodId, Name = p.g.Name }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult BackPayments_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<BackPayment> backpayments = db.BackPayments.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = backpayments.ToDataSourceResult(request, backPayment => new {
                BackPaymentId = backPayment.BackPaymentId,
                GLPeriodId = backPayment.GLPeriodId,
                EmployeeId = backPayment.EmployeeId,
                BasicSalary = backPayment.BasicSalary,
                IncomeTax = backPayment.IncomeTax,
                Pension = backPayment.Pension,
                Allowances = backPayment.Allowances,
                Net = backPayment.Net,
                IsClosed = backPayment.IsClosed,
                Remark = backPayment.Remark,
                DateCreated = backPayment.DateCreated,
                LastModified = backPayment.LastModified,
                CreatedBy = backPayment.CreatedBy,
                OrgBranchName = backPayment.OrgBranchName,
                ModifiedBy = backPayment.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPayments_Create([DataSourceRequest]DataSourceRequest request, BackPayment backPayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new BackPayment
                {
                    BasicSalary = backPayment.BasicSalary,
                    GLPeriodId = backPayment.GLPeriodId,
                    EmployeeId = backPayment.EmployeeId,
                    IncomeTax = backPayment.IncomeTax,
                    Pension = backPayment.Pension,
                    Allowances = backPayment.Allowances,
                    Net = backPayment.Net,
                    IsClosed = backPayment.IsClosed,
                    Remark = backPayment.Remark,
                    DateCreated = DateTime.Now,
                    LastModified = backPayment.LastModified,
                    CreatedBy = User.Identity.Name,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    ModifiedBy = backPayment.ModifiedBy
                };

                db.BackPayments.Add(entity);
                db.SaveChanges();
                backPayment.BackPaymentId = entity.BackPaymentId;
            }

            return Json(new[] { backPayment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPayments_Update([DataSourceRequest]DataSourceRequest request, BackPayment backPayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new BackPayment
                {
                    BackPaymentId = backPayment.BackPaymentId,
                    GLPeriodId = backPayment.GLPeriodId,
                    EmployeeId = backPayment.EmployeeId,
                    BasicSalary = backPayment.BasicSalary,
                    IncomeTax = backPayment.IncomeTax,
                    Pension = backPayment.Pension,
                    Allowances = backPayment.Allowances,
                    Net = backPayment.Net,
                    IsClosed = backPayment.IsClosed,
                    Remark = backPayment.Remark,
                    DateCreated = backPayment.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = backPayment.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = backPayment.OrgBranchName,
                };

                db.BackPayments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { backPayment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BackPayments_Destroy([DataSourceRequest]DataSourceRequest request, BackPayment backPayment)
        {
            if (ModelState.IsValid)
            {
                var entity = new BackPayment
                {
                    BackPaymentId = backPayment.BackPaymentId,
                    GLPeriodId = backPayment.GLPeriodId,
                    EmployeeId = backPayment.EmployeeId,
                    BasicSalary = backPayment.BasicSalary,
                    IncomeTax = backPayment.IncomeTax,
                    Pension = backPayment.Pension,
                    Allowances = backPayment.Allowances,
                    Net = backPayment.Net,
                    IsClosed = backPayment.IsClosed,
                    Remark = backPayment.Remark,
                    DateCreated = backPayment.DateCreated,
                    LastModified = backPayment.LastModified,
                    CreatedBy = backPayment.CreatedBy,
                    OrgBranchName = backPayment.OrgBranchName,
                    ModifiedBy = backPayment.ModifiedBy
                };
                #region delete log
               // UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee loan information of employee " + loan.EmployeeId + " and period " + loan.GLPeriodId);
                #endregion
                db.BackPayments.Attach(entity);
                db.BackPayments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { backPayment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult PrepareBackPayment(int period1, int period2)
        {
            GetPensionSetting();
            var periodOne = db.PayrollPeriods .Where(x => x.GLPeriodId == period1).FirstOrDefault();
            var periodTwo = db.PayrollPeriods .Where(x => x.GLPeriodId == period2).FirstOrDefault();
            var placments = db.Placements.Where(x => x.isCurrent).ToList();
            decimal salary = 0;
            if (periodOne != null && periodTwo !=null)
            {
                var periods =
                    db.PayrollPeriods.Where(x => x.DateFrom >= periodOne.DateFrom && x.DateTo <= periodTwo.DateTo && x.Status ==FiscalYearStatus.Open).ToList();
                if (periods.Any())
                {
                    foreach (var p in periods)
                    {
                        var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
                        foreach (var l in list)
                        {

                            var entity = new BackPayment();
                            salary = 0;
                            entity.EmployeeId = l.PersonId;
                            entity.GLPeriodId = p.GLPeriodId ;
                            var empPlacemnt = placments.Where(x => x.EmployeeId == l.PersonId).FirstOrDefault();
                            if (empPlacemnt != null)
                            {
                                salary = empPlacemnt.Salary;
                            }
                            entity.BasicSalary = salary;
                            //allowances
                            entity.Allowances = GetAllowacne(l.PersonId, salary, p);
                            entity.IncomeTax = Convert.ToDecimal( XAPI.IncomeTax.GetIncomeTax((float)(salary + totalTaxableAllowance)));
                            entity.Pension = GetPension(salary);
                            entity.Remark = null;
                            entity.Net = ( entity.BasicSalary + entity.Allowances) - entity.IncomeTax - pensionFromEmployee;
                            entity.DateCreated = DateTime.Now;
                            entity.LastModified = null;
                            entity.CreatedBy = User.Identity.Name;
                            entity.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                            entity.ModifiedBy = null;
                            db = new ExceedDbContext();
                            db.BackPayments.Add(entity);
                            db.SaveChanges();
                           
                        }
                    }
                }
            }
            return null;
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
                   


                    return true;
                }

            }
            catch (Exception)
            {

                return false;
            }
            return false;
        }
        private decimal GetPension(decimal salary)
        {
             pensionFromEmployee = 0;
             pensionFromCompany = 0;
             totalPension = 0;
             
            pensionFromEmployee =  salary * EmployeeContribution / 100;
            pensionFromCompany = salary * CompanyContribution / 100;
            totalPension = pensionFromCompany + pensionFromEmployee;
            return totalPension;
        }
        private decimal GetAllowacne(int empId, decimal salary, PayrollPeriod findPeriod)
        {

            #region allowances

            var allowances = db.AllowancePerEmployees.Where(x => x.EmployeeId == empId && x.Pend == false).ToList();
            decimal totalAllowance = 0;
            totalTaxableAllowance = 0;
            decimal nonTaxableAllowance = 0;
            decimal taxableAllowance = 0;
            decimal TotalAllowance;
            TotalAllowance = 0;
           
            totalNonTaxableAllowance = 0;
            AllowanceDefination allowancemodel;
            PayrollProcessAllowances allowancedata;
            decimal Threshold = 0; var allowanceTypeRule =
                        db.AllowanceTypeRule.ToList();
            foreach (AllowancePerEmployee allowance in allowances)
            {
                var absentdays = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == empId)
                             .Where(x => x.GLPeriodId == findPeriod.GLPeriodId).ToList();
               
                allowancemodel = new AllowanceDefination();
                allowancemodel =
                    db.AllowanceDefinations.Where(
                        x => x.AllowanceDefinationId == allowance.AllowanceDefinationId)
                        .FirstOrDefault();

                allowancedata = new PayrollProcessAllowances();
                allowancedata.CreatedOn = DateTime.Now.Date;
                allowancedata.CreatedBy = User.Identity.Name;

                if (allowancemodel != null)
                {
                    allowancedata.Name = allowancemodel.AllowanceName;

                    allowancedata.AllowanceType = allowancemodel.AllowanceType;


                    var rule =
                        allowanceTypeRule.Where(a => a.AllowanceType == allowancemodel.AllowanceType)
                            .FirstOrDefault();

                    if (rule == null)
                    {
                        //  message = "Please Set Allowance Rule For " + allowancemodel.AllowanceType.ToString();
                        // RedirectToAction("Index");
                        return 0;
                    }
                    if (allowancemodel.AmtPercent == FixedContPecntType.PERCENT)
                    {
                        allowancemodel.Amount = salary * (allowancemodel.Amount / 100);
                    }

                    if (allowancemodel.AllowanceType == AllowanceType.PROFESSIONAL)
                    {
                        decimal totalSickleave = 0;
                        var absentdayss = db.EmpPayrollNoDaysWorkeds.Where(x => x.EmployeeId == empId )
                            .Where(x => x.GLPeriodId == findPeriod.GLPeriodId).ToList();
                        if (absentdays.Count() != 0)
                        {
                            totalSickleave += absentdayss.Select(x => x.Days).FirstOrDefault();
                        }

                        var checkFiscalYear =
                            db.PayrollPeriods.Where(x => x.GLPeriodId == findPeriod.GLPeriodId).FirstOrDefault();
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
                        allowancemodel.Amount = allowancemodel.Amount - (allowancemodel.Amount/30)*totalSickleave;

                    }
                    if (allowancemodel.AllowanceType == AllowanceType.POSITION)
                    {

                        decimal totalDelagatin = 0;

                        var checkDelegation =
                                     db.Delegations.Where(
                                             x =>
                                                 x.FromEmployeeId == allowance.EmployeeId &&
                                                 x.FromDateTime >= findPeriod.DateFrom  &&
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
                                    double dateDiff = (checkDelegation.FromDateTime - delegationEndDate).TotalDays;
                                    if (dateDiff >= 30)
                                    {
                                        totalDelagatin = Convert.ToDecimal(dateDiff);
                                    }
                                }
                                else if (delegationEndDate > findPeriod.DateTo &&
                                         checkDelegation.FromDateTime > findPeriod.DateFrom)
                                {
                                    double dateDiff = (findPeriod.DateTo - checkDelegation.FromDateTime).TotalDays;
                                    if (dateDiff >= 30)
                                    {
                                        totalDelagatin = Convert.ToDecimal(dateDiff);
                                    }
                                }
                                else if (findPeriod.DateTo < checkDelegation.FromDateTime)
                                {
                                    double dateDiff = (findPeriod.DateTo - checkDelegation.FromDateTime).TotalDays;
                                    if (dateDiff >= 30)
                                    {
                                        totalDelagatin = Convert.ToDecimal(dateDiff);
                                    }
                                }
                            }
                        }
                        allowancemodel.Amount = allowancemodel.Amount - (allowancemodel.Amount/30)*totalDelagatin;

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
                                    double dateDiff = (checkDelegation.FromDateTime - delegationEndDate).TotalDays;
                                    if (dateDiff >= 30)
                                    {
                                        totalDelagatin = Convert.ToDecimal(dateDiff);
                                    }
                                }
                                else if (delegationEndDate > findPeriod.DateTo &&
                                         checkDelegation.FromDateTime > findPeriod.DateFrom)
                                {
                                    double dateDiff = (findPeriod.DateTo - checkDelegation.FromDateTime).TotalDays;
                                    if (dateDiff >= 30)
                                    {
                                        totalDelagatin = Convert.ToDecimal(dateDiff);
                                    }
                                }
                                else if (findPeriod.DateTo < checkDelegation.FromDateTime)
                                {
                                    double dateDiff = (findPeriod.DateTo - checkDelegation.FromDateTime).TotalDays;
                                    if (dateDiff >= 30)
                                    {
                                        totalDelagatin = Convert.ToDecimal(dateDiff);
                                    }
                                }
                            }
                            allowancemodel.Amount = allowancemodel.Amount + (allowancemodel.Amount/30)*totalDelagatin;

                        }
                    }
                    Threshold = ((decimal) (rule.Percentage)*salary);

                    #region maximum non taxable rule

                    if (rule.TaxTrasholdAmount == 0 && rule.Percentage == 0)
                    {
                        allowancedata.NonTaxable = Convert.ToDecimal(allowancemodel.Amount);
                        allowancedata.Taxable = 0;
                        allowancedata.Total = Convert.ToDecimal(allowancemodel.Amount);
                        totalTaxableAllowance = totalTaxableAllowance + allowancedata.Taxable;
                        totalNonTaxableAllowance = totalNonTaxableAllowance + allowancedata.NonTaxable;
                        totalAllowance = totalAllowance + allowancedata.Total;

                        nonTaxableAllowance += Convert.ToDecimal(allowancemodel.Amount);
                        taxableAllowance = 0;
                        TotalAllowance += totalTaxableAllowance +
                                                totalNonTaxableAllowance;
                    }
                    else
                    {
                        if (allowancemodel.Amount > Threshold)
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

                                nonTaxableAllowance = Convert.ToDecimal(rule.TaxTrasholdAmount);
                                taxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                                   Convert.ToDecimal(rule.TaxTrasholdAmount);
                                TotalAllowance += totalTaxableAllowance +
                                                totalNonTaxableAllowance;
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

                                nonTaxableAllowance = Threshold;
                                taxableAllowance = Convert.ToDecimal(allowancemodel.Amount) -
                                                   Threshold;
                                TotalAllowance += totalTaxableAllowance +
                                                 totalNonTaxableAllowance;
                            }
                        }

                    }
                }

                #endregion


                



                //    #region Allowance Additions

                //    db = new ExceedDbContext();
                //    additiondata = new PayrollProcessAdditions();
                //    additiondata.CreatedOn = DateTime.Now.Date;
                //    additiondata.CreatedBy = User.Identity.Name;
                //    //additiondata.EmployeeId = employee.PersonId;
                //    additiondata.Name = allowancedata.Name;
                //    //additiondata.Name = period;
                //    additiondata.Amount = allowancedata.Taxable + allowancedata.NonTaxable;


                }
                taxableAllowance = totalTaxableAllowance;
                nonTaxableAllowance = totalNonTaxableAllowance;
              //  totalAllowance = TotalAllowance;

                //    #endregion

                #endregion

                return totalAllowance;
            }


        public string CheckStatus(int id)
        {
            var list = db.BackPayments.Where(x => x.GLPeriodId == id).FirstOrDefault();
            if (list != null)
            {
                if (list.IsClosed)
                {
                    return "c";
                }
                else
                {
                    return "n";
                }
            }
            else
            {
                return "r";
            }
        }
        public ActionResult Close(int period1, int period2)
        { var periodOne = db.GLPeriods.Where(x => x.GLPeriodId == period1).FirstOrDefault();
            var periodTwo = db.GLPeriods.Where(x => x.GLPeriodId == period2).FirstOrDefault();
            if (periodOne != null && periodTwo != null)
            {
                var periods =
                    db.GLPeriods.Where(x => x.DateFrom <= periodOne.DateFrom && x.DateTo >= periodTwo.DateTo).ToList();
                if (periods.Any())
                {
                    foreach (var p in periods)
                    {
                        var list = db.BackPayments.Where(x => x.GLPeriodId == p.GLPeriodId ).ToList();
                        foreach (var l in list)
                        {
                            var entity = new BackPayment
                            {
                                BackPaymentId = l.BackPaymentId,
                                GLPeriodId = p.GLPeriodId,
                                EmployeeId = l.EmployeeId,
                                BasicSalary = l.BasicSalary,
                                IncomeTax = l.IncomeTax,
                                Pension = l.Pension,
                                Net = l.Net,
                                IsClosed = true,
                                Remark = l.Remark
                            };

                            db.BackPayments.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();

                        }
                    }
                }
            }
            return null;
        }
        public ActionResult Reverse(int period1, int period2)
        {
            var periodOne = db.GLPeriods.Where(x => x.GLPeriodId == period1).FirstOrDefault();
            var periodTwo = db.GLPeriods.Where(x => x.GLPeriodId == period2).FirstOrDefault();
            if (periodOne != null && periodTwo != null)
            {
                var periods =
                    db.GLPeriods.Where(x => x.DateFrom <= periodOne.DateFrom && x.DateTo >= periodTwo.DateTo).ToList();
                if (periods.Any())
                {
                    foreach (var p in periods)
                    {
                        var list = db.BackPayments.Where(x => x.GLPeriodId == p.GLPeriodId).ToList();
                        db.BackPayments.RemoveRange(list);
                        db.SaveChanges();
                    }
                }
            }
            return null;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
