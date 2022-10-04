using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SalaryStructure;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>   A placement. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class Placement : TrackUserOperation
    {
        [Key]
        public int PlacementId { get; set; }
        [Display(Name = "Placement_immediate_Supervisor", ResourceType = typeof(Localization.Resources))]
        public int? ImmediateSupervisor { get; set; }
        // public bool IsSupervisor { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
      //  public virtual Person_Employee Employee { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "HR_EmployeeDetail_Placement_AssignmentDate", ResourceType = typeof(Localization.Resources))]
        public DateTime AssignmentDate { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_AssignmentDate", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string AssignmentDateAmharic { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
       // public virtual Lookup Lookup { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
       // public virtual MainCompanyStructure MainCompanyStructure { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public string JobTitleName { get; set; }

        [Required]
        public int MappingId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_Rank", ResourceType = typeof(Localization.Resources))]
        public int RankId { get; set; }
       // public virtual Rank Rankrel { get; set; }

        [Display(Name = "HR_EmployeeDetail_Placement_Rank", ResourceType = typeof(Localization.Resources))]
        ////[NotMapped ]
        public string RankName { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Step", ResourceType = typeof(Localization.Resources))]
        //[NotMapped ]
        public string StepName { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_Step", ResourceType = typeof(Localization.Resources))]
        public int StepId { get; set; }
       // public virtual Step StepRel { get; set; }
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Please enter valid integer Number")]
        [Display(Name = "HR_EmployeeDetail_Placement_Salary", ResourceType = typeof(Localization.Resources))]
        public decimal Salary { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_AssignmentType", ResourceType = typeof(Localization.Resources))]
        public AssignmentType AssignmentType { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_ResponsibilityLevel", ResourceType = typeof(Localization.Resources))]
        public ResponsibilityLevel ResponsibilityLevel { get; set; }


        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_ReportsTo", ResourceType = typeof(Localization.Resources))]

        public int ReportsTo { get; set; }

        public int ReportsToMappingId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Department", ResourceType = typeof(Localization.Resources))]
        public int Department { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
       // public virtual Branch Branch { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_IsCurrent", ResourceType = typeof(Localization.Resources))]

        public bool isCurrent { get; set; }
        [Display(Name = "Placement_IsHead", ResourceType = typeof(Localization.Resources))]
        public bool IsHead { get; set; }
        public int Index { get; set; }
        [Display(Name = "Placement_HaveFixedAsset", ResourceType = typeof(Localization.Resources))]
        public bool HaveFixedAsset { get; set; }
        [Display(Name = "Common_PersonEmployee_Status", ResourceType = typeof(Localization.Resources))]
        public EmployeeStatus EmployementType { get; set; }
        [NotMapped]
        public string Boss { get; set; }
        public bool IsPend { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int BusinessUnitId { get; set; }
        // This field is used for check to group de activate employees
        [NotMapped]
        public bool Check { get; set; }
        public bool IsApprove { get; set; }
        public int ApprovedByEmpId { get; set; }
        public int PreparedByEmpId { get; set; }
        [NotMapped]
        public string FullName { get; set; }
        [NotMapped]
        public string HireDate { get; set; }
    }
}