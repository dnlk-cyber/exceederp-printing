using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProductionStores
{
    public class CleanedSeedSellingPriceItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult SellingPrices_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CleanedSeedSellingPriceItem> items = db.CleanedSeedSellingPriceItems.Where(i => i.CleanedSeedSellingPriceId == id);
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                CleanedSeedSellingPriceItemId = item.CleanedSeedSellingPriceItemId,
                CleanedSeedSellingPriceId = item.CleanedSeedSellingPriceId,
                SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                RawQuantity = item.RawQuantity,
                CleanedQuantity = item.CleanedQuantity,
                ProductionCost = item.ProductionCost,
                ProcessingAndPackingCost = item.ProcessingAndPackingCost,
                Recovery = item.Recovery,
                TotalNetCost = item.TotalNetCost,
                

                OrgBranchName = item.OrgBranchName,
                DateCreated = item.DateCreated,
                LastModified = item.LastModified,
                CreatedBy = item.CreatedBy,
                ModifiedBy = item.ModifiedBy
            });

            return Json(result);
        }

        private bool ItemExists(int parentId, CleanedSeedSellingPriceItem model)
        {
            return db.CleanedSeedSellingPriceItems
                .FirstOrDefault(it => it.CleanedSeedSellingPriceId == parentId && 
                                        it.SeedProcessingOrderItemId == model.SeedProcessingOrderItemId && 
                                        it.CleanedSeedSellingPriceItemId != model.CleanedSeedSellingPriceItemId) != null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SellingPrice_Create([DataSourceRequest]DataSourceRequest request, CleanedSeedSellingPriceItem item, int id)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(id, item))
                {
                    ModelState.AddModelError("", "Item Exists!");
                }
                else
                {
                    var processingOrderItem = this.GetProcessingOrderItem(item.SeedProcessingOrderItemId);
                    decimal rawQty = 0, cleanQty = 0, productionCost = 0, processingCost = 0;
                    if (processingOrderItem != null)
                    {
                        rawQty = processingOrderItem.RawQuantity;
                        cleanQty = processingOrderItem.QuantityProduced;

                        processingCost = processingOrderItem.TotalCost;
                        productionCost = processingOrderItem.TotalProductionCost;
                    }
                    var entity = new CleanedSeedSellingPriceItem
                    {
                        CleanedSeedSellingPriceItemId = item.CleanedSeedSellingPriceItemId,
                        CleanedSeedSellingPriceId = id,
                        SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                        RawQuantity = rawQty,
                        CleanedQuantity = cleanQty,
                        ProductionCost = productionCost,
                        ProcessingAndPackingCost = processingCost,
                        Recovery = item.Recovery,
                        TotalNetCost = item.TotalNetCost,

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = item.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = item.ModifiedBy
                    };
                    entity.TotalNetCost = entity.ProductionCost + entity.ProcessingAndPackingCost - entity.Recovery;

                    db.CleanedSeedSellingPriceItems.Add(entity);
                    db.SaveChanges();
                    item.CleanedSeedSellingPriceItemId = entity.CleanedSeedSellingPriceItemId;

                    CleanedProductsSellingPriceHelper.CalculateSellingPrice(id, ProductType.Seed);

                }
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SellingPrice_Update([DataSourceRequest]DataSourceRequest request, CleanedSeedSellingPriceItem item)
        {
            if (ModelState.IsValid)
            {
                if (ItemExists(item.CleanedSeedSellingPriceId, item))
                {
                    ModelState.AddModelError("", "Item Exists!");
                }
                else
                {
                    var entity = new CleanedSeedSellingPriceItem
                    {
                        CleanedSeedSellingPriceItemId = item.CleanedSeedSellingPriceItemId,
                        CleanedSeedSellingPriceId = item.CleanedSeedSellingPriceId,
                        SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                        RawQuantity = item.RawQuantity,
                        CleanedQuantity = item.CleanedQuantity,
                        ProductionCost = item.ProductionCost,
                        ProcessingAndPackingCost = item.ProcessingAndPackingCost,
                        Recovery = item.Recovery,
                        TotalNetCost = item.TotalNetCost,

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = item.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = item.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };
                    entity.TotalNetCost = entity.ProductionCost + entity.ProcessingAndPackingCost - entity.Recovery;


                    db.CleanedSeedSellingPriceItems.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    CleanedProductsSellingPriceHelper.CalculateSellingPrice(entity.CleanedSeedSellingPriceId, ProductType.Seed);

                }
            }
            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SellingPrice_Destroy([DataSourceRequest]DataSourceRequest request, CleanedSeedSellingPriceItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new CleanedSeedSellingPriceItem
                {
                    CleanedSeedSellingPriceItemId = item.CleanedSeedSellingPriceItemId,
                    CleanedSeedSellingPriceId = item.CleanedSeedSellingPriceId,
                    SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                    RawQuantity = item.RawQuantity,
                    CleanedQuantity = item.CleanedQuantity,
                    ProductionCost = item.ProductionCost,
                    ProcessingAndPackingCost = item.ProcessingAndPackingCost,
                    Recovery = item.Recovery,
                    TotalNetCost = item.TotalNetCost,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = item.LastModified,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy
                };

                db.CleanedSeedSellingPriceItems.Attach(entity);
                db.CleanedSeedSellingPriceItems.Remove(entity);
                db.SaveChanges();

                CleanedProductsSellingPriceHelper.CalculateSellingPrice(entity.CleanedSeedSellingPriceId, ProductType.Seed);
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        private SeedProcessingOrderItem GetProcessingOrderItem(int id)
        {
            return db.SeedProcessingOrderItems.Find(id);
        }
        private decimal GetProductionCost(int id)
        {
            var amnt = new decimal();
            var item = db.SeedProductionOrderItems.Find(id);
            if(item != null)
            {
                amnt = item.TotalCost;
            }
            return amnt;
        }
        //private string[] GetInventoryItemCodes(int seedProcessingItemId)
        //{
        //    string[] codes = new string[2];
        //    var seedProcessingItem = db.SeedProcessingOrderItems.Find(seedProcessingItemId);
        //    if( seedProcessingItem != null)
        //    {
                
        //            var currentSeedItem = db.SeedProductionOrderItems.Find(seedProcessingItem.SeedProductionOrderItemId);
        //            if(currentSeedItem != null)
        //            {
        //                var crop = db.Crops.Find(currentSeedItem.CropId);
        //                if(crop != null)
        //                {
        //                    codes[0] = crop.ItemCode;
        //                    codes[1] = crop.ItemCategoryCode;
        //                }
        //            }
        //    }
            
        //    return codes;
        //}
        //private decimal[]  GetProductionItemQuantities(int parentId, CleanedSeedSellingPriceItem record) {
        //    var itemToreturn = new CleanedSeedSellingPriceItem();
        //    decimal[] valuesToReturn = new decimal[2];
        //    var parentItem = db.ProductionStoreReceives.Find(parentId);

        //    if(parentItem != null)
        //    {
        //        if(parentItem.ProductType == ProductType.Seed)
        //        {
        //            var productionItem = db.SeedProductionOrderItems.Find(record.SeedProductionOrderItemId);
        //            if(productionItem != null)
        //            {
        //                valuesToReturn[0] = productionItem.QuantityProduced;
        //                valuesToReturn[1] = productionItem.TotalCost;
        //            }
        //        }
        //        else
        //        {
        //            var productionItem = db.GumProductionOrderItems.Find(record.GumProductionOrderItemId);
        //            if(productionItem != null)
        //            {
        //                valuesToReturn[0] = productionItem.QuantityProduced;
        //                valuesToReturn[1] = productionItem.TotalCost;
        //            }
        //        }
        //    }
        //    return valuesToReturn;
        //}
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}