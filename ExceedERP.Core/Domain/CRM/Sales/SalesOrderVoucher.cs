using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{

    public class SalesOrderVoucher : Operations
    {
        [Key]
        public int SalesOrderVoucherID { get; set; }
        [Display(Name = "Customer")]
        public int OrganizationCustomerId { get; set; }
        public string References { get; set; }


        
        [DisplayName("Issued Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime IssuedDate { get; set; }

        public bool IsIssued { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [Required]
        public DateTime Date { get; set; }

        
        public bool IsVoid { get; set; }

        [DisplayName("Grand Total")]
        public decimal GrandTotal { get; set; }

        public string Period { get; set; }

        public string LastObjectState { get; set; }

        [Display(Name = "Branch")]
        public int BranchId { get; set; }
        [DisplayName("Store")]
        public int StoreID { get; set; }
        [Display(Name = "Period")]
        public int CRMPeriodId { get; set; }
        [DisplayName("Payment Method")]
        public string PaymentMethod { get; set; }
        public string Remark { get; set; }
        public virtual OrganizationCustomer OrganizationCustomer { get; set; }
        
        public SalesOrderSource Source { get; set; }

        [NotMapped]
        public string Status { get; set; }

        [NotMapped]
        [DisplayName("Customer Name")]
        public string NewCustomer { get; set; }

        /// <summary>
        /// flag to hold if this sales order record is completely shipped or not
        /// </summary>
        public bool IsShipped { get; set; }

        /// <summary>
        /// maintenance job order number
        /// </summary>
        public string JobOrderNumber { get; set; }
        //public bool IsFromMaintenance { get; set; }
        [Display(Name = "Plate No")]
        public string PlateNo { get; set; }
    }


}
