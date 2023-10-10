using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Procurement.Need
{
    public class AnnualNeedAssesmentValidation : TrackUserOperation
    {
        public int AnnualNeedAssesmentValidationID { get; set; }
        public int AnnualNeedAssesmentID { get; set; }
        public virtual AnnualNeedAssesment AnnualNeedAssesment { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
