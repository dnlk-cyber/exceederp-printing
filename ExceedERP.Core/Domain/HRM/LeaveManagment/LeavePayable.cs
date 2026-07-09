using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Localization;

namespace ExceedERP.Core.Domain.HRM.LeaveManagment
{
    /// <summary>   A leave. Payable </summary>
    ///
    /// <remarks>   Kibrom Abera, 9/21/2023. </remarks>
    ////////////////////////////////////////////////////////////////////////////////////////////////////
    //Leave Payable 
    public class LeavePayable: TrackUserOperation  //About Le
    {
        [Key ]
        public int LeavePayableId { get; set; }
        [Required]
        [Display(Name = "Fiscal_Year", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        public virtual GlFiscalYear GlFiscalYear { get; set; }
        public string Description { get; set; }
        [NotMapped]
        public bool IsAnySentForPayment { get; set; }
       
    }
    public class LeavePayableDetail : TrackUserOperation  //About Le
    {
        [Key]
        public int LeavePayableDetailId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int? BusinessUnitId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        [Display(Name = "HR_Prohibition_Prohibition_EmployeeCode", ResourceType = typeof(Localization.Resources))]
        public string EmployeeCode { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_PersonEmployee", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Required]
        [Display(Name = "Total Payable Days")]
        public decimal PayableDays { get; set; }
        [Required]
        [Display(Name = "HR_LeaveManagment_Leave_Payable__Balance", ResourceType = typeof(Localization.Resources))]
        public decimal Balance { get; set; }
        [Display(Name = "Year 1")]
        public decimal Year1 { get; set; }
        [Display(Name = "Year 2")]
        public decimal Year2 { get; set; }
        //[Display(Name = "Total Annual leave")]
        //public decimal TotalAnnualLeave { get; set; }
        [Display(Name = "Annual leave per month")]
        public decimal AnnualLeavePerMonth { get; set; }
       
        [Display(Name = "Basic Salary ")]
        public decimal BasicSalary  { get; set; }
        [Display(Name = "Agregate taxable Amount ")]
        public decimal AgregateTaxableAmount { get; set; }
        [Display(Name = "Agregate Income Tax")]
        public decimal AgregateIncomeTax { get; set; }
        [Display(Name = "Basic Salary Income Tax ")]
        public decimal BasicSalaryIncomeTax  { get; set; }
        [Display(Name = "Tax difference")]
        public decimal TaxDifference  { get; set; }
        [Display(Name = "Net Payment ")]
        public decimal NetPayment   { get; set; }
        public bool IsSendForPayment { get; set; }
        public string SendForPaymentBy { get; set; }
        public DateTime? SendForPaymentTime { get; set; }
        public int? PaymentRequestId { get; set; }
        public string Remark { get; set; }
        public int LeavePayableId { get; set; }
        public virtual LeavePayable Payable { get; set; }

    }

    public class LeavePayableWithDetailFiscalYear
    {
        public int LeavePayableWithDetailFiscalYearId { get; set; }
        [Display(Name = "Fiscal_Year", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "HR_LeaveManagment_Leave_Payable__Days", ResourceType = typeof(Localization.Resources))]    
        public decimal PayableDays { get; set; }
        [Display(Name = "HR_LeaveManagment_Leave_Payable__Balance", ResourceType = typeof(Localization.Resources))]
        public decimal Balance { get; set; }
        public int LeavePayableDetailId { get; set; }
        public virtual LeavePayableDetail PayableDetail { get; set; }
        
    }
    public class LeaveEmployeeVM
    {
        public int EmployeeId { get; set; }
        public string FullNameEnglish { get; set; }
        public string FullName { get; set; }

    }
}