using ExceedERP.Core.Domain.Inventory;
using ExceedERP.Core.Domain.Procurement.PR;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Procurement
{
    public  class PaymentHelper
    {
        public static string ValidatePurchaseOrder(int id)
        {
            string error=string.Empty;
            ExceedDbContext db = new ExceedDbContext();
            var purchaseOrder=db.PurchaseOrders.Find(id); 

            var schedule=db.PoPaymentSchedules
                    .Where(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID).ToList();
            if (!schedule.Any())
                return "Please add payment schedule first.";

            if (purchaseOrder.PurchasePaymentType == PurchasePaymentType.Credit)
            {
                var creditPaymentSchedule = db.PoPaymentSchedules
                    .Where(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                    .Where(x => x.PaymentType == PaymentType.Credit).ToList();
                if (!creditPaymentSchedule.Any())
                    return "There is no credit payment schedule defined hence can't use credit payment type.";
            }
            if (purchaseOrder.PurchasePaymentType == PurchasePaymentType.Cash)
            {
                var cashPaymentSchedule = db.PoPaymentSchedules
                    .Where(x => x.PurchaseOrderID == purchaseOrder.PurchaseOrderID)
                    .Where(x => x.PaymentType != PaymentType.Credit).ToList();
                if (!cashPaymentSchedule.Any())
                   return "There is no payment or advance payment schedule defined hence can't use cash payment type.";
            }
            return error;
        }
        public static void UpdatePurchasePaymentTotals(int id)
        {
            ExceedDbContext db = new ExceedDbContext();
            decimal totalServiceCharge = 0;
            var record = db.PurchasePayments.Find(id);
            if(record.PaymentType==PaymentType.Credit)
            {
                var receiveType = db.ReceiveTypes.Where(x => x.Type == GoodReceiveType.PurchaseOrder).FirstOrDefault();
                if (receiveType != null)
                {
                    decimal amount = 0;
                    var grns = db.PurchasePaymentGrns.Where(x => x.PurchasePaymentId == id).ToList();
                    foreach (var grn in grns)
                    {
                        if (grn.ItemType == PurchaseItemType.Stock)
                        {
                            #region stock
                            var receive = db.GoodReceives.Find(grn.GrnNo);
                            if (receive != null
                                && receive.IsOnlineApproved
                                && !receive.IsVoid
                                && receive.ReceiveTypeId == receiveType.ReceiveTypeId
                                && receive.ReceiveRef == record.PoOrContractNumber)
                            {
                                var receiveItems = db.GoodReceiveItems
                                                   .Where(x => x.GoodReceiveId == grn.GrnNo).ToList();
                                foreach (var item in receiveItems)
                                {
                                    amount = amount + item.UnitPrice * item.Quantity;
                                }
                            }
                            #endregion
                        }
                        else if (grn.ItemType == PurchaseItemType.FixedAsset)
                        {
                            #region fixed asset 
                            var receive = db.FixedAssetReceives.Find(grn.GrnNo);
                            if (receive != null
                                && receive.IsOnlineApproved
                                && !receive.IsVoid
                                && receive.ReceiveTypeId == receiveType.ReceiveTypeId
                                && receive.ReceiveRef == record.PoOrContractNumber)
                            {
                                var receiveItems = db.FixedAssetReceiveItems
                                                   .Where(x => x.FixedAssetReceiveId == grn.GrnNo).ToList();
                                foreach (var item in receiveItems)
                                {
                                    amount = amount + item.UnitPrice * item.Quantity;
                                }
                            }
                            #endregion
                        }
                        else if (grn.ItemType == PurchaseItemType.Stock)
                        {
                            #region service
                            var receive = db.ServiceReceives.Find(grn.GrnNo);
                            if (receive != null
                                && receive.IsOnlineApproved
                                && !receive.IsVoid
                                && receive.ReceiveTypeId == receiveType.ReceiveTypeId
                                && receive.ReceiveRef == record.PoOrContractNumber)
                            {
                                var receiveItems = db.ServiceReceiveItems
                                                   .Where(x => x.ServiceReceiveId == grn.GrnNo).ToList();
                                foreach (var item in receiveItems)
                                {
                                    amount = amount + item.UnitPrice * item.Quantity;
                                }
                            }
                            #endregion
                        }
                    }

                    #region additional charge
                    int did=int.Parse(record.PoOrContractNumber);
                    var additionalCharge = db.PoAdditionalCharges.Where(x => x.PurchaseOrderID == did).ToList();
                    foreach (var item in additionalCharge)
                    {
                        if (item.AmountType == AmountType.Percent)
                        {
                            totalServiceCharge = amount * (item.Amount / 100);
                        }
                        else
                        {
                            totalServiceCharge = totalServiceCharge + item.Amount;
                        }
                    }
                    #endregion

                    record.PaymentAmount = amount;
                    record.NetPayable = amount;
                    record.AmountDue = amount;
                    db.PurchasePayments.Attach(record);
                    db.Entry(record).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
        }
    }
}
