using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>   A placement. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    /// <remarks>   Kibrom Abera , 02/31/2023-12/17/2025. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Placement : TrackUserOperation
    {
    
        [Key]
        public int PlacementId { get; set; }
        [Display(Name = "Placement_immediate_Supervisor", ResourceType = typeof(Localization.Resources))]
        public int? ImmediateSupervisor { get; set; }
        [Display(Name = "Common_PersonEmployee_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployeeID", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_EmployeeDetail_Placement_AssignmentDate", ResourceType = typeof(Localization.Resources))]
        public DateTime AssignmentDate { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_AssignmentDate", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string AssignmentDateAmharic { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Salary Change Date")]
        public DateTime? SalarChangeDate { get; set; }
        [Display(Name = "Salary Change Date (Geez)")]
        public string SalarChangeDateAmharic { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Next Salary Change Date")]
        public DateTime? NextSalaryChangeDate { get; set; }
        [Display(Name = "Next Salary Change Date (Geez)")]
        public string NextSalaryChangeDateAmharic { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
       // public virtual Lookup Lookup { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_ParentOrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int? ParentOrgStructureId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public string JobTitleName { get; set; }
        public int? MappingId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_Rank", ResourceType = typeof(Localization.Resources))]
        public int RankId { get; set; }

        //public virtual Rank Rankrel { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Rank", ResourceType = typeof(Localization.Resources))]
        public string RankName { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Step", ResourceType = typeof(Localization.Resources))]
        public string StepName { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_Step", ResourceType = typeof(Localization.Resources))]
        public int StepId { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "HR_EmployeeDetail_Placement_Salary", ResourceType = typeof(Localization.Resources))]
        public decimal Salary { get; set; }
        [Display(Name = "HR_SalaryStructure_SalaryJobTitle_SalaryStructureVersionId", ResourceType = typeof(Localization.Resources))]
        public int? SalaryStructureVersionId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_AssignmentType", ResourceType = typeof(Localization.Resources))]
        public AssignmentType AssignmentType { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_ResponsibilityLevel", ResourceType = typeof(Localization.Resources))]
        public ResponsibilityLevel ResponsibilityLevel { get; set; } 
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_ReportsTo", ResourceType = typeof(Localization.Resources))]
        public int ReportsTo { get; set; }
        public int? ReportsToMappingId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Department", ResourceType = typeof(Localization.Resources))]
        public int Department { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        // public virtual Branch Branch { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_IsCurrent", ResourceType = typeof(Localization.Resources))]
        public bool isCurrent { get; set; }
        [Display(Name = "Is Temporary")]
        public bool IsTemporary { get; set; }
        [Display(Name = "Placement_IsHead", ResourceType = typeof(Localization.Resources))]
        public bool IsHead { get; set; }
        public int Index { get; set; }
        [Display(Name = "Placement_HaveFixedAsset", ResourceType = typeof(Localization.Resources))]
        public bool HaveFixedAsset { get; set; }
        [Display(Name = "Common_PersonEmployee_Status", ResourceType = typeof(Localization.Resources))]
        public EmployeeStatus EmployementType { get; set; }
        [Display(Name = "Placement_Boss", ResourceType = typeof(Localization.Resources))]
        public int?  HeadId { get; set; }
        public bool IsPend { get; set; }
        public bool IsEditable { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int BusinessUnitId { get; set; }
        [Display(Name = "HR_SelfCare_TrainingApplicant_IsApproved", ResourceType = typeof(Localization.Resources))]
        public bool IsApprove { get; set; }
       [Display(Name = "HR_SelfCare_TrainingApplicant_ApprovedBy", ResourceType = typeof(Localization.Resources))]
        public int? ApprovedByEmpId { get; set; }
       // [Display(Name = "HR_SelfCare_TrainingApplicant_IsApproved", ResourceType = typeof(Localization.Resources))]
        public int? PreparedByEmpId { get; set; }
        [Display(Name = "Employee Type")]
        public int? EmployeeTypeId { get; set; }
        //This field is used for check to group de activate employees
        [Display(Name = "Highest position or CEO")]
        public bool CheckCEO { get; set; }
        [NotMapped]
        [Display(Name = "Placement_FullName", ResourceType = typeof(Localization.Resources))]
        public string FullName { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Rpt_HireDate", ResourceType = typeof(Localization.Resources))]
        [NotMapped]
        public DateTime? HireDateGorg { get; set; }
        [Display(Name = "Rpt_HireDate", ResourceType = typeof(Localization.Resources))]
        [NotMapped]
        public string HireDate { get; set; }
        public double? Age { get; set; }
        [NotMapped]
        [Display(Name = "Placement_Boss", ResourceType = typeof(Localization.Resources))]
        public string Boss { get; set; }
        [NotMapped]
        [Display(Name = "Common_PersonEmployee_Gender", ResourceType = typeof(Localization.Resources))]
        public string Gender { get; set; }

        public bool IsChecked { get; set; }

        [Display(Name = "Exempt from Attendance")]
        public bool IsAttendanceExempt { get; set; }
        public string CheckedBy { get; set; }
        public DateTime? CheckedDate { get; set; }


        //This field is used for check to identify employeeTo be promoted in  specified period
        [NotMapped]
        public bool IsToBePromoted { get; set; }
        [NotMapped]
        public bool IsFit { get; set; }

        
    }

    public class PlacementWorkflowSetting : TrackUserOperation
    {
        [Key]
        public int PlacementWorkflowSettingId { get; set; }

        /// <summary>
        /// Determines whether placement requires checking before approval.
        /// If false, placement can proceed directly to approval.
        /// </summary>
        public bool RequireChecker { get; set; }
    }
}