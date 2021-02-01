using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.Processing
{
    public class SeedProcessingOrderItemController : Controller
    {
        ExceedDbContext db;

        public SeedProcessingOrderItemController()
        {
            db = new ExceedDbContext();
        }
        public ActionResult Index()
        {
            //var stages = db.ProductionSettings.Where(s => s.Type == ProductionSettingType.SeedProductionStage || s.Type == ProductionSettingType.NaturalGumProductionStage).ToList();
            //ViewData["LandStatus"] = stages;

            ViewData["Branches"] = db.Branch.ToList();

            return View();
        }

        public ActionResult ProcessingItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<SeedProcessingOrderItem> items = db.SeedProcessingOrderItems.Where(itm => itm.SeedProcessingOrderId == id);
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                //ProductionIssueId = item.ProductionIssueId,
                ItemCategoryCode = item.ItemCategoryCode,
                ItemCode = item.ItemCode,
                TotalProductionCost = item.TotalProductionCost,
                SeedProcessingOrderId = item.SeedProcessingOrderId,

                RawQuantity = item.RawQuantity,
                RecoveryRate = item.RecoveryRate,
                ProcessingCode = item.ProcessingCode,

                MaterialCost = item.MaterialCost,
                LaborCost = item.LaborCost,
                EquipmentCost = item.EquipmentCost,
                DirectCost = item.DirectCost,
                OverheadCost = item.OverheadCost,
                //ProfitCost = item.ProfitCost,
                TotalCost = item.TotalCost,

                QuantityProduced = item.QuantityProduced,
                UnitOfOutPutProduced = InventoryHelper.GetUnitOfMeasurment(item.ItemCode),

                OrgBranchName = item.OrgBranchName,
                DateCreated = item.DateCreated,
                LastModified = item.LastModified,
                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy,


            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessingItem_Create([DataSourceRequest]DataSourceRequest request, SeedProcessingOrderItem item, int id)
        {
            if (ModelState.IsValid)
            {
                //var avgPrice = ProductionStoreHelper.GetAveragePriceOfRawProductInStore(item.ItemCode, item.StoreCode);
                var entity = new SeedProcessingOrderItem
                {
                    SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                    //ProductionIssueId = item.ProductionIssueId,
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,
                    //TotalProductionCost = avgPrice * item.RawQuantity,
                    SeedProcessingOrderId = id,

                    RawQuantity = item.RawQuantity,
                    RecoveryRate = item.RecoveryRate,
                    ProcessingCode = item.ProcessingCode,

                    MaterialCost = item.MaterialCost,
                    LaborCost = item.LaborCost,
                    EquipmentCost = item.EquipmentCost,
                    DirectCost = item.DirectCost,
                    OverheadCost = item.OverheadCost,
                    //ProfitCost = item.ProfitCost,
                    TotalCost = item.TotalCost,

                    QuantityProduced = item.QuantityProduced,
                    UnitOfOutPutProduced =item.UnitOfOutPutProduced,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = item.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = item.ModifiedBy,
                };
                entity.QuantityProduced = entity.RawQuantity * entity.RecoveryRate / 100;

                db.SeedProcessingOrderItems.Add(entity);
                db.SaveChanges();
                item.SeedProcessingOrderItemId = entity.SeedProcessingOrderItemId;

                UpdateProcessingOrderItemCode(entity.SeedProcessingOrderItemId);
                //UpdateProductionOrderItemStatusOnProcessingOrderCreate(entity.SeedProductionOrderItemId);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessingItem_Update([DataSourceRequest]DataSourceRequest request, SeedProcessingOrderItem item)
        {
            if (ModelState.IsValid)
            {
                //var avgPrice = ProductionStoreHelper.GetAveragePriceOfRawProductInStore(item.ItemCode, item.StoreCode);
                var entity = new SeedProcessingOrderItem
                {
                    SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                    //ProductionIssueId = item.ProductionIssueId,
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,
                    TotalProductionCost = item.TotalProductionCost,
                    SeedProcessingOrderId = item.SeedProcessingOrderId,
                    
                    RawQuantity = item.RawQuantity,
                    RecoveryRate = item.RecoveryRate,
                    ProcessingCode = item.ProcessingCode,

                    MaterialCost = item.MaterialCost,
                    LaborCost = item.LaborCost,
                    EquipmentCost = item.EquipmentCost,
                    DirectCost = item.DirectCost,
                    OverheadCost = item.OverheadCost,
                    //ProfitCost = item.ProfitCost,
                    TotalCost = item.TotalCost,

                    QuantityProduced = item.QuantityProduced,
                    UnitOfOutPutProduced = item.UnitOfOutPutProduced,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };
                entity.QuantityProduced = entity.RawQuantity * entity.RecoveryRate / 100;

                db.SeedProcessingOrderItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessingItem_Destroy([DataSourceRequest]DataSourceRequest request, SeedProcessingOrderItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new SeedProcessingOrderItem
                {
                    SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                    //ProductionIssueId = item.ProductionIssueId,
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,
                    TotalProductionCost = item.TotalProductionCost,
                    SeedProcessingOrderId = item.SeedProcessingOrderId,

                    RawQuantity = item.RawQuantity,
                    RecoveryRate = item.RecoveryRate,
                    ProcessingCode = item.ProcessingCode,

                    MaterialCost = item.MaterialCost,
                    LaborCost = item.LaborCost,
                    EquipmentCost = item.EquipmentCost,
                    DirectCost = item.DirectCost,
                    OverheadCost = item.OverheadCost,
                    //ProfitCost = item.ProfitCost,
                    TotalCost = item.TotalCost,

                    QuantityProduced = item.QuantityProduced,
                    UnitOfOutPutProduced = item.UnitOfOutPutProduced,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = item.LastModified,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy,
                };

                db.SeedProcessingOrderItems.Attach(entity);
                db.SeedProcessingOrderItems.Remove(entity);
                db.SaveChanges();
                this.RemoveMappedIssueItem(entity);
                //UpdateProductionOrderItemStatusOnProcessingOrderDelete(entity.SeedProductionOrderItemId);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
       
        private void RemoveMappedIssueItem(SeedProcessingOrderItem item)
        {
            var mappings = 
                db.IssueAndProcessingOrderItemMappings
                .Where(m => m.Type == ProductType.Seed && m.ProcessingOrderItemId == item.SeedProcessingOrderItemId).ToList();
            if (mappings.Any())
            {
                db.IssueAndProcessingOrderItemMappings.RemoveRange(mappings);
                db.SaveChanges();
            }
        }
        //private decimal GetRawQuantityFromProduction(int id) {
        //    var qt = new decimal();
        //    var record = db.SeedProductionOrderItems.Find(id);
        //    if(record != null)
        //    {
        //        qt = record.QuantityProduced;
        //    }
        //    return qt;
        //}
        //private string GetSeedName(int id)
        //{
        //    var name = "";
        //    var temp = db.SeedProductionOrderItems.Find(id);
        //    if (temp != null)
        //    {
        //        var crop = db.Crops.Find(temp.CropId);
        //        if (crop != null)
        //        {
        //            name = crop.Name;
        //        }
        //    }
        //    return name;
        //}
        //private string GetUnitFromProduction(int id)
        //{
        //    var name = "";
        //    var item = db.SeedProductionOrderItems.Find(id);
        //    if(item != null)
        //    {
        //        name = item.UnitOfOutPutProduced;
        //    }
        //    return name;
        //}
        private void UpdateProcessingOrderItemCode(int id)
        {
            var currentItem = db.SeedProcessingOrderItems.Find(id);
            if (currentItem != null)
            {
                currentItem.ProcessingCode = currentItem.SeedProcessingOrderItemId.ToString("D12");

                db.SeedProcessingOrderItems.Attach(currentItem);
                db.Entry(currentItem).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        //private void UpdateProductionOrderItemStatusOnProcessingOrderCreate(int id)
        //{
        //    var orderItem = db.SeedProductionOrderItems.Find(id);
        //    if(orderItem != null)
        //    {
        //        orderItem.Status = ProductStatus.Processing;

        //        db.Entry(orderItem).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //}
        //private void UpdateProductionOrderItemStatusOnProcessingOrderDelete(int id)
        //{
        //    var orderItem = db.SeedProductionOrderItems.Find(id);
        //    if (orderItem != null && orderItem.Status == ProductStatus.Processing)
        //    {
        //        orderItem.Status = ProductStatus.RawProduct;

        //        db.Entry(orderItem).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}