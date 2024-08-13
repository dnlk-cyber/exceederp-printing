using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.Health
{
    public class HealthRankAndValue:Operations
    {
        public int HealthRankAndValueId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int BusinessUnitId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        public virtual Branch GetBranch { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_Advance_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
        [Display(Name = "HR_HealthAndInsurance_FixedAssetAccidentRecord_GlFiscalYearId", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
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
        [Display(Name = "Payment Coverage")]
        public decimal PaymentCoverage { get; set; }
        [Display(Name = "Total Paid")]
        public decimal TotalPaid { get; set; }
        [Display(Name = "Used Amount")]
        public decimal UsedAmount { get; set; }
        [Display(Name ="Staff Contribution")]
        public decimal StaffContribution { get; set; }
        public decimal RemainingAmount { get; set; }
        public bool IsPartnerOrGov { get; set; }
        [Display(Name = "Benefit Type")]
        public HealthBenefitType BenefitType { get; set; }
        [Display(Name = "Payment Type")]
        public HealthPaymentType PaymentType { get; set; }
        public bool IsOtherBenifit { get; set; }
        public int? PaymentRequestId { get; set; }

    }
    public class HealthRankAndValueValidation : VoucherValidation
    {
        public int HealthRankAndValueValidationId { get; set; }
        public int HealthRankAndValueId { get; set; }
        public virtual HealthRankAndValue  RankAndValue { get; set; }

    }
    public class HealthAttachment
    {
        public int HealthAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int HealthRankAndValueId { get; set; }
        public virtual HealthRankAndValue RankAndValue { get; set; }
    }

    public enum HealthPaymentType
    {
        Credit,
        Cash
    }
}
