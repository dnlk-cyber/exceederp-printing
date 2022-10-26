using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class SalesQuoteVoucher : OperationWithThirdValidation
    {
        [Key]
        public int SalesQuoteVoucherID { get; set; }

        public SalesQuoteSource Source { get; set; }

        [Display(Name ="Customer")]
        public int OrganizationCustomerId   { get; set; }
        [DisplayName("Issued Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime IssuedDate { get; set; }

        public bool IsIssued { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime Date { get; set; }
        [Display(Name = "Branch")]
        public int BranchId { get; set; }

        [DisplayName("Store")]
        public int StoreID { get; set; }

        [Display(Name = "Period")]
        public int CRMPeriodId { get; set; }
        public bool IsVoid { get; set; }

        [DisplayName("Grand Total")]
        public decimal GrandTotal { get; set; }

        public string Period { get; set; }

        public string LastObjectState { get; set; }


        [DisplayName("Grand Total")]
        public string PaymentMethod { get; set; }
        public string Remark { get; set; }
        public virtual OrganizationCustomer OrganizationCustomer { get; set; }

        [Display(Name ="Quotation Term")]
        public int QuotationTermId { get; set; }

        [Display(Name ="Due date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? DueDate { get; set; }

        [NotMapped]
        public string Status { get; set; }

        [NotMapped]
        [DisplayName("Customer Name")]
        public string NewCustomer { get; set; }

        /// <summary>
        /// maintenance job order number
        /// </summary>
        public string JobOrderNumber { get; set; }

        [Display(Name ="Plate No")]
        public string PlateNo { get; set; }

        [Display(Name = "Bank Account")]
        public int? BankAccountId { get; set; }

    }


}
