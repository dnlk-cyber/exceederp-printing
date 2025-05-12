using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Printing;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingMaterialCategoryItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PrintingMaterialCategoryItem_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PrintingMaterialCategoryItem> manufacturingmaterialcategorydetails = db.PrintingMaterialCategoryItems.Where(x => x.PrintingMaterialCategoryId == id);

            return Json(manufacturingmaterialcategorydetails.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMaterialCategoryItem_Create([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategoryItem model, int id)
        {
            var item = db.InventoryItems.FirstOrDefault(x => x.ItemCode == model.ItemCode);
            if (item == null)
                ModelState.AddModelError(string.Empty, "Inventory Item Not Found");
            var exsitingItems = db.PrintingMaterialCategoryItems
                .Where(x => x.ItemCode == model.ItemCode && x.PrintingMaterialCategoryId == id);
            if (exsitingItems.Any())
                ModelState.AddModelError("", "Item Exists");
            if (ModelState.IsValid)
            {              
                var entity = new PrintingMaterialCategoryItem
                {
                    InventorySubCategoryCode = item.SubCategoryCode,
                    ItemName = item.Name,
                    ItemCode = model.ItemCode, 
                    UnitOfMeasurement = item.UoMName,
                    UnitPrice = model.UnitPrice,
                    IsActive = model.IsActive,
                    PrintingMaterialCategoryId = id,
                    Length = model.Length,
                    Width = model.Width,
                    Weight = model.Weight,
                    PaperType = model.PaperType,
                    ConsumptionRate = model.ConsumptionRate
                };

                db.PrintingMaterialCategoryItems.Add(entity);
                db.SaveChanges();
                model.PrintingMaterialCategoryItemId = entity.PrintingMaterialCategoryItemId;
                model.ItemCode = entity.ItemCode;
                model.ItemName = entity.ItemName;
                model.UnitOfMeasurement = entity.UnitOfMeasurement;
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMaterialCategoryItem_Update([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategoryItem model)
        {
            var item = db.InventoryItems.FirstOrDefault(x => x.ItemCode == model.ItemCode);
            if (item == null)
                ModelState.AddModelError(string.Empty, "Inventory Item Not Found");
            var exsitingItems = db.PrintingMaterialCategoryItems
                .Where(x => x.ItemCode == model.ItemCode && x.PrintingMaterialCategoryId == model.PrintingMaterialCategoryId);
            //if (exsitingItems.Any())
            //    ModelState.AddModelError("", "Item Exists");
            if (ModelState.IsValid)
            {
                
                var entity = new PrintingMaterialCategoryItem
                {
                    PrintingMaterialCategoryItemId = model.PrintingMaterialCategoryItemId,
                    InventorySubCategoryCode = item.SubCategoryCode,
                    ItemName = item.Name,
                    ItemCode = item.ItemCode,
                    UnitOfMeasurement = model.UnitOfMeasurement,
                    UnitPrice = model.UnitPrice,
                    IsActive = model.IsActive,
                    PrintingMaterialCategoryId = model.PrintingMaterialCategoryId,
                    Length = model.Length,
                    Width = model.Width,
                    Weight = model.Weight,
                    PaperType = model.PaperType,
                    ConsumptionRate = model.ConsumptionRate
                };

                db.PrintingMaterialCategoryItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMaterialCategoryItem_Delete([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategoryItem model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMaterialCategoryItem
                {
                    PrintingMaterialCategoryItemId = model.PrintingMaterialCategoryItemId,
                    InventorySubCategoryCode = model.InventorySubCategoryCode,
                    ItemName = model.ItemName,
                    ItemCode = model.ItemCode,
                    UnitOfMeasurement = model.UnitOfMeasurement,
                    UnitPrice = model.UnitPrice,
                    IsActive = model.IsActive,
                    PrintingMaterialCategoryId = model.PrintingMaterialCategoryId,
                    Length = model.Length,
                    Width = model.Width,
                    Weight = model.Weight,
                    PaperType = model.PaperType,
                    ConsumptionRate = model.ConsumptionRate
                };

                db.PrintingMaterialCategoryItems.Attach(entity);
                db.PrintingMaterialCategoryItems.Remove(entity);
                db.SaveChanges();
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
