using System.ComponentModel;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.CRM
{
    public class CashSalesPartialLineItem : Operations
    {
        public int CashSalesPartialLineItemId { get; set; }
        public int LineItemID { get; set; }
        public int CashSalesPartialItemId { get; set; }
          [DisplayName("Partial Quantity")]
        public decimal PartialPayedAmount { get; set; }
          [DisplayName("Product")]
          [NotMapped]
        public string ItemName { get; set; }
          [NotMapped]
          public string  Store { get; set; }
          [NotMapped]
      [DisplayName("UOM")]
          public string ItemUOM { get; set; }
        [NotMapped]
        public decimal UnitPrice { get; set; }
        [NotMapped]
        public decimal Quantity { get; set; }
        [NotMapped]
        public bool IsPaymentProcessed { get; set; }
        [ForeignKey("LineItemID")]
        public virtual LineItem LineItem { get; set; }
        [ForeignKey("CashSalesPartialItemId")]
        public virtual CashSalesPartialItem CashSalesPartialItem { get; set; }

    }


}
