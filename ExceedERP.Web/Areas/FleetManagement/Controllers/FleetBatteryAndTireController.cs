using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(InventoryConstants.PropertyAdmin)]
    public class FleetBatteryAndTireController : Controller
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

        public ActionResult FleetBatteryAndTire_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FleetBatteryAndTire> batteryAndTire = db.FleetBatteryAndTires;
            DataSourceResult result = batteryAndTire.ToDataSourceResult(request, model => new
            {
                FleetBatteryAndTireId = model.FleetBatteryAndTireId,
                ItemCategoryCode = model.ItemCategoryCode,
                ItemCode = model.ItemCode,
                DateCreated = model.DateCreated,
                LastModified = model.LastModified,
                CreatedBy = model.CreatedBy,
                ModifiedBy = model.ModifiedBy,
                FleetBatteryOrTireType = model.FleetBatteryOrTireType
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetBatteryAndTire_Create([DataSourceRequest]DataSourceRequest request, FleetBatteryAndTire model)
        {
              #region validation

                bool valid = true;
                var exists = db.FleetBatteryAndTires.Where(x => x.ItemCode == model.ItemCode).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("",model.ItemCode + " already exists in the system.");
                    valid = false;
                }

              #endregion

                
                
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FleetBatteryAndTire
                    {
                        ItemCategoryCode = model.ItemCategoryCode,
                        ItemCode = model.ItemCode,
                        DateCreated = DateTime.Now,
                        LastModified = model.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = model.ModifiedBy,
                        FleetBatteryOrTireType = model.FleetBatteryOrTireType
                    };

                    db.FleetBatteryAndTires.Add(entity);
                    db.SaveChanges();
                    model.FleetBatteryAndTireId = entity.FleetBatteryAndTireId;
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "battery item setting", "Success", User.Identity.Name, $"created item {entity.ItemCode} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetBatteryAndTire_Update([DataSourceRequest]DataSourceRequest request, FleetBatteryAndTire model)
        
        {
            
            #region validation

                bool valid = true;
                var exists = db.FleetBatteryAndTires.Where(x => x.FleetBatteryAndTireId != model.FleetBatteryAndTireId)
                                                 .Where(x => x.ItemCode == model.ItemCode).ToList();

                if (exists.Any())
                {
                    ModelState.AddModelError("FuelItem",model.ItemCode + " already exists in the system.");
                    valid = false;
                }

              #endregion

                
                
            if (ModelState.IsValid)
            {
                if (valid)
                {
                    var entity = new FleetBatteryAndTire
                    {
                        FleetBatteryAndTireId = model.FleetBatteryAndTireId,
                        ItemCategoryCode = model.ItemCategoryCode,
                        ItemCode = model.ItemCode,
                        DateCreated = model.DateCreated,
                        LastModified =DateTime.Now,
                        CreatedBy = model.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        FleetBatteryOrTireType = model.FleetBatteryOrTireType
                    };

                    db.FleetBatteryAndTires.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "fuel item setting", "Success", User.Identity.Name, $"updated item {entity.ItemCode} ", Modules.Inventory);
                    #endregion
                }
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FleetBatteryAndTire_Destroy([DataSourceRequest]DataSourceRequest request, FleetBatteryAndTire model)
        {
            if (ModelState.IsValid)
            {
                var entity = new FleetBatteryAndTire
                {
                    FleetBatteryAndTireId = model.FleetBatteryAndTireId,
                    ItemCategoryCode = model.ItemCategoryCode,
                    ItemCode = model.ItemCode,
                    DateCreated = model.DateCreated,
                    LastModified = model.LastModified,
                    CreatedBy = model.CreatedBy,
                    ModifiedBy = model.ModifiedBy,
                    FleetBatteryOrTireType = model.FleetBatteryOrTireType
                };

                db.FleetBatteryAndTires.Attach(entity);
                db.FleetBatteryAndTires.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted fuel item setting "+model.InventoryItem.Name);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
