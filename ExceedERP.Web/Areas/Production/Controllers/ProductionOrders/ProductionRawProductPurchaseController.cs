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
    public class ProductionRawProductPurchaseController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Purchases_Read([DataSourceRequest]DataSourceRequest request, int itemId, ProductType productionType)
        {

            IQueryable<ProductionRawProductPurchase> purchases = db.ProductionRawProductPurchase.Where(a => a.SeedProductionOrderItemId == itemId || a.GumProductionOrderItemId == itemId);
            if (productionType == ProductType.Seed)
            {
                purchases = purchases.Where(a => a.SeedProductionOrderItemId == itemId);
            }
            else if (productionType == ProductType.NaturalGum)
            {
                purchases = purchases.Where(a => a.GumProductionOrderItemId == itemId);
            }
            DataSourceResult result = purchases.ToDataSourceResult(request, purchase => new
            {
                SeedProductionOrderItemId = purchase.SeedProductionOrderItemId,
                ProductionRawProductPurchaseId = purchase.ProductionRawProductPurchaseId,
                GumProductionOrderItemId = purchase.GumProductionOrderItemId,

                SupplierName = purchase.SupplierName,
                PurchasedQuantity = purchase.PurchasedQuantity,
                UnitPrice = purchase.UnitPrice,
                TotalPrice = purchase.TotalPrice,
               
                OrgBranchName = purchase.OrgBranchName,
                DateCreated = purchase.DateCreated,
                LastModified = purchase.LastModified,
                CreatedBy = purchase.CreatedBy,
                ModifiedBy = purchase.ModifiedBy,
            });

            return Json(result);
        }

        private bool IsInvalidTimeRange(int periodId, DateTime time)
        {
            var isInvalid = false;
            var currentPeriod = db.ProductionPeriods.Find(periodId);
            if (currentPeriod != null)
            {
                if (currentPeriod.DateFrom > time || currentPeriod.DateTo < time)
                {
                    isInvalid = true;
                }
            }
            return isInvalid;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Create([DataSourceRequest]DataSourceRequest request, ProductionRawProductPurchase purchase, int itemId, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                //if (IsInvalidTimeRange(purchase.ProductionPeriodId, purchase.DateOfInformation))
                //{
                //    ModelState.AddModelError("", "Date of info. must be within the period");
                //}
                //else
                //{
                    var entity = new ProductionRawProductPurchase
                    {
                        SeedProductionOrderItemId = productionType == ProductType.Seed ? itemId : purchase.SeedProductionOrderItemId,
                        GumProductionOrderItemId = productionType == ProductType.NaturalGum ? itemId : purchase.GumProductionOrderItemId,

                        SupplierName = purchase.SupplierName,
                        PurchasedQuantity = purchase.PurchasedQuantity,
                        UnitPrice = purchase.UnitPrice,
                        TotalPrice = purchase.TotalPrice,

                        OrgBranchName = purchase.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = purchase.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = purchase.ModifiedBy,

                    };
                entity.TotalPrice = entity.PurchasedQuantity * entity.UnitPrice;

                    db.ProductionRawProductPurchase.Add(entity);
                    db.SaveChanges();

                    purchase.ProductionRawProductPurchaseId = entity.ProductionRawProductPurchaseId;

                    if (productionType == ProductType.Seed)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.SeedProductionOrderItemId, productionType);
                    }
                    else if (productionType == ProductType.NaturalGum)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.GumProductionOrderItemId, productionType);
                    }
                //}
            }

            return Json(new[] { purchase }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Update([DataSourceRequest]DataSourceRequest request, ProductionRawProductPurchase purchase, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                //if (IsInvalidTimeRange(purchase.ProductionPeriodId, purchase.DateOfInformation))
                //{
                //    ModelState.AddModelError("", "Date of info. must be within the period");
                //}
                //else
                //{
                    var entity = new ProductionRawProductPurchase
                    {
                        SeedProductionOrderItemId = purchase.SeedProductionOrderItemId,
                        ProductionRawProductPurchaseId = purchase.ProductionRawProductPurchaseId,
                        GumProductionOrderItemId = purchase.GumProductionOrderItemId,

                        SupplierName = purchase.SupplierName,
                        PurchasedQuantity = purchase.PurchasedQuantity,
                        UnitPrice = purchase.UnitPrice,
                        TotalPrice = purchase.TotalPrice,

                        OrgBranchName = purchase.OrgBranchName,
                        DateCreated = purchase.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = purchase.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                    };
                entity.TotalPrice = entity.PurchasedQuantity * entity.UnitPrice;

                    db.ProductionRawProductPurchase.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    if (productionType == ProductType.Seed)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.SeedProductionOrderItemId, productionType);
                    }
                    else if (productionType == ProductType.NaturalGum)
                    {
                        ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.GumProductionOrderItemId, productionType);
                    }
                //}
            }

            return Json(new[] { purchase }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Purchase_Destroy([DataSourceRequest]DataSourceRequest request, ProductionRawProductPurchase purchase, ProductType productionType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionRawProductPurchase
                {
                    SeedProductionOrderItemId = purchase.SeedProductionOrderItemId,
                    ProductionRawProductPurchaseId = purchase.ProductionRawProductPurchaseId,
                    GumProductionOrderItemId = purchase.GumProductionOrderItemId,
                    
                    SupplierName = purchase.SupplierName,
                    PurchasedQuantity = purchase.PurchasedQuantity,
                    UnitPrice = purchase.UnitPrice,
                    TotalPrice = purchase.TotalPrice,

                    OrgBranchName = purchase.OrgBranchName,
                    DateCreated = purchase.DateCreated,
                    LastModified = purchase.LastModified,
                    CreatedBy = purchase.CreatedBy,
                    ModifiedBy = purchase.ModifiedBy,

                };

                db.ProductionRawProductPurchase.Attach(entity);
                db.ProductionRawProductPurchase.Remove(entity);
                db.SaveChanges();

                if (productionType == ProductType.Seed)
                {
                    ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.SeedProductionOrderItemId, productionType);
                }
                else if (productionType == ProductType.NaturalGum)
                {
                    ProductionOrderHelper.UpdateProductionOrderInCostItemChange(entity.GumProductionOrderItemId, productionType);
                }
            }

            return Json(new[] { purchase }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}