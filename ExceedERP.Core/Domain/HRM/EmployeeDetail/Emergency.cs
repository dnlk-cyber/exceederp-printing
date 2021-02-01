using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
	public class Emergency :TrackUserOperation 
	{
		public int EmergencyId { get; set; }
		[Required ]
		[Display(Name = "HR_EmployeeDetail_Emergency_PersonEmployee", ResourceType = typeof(Localization.Resources))]
		public int EmployeeId { get; set; }
		public virtual Person_Employee Employee { get; set; }
		[Display(Name = "HR_EmployeeDetail_Emergency_Name", ResourceType = typeof(Localization.Resources))]
		public string Name { get; set; }
		[Display(Name = "HR_EmployeeDetail_Emergency_Relationship", ResourceType = typeof(Localization.Resources))]
		 
		public int LookupId { get; set; }
		public virtual Lookup Lookup { get; set; }
		[Display(Name = "HR_EmployeeDetail_Emergency_HomePhoneNo", ResourceType = typeof(Localization.Resources))]
		//[RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid home phone no")]
		public string HomePhoneNo { get; set; }

		[Display(Name = "HR_EmployeeDetail_Emergency_MobilePhoneNo", ResourceType = typeof(Localization.Resources))]
		//[RegularExpression (@"^[0-9]+$",ErrorMessage = "Please enter a valid mobile phone no")]
		public string MobilePhoneNo { get; set; }
		[Display(Name = "HR_EmployeeDetail_Emergency_WorkPhoneno", ResourceType = typeof(Localization.Resources))]
		//[RegularExpression(@"^[0-9]+$", ErrorMessage = "Please enter a valid work phone no")]
		
		public string WorkPhoneno { get; set; }
		[Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
		public string Remark { get; set; }
	}
}