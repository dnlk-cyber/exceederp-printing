using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AP
{
    public class APPaybleInvoicePrepayment : TrackUserOperation
    {
        public int APPaybleInvoicePrepaymentId { get; set; }
        public int ApPaybleInvoicesId { get; set; }
        public virtual ApPaybleInvoices APPaybleInvoices { get; set; }       
        [Display(Name = "Payment Id")]
        public int PaymentId { get; set; }
        public decimal PrepaymentAmount { get; set; }
        public bool Apply { get; set; }
        public decimal AppliedPrepaymentAmount { get; set; }

    }
}
