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
    public class InventorySettingController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InventorySettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InventorySetting> inventorySettings = _db.InventorySettings;
            DataSourceResult result = inventorySettings.ToDataSourceResult(request, inventorySetting => new {
                InventorySettingId = inventorySetting.InventorySettingId,
                Type = inventorySetting.Type,
                Value = inventorySetting.Value,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InventorySettings_Create([DataSourceRequest]DataSourceRequest request, InventorySetting inventorySetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new InventorySetting
                {
                    Type = inventorySetting.Type,
                    Value = inventorySetting.Value,
                };

                _db.InventorySettings.Add(entity);
                _db.SaveChanges();
                inventorySetting.InventorySettingId = entity.InventorySettingId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "inventory setting", "Success", User.Identity.Name, $"created type {entity.Type} and value {entity.Value}", Modules.Inventory);
                #endregion
            }

            return Json(new[] { inventorySetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InventorySettings_Update([DataSourceRequest]DataSourceRequest request, InventorySetting inventorySetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new InventorySetting
                {
                    InventorySettingId = inventorySetting.InventorySettingId,
                    Type = inventorySetting.Type,
                    Value = inventorySetting.Value,
                };

                _db.InventorySettings.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "inventory setting", "Success", User.Identity.Name, $"updated type {entity.Type} and value {entity.Value}", Modules.Inventory);
                #endregion
            }

            return Json(new[] { inventorySetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InventorySettings_Destroy([DataSourceRequest]DataSourceRequest request, InventorySetting inventorySetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new InventorySetting
                {
                    InventorySettingId = inventorySetting.InventorySettingId,
                    Type = inventorySetting.Type,
                    Value = inventorySetting.Value,
                };

                _db.InventorySettings.Attach(entity);
                _db.InventorySettings.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted inventory setting " + inventorySetting.InventorySettingId + " " + inventorySetting.Type);
                #endregion
            }

            return Json(new[] { inventorySetting }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileType)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileType);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileType)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileType);
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
