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
            var printingMaterialCategories = db.PrintingMaterialCategories;
            return Json(printingMaterialCategories.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategory model)
        {
            var exsistingCategoryCode = db.PrintingMaterialCategories
                .Where(x => x.InventoryCategoryCode == model.InventoryCategoryCode);
            if (exsistingCategoryCode.Any())
                ModelState.AddModelError("", "Exsisting Category Code");
            if (ModelState.IsValid)
            {
                var itemCategory = db.ItemCategorys.FirstOrDefault(x => x.ItemCategoryCode == model.InventoryCategoryCode);

                var entity = new PrintingMaterialCategory
                {
                    InventoryCategoryCode = model.InventoryCategoryCode,
                    CategoryName = itemCategory.Name,
                    CostAccountCode = itemCategory.AccountCode,
                    InventoryAccountCode = itemCategory.IssueAccountCode,
                    ProductionAccountCode = model.ProductionAccountCode,
                    IsActive = model.IsActive
                };
                
                db.PrintingMaterialCategories.Add(entity);
                db.SaveChanges();
                //GenerateItemCategoryDetail(entity);
                model.PrintingMaterialCategoryId = entity.PrintingMaterialCategoryId;
                model.CategoryName = entity.CategoryName;
                model.CostAccountCode = entity.CostAccountCode;
                model.InventoryAccountCode = entity.InventoryAccountCode;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategory model)
        {
            var exsistingCategoryCode = db.PrintingMaterialCategories
                .Where(x => x.InventoryCategoryCode == model.InventoryCategoryCode
                && x.PrintingMaterialCategoryId != model.PrintingMaterialCategoryId);
            if (exsistingCategoryCode.Any())
                ModelState.AddModelError("", "Exsisting Category Code");
            if (ModelState.IsValid)
            {
                var entity = new PrintingMaterialCategory
                {
                    PrintingMaterialCategoryId = model.PrintingMaterialCategoryId,
                    InventoryCategoryCode = model.InventoryCategoryCode,
                     CategoryName = model.CategoryName,
                    CostAccountCode = model.CostAccountCode,
                    InventoryAccountCode = model.InventoryAccountCode,
                    ProductionAccountCode = model.ProductionAccountCode,
                    IsActive = model.IsActive
                };
                DeleteItems(entity.PrintingMaterialCategoryId);
                db.PrintingMaterialCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                //GenerateItemCategoryDetail(entity);

            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategory model)
        {
            if (ModelState.IsValid)
            {
                var category = db.PrintingMaterialCategories.Find(model.PrintingMaterialCategoryId);

                DeleteItems(category.PrintingMaterialCategoryId);
                db.PrintingMaterialCategories.Attach(category);
                db.PrintingMaterialCategories.Remove(category);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void DeleteItems(int id)
        {
            var items = db.PrintingMaterialCategoryItems.Where(x => x.PrintingMaterialCategoryId == id).ToList();
            db.PrintingMaterialCategoryItems.RemoveRange(items);
            db.SaveChanges();
        }
        private void GenerateItemCategoryDetail(PrintingMaterialCategory materialCategory)
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
