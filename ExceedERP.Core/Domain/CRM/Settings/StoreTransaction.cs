using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{


    public class StoreTransaction : Operations
    {
        public int StoreTransactionID { get; set; }

        public string Voucher { get; set; }

        public string Source { get; set; }

        public string TransactionType { get; set; }
        public string Amount { get; set; }
        public string Destination { get; set; }

        public bool HasEffect { get; set; }
        public decimal Quantity { get; set; }
        public decimal beforeStock { get; set; }
        public decimal afterStock { get; set; }
        public string Remark { get; set; }


    }


}
