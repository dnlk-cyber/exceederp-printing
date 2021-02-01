using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ArStandardCollection : Operations
    {
        public int ArStandardCollectionId { get; set; }
        [Required]
        [Display(Name = "Receipt Number")]
        public string ReceiptNumber { get; set; }
        [Display(Name = "Line Amount")]
        public decimal LineAmount { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        [Required]
        [Display(Name = "Receipt Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiptDate { get; set; }
        [Required]
        [Display(Name = "Receipt Method ")]
        public string ReceiptMethod { get; set; }
        [Display(Name = "Customer ID")]
        public string CustomerId { get; set; }
        [Display(Name = "New Customer")]
        public bool IsNewCustomer { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }//for cash sales 
        [Display(Name = "Unidentified")]
        public decimal UnidentifiedBalance { get; set; }
        [Display(Name = "Applied")]
        public decimal AppliedBalance { get; set; }
        [Display(Name = "On Account")]
        public decimal OnAccountBalance { get; set; }
        [Display(Name = "Unapplied")]
        public decimal UnappliedBalance { get; set; }
        [Display(Name = "Prepayment")]
        public decimal PrepaymentBalance { get; set; }
        public string Reference { get; set; }
        public bool IsPostedToSecondLedger { get; set; }

        public virtual ICollection<ArStandardCollectionLines> ARStandardCollectionLines { get; set; }
        public virtual ICollection<ArStandardCollectionDistribution> ARStandardCollectionDistribution { get; set; }
        public virtual ICollection<ArStandardCollectionValidation> ARStandardCollectionValidation { get; set; }
    }
}
