using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>   An experience. </summary>
    ///Used to Store Employes Expriance 
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Experience
    {
        [Key ]
        public int ExperienceId { get; set; }
  
        [Display(Name = "Common_PersonEmployee_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Experience_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_Organozation", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string Organozation { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_OrgFullAddress", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string OrgFullAddress { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Experience_EmploymentType", ResourceType = typeof(Localization.Resources))]
        public EmployeeStatus  EmploymentType { get; set; }
	  
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_EmployeeDetail_Experience_From", ResourceType = typeof(Localization.Resources))]
        public DateTime From { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_From", ResourceType = typeof(Localization.Resources))]
        public string FromExAmharic { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_EmployeeDetail_Experience_To", ResourceType = typeof(Localization.Resources))]
        public DateTime To { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_To", ResourceType = typeof(Localization.Resources))]
        public string  ToExAmharic { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_TotalDuration", ResourceType = typeof(Localization.Resources))]
        public decimal TotalDuration { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_TotalDuration", ResourceType = typeof(Localization.Resources))]
        public string TotalExp { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_Responsibility", ResourceType = typeof(Localization.Resources))]
        public string Responsibility { get; set; }
	
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_EmployeeDetail_Experience_ClearanceDateTime", ResourceType = typeof(Localization.Resources))]
        public DateTime ClearanceDateTime { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_ClearanceDateTime", ResourceType = typeof(Localization.Resources))]
        public string ClearanceDateTimeAm { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_CleranceReason", ResourceType = typeof(Localization.Resources))]
        public string CleranceReason { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_IncomeTaxPaymentRefNo", ResourceType = typeof(Localization.Resources))]
        public string IncomeTaxPaymentRefNo { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_Salary", ResourceType = typeof(Localization.Resources))]
        public decimal Salary { get; set; }
        [Display(Name = "HR_EmployeeDetail_Experience_IsPrivate", ResourceType = typeof(Localization.Resources))]
        public bool IsPrivate { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
}