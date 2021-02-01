using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{
    public class MonthlyDepreciation : Operations
    {
        public int MonthlyDepreciationId { get; set; }
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Period")]public int GLPeriodId { get; set; }
        [Display(Name = "Branch/Project")]
        public int BranchId { get; set; }
        public virtual ICollection<MonthlyDepreciationDistribution> MonthlyDepreciationDistributions { get; set; }
        public virtual ICollection<MonthlyDepreciationItem> MonthlyDepreciationItems { get; set; }
        public virtual ICollection<MonthlyDepreciationValidation> MonthlyDepreciationValidations { get; set; }
    }
}
