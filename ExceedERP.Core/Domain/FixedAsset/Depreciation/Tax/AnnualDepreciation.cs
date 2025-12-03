using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{

	public enum DepreciationStatus
	{		
		Processing,
		Reversed,
        Closed

    }

	public class AnnualDepreciation : Operations
	{
		public int AnnualDepreciationId { get; set; }
		[Display(Name = "Fiscal Year")]
		public int GlFiscalYearId { get; set; }
		public int? GlPeriodId { get; set; }		
		public virtual ICollection<AnnualDepreciationDistribution> AnnualDepreciationDistributions { get; set; }
		public virtual ICollection<AnnualDepreciationItem> AnnualDepreciationItems { get; set; }
		public virtual ICollection<AnnualDepreciationValidation> AnnualDepreciationValidations { get; set; }
	}
}
