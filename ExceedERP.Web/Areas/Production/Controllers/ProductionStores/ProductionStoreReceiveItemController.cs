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
    public class ProductionStoreReceiveItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult ReceiveItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ProductionStoreReceiveItem> items = db.ProductionStoreReceiveItems.Where(itm => itm.ProductionStoreReceiveId == id);
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                ProductionStoreReceiveItemId = item.ProductionStoreReceiveItemId,
                ProductionStoreReceiveId = item.ProductionStoreReceiveId,
                GumProductionOrderItemId = item.GumProductionOrderItemId,
                SeedProductionOrderItemId = item.SeedProductionOrderItemId,
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
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveItem_Create([DataSourceRequest]DataSourceRequest request, ProductionStoreReceiveItem item, int id)
        {
            if (ModelState.IsValid)
            {
                //if (CheckExistance(projectIdCreateAdvance))
                //{
                //    ModelState.AddModelError("", "Advance Payment Exists for this project!!");
                //}
                //else
                //{
                var entity = new ProductionStoreReceiveItem
                {
                    ProductionStoreReceiveItemId = item.ProductionStoreReceiveItemId,
                    ProductionStoreReceiveId = id,
                    GumProductionOrderItemId = item.GumProductionOrderItemId,
                    SeedProductionOrderItemId = item.SeedProductionOrderItemId,
                    AccountCode = item.AccountCode,
                    Quantity = GetProductionItemQuantities(id, item)[0],
                    UnitPrice = item.UnitPrice,
                    SubTotal = GetProductionItemQuantities(id, item)[1],
                    GLTaxRateID = item.GLTaxRateID,
                    TaxAmount = item.TaxAmount,
                    Total = item.Total,
                    Balance = item.Balance,

                    ItemCategoryCode = GetInventoryItemCodes(id, item)[1],
                    ItemCode = GetInventoryItemCodes(id, item)[0],

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = item.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = item.ModifiedBy
                };

                entity.UnitPrice = entity.Quantity > 0 ?  entity.SubTotal  / entity.Quantity : 0;
                entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                entity.Total = entity.TaxAmount + entity.SubTotal;

                db.ProductionStoreReceiveItems.Add(entity);
                db.SaveChanges();
                item.ProductionStoreReceiveItemId = entity.ProductionStoreReceiveItemId;

                item.SubTotal = entity.SubTotal;
                item.TaxAmount = entity.TaxAmount;
                item.Total = entity.Total;

                //}
                new ProductionDistributionHelper().UpdateGoodReceiveDistribution(GetReceiveHeader(entity.ProductionStoreReceiveId));
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveItem_Update([DataSourceRequest]DataSourceRequest request, ProductionStoreReceiveItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionStoreReceiveItem
                {
                    ProductionStoreReceiveItemId = item.ProductionStoreReceiveItemId,
                    ProductionStoreReceiveId = item.ProductionStoreReceiveId,
                    GumProductionOrderItemId = item.GumProductionOrderItemId,
                    SeedProductionOrderItemId = item.SeedProductionOrderItemId,
                    AccountCode = item.AccountCode,
                    Quantity = GetProductionItemQuantities(item.ProductionStoreReceiveId, item)[0],
                    UnitPrice = item.UnitPrice,
                    SubTotal = GetProductionItemQuantities(item.ProductionStoreReceiveId, item)[1],
                    GLTaxRateID = item.GLTaxRateID,
                    TaxAmount = item.TaxAmount,
                    Total = item.Total,
                    Balance = item.Balance,

                    ItemCategoryCode = GetInventoryItemCodes(item.ProductionStoreReceiveId, item)[1],
                    ItemCode = GetInventoryItemCodes(item.ProductionStoreReceiveId, item)[0],

                    OrgBranchName = item.OrgBranchName,
                    DateCreated = item.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = item.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.UnitPrice = entity.Quantity > 0 ? entity.SubTotal / entity.Quantity : 0;
                entity.TaxAmount = BillingHelper.CalculateTaxAmount(entity.GLTaxRateID, entity.SubTotal);
                entity.Total = entity.TaxAmount + entity.SubTotal;

                db.ProductionStoreReceiveItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                item.SubTotal = entity.SubTotal;
                item.TaxAmount = entity.TaxAmount;
                item.Total = entity.Total;

                new ProductionDistributionHelper().UpdateGoodReceiveDistribution(GetReceiveHeader(entity.ProductionStoreReceiveId));

            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveItem_Destroy([DataSourceRequest]DataSourceRequest request, ProductionStoreReceiveItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionStoreReceiveItem
                {
                    ProductionStoreReceiveItemId = item.ProductionStoreReceiveItemId,
                    ProductionStoreReceiveId = item.ProductionStoreReceiveId,
                    GumProductionOrderItemId = item.GumProductionOrderItemId,
                    SeedProductionOrderItemId = item.SeedProductionOrderItemId,
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

                db.ProductionStoreReceiveItems.Attach(entity);
                db.ProductionStoreReceiveItems.Remove(entity);
                db.SaveChanges();

                new ProductionDistributionHelper().UpdateGoodReceiveDistribution(GetReceiveHeader(entity.ProductionStoreReceiveId));
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        private string[] GetInventoryItemCodes(int parentId, ProductionStoreReceiveItem item)
        {
            string[] codes = new string[2];
            var temp = db.ProductionStoreReceives.Find(parentId);
            if( temp != null)
            {
                if(temp.ProductType == ProductType.Seed)
                {
                    var currentSeedItem = db.SeedProductionOrderItems.Find(item.SeedProductionOrderItemId);
                    if(currentSeedItem != null)
                    {
                        var crop = db.Crops.Find(currentSeedItem.CropId);
                        if(crop != null)
                        {
                            codes[0] = crop.ItemCode;
                            codes[1] = crop.ItemCategoryCode;
                        }
                    }
                }
                else if(temp.ProductType == ProductType.NaturalGum)
                {
                    var currentGumItem = db.GumProductionOrderItems.Find(item.GumProductionOrderItemId);
                    if(currentGumItem != null)
                    {
                        var gum = db.NaturalGums.Find(currentGumItem.NaturalGumId);
                        if(gum != null)
                        {
                            codes[0] = gum.ItemCode;
                            codes[1] = gum.ItemCategoryCode;
                        }
                    }
                }
            }
            
            return codes;
        }
        private decimal[]  GetProductionItemQuantities(int parentId, ProductionStoreReceiveItem record) {
            var itemToreturn = new ProductionStoreReceiveItem();
            decimal[] valuesToReturn = new decimal[2];
            var parentItem = db.ProductionStoreReceives.Find(parentId);

            if(parentItem != null)
            {
                if(parentItem.ProductType == ProductType.Seed)
                {
                    var productionItem = db.SeedProductionOrderItems.Find(record.SeedProductionOrderItemId);
                    if(productionItem != null)
                    {
                        valuesToReturn[0] = productionItem.QuantityProduced;
                        valuesToReturn[1] = productionItem.TotalCost;
                    }
                }
                else
                {
                    var productionItem = db.GumProductionOrderItems.Find(record.GumProductionOrderItemId);
                    if(productionItem != null)
                    {
                        valuesToReturn[0] = productionItem.QuantityProduced;
                        valuesToReturn[1] = productionItem.TotalCost;
                    }
                }
            }
            return valuesToReturn;
        }
        private ProductionStoreReceive GetReceiveHeader(int productionStoreReceiveId)
        {
            return db.ProductionStoreReceives.Find(productionStoreReceiveId);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}