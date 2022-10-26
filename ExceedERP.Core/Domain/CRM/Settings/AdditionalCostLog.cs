using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.CRM
{
    public class AdditionalCostLog : TrackUserOperation
    {
        public int AdditionalCostLogId { get; set; }
        [Display(Name ="Cost Type")]
        //public int LineItemId { get; set; }
        public int AdditionalCostId { get; set; }
        public decimal CostAmount { get; set; }

        public virtual AdditionalCost AdditionalCost { get; set; }
        //public virtual LineItem LineItem { get; set; }

        [Display(Name = "Tax")]
        public int GLTaxRateID { get; set; }

        /// <summary>
        /// This is reference to Header voucher id, it can be SalesQuoteId, SalesOrderId, CashSalesId - depending on the VoucherType
        /// </summary>
        public int VoucherId { get; set; }

        public VoucherType VoucherType { get; set; }

        public decimal TaxAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
