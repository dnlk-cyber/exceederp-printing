
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Operation
{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpFinishedGoodUser)]
    public class FurnitureProductionStoreReceiveItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult ReceiveItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureProductionFinishedGoodsStoreReceiveItem> items = db.FurnitureProductionFinishedGoodsStoreReceiveItems.Where(itm => itm.FinishedGoodsStoreReceiveId == id);
            DataSourceResult result = items.ToDataSourceResult(request, item => new
            {
                FinishedGoodsStoreReceiveItemId = item.FurnitureFinishedGoodsStoreReceiveItemId,
                FinishedGoodsStoreReceiveId = item.FinishedGoodsStoreReceiveId,
                //GumProductionOrderItemId = item.GumProductionOrderItemId,
                //SeedProductionOrderItemId = item.SeedProductionOrderItemId,
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
        public ActionResult ReceiveItem_Create([DataSourceRequest]DataSourceRequest request, FurnitureProductionFinishedGoodsStoreReceiveItem item, int id)
        {
            if (ModelState.IsValid)
            {
                item.FinishedGoodsStoreReceiveId = id;
                decimal unitPrice = getTaxAmount(item).Item1;
                var entity = new FurnitureProductionFinishedGoodsStoreReceiveItem
                {
                    FurnitureFinishedGoodsStoreReceiveItemId = item.FurnitureFinishedGoodsStoreReceiveItemId,
                    FinishedGoodsStoreReceiveId = id,
                    //GumProductionOrderItemId = item.GumProductionOrderItemId,
                    //SeedProductionOrderItemId = item.SeedProductionOrderItemId,
                    AccountCode = item.AccountCode,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    SubTotal = item.Quantity * unitPrice,
                    GLTaxRateID = getTaxAmount(item).Item2,
                    TaxAmount = getTaxAmount(item).Item3 * unitPrice * item.Quantity,
                    Total = item.Quantity * unitPrice + (item.Quantity * unitPrice * getTaxAmount(item).Item3),
                    Balance = item.Balance,
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,
                    OrgBranchName = item.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = item.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = item.ModifiedBy
                };

                entity.Total = entity.TaxAmount+ entity.SubTotal;

                db.FurnitureProductionFinishedGoodsStoreReceiveItems.Add(entity);
                db.SaveChanges();
                item.FurnitureFinishedGoodsStoreReceiveItemId = entity.FurnitureFinishedGoodsStoreReceiveItemId;

                item.SubTotal = entity.SubTotal;
                item.TaxAmount = entity.TaxAmount;
                item.Total = entity.Total;

                //}
           //     new ProductionDistributionHelper().UpdateGoodReceiveDistribution(GetReceiveHeader(entity.FinishedGoodsStoreReceiveId));
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveItem_Update([DataSourceRequest]DataSourceRequest request, FurnitureProductionFinishedGoodsStoreReceiveItem item)
        {
            if (ModelState.IsValid)
            {
                decimal unitPrice = getTaxAmount(item).Item1;

                var entity = new FurnitureProductionFinishedGoodsStoreReceiveItem
                {
                    FurnitureFinishedGoodsStoreReceiveItemId = item.FurnitureFinishedGoodsStoreReceiveItemId,
                    FinishedGoodsStoreReceiveId = item.FinishedGoodsStoreReceiveId,
                    //GumProductionOrderItemId = item.GumProductionOrderItemId,
                    //SeedProductionOrderItemId = item.SeedProductionOrderItemId,
                    AccountCode = item.AccountCode,
                    Quantity = item.Quantity,
                    UnitPrice = unitPrice,
                    SubTotal = item.Quantity * unitPrice,
                    GLTaxRateID = getTaxAmount(item).Item2,
                    TaxAmount = getTaxAmount(item).Item3 * unitPrice * item.Quantity,
                //Total = item.Quantity * unitPrice + (item.Quantity * unitPrice * getTaxAmount(item).Item3),
                    Balance = item.Balance,
                    ItemCategoryCode = item.ItemCategoryCode,
                    ItemCode = item.ItemCode,
                    OrgBranchName = item.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = item.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = item.ModifiedBy
                };

                entity.Total = entity.TaxAmount+ entity.SubTotal;
                db.FurnitureProductionFinishedGoodsStoreReceiveItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                item.SubTotal = entity.SubTotal;
                item.TaxAmount = entity.TaxAmount;
                item.Total = entity.Total;

             //   new ProductionDistributionHelper().UpdateGoodReceiveDistribution(GetReceiveHeader(entity.FinishedGoodsStoreReceiveId));

            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }
        public Tuple<decimal, int, decimal> getTaxAmount(FurnitureProductionFinishedGoodsStoreReceiveItem item)
        {
            decimal taxAmount = 0;
            decimal unitPrice = 0;
            int taxId = 0;
            var finishedGoods = db.FurnitureProductionFinishedGoodsStoreReceives.Find(item.FinishedGoodsStoreReceiveId);
            if (finishedGoods != null)
            {
                var jobOrder = db.FurnitureJobs.Find(finishedGoods.FurnitureJobId);
                if (jobOrder != null)
                {
                    unitPrice = (decimal)jobOrder.UnitPrice;
                    var jobType = db.JobCategories.Find(jobOrder.FurnitureJobTypeId);
                    if (jobType != null)
                    {
                        var tax = db.GLTaxRates.Find(jobType.GLTaxRateID);
                        if (tax != null)
                        {
                            taxAmount = tax.CalculatedRate;
                            taxId = tax.GLTaxRateID;
                        }

                    }
                }
            }
            return Tuple.Create(unitPrice, taxId, taxAmount);
;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReceiveItem_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureProductionFinishedGoodsStoreReceiveItem item)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureProductionFinishedGoodsStoreReceiveItem
                {
                    FurnitureFinishedGoodsStoreReceiveItemId = item.FurnitureFinishedGoodsStoreReceiveItemId,
                    FinishedGoodsStoreReceiveId = item.FinishedGoodsStoreReceiveId,
                    //GumProductionOrderItemId = item.GumProductionOrderItemId,
                    //SeedProductionOrderItemId = item.SeedProductionOrderItemId,
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

                db.FurnitureProductionFinishedGoodsStoreReceiveItems.Attach(entity);
                db.FurnitureProductionFinishedGoodsStoreReceiveItems.Remove(entity);
                db.SaveChanges();

               // new ProductionDistributionHelper().UpdateGoodReceiveDistribution(GetReceiveHeader(entity.FinishedGoodsStoreReceiveId));
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        private FurnitureProductionFinishedGoodsStoreReceive GetReceiveHeader(int productionStoreReceiveId)
        {
            return db.FurnitureProductionFinishedGoodsStoreReceives.Find(productionStoreReceiveId);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}