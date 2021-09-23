using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.HRM.HealthAndInsurance
{
    public class HealthRankAndValue
    {
        public int HealthRankAndValueId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public virtual Branch Branch { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_Advance_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
        [Display(Name = "HR_HealthAndInsurance_FixedAssetAccidentRecord_GlFiscalYearId", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        [NotMapped]
        public string FiscalYearName { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        [DataType(DataType.Date)]
        [Display(Name ="Treated Date")]
        public DateTime Date { get; set; }
        [Display(Name = "Treated Date")]
        public string DateAmh { get; set; }
        [DataType(DataType.Date)]
        [Display(Name = "Recored Date")]
        public DateTime RecoredDate { get; set; }
        [Display(Name = "RecoredDate")]
        public string RecoredDateAmh { get; set; }
        public string ClinicName { get; set; }
        public decimal AllowedAmount { get; set; }
        public decimal RemainingAmount { get; set; }
        public decimal UsedAmount { get; set; } 
        [NotMapped]
        public string EmployeeCode { get; set; }
    }
}
