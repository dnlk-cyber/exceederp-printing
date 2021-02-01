using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Settings
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
    public class AveragePriceSettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.Count = db.AveragePriceSettings.ToList().Count();
            return View();
        }

        public ActionResult AveragePriceSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AveragePriceSetting> averagepricesettings = db.AveragePriceSettings;
            DataSourceResult result = averagepricesettings.ToDataSourceResult(request, averagePriceSetting => new {
                AveragePriceSettingId = averagePriceSetting.AveragePriceSettingId,
                Level = averagePriceSetting.Level,
                DateCreated = averagePriceSetting.DateCreated,
                LastModified = averagePriceSetting.LastModified,
                CreatedBy = averagePriceSetting.CreatedBy,
                ModifiedBy = averagePriceSetting.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AveragePriceSettings_Create([DataSourceRequest]DataSourceRequest request, AveragePriceSetting averagePriceSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new AveragePriceSetting
                {
                    Level = averagePriceSetting.Level,
                    DateCreated = DateTime.Now,
                    LastModified = averagePriceSetting.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = averagePriceSetting.ModifiedBy
                };

                db.AveragePriceSettings.Add(entity);
                db.SaveChanges();
                averagePriceSetting.AveragePriceSettingId = entity.AveragePriceSettingId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Average price setting", "Success", User.Identity.Name, $"created  {averagePriceSetting.Level}" , Modules.Inventory);
                #endregion
            }

            return Json(new[] { averagePriceSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AveragePriceSettings_Update([DataSourceRequest]DataSourceRequest request, AveragePriceSetting averagePriceSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new AveragePriceSetting
                {
                    AveragePriceSettingId = averagePriceSetting.AveragePriceSettingId,
                    Level = averagePriceSetting.Level,
                    DateCreated = averagePriceSetting.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = averagePriceSetting.CreatedBy,
                    ModifiedBy =User.Identity.Name
                };

                db.AveragePriceSettings.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Average price setting", "Success", User.Identity.Name, $"updated  {averagePriceSetting.Level}", Modules.Inventory);
                #endregion
            }

            return Json(new[] { averagePriceSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AveragePriceSettings_Destroy([DataSourceRequest]DataSourceRequest request, AveragePriceSetting averagePriceSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new AveragePriceSetting
                {
                    AveragePriceSettingId = averagePriceSetting.AveragePriceSettingId,
                    Level = averagePriceSetting.Level,
                    DateCreated = averagePriceSetting.DateCreated,
                    LastModified = averagePriceSetting.LastModified,
                    CreatedBy = averagePriceSetting.CreatedBy,
                    ModifiedBy = averagePriceSetting.ModifiedBy
                };

                db.AveragePriceSettings.Attach(entity);
                db.AveragePriceSettings.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted average price setting");
                #endregion
            }

            return Json(new[] { averagePriceSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
