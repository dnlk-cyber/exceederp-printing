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
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Helpers.Common;
using System;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
     [Authorize(Roles = Payroll_Input + "," + Payroll_Admin)]
    public class AdvanceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            var periodList = db.PayrollPeriods.Where(x => x.Status == FiscalYearStatus.Open || x.Status == FiscalYearStatus.Close);
            var list = from p in periodList
                       join g in db.GLPeriods
                           on p.Name equals g.Name
                       select (new
                       {
                           g
                       });
            ViewData["PeriodList"] = list.Select(x => x.g).ToList(); ;
        
           var lists = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = lists.Where(x => x.IsActive);
            return View();
        }
    
        public ActionResult Advances_Read([DataSourceRequest]DataSourceRequest request)
        {
           //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<string> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<Advance> advances = db.Advances.Where(x => employees.Contains(x.EmployeeId.ToString()));
            DataSourceResult result = advances.ToDataSourceResult(request, advance => new {
                AdvanceID = advance.AdvanceId,
                EmployeeId = advance.EmployeeId,
                AdvanceAmount = advance.AdvanceAmount,
                Months = advance.Months,
                JumpHoliday = advance.JumpHoliday,
                Description = advance.Description,
                Balance = advance.Balance,
                MonthlyAmount = advance.MonthlyAmount,
                GLPeriodId = advance.GLPeriodId,
                Remark = advance.Remark,
                DateCreated = advance.DateCreated,
                LastModified = advance.LastModified,
                CreatedBy = advance.CreatedBy,
                OrgBranchName = advance.OrgBranchName,
                ModifiedBy = advance.ModifiedBy,
                IsActive = advance.IsActive
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Advances_Create([DataSourceRequest]DataSourceRequest request, Advance advance)
        {
            if (advance.Months != 0)
            {
                advance.MonthlyAmount = advance.AdvanceAmount/advance.Months;
                advance.Balance = advance.AdvanceAmount;
            }

            ModelState.Clear();
            if (ModelState.IsValid)
            {
                advance.OrgBranchName =UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                var checkexistancee = db.PayrollProcessViewModel.Where(x => x.GLPeriodId == advance.GLPeriodId && advance.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistancee != null)
                {


                    ModelState.AddModelError("", "You cannot add this data. This period is already processed!!");
                }
                else
                {

                    var checkexistance = db.Advances.Where(x => x.EmployeeId == advance.EmployeeId && x.GLPeriodId == advance.GLPeriodId).FirstOrDefault();
                    if (checkexistance != null)
                    {
                        ModelState.AddModelError("", "This Advance on this period is already registered!!");
                    }
                    else if (advance.Months == 0)
                    {
                        ModelState.AddModelError("", "No of month should be greater than 0!!");
                    }

                    else
                    {
                        var entity = new Advance
                        {
                            EmployeeId = advance.EmployeeId,
                            AdvanceAmount = advance.AdvanceAmount,
                            Months = advance.Months,
                            JumpHoliday = advance.JumpHoliday,
                            Description = advance.Description,
                            GLPeriodId = advance.GLPeriodId,
                            Balance = advance.Balance,
                            MonthlyAmount = advance.MonthlyAmount,
                            Remark = advance.Remark,
                            DateCreated = DateTime.Now,
                            LastModified = advance.LastModified,
                            CreatedBy = User.Identity.Name,
                            OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                            ModifiedBy = advance.ModifiedBy,
                            IsActive = advance.IsActive
                        };

                        db.Advances.Add(entity);
                        db.SaveChanges();
                        advance.AdvanceId = entity.AdvanceId;

                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "payroll addition", "Success", User.Identity.Name, $"added adavnce for period {entity.GLPeriodId} and employee {entity.EmployeeId} type {entity.Description} amount {entity.Balance} ", Modules.Payroll);
                        #endregion
                    }
                }
            }
            return Json(new[] { advance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Advances_Update([DataSourceRequest]DataSourceRequest request, Advance advance)
        {
            if (ModelState.IsValid)
            {
                if (advance.Months != 0)
                {
                    advance.MonthlyAmount = advance.AdvanceAmount / advance.Months;
                    advance.Balance = advance.AdvanceAmount;
                }

                if (advance.Months == 0)
                {
                    ModelState.AddModelError("", "No of month should be greater than 0!!");
                }

                else
                {

                        var checkLoanS = db.Advances.Where(x => x.AdvanceId == advance.AdvanceId).FirstOrDefault();
                    if (checkLoanS != null)
                    {
                        if (checkLoanS.IsActive != advance.IsActive)
                        {
                            var entity = new Advance
                            {
                                AdvanceId = checkLoanS.AdvanceId,
                                EmployeeId = checkLoanS.EmployeeId,
                                AdvanceAmount = checkLoanS.AdvanceAmount,
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
                                IsActive = advance.IsActive
                            };
                            db = new ExceedDbContext();
                            db.Advances.Attach(entity);
                            db.Entry(entity).State = EntityState.Modified;
                            db.SaveChanges();

                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "payroll addition", "Success", User.Identity.Name, $"updated adavnce for period {entity.GLPeriodId} and employee {entity.EmployeeId} type {entity.Description} amount {entity.Balance} ", Modules.Payroll);
                            #endregion
                        }
                        else
                        {


                            var checkexistance =
                                db.PayrollProcessViewModel.Where(
                                    x =>
                                        x.EmployeeId == advance.EmployeeId && x.GLPeriodId == advance.GLPeriodId &&
                                        advance.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                            if (checkexistance != null)
                            {
                                var checkLoan =
                                    db.PayrollProcessAdvances.Where(
                                        x =>
                                            x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId &&
                                            x.AdvanceId == advance.AdvanceId).FirstOrDefault();
                                if (checkLoan != null)
                                {
                                    ModelState.AddModelError("",
                                        "You can update only is active value, it already processed!!");
                                }
                                else
                                {

                                    var entity = new Advance
                                    {
                                        AdvanceId = advance.AdvanceId,
                                        EmployeeId = advance.EmployeeId,
                                        AdvanceAmount = advance.AdvanceAmount,
                                        Months = advance.Months,
                                        JumpHoliday = advance.JumpHoliday,
                                        Description = advance.Description,
                                        Balance = advance.Balance,
                                        MonthlyAmount = advance.MonthlyAmount,
                                        GLPeriodId = advance.GLPeriodId,
                                        Remark = advance.Remark,
                                        DateCreated = advance.DateCreated,
                                        LastModified = DateTime.Now,
                                        CreatedBy = advance.CreatedBy,
                                        ModifiedBy = User.Identity.Name,
                                        OrgBranchName = advance.OrgBranchName,
                                        IsActive = advance.IsActive
                                    };
                                    db = new ExceedDbContext();
                                    db.Advances.Attach(entity);
                                    db.Entry(entity).State = EntityState.Modified;
                                    db.SaveChanges();

                                    #region operation log
                                    UserHelper.OperationLog(Request.UserHostAddress, "payroll addition", "Success", User.Identity.Name, $"updated adavnce for period {entity.GLPeriodId} and employee {entity.EmployeeId} type {entity.Description} amount {entity.Balance} ", Modules.Payroll);
                                    #endregion
                                }
                            }

                            else
                            {
                                var entity = new Advance
                                {
                                    AdvanceId = advance.AdvanceId,
                                    EmployeeId = advance.EmployeeId,
                                    AdvanceAmount = advance.AdvanceAmount,
                                    Months = advance.Months,
                                    JumpHoliday = advance.JumpHoliday,
                                    Description = advance.Description,
                                    Balance = advance.Balance,
                                    MonthlyAmount = advance.MonthlyAmount,
                                    GLPeriodId = advance.GLPeriodId,
                                    Remark = advance.Remark,
                                    DateCreated = advance.DateCreated,
                                    LastModified = DateTime.Now,
                                    CreatedBy = advance.CreatedBy,
                                    ModifiedBy = User.Identity.Name,
                                    OrgBranchName = advance.OrgBranchName,
                                    IsActive = advance.IsActive
                                };
                                db = new ExceedDbContext();
                                db.Advances.Attach(entity);
                                db.Entry(entity).State = EntityState.Modified;
                                db.SaveChanges();

                                #region operation log
                                UserHelper.OperationLog(Request.UserHostAddress, "payroll addition", "Success", User.Identity.Name, $"updated adavnce for period {entity.GLPeriodId} and employee {entity.EmployeeId} type {entity.Description} amount {entity.Balance} ", Modules.Payroll);
                                #endregion
                            }
                        }
                    }
                }
            }

            return Json(new[] { advance }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Advances_Destroy([DataSourceRequest]DataSourceRequest request, Advance advance)
        {
            if (ModelState.IsValid)
            {
                var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == advance.EmployeeId && x.GLPeriodId == advance.GLPeriodId && advance.OrgBranchName == x.BranchId.ToString()).FirstOrDefault();
                if (checkexistance != null)
                {
                    //var checkLoan = db.PayrollProcessAdvances.Where(x => x.PayrollProcessViewModelId == checkexistance.PayrollProcessViewModelId && x.AdvanceId == advance.AdvanceId).FirstOrDefault();
                    //if (checkLoan != null)
                    //{
                       ModelState.AddModelError("", "You cannot delete this data . It already processed!!");
                    }
                    else
                    {
                        var entity = new Advance
                        {
                            AdvanceId = advance.AdvanceId,
                            EmployeeId = advance.EmployeeId,
                            AdvanceAmount = advance.AdvanceAmount,
                            Months = advance.Months,
                            JumpHoliday = advance.JumpHoliday,
                            Description = advance.Description,
                            Balance = advance.Balance,
                            MonthlyAmount = advance.MonthlyAmount,
                            GLPeriodId = advance.GLPeriodId,
                            Remark = advance.Remark,
                            DateCreated = advance.DateCreated,
                            LastModified = advance.LastModified,
                            CreatedBy = advance.CreatedBy,
                            ModifiedBy = advance.ModifiedBy,
                            OrgBranchName = advance.OrgBranchName,
                            IsActive = advance.IsActive
                        };

                        db.Advances.Attach(entity);
                        db.Advances.Remove(entity);
                        db.SaveChanges();
                        #region delete log
                        UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Advance information of employee " + advance.EmployeeId,Modules.Payroll);
                        #endregion
                    }
                
            }
               
           
            

            return Json(new[] { advance }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Advance");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Advance");
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
