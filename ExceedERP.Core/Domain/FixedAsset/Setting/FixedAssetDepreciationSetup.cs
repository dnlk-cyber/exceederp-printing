using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public class FixedAssetDepreciationSetup : TrackUserSettingOperation
    {
        public int FixedAssetDepreciationSetupID { get; set; }
        [Required]
        [Display(Name = "Fixed Asset Account Code")]
        public string FixedAssetAccountCode { get; set; }
        [Required]
        [Display(Name = "Description")]
        public string FixedAssetAccountCodeDescription { get; set; }
        [Required]
        [Display(Name = "AnnualDepreciation Account Code")]
        public string DepreciationAccountCode { get; set; }
        [Required]
        [Display(Name = "Accumulated AnnualDepreciation Account Code")]
        public string AccumulatedDepreciationAccountCode { get; set; }

        public string FullAccountCode
        {
            get { return FixedAssetAccountCode + "_" + FixedAssetAccountCodeDescription; }
        }
    }
}