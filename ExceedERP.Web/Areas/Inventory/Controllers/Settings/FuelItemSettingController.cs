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
    public class FuelItemSettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }

        private void PopulateViewData()
        {
            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            ViewData["Items"] = items;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();
        }

        public ActionResult FuelItemSettings_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FuelItemSetting> fuelitemsettings = db.FuelItemSettings;
            DataSourceResult result = fuelitemsettings.ToDataSourceResult(request, fuelItemSetting => new
            {
                FuelItemSettingId = fuelItemSetting.FuelItemSettingId,
                ItemCategoryCode = fuelItemSetting.ItemCategoryCode,
                ItemCode = fuelItemSetting.ItemCode,
                DateCreated = fuelItemSetting.DateCreated,
                LastModified = fuelItemSetting.LastModified,
                CreatedBy = fuelItemSetting.CreatedBy,
                ModifiedBy = fuelItemSetting.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelItemSettings_Create([DataSourceRequest]DataSourceRequest request, FuelItemSetting fuelItemSetting)
        {
              #region validation

                bool valid = true;
                var exists = db.FuelItemSettings.Where(x => x.ItemCode == fuelItemSetting.ItemCode).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("FuelItem",fuelItemSetting.ItemCode + " already exists in the system.");
                    valid = false;
                }

              #endregion

                
                
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FuelItemSetting
                    {
                        ItemCategoryCode = fuelItemSetting.ItemCategoryCode,
                        ItemCode = fuelItemSetting.ItemCode,
                        DateCreated = DateTime.Now,
                        LastModified = fuelItemSetting.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = fuelItemSetting.ModifiedBy
                    };

                    db.FuelItemSettings.Add(entity);
                    db.SaveChanges();
                    fuelItemSetting.FuelItemSettingId = entity.FuelItemSettingId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "fuel item setting", "Success", User.Identity.Name, $"created item {entity.ItemCode} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { fuelItemSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelItemSettings_Update([DataSourceRequest]DataSourceRequest request, FuelItemSetting fuelItemSetting)
        
        {
            
            #region validation

                bool valid = true;
                var exists = db.FuelItemSettings.Where(x => x.FuelItemSettingId != fuelItemSetting.FuelItemSettingId)
                                                 .Where(x => x.ItemCode == fuelItemSetting.ItemCode).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("FuelItem",fuelItemSetting.ItemCode + " already exists in the system.");
                    valid = false;
                }

              #endregion

                
                
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FuelItemSetting
                    {
                        FuelItemSettingId = fuelItemSetting.FuelItemSettingId,
                        ItemCategoryCode = fuelItemSetting.ItemCategoryCode,
                        ItemCode = fuelItemSetting.ItemCode,
                        DateCreated = fuelItemSetting.DateCreated,
                        LastModified =DateTime.Now,
                        CreatedBy = fuelItemSetting.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };

                    db.FuelItemSettings.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "fuel item setting", "Success", User.Identity.Name, $"updated item {entity.ItemCode} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { fuelItemSetting }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FuelItemSettings_Destroy([DataSourceRequest]DataSourceRequest request, FuelItemSetting fuelItemSetting)
        {
            if (ModelState.IsValid)
            {
                var entity = new FuelItemSetting
                {
                    FuelItemSettingId = fuelItemSetting.FuelItemSettingId,
                    ItemCategoryCode = fuelItemSetting.ItemCategoryCode,
                    ItemCode = fuelItemSetting.ItemCode,
                    DateCreated = fuelItemSetting.DateCreated,
                    LastModified = fuelItemSetting.LastModified,
                    CreatedBy = fuelItemSetting.CreatedBy,
                    ModifiedBy = fuelItemSetting.ModifiedBy
                };

                db.FuelItemSettings.Attach(entity);
                db.FuelItemSettings.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted fuel item setting "+fuelItemSetting.InventoryItem.Name);
                #endregion
            }

            return Json(new[] { fuelItemSetting }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
