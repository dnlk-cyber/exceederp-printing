using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;

namespace ExceedERP.Core.Domain.HRM.Severance
{
    /// <summary>   A leave. </summary>
    ///
    /// <remarks>   Mekanehiwot, 10/31/2016. </remarks>
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
        [Required]
        [Display(Name = "Balance")]
        public decimal Balance { get; set; }
        [Display(Name = "None Taxable No.of years")]
        public string NumberOFYears { get; set; }
        [Display(Name = "Gross Componsation")]
        public decimal GrossComponsation { get; set; }
        [Display(Name = "Monthly Income Tax")]
        public decimal TaxWithheldIncomeTax { get; set; }
        [Display(Name = "Total")]
        public decimal NoneTaxableTotal { get; set; }
        //what is this could you explain
        [Display(Name = "Total Income Tax")]
        public decimal TaxWithheldComTax { get; set; }
        [Display(Name = "Deduction ")]
        public decimal SpecialSalaryDeduction { get; set; }
        [Display(Name = "Total Deduction")]
        public decimal TotalDeductions { get; set; }
        [Display(Name = "Net Pay")]
        public decimal NetPay { get; set; }

       
    }
}