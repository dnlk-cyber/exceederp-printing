using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// 
    /// <summary>    Termination. </summary>
    ///
    /// <remarks>  First  Mekanehiwot, 10/31/2016. </remarks>
    /// <remarks>  last   Kibrom, 10/31/2022 to Feb, 2026. </remarks>
    /// <remarks>  last   Kibrom, Sept 05, 2026 </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Termination: DepartmentOperations 
    {
        [Key]
        public int TerminationId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Termination_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int? OrgStructureId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int? BusinessUnitId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int? BranchId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Termination_TerminationType", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        public virtual Lookup Lookup { get; set; }
        [Display(Name = "HR_EmployeeDetail_Termination_Reason", ResourceType = typeof(Localization.Resources))]
        public string Reason { get; set; }
        [Display(Name = "HR_EmployeeDetail_Termination_ApplicationDate", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime ApplicationDate { get; set; }
        [Display(Name = "HR_EmployeeDetail_Termination_ApplicationDate", ResourceType = typeof(Localization.Resources))]
        public string ApplicationDateAm { get; set; }
        [Display(Name = "HR_EmployeeDetail_Termination_TerminationDate", ResourceType = typeof(Localization.Resources))]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? TerminationDate { get; set; }
        [Display(Name = "HR_EmployeeDetail_Termination_TerminationDate", ResourceType = typeof(Localization.Resources))]
        public string TerminationDateAm { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        public bool IsForOther { get; set; }
        [Display(Name = "Is Forcibly Terminated ")]
        public bool IsForciblyTerminated { get; set; }
        //to save status
        [Display(Name = "IsApplied")]
        public bool IsApplied { get; set; }
        /// <summary>
        /// Indicates whether the employee is applying for early retirement.
        /// Normal retirement age = 60.
        /// Early retirement is allowed only if employee age >= 55.
        /// </summary>
        [Display(Name = "Is Early Retirement")]
        public bool IsEarlyRetirement { get; set; }

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
        // Tier 1: Project HR Admin
        [Display(Name = "Is Project HR Approved")]
        public bool IsProjectHRApproved { get; set; }
        public string ProjectHRApprovedBy { get; set; }
        public DateTime? ProjectHRApprovedDate { get; set; }

        // Tier 2: Project Manager (PM)
        [Display(Name = "Is PM Approved")]
        public bool IsPMApproved { get; set; }
        public string PMApprovedBy { get; set; }
        public DateTime? PMApprovedDate { get; set; }

        // Optional Config Role Approval Step (e.g., Dail / DGM)
        public bool IsAdditionalApproved { get; set; }
        public string AdditionalApprovedBy { get; set; }
        public DateTime? AdditionalApprovedDate { get; set; }

        [NotMapped]
        public bool IsApproved { get; set; }
        [NotMapped]
        public bool IsActive { get; set; }
        [NotMapped]
        public bool IsAgreeRetunSalary { get; set; }

        // Optional Step 3: Department Head (Head Office)
        [Display(Name = "Is Department Head Approved")]
        public bool IsDepartmentHeadApproved { get; set; }
        public string DepartmentHeadApprovedBy { get; set; }
        public DateTime? DepartmentHeadApprovedDate { get; set; }

        // Optional Step 4: HR Manager (Head Office)
        [Display(Name = "Is HR Manager Approved")]
        public bool IsHRManagerApproved { get; set; }
        public string HRManagerApprovedBy { get; set; }
        public DateTime? HRManagerApprovedDate { get; set; }

        

    }

    public class TerminationPMValidation : VoucherValidation
    {
        public int TerminationPMValidationId { get; set; }
        public int TerminationId { get; set; }
        public Termination Termination { get; set; }

    }

    public class TerminationHRManagerValidation : VoucherValidation
    {
        public int TerminationHRManagerValidationId { get; set; }

        public int TerminationId { get; set; }

        public Termination Termination { get; set; }


    }
}