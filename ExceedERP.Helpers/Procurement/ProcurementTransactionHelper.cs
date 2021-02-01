using System.Data.Entity;
using System.Linq;
using ExceedERP.Core.Domain.Procurement.Settings;
using ExceedERP.DataAccess.Context;
using ExceedERP.DataAccess.Migrations;

namespace ExceedERP.Helpers.Procurement
{
    public  static class ProcurementTransactionHelper
    {
       

        public static void ClosePoPaymentStatus()
        {
            ExceedDbContext db = new ExceedDbContext();
            var purchaseOrders = db.PurchaseOrders.Where(x => x.IsPaid == false).ToList();
            foreach (var po in purchaseOrders)
            {
                ClosePoPayment(po.PurchaseOrderID);
                //#region check over advance taken

                //var advanceTaken = db.APPayments
                //    .Where(x => x.PurchaseReference == po.PurchaseOrderID.ToString())
                //    .Where(x => x.PoType == "Purchase Order")
                //    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                //    .Select(x => new { x.PrepaymentAmount })
                //    .AsQueryable();
                //if (advanceTaken.Any())
                //{
                //    decimal advanceAmount = db.PurchaseOrderItems.Where(x => x.PurchaseOrderID == po.PurchaseOrderID)
                //        .Select(x => new { x.UnitPrice, x.Quantity }).ToList()
                //        .Sum(x => x.UnitPrice * x.Quantity);
                //    decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount);
                //    if (advanceTakenAmount >= advanceAmount)
                //    {
                //        po.IsPaid = true;
                //        db.Entry(po).State = EntityState.Modified;
                //        db.SaveChanges();
                //    }
                //}

                //#endregion


            }

        }

        public static void ClosePoPayment(int poid)
        {
            ExceedDbContext db = new ExceedDbContext();
            var po = db.PurchaseOrders.Find(poid);

            #region check over advance taken

            var advanceTaken = db.APPayments
                .Where(x => x.PurchaseReference ==poid.ToString())
                .Where(x => x.PoType == "Purchase Order")
                .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                .Select(x => new { x.PrepaymentAmount })
                .AsQueryable();
            if (advanceTaken.Any())
            {
                decimal advanceAmount = db.PurchaseOrderItems.Where(x => x.PurchaseOrderID ==poid)
                    .Select(x => new { x.UnitPrice, x.Quantity }).ToList()
                    .Sum(x => x.UnitPrice * x.Quantity);
                decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount);
                if (advanceTakenAmount >= advanceAmount)
                {
                    if (po != null)
                    {
                        po.IsPaid = true;
                        db.Entry(po).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }

            #endregion
        }

        public static void CloseContractPaymentStatus()
        {
            ExceedDbContext db = new ExceedDbContext();
            var purchaseOrders = db.PurchaseOrders.Where(x => x.IsPaid == false).ToList();
            foreach (var po in purchaseOrders)
            {
                ClosePoPayment(po.PurchaseOrderID);

            }

        }

        public static void CloseContractPayment(int contid)
        {
            ExceedDbContext db = new ExceedDbContext();
            var contract = db.ContractManagements.Find(contid);

            #region check over advance taken

            var advanceTaken = db.APPayments
                    .Where(x => x.PurchaseReference == contract.ProcurementRefNo.ToString())
                    .Where(x => x.PoType == "Tender Award")
                    .Where(x => x.IsOnlineApproved && x.IsVoid == false)
                   
                .AsQueryable();
            if (advanceTaken.Any())
            {
                decimal advanceAmount = 0;

                var items = db.ContractManagementItems.Where(x => x.ContractManagementID == contract.ContractManagementID).ToList();

                var schedule = db.ContractPaymentSchedules
                    .Where(x => x.ContractManagementID == contract.ContractManagementID)
                    .FirstOrDefault(x => x.PaymentType == PaymentType.Advance);

                foreach (var item in items)
                {
                    advanceAmount = advanceAmount + (item.Quantity + item.Addition + item.Omission) * item.UnitPrice;
                }

                if (schedule != null && schedule.Percentage > 0)
                    advanceAmount = advanceAmount * schedule.Percentage / 100;
                decimal advanceTakenAmount = advanceTaken.Sum(x => x.PrepaymentAmount);
            
                if (advanceTakenAmount >= advanceAmount)
                {
                    if (contract != null)
                    {
                        contract.IsPaid = true;
                        db.Entry(contract).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }

            #endregion
        }

    }
}
