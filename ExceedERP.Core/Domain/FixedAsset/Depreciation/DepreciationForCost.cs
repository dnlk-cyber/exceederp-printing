using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FixedAsset.Registrations;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class DepreciationForCost : TrackUserOperation
    {
        public int DepreciationForCostId { get; set; }
        [Required]
        [Display(Name = "Asset")]
        public int AssetRegistrationId { get; set; }
        public virtual AssetRegistration AssetRegistration { get; set; }
        [DisplayName("Amount")]
        public decimal DepreciationAmount { get; set; }
        [Display(Name = "Book Value")]
        public decimal BookValue { get; set; }
        [Display(Name = "Period")]
        public int GLPeriodId { get; set; }
        [Display(Name = "Branch/Project")]
        public int BranchId { get; set; }
        public virtual ICollection<DepreciationForCostDistribution> DepreciationForCostDistributions { get; set; }
    }
}
