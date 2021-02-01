using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProductionOrders
{
    public class SeedProductionOrderItemController : Controller
    {
        ExceedDbContext db;
        // GET: Production/SeedProductionPlanYear
        public SeedProductionOrderItemController()
        {
            db = new ExceedDbContext();
        }
        //public ActionResult Index()
        //{
        //    var plots = db.Plots.ToList();
        //    ViewData["FarmingPlots"] = plots;

        //    var crops = db.Crops.ToList();
        //    ViewData["CropTypes"] = crops;
        //    return View();
        //}
        public ActionResult ProductionOrderItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SeedProductionOrderItem> productionItems = db.SeedProductionOrderItems.Where(itm => itm.ProductionOrderId == id);
            DataSourceResult result = productionItems.ToDataSourceResult(request, productionItem => new
            {
                SeedProductionOrderItemId = productionItem.SeedProductionOrderItemId,
                ProductionOrderId = productionItem.ProductionOrderId,
                PlotId = productionItem.PlotId,
                //DateFrom = productionItem.DateFrom,
                //DateTo = productionItem.DateTo,
                ProductionYearId = productionItem.ProductionYearId,
                Status = productionItem.Status,
                PurchaseCost = productionItem.PurchaseCost,
                ProductionCode = productionItem.ProductionCode,
                Description = productionItem.Description,
                CropId = productionItem.CropId,
                //SeedType = productionItem.SeedType,
                //Generation = productionItem.Generation,
                ProductionType = productionItem.ProductionType,

                QuantityProduced = productionItem.QuantityProduced,
                UnitOfOutPutProduced = productionItem.UnitOfOutPutProduced,

                MaterialCost = productionItem.MaterialCost,
                LaborCost = productionItem.LaborCost,
                EquipmentCost = productionItem.EquipmentCost,
                DirectCost = productionItem.DirectCost,
                OverheadCost = productionItem.OverheadCost,
                //ProfitCost = productionItem.ProfitCost,
                TotalCost = productionItem.TotalCost,

                

                OrgBranchName = productionItem.OrgBranchName,
                DateCreated = productionItem.DateCreated,
                LastModified = productionItem.LastModified,
                CreatedBy = productionItem.CreatedBy,
                ModifiedBy = productionItem.ModifiedBy,
            });

            return Json(result);
        }

        public ActionResult ProductionOrderPurchasePrice_Read([DataSourceRequest]DataSourceRequest request, int itemId)
        {
            var currentProductionOrderItem = db.SeedProductionOrderItems.Find(itemId);
            List<SeedPurchasePriceViewModel> list = new List<SeedPurchasePriceViewModel>();
            if (currentProductionOrderItem != null)
            {
                #region add components
                var record = new SeedPurchasePriceViewModel
                {
                    Id = currentProductionOrderItem.SeedProductionOrderItemId,
                    PurchasedQuantity = currentProductionOrderItem.QuantityProduced,
                    PurchaseCost = currentProductionOrderItem.PurchaseCost,
                    TotalCost = currentProductionOrderItem.TotalCost
                };
                
                #endregion
                list.Add(record);
                
            }
            return Json(list.AsQueryable().ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionOrderPurchasePrice_Update([DataSourceRequest]DataSourceRequest request, SeedPurchasePriceViewModel item)
        {
            var currentProductionOrderItem = db.SeedProductionOrderItems.Find(item.Id);
            List<SeedPurchasePriceViewModel> list = new List<SeedPurchasePriceViewModel>();
            if (currentProductionOrderItem != null)
            {

                currentProductionOrderItem.QuantityProduced = item.PurchasedQuantity;
                currentProductionOrderItem.PurchaseCost = item.PurchaseCost;

                db.SeedProductionOrderItems.Attach(currentProductionOrderItem);
                db.Entry(currentProductionOrderItem).State = EntityState.Modified;
                db.SaveChanges();
                ProductionOrderHelper.UpdateProductionOrderInCostItemChange(currentProductionOrderItem.SeedProductionOrderItemId, ProductType.Seed);

                item.TotalCost = currentProductionOrderItem.TotalCost;
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        private bool PeriodExists(int projectId, SeedProductionPlanYear model, ControllerFunctionType action)
        {
            var valueExists = false;
            if (action == ControllerFunctionType.Create)
            {
                var temp = db.SeedProductionPlanYear
                    .FirstOrDefault(y => y.ProductionProjectId == projectId &&
                                    y.GlFiscalYearId == model.GlFiscalYearId);
                if (temp != null)
                {
                    valueExists = true;
                }
            }
            else if (action == ControllerFunctionType.Update)
            {
                var temp = db.SeedProductionPlanYear
                    .FirstOrDefault(y => y.ProductionProjectId == projectId &&
                                    y.GlFiscalYearId == model.GlFiscalYearId &&
                                    y.SeedProductionPlanYearId != model.SeedProductionPlanYearId);
                if (temp != null)
                {
                    valueExists = true;
                }
            }

            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionOrderItem_Create([DataSourceRequest]DataSourceRequest request, SeedProductionOrderItem productionItem, int id)
        {
            if (ModelState.IsValid)
            {
                //if (PeriodExists(id, productionItem, ControllerFunctionType.Create))
                //{
                //    ModelState.AddModelError("", "This Year is Registered already");
                //}
                //else
                //{
                var entity = new SeedProductionOrderItem
                {
                    ProductionOrderId = id,
                    PlotId = productionItem.PlotId,
                    //DateFrom = productionItem.DateFrom,
                    //DateTo = productionItem.DateTo,
                    ProductionYearId = productionItem.ProductionYearId,
                    Status = productionItem.Status,
                    PurchaseCost = productionItem.PurchaseCost,
                    ProductionCode = productionItem.ProductionCode,
                    Description = productionItem.Description,
                    CropId = productionItem.CropId,
                    //SeedType = productionItem.SeedType,
                    //Generation = productionItem.Generation,
                    ProductionType = productionItem.ProductionType,

                    MaterialCost = productionItem.MaterialCost,
                    LaborCost = productionItem.LaborCost,
                    EquipmentCost = productionItem.EquipmentCost,
                    DirectCost = productionItem.DirectCost,
                    OverheadCost = productionItem.OverheadCost,
                    //ProfitCost = productionItem.ProfitCost,
                    TotalCost = productionItem.TotalCost,

                    QuantityProduced = productionItem.QuantityProduced,
                    UnitOfOutPutProduced = ProductionSettingHelper.GetProductionItemUnitOfMeasurnment(productionItem.CropId),

                    OrgBranchName = productionItem.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = productionItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = productionItem.ModifiedBy,

                };

                db.SeedProductionOrderItems.Add(entity);
                db.SaveChanges();

                productionItem.SeedProductionOrderItemId = entity.SeedProductionOrderItemId;

                //}
                this.UpdateProductionOrderItemCode(entity.SeedProductionOrderItemId);

            }

            return Json(new[] { productionItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionOrderItem_Update([DataSourceRequest]DataSourceRequest request, SeedProductionOrderItem productionItem)
        {
            if (ModelState.IsValid)
            {
                //if (PeriodExists(productionItem.ProductionProjectId, productionItem, ControllerFunctionType.Update))
                //{
                //    ModelState.AddModelError("", "This Year is Registered already");
                //}
                //else
                //{
                var entity = new SeedProductionOrderItem
                {
                    SeedProductionOrderItemId = productionItem.SeedProductionOrderItemId,
                    ProductionOrderId = productionItem.ProductionOrderId,
                    PlotId = productionItem.PlotId,
                    //DateFrom = productionItem.DateFrom,
                    //DateTo = productionItem.DateTo,
                    ProductionYearId = productionItem.ProductionYearId,
                    Status = productionItem.Status,
                    PurchaseCost = productionItem.PurchaseCost,
                    ProductionCode = productionItem.ProductionCode,
                    Description = productionItem.Description,
                    CropId = productionItem.CropId,
                    //SeedType = productionItem.SeedType,
                    //Generation = productionItem.Generation,
                    ProductionType = productionItem.ProductionType,

                    MaterialCost = productionItem.MaterialCost,
                    LaborCost  = productionItem.LaborCost,
                    EquipmentCost = productionItem.EquipmentCost,
                    DirectCost = productionItem.DirectCost,
                    OverheadCost = productionItem.OverheadCost,
                   // ProfitCost = productionItem.ProfitCost,
                    TotalCost = productionItem.TotalCost,

                    QuantityProduced = productionItem.QuantityProduced,
                    UnitOfOutPutProduced = ProductionSettingHelper.GetProductionItemUnitOfMeasurnment(productionItem.CropId),

                    OrgBranchName = productionItem.OrgBranchName,
                    DateCreated = productionItem.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = productionItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.SeedProductionOrderItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                this.UpdateProductionOrderItemCode(entity.SeedProductionOrderItemId);

            }


            //}

            return Json(new[] { productionItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionOrderItem_Destroy([DataSourceRequest]DataSourceRequest request, SeedProductionOrderItem productionItem)
        {
            if (ModelState.IsValid)
            {


                var entity = new SeedProductionOrderItem
                {
                    SeedProductionOrderItemId = productionItem.SeedProductionOrderItemId,
                    ProductionOrderId = productionItem.ProductionOrderId,
                    PlotId = productionItem.PlotId,
                    //DateFrom = productionItem.DateFrom,
                    //DateTo = productionItem.DateTo,
                    ProductionYearId = productionItem.ProductionYearId,
                    Status = productionItem.Status,
                    PurchaseCost = productionItem.PurchaseCost,
                    ProductionCode = productionItem.ProductionCode,
                    Description = productionItem.Description,
                    CropId = productionItem.CropId,
                    //SeedType = productionItem.SeedType,
                    //Generation = productionItem.Generation,
                    ProductionType = productionItem.ProductionType,

                    MaterialCost = productionItem.MaterialCost,
                    LaborCost = productionItem.LaborCost,
                    EquipmentCost = productionItem.EquipmentCost,
                    DirectCost = productionItem.DirectCost,
                    OverheadCost = productionItem.OverheadCost,
                    //ProfitCost = productionItem.ProfitCost,
                    TotalCost = productionItem.TotalCost,

                    QuantityProduced = productionItem.QuantityProduced,
                    UnitOfOutPutProduced = productionItem.UnitOfOutPutProduced,

                    OrgBranchName = productionItem.OrgBranchName,
                    DateCreated = productionItem.DateCreated,
                    LastModified = productionItem.LastModified,
                    CreatedBy = productionItem.CreatedBy,
                    ModifiedBy = productionItem.ModifiedBy,

                };

                db.SeedProductionOrderItems.Attach(entity);
                db.SeedProductionOrderItems.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { productionItem }.ToDataSourceResult(request, ModelState));
        }

        private void UpdateProductionOrderItemCode(int id) {
            var currentItem = db.SeedProductionOrderItems.Find(id);
            if(currentItem != null)
            {
                currentItem.ProductionCode = currentItem.SeedProductionOrderItemId.ToString("D12");

                db.SeedProductionOrderItems.Attach(currentItem);
                db.Entry(currentItem).State = EntityState.Modified;
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