using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.FundTransfer
{
    public class FundTransferDetail : TrackUserSettingOperation
    {
        public int FundTransferDetailId { get; set; }
        public int FundTransferId { get; set; }
        public FundTransfer FundTransfer { get; set; }
        public string Description { get; set; }
        public decimal Requested { get; set; }
        public decimal Approved { get; set; }
        public string Remark { get; set; }
    }
}
