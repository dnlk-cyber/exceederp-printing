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
    [AuthorizeRoles(InventoryConstants.PropertyAdmin, InventoryConstants.PropertyApprover, InventoryConstants.PropertyUser)]
    public class TransferSendItemController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();
        private readonly DistributionHelper _distributionHelper = new DistributionHelper();

        [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover)]
        public ActionResult Index()
        {
            return View();
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper, InventoryConstants.PropertyApprover,FinanceConstants.GeneralSubLedgerTransfer)]
        public ActionResult ItemTransferItems_Read([DataSourceRequest]DataSourceRequest request , int id )
        {
            IQueryable<ItemTransferItem> itemtransferItems = _db.ItemTransferItems.Where(iti  => iti.ItemTransferID == id);
            DataSourceResult result = itemtransferItems.ToDataSourceResult(request, itemTransferItem => new {
                ItemTransferItemID = itemTransferItem.ItemTransferItemID,
                ItemTransferID = itemTransferItem.ItemTransferID,
                ItemCategoryCode = itemTransferItem.ItemCategoryCode,
                ItemCode = itemTransferItem.ItemCode,
                PartOrCodeNumber = itemTransferItem.PartOrCodeNumber,
                RequestedQuantity = itemTransferItem.RequestedQuantity,
                ShippedQuantity = itemTransferItem.ShippedQuantity,
                UPrice = itemTransferItem.UPrice,
                TPrice = itemTransferItem.TPrice,
                ReceivedQuantity = itemTransferItem.ReceivedQuantity,
                Remark = itemTransferItem.Remark,
                 
            });

            return Json(result);
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferItems_Create([DataSourceRequest]DataSourceRequest request, ItemTransferItem itemTransferItem , int id )
         {

             var exist =
              _db.ItemTransferItems.Where(x => x.ItemTransferID == id)
                  .Where(x => x.ItemCode == itemTransferItem.ItemCode)
                  .AsQueryable();
             if (exist.Any())
             {
                 ModelState.AddModelError("exist", "Item " + itemTransferItem.ItemCode + " already exists");
             }

             decimal avgPrice = 0;
             if (id == 0)
             {
                 ModelState.AddModelError("header","Please add header first.");
             }
             var record = _db.ItemTransfers.Find(id);
             if (record != null)
             {
                 avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(itemTransferItem.ItemCode, record.FromStore);
             }

             if (itemTransferItem.ShippedQuantity> itemTransferItem.RequestedQuantity)
                 ModelState.AddModelError("header", "Shpped Quantity should not be greater than requested quanity.");

            if (ModelState.IsValid)
            {
                var entity = new ItemTransferItem
                {
                    ItemTransferID = id,
                    ItemCategoryCode = itemTransferItem.ItemCategoryCode,
                    ItemCode = itemTransferItem.ItemCode,
                    PartOrCodeNumber = itemTransferItem.PartOrCodeNumber,
                    RequestedQuantity = itemTransferItem.RequestedQuantity,
                    ShippedQuantity = itemTransferItem.ShippedQuantity,
                    UPrice = avgPrice,
                    TPrice = itemTransferItem.ShippedQuantity * avgPrice,
                    ReceivedQuantity = 0,
                    Remark = itemTransferItem.Remark,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now

                };

                _db.ItemTransferItems.Add(entity);
                _db.SaveChanges();
                itemTransferItem.ItemTransferItemID = entity.ItemTransferItemID;
                
                if (record != null)
                _distributionHelper.UpdateTransferSendDistribution(record);

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "transfer send item", "Success", User.Identity.Name, $"created transfer send item {entity.ItemCode} under record {entity.ItemTransferID} ", Modules.Inventory);
                #endregion
            }

            return Json(new[] { itemTransferItem }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferItems_Update([DataSourceRequest]DataSourceRequest request, ItemTransferItem itemTransferItem , int id)
        {

            var exist =
            _db.ItemTransferItems.Where(x => x.ItemTransferID == id)
                .Where(x => x.ItemTransferItemID != itemTransferItem.ItemTransferItemID)
                .Where(x => x.ItemCode == itemTransferItem.ItemCode)
                .AsQueryable();
            if (exist.Any())
            {
                ModelState.AddModelError("exist", "Item " + itemTransferItem.ItemCode + " already exists");
            }

            decimal avgPrice = 0;
            if (id == 0)
            {
                ModelState.AddModelError("header", "Please add header first.");
            }
            var record = _db.ItemTransfers.Find(id);
            if (record != null)
            {
                avgPrice = ExceedERP.Helpers.Inventory.InventoryHelper.GetItemAveragePrice(itemTransferItem.ItemCode, record.FromStore);
            }
            if (itemTransferItem.ShippedQuantity > itemTransferItem.RequestedQuantity)
                ModelState.AddModelError("header", "Shpped Quantity should not be greater than requested quanity.");

            if (ModelState.IsValid)
            {
                var entity = new ItemTransferItem
                {
                    ItemTransferItemID = itemTransferItem.ItemTransferItemID,
                    ItemTransferID = itemTransferItem.ItemTransferID,
                    ItemCategoryCode = itemTransferItem.ItemCategoryCode,
                    ItemCode = itemTransferItem.ItemCode,
                    PartOrCodeNumber = itemTransferItem.PartOrCodeNumber,
                    RequestedQuantity = itemTransferItem.RequestedQuantity,
                    ShippedQuantity = itemTransferItem.ShippedQuantity,
                    UPrice = avgPrice,
                    TPrice = itemTransferItem.ShippedQuantity * avgPrice,
                    ReceivedQuantity = 0,
                    Remark = itemTransferItem.Remark,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,

                };

                _db.ItemTransferItems.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
                
                if (record != null)
                    _distributionHelper.UpdateTransferSendDistribution(record);
            }
            
            return Json(new[] { itemTransferItem }.ToDataSourceResult(request, ModelState));
        }

         [AuthorizeRoles(InventoryConstants.PropertyStoreKeeper)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ItemTransferItems_Destroy([DataSourceRequest]DataSourceRequest request, ItemTransferItem itemTransferItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new ItemTransferItem
                {
                    ItemTransferItemID = itemTransferItem.ItemTransferItemID,
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
                _db.ItemTransferItems.Attach(entity);
                _db.ItemTransferItems.Remove(entity);
                _db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted item transfer item " + itemTransferItem.ItemTransferID + " " + itemTransferItem.InventoryItem.Name);
                #endregion

                var record = _db.ItemTransfers.Find(itemTransferItem.ItemTransferID);
                if (record != null)
                    _distributionHelper.UpdateTransferSendDistribution(record);
            }

            return Json(new[] { itemTransferItem }.ToDataSourceResult(request, ModelState));
        }

       

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
