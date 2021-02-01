using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public class FixedAssetCondition : TrackUserSettingOperation
    {
        public int FixedAssetConditionId { get; set; }
        [Required]
        [Display(Name = "Name")]
        public string Name { get; set; }
        public string Description { get; set; }
    }
}