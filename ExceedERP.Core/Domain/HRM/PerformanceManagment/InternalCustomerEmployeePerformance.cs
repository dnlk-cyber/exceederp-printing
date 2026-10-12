using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.HRM.PerformanceManagment
{
    public class InternalCustomerEmployeePerformance
    {
        public int InternalCustomerEmployeePerformanceId { get; set; }
        [Required]
        public int EmployeeId { get; set; }
        public int EvaluatorEmployeeId { get; set; }
        [Required]
        public int GlFiscalYearId { get; set; }
        [Required]
        public int GlPeriodId { get; set; }
        [Required]
        public int EvaluationCriteriaTypeId { get; set; }
        [Display(Name = "Criteria-Category")]
        public int CriteriaCategoryId { get; set; }
        [Display(Name = "Work-Unit")]
        public int OrgStructureId { get; set; }
        [Display(Name = "DeadLine(Days)")]
        public decimal DeadLine { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}",
            ApplyFormatInEditMode = true), Display(Name = "Evaluated Date")]
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
        [Display(Name = "Is Approved")]
        public bool IsApproved { get; set; }
        [Display(Name = "Is Corporate Applied")]
        public bool IsCorporateApplied { get; set; }
        [Display(Name = "Is Sector Applied")]
        public bool IsSectorApplied { get; set; }
        [Display(Name = "Is Immediate Suppurvisor Approved")]
        public bool IsImmediateSuppurvisorApproved { get; set; }
        [Display(Name = "Is Accepted")]
        public bool IsAcceptedByEmployee { get; set; }
        [Display(Name = "Is Rejected By Employee ")]
        public bool IsRejectedByEmployee { get; set; }
        public bool IsCurrent { get; set; }
        public bool IsSummary { get; set; }
        public string Remark { get; set; }
        public string Comment { get; set; }

    }
    public class InternalCustomerEmployeeMeasurement
    {
        [Key]
        public int InternalCustomerEmployeeMeasurementId { get; set; }
        public int InternalCustomerEmployeePerformanceId { get; set; }
        public int MeasurmentId { get; set; }
        public decimal Weight { get; set; }
        public decimal Result { get; set; }
        public string Remark { get; set; }
    }

    public class InternalCustomerEmployeePerformanceScore
    {
        [Key]
        public int InternalCustomerEmployeePerformanceScoreId { get; set; }
        public int InternalCustomerEmployeeMeasurementId { get; set; }
        public int PerformanceActionId { get; set; }
        public decimal Weight { get; set; }
        public decimal Score { get; set; }
        public string Remark { get; set; }
    }
}
