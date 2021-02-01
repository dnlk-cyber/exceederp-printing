using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
	/// <summary>   A commitment. </summary>
	///Used to Store Employees Commitment
	/// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public class Commitment:TrackUserOperation 
	{
	  
		[Key]
		public int CommitmentId { get; set; }
		[Display(Name = "Common_PersonEmployee_EmployeeCode", ResourceType = typeof(Localization.Resources))]
		public string EmployeeCode { get; set; }
		[Required]
		[Display(Name = "HR_EmployeeDetail_Commitment_PersonEmployee", ResourceType = typeof(Localization.Resources))]
		public int EmployeeId { get; set; }
		public virtual Person_Employee Employee { get; set; }
	 
		[Required ]
		[Display(Name = "HR_EmployeeDetail_Commitment_Reason", ResourceType = typeof(Localization.Resources))]
		public string Reason { get; set; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		[Display(Name = "HR_EmployeeDetail_Commitment_From", ResourceType = typeof(Localization.Resources))]
		public DateTime From { get; set; }
		[Display(Name = "HR_EmployeeDetail_Commitment_From", ResourceType = typeof(Localization.Resources))]
		[Required]
		public string FromCometAm { get; set; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		[Display(Name = "HR_EmployeeDetail_Commitment_To", ResourceType = typeof(Localization.Resources))]
		public DateTime To { get; set; }
		[Display(Name = "HR_EmployeeDetail_Commitment_To", ResourceType = typeof(Localization.Resources))]
		[Required ]
		public string ToCometAm { get; set; }
		[Required]
		[Display(Name = "HR_EmployeeDetail_Commitment_CommitmentType", ResourceType = typeof(Localization.Resources))]
		public int   LookupId { get; set; }
		public virtual Lookup Lookup { get; set; }
		[Required]
		[Display(Name = "HR_EmployeeDetail_Commitment_Value", ResourceType = typeof(Localization.Resources))]
		public decimal Value { get; set; }
		[Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
		public string Remark { get; set; }
		public bool IsActive { get; set; }
	}
}