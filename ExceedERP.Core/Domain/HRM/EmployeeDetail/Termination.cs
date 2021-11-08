using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>   A termination. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Termination:DepartmentOperations 
    {
        [Key]
        public int TerminationId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Termination_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Termination_TerminationType", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Display(Name = "HR_EmployeeDetail_Termination_Reason", ResourceType = typeof(Localization.Resources))]
        public string Reason { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Termination_ApplicationDate", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ApplicationDate { get; set; }
        [Display(Name = "HR_EmployeeDetail_Termination_ApplicationDate", ResourceType = typeof(Localization.Resources))]
        public string ApplicationDateAm { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Termination_TerminationDate", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime TerminationDate { get; set; }
        [Display(Name = "HR_EmployeeDetail_Termination_TerminationDate", ResourceType = typeof(Localization.Resources))]
        public string TerminationDateAm { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        public bool IsForOther { get; set; }
        /// <summary>
        /// IsDirect Field is added to check if the termination is direct or with request 
        /// if it is from request all the button are hidden
        /// if it is direct the button being hide after approval 
        /// </summary>
        public bool IsDirect { get; set; }

        /// <summary>
        /// //this field is added to check the employee active or not , if he is 'InActive' we will hide 'Force Terminate' Button
        /// </summary>
        /// 
        [NotMapped]
        public bool IsActive { get; set; }

    }
}