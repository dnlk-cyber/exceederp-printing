using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Setting;

namespace ExceedERP.Core.Domain.FixedAsset.Registrations
{
   public enum DisposalType
    {
      Donation,
      Sale,
      Burning
    }

   public class Disposal : Operations
   {
       public int DisposalId { get; set; }
       [Required]
       public int AssetRegistrationId { get; set; }
       [Display(Name = "Fixed Asset")]
       public virtual AssetRegistration AssetRegistration { get; set; }
       [Required]
       public string Description { get; set; }
   }

   public class AssetDisposal : Operations
    {
        public int AssetDisposalId { get; set; }
        [Required]
        [Display(Name = "Customer")]
        public string CustomerId { get; set; }
        [Display(Name = "Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Date { get; set; }
        [Display(Name = "Disposal Type")]
        public DisposalType DisposalType { get; set; }
        [Required]
        public string Description { get; set; }
        public virtual ICollection<DisposalItem> DisposalItems { get; set; }
        public virtual ICollection<AssetDisposalValidation> AssetDisposalValidations { get; set; }
    }

    public class DisposalItem : TrackUserOperation
    {
        public int DisposalItemId { get; set; }
        public int AssetDisposalId { get; set; }
        public virtual AssetDisposal AssetDisposal { get; set; }
        [Required]
        [DisplayName("Category")]
        public int FixedAssetCategoryID { get; set; }
        public virtual FixedAssetCategory FixedAssetCategory { get; set; }
        [Required]
        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryID { get; set; }
        public virtual FixedAssetSubCategory FixedAssetSubCategory { get; set; }
        [Required]
        [DisplayName("Asset")]
        public int AssetRegistrationId { get; set; }
        [Display(Name = "Book Value")]
        public decimal BookValue { get; set; }
        [Display(Name = "Sales Price")]
        public decimal SalesPrice { get; set; }
        [DisplayName("Tax Type")]
        public int GlTaxRateId { get; set; }
        [Display(Name = "Tax Amount")]
        public decimal TaxAmount { get; set; }
        [Display(Name = "Total")]
        public decimal Total { get; set; }
    }

    public class AssetDisposalValidation : TrackUserSettingOperation
    {
        public int AssetDisposalValidationId { get; set; }
        public int AssetDisposalId { get; set; }
        public virtual AssetDisposal AssetDisposal { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
