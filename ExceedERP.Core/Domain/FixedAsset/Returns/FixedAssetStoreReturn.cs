using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
//using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Core.Domain.FixedAsset.FixedAsset
{
    public class FixedAssetStoreReturn : Operations
    {
        public int FixedAssetStoreReturnId { get; set; }

        public string Reference { get; set; }
        [Required]
        [Display(Name = "Store")]
        public string StoreCode { get; set; }
        [Display(Name = "Return Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime ReturnDate { get; set; }
        [Required]
        [Display(Name = "Reason")]
        public string Reason { get; set; }
        [Required]
        [DisplayName("Work Unit")]
        public int OrgStructureId { get; set; }
        [Display(Name = "Period")]
        public int GlPeriodId { get; set; }
        public virtual ICollection<FixedAssetStoreReturnItem> FixedAssetStoreReturnItems { get; set; }
        public virtual ICollection<FixedAssetStoreReturnValidation> FixedAssetStoreReturnValidation { get; set; }

    }
    public class FixedAssetStoreReturnItem : TrackUserOperation
    {
        public int FixedAssetStoreReturnItemId { get; set; }

        public int FixedAssetStoreReturnId { get; set; }
        public virtual FixedAssetStoreReturn FixedAssetStoreReturn { get; set; }
        [DisplayName("Category")]
        public int FixedAssetCategoryId { get; set; }

        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryId { get; set; }
        [DisplayName("Asset")]
        public int AssetRegistrationId { get; set; }
        public virtual AssetRegistration AssetRegistration { get; set; }
        public string Size { get; set; }
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Tag No.")]
        public string TagNo { get; set; }
        [DisplayName("QTY")]
        public int Quantity { get; set; }
    }
    public class FixedAssetStoreReturnValidation : VoucherValidation
    {
        public int FixedAssetStoreReturnValidationId { get; set; }

        public int FixedAssetStoreReturnId { get; set; }
        public virtual FixedAssetStoreReturn FixedAssetStoreReturn { get; set; }
    }
    
}