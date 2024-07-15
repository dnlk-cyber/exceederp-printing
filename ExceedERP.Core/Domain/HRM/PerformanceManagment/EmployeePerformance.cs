using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Localization;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class EmployeePerformance: TrackUserSettingOperation
    {
        [Key]
        public int EmployeePerformanceId { get; set; }
        [Display(Name ="Employee")]
        public int EmployeeId { get; set; }
        public int DirectBossId { get; set; }
        public int? SecondLevelApproverId { get; set; }
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "Period")]
        public int GlPeriodId { get; set; }
        [Display(Name = "Evaluation Criteria Type")]
        public int EvaluationCriteriaTypeId { get; set; }
        [Display(Name = "Criteria Category")]
        public int CriteriaCategoryId { get; set; }
        [Display(Name = "Work Unit")]
        public int OrgStructureId { get; set; }
        [Display(Name ="DeadLine(Days)")]
        public decimal DeadLine { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
            ApplyFormatInEditMode = true),Display(Name ="Evaluated Date")]
        public DateTime? Date { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [Display(Name = "Bussiness Unit")]
        public int BusinessUnitId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_ResponsibilityLevel", ResourceType = typeof(Localization.Resources))]
        public ResponsibilityLevel ResponsibilityLevel { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int JobTitleId { get; set; }
        [Display(Name = "Weight")]
        public decimal Weight { get; set; }
        [Display(Name = "Result")]
        public decimal Result { get; set; }
        //[Display(Name = "Is IsSubmitted")]
        ////for summit  / it serves as submit status handler
        //public bool IsSubmitted { get; set; }
        [Display(Name = "Is Approved")]
        //for summit  / it serves as submit status handler
        public bool IsApproved { get; set; }
        [Display(Name = "Is Corporate Applied")]
        public bool IsCorporateApplied { get; set; }
        [Display(Name = "Is Sector Applied")]
        public bool IsSectorApplied { get; set; }
        [Display(Name = "Is Immediate Suppurvisor Approved")]
        public bool IsImmediateSuppurvisorApproved { get; set; }
        [Display(Name = "Is Accepted By Employee ")]
        public bool IsAcceptedByEmployee { get; set; }
        [Display(Name = "Is Rejected By Employee ")]
        public bool IsRejectedByEmployee { get; set; }
        [Display(Name = "Employee Remark")]
        public string EmployeeRemark { get; set; }
        [Display(Name = "Reason for Rejection")]
        public string ReasonFor2ndRejection { get; set; }
        [Display(Name = "Is Second Level Approved")]
        public bool IsSecondLevelApproved { get; set; }
        [Display(Name = "Is Second Level Rejected")]
        public bool IsSecondLevelRejected { get; set; }
        [Display(Name = "Is HR Approved")]
        public bool IsHRApproved { get; set; }
        public bool IsHRLevelEvalution { get; set; }
        public bool IsCurrent { get; set; }
        public string Remark { get; set; }
        public string Comment { get; set; }
        [NotMapped]
        public string Grade { get; set; }

    }

    public enum EvaluationTypeLevel
    {
        [Display(Name = "Enum_Evaluation_Type_Corporate_Result", ResourceType = typeof(Resources))]
        Corporate_Result=6,
        [Display(Name = "Enum_Evaluation_Type_Sector_Result", ResourceType = typeof(Resources))]
        Sector_Result=7,
        [Display(Name = "Enum_Evaluation_Type_Main_Department_Level", ResourceType = typeof(Resources))]
        MainDepartmentLevel=8,
        [Display(Name = "Enum_LookUpTypes_PROJECT_MANAGER", ResourceType = typeof(Resources))]
        Project_Manager=9,
    }
}