using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
	/// <summary>   A competency. </summary>
	///Used to Store Employees Comptency
	/// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
	/// <remarks>   Modified by Kibrom Abera, 10/12/202023. </remarks>
	////////////////////////////////////////////////////////////////////////////////////////////////////

	public class Competency:TrackUserOperation 
	{
		[Key]
		public int CompetencyId { get; set; }
		[Required]
		[Display(Name = "HR_EmployeeDetail_Competency_EmployeeId", ResourceType = typeof(Localization.Resources))]
		public int EmployeeId { get; set; }
		public virtual Person_Employee Employee { get; set; }
		[Required]
		[Display(Name = "HR_EmployeeDetail_Competency_SkillType", ResourceType = typeof(Localization.Resources))]
	   
		public int LookupId { get; set; }
		public virtual Lookup Lookup { get; set; }
		[Display(Name = "HR_EmployeeDetail_Competency_Level", ResourceType = typeof(Localization.Resources))]
		public string Level { get; set; }
		[Display(Name = "HR_EmployeeDetail_Competency_Status", ResourceType = typeof(Localization.Resources))]
		public string Status { get; set; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		[Display(Name = "HR_EmployeeDetail_Competency_From", ResourceType = typeof(Localization.Resources))]
		public DateTime From { get; set; }
		[Display(Name = "HR_EmployeeDetail_Competency_From", ResourceType = typeof(Localization.Resources))]
		public string FromComAm { get; set; }
		[DataType(DataType.Date)]
		[DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
		[Display(Name = "HR_EmployeeDetail_Competency_To", ResourceType = typeof(Localization.Resources))]
		public DateTime To { get; set; }
		[Display(Name = "HR_EmployeeDetail_Competency_To", ResourceType = typeof(Localization.Resources))]
		public string ToCompAm { get; set; }
		[Display(Name = "HR_TrainingEducation_CourseTracking_Organization", ResourceType = typeof(Localization.Resources))]
		public string Organization { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
	}
}