using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.PurchaseOrder;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Helpers.Inventory
{
    public class InventoryValidationHelper
    {
        public static string ValidateGrN(int grnId)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                var record = db.GoodReceives.Find(grnId);
                decimal prevReceivedQty = 0;
                decimal currentReceivedQty = 0;
                ReceiveType type = record.ReceiveType;

                if (type.Type == GoodReceiveType.PurchaseOrder)
                {
                    #region get total receive

                    var previousReceiveIds = db.GoodReceives
                                            .Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder)
                                            .Where(x => x.ReceiveRef == record.ReceiveRef)
                                            .Where(x => x.IsOnlineApproved)
                                            .Where(x => x.IsVoid == false)
                                            .Where(x => x.GoodReceiveId != grnId)//added
                                            .Select(x => x.GoodReceiveId)
                                            .ToList();

                    var prevReceiveItems = db.GoodReceiveItems
                                            .Where(x => previousReceiveIds.Contains(x.GoodReceiveId))
                                            .ToList();

                    #endregion get total receive

                    #region Purchase Order Items

                    int pid = int.Parse(record.ReceiveRef);
                    List<PurchaseOrderItem> purchaseOrderItems = db.PurchaseOrderItems
                        .Where(pi => pi.PurchaseOrderID == pid)
                        .Where(pi => pi.ItemType == PurchaseItemType.Stock)
                        .ToList();

					var changes = db.PoChangeOforders
				   .Where(x => x.IsOnlineApproved && x.IsVoid == false)
				   .Where(x => x.PurchaseOrderID == pid);
                    if (changes.Any())
                    {
                        int maxChangeId = changes.Max(x => x.PoChangeOforderId);
                        var newItems = db.PoChangeOfOrderItems.Where(x => x.PoChangeOforderId == maxChangeId).ToList();
						foreach (var orderItem in newItems)
						{
							prevReceivedQty = 0;
							currentReceivedQty = 0;

							var prevReceiveItem = prevReceiveItems.Where(x => x.ItemCode == orderItem.ItmCode).ToList();
							if (prevReceiveItem.Any())
								prevReceivedQty = prevReceiveItem.Sum(x => x.Quantity);

							var currentReceiveItem = db.GoodReceiveItems
								.Where(x => x.ItemCode == orderItem.ItmCode)
								.Where(x => x.GoodReceiveId == record.GoodReceiveId).ToList();

							if (currentReceiveItem.Any())
								currentReceivedQty = currentReceiveItem.Sum(x => x.Quantity);

							decimal totalReceived = prevReceivedQty + currentReceivedQty;

							if (totalReceived > orderItem.Quantity)
								return $"You can not receive above PO quantity for item {orderItem.ItmCode}.\n Award quanity is {orderItem.Quantity} ,but total receive including previously received will be {totalReceived}";
						}
					}

                    else
                    {

                        foreach (var orderItem in purchaseOrderItems)
                        {
                            prevReceivedQty = 0;
                            currentReceivedQty = 0;

                            var prevReceiveItem = prevReceiveItems.Where(x => x.ItemCode == orderItem.ItmCode).ToList();
                            if (prevReceiveItem.Any())
                                prevReceivedQty = prevReceiveItem.Sum(x => x.Quantity);

                            var currentReceiveItem = db.GoodReceiveItems
                                .Where(x => x.ItemCode == orderItem.ItmCode)
                                .Where(x => x.GoodReceiveId == record.GoodReceiveId).ToList();

                            if (currentReceiveItem.Any())
                                currentReceivedQty = currentReceiveItem.Sum(x => x.Quantity);

                            decimal totalReceived = prevReceivedQty + currentReceivedQty;

                            if (totalReceived > orderItem.Quantity)
                                return $"You can not receive above PO quantity for item {orderItem.ItmCode}.\n Award quanity is {orderItem.Quantity} ,but total receive including previously received will be {totalReceived}";
                        }
                    }

                    #endregion Purchase Order Items
                }
                else if (type.Type == GoodReceiveType.Tender)
                {
                    #region get prev receive

                    var previousReceiveIds = db.GoodReceives.Where(x => x.ReceiveType.Type == GoodReceiveType.Tender)
                                                .Where(x => x.ReceiveRef == record.ReceiveRef)
                                                .Where(x => x.IsOnlineApproved)
                                                .Where(x => x.IsVoid == false)
                                                .Where(x => x.GoodReceiveId != grnId)//added
                                                .Select(x => x.GoodReceiveId)
                                                .ToList();

                    var prevReceiveItems = db.GoodReceiveItems
                                                .Where(x => previousReceiveIds.Contains(x.GoodReceiveId))
                                                .ToList();

                    #endregion get prev receive

                    #region tender

                    int awardid = int.Parse(record.ReceiveRef);
                    var awarditems = db.BidAwardedSupplierItems
                                            .Where(x => x.ItemType == PurchaseItemType.Stock)
                                            .Where(x => x.BidAwardedSupplierID == awardid)
                                            .ToList();
                    foreach (var awardItem in awarditems)
                    {
                        decimal unitprice = awardItem.FinalUPrice;

                        #region check foreign purchase

                        var award = db.BidAwardedSuppliers.Find(awardid);
                        if (award != null)
                        {
                            var lot = db.Lots.Find(award.LotID);
                            if (lot != null)
                            {
                                var tender = db.Tenders.Find(lot.TenderID);
                                if (tender != null && tender.PurchaseCategory == PurchaseCategory.ForeignPurchase)
                                {
                                    var foreignpurchase = db.ForeignPurchases.FirstOrDefault(x => x.PurchaseRef == awardItem.BidAwardedSupplierID);
                                    if (foreignpurchase != null)
                                    {
                                        var item = db.ForeignPurchaseFollowUpItems
                                            .Where(x => x.ItmCode == awardItem.ItmCode)
                                            .FirstOrDefault(x => x.ForeignPurchaseId == foreignpurchase.ForeignPurchaseId);
                                        if (item != null)
                                            unitprice = item.FinalUnitPrice;
                                    }
                                }
                            }
                        }

                        #endregion check foreign purchase

                        prevReceivedQty = 0;
                        currentReceivedQty = 0;

                        var prevReceiveItem = prevReceiveItems.Where(x => x.ItemCode == awardItem.ItmCode).ToList();
                        if (prevReceiveItem.Any())
                            prevReceivedQty = prevReceiveItem.Sum(x => x.Quantity);

                        var currentReceiveItem = db.GoodReceiveItems
                          .Where(x => x.ItemCode == awardItem.ItmCode)
                          .Where(x => x.GoodReceiveId == record.GoodReceiveId).ToList();

                        if (currentReceiveItem.Any())
                            currentReceivedQty = currentReceiveItem.Sum(x => x.Quantity);

                        decimal totalReceived = prevReceivedQty + currentReceivedQty;

                        //if (totalReceived > awardItem.Quanity)
                        //    errorMessage = $"You can not receive above Award quantity for item {awardItem.ItmCode}.\n Award quanity is {awardItem.Quanity} ,but total receive including previously received will be {totalReceived}";
                    }

                    #endregion tender
                }
                return "";
            }
        }

        public static void UpdatePurchaseOrderFullFilledStatus(PurchaseOrder record)
        {
            using (ExceedDbContext db = new ExceedDbContext())
            {
                List<PurchaseOrderItem> purchaseOrderItems = db.PurchaseOrderItems
                    .Where(pi => pi.PurchaseOrderID == record.PurchaseOrderID)
                    .Where(pi => pi.ItemType == PurchaseItemType.Stock)
                    .ToList();

                decimal totalPurchaseOrderQuantity = purchaseOrderItems.Any() ? purchaseOrderItems.Sum(pi => pi.Quantity) : 0;

                var allReceiveIds = db.GoodReceives
                                        .Where(x => x.ReceiveType.Type == GoodReceiveType.PurchaseOrder)
                                        .Where(x => x.ReceiveRef == record.PurchaseOrderID.ToString())
                                        .Where(x => x.IsOnlineApproved)
                                        .Where(x => x.IsVoid == false)
                                        .Select(x => x.GoodReceiveId)
                                        .ToList();

                var allReceiveItems = db.GoodReceiveItems
                                        .Where(x => allReceiveIds.Contains(x.GoodReceiveId))
                                        .ToList();

                decimal totalReceiveQuantity = allReceiveItems.Any() ? allReceiveItems.Sum(ri => ri.Quantity) : 0;

                if (totalReceiveQuantity >= totalPurchaseOrderQuantity)
                {
                    var currentPurchaseOrder = db.PurchaseOrders.Find(record.PurchaseOrderID);
                    if (currentPurchaseOrder != null)
                    {
                        currentPurchaseOrder.IsFullfilled = true;

                        db.PurchaseOrders.Attach(currentPurchaseOrder);
                        db.Entry(currentPurchaseOrder).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
                else // in case it has been set to true when it shouldn't have
                {
                    var currentPurchaseOrder = db.PurchaseOrders.Find(record.PurchaseOrderID);
                    if (currentPurchaseOrder != null)
                    {
                        currentPurchaseOrder.IsFullfilled = false;

                        db.PurchaseOrders.Attach(currentPurchaseOrder);
                        db.Entry(currentPurchaseOrder).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
        }
    }
}