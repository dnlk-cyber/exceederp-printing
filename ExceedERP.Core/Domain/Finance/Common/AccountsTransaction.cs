using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.Common
{
    //not used
    public class AccountsTransaction : TrackUserOperation
    {
        public int AccountsTransactionID { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public decimal Debit { get; set; }
        public decimal Credit { get; set; }
        public decimal Balance { get; set; }
        public string Type { get; set; }
        public string Currency { get; set; }
        //module or class
        public string Source { get; set; }
        public int SourceID { get; set; }
    }
}
