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
            IQueryable<ManufacturingMaterialCategoryDetail> manufacturingmaterialcategorydetails = db.ManufacturingMaterialCategoryDetails.Where(x => x.ManufacturingMaterialCategoryId == id);
            DataSourceResult result = manufacturingmaterialcategorydetails.ToDataSourceResult(request, manufacturingMaterialCategoryDetail => new ManufacturingMaterialCategoryDetail
            {
                ManufacturingMaterialCategoryDetailId = manufacturingMaterialCategoryDetail.ManufacturingMaterialCategoryDetailId,
                InventorySubCategoryCode = manufacturingMaterialCategoryDetail.InventorySubCategoryCode,
                ItemName = manufacturingMaterialCategoryDetail.ItemName,
                ItemCode = manufacturingMaterialCategoryDetail.ItemCode,
                UnitOfMeasurement = manufacturingMaterialCategoryDetail.UnitOfMeasurement,
                UnitPrice = manufacturingMaterialCategoryDetail.UnitPrice,
                IsActive = manufacturingMaterialCategoryDetail.IsActive,
               ManufacturingMaterialCategoryId = manufacturingMaterialCategoryDetail.ManufacturingMaterialCategoryId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManufacturingMaterialCategoryDetails_Create([DataSourceRequest]DataSourceRequest request, ManufacturingMaterialCategoryDetail manufacturingMaterialCategoryDetail, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ManufacturingMaterialCategoryDetail
                {
                    InventorySubCategoryCode = manufacturingMaterialCategoryDetail.InventorySubCategoryCode,
                    ItemName = manufacturingMaterialCategoryDetail.ItemName,
                    ItemCode = manufacturingMaterialCategoryDetail.ItemCode,
                    UnitOfMeasurement = manufacturingMaterialCategoryDetail.UnitOfMeasurement,
                    UnitPrice = manufacturingMaterialCategoryDetail.UnitPrice,
                    IsActive = manufacturingMaterialCategoryDetail.IsActive,
                    ManufacturingMaterialCategoryId = id
                };

                db.ManufacturingMaterialCategoryDetails.Add(entity);
                db.SaveChanges();
                manufacturingMaterialCategoryDetail.ManufacturingMaterialCategoryDetailId = entity.ManufacturingMaterialCategoryDetailId;
            }

            return Json(new[] { manufacturingMaterialCategoryDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManufacturingMaterialCategoryDetails_Update([DataSourceRequest]DataSourceRequest request, ManufacturingMaterialCategoryDetail manufacturingMaterialCategoryDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new ManufacturingMaterialCategoryDetail
                {
                    ManufacturingMaterialCategoryDetailId = manufacturingMaterialCategoryDetail.ManufacturingMaterialCategoryDetailId,
                    InventorySubCategoryCode = manufacturingMaterialCategoryDetail.InventorySubCategoryCode,
                    ItemName = manufacturingMaterialCategoryDetail.ItemName,
                    ItemCode = manufacturingMaterialCategoryDetail.ItemCode,
                    UnitOfMeasurement = manufacturingMaterialCategoryDetail.UnitOfMeasurement,
                    UnitPrice = manufacturingMaterialCategoryDetail.UnitPrice,
                    IsActive = manufacturingMaterialCategoryDetail.IsActive,
                    ManufacturingMaterialCategoryId = manufacturingMaterialCategoryDetail.ManufacturingMaterialCategoryId
                };

                db.ManufacturingMaterialCategoryDetails.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { manufacturingMaterialCategoryDetail }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ManufacturingMaterialCategoryDetails_Destroy([DataSourceRequest]DataSourceRequest request, ManufacturingMaterialCategoryDetail manufacturingMaterialCategoryDetail)
        {
            if (ModelState.IsValid)
            {
                var entity = new ManufacturingMaterialCategoryDetail
                {
                    ManufacturingMaterialCategoryDetailId = manufacturingMaterialCategoryDetail.ManufacturingMaterialCategoryDetailId,
                    InventorySubCategoryCode = manufacturingMaterialCategoryDetail.InventorySubCategoryCode,
                    ItemName = manufacturingMaterialCategoryDetail.ItemName,
                    ItemCode = manufacturingMaterialCategoryDetail.ItemCode,
                    UnitOfMeasurement = manufacturingMaterialCategoryDetail.UnitOfMeasurement,
                    UnitPrice = manufacturingMaterialCategoryDetail.UnitPrice,
                    IsActive = manufacturingMaterialCategoryDetail.IsActive,
                    ManufacturingMaterialCategoryId = manufacturingMaterialCategoryDetail.ManufacturingMaterialCategoryId
                };

                db.ManufacturingMaterialCategoryDetails.Attach(entity);
                db.ManufacturingMaterialCategoryDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { manufacturingMaterialCategoryDetail }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
