using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Finance.GL;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AP.PaymentRequest
{
   public class PaymentRequestLine: TrackUserSettingOperation
    {
        public int PaymentRequestLineId { get; set; }
        public int PaymentRequestId { get; set; }
        public PaymentRequest PaymentRequest { get; set; }
        [Display(Name = "Tax")]
        public int GLTaxRateID { get; set; }
        public GLTaxRate GLTaxRate { get; set; }       
        [Required]
        public string ItemName { get; set; }
        public string Description { get; set; }
        public decimal Amount { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public string Remark { get; set; }
    }
}
