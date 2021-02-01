using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
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
    public class CleanedNaturalGumSellingPriceItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult SellingPrices_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CleanedNaturalGumSellingPriceItem> items = db.CleanedNaturalGumSellingPriceItem.Where(i => i.CleanedNaturalGumSellingPriceId == id);
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                CleanedNaturalGumSellingPriceItemId = item.CleanedNaturalGumSellingPriceItemId,
                CleanedNaturalGumSellingPriceId = item.CleanedNaturalGumSellingPriceId,
                GumProcessingOrderItemId = item.GumProcessingOrderItemId,
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

        //private bool CheckExistance(int projectId)
        //{
        //    bool paymentExists = false;
        //    var item = db.ProjectAdvancePayments.Where(ap => ap.ProjectInformationId == projectId).FirstOrDefault();
        //    if (item != null)
        //    {
        //        paymentExists = true;
        //    }
        //    return paymentExists;
        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SellingPrice_Create([DataSourceRequest]DataSourceRequest request, CleanedNaturalGumSellingPriceItem item)
        {
            if (ModelState.IsValid)
            {
                //if (CheckExistance(projectIdCreateAdvance))
                //{
                //    ModelState.AddModelError("", "Advance Payment Exists for this project!!");
                //}
                //else
                //{
                var entity = new CleanedNaturalGumSellingPriceItem
                {
                    CleanedNaturalGumSellingPriceItemId = item.CleanedNaturalGumSellingPriceItemId,
                    CleanedNaturalGumSellingPriceId = item.CleanedNaturalGumSellingPriceId,
                    GumProcessingOrderItemId = item.GumProcessingOrderItemId,
                    RawQuantity = item.RawQuantity,
                    CleanedQuantity = item.CleanedQuantity,
                    ProductionCost = item.ProductionCost,
                    ProcessingAndPackingCost = item.ProcessingAndPackingCost,
                    Recovery = item.Recovery,
                    TotalNetCost = item.TotalNetCost,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = item.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = item.ModifiedBy
                };
                entity.TotalNetCost = entity.ProductionCost + entity.ProcessingAndPackingCost - entity.Recovery;

                db.CleanedNaturalGumSellingPriceItem.Add(entity);
                db.SaveChanges();
                item.CleanedNaturalGumSellingPriceItemId = entity.CleanedNaturalGumSellingPriceItemId;
                
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SellingPrice_Update([DataSourceRequest]DataSourceRequest request, CleanedNaturalGumSellingPriceItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new CleanedNaturalGumSellingPriceItem
                {
                    CleanedNaturalGumSellingPriceItemId = item.CleanedNaturalGumSellingPriceItemId,
                    CleanedNaturalGumSellingPriceId = item.CleanedNaturalGumSellingPriceId,
                    GumProcessingOrderItemId = item.GumProcessingOrderItemId,
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
                entity.TotalNetCost = entity.ProductionCost +  entity.ProcessingAndPackingCost - entity.Recovery;
                

                db.CleanedNaturalGumSellingPriceItem.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SellingPrice_Destroy([DataSourceRequest]DataSourceRequest request, CleanedNaturalGumSellingPriceItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new CleanedNaturalGumSellingPriceItem
                {
                    CleanedNaturalGumSellingPriceItemId = item.CleanedNaturalGumSellingPriceItemId,
                    CleanedNaturalGumSellingPriceId = item.CleanedNaturalGumSellingPriceId,
                    GumProcessingOrderItemId = item.GumProcessingOrderItemId,
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

                db.CleanedNaturalGumSellingPriceItem.Attach(entity);
                db.CleanedNaturalGumSellingPriceItem.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
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
        //private decimal[]  GetProductionItemQuantities(int parentId, CleanedNaturalGumSellingPriceItem record) {
        //    var itemToreturn = new CleanedNaturalGumSellingPriceItem();
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