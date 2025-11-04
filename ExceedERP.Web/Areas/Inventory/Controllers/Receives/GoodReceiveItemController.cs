using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Tender.Lot;
using ExceedERP.Core.Domain.Procurement.Tender.Lots;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Receives
{

    public class GoodReceiveItemController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly DistributionHelper _distributionHelper = new DistributionHelper();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover, InventoryConstants.PropertyInspector, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult GoodReceiveItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<GoodReceiveItem> goodreceiveItems = db.GoodReceiveItems.Where(gi => gi.GoodReceiveId == id);
            DataSourceResult result = goodreceiveItems.ToDataSourceResult(request, goodReceiveItem => new
            {

                GoodReceiveItemID = goodReceiveItem.GoodReceiveItemID,
                GoodReceiveId = goodReceiveItem.GoodReceiveId,
                ItemCategoryCode = goodReceiveItem.ItemCategoryCode,
                ItemCode = goodReceiveItem.ItemCode,
                UOM = goodReceiveItem.UOM,
                Quantity = goodReceiveItem.Quantity,
                UnitPrice = goodReceiveItem.UnitPrice,
                SubTotal = goodReceiveItem.SubTotal,
                Total = goodReceiveItem.Total,
                GLTaxRateID = goodReceiveItem.GLTaxRateID,
                TaxAmount = goodReceiveItem.TaxAmount,
                IsExtra = goodReceiveItem.IsExtra,
                ExtraQuantity = goodReceiveItem.ExtraQuantity,
                ExtraAmount = goodReceiveItem.ExtraAmount,
                WarrantyInformation = goodReceiveItem.WarrantyInformation,
                Specification = goodReceiveItem.Specification,
                Expire = goodReceiveItem.Expire,
                ExpireDate = goodReceiveItem.ExpireDate,
                Balance = goodReceiveItem.Balance,
                OrgBranchName = goodReceiveItem.OrgBranchName,
            });

            return Json(result);
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodReceiveItems_Create([DataSourceRequest]DataSourceRequest request, GoodReceiveItem goodReceiveItem, int id)
        {
            var tax = db.GLTaxRates.Find(goodReceiveItem.GLTaxRateID);
            var exist = db.GoodReceiveItems
                .Where(x => x.GoodReceiveId == id)
                .Where(x => x.ItemCode == goodReceiveItem.ItemCode)
                 .Where(x => x.UnitPrice == goodReceiveItem.UnitPrice)
                .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + goodReceiveItem.ItemCode + " with same price already exists");
            }
            if (goodReceiveItem.UnitPrice==0)
            {
                ModelState.AddModelError("zero", "Item " + goodReceiveItem.ItemCode + " price can't be zero");
            }

            #region qty validation

            var grn = db.GoodReceives.Find(id);
            if (grn != null)
            {
                ReceiveType type = db.ReceiveTypes.Find(grn.ReceiveTypeId);
                if (type != null)
                {
                    switch (type.Type)
                    {
                        case GoodReceiveType.PurchaseOrder:
                            int pid = int.Parse(grn.ReceiveRef);
                            PurchaseOrder purchaseOrder = db.PurchaseOrders.Find(pid);
                            var poitem = db.PurchaseOrderItems
                                .Where(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                .FirstOrDefault(x => x.ItmCode == goodReceiveItem.ItemCode);

                            if(poitem != null && goodReceiveItem.Quantity>poitem.Quantity)
                                ModelState.AddModelError("overqty", "Quanity is above purchase order quanity:"+ poitem.Quantity);

                            break;
                        case GoodReceiveType.Tender:
                            int awardid = int.Parse(grn.ReceiveRef);
                            BidAwardedSupplier award = db.BidAwardedSuppliers.Find(awardid);
                            if (award != null)
                            {
                                var awarditem = db.BidAwardedSupplierItems
                                    .Where(x => x.BidAwardedSupplierID == award.BidAwardedSupplierID)
                                    .FirstOrDefault(x => x.ItmCode == goodReceiveItem.ItemCode);

                                if (awarditem != null && goodReceiveItem.Quantity > awarditem.Quanity)
                                    ModelState.AddModelError("overqty", "Quanity is above award quanity:" + awarditem.Quanity);
                            }
                    break;
                        default:
                            ModelState.AddModelError("auto", "The purchase type is not found.");
                            break;
                    }
                }
            }

            #endregion
            if (ModelState.IsValid)
            {
                if (tax != null)
                {
                    var entity = new GoodReceiveItem
                    {
                        GoodReceiveId = id,
                        ItemCategoryCode = goodReceiveItem.ItemCategoryCode,
                        ItemCode = goodReceiveItem.ItemCode,
                        UOM = goodReceiveItem.UOM,
                        Quantity = goodReceiveItem.Quantity,
                        UnitPrice = goodReceiveItem.UnitPrice,
                        SubTotal = goodReceiveItem.UnitPrice * goodReceiveItem.Quantity,
                        GLTaxRateID = goodReceiveItem.GLTaxRateID,
                        TaxAmount = goodReceiveItem.UnitPrice * goodReceiveItem.Quantity * tax.CalculatedRate,
                        Total = goodReceiveItem.UnitPrice * goodReceiveItem.Quantity + (goodReceiveItem.UnitPrice * goodReceiveItem.Quantity * tax.CalculatedRate),
                        IsExtra = goodReceiveItem.IsExtra,
                        ExtraQuantity = goodReceiveItem.ExtraQuantity,
                        ExtraAmount = goodReceiveItem.ExtraAmount,
                        WarrantyInformation = goodReceiveItem.WarrantyInformation,
                        Specification = goodReceiveItem.Specification,
                        Expire = goodReceiveItem.Expire,
                        ExpireDate = goodReceiveItem.ExpireDate,
                        Balance = goodReceiveItem.Balance,
                        OrgBranchName = goodReceiveItem.OrgBranchName,
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Now,
                    };
                    db.GoodReceiveItems.Add(entity);
                    db.SaveChanges();
                    goodReceiveItem.GoodReceiveItemID = entity.GoodReceiveItemID;

                    var record = db.GoodReceives.Find(id);
                    if (record != null)
                       _distributionHelper.UpdateGoodReceiveDistribution(record);

                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "good receive item", "Success", User.Identity.Name, $"created good receive item {entity.ItemCode} under record {entity.GoodReceiveId} ", Modules.Inventory);
                    #endregion
                }


            }
            return Json(new[] { goodReceiveItem }.ToDataSourceResult(request, ModelState));
        }



        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodReceiveItems_Update([DataSourceRequest]DataSourceRequest request, GoodReceiveItem goodReceiveItem, int id)
        {
            var tax = db.GLTaxRates.Find(goodReceiveItem.GLTaxRateID);
            var exist = db.GoodReceiveItems.Where(x => x.GoodReceiveItemID != goodReceiveItem.GoodReceiveItemID)
                       .Where(x => x.GoodReceiveId == goodReceiveItem.GoodReceiveId)
                       .Where(x => x.ItemCode == goodReceiveItem.ItemCode)
                       .Where(x => x.UnitPrice == goodReceiveItem.UnitPrice)
                       .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + goodReceiveItem.ItemCode + " with same price already exists");
            }
            if (goodReceiveItem.UnitPrice == 0)
            {
                ModelState.AddModelError("zero", "Item " + goodReceiveItem.ItemCode + " price can't be zero");
            }

            #region qty validation

            var grn = db.GoodReceives.Find(id);
            if (grn != null)
            {
                ReceiveType type = db.ReceiveTypes.Find(grn.ReceiveTypeId);
                if (type != null)
                {
                    switch (type.Type)
                    {
                        case GoodReceiveType.PurchaseOrder:
                            int pid = int.Parse(grn.ReceiveRef);
                            PurchaseOrder purchaseOrder = db.PurchaseOrders.Find(pid);
                            var poitem = db.PurchaseOrderItems
                                .Where(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                                .FirstOrDefault(x => x.ItmCode == goodReceiveItem.ItemCode);

                            if (poitem != null && goodReceiveItem.Quantity > poitem.Quantity)
                                ModelState.AddModelError("overqty", "Quanity is above purchase order quanity:" + poitem.Quantity);

                            break;
                        case GoodReceiveType.Tender:
                            int awardid = int.Parse(grn.ReceiveRef);
                            BidAwardedSupplier award = db.BidAwardedSuppliers.Find(awardid);
                            if (award != null)
                            {
                                var awarditem = db.BidAwardedSupplierItems
                                    .Where(x => x.BidAwardedSupplierID == award.BidAwardedSupplierID)
                                    .FirstOrDefault(x => x.ItmCode == goodReceiveItem.ItemCode);

                                if (awarditem != null && goodReceiveItem.Quantity > awarditem.Quanity)
                                    ModelState.AddModelError("overqty", "Quanity is above award quanity:" + awarditem.Quanity);
                            }
                            break;
                        default:
                            ModelState.AddModelError("auto", "The purchase type is not found.");
                            break;
                    }
                }
            }

            #endregion

            if (ModelState.IsValid)
            {
                var entity = new GoodReceiveItem
                {
                    GoodReceiveId = goodReceiveItem.GoodReceiveId,
                    GoodReceiveItemID = goodReceiveItem.GoodReceiveItemID,
                    ItemCategoryCode = goodReceiveItem.ItemCategoryCode,
                    ItemCode = goodReceiveItem.ItemCode,
                    UOM = goodReceiveItem.UOM,
                    Quantity = goodReceiveItem.Quantity,
                    UnitPrice = goodReceiveItem.UnitPrice,
                    SubTotal = goodReceiveItem.UnitPrice * goodReceiveItem.Quantity,
                    GLTaxRateID = goodReceiveItem.GLTaxRateID
                };
                if (tax != null)
                    entity.TaxAmount = goodReceiveItem.UnitPrice * goodReceiveItem.Quantity * tax.CalculatedRate;
                entity.Total = entity.SubTotal + entity.TaxAmount;
                entity.IsExtra = goodReceiveItem.IsExtra;
                entity.WarrantyInformation = goodReceiveItem.WarrantyInformation;
                entity.Specification = goodReceiveItem.Specification;
                entity.Expire = goodReceiveItem.Expire;
                entity.ModifiedBy = User.Identity.Name;
                entity.LastModified = DateTime.Now;

                if (goodReceiveItem.ExtraQuantity != null)
                {
                    entity.Balance = goodReceiveItem.Quantity + (decimal)goodReceiveItem.ExtraQuantity;

                }
                else
                {
                    entity.Balance = goodReceiveItem.Quantity;
                }
                

                if (goodReceiveItem.IsExtra)
                {
                    //entity.ExtraQuantity = goodReceiveItem.ExtraQuantity;
                    //entity.ExtraAmount =
                    //    (decimal)
                    //        ((goodReceiveItem.ExtraQuantity*goodReceiveItem.UnitPrice) +
                    //         (goodReceiveItem.ExtraQuantity*goodReceiveItem.UnitPrice*goodReceiveItem.TaxAmount));

                }
                
                db.GoodReceiveItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                var record = db.GoodReceives.Find(id);
                if (record != null)
                   _distributionHelper.UpdateGoodReceiveDistribution(record);


                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "good receive item", "Success", User.Identity.Name, $"updated good receive item {entity.ItemCode} under record {entity.GoodReceiveId} ", Modules.Inventory);
                #endregion

            }

            return Json(new[] { goodReceiveItem }.ToDataSourceResult(request, ModelState));
        }



        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GoodReceiveItems_Destroy([DataSourceRequest]DataSourceRequest request, GoodReceiveItem goodReceiveItem)
        {
            ModelState.Remove("GoodReceiveId");
            if (ModelState.IsValid)
            {
                var entity = new GoodReceiveItem
                {
                    GoodReceiveItemID = goodReceiveItem.GoodReceiveItemID,
                    ItemCode = goodReceiveItem.ItemCode,
                    UOM = goodReceiveItem.UOM,
                    Quantity = goodReceiveItem.Quantity,
                    UnitPrice = goodReceiveItem.UnitPrice,
                    SubTotal = goodReceiveItem.SubTotal,
                    GLTaxRateID = goodReceiveItem.GLTaxRateID,
                    TaxAmount = goodReceiveItem.TaxAmount,
                    Total = goodReceiveItem.Total,
                    IsExtra = goodReceiveItem.IsExtra,
                    ExtraQuantity = goodReceiveItem.ExtraQuantity,
                    ExtraAmount = goodReceiveItem.ExtraAmount,
                    WarrantyInformation = goodReceiveItem.WarrantyInformation,
                    Specification = goodReceiveItem.Specification,
                    Expire = goodReceiveItem.Expire,
                    ExpireDate = goodReceiveItem.ExpireDate,
                    Balance = goodReceiveItem.Balance,
                    OrgBranchName = goodReceiveItem.OrgBranchName,
                };

                db.GoodReceiveItems.Attach(entity);
                db.GoodReceiveItems.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted good receive item " + goodReceiveItem.GoodReceiveId + " " + goodReceiveItem.InventoryItem.Name);
                #endregion
                var record = db.GoodReceives.Find(goodReceiveItem.GoodReceiveId);
                if (record != null)
                   _distributionHelper.UpdateGoodReceiveDistribution(record);
            }

            return Json(new[] { goodReceiveItem }.ToDataSourceResult(request, ModelState));
        }



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
