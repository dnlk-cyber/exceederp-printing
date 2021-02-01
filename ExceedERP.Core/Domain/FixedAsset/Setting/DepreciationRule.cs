using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FixedAsset.Setting
{
    public class DepreciationRule
    {
        public int DepreciationRuleId { get; set;  }
        [Required]
        [DisplayName("Category")]
        public string CategoryCode { get; set; }
        public string AccountCode { get; set;  }
        public string DepreciationCode { get; set; }
        public string ClearingAccount { get; set;  }
        [DisplayName("Accumulated Account Code")]
        public string AccumulatedAccount { get; set; }
        [DisplayName("Fixed Asset Group")]
        public int FixedAssetGroupID { get; set; }
        public virtual FixedAssetGroup FixedAssetGroup { get; set; }

    }
}
