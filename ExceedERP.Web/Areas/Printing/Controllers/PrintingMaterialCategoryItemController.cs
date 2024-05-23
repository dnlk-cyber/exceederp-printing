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

            DataSourceResult result = manufacturingmaterialcategorydetails.ToDataSourceResult(request, PrintingMaterialCategoryItem => new PrintingMaterialCategoryItem
            {
                PrintingMaterialCategoryItemId = PrintingMaterialCategoryItem.PrintingMaterialCategoryItemId,
                InventorySubCategoryCode = PrintingMaterialCategoryItem.InventorySubCategoryCode,
                ItemName = PrintingMaterialCategoryItem.ItemName,
                ItemCode = PrintingMaterialCategoryItem.ItemCode,
                UnitOfMeasurement = PrintingMaterialCategoryItem.UnitOfMeasurement,
                UnitPrice = PrintingMaterialCategoryItem.UnitPrice,
                IsActive = PrintingMaterialCategoryItem.IsActive,
                PrintingMaterialCategoryId = PrintingMaterialCategoryItem.PrintingMaterialCategoryId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMaterialCategoryItem_Create([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategoryItem PrintingMaterialCategoryItem, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMaterialCategoryItem
                {
                    InventorySubCategoryCode = PrintingMaterialCategoryItem.InventorySubCategoryCode,
                    ItemName = PrintingMaterialCategoryItem.ItemName,
                    ItemCode = PrintingMaterialCategoryItem.ItemCode,
                    UnitOfMeasurement = PrintingMaterialCategoryItem.UnitOfMeasurement,
                    UnitPrice = PrintingMaterialCategoryItem.UnitPrice,
                    IsActive = PrintingMaterialCategoryItem.IsActive,
                    PrintingMaterialCategoryId = id
                };

                db.PrintingMaterialCategoryItems.Add(entity);
                db.SaveChanges();
                PrintingMaterialCategoryItem.PrintingMaterialCategoryItemId = entity.PrintingMaterialCategoryItemId;
            }

            return Json(new[] { PrintingMaterialCategoryItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PrintingMaterialCategoryItem_Update([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategoryItem PrintingMaterialCategoryItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingMaterialCategoryItem
                {
                    PrintingMaterialCategoryItemId = PrintingMaterialCategoryItem.PrintingMaterialCategoryItemId,
                    InventorySubCategoryCode = PrintingMaterialCategoryItem.InventorySubCategoryCode,
                    ItemName = PrintingMaterialCategoryItem.ItemName,
                    ItemCode = PrintingMaterialCategoryItem.ItemCode,
                    UnitOfMeasurement = PrintingMaterialCategoryItem.UnitOfMeasurement,
                    UnitPrice = PrintingMaterialCategoryItem.UnitPrice,
                    IsActive = PrintingMaterialCategoryItem.IsActive,
                    PrintingMaterialCategoryId = PrintingMaterialCategoryItem.PrintingMaterialCategoryId
                };

                db.PrintingMaterialCategoryItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { PrintingMaterialCategoryItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)] 
        public ActionResult PrintingMaterialCategoryItem_Delete([DataSourceRequest]DataSourceRequest request, PrintingMaterialCategoryItem PrintingMaterialCategoryItem)
        { 
            if (ModelState.IsValid)
            {
                var entity = new PrintingMaterialCategoryItem
                {
                    PrintingMaterialCategoryItemId = PrintingMaterialCategoryItem.PrintingMaterialCategoryItemId,
                    InventorySubCategoryCode = PrintingMaterialCategoryItem.InventorySubCategoryCode,
                    ItemName = PrintingMaterialCategoryItem.ItemName,
                    ItemCode = PrintingMaterialCategoryItem.ItemCode,
                    UnitOfMeasurement = PrintingMaterialCategoryItem.UnitOfMeasurement,
                    UnitPrice = PrintingMaterialCategoryItem.UnitPrice,
                    IsActive = PrintingMaterialCategoryItem.IsActive,
                    PrintingMaterialCategoryId = PrintingMaterialCategoryItem.PrintingMaterialCategoryId
                };

                db.PrintingMaterialCategoryItems.Attach(entity);
                db.PrintingMaterialCategoryItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { PrintingMaterialCategoryItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
