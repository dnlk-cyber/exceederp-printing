using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public class CategoryDepreciationValue : TrackUserSettingOperation
    {
        public int CategoryDepreciationValueId { get; set; }
        [Display(Name = "Category")]
        public int FixedAssetCategoryID { get; set; }
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }
        [DisplayName("Amount")]
        public decimal DepreciationAmount { get; set; }

    }
}