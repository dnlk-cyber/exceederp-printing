using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using ExceedERP.Helpers.Production;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProductionStores
{
    public class ProductionFinishedGoodsStoreReceiveItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult ReceiveItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProductionFinishedGoodsStoreReceiveItem> items = db.ProductionFinishedGoodsStoreReceiveItems.Where(itm => itm.ProductionFinishedGoodsStoreReceiveId == id);
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                ProductionFinishedGoodsStoreReceiveItemId = item.ProductionFinishedGoodsStoreReceiveItemId,
                ProductionFinishedGoodsStoreReceiveId = item.ProductionFinishedGoodsStoreReceiveId,
                GumProcessingOrderItemId = item.GumProcessingOrderItemId,
                SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                AccountCode = item.AccountCode,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                SubTotal = item.SubTotal,
                GLTaxRateID = item.GLTaxRateID,
                TaxAmount = item.TaxAmount,
                Total = item.Total,
                Balance = item.Balance,

                ItemCategoryCode = item.ItemCategoryCode,
                ItemCode = item.ItemCode,

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
        private bool RawItemNotMapped(int seedProcessingItemId)
        {
            var notMapped = false;
            var processingItem = db.SeedProcessingOrderItems.Find(seedProcessingItemId);
            if (processingItem != null)
            {
                var mapping = db.RawProductFinishedGoodMapping.FirstOrDefault(m => m.RawItemCode == processingItem.ItemCode);
                notMapped = (mapping == null);
            }
            return notMapped;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveItem_Create([DataSourceRequest]DataSourceRequest request, ProductionFinishedGoodsStoreReceiveItem item, int id)
        {
            if (ModelState.IsValid)
            {
                if (RawItemNotMapped(item.SeedProcessingOrderItemId))
                {
                    ModelState.AddModelError("", "Raw Vs. Cleaned Mapping Not Found!");
                }
                else
                {
                    var entity = new ProductionFinishedGoodsStoreReceiveItem
                    {
                        ProductionFinishedGoodsStoreReceiveItemId = item.ProductionFinishedGoodsStoreReceiveItemId,
                        ProductionFinishedGoodsStoreReceiveId = id,
                        GumProcessingOrderItemId = item.GumProcessingOrderItemId,
                        SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                        AccountCode = item.AccountCode,
                        Quantity = this.GetProductionItemQuantities(id, item)[0],
                        UnitPrice = item.UnitPrice,
                        SubTotal = this.GetProductionItemQuantities(id, item)[1],
                        GLTaxRateID = item.GLTaxRateID,
                        TaxAmount = item.TaxAmount,
                        Total = item.Total,
                        Balance = item.Balance,

                        ItemCategoryCode = this.GetInventoryItemCodes(id, item)[1],
                        ItemCode = this.GetInventoryItemCodes(id, item)[0],

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = item.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = item.ModifiedBy
                    };

                    entity.UnitPrice = entity.Quantity > 0 ? entity.SubTotal / entity.Quantity : 0;
                    entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                    entity.Total = entity.TaxAmount + entity.SubTotal;

                    db.ProductionFinishedGoodsStoreReceiveItems.Add(entity);
                    db.SaveChanges();
                    item.ProductionFinishedGoodsStoreReceiveItemId = entity.ProductionFinishedGoodsStoreReceiveItemId;

                    item.SubTotal = entity.SubTotal;
                    item.TaxAmount = entity.TaxAmount;
                    item.Total = entity.Total;

                    new ProductionDistributionHelper().UpdateFinishedGoodReceiveDistribution(GetHeader(entity.ProductionFinishedGoodsStoreReceiveId));


                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        private ProductionFinishedGoodsStoreReceive GetHeader(int productionFinishedGoodsStoreReceiveId)
        {
            return db.ProductionFinishedGoodsStoreReceives.Find(productionFinishedGoodsStoreReceiveId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveItem_Update([DataSourceRequest]DataSourceRequest request, ProductionFinishedGoodsStoreReceiveItem item)
        {
            if (ModelState.IsValid)
            {
                if (RawItemNotMapped(item.SeedProcessingOrderItemId))
                {
                    ModelState.AddModelError("", "Raw Vs. Cleaned Mapping Not Found!");
                }
                else
                {
                    var entity = new ProductionFinishedGoodsStoreReceiveItem
                    {
                        ProductionFinishedGoodsStoreReceiveItemId = item.ProductionFinishedGoodsStoreReceiveItemId,
                        ProductionFinishedGoodsStoreReceiveId = item.ProductionFinishedGoodsStoreReceiveId,
                        GumProcessingOrderItemId = item.GumProcessingOrderItemId,
                        SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                        AccountCode = item.AccountCode,
                        Quantity = GetProductionItemQuantities(item.ProductionFinishedGoodsStoreReceiveId, item)[0],
                        UnitPrice = item.UnitPrice,
                        SubTotal = GetProductionItemQuantities(item.ProductionFinishedGoodsStoreReceiveId, item)[1],
                        GLTaxRateID = item.GLTaxRateID,
                        TaxAmount = item.TaxAmount,
                        Total = item.Total,
                        Balance = item.Balance,

                        ItemCategoryCode = GetInventoryItemCodes(item.ProductionFinishedGoodsStoreReceiveId, item)[1],
                        ItemCode = GetInventoryItemCodes(item.ProductionFinishedGoodsStoreReceiveId, item)[0],

                        OrgBranchName = item.OrgBranchName,
                        DateCreated = item.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = item.CreatedBy,
                        ModifiedBy = User.Identity.Name
                    };
                    entity.UnitPrice = entity.Quantity > 0 ? entity.SubTotal / entity.Quantity : 0;
                    entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                    entity.Total = entity.TaxAmount + entity.SubTotal;

                    db.ProductionFinishedGoodsStoreReceiveItems.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    item.SubTotal = entity.SubTotal;
                    item.TaxAmount = entity.TaxAmount;
                    item.Total = entity.Total;
                    new ProductionDistributionHelper().UpdateFinishedGoodReceiveDistribution(GetHeader(entity.ProductionFinishedGoodsStoreReceiveId));

                }

            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveItem_Destroy([DataSourceRequest]DataSourceRequest request, ProductionFinishedGoodsStoreReceiveItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionFinishedGoodsStoreReceiveItem
                {
                    ProductionFinishedGoodsStoreReceiveItemId = item.ProductionFinishedGoodsStoreReceiveItemId,
                    ProductionFinishedGoodsStoreReceiveId = item.ProductionFinishedGoodsStoreReceiveId,
                    GumProcessingOrderItemId = item.GumProcessingOrderItemId,
                    SeedProcessingOrderItemId = item.SeedProcessingOrderItemId,
                    AccountCode = item.AccountCode,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    SubTotal = item.SubTotal,
                    GLTaxRateID = item.GLTaxRateID,
                    TaxAmount = item.TaxAmount,
                    Total = item.Total,
                    Balance = item.Balance,

                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = item.LastModified,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = item.ModifiedBy
                };

                db.ProductionFinishedGoodsStoreReceiveItems.Attach(entity);
                db.ProductionFinishedGoodsStoreReceiveItems.Remove(entity);
                db.SaveChanges();

                new ProductionDistributionHelper().UpdateFinishedGoodReceiveDistribution(GetHeader(entity.ProductionFinishedGoodsStoreReceiveId));
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        private string[] GetInventoryItemCodes(int parentId, ProductionFinishedGoodsStoreReceiveItem item)
        {
            string[] codes = new string[2];
            var temp = db.ProductionFinishedGoodsStoreReceives.Find(parentId);
            if (temp != null)
            {
                if (temp.ProductType == ProductType.Seed)
                {
                    var processItem = db.SeedProcessingOrderItems.Find(item.SeedProcessingOrderItemId);
                    if (processItem != null)
                    {
                        //var currentSeedItem = db.SeedProductionOrderItems.Find(processItem.SeedProductionOrderItemId);
                        //if (currentSeedItem != null)
                        //{
                        //    var crop = db.Crops.Find(currentSeedItem.CropId);
                        //    if (crop != null)
                        //    {
                        //        codes[0] = crop.ItemCode;
                        //        codes[1] = crop.ItemCategoryCode;
                        //    }
                        //}

                        //codes[0] = processItem.ItemCode;
                        codes[1] = processItem.ItemCategoryCode;

                        var mapping = db.RawProductFinishedGoodMapping.FirstOrDefault(m => m.RawItemCode == processItem.ItemCode);
                        if(mapping != null)
                        {
                            codes[0] = mapping.CleanedItemCode;
                        }
                    }

                }
                else if (temp.ProductType == ProductType.NaturalGum)
                {
                    var processItem = db.GumProcessingOrderItems.Find(item.GumProcessingOrderItemId);
                    if (processItem != null)
                    {
                        //var currentGumItem = db.GumProductionOrderItems.Find(processItem.GumProductionOrderItemId);
                        //if (currentGumItem != null)
                        //{
                        //    var gum = db.NaturalGums.Find(currentGumItem.NaturalGumId);
                        //    if (gum != null)
                        //    {
                        //        codes[0] = gum.ItemCode;
                        //        codes[1] = gum.ItemCategoryCode;
                        //    }
                        //}
                        codes[0] = processItem.ItemCode;
                        codes[1] = processItem.ItemCategoryCode;
                    }
                }
            }

            return codes;
        }
        private decimal[] GetProductionItemQuantities(int parentId, ProductionFinishedGoodsStoreReceiveItem record)
        {
            //var itemToreturn = new ProductionFinishedGoodsStoreReceiveItem();
            decimal[] valuesToReturn = new decimal[2];
            var parentItem = db.ProductionFinishedGoodsStoreReceives.Find(parentId);

            if (parentItem != null)
            {
                if (parentItem.ProductType == ProductType.Seed)
                {
                    var processingItem = db.SeedProcessingOrderItems.Find(record.SeedProcessingOrderItemId);
                    if (processingItem != null)
                    {
                        valuesToReturn[0] = processingItem.QuantityProduced;
                        valuesToReturn[1] = processingItem.TotalCost + processingItem.TotalProductionCost;
                    }
                }
                else
                {
                    var processingItem = db.GumProcessingOrderItems.Find(record.GumProcessingOrderItemId);
                    if (processingItem != null)
                    {
                        valuesToReturn[0] = processingItem.QuantityProduced;
                        valuesToReturn[1] = processingItem.TotalCost + processingItem.TotalProductionCost;
                    }
                }
            }
            return valuesToReturn;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}