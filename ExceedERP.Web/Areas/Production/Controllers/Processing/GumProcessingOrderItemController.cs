using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
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
    public class GumProcessingOrderItemController : Controller
    {
        ExceedDbContext db;

        public GumProcessingOrderItemController()
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
            IQueryable<GumProcessingOrderItem> items = db.GumProcessingOrderItems.Where(itm => itm.GumProcessingOrderId == id);
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                GumProcessingOrderItemId = item.GumProcessingOrderItemId,
                //ProductionIssueId = item.ProductionIssueId,
                ItemCategoryCode = item.ItemCategoryCode,
                ItemCode = item.ItemCode,
                TotalProductionCost = item.TotalProductionCost,
                GumProcessingOrderId = item.GumProcessingOrderId,

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


            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessingItem_Create([DataSourceRequest]DataSourceRequest request, GumProcessingOrderItem item, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new GumProcessingOrderItem
                {
                    GumProcessingOrderItemId = item.GumProcessingOrderItemId,
                    //ProductionIssueId = item.ProductionIssueId,
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,
                    TotalProductionCost = item.TotalProductionCost,
                    GumProcessingOrderId = id,

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
                    DateCreated = DateTime.Now,
                    LastModified = item.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = item.ModifiedBy,
                };
                entity.QuantityProduced = entity.RawQuantity * entity.RecoveryRate / 100;

                db.GumProcessingOrderItems.Add(entity);
                db.SaveChanges();
                item.GumProcessingOrderItemId = entity.GumProcessingOrderItemId;

                UpdateProcessingOrderItemCode(entity.GumProcessingOrderItemId);
                //UpdateProductionOrderItemStatusOnProcessingOrderCreate(entity.GumProductionOrderItemId);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessingItem_Update([DataSourceRequest]DataSourceRequest request, GumProcessingOrderItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new GumProcessingOrderItem
                {
                    GumProcessingOrderItemId = item.GumProcessingOrderItemId,
                    //ProductionIssueId = item.ProductionIssueId,
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,
                    TotalProductionCost = item.TotalProductionCost,
                    GumProcessingOrderId = item.GumProcessingOrderId,

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

                db.GumProcessingOrderItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProcessingItem_Destroy([DataSourceRequest]DataSourceRequest request, GumProcessingOrderItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new GumProcessingOrderItem
                {
                    GumProcessingOrderItemId = item.GumProcessingOrderItemId,
                    //ProductionIssueId = item.ProductionIssueId,
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,
                    TotalProductionCost = item.TotalProductionCost,
                    GumProcessingOrderId = item.GumProcessingOrderId,

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

                db.GumProcessingOrderItems.Attach(entity);
                db.GumProcessingOrderItems.Remove(entity);
                db.SaveChanges();

                //this.UpdateProductionOrderItemStatusOnProcessingOrderDelete(entity.GumProductionOrderItemId);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        private decimal GetRawQuantityFromProduction(int id)
        {
            var qt = new decimal();
            var record = db.GumProductionOrderItems.Find(id);
            if (record != null)
            {
                qt = record.QuantityProduced;
            }
            return qt;
        }
        private void UpdateProcessingOrderItemCode(int id)
        {
            var currentItem = db.GumProcessingOrderItems.Find(id);
            if (currentItem != null)
            {
                currentItem.ProcessingCode = currentItem.GumProcessingOrderItemId.ToString("D12");

                db.GumProcessingOrderItems.Attach(currentItem);
                db.Entry(currentItem).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private void UpdateProductionOrderItemStatusOnProcessingOrderCreate(int id)
        {
            var orderItem = db.GumProductionOrderItems.Find(id);
            if (orderItem != null)
            {
                orderItem.Status = ProductStatus.Processing;

                db.Entry(orderItem).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private void UpdateProductionOrderItemStatusOnProcessingOrderDelete(int id)
        {
            var orderItem = db.GumProductionOrderItems.Find(id);
            if (orderItem != null && orderItem.Status == ProductStatus.Processing)
            {
                orderItem.Status = ProductStatus.RawProduct;

                db.Entry(orderItem).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        private string GetGumName(int id) {
            var name = "";
            var temp = db.GumProductionOrderItems.Find(id);
            if(temp != null)
            {
                //name = temp.ProductType;
                var gumItem = db.NaturalGums.Find(temp.NaturalGumId);
                if(gumItem != null)
                {
                    name = gumItem.Name;
                }
            }
            return name;
        }
        private string GetUnitFromProduction(int id)
        {
            var name = "";
            var item = db.GumProductionOrderItems.Find(id);
            if (item != null)
            {
                name = item.UnitOfOutPutProduced;
            }
            return name;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}