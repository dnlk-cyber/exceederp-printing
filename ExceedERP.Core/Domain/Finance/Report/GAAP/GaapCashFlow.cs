using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.Report.GAAP
{
    public class GaapCashFlow
    {
        public int GaapCashFlowId { get; set; }
        public virtual ICollection<GaapCashFlowReceipt> Receipts { get; set; }
        public virtual ICollection<GaapCashFlowPayOut> PayOuts { get; set; }
    }

    public class GaapCashFlowReceipt
    {
        public int GaapCashFlowReceiptId { get; set; }
        public int GaapCashFlowId { get; set; }
        public virtual GaapCashFlow GaapCashFlow { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<GaapCashFlowReceiptRange> GaapCashFlowReceiptRanges { get; set; }
    }

    public class GaapCashFlowReceiptRange : ReportSetupBase
    {
        public int GaapCashFlowReceiptRangeId { get; set; }
        public int GaapCashFlowReceiptId { get; set; }
        public virtual GaapCashFlowReceipt GaapCashFlowReceipt { get; set; }
    }

    public class GaapCashFlowPayOut
    {
        public int GaapCashFlowPayOutId { get; set; }
        public int GaapCashFlowId { get; set; }
        public virtual GaapCashFlow GaapCashFlow { get; set; }
        [Required]
        public string Description { get; set; }
        [Display(Name = "Note Number")]
        public string NoteNumber { get; set; }
        public virtual ICollection<GaapCashFlowPayOutRange> GaapCashFlowPayOutRanges { get; set; }
    }

    public class GaapCashFlowPayOutRange : ReportSetupBase
    {
        public int GaapCashFlowPayOutRangeId { get; set; }
        public int GaapCashFlowPayOutId { get; set; }
        public virtual GaapCashFlowPayOut GaapCashFlowPayOut { get; set; }
    }


}
