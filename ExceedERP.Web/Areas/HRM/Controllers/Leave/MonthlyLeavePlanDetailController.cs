using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.LeaveManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Leave
{
    public class MonthlyLeavePlanDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult MonthlyLeavePlanDetail_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<MonthlyLeavePlanDetail> leaveplandetail = db.MonthlyLeavePlanDetails.Where(e => e.MonthlyLeavePlanId == id);
            DataSourceResult result = leaveplandetail.ToDataSourceResult(request, monthlyLeavePlanDetail => new {

                MonthlyLeavePlanDetailId = monthlyLeavePlanDetail.MonthlyLeavePlanDetailId,
                MonthlyLeavePlanId = monthlyLeavePlanDetail.MonthlyLeavePlanId,
                GlFiscalYearId = monthlyLeavePlanDetail.GlFiscalYearId,
                GLPeriodId = monthlyLeavePlanDetail.GLPeriodId,
                Amount = monthlyLeavePlanDetail.Amount,
                LastModified = monthlyLeavePlanDetail.LastModified,
                DateCreated = monthlyLeavePlanDetail.DateCreated,
                CreatedBy = monthlyLeavePlanDetail.CreatedBy,
                ModifiedBy = monthlyLeavePlanDetail.ModifiedBy,
                Remark = monthlyLeavePlanDetail.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyLeavePlanDetail_Create([DataSourceRequest]DataSourceRequest request, MonthlyLeavePlanDetail monthlyLeavePlanDetail, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyLeavePlanDetail
                {
                    MonthlyLeavePlanId = id,
                    GlFiscalYearId = monthlyLeavePlanDetail.GlFiscalYearId,
                    GLPeriodId = monthlyLeavePlanDetail.GLPeriodId,
                    Amount = monthlyLeavePlanDetail.Amount,
                    LastModified = monthlyLeavePlanDetail.LastModified,
                    DateCreated = monthlyLeavePlanDetail.DateCreated,
                    CreatedBy = monthlyLeavePlanDetail.CreatedBy,
                    ModifiedBy = monthlyLeavePlanDetail.ModifiedBy,
                    Remark = monthlyLeavePlanDetail.Remark
                };

                db.MonthlyLeavePlanDetails.Add(entity);
                db.SaveChanges();

                #region operation log
             
                    UserHelper.OperationLog(Request.UserHostAddress, "Monthly Leave Plan Detail", "Success", User.Identity.Name, User.Identity.Name + "add New Monthly Plan Detail ID " + entity.MonthlyLeavePlanDetailId, Modules.HRM);

                #endregion
            }

            return Json(new[] { monthlyLeavePlanDetail }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyLeavePlanDetail_Update([DataSourceRequest]DataSourceRequest request, MonthlyLeavePlanDetail monthlyLeavePlanDetail)
        {
            var monthlyLeavePlan = db.MonthlyLeavePlans.Find(monthlyLeavePlanDetail.MonthlyLeavePlanId);
            if(monthlyLeavePlan != null)
            {
                var entitlements = db.LeavesEntitle.Where(x=>x.EmployeeId == monthlyLeavePlan.EmployeeId && !x.IsExpired).ToList();
                if(entitlements.Any())
                {
                    var balance = entitlements.Sum(x=>x.Balance);
                    if(monthlyLeavePlanDetail.Amount > balance)
                    {
                        ModelState.AddModelError("","You are planning more than your balance");
                    }
                    var plans = db.MonthlyLeavePlanDetails.Where(x=>x.MonthlyLeavePlanId == monthlyLeavePlanDetail.MonthlyLeavePlanId && x.MonthlyLeavePlanDetailId != monthlyLeavePlanDetail.MonthlyLeavePlanDetailId).ToList();
                    if(plans.Any())
                    {
                        var plansSum = plans.Sum(x=>x.Amount);
                        if(plansSum > balance)
                        {
                            ModelState.AddModelError("", "Sum of your plan can't exceed your balance");
                        }
                    }
                }
            }
           
            if (ModelState.IsValid)
            {
                var entity = new MonthlyLeavePlanDetail
                {
                    MonthlyLeavePlanDetailId = monthlyLeavePlanDetail.MonthlyLeavePlanDetailId,
                    MonthlyLeavePlanId = monthlyLeavePlanDetail.MonthlyLeavePlanId,
                    GlFiscalYearId = monthlyLeavePlanDetail.GlFiscalYearId,
                    GLPeriodId = monthlyLeavePlanDetail.GLPeriodId,
                    Amount = monthlyLeavePlanDetail.Amount,
                    LastModified = monthlyLeavePlanDetail.LastModified,
                    DateCreated = monthlyLeavePlanDetail.DateCreated,
                    CreatedBy = monthlyLeavePlanDetail.CreatedBy,
                    ModifiedBy = monthlyLeavePlanDetail.ModifiedBy,
                    Remark = monthlyLeavePlanDetail.Remark
                };

                db.MonthlyLeavePlanDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                
                #region operation log

                UserHelper.OperationLog(Request.UserHostAddress, "Monthly Leave Plan Detail", "Success", User.Identity.Name, User.Identity.Name + "update Monthly Plan Detail ID " + entity.MonthlyLeavePlanDetailId, Modules.HRM);
                
                #endregion
            }

            return Json(new[] { monthlyLeavePlanDetail }.ToDataSourceResult(request, ModelState));
        }
        
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyLeavePlanDetail_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyLeavePlanDetail monthlyLeavePlanDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyLeavePlanDetail
                {
                    MonthlyLeavePlanDetailId = monthlyLeavePlanDetail.MonthlyLeavePlanDetailId,
                    MonthlyLeavePlanId = monthlyLeavePlanDetail.MonthlyLeavePlanId,
                    GlFiscalYearId = monthlyLeavePlanDetail.GlFiscalYearId,
                    GLPeriodId = monthlyLeavePlanDetail.GLPeriodId,
                    Amount = monthlyLeavePlanDetail.Amount,
                    LastModified = monthlyLeavePlanDetail.LastModified,
                    DateCreated = monthlyLeavePlanDetail.DateCreated,
                    CreatedBy = monthlyLeavePlanDetail.CreatedBy,
                    ModifiedBy = monthlyLeavePlanDetail.ModifiedBy,
                    Remark = monthlyLeavePlanDetail.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Monthly Leave Plan Detail Id " + entity.MonthlyLeavePlanDetailId);
                #endregion
                db.MonthlyLeavePlanDetails.Attach(entity);
                db.MonthlyLeavePlanDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { monthlyLeavePlanDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
