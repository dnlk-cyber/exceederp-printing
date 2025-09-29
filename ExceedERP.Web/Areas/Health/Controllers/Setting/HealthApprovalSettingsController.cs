using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Health.Controllers.Setting
{
    public class HealthApprovalSettingsController : BaseController
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
       
        public JsonResult HealthApprovalSettings_Read([DataSourceRequest] DataSourceRequest request)
        {
            var healthApprovalSettings = db.HealthApprovalSettings.ToList();
            var accidents = healthApprovalSettings.Select(healthApprovalSetting => new HealthApprovalSetting
            {

                HealthApprovalSettingId = healthApprovalSetting.HealthApprovalSettingId,
                RequiresHrApprovalForPaymentRequest = healthApprovalSetting.RequiresHrApprovalForPaymentRequest,
                Description = healthApprovalSetting.Description


            }).ToList();

            return Json(accidents.ToDataSourceResult(request));
        }
        public JsonResult HealthApprovalSettings_Create([DataSourceRequest] DataSourceRequest request, HealthApprovalSetting  healthApprovalSetting)
        {
            var exist = db.HealthBenefitSettings.Any();
            if (exist)
            {
                ModelState.AddModelError("eexist", " already Setted Once you can modify it");
            }
            if(ModelState.IsValid)
            {
                var entity = GetMappedObject(healthApprovalSetting);
                db.HealthApprovalSettings.Add(entity);
                db.SaveChanges();

                #region operation log                
              
                    UserHelper.OperationLog(Request.UserHostAddress, "Health approval setting", "Success", User.Identity.Name, User.Identity.Name + " add health approval setting for  "+ healthApprovalSetting.RequiresHrApprovalForPaymentRequest.ToString() + " at date " + DateTime.Today, Modules.Health);
                #endregion
            }

            return Json(new[] { healthApprovalSetting }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthApprovalSettings_Update([DataSourceRequest] DataSourceRequest request, HealthApprovalSetting healthApprovalSetting)
        {

            var exist = db.HealthApprovalSettings.Any();
            if (exist)
            {
                ModelState.AddModelError("eexist", " this Benefit Type already Setted Once!");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(healthApprovalSetting);
                db.HealthApprovalSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Health Approval setting", "Success", User.Identity.Name, User.Identity.Name + " update Health approval setting of  " + healthApprovalSetting.RequiresHrApprovalForPaymentRequest.ToString() + " at date " + DateTime.Today, Modules.Health);
                

                #endregion

            }

            return Json(new[] { healthApprovalSetting }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthApprovalSettings_Destroy([DataSourceRequest] DataSourceRequest request, HealthApprovalSetting healthApprovalSetting)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(healthApprovalSetting);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted health approval setting " );
                #endregion
                db.HealthApprovalSettings.Attach(entity);
                db.HealthApprovalSettings.Remove(entity);
                db.SaveChanges();

               
            }

            return Json(new[] { healthApprovalSetting }.ToDataSourceResult(request));
        }
        private HealthApprovalSetting GetMappedObject(HealthApprovalSetting  healthApprovalSetting)
        {
            var entity = new HealthApprovalSetting
            {
                HealthApprovalSettingId = healthApprovalSetting.HealthApprovalSettingId,
                RequiresHrApprovalForPaymentRequest = healthApprovalSetting.RequiresHrApprovalForPaymentRequest,
                Description = healthApprovalSetting.Description
            };
            return entity;

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}