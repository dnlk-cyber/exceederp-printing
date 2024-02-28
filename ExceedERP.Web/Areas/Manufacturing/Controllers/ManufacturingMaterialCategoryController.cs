using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers
{
    public class ManufacturingMaterialCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {

            ViewData["ItemCategory"] = db.ItemCategorys.Where(x => !x.HaveParent).Select(s => new
            {

                Text = s.Name,
                Value = s.ItemCategoryCode
            });
            
            return View();
        }

        public ActionResult ManufacturingMaterialCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ManufacturingMaterialCategory> manufacturingmaterialcategories = db.ManufacturingMaterialCategories;
            DataSourceResult result = manufacturingmaterialcategories.ToDataSourceResult(request, manufacturingMaterialCategory => new {
                ManufacturingMaterialCategoryId = manufacturingMaterialCategory.ManufacturingMaterialCategoryId,
                InventoryCategoryCode = manufacturingMaterialCategory.InventoryCategoryCode,
                ManufacturingCategory = manufacturingMaterialCategory.ManufacturingCategory,
                CostAccountCode = manufacturingMaterialCategory.CostAccountCode,
                InventoryAccountCode = manufacturingMaterialCategory.InventoryAccountCode,
                ProductionAccountCode = manufacturingMaterialCategory.ProductionAccountCode,
                IsActive = manufacturingMaterialCategory.IsActive
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManufacturingMaterialCategories_Create([DataSourceRequest]DataSourceRequest request, ManufacturingMaterialCategory manufacturingMaterialCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ManufacturingMaterialCategory
                {
                    InventoryCategoryCode = manufacturingMaterialCategory.InventoryCategoryCode,
                    ManufacturingCategory = manufacturingMaterialCategory.ManufacturingCategory,
                    CostAccountCode = manufacturingMaterialCategory.CostAccountCode,
                    InventoryAccountCode = manufacturingMaterialCategory.InventoryAccountCode,
                    ProductionAccountCode = manufacturingMaterialCategory.ProductionAccountCode,
                    IsActive = manufacturingMaterialCategory.IsActive
                };

                db.ManufacturingMaterialCategories.Add(entity);
                db.SaveChanges();
                manufacturingMaterialCategory.ManufacturingMaterialCategoryId = entity.ManufacturingMaterialCategoryId;
            }

            return Json(new[] { manufacturingMaterialCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManufacturingMaterialCategories_Update([DataSourceRequest]DataSourceRequest request, ManufacturingMaterialCategory manufacturingMaterialCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ManufacturingMaterialCategory
                {
                    ManufacturingMaterialCategoryId = manufacturingMaterialCategory.ManufacturingMaterialCategoryId,
                    InventoryCategoryCode = manufacturingMaterialCategory.InventoryCategoryCode,
                    ManufacturingCategory = manufacturingMaterialCategory.ManufacturingCategory,
                    CostAccountCode = manufacturingMaterialCategory.CostAccountCode,
                    InventoryAccountCode = manufacturingMaterialCategory.InventoryAccountCode,
                    ProductionAccountCode = manufacturingMaterialCategory.ProductionAccountCode,
                    IsActive = manufacturingMaterialCategory.IsActive
                };

                db.ManufacturingMaterialCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { manufacturingMaterialCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManufacturingMaterialCategories_Destroy([DataSourceRequest]DataSourceRequest request, ManufacturingMaterialCategory manufacturingMaterialCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ManufacturingMaterialCategory
                {
                    ManufacturingMaterialCategoryId = manufacturingMaterialCategory.ManufacturingMaterialCategoryId,
                    InventoryCategoryCode = manufacturingMaterialCategory.InventoryCategoryCode,
                    ManufacturingCategory = manufacturingMaterialCategory.ManufacturingCategory,
                    CostAccountCode = manufacturingMaterialCategory.CostAccountCode,
                    InventoryAccountCode = manufacturingMaterialCategory.InventoryAccountCode,
                    ProductionAccountCode = manufacturingMaterialCategory.ProductionAccountCode,
                    IsActive = manufacturingMaterialCategory.IsActive
                };

                db.ManufacturingMaterialCategories.Attach(entity);
                db.ManufacturingMaterialCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { manufacturingMaterialCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
