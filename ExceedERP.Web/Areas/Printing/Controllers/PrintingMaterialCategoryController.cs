using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Manufacturing.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Printing;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingMaterialCategoryController : Controller
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

        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var PrintingMaterialCategories = db.PrintingMaterialCategories;
            DataSourceResult result = PrintingMaterialCategories.ToDataSourceResult(request, PrintingMaterialCategory => new
            {
                PrintingMaterialCategoryId = PrintingMaterialCategory.PrintingMaterialCategoryId,
                InventoryCategoryCode = PrintingMaterialCategory.InventoryCategoryCode,
                CategoryName = PrintingMaterialCategory.CategoryName,
                CostAccountCode = PrintingMaterialCategory.CostAccountCode,
                InventoryAccountCode = PrintingMaterialCategory.InventoryAccountCode,
                ProductionAccountCode = PrintingMaterialCategory.ProductionAccountCode,
                IsActive = PrintingMaterialCategory.IsActive
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategory PrintingMaterialCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMaterialCategory
                {
                    InventoryCategoryCode = PrintingMaterialCategory.InventoryCategoryCode,
                    CategoryName = PrintingMaterialCategory.CategoryName,
                    CostAccountCode = PrintingMaterialCategory.CostAccountCode,
                    InventoryAccountCode = PrintingMaterialCategory.InventoryAccountCode,
                    ProductionAccountCode = PrintingMaterialCategory.ProductionAccountCode,
                    IsActive = PrintingMaterialCategory.IsActive
                };

                db.PrintingMaterialCategories.Add(entity);
                db.SaveChanges();
                GenerateManufactureCategoryDetail(entity);
                PrintingMaterialCategory.PrintingMaterialCategoryId = entity.PrintingMaterialCategoryId;
            }

            return Json(new[] { PrintingMaterialCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategory PrintingMaterialCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMaterialCategory
                {
                    PrintingMaterialCategoryId = PrintingMaterialCategory.PrintingMaterialCategoryId,
                    InventoryCategoryCode = PrintingMaterialCategory.InventoryCategoryCode,
                     CategoryName = PrintingMaterialCategory.CategoryName,
                    CostAccountCode = PrintingMaterialCategory.CostAccountCode,
                    InventoryAccountCode = PrintingMaterialCategory.InventoryAccountCode,
                    ProductionAccountCode = PrintingMaterialCategory.ProductionAccountCode,
                    IsActive = PrintingMaterialCategory.IsActive
                };

                db.PrintingMaterialCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { PrintingMaterialCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategory PrintingMaterialCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMaterialCategory
                {
                    PrintingMaterialCategoryId = PrintingMaterialCategory.PrintingMaterialCategoryId,
                    InventoryCategoryCode = PrintingMaterialCategory.InventoryCategoryCode,
                     CategoryName = PrintingMaterialCategory.CategoryName,
                    CostAccountCode = PrintingMaterialCategory.CostAccountCode,
                    InventoryAccountCode = PrintingMaterialCategory.InventoryAccountCode,
                    ProductionAccountCode = PrintingMaterialCategory.ProductionAccountCode,
                    IsActive = PrintingMaterialCategory.IsActive
                };

                db.PrintingMaterialCategories.Attach(entity);
                db.PrintingMaterialCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { PrintingMaterialCategory }.ToDataSourceResult(request, ModelState));
        }

        private void GenerateManufactureCategoryDetail(PrintingMaterialCategory materialCategory)
        {
            var itemCategory = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == materialCategory.InventoryCategoryCode);

            var items = db.InventoryItems.Where(x => x.ItemCategoryCode == itemCategory.ItemCategoryCode).ToList();
            foreach (var item in items)
            {
                var entity = new PrintingMaterialCategoryItem
                {
                    ItemName = item.Name,
                    ItemCode = item.ItemCode,
                    UnitOfMeasurement = item.UoMName,
                    InventorySubCategoryCode = item.SubCategoryCode,
                    UnitPrice = 0,
                    IsActive = true,
                    PrintingMaterialCategoryId = materialCategory.PrintingMaterialCategoryId
                };
                db.PrintingMaterialCategoryItems.Add(entity);
                db.SaveChanges();
            }

        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
