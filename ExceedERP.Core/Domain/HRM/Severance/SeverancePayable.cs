using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.Severance
{
    /// <summary>   A Severance Payable. </summary>
    ///
    /// <remarks>   Kibrom Abera, 10/31/2023. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////

    public class SeverancePayable : TrackUserOperation  //About Le
    {
        [Key ]
        public int SeverancePayableId { get; set; }
        [Required]
        [Display(Name = "Fiscal_Year", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        public string Description { get; set; }
       
    }
    public class SeverancePayableDetail : TrackUserOperation  //About Le
    {
        [Key]
        public int SeverancePayableDetailId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int? BusinessUnitId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        public int SeverancePayableId { get; set; }
        public virtual SeverancePayable Severance { get; set; }

        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "Basic Salary")]
        public decimal BasicSalary { get; set; }
        public decimal TaxWithheldIncomeTax { get; set; }
        [Required]
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
        [Display(Name = "None Taxable No.of years")]
        public string NumberOFYears { get; set; }
        public string ServicePeriod { get; set; }
        [Display(Name = "Gross Componsation Payment")]
        public decimal GrossComponsation { get; set; }
        [Display(Name = "Tax on Basic Salary")]
        public decimal TaxOnBasicSalary { get; set; }
        [Display(Name = "Total")]
        public decimal NoneTaxableTotal { get; set; }
        //what is this could you explain
        [Display(Name = "Total Income Tax")]
        public decimal TaxWithheldComTax { get; set; }
        [Display(Name = "Deduction ")]
        public decimal SpecialSalaryDeduction { get; set; }
        [Display(Name = "Total Deduction")]
        public decimal TotalDeductions { get; set; }
        [Display(Name = "Years")]
        public decimal Years { get; set; }
        [Display(Name = "Months")]
        public decimal Months { get; set; }
        [Display(Name = "Days")]
        public decimal Days { get; set; }
        [Display(Name = "Total Service Months")]
        public decimal TotalServiceMonths { get; set; }
        [Display(Name = "G.Compensation Pay't  ")]
        public decimal GrossCompensationPayment   { get; set; }
        [Display(Name = "Compensation Pay'tT")]
        public decimal CompensationPaymentT { get; set; }
        [Display(Name = "Tax on Monthly Salary + Compensation")]
        public decimal TaxOnMonthlySalaryAndCompensation { get; set; }
        [Display(Name = "Tax Difference")]
        public decimal TaxDifference { get; set; }
        [Display(Name = "Acc. Tax Difference")]
        public decimal AccTaxDifference { get; set; }
        [Display(Name = "Net Pay")]
        public decimal NetPay { get; set; }
        public string  Remark { get; set; }
        public bool  IsSendForPayment { get; set; }
        public string SendBy { get; set; }
        public DateTime? DateSent { get; set; }
        public int? PaymentRequestId { get; set; }

       
    }
}