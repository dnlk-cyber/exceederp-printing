
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    //[AuthorizeRoles(PrintingEstimationRoles.PrintingEstimationFormUser, PrintingEstimationRoles.PrintingEstimationAdmin)]
    public class FurnitureMaterialCostsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();



        public ActionResult FurnitureMaterialCosts_Read([DataSourceRequest]DataSourceRequest request, int estimationFormId)

        {
            IQueryable<FurnitureMaterialCost> materialcosts = db.FurnitureMaterialCosts.Where(q => q.FurnitureEstimationId == estimationFormId).OrderByDescending(a => a.FurnitureMaterialCostId);
            DataSourceResult result = materialcosts.ToDataSourceResult(request, materialCost => new FurnitureMaterialCost
            {
                FurnitureMaterialCostId = materialCost.FurnitureMaterialCostId,
                FurnitureEstimationId = materialCost.FurnitureEstimationId,
                InventoryCategoryId = materialCost.InventoryCategoryId,
                InventoryId = materialCost.InventoryId,
                StoreCode = materialCost.StoreCode,
                Quantity = materialCost.Quantity,
                UnitPrice = materialCost.UnitPrice,
                TotalPrice = materialCost.TotalPrice,
                DateCreated = materialCost.DateCreated,
                LastModified = materialCost.LastModified,
                UnitOfMeasurment = materialCost.UnitOfMeasurment,
                ManufacturingMaterialCategoryId = materialCost.ManufacturingMaterialCategoryId,
                TaskType = materialCost.TaskType,
                ManufacturingMaterialCategoryItemId = materialCost.ManufacturingMaterialCategoryItemId,
                
            });

            return Json(result);
        }
        public ActionResult MaterialJobOrders_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            //var estId = (int)db.Proformas.Find(db.Jobs.Find(id)?.ProformaId)?.EstimationFormId;
            IQueryable<FurnitureMaterialCost> materialcosts = db.FurnitureMaterialCosts.Where(q => q.JobId == id);
            DataSourceResult result = materialcosts.ToDataSourceResult(request, materialCost => new
            {
                FurnitureMaterialCostId = materialCost.FurnitureMaterialCostId,
                FurnitureEstimationId = materialCost.FurnitureEstimationId,
                InventoryCategoryId = materialCost.InventoryCategoryId,
                InventoryId = materialCost.InventoryId,
                Quantity = materialCost.Quantity,
                StoreCode = materialCost.StoreCode,
                UnitPrice = materialCost.UnitPrice,
                TotalPrice = materialCost.TotalPrice,
                DateCreated = materialCost.DateCreated,
                LastModified = materialCost.LastModified,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureMaterialCosts_Create([DataSourceRequest]DataSourceRequest request, FurnitureMaterialCost materialCost, int parentId)
        {

            if (ModelState.IsValid)
            {
                var invs = db.FurnitureMaterialCosts.Where(q => q.InventoryId == materialCost.InventoryId && q.JobId == parentId).ToList();
                if (invs.Any())
                {
                    ModelState.AddModelError("403", "Inventory already available");
                }

                //var materialCategory = db.ManufacturingMaterialCategories.FirstOrDefault(x => x.ManufacturingMaterialCategoryId == materialCost.ManufacturingMaterialCategoryId);
                var materialCategoryDetails = db.ManufacturingMaterialCategoryItems.Where(x => x.ManufacturingMaterialCategoryId == materialCost.ManufacturingMaterialCategoryId).ToList();
                foreach (var item in materialCategoryDetails)
                {
                    var entity = new FurnitureMaterialCost
                    {
                        
                        FurnitureEstimationId = parentId,
                        ManufacturingMaterialCategoryId = materialCost.ManufacturingMaterialCategoryId,
                        ManufacturingMaterialCategoryItemId = materialCost.ManufacturingMaterialCategoryItemId,
                        Quantity = materialCost.Quantity,                        
                        UnitPrice = item.UnitPrice,                       
                        UnitOfMeasurment = item.UnitOfMeasurement,
                        
                    };
                    entity.TotalPrice = entity.UnitPrice * entity.Quantity;
                    db.FurnitureMaterialCosts.Add(entity);
                    db.SaveChanges();

                }
               
                
                //var estimation = new UpdateFurnitureOverallCost();
                //estimation.update(parentId);

            }


            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult JobMaterialCosts_Create([DataSourceRequest]DataSourceRequest request, FurnitureMaterialCost materialCost, int id)
        {

            if (ModelState.IsValid)
            {
                int estId = 0;
                var job = db.Jobs.Find(id);
                if (job != null)
                {
                    var performa = db.Proformas.Find(job.ProformaId);
                    if (performa != null)
                    {
                        estId = (int)performa.EstimationFormId;
                        // var estId = (int)db.Proformas.Find(db.Jobs.Find(id)?.ProformaId)?.EstimationFormId;
                        var invs = db.MaterialCosts.Where(q => q.InventoryId == materialCost.InventoryId && q.EstimationFormId == performa.EstimationFormId).ToList();
                        if (invs.Any())
                        {
                            ModelState.AddModelError("403", "Inventory already available");
                        }
                    }
                }

                materialCost.TotalPrice = this.totalPrice(materialCost.UnitPrice, materialCost.Quantity);
                var entity = new FurnitureMaterialCost
                {
                    //MaterialCostId = materialCost.MaterialCostId,
                    JobId = id,
                    FurnitureEstimationId = estId,
                    InventoryCategoryId = materialCost.InventoryCategoryId,
                    InventoryId = materialCost.InventoryId,
                    Quantity = materialCost.Quantity,
                    StoreCode = materialCost.StoreCode,
                    UnitPrice = materialCost.UnitPrice,
                    TotalPrice = materialCost.TotalPrice
                };

                db.FurnitureMaterialCosts.Add(entity);
                db.SaveChanges();
                //var estimation = new FurnitureUpdateOverallCost();
                //estimation.update(estId);

            }


            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }

        public decimal totalPrice(decimal UP, decimal Q)
        {
            return UP * Q;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureMaterialCosts_Update([DataSourceRequest]DataSourceRequest request, FurnitureMaterialCost materialCost)
        {
            if (ModelState.IsValid)
            {
                materialCost.TotalPrice = materialCost.Quantity * materialCost.UnitPrice;

                var entity = new FurnitureMaterialCost
                {
                    FurnitureMaterialCostId = materialCost.FurnitureMaterialCostId,
                    FurnitureEstimationId = materialCost.FurnitureEstimationId,
                    InventoryCategoryId = materialCost.InventoryCategoryId,
                    InventoryId = materialCost.InventoryId,
                    StoreCode = materialCost.StoreCode,
                    Quantity = materialCost.Quantity,
                    UnitPrice = materialCost.UnitPrice,
                    TotalPrice = materialCost.TotalPrice,
                    UnitOfMeasurment = materialCost.UnitOfMeasurment,
                    ManufacturingMaterialCategoryId = materialCost.ManufacturingMaterialCategoryId,
                    TaskType = materialCost.TaskType,
                    ManufacturingMaterialCategoryItemId = materialCost.ManufacturingMaterialCategoryItemId,
                };

                db.FurnitureMaterialCosts.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FurnitureMaterialCosts_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureMaterialCost materialCost)
        {

            var furnitureMaterialCost = db.FurnitureMaterialCosts.Find(materialCost.FurnitureMaterialCostId);
            db.FurnitureMaterialCosts.Remove(furnitureMaterialCost);
            db.SaveChanges();

            return Json(new[] { materialCost }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
