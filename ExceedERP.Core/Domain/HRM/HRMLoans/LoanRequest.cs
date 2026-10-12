using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.HRM.HRMLoans
{
    public class LoanRequest : DepartmentOperations
    {
        [Key]
        public int LoanRequestId { get; set; }
        [Required]
        [Display(Name = "HR_EmployeeDetail_Placement_PersonEmployeeID", ResourceType = typeof(Localization.Resources))]
        public int EmployeeId { get; set; }
        public virtual Person_Employee Employee { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_JobTitleName", ResourceType = typeof(Localization.Resources))]
        public int LookupId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_OrgStructureId", ResourceType = typeof(Localization.Resources))]
        public int OrgStructureId { get; set; }
        [Display(Name = "HR_EmployeeDetail_Placement_Branch", ResourceType = typeof(Localization.Resources))]
        public int BranchId { get; set; }
        [Display(Name = "Placement_BusinessUnit", ResourceType = typeof(Localization.Resources))]
        public int BusinessUnitId { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name = "Request Date")]
        public DateTime RequestDate { get; set; }
        [Display(Name = "Request Date")]
        [Required]
        public string RequestDateAm { get; set; }
        [Display(Name = "GLPeriod_GlFiscalYearId", ResourceType = typeof(Localization.Resources))]
        public int GlFiscalYearId { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_Loan_PeriodId", ResourceType = typeof(Localization.Resources))]
        public int GLPeriodId { get; set; }
        public virtual GLPeriod GLPeriod { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_RegularContribution_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }
        [Display(Name = "HR_SelfCare_LeaveRequest_IsForOther", ResourceType = typeof(Localization.Resources))]
        public bool IsForOther { get; set; }
        public bool IsFinalReviewCompleted { get; set; }

        public string FinalReviewedBy { get; set; }

        public DateTime? FinalReviewedOn { get; set; }
        public bool IsSentToPayment { get; set; } = false;
        public int? PaymentRequestId { get; set; }
        public int? NextApproverEmployeeId { get; set; }

    }
    public class LoanRequestAttachment : TrackUserSettingOperation
    {
        [Key]
        public int LoanRequestAttachmentId { get; set; }
     
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
        public int LoanRequestId { get; set; }
        public virtual LoanRequest Request { get; set; }
    }

    public class LoanRequestDetail: TrackUserSettingOperation
    {
        [Key]
        public int LoanRequestDetailId { get; set; }
        public int LoanRequestId { get; set; }
        public virtual LoanRequest Request { get; set; }
        [Display(Name = "Loan Type")]
        public LoanType LoanType { get; set; }
        public string Descrption { get; set; }
        [Display(Name = "Loan Amount")]
        public decimal LoanAmount { get; set; }
        [Display(Name = "Loan Repayment Month")]
        public decimal LoanRepaymentMonth { get; set; }
        [Display(Name = "Monthly Deduction")]
        public decimal MonthlyDeduction { get; set; }
        public string Remark { get; set; }
        [Display(Name = "HR_Payroll_EmployeePayroll_RegularContribution_IsActive", ResourceType = typeof(Localization.Resources))]
        public bool IsActive { get; set; }



    }
    public class LoanRequestDepartmentValidation : VoucherValidation
    {
        public int LoanRequestDepartmentValidationId { get; set; }
        public int LoanRequestId { get; set; }
        public virtual LoanRequest LoanRequest { get; set; }

    }

    public class LoanRequestValidation : VoucherValidation
    {
        [Key]
        public int LoanRequestValidationId { get; set; }
        public int LoanRequestId { get; set; }
        public virtual LoanRequest Request { get; set; }

    }
    public enum LoanType
    {
        Long_Term,
        Emergency_Loan,
        Medium_Loan,
        Medical_Emergency,
        Education,
        Other
    }
}
