using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public class FixedAssetClearingAccountSetup : TrackUserSettingOperation
    {
        public int FixedAssetClearingAccountSetupID { get; set; }
        [Required]
        [Display(Name = "Account Code")]
        public string AccountCode { get; set; }
    }
}