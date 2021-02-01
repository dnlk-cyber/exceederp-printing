using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Receives
{
    public class FixedAssetReceiveDonation : Operations
    {
        public int FixedAssetReceiveDonationId { get; set; }


        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Display(Name = "Donated By")]
        public string SupplierName { get; set; }
        [Display(Name = "Supplier Invoice No.")]
        public string SupplierInvoiceNo { get; set; }
        [Display(Name = "Fiscal Year")]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FiscalYear { get; set; }
        [Display(Name = "Period")]
        public string Period { get; set; }
        [Display(Name = "Receive Reference")]
        public string Reference { get; set; }
        [Display(Name = "Receive Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReceiveDate { get; set; }
        public virtual ICollection<FixedAssetInspection> Inspections { get; set; }
        public virtual ICollection<FixedAssetReceiveDonationItem> FixedAssetReceiveDonationItemsItem { get; set; }
        public virtual ICollection<FixedAssetReceiveDonationValidation> FixedAssetReceivedDonationValidations { get; set; }
        public virtual ICollection<FixedAssetReceiveDonationDistribution> FixedAssetReceiveDonationDistributions { get; set; }
    }
}