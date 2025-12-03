using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class IfrsDepreciationItem : TrackUserSettingOperation
    {
        public int IfrsDepreciationItemId { get; set; }
        public int IfrsDepreciationId { get; set; }
        public virtual IfrsDepreciation IfrsDepreciation { get; set; }
        [Display(Name = "Asset")]
        public int AssetRegistrationId { get; set; }
        public virtual AssetRegistration AssetRegistration { get; set; }
        [DisplayName("Amount")]
        public decimal DepreciationAmount { get; set; }
        [DisplayName("Accumulated Dep. Amount")]
        public decimal AccumulatedDepreciationAmount { get; set; }
        [Display(Name = "Book Value")]
        public decimal BookValue { get; set; }
        public int BranchId { get; set; }//for distribution
        public int BusinessUnitId { get; set; }//for distribution
    }
}