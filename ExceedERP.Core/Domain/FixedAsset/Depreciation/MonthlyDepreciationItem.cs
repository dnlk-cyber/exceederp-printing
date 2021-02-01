using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class MonthlyDepreciationItem : TrackUserSettingOperation
    {
        public int MonthlyDepreciationItemId { get; set; }
        public int MonthlyDepreciationId { get; set; }
        public virtual MonthlyDepreciation MonthlyDepreciation { get; set; }
        [Display(Name = "Asset")]
        public int AssetRegistrationId { get; set; }
        public virtual AssetRegistration AssetRegistration { get; set; }
        [DisplayName("Amount")]
        public decimal DepreciationAmount { get; set; }
        [Display(Name = "Book Value")]
        public decimal BookValue { get; set; }
    }
}