using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AP
{

    public class ApTPayment : NewOperations
    {
        public int ApTPaymentId { get; set; }
        [Display(Name = "Payment Number")]
        public int PaymentNum { get; set; }
        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }       
        [Display(Name = "Supplier Name")]
        public string SupplierName { get; set; }     
        [Required]
        [Display(Name = "Payment Type")]
        public PaymentSource PaymentType { get; set; }
        [Display(Name = "Payment Reference")]
        public string PaymentReference { get; set; }
        [Display(Name = "Payment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime PaymentDate { get; set; }
        [Display(Name = "Telebirr Code")]
        public string TCode { get; set; }
        [Display(Name = "Telebirr Account")]
        public string TAccount { get; set; }
        public decimal Amount { get; set; }

        public virtual ICollection<ApTPaymentValidation> ApTPaymentValidations { get; set; }
        public virtual ICollection<ApTPaymentRequestResponse> ApTPaymentRequestResponses { get; set; }
    }

    public class ApTPaymentValidation : VoucherValidation
    {
        public int ApTPaymentValidationId { get; set; }
        public int ApTPaymentId { get; set; }
        public virtual ApTPayment ApTPayment { get; set; }
    }

    public class ApTPaymentRequestResponse : TrackUserSettingOperation
    {
        public int ApTPaymentRequestResponseId { get; set; }
        public string Request { get; set; }
        public string Response { get; set; }
        public string Status { get; set; }
        public int ApTPaymentId { get; set; }
        public virtual ApTPayment ApTPayment { get; set; }

    }

    public enum PaymentSource
    {
        Payment,
        DirectPayment,
        AdvancePayment,
        MiscPayment,
    }


}
