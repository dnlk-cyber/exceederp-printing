using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.FundTransfer
{
    public class FundTransferValidation : VoucherValidation
    {
        public int FundTransferValidationId { get; set; }
        public int FundTransferId { get; set; }
        public FundTransfer FundTransfer { get; set; }

    }

    public class FundTransferDepartmentValidation : VoucherValidation
    {
        public int FundTransferDepartmentValidationId { get; set; }
        public int FundTransferId { get; set; }
        public FundTransfer FundTransfer { get; set; }

    }

    public class FundTransferBudgetValidation : VoucherValidation
    {
        public int FundTransferBudgetValidationId { get; set; }
        public int FundTransferId { get; set; }
        public FundTransfer FundTransfer { get; set; }
    }
}
