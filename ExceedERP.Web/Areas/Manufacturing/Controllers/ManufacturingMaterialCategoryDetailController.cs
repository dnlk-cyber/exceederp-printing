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
        public ActionResult ManufacturingMaterialCategoryDetails_Create([DataSourceRequest]DataSourceRequest request, ManufacturingMaterialCategoryItem ManufacturingMaterialCategoryItem, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ManufacturingMaterialCategoryItem
                {
                    InventorySubCategoryCode = ManufacturingMaterialCategoryItem.InventorySubCategoryCode,
                    ItemName = ManufacturingMaterialCategoryItem.ItemName,
                    ItemCode = ManufacturingMaterialCategoryItem.ItemCode,
                    UnitOfMeasurement = ManufacturingMaterialCategoryItem.UnitOfMeasurement,
                    UnitPrice = ManufacturingMaterialCategoryItem.UnitPrice,
                    IsActive = ManufacturingMaterialCategoryItem.IsActive,
                    ManufacturingMaterialCategoryId = id
                };

                db.ManufacturingMaterialCategoryItems.Add(entity);
                db.SaveChanges();
                ManufacturingMaterialCategoryItem.ManufacturingMaterialCategoryItemId = entity.ManufacturingMaterialCategoryItemId;
            }

            return Json(new[] { ManufacturingMaterialCategoryItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManufacturingMaterialCategoryDetails_Update([DataSourceRequest]DataSourceRequest request, ManufacturingMaterialCategoryItem ManufacturingMaterialCategoryItem)
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
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ManufacturingMaterialCategoryItem }.ToDataSourceResult(request, ModelState));
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
