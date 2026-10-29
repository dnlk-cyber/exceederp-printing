using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.Allowance;

namespace ExceedERP.Core.Domain.HRM.EmployeeDetail
{
    /// <summary>    Employee Reward . </summary>
    ///
    /// <remarks>   Kibrom Abera, 10/28/2024. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    public class EmployeeReward : TrackUserOperation
    {
        [Key]
        public int EmployeeRewardId { get; set; }
        [Required]
        [Display(Name = "Full-Name")]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "Sub-Branch")]
        public string SubBranch { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        public string DateAm { get; set; }
        public RewardType RewardType { get; set; }
        [Display(Name = "Reason for Reward ")]
        public string ReasonForReward { get; set; }
        public string AwardedBy { get; set; }
        public decimal? Salary { get; set; }
        [Display(Name = "Person of Salary ")]
        public decimal? PersonOfSalary { get; set; }
        public decimal? Amount { get; set; }
        public string Unit { get; set; }
        [Display(Name = "Fiscal_Year", ResourceType = typeof(Localization.Resources))]
        public int? GlFiscalYearId { get; set; }
        [Display(Name = "HR_Attendance_EmpPayrollNoDaysWorked_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int? GLPeriodId { get; set; }
        [Display(Name = "Account")]
        public string PayrollAccount { get; set; }
        [Display(Name = "Remark", ResourceType = typeof(Localization.Resources))]
        public string Remark { get; set; }
    }
    public enum RewardType
    {
        Certification,
        Money, 
        Kind_Gold_Or_Silver,
        Leave,
        Vacation,
        Others
    }
    public class RewardSetting
    {
        [Key]
        public int RewardSettingId { get; set; }
        /// <summary>
        /// Minimum number of months an employee must serve to be eligible for a reward.
        /// </summary>
        [Display(Name = "Minimum Months Required for Reward")]
        public int MinimumMonthsRequired { get; set; } = 6;

        /// <summary>
        /// Whether employees who are newly hired (less than MinimumMonthsRequired) are allowed rewards.
        /// </summary>
        [Display(Name = "Allow New Employees")]
        public bool AllowNewEmployees { get; set; } = false;

        /// <summary>
        /// Whether employees who are transferred within the MinimumMonthsRequired period are allowed rewards.
        /// </summary>
        
        [Display(Name = "Allow Recently Transferred Employees")]
        public bool AllowTransferredEmployees { get; set; } = false;

        /// <summary>
        /// Whether employees with active disciplinary actions are allowed rewards.
        /// </summary>
        [Display(Name = "Allow Employees with Disciplinary Records")]
        public bool AllowDisciplinedEmployees { get; set; } = false;
        [Display(Name = "Maximum Months Disciplined Employees Must Wait")]
        public int MaximumDisciplinedMonthsRequired { get; set; } = 6;
    }



}