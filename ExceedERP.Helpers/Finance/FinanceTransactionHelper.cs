using System.Data.Entity;
using System.Linq;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Procurement
{
    public  static class FinanceTransactionHelper
    {

        public static void ClosePaymentRequestStatus()
        {
            ExceedDbContext db = new ExceedDbContext();
            var paymentRequests = db.PaymentRequests.Where(x => x.Paid == false).Select(x => x.PaymentRequestId).ToList();
            foreach (var pr in paymentRequests)
            {
                ClosePaymentRequest(pr);             

            }

        }
        public static void ClosePaymentRequest(int id)
        {
            ExceedDbContext db = new ExceedDbContext();
            var po = db.PaymentRequests.Find(id);

            #region check over advance taken

            var paidData = db.ApDirectPaymentLines
                .Where(x => x.ApDirectPayment.SourceId == "PAYMENT-REQUEST_"+id.ToString())                
                .Where(x => x.ApDirectPayment.IsOnlineApproved && x.ApDirectPayment.IsVoid == false)
                .Select(x => new { x.Amount })
                .AsQueryable();

            if (paidData.Any())
            {
                decimal requestedAmount = db.PaymentRequestLines.Where(x => x.PaymentRequestId ==id)
                    .Select(x => new { x.Amount }).ToList()
                    .Sum(x => x.Amount);

                decimal paidAmount = paidData.Sum(x => x.Amount);
                if (paidAmount >= requestedAmount)
                {
                    if (po != null)
                    {
                        po.Paid = true;
                        db.Entry(po).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }

            #endregion
        }

        public static void CloseTravelOrderStatus()
        {
            ExceedDbContext db = new ExceedDbContext();
            var travelOrders = db.ApTravelOrders.Where(x => x.Paid == false).Select(x => x.ApTravelOrderId).ToList();
            foreach (var pr in travelOrders)
            {
                CloseTravelOrder(pr);

            }

        }
        public static void CloseTravelOrder(int id)
        {
            ExceedDbContext db = new ExceedDbContext();
            var po = db.PaymentRequests.Find(id);

            #region check over advance taken

            var paidData = db.ApDirectPaymentLines
                .Where(x => x.ApDirectPayment.SourceId == "TRAVEL-ORDER_" + id.ToString())
                .Where(x => x.ApDirectPayment.IsOnlineApproved && x.ApDirectPayment.IsVoid == false)
                .Select(x => new { x.Amount })
                .AsQueryable();

            if (paidData.Any())
            {
                decimal requestedAmount = db.ApTravelOrderAdvancePayments.Where(x => x.ApTravelOrderId == id)
                    .Select(x => new { x.Total }).ToList()
                    .Sum(x => x.Total);

                decimal paidAmount = paidData.Sum(x => x.Amount);
                if (paidAmount >= requestedAmount)
                {
                    if (po != null)
                    {
                        po.Paid = true;
                        db.Entry(po).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }

            #endregion
        }

        public static void CloseTravelExpenseClaimStatus()
        {
            ExceedDbContext db = new ExceedDbContext();
            var expenseClaim = db.ApTravelExpenseClaims.Where(x => x.Paid == false).Select(x => x.ApTravelExpenseClaimId).ToList();
            foreach (var pr in expenseClaim)
            {
                CloseTravelExpenseClaim(pr);

            }

        }
        public static void CloseTravelExpenseClaim(int id)
        {
            ExceedDbContext db = new ExceedDbContext();
            var po = db.PaymentRequests.Find(id);

            #region check over advance taken

            var paidData = db.ApDirectPaymentLines
                .Where(x => x.ApDirectPayment.SourceId == "EXPENSE-CLAIM_" + id.ToString())
                .Where(x => x.ApDirectPayment.IsOnlineApproved && x.ApDirectPayment.IsVoid == false)
                .Select(x => new { x.Amount })
                .AsQueryable();

            if (paidData.Any())
            {
                decimal requestedAmount = db.PaymentRequestLines.Where(x => x.PaymentRequestId == id)
                    .Select(x => new { x.Amount }).ToList()
                    .Sum(x => x.Amount);

                decimal paidAmount = paidData.Sum(x => x.Amount);
                if (paidAmount >= requestedAmount)
                {
                    if (po != null)
                    {
                        po.Paid = true;
                        db.Entry(po).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }

            #endregion
        }

    }
}
