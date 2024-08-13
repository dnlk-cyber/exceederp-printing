using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.Insurance.Setting;
using ExceedERP.Core.Localization;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.Insurance.InsuranceNew
{
    public class EmployeeMedicalExpenseRefund: DepartmentNewOperations
    {
        public int EmployeeMedicalExpenseRefundId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployeeID", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "Common_PersonEmployee_Status", ResourceType = typeof(Localization.Resources))]
        public EmployeeStatus EmployementType { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_ResponsibilityLevel", ResourceType = typeof(Localization.Resources))]
        public ResponsibilityLevel ResponsibilityLevel { get; set; }
        [Display(Name = "Benefit Type")]
        public InsuranceRefuendType BenefitType { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_Advance_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
        [Display(Name = "HR_HealthAndInsurance_FixedAssetAccidentRecord_GlFiscalYearId", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int BusinessUnitId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Date", ResourceType = typeof(Resources))]
        public DateTime Date { get; set; }
        [Display(Name = "Date", ResourceType = typeof(Localization.Resources))]
        [Required]
        public string DateAm { get; set; }
        [Display(Name = "Amount in Birr")]
        public decimal AmountInBirr { get; set; }
        [Display(Name = "Allowed Amount")]
        public decimal? AllowedAmount { get; set; }
        [Display(Name = "Used Amount")]
        public decimal? UsedAmount { get; set; }
        [Display(Name = "Remaining Amount")]
        public decimal? RemainingAmount { get; set; }
        #region II-	በኢንሹራንስ ክፍል ወይም ክፍያ በሚያስፈፅመው አካል የሚሞላ
        [Display(Name = "Total Expense")]
        public decimal TotalExpense { get; set; }
        [Display(Name = "Deduction")]
        public decimal Deduction { get; set; }
        [Display(Name = "Reason for Deduction")]
        public string ReasonForDeduction { get; set; }
        [Display(Name = "Net Pay")]
        public decimal NetPay { get; set; }
        #endregion
        [Display(Name = "Number of documents")]
        public int NumberOfDocument { get; set; }
        public int? DepartmentAproverId { get; set; }
        public bool IsVerifyied { get; set; }
        public bool IsTheDocumentComplete { get; set; }


    }
    public class MedicalExpenseRefundAttachment
    {
        public int MedicalExpenseRefundAttachmentId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int EmployeeMedicalExpenseRefundId { get; set; }
        public virtual EmployeeMedicalExpenseRefund MedicalExpenseRefund { get; set; }

    }


    public class MedicalExpenseRefundDepartmentApproval : TrackUserOperation
    {
        public int MedicalExpenseRefundDepartmentApprovalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public int DepartmentAproverId { get; set; }

        public string Remark { get; set; }
        public int EmployeeMedicalExpenseRefundId { get; set; }
        public virtual EmployeeMedicalExpenseRefund MedicalExpenseRefund { get; set; }


    }
    public class MedicalExpenseRefundApproval : TrackUserOperation
    {
        public int MedicalExpenseRefundApprovalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public int AproverId { get; set; }
        public string Remark { get; set; }
        public int EmployeeMedicalExpenseRefundId { get; set; }
        public virtual EmployeeMedicalExpenseRefund MedicalExpenseRefund { get; set; }

    }



}
