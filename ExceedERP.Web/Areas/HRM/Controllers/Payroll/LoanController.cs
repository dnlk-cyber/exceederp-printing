using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = Payroll_Input + "," + Payroll_Admin)]
    public class LoanController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
        List<string> mybraches = new List<string>();
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open || x.Status == FiscalYearStatus.Close);
            var lists = from p in periodList
                        join g in db.GLPeriods
                            on p.Name equals g.Name
                        select (new
                        {
                            g
                        });
            ViewData["PeriodList"] = lists.Select(x => x.g).ToList(); ;
           var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive);
            mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            return View();
        }
        public ActionResult GetEmployees([DataSourceRequest] DataSourceRequest request)
        {
            db = new ExceedDbContext();
            var employees = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);

            return Json(employees.ToDataSourceResult(request));
        }


        public ActionResult Employee_ValueMapper(string value)
        {
            var dataItemIndex = -1;
            var data = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            if (value != "")
            {
                var index = 0;
                foreach (var vm in data)
                {
                    if (vm.EmployeeCode == value)
                    {
                        dataItemIndex = index;
                        break;
                    }

                    index += 1;
                }
            }
            return Json(dataItemIndex, JsonRequestBehavior.AllowGet);
        }
      
        public ActionResult Loans_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<Loan> loans = db.Loans.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = loans.ToDataSourceResult(request, loan => new {
                LoanId = loan.LoanId,
                EmployeeId = loan.EmployeeId,
                LoanAmount = loan.LoanAmount,
                Months = loan.Months,
                Balance = loan.Balance,
                MonthlyAmount = loan.MonthlyAmount,
                JumpHoliday = loan.JumpHoliday,
                Description = loan.Description,
                GLPeriodId = loan.GLPeriodId,
                Remark = loan.Remark,
                DateCreated = loan.DateCreated,
                LastModified = loan.LastModified,
                CreatedBy = loan.CreatedBy,
                OrgBranchName= loan .OrgBranchName,
                ModifiedBy = loan.ModifiedBy,
                IsActive = loan.IsActive
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Loans_Create([DataSourceRequest]DataSourceRequest request, Loan loan)
        {
            if (loan.Months != 0)
            {
                loan.MonthlyAmount = loan.LoanAmount / loan.Months;
                loan.Balance = loan.LoanAmount;
            }
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (loan.Months == 0)
                {
                    ModelState.AddModelError("", "Months Value should be greater than 0!!");
                }
                else
                {
                    loan.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    var checkexistancep =
                        db.PayrollProcessViewModel.Where(
                            x => x.GLPeriodId == loan.GLPeriodId && loan.OrgBranchName == x.OrgBranchName.ToString())
                            .FirstOrDefault();
                    if (checkexistancep != null)
                    {


                        ModelState.AddModelError("", "You cannot add this data. This period is already processed!!");
                    }
                    else
                    {

                        var checkexistance =
                            db.Loans.Where(x => x.EmployeeId == loan.EmployeeId && x.GLPeriodId == loan.GLPeriodId)
                                .FirstOrDefault();
                        if (checkexistance != null)
                        {
                            ModelState.AddModelError("", "This Loan on this period is already registered!!");
                        }
                        else if (loan.Months == 0)
                        {
                            ModelState.AddModelError("", "Months Value should be greater than 0!!");
                        }
                        else
                        {
                            var entity = new Loan
                            {
                                EmployeeId = loan.EmployeeId,
                                LoanAmount = loan.LoanAmount,
                                Months = loan.Months,
                                JumpHoliday = loan.JumpHoliday,
                                Balance = loan.Balance,
                                MonthlyAmount = loan.MonthlyAmount,
                                Description = loan.Description,
                                GLPeriodId = loan.GLPeriodId,
                                Remark = loan.Remark,
                                DateCreated = DateTime.Now,
                                LastModified = loan.LastModified,
                                CreatedBy = User.Identity.Name,
                                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                                ModifiedBy = loan.ModifiedBy,
                                IsActive = loan.IsActive
                            };

                            db.Loans.Add(entity);
                            db.SaveChanges();
                            loan.LoanId = entity.LoanId;
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll loan", "Success", User.Identity.Name, $"added loan  {entity.Description} for employee {entity.Employee.EmployeeCode} amount {entity.LoanAmount} monthly amount {entity.MonthlyAmount} months {entity.Months} balance {entity.Balance} period {entity.GLPeriodId} active {entity.IsActive}", Modules.Payroll);
                            #endregion
                        }
                    }
                }
            }
            return Json(new[] { loan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Loans_Update([DataSourceRequest]DataSourceRequest request, Loan loan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (loan.Months == 0)
                {
                    ModelState.AddModelError("", "Months Value should be greater than 0!!");
                }
                else
                {
                    
                        var checkexistance =
                            db.PayrollProcessViewModel.Where(
                                x => x.EmployeeId == loan.EmployeeId && x.GLPeriodId == loan.GLPeriodId)
                                .FirstOrDefault();
                    if (checkexistance != null)
                    {
                       

                        var checkLoanS = db.Loans.Where(x => x.LoanId == loan.LoanId).FirstOrDefault();
                        if (checkLoanS != null)
                        {
                            if (checkLoanS.IsActive != loan.IsActive)
                            {
                                var entity = new Loan
                                {
                                    LoanId = checkLoanS.LoanId,
                                    EmployeeId = checkLoanS.EmployeeId,
                                    LoanAmount = checkLoanS.LoanAmount,
                                    Months = checkLoanS.Months,
                                    JumpHoliday = checkLoanS.JumpHoliday,
                                    Description = checkLoanS.Description,
                                    Balance = checkLoanS.Balance,
                                    MonthlyAmount = checkLoanS.MonthlyAmount,
                                    GLPeriodId = checkLoanS.GLPeriodId,
                                    Remark = checkLoanS.Remark,
                                    DateCreated = checkLoanS.DateCreated,
                                    LastModified = DateTime.Now,
                                    CreatedBy = checkLoanS.CreatedBy,
                                    ModifiedBy = User.Identity.Name,
                                    OrgBranchName = checkLoanS.OrgBranchName,
                                    IsActive = loan.IsActive
                                };
                                if (checkLoanS.Months != 0)
                                {
                                    entity.MonthlyAmount = checkLoanS.LoanAmount/checkLoanS.Months;
                                    entity.Balance = checkLoanS.LoanAmount;
                                }
                                db = new ExceedDbContext();
                                db.Loans.Attach(entity);
                                db.Entry(entity).State = EntityState.Modified;
                                db.SaveChanges();
                                #region operation log
                                UserHelper.OperationLog(Request.UserHostAddress, "payroll loan", "Success", User.Identity.Name, $"updated loan  {entity.Description} for employee {entity.Employee.EmployeeCode} amount {entity.LoanAmount} monthly amount {entity.MonthlyAmount} months {entity.Months} balance {entity.Balance} period {entity.GLPeriodId} active {entity.IsActive}", Modules.Payroll);
                                #endregion
                            }
                            else
                            {
                                ModelState.AddModelError("",
                                    "You can update only is active value, it already processed!!");
                            }

                        }



                        else
                        {
                            var entity = new Loan
                            {
                                LoanId = loan.LoanId,
                                EmployeeId = loan.EmployeeId,
                                LoanAmount = loan.LoanAmount,
                                Months = loan.Months,
                                JumpHoliday = loan.JumpHoliday,
                                Description = loan.Description,
                                Balance = loan.Balance,
                                MonthlyAmount = loan.MonthlyAmount,
                                GLPeriodId = loan.GLPeriodId,
                                Remark = loan.Remark,
                                DateCreated = loan.DateCreated,
                                LastModified = DateTime.Now,
                                CreatedBy = loan.CreatedBy,
                                ModifiedBy = User.Identity.Name,
                                OrgBranchName = loan.OrgBranchName,
                                IsActive = loan.IsActive
                            };
                            db = new ExceedDbContext();
                            db.Loans.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll loan", "Success", User.Identity.Name, $"updated loan  {entity.Description} for employee {entity.Employee.EmployeeCode} amount {entity.LoanAmount} monthly amount {entity.MonthlyAmount} months {entity.Months} balance {entity.Balance} period {entity.GLPeriodId} active {entity.IsActive}", Modules.Payroll);
                            #endregion
                        }
                    }
                    else

                    {
                        
                            var entity = new Loan
                            {
                                LoanId = loan.LoanId,
                                EmployeeId = loan.EmployeeId,
                                LoanAmount = loan.LoanAmount,
                                Months = loan.Months,
                                JumpHoliday = loan.JumpHoliday,
                                Description = loan.Description,
                                Balance = loan.Balance,
                                MonthlyAmount = loan.MonthlyAmount,
                                GLPeriodId = loan.GLPeriodId,
                                Remark = loan.Remark,
                                DateCreated = loan.DateCreated,
                                LastModified = DateTime.Now,
                                CreatedBy = loan.CreatedBy,
                                ModifiedBy = User.Identity.Name,
                                OrgBranchName = loan.OrgBranchName,
                                IsActive = loan.IsActive
                            };
                            if (loan.Months != 0)
                            {
                                entity.MonthlyAmount = loan.LoanAmount / loan.Months;
                                entity.Balance = loan.LoanAmount;
                            }
                            db = new ExceedDbContext();
                            db.Loans.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "payroll loan", "Success", User.Identity.Name, $"updated loan  {entity.Description} for employee {entity.Employee.EmployeeCode} amount {entity.LoanAmount} monthly amount {entity.MonthlyAmount} months {entity.Months} balance {entity.Balance} period {entity.GLPeriodId} active {entity.IsActive}", Modules.Payroll);
                        #endregion

                    }
                }
            }
            return Json(new[] { loan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Loans_Destroy([DataSourceRequest]DataSourceRequest request, Loan loan)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == loan.EmployeeId && x.GLPeriodId == loan.GLPeriodId).FirstOrDefault();
                if (checkexistance != null)
                {
                   
                        ModelState.AddModelError("", "You cannot  delete this information, it already processed!!");
                    }
                    else
                    {
                        var entity = new Loan
                        {
                            LoanId = loan.LoanId,
                            EmployeeId = loan.EmployeeId,
                            LoanAmount = loan.LoanAmount,
                            Months = loan.Months,
                            JumpHoliday = loan.JumpHoliday,
                            Description = loan.Description,
                            Balance = loan.Balance,
                            MonthlyAmount = loan.MonthlyAmount,
                            GLPeriodId = loan.GLPeriodId,
                            Remark = loan.Remark,
                            DateCreated = loan.DateCreated,
                            LastModified = loan.LastModified,
                            CreatedBy = loan.CreatedBy,
                            ModifiedBy = loan.ModifiedBy,
                            OrgBranchName = loan.OrgBranchName,
                            IsActive = loan.IsActive
                        };
                        #region delete log
                        UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee loan information of employee " + loan.EmployeeId + " and period " + loan.GLPeriodId,Modules.Payroll);
                        #endregion
                        db.Loans.Attach(entity);
                        db.Loans.Remove(entity);
                        db.SaveChanges();
                    }
               
            }

            return Json(new[] { loan }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Loan");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Loan");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
