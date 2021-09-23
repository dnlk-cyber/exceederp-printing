using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Core.Domain.Finance.AP.PaymentRequest
{
    public class PaymentRequest: DepartmentOperations
    {
        public int PaymentRequestId { get; set; }

        [Display(Name = "Supplier ID")]
        public string SupplierId { get; set; }
        [Display(Name = "Is New Supplier")]
        public bool IsNewSupplier { get; set; }
        [Display(Name = "Supplier Name")]
        public string Supplier { get; set; }
        public int? Purchaser { get; set; }
        public string ReasonOfPayment { get; set; }
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [Display(Name = "Request Date")]
        public string RequestDateAm { get; set; }
        public string InvoiceNo { get; set; }
        public string PurchaseRequestNo { get; set; }
        public string PONo { get; set; }
        public string GRN { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        public Branch Branch { get; set; }
        [Display(Name = "Department")]
        public int OrgStuructureId { get; set; }
        public MainCompanyStructure MainCompanySturucture { get; set; }
        [Display(Name = "Employee")]
        public int? EmployeeId { get; set; }
        public bool Paid { get; set; }
        public virtual ICollection<PaymentRequestLine> PaymentRequestLines { get; set; }
        public virtual ICollection<PaymentRequestDepartmentValidation> PaymentRequestDepartmentValidations { get; set; }
        public virtual ICollection<PaymentRequestValidation> PaymentRequestValidations { get; set; }

    }
}
