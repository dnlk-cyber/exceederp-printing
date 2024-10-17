using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;

namespace ExceedERP.Core.Domain.FixedAsset.Counts
{
    public class FixedAssetCountItem: TrackUserSettingOperation
    {
        public int FixedAssetCountItemId { get; set; }
        public int FixedAssetCountId { get; set; }
        public virtual FixedAssetCount FixedAssetCount { get; set; }
        [DisplayName("Category")]
        public int FixedAssetCategoryId { get; set; }

        [DisplayName("Sub Category")]
        public int FixedAssetSubCategoryId { get; set; }

        [DisplayName("Asset")]
        public int AssetRegistrationId { get; set; }
        [DisplayName("Book Value")]
        public decimal Value { get; set; }
        [DisplayName("Qty.")]
        public decimal PreviousBalance { get; set; }
        [DisplayName("Count")]
        public decimal Count { get; set; }
        [DisplayName("Status")]
        public bool Status { get; set; }
        public bool Updated { get; set; }       
        [DisplayName("Asset Status")]
        public RegistrationStatus AssetStatus { get; set; }
        [DisplayName("Employee")]
        public int? EmployeeId { get; set; }
        [DisplayName("Store")]
        public string StoreCode { get; set; }
        public string PartNo { get; set; }
        [Display(Name = "Model No.")]
        public string ModelNo { get; set; }
        [Display(Name = "Serial No.")]
        public string SerialNo { get; set; }
        [Display(Name = "Engine No.")]
        public string EngineNo { get; set; }
        [Display(Name = "Chasis No.")]
        public string ChasisNo { get; set; }
        [Display(Name = "Plate No.")]
        public string PlateNo { get; set; }

    }
}