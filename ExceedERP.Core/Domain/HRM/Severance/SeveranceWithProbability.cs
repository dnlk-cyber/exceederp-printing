using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Localization;

namespace ExceedERP.Core.Domain.HRM.Severance
{
    /// <summary>   A Severance. </summary>
    ///
    /// <remarks>   Kibrom Abera, July 15, 202. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    
    public class SeveranceWithProbability : TrackUserOperation  //About Le
    {
        [Key]
        public int SeveranceWithProbabilityId { get; set; }
        [Required]
        [Display(Name = "Fiscal_Year", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Common_PersonEmployee_DateOfBirth", ResourceType = typeof(Localization.Resources))]
        public DateTime DateOfBirth { get; set; }
        [Display(Name = "Common_PersonEmployee_DateOfBirth", ResourceType = typeof(Localization.Resources))]
        public string DateOfBirthAm { get; set; }
        [Display(Name = "Common_PersonEmployee_Gender", ResourceType = typeof(Localization.Resources))]
        public Gender Gender { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Common_PersonEmployee_HiredDate", ResourceType = typeof(Localization.Resources))]
        public DateTime HiredDate { get; set; }
        [Display(Name = "Common_PersonEmployee_HiredDate", ResourceType = typeof(Localization.Resources))]
        public string HiredDateAmharic { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Normal Retirment Age")]
        public DateTime NormalRetirmentAge { get; set; }
        [Display(Name = "Normal Retirment Age")]
        public string NormalRetirmentAgeAm { get; set; }
        public decimal Age { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int BusinessUnitId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        [Display(Name = "Salary")]
        public decimal Salary { get; set; }
        [Required]
        [Display(Name = "Year Worked")]
        public decimal YearWorked { get; set; }
        [Display(Name = "Remaining No of years")]
        public decimal RemainingNoOfYears { get; set; }
        [Display(Name = "Prob of turnover")]
        public decimal ProbabilityOfTurnover { get; set; }
        [Display(Name = "Salary up on Retirement")]
        public decimal SalaryUpOnRetirement { get; set; }
        [Display(Name = "PV of salary upon Retirement")]
        public decimal PVOfSalaryUponRetirement { get; set; }
        [Display(Name = "Max Allowed period(in years)")]
        public decimal MaxAllowedPeriod { get; set; }
        [Display(Name = "Max Entitelement Amount ")]
        public decimal MaxEntitelementAmount { get; set; }
        [Display(Name = "EV 1")]
        public decimal EV1 { get; set; }
        [Display(Name = "Mortality rate")]
        public decimal MortalityRate { get; set; }
        [Display(Name = "EV 2")]
        public decimal EV2 { get; set; }
        [Display(Name = "Total EV")]
        public decimal TotalEV { get; set; }
        [Display(Name = "Description", ResourceType = typeof(Resources))]
        public string Description { get; set; }
    }
}