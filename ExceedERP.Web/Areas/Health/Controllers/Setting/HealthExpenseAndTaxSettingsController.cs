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
    public class HealthExpenseAndTaxSettingsController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
       
        public JsonResult HealthExpenseAndTaxSettings_Read([DataSourceRequest] DataSourceRequest request)
        {
            var healthExpenses = db.HealthExpenseAndTaxSettings.ToList();
            var accidents = healthExpenses.Select(healthExpense => new HealthExpenseAndTaxSetting
            {

                HealthExpenseAndTaxSettingId = healthExpense.HealthExpenseAndTaxSettingId,
                GLTaxRateID = healthExpense.GLTaxRateID,
                ApSetupItemId = healthExpense.ApSetupItemId,
                Description = healthExpense.Description


            }).ToList();

            return Json(accidents.ToDataSourceResult(request));
        }
        public JsonResult HealthExpenseAndTaxSettings_Create([DataSourceRequest] DataSourceRequest request, HealthExpenseAndTaxSetting healthExpense)
        {
            var exist = db.HealthExpenseAndTaxSettings.FirstOrDefault();
            if (exist !=null)
            {
                ModelState.AddModelError("exist", " already you have  Setted One !  you can modify it");
            }
            if(ModelState.IsValid)
            {
                var entity = GetMappedObject(healthExpense);
                db.HealthExpenseAndTaxSettings.Add(entity);
                db.SaveChanges();

                #region operation log                
              
                    UserHelper.OperationLog(Request.UserHostAddress, "Health Expense And Tax Setting ", "Success", User.Identity.Name, User.Identity.Name + " add health benefit setting for  "+ healthExpense.ApSetupItemId + " at date " + DateTime.Today, Modules.Health);
                #endregion
            }

            return Json(new[] { healthExpense }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthExpenseAndTaxSettings_Update([DataSourceRequest] DataSourceRequest request, HealthExpenseAndTaxSetting healthExpense)
        {

         
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(healthExpense);
                db.HealthExpenseAndTaxSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log                
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Health Expense And Tax Setting", "Success", User.Identity.Name, User.Identity.Name + " update health benefit setting of  " + healthExpense.ApSetupItemId + " at date " + DateTime.Today, Modules.Health);
                

                #endregion

            }

            return Json(new[] { healthExpense }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public JsonResult HealthExpenseAndTaxSettings_Destroy([DataSourceRequest] DataSourceRequest request, HealthExpenseAndTaxSetting healthExpense)
        {

            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(healthExpense);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted health benefit setting " + healthExpense.ApSetup.Name);
                #endregion
                db.HealthExpenseAndTaxSettings.Attach(entity);
                db.HealthExpenseAndTaxSettings.Remove(entity);
                db.SaveChanges();

               
            }

            return Json(new[] { healthExpense }.ToDataSourceResult(request));
        }
        private HealthExpenseAndTaxSetting GetMappedObject(HealthExpenseAndTaxSetting healthExpense)
        {
            var entity = new HealthExpenseAndTaxSetting
            {
                HealthExpenseAndTaxSettingId = healthExpense.HealthExpenseAndTaxSettingId,
                GLTaxRateID = healthExpense.GLTaxRateID,
                ApSetupItemId = healthExpense.ApSetupItemId,
                Description = healthExpense.Description
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