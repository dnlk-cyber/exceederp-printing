using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Finance;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Inventory.Controllers.Transfer
{
    
    public class TransferReceiveItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly DistributionHelper _distributionHelper = new DistributionHelper();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover, FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult ItemTransferReceivesItems_Read([DataSourceRequest]DataSourceRequest request , int id)
        {
            IQueryable<ItemTransferItemReceive> itemtransferreceivesItems = db.ItemTransferItemReceive.Where(i => i.ItemTransferReceiveId == id);
            DataSourceResult result = itemtransferreceivesItems.ToDataSourceResult(request, itemTransferReceiveItem => new {
                ItemTransferItemReceiveId = itemTransferReceiveItem.ItemTransferItemReceiveId,
                ItemTransferReceiveId = itemTransferReceiveItem.ItemTransferReceiveId,
                ItemCategoryCode = itemTransferReceiveItem.ItemCategoryCode,
                ItemCode = itemTransferReceiveItem.ItemCode,
                PartOrCodeNumber = itemTransferReceiveItem.PartOrCodeNumber,
                RequestedQuantity = itemTransferReceiveItem.RequestedQuantity,
                ShippedQuantity = itemTransferReceiveItem.ShippedQuantity,
                UPrice = itemTransferReceiveItem.UPrice,
                TPrice = itemTransferReceiveItem.TPrice,
                ReceivedQuantity = itemTransferReceiveItem.ReceivedQuantity,
                Remark = itemTransferReceiveItem.Remark,
              
                DateCreated = itemTransferReceiveItem.DateCreated,
                LastModified = itemTransferReceiveItem.LastModified,
                CreatedBy = itemTransferReceiveItem.CreatedBy,
                ModifiedBy = itemTransferReceiveItem.ModifiedBy
            });

            return Json(result);
        }

    

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferReceivesItems_Update([DataSourceRequest]DataSourceRequest request, ItemTransferItemReceive itemTransferReceiveItem)
        {
            #region validation
            var exist = db.ItemTransferItemReceive.Where(x => x.ItemTransferReceiveId == itemTransferReceiveItem.ItemTransferReceiveId)
               .Where(x => x.ItemTransferItemReceiveId != itemTransferReceiveItem.ItemTransferItemReceiveId)
               .Where(x => x.ItemCode == itemTransferReceiveItem.ItemCode)
               .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + itemTransferReceiveItem.ItemCode + " already exists");
            }

            if (itemTransferReceiveItem.ReceivedQuantity > itemTransferReceiveItem.ShippedQuantity)
            {
                ModelState.AddModelError("extra", "The received quantity should be less than or equal to shipped quantity: " + itemTransferReceiveItem.ShippedQuantity);
            }

            #region check if they receive extra
            var transferReceive = db.ItemTransferReceive.Find(itemTransferReceiveItem.ItemTransferReceiveId);
            var sentItem = db.ItemTransferItems
                .Where(x => x.ItemTransferID == transferReceive.ItemTransferID)
                .Where(x => x.ItemCode == itemTransferReceiveItem.ItemCode)
                .FirstOrDefault();

            if (sentItem != null)
            {
                decimal totalQty = sentItem.ShippedQuantity;
                decimal receivedQty = 0;
                decimal remainingQty = 0;

                var alreadyReceivedQty = db.ItemTransferItemReceive
                    .Where(x => x.ItemTransferReceive.IsOnlineApproved)
                     .Where(x => x.ItemTransferReceive.IsVoid == false)
                    .Where(x => x.ItemCode == itemTransferReceiveItem.ItemCode)
                    .Where(x => x.ItemTransferReceive.ItemTransferID == transferReceive.ItemTransferID).ToList();
                if (alreadyReceivedQty != null)
                    receivedQty = alreadyReceivedQty.Sum(x => x.ReceivedQuantity);
                remainingQty = totalQty - receivedQty;

                if (remainingQty <= 0)
                    ModelState.AddModelError("over","The remaining unreceived balance is 0, hence you can't add.");
                if (remainingQty < itemTransferReceiveItem.ReceivedQuantity)
                    ModelState.AddModelError("over", $"The remaining unreceived balance is {remainingQty}, hence you receive only {remainingQty}.");
            }
            #endregion
            #endregion
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferItemReceive
                {
                    ItemTransferItemReceiveId = itemTransferReceiveItem.ItemTransferItemReceiveId,
                    ItemTransferReceiveId = itemTransferReceiveItem.ItemTransferReceiveId,
                    ItemCategoryCode = itemTransferReceiveItem.ItemCategoryCode,
                    ItemCode = itemTransferReceiveItem.ItemCode,
                    PartOrCodeNumber = itemTransferReceiveItem.PartOrCodeNumber,
                    RequestedQuantity = itemTransferReceiveItem.RequestedQuantity,
                    ShippedQuantity = itemTransferReceiveItem.ShippedQuantity,
                    UPrice = itemTransferReceiveItem.UPrice,
                    TPrice = itemTransferReceiveItem.ReceivedQuantity * itemTransferReceiveItem.UPrice,
                    ReceivedQuantity = itemTransferReceiveItem.ReceivedQuantity,
                    Remark = itemTransferReceiveItem.Remark,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,

                };

                db.ItemTransferItemReceive.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                var record = db.ItemTransferReceive.Find(itemTransferReceiveItem.ItemTransferReceiveId);
                if (record != null)
                   _distributionHelper.UpdateTransferReceiveDistribution(record);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "transfer receive item", "Success", User.Identity.Name, $"updated transfer receive item {entity.ItemCode} under record {entity.ItemTransferReceiveId} ", Modules.Inventory);
                #endregion
            }
            return Json(new[] { itemTransferReceiveItem }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferReceivesItems_Destroy([DataSourceRequest]DataSourceRequest request, ItemTransferItemReceive itemTransferItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferItemReceive
                {
                    ItemTransferReceiveId = itemTransferItem.ItemTransferReceiveId,
                    ItemCategoryCode = itemTransferItem.ItemCategoryCode,
                    ItemCode = itemTransferItem.ItemCode,
                    PartOrCodeNumber = itemTransferItem.PartOrCodeNumber,
                    RequestedQuantity = itemTransferItem.RequestedQuantity,
                    ShippedQuantity = itemTransferItem.ShippedQuantity,
                    UPrice = itemTransferItem.UPrice,
                    TPrice = itemTransferItem.TPrice,
                    ReceivedQuantity = itemTransferItem.ReceivedQuantity,
                    Remark = itemTransferItem.Remark,

                };
                //DistributionHelper.UpdateTransferSendDistribution(entity.ItemTransfer);
                db.ItemTransferItemReceive.Attach(entity);
                db.ItemTransferItemReceive.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted item transfer receive item " + itemTransferItem.ItemTransferReceiveId + " " + itemTransferItem.InventoryItem.Name);
                #endregion

                var record = db.ItemTransferReceive.Find(itemTransferItem.ItemTransferReceiveId);
                if (record != null)
                   _distributionHelper.UpdateTransferReceiveDistribution(record);
            }

            return Json(new[] { itemTransferItem }.ToDataSourceResult(request, ModelState));
        }

       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
