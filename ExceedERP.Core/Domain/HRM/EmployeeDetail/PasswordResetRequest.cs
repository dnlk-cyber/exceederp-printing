using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>    Password Reset Request . </summary>
    ///
    /// <remarks>   Kibrom Abera, 09/01/2025. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class PasswordResetRequest : DepartmentNewOperations
    {
        [Key]
        public int PasswordResetRequestId { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int? OrgStructureId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int? LookupId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int? BusinessUnitId { get; set; }
        public string UserName { get; set; } // FK to your User table
        public string Email { get; set; }
        public string Token { get; set; } // Secure token for reset
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Display(Name = "Requested At")]
        public DateTime RequestedAt { get; set; } = DateTime.UtcNow;
        [Display(Name = "Requested At")]
        public string RequestedAtAm { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime Expiration { get; set; } // Expiry time for the reset link
        public string ExpirationAm { get; set; }
        [Display(Name = "Reset Type")]
        public string ResetType { get; set; }
        public bool IsUsed { get; set; } = false; // To track if token already used


    }

}