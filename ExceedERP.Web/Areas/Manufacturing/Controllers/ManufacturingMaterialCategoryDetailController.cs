using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class ManufacturingMaterialCategoryDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ManufacturingMaterialCategoryDetails_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ManufacturingMaterialCategoryItem> manufacturingmaterialcategorydetails = db.ManufacturingMaterialCategoryItems.Where(x => x.ManufacturingMaterialCategoryId == id);

            DataSourceResult result = manufacturingmaterialcategorydetails.ToDataSourceResult(request, ManufacturingMaterialCategoryItem => new ManufacturingMaterialCategoryItem
            {
                ManufacturingMaterialCategoryItemId = ManufacturingMaterialCategoryItem.ManufacturingMaterialCategoryItemId,
                InventorySubCategoryCode = ManufacturingMaterialCategoryItem.InventorySubCategoryCode,
                ItemName = ManufacturingMaterialCategoryItem.ItemName,
                ItemCode = ManufacturingMaterialCategoryItem.ItemCode,
                UnitOfMeasurement = ManufacturingMaterialCategoryItem.UnitOfMeasurement,
                UnitPrice = ManufacturingMaterialCategoryItem.UnitPrice,
                IsActive = ManufacturingMaterialCategoryItem.IsActive,
                ManufacturingMaterialCategoryId = ManufacturingMaterialCategoryItem.ManufacturingMaterialCategoryId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManufacturingMaterialCategoryDetails_Create([DataSourceRequest]DataSourceRequest request, ManufacturingMaterialCategoryItem model, int id)
        {
            var item = db.InventoryItems.FirstOrDefault(x => x.ItemCode == model.ItemCode);
            if (item == null)
                ModelState.AddModelError(string.Empty, "Inventory Item Not Found");
            var exsitingItems = db.ManufacturingMaterialCategoryItems
                .Where(x => x.ItemCode == model.ItemCode && x.ManufacturingMaterialCategoryId == id);
            if (exsitingItems.Any())
                ModelState.AddModelError("", "Item Exists");
            if (ModelState.IsValid)
            {
                var entity = new ManufacturingMaterialCategoryItem
                {
                    InventorySubCategoryCode = item.SubCategoryCode,
                    ItemName = item.Name,
                    ItemCode = model.ItemCode,
                    UnitOfMeasurement = model.UnitOfMeasurement,
                    UnitPrice = model.UnitPrice,
                    IsActive = model.IsActive,
                    ManufacturingMaterialCategoryId = id
                };

                db.ManufacturingMaterialCategoryItems.Add(entity);
                db.SaveChanges();
                model.ManufacturingMaterialCategoryItemId = entity.ManufacturingMaterialCategoryItemId;
                model.ItemName = entity.ItemName;
                model.ItemCode = entity.ItemCode;
                
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManufacturingMaterialCategoryDetails_Update([DataSourceRequest]DataSourceRequest request, ManufacturingMaterialCategoryItem model)
        {
            var item = db.InventoryItems.FirstOrDefault(x => x.ItemCode == model.ItemCode);
            if (item == null)
                ModelState.AddModelError(string.Empty, "Inventory Item Not Found");
            var exsitingItems = db.ManufacturingMaterialCategoryItems
                .Where(x => x.ItemCode == model.ItemCode 
                && x.ManufacturingMaterialCategoryId == model.ManufacturingMaterialCategoryId
                && x.ManufacturingMaterialCategoryItemId != model.ManufacturingMaterialCategoryItemId);
            if (exsitingItems != null)
                ModelState.AddModelError("", "Existing Item");
            if (ModelState.IsValid)
            {
                var entity = new ManufacturingMaterialCategoryItem
                {
                    ManufacturingMaterialCategoryItemId = model.ManufacturingMaterialCategoryItemId,
                    InventorySubCategoryCode = item.SubCategoryCode,
                    ItemName = item.Name,
                    ItemCode = model.ItemCode,
                    UnitOfMeasurement = model.UnitOfMeasurement,
                    UnitPrice = model.UnitPrice,
                    IsActive = model.IsActive,
                    ManufacturingMaterialCategoryId = model.ManufacturingMaterialCategoryId,
                    
                };

                db.ManufacturingMaterialCategoryItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManufacturingMaterialCategoryDetails_Destroy([DataSourceRequest]DataSourceRequest request, ManufacturingMaterialCategoryItem ManufacturingMaterialCategoryItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ManufacturingMaterialCategoryItem
                {
                    ManufacturingMaterialCategoryItemId = ManufacturingMaterialCategoryItem.ManufacturingMaterialCategoryItemId,
                    InventorySubCategoryCode = ManufacturingMaterialCategoryItem.InventorySubCategoryCode,
                    ItemName = ManufacturingMaterialCategoryItem.ItemName,
                    ItemCode = ManufacturingMaterialCategoryItem.ItemCode,
                    UnitOfMeasurement = ManufacturingMaterialCategoryItem.UnitOfMeasurement,
                    UnitPrice = ManufacturingMaterialCategoryItem.UnitPrice,
                    IsActive = ManufacturingMaterialCategoryItem.IsActive,
                    ManufacturingMaterialCategoryId = ManufacturingMaterialCategoryItem.ManufacturingMaterialCategoryId
                };

                db.ManufacturingMaterialCategoryItems.Attach(entity);
                db.ManufacturingMaterialCategoryItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { ManufacturingMaterialCategoryItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
