using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.Common
{
    public class FinInStatementRecord : TrackUserOperation
    {
        public int FinInStatementRecordID { get; set; }
        public string FYear { get; set; }
        public string Status { get; set; }
    }
}
