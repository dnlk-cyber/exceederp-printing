using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FixedAsset.Depreciation
{

	

	public class IfrsDepreciation : Operations
	{
		public int IfrsDepreciationId { get; set; }
		[Display(Name = "Fiscal Year")]
		public int GlFiscalYearId { get; set; }
		public int? GlPeriodId { get; set; }		
		public virtual ICollection<IfrsDepreciationDistribution> IfrsDepreciationDistributions { get; set; }
		public virtual ICollection<IfrsDepreciationItem> IfrsDepreciationItems { get; set; }
		public virtual ICollection<IfrsDepreciationValidation> IfrsDepreciationValidations { get; set; }
	}
}
