using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
	/// <summary>   A disability. </summary>
	///
	/// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public class Disability:TrackUserOperation 
	{
		[Key]
		public int DisabilityId { get; set; }
		[Required ]
		[Display(Name = "HR_EmployeeDetail_Disability_PersonEmployee", ResourceType = typeof(Localization.Resources))]
		public int EmployeeId { get; set; }
		public virtual Person_Employee Employee { get; set; }
		[Required ]
		[Display(Name = "HR_EmployeeDetail_Disability_Type", ResourceType = typeof(Localization.Resources))]
		
		public string Type { get; set; }
		[Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
		public string Remark { get; set; }
	}
}