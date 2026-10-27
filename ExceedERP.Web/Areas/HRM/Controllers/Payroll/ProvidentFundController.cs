using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using System;
using ExceedERP.Core.Domain.HRM.Payroll;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Payroll
{
   [Authorize(Roles = Payroll_Input + "," + Payroll_Admin)]
    public class ProvidentFundController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        private const string Payroll_Input = "Payroll_Input";
        private const string Payroll_Admin = "Payroll_Admin";
        HRMInputs inputs = new HRMInputs();
        public static DateTime? From { get; set; }
        public static DateTime? To { get; set; }
        public ActionResult Index()
        {
           
            var list = inputs.GetPayrollPlacmentEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = list.Where(x => x.IsActive).Where(x => x.IsActive);
           
            return View();
        }
       [Authorize(Roles = Payroll_Admin + "," + Payroll_Admin + "," + Payroll_Input)]
        public ActionResult ProvidentFunds_Read([DataSourceRequest]DataSourceRequest request)
        {
            //List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            List<int> employees = inputs.GetUserBranchEmployees(User.Identity.Name);
            IQueryable<ProvidentFund> ProvidentFunds = db.ProvidentFunds.Where(x => employees.Contains(x.EmployeeId));
            DataSourceResult result = ProvidentFunds.ToDataSourceResult(request, ProvidentFund => new {
                EmployeeId = ProvidentFund.EmployeeId,
                ProvidentFundId = ProvidentFund.ProvidentFundId,
                IsMemeber = ProvidentFund.IsMemeber,
                Remark = ProvidentFund.Remark,
                DateCreated = ProvidentFund.DateCreated,
                LastModified = ProvidentFund.LastModified,
                CreatedBy = ProvidentFund.CreatedBy,
                ModifiedBy = ProvidentFund.ModifiedBy,
                OrgBranchName = ProvidentFund.OrgBranchName,
                
            });

            return Json(result);
        }
       
        [Authorize(Roles = Payroll_Input)]
       [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProvidentFunds_Create([DataSourceRequest]DataSourceRequest request, ProvidentFund ProvidentFund)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new ProvidentFund
                {
                   EmployeeId = ProvidentFund.EmployeeId,
                   ProvidentFundId = ProvidentFund.ProvidentFundId,
                    Remark = ProvidentFund.Remark,
                   IsMemeber = ProvidentFund.IsMemeber,
                   DateCreated = DateTime.Now,
                   LastModified = ProvidentFund.LastModified,
                   CreatedBy = User.Identity .Name ,
                   ModifiedBy = ProvidentFund.ModifiedBy,
                   OrgBranchName = UserInformationHelper .GetUserDefaultBranch (User.Identity .Name ),
                  
                };

                db.ProvidentFunds.Add(entity);
                db.SaveChanges();
                ProvidentFund.ProvidentFundId = entity.ProvidentFundId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll providunt fund", "Success", User.Identity.Name, $"added providunt fund for employee {entity.Employee.EmployeeCode} is active {entity.IsMemeber} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { ProvidentFund }.ToDataSourceResult(request, ModelState));
        }
      
    
     
        [Authorize(Roles = Payroll_Input)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProvidentFunds_Update([DataSourceRequest]DataSourceRequest request, ProvidentFund ProvidentFund)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ProvidentFund
                {
                   EmployeeId = ProvidentFund.EmployeeId,
                   
                    ProvidentFundId = ProvidentFund.ProvidentFundId,
                    Remark = ProvidentFund.Remark,
                   IsMemeber = ProvidentFund.IsMemeber,
                   DateCreated = ProvidentFund.DateCreated,
                   LastModified = DateTime .Now ,
                   CreatedBy = ProvidentFund.CreatedBy,
                   ModifiedBy = User.Identity .Name ,
                   OrgBranchName = ProvidentFund.OrgBranchName,
                  
                };

                db.ProvidentFunds.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "payroll providunt fund", "Success", User.Identity.Name, $"updated providunt fund for employee {entity.Employee.EmployeeCode} is active {entity.IsMemeber} ", Modules.Payroll);
                #endregion
            }

            return Json(new[] { ProvidentFund }.ToDataSourceResult(request, ModelState));
        }
          [Authorize(Roles = Payroll_Admin + "," + Payroll_Admin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProvidentFunds_Destroy([DataSourceRequest]DataSourceRequest request, ProvidentFund ProvidentFund)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                //var checkexistance = db.PayrollProcessViewModel.Where(x => x.EmployeeId == ProvidentFund.EmployeeId).FirstOrDefault();
                //if (checkexistance != null)
                //{
                //    ModelState.AddModelError("", "You can not delete this ProvidentFund is already processed!!");
                //}
                //else
                //{
                    var entity = new ProvidentFund
                    {
                        EmployeeId = ProvidentFund.EmployeeId,
                        IsMemeber = ProvidentFund.IsMemeber,
                        ProvidentFundId = ProvidentFund.ProvidentFundId,
                        Remark = ProvidentFund.Remark,
                        DateCreated = ProvidentFund.DateCreated,
                        LastModified = ProvidentFund.LastModified,
                        CreatedBy = ProvidentFund.CreatedBy,
                        ModifiedBy = ProvidentFund.ModifiedBy,
                        OrgBranchName = ProvidentFund.OrgBranchName,
                        
                    };
                    #region delete log
                    UserHelper.OperationLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted employee ProvidentFund information of employee " + ProvidentFund.EmployeeId,Modules.Payroll);
                    #endregion
                    db.ProvidentFunds.Attach(entity);
                    db.ProvidentFunds.Remove(entity);
                    db.SaveChanges();
                }
            //}

            return Json(new[] { ProvidentFund }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
