using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AP.PaymentRequest
{
    
   public class PaymentRequest: DepartmentNewOperations
    {
        public int PaymentRequestId { get; set; }

        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }
        [Display(Name = "New Supplier?")]
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]
        public string Supplier { get; set; }
        public int? Purchaser { get; set; }
        [Required]
        public string ReasonOfPayment { get; set; }
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [Display(Name = "Request Date")]
        public string RequestDateAm { get; set; }
        public string InvoiceNo { get; set; }
        [Display(Name = "PR No.")]
        public string PurchaseRequestNo { get; set; }
        [Display(Name = "PO No.")]
        public string PONo { get; set; }
        public string GRN { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        [Display(Name = "Department")]
        public int OrgStuructureId { get; set; }
        [Display(Name = "Employee")]
        public int EmployeeId { get; set; }
        public bool Paid { get; set; }
        //budget controll
        public bool IsBudgetApproved { get; set; }
        public string BudgetApprovedBy { get; set; }
        public DateTime? BudgetApprovedTime { get; set; }
        //authorization
        public bool IsAuthorized { get; set; }
        public string AuthorizedBy { get; set; }
        public DateTime? AuthorizedTime { get; set; }
        //for access or view
        public int CreatedAtBranchId { get; set; }
        public int CreatedAtBusinessUnitId { get; set; }
        public virtual ICollection<PaymentRequestLine> PaymentRequestLines { get; set; }
        public virtual ICollection<FundTransferLine> FundTransferLines { get; set; }
        public virtual ICollection<PaymentRequestAttachment> PaymentRequestAttachments { get; set; }
        public virtual ICollection<PaymentRequestDepartmentValidation> PaymentRequestDepartmentValidations { get; set; }       
        public virtual ICollection<PaymentRequestBudgetValidation> PaymentRequestBudgetValidations { get; set; }
        public virtual ICollection<PaymentRequestAuthorizeValidation> PaymentRequestAuthorizeValidations { get; set; }
        public virtual ICollection<PaymentRequestValidation> PaymentRequestValidations { get; set; }
    }

    public class FundTransferLine : TrackUserSettingOperation
    {
        public int FundTransferLineId { get; set; }
        public int PaymentRequestId { get; set; }
        public PaymentRequest PaymentRequest { get; set; }
        [Display(Name = "Branch/Project")]
        public int? BranchId { get; set; }    
        public decimal Material { get; set; }
        public decimal FuelAndLubricant { get; set; }
        public decimal ManPower { get; set; }
        public decimal Machinery { get; set; }
        public decimal CompensationAndSeverance { get; set; }
        public decimal FoodAndPerDiem { get; set; }
        public decimal TotalCost { get; set; }
        public decimal TotalApproved { get; set; }
        public decimal TotalPermited { get; set; }
        public string Remark { get; set; }
    }

    public class PaymentRequestAttachment
    {
        public int PaymentRequestAttachmentId { get; set; }
        public int PaymentRequestId { get; set; }
        public PaymentRequest PaymentRequest { get; set; }
        public string FileName { get; set; }
        public string FilePath { get; set; }
    }
}
