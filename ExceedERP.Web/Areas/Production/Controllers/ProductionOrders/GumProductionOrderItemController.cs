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
    public class GumProductionOrderItemController : Controller
    {
        ExceedDbContext db;
        public GumProductionOrderItemController()
        {
            db = new ExceedDbContext();
        }
        
        public ActionResult ProductionOrderItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<GumProductionOrderItem> productionItems = db.GumProductionOrderItems.Where(itm => itm.ProductionOrderId == id);
            DataSourceResult result = productionItems.ToDataSourceResult(request, productionItem => new
            {
                GumProductionOrderItemId = productionItem.GumProductionOrderItemId,
                ProductionOrderId = productionItem.ProductionOrderId,
                //ProductType = productionItem.ProductType,
                ProductionType = productionItem.ProductionType,
                ProductionCode = productionItem.ProductionCode,
                NaturalGumId = productionItem.NaturalGumId,

                ProductionYearId = productionItem.ProductionYearId,
                Status = productionItem.Status,
                PurchaseCost = productionItem.PurchaseCost,

                QuantityProduced = productionItem.QuantityProduced,
                UnitOfOutPutProduced = productionItem.UnitOfOutPutProduced,

                MaterialCost = productionItem.MaterialCost,
                LaborCost = productionItem.LaborCost,
                EquipmentCost = productionItem.EquipmentCost,
                DirectCost = productionItem.DirectCost,
                OverheadCost = productionItem.OverheadCost,
                //ProfitCost = productionItem.ProfitCost,
                TotalCost = productionItem.TotalCost,

                //PlotId = productionItem.PlotId,
                //DateFrom = productionItem.DateFrom,
                //DateTo = productionItem.DateTo,
                //ProductionCode = productionItem.ProductionCode,
                //Description = productionItem.Description,
                //CropId = productionItem.CropId,
                //SeedType = productionItem.SeedType,
                //Generation = productionItem.Generation,

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
            var currentProductionOrderItem = db.GumProductionOrderItems.Find(itemId);
            List<GumPurchasePriceViewModel> list = new List<GumPurchasePriceViewModel>();
            if (currentProductionOrderItem != null)
            {
                #region add components
                var record = new GumPurchasePriceViewModel
                {
                    Id = currentProductionOrderItem.GumProductionOrderItemId,
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
        public ActionResult ProductionOrderPurchasePrice_Update([DataSourceRequest]DataSourceRequest request, GumPurchasePriceViewModel item)
        {
            var currentProductionOrderItem = db.GumProductionOrderItems.Find(item.Id);
            List<GumPurchasePriceViewModel> list = new List<GumPurchasePriceViewModel>();
            if (currentProductionOrderItem != null)
            {

                currentProductionOrderItem.QuantityProduced = item.PurchasedQuantity;
                currentProductionOrderItem.PurchaseCost = item.PurchaseCost;

                db.GumProductionOrderItems.Attach(currentProductionOrderItem);
                db.Entry(currentProductionOrderItem).State = EntityState.Modified;
                db.SaveChanges();
                ProductionOrderHelper.UpdateProductionOrderInCostItemChange(currentProductionOrderItem.GumProductionOrderItemId, ProductType.NaturalGum);

                item.TotalCost = currentProductionOrderItem.TotalCost;
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionOrderItem_Create([DataSourceRequest]DataSourceRequest request, GumProductionOrderItem productionItem, int id)
        {
            if (ModelState.IsValid)
            {
                //if (PeriodExists(id, productionItem, ControllerFunctionType.Create))
                //{
                //    ModelState.AddModelError("", "This Year is Registered already");
                //}
                //else
                //{
                var entity = new GumProductionOrderItem
                {
                    ProductionOrderId = id,
                    //ProductType = productionItem.ProductType,
                    ProductionType = productionItem.ProductionType,
                    ProductionCode = productionItem.ProductionCode,
                    NaturalGumId = productionItem.NaturalGumId,

                    ProductionYearId = productionItem.ProductionYearId,
                    Status = productionItem.Status,
                    PurchaseCost = productionItem.PurchaseCost,

                    QuantityProduced = productionItem.QuantityProduced,
                    UnitOfOutPutProduced = ProductionSettingHelper.GetGumProductionItemUnitOfMeasurnment(productionItem.NaturalGumId),

                    MaterialCost = productionItem.MaterialCost,
                    LaborCost = productionItem.LaborCost,
                    EquipmentCost = productionItem.EquipmentCost,
                    DirectCost = productionItem.DirectCost,
                    OverheadCost = productionItem.OverheadCost,
                    //ProfitCost = productionItem.ProfitCost,
                    TotalCost = productionItem.TotalCost,

                    //PlotId = productionItem.PlotId,
                    //DateFrom = productionItem.DateFrom,
                    //DateTo = productionItem.DateTo,
                    //ProductionCode = productionItem.ProductionCode,
                    //Description = productionItem.Description,
                    //CropId = productionItem.CropId,
                    //SeedType = productionItem.SeedType,
                    //Generation = productionItem.Generation,

                    OrgBranchName = productionItem.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = productionItem.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = productionItem.ModifiedBy,

                };

                db.GumProductionOrderItems.Add(entity);
                db.SaveChanges();

                productionItem.GumProductionOrderItemId = entity.GumProductionOrderItemId;

                //}

                this.UpdteProductionItemCode(entity.GumProductionOrderItemId);
            }

            return Json(new[] { productionItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionOrderItem_Update([DataSourceRequest]DataSourceRequest request, GumProductionOrderItem productionItem)
        {
            if (ModelState.IsValid)
            {
                //if (PeriodExists(productionItem.ProductionProjectId, productionItem, ControllerFunctionType.Update))
                //{
                //    ModelState.AddModelError("", "This Year is Registered already");
                //}
                //else
                //{
                var entity = new GumProductionOrderItem
                {
                    GumProductionOrderItemId = productionItem.GumProductionOrderItemId,
                    ProductionOrderId = productionItem.ProductionOrderId,
                    //ProductType = productionItem.ProductType,
                    ProductionType = productionItem.ProductionType,
                    ProductionCode = productionItem.ProductionCode,
                    NaturalGumId = productionItem.NaturalGumId,

                    ProductionYearId = productionItem.ProductionYearId,
                    Status = productionItem.Status,
                    PurchaseCost = productionItem.PurchaseCost,

                    MaterialCost = productionItem.MaterialCost,
                    LaborCost = productionItem.LaborCost,
                    EquipmentCost = productionItem.EquipmentCost,
                    DirectCost = productionItem.DirectCost,
                    OverheadCost = productionItem.OverheadCost,
                    //ProfitCost = productionItem.ProfitCost,
                    TotalCost = productionItem.TotalCost,

                    QuantityProduced = productionItem.QuantityProduced,
                    UnitOfOutPutProduced = ProductionSettingHelper.GetGumProductionItemUnitOfMeasurnment(productionItem.NaturalGumId),

                    //PlotId = productionItem.PlotId,
                    //DateFrom = productionItem.DateFrom,
                    //DateTo = productionItem.DateTo,
                    //ProductionCode = productionItem.ProductionCode,
                    //Description = productionItem.Description,
                    //CropId = productionItem.CropId,
                    //SeedType = productionItem.SeedType,
                    //Generation = productionItem.Generation,

                    OrgBranchName = productionItem.OrgBranchName,
                    DateCreated = productionItem.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = productionItem.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.GumProductionOrderItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                this.UpdteProductionItemCode(entity.GumProductionOrderItemId);

            }


            //}

            return Json(new[] { productionItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProductionOrderItem_Destroy([DataSourceRequest]DataSourceRequest request, GumProductionOrderItem productionItem)
        {
            if (ModelState.IsValid)
            {


                var entity = new GumProductionOrderItem
                {
                    GumProductionOrderItemId = productionItem.GumProductionOrderItemId,
                    ProductionOrderId = productionItem.ProductionOrderId,
                    //ProductType = productionItem.ProductType,
                    ProductionType = productionItem.ProductionType,
                    ProductionCode = productionItem.ProductionCode,
                    NaturalGumId = productionItem.NaturalGumId,

                    ProductionYearId = productionItem.ProductionYearId,
                    Status = productionItem.Status,
                    PurchaseCost = productionItem.PurchaseCost,

                    MaterialCost = productionItem.MaterialCost,
                    LaborCost = productionItem.LaborCost,
                    EquipmentCost = productionItem.EquipmentCost,
                    DirectCost = productionItem.DirectCost,
                    OverheadCost = productionItem.OverheadCost,
                    //ProfitCost = productionItem.ProfitCost,
                    TotalCost = productionItem.TotalCost,

                    QuantityProduced = productionItem.QuantityProduced,
                    UnitOfOutPutProduced = productionItem.UnitOfOutPutProduced,

                    //PlotId = productionItem.PlotId,
                    //DateFrom = productionItem.DateFrom,
                    //DateTo = productionItem.DateTo,
                    //ProductionCode = productionItem.ProductionCode,
                    //Description = productionItem.Description,
                    //CropId = productionItem.CropId,
                    //SeedType = productionItem.SeedType,
                    //Generation = productionItem.Generation,

                    OrgBranchName = productionItem.OrgBranchName,
                    DateCreated = productionItem.DateCreated,
                    LastModified = productionItem.LastModified,
                    CreatedBy = productionItem.CreatedBy,
                    ModifiedBy = productionItem.ModifiedBy,

                };

                db.GumProductionOrderItems.Attach(entity);
                db.GumProductionOrderItems.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { productionItem }.ToDataSourceResult(request, ModelState));
        }

        private void UpdteProductionItemCode(int id) {
            var currentItem = db.GumProductionOrderItems.Find(id);
            if(currentItem != null)
            {
                currentItem.ProductionCode = currentItem.GumProductionOrderItemId.ToString("D12");

                db.GumProductionOrderItems.Attach(currentItem);
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