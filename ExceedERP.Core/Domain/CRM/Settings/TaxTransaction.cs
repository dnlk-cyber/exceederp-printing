using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.CRM
{
    public class TaxTransaction : TrackUserOperation
    {
        public int TaxTransactionID { get; set; }
        public string Voucher { get; set; }
        public string TaxType { get; set; }
        public double TaxableAmount { get; set; }
        public double TaxAmount { get; set; }

    }
}
