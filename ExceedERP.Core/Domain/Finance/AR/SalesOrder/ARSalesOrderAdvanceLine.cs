using System;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Finance.AR.SalesOrder
{
    public class ArSalesOrderAdvanceLine : TrackUserOperation
    {
        public int ArSalesOrderAdvanceLineId { get; set; }
        public int ArSalesOrderId { get; set; }
        public virtual ArSalesOrder ArSalesOrder { get; set; }
        [Display(Name = "Deposit Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime DepositDate { get; set; }
        [Required]
        [Display(Name = "reason")]
        public string Reason { get; set; }
        [Required]
        [Display(Name = "Deposited Amount")]
        public decimal Amount { get; set; }
        [Display(Name = "Deposit Reference")]
        public string Reference { get; set; }
    }
}
