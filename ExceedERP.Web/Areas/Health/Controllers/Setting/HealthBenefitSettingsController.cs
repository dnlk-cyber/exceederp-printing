using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Health;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Health.Controllers.Setting
{
    public class HealthBenefitSettingsController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
       
        public JsonResult HealthBenefitSettings_Read([DataSourceRequest] DataSourceRequest request)
        {
            var healthBenefits = db.HealthBenefitSettings.ToList();
            var accidents = healthBenefits.Select(healthBenefit => new HealthBenefitSetting
            {

                HealthBenefitSettingId = healthBenefit.HealthBenefitSettingId,
                BenefitType = healthBenefit.BenefitType,
                BenfitValue = healthBenefit.BenfitValue,
                Description = healthBenefit.Description


            }).ToList();

            return Json(accidents.ToDataSourceResult(request));
        }
        public JsonResult HealthBenefitSettings_Create([DataSourceRequest] DataSourceRequest request, HealthBenefitSetting health)
        {
            var exist = db.HealthBenefitSettings.Where(x => x.BenefitType == health.BenefitType).Any();
            if (exist)
            {
                ModelState.AddModelError("eexist", " already Setted Once you can modify it");
            }
            if(ModelState.IsValid)
            {
                var entity = GetMappedObject(health);
                db.HealthBenefitSettings.Add(entity);
                db.SaveChanges();

                #region operation log                
              
                    UserHelper.OperationLog(Request.UserHostAddress, "Health benefit setting", "Success", User.Identity.Name, User.Identity.Name + " add health benefit setting for  "+ health.BenefitType.ToString() + " at date " + DateTime.Today, Modules.Health);
                #endregion
            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthBenefitSettings_Update([DataSourceRequest] DataSourceRequest request, HealthBenefitSetting health)
        {

            var exist = db.HealthBenefitSettings.Where(x => x.BenefitType == health.BenefitType&&x.HealthBenefitSettingId !=health.HealthBenefitSettingId).Any();
            if (exist)
            {
                ModelState.AddModelError("eexist", " this Benefit Type already Setted Once!");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(health);
                db.HealthBenefitSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log                
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Health benefit setting", "Success", User.Identity.Name, User.Identity.Name + " update health benefit setting of  " + health.BenefitType.ToString() + " at date " + DateTime.Today, Modules.Health);
                

                #endregion

            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthBenefitSettings_Destroy([DataSourceRequest] DataSourceRequest request, HealthBenefitSetting health)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(health);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted health benefit setting " + health.BenefitType.ToString());
                #endregion
                db.HealthBenefitSettings.Attach(entity);
                db.HealthBenefitSettings.Remove(entity);
                db.SaveChanges();

               
            }

            return Json(new[] { health }.ToDataSourceResult(request));
        }
        private HealthBenefitSetting GetMappedObject(HealthBenefitSetting healthBenefit)
        {
            var entity = new HealthBenefitSetting
            {
                HealthBenefitSettingId = healthBenefit.HealthBenefitSettingId,
                BenefitType = healthBenefit.BenefitType,
                BenfitValue = healthBenefit.BenfitValue,
                Description = healthBenefit.Description
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