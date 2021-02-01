using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Helpers.Finance
{
    public static class FinanceTransferHelper
    {

        #region AP

        public static List<int> GetApMiscPaymentTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.APMiscellaneousPayments.Where(x=>x.IsOnlineApproved&&x.IsOnlineTransferred==false&&x.IsVoid==false).Select(x => x.ApMiscellaneousPaymentId).ToList();
           
        }
        public static List<int> GetApPaymentTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.APPayments.Where(x=>x.IsPrepayment==false).Where(x=>x.IsOnlineApproved&&x.IsOnlineTransferred==false&&x.IsVoid==false).Select(x => x.ApPaymentId).ToList();
           
        }public static List<int> GetApAdvancePaymentTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.APPayments.Where(x => x.IsPrepayment).Where(x=>x.IsOnlineApproved&&x.IsOnlineTransferred==false&&x.IsVoid==false).Select(x => x.ApPaymentId).ToList();
           
        }
        public static List<int> GetApDirectPaymentTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.ApDirectPayments.Where(x=>x.IsOnlineApproved&&x.IsOnlineTransferred==false&&x.IsVoid==false).Select(x => x.ApDirectPaymentId).ToList();
           
        }
        public static List<int> GetApPaymentSettlementTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.ApPaymentSettlements.Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ApPaymentSettlementId).ToList();

        }
        public static List<int> GetApManualInvoiceTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.APPaybleInvoices.Where(x=>x.IsManual).Where(x=>x.IsOnlineApproved&&x.IsOnlineTransferred==false&&x.IsVoid==false).Select(x => x.ApPaybleInvoicesId).ToList();
           
        }
        public static List<int> GetApInvoiceTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.APPaybleInvoices.Where(x=>x.IsManual==false).Where(x=>x.IsOnlineApproved&&x.IsOnlineTransferred==false&&x.IsVoid==false).Select(x => x.ApPaybleInvoicesId).ToList();
           
        }

        #endregion
        #region AR

        public static List<int> GetArMiscCollectionTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.ARMiscelaneousCollections.Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ARMiscelaneousCollectionID).ToList();

        }
        public static List<int> GetArStandardCollectionTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.ARStandardCollections.Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ArStandardCollectionId).ToList();

        }
        public static List<int> GetArInvoiceManualTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.ARInvoices.Where(x => x.IsManual == false).Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ArInvoiceId).ToList();

        }
        public static List<int> GetArInvoiceTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.ARInvoices.Where(x=>x.IsManual==false).Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.IsVoid == false).Select(x => x.ArInvoiceId).ToList();

        }
        public static List<int> GetArSalesOrderTranferableList()
        {
            ExceedDbContext db = new ExceedDbContext();
            return db.ArSalesOrders.Where(x => x.IsOnlineApproved && x.IsOnlineTransferred == false && x.isVoid == false).Select(x => x.ArSalesOrderId).ToList();

        }
        

        #endregion

    }
}
