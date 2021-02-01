using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR
{
    public class ARMiscelaneousCollection : Operations
    {
        public int ARMiscelaneousCollectionID { get; set; }
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "New Customer")]
        public bool IsNewCustomer { get; set; }
        [Display(Name = "Customer Name")]
        public string CustomerName { get; set; }
        [Required]
        [Display(Name = "Receipt Number")]
        public string ReceiptNumber { get; set; }
        [Display(Name = "Line Amount")]
        public decimal LineAmount { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Receipt Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiptDate { get; set; }
        [Required]
        [Display(Name = "Receipt Method ")]
        public string ReceiptMethod { get; set; }
        [DisplayName("Branch or Project")]
        public int BranchId { get; set; }
        [Display(Name = "Is Direct Collection")]
        public bool IsDirectCollection { get; set; }
        public bool IsPostedToSecondLedger { get; set; }
        public virtual ICollection<ArMiscelaneousCollectionLine> ArMiscelaneousCollectionLines { get; set; }
        public virtual ICollection<ARMiscelaneousCollectionDistribution> ARMiscelaneousCollectionDistribution { get; set; }
        public virtual ICollection<ARMiscelaneousCollectionValidation> ARMiscelaneousCollectionValidation { get; set; }
    }

    public class ArMiscelaneousCollectionLine : TrackUserSettingOperation
    {
        public int ArMiscelaneousCollectionLineId { get; set; }
        public int ARMiscelaneousCollectionID { get; set; }
        public virtual ARMiscelaneousCollection ARMiscelaneousCollection { get; set; }
        [Required]
        [Display(Name = "Activity")]
        public string Activity { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        [Display(Name = "Revenue Account")]
        public string RevenueAccount { get; set; }
        [Display(Name = "Description")]
        public string RevenueAccountDesc { get; set; }
        [Required]
        [Display(Name = "Tax Type")]
        public string TaxType { get; set; }
        [Required]
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Required]
        [Display(Name = "Withholding Type")]
        public string WithholdingType { get; set; }
        [Display(Name = "Withholding Amount")]
        public decimal WithholdingAmount { get; set; }
        [Display(Name = "Withhold Tax(VAT)")]
        public bool WithholdTax { get; set; }
        [Display(Name = "Withhold Tax (VAT)")]
        public decimal WithholdTaxAmount { get; set; }
        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }
        [Display(Name = "Net Receipt")]
        public decimal NetReceipt { get; set; }
        public string Remark { get; set; }
    }
}
