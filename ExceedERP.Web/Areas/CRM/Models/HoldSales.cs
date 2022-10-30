using System;
using System.Collections.Generic;

namespace ExceedERP.Web.Areas.CRM.Models
{
    public class HoldSales
    {
        //[Key]
        public string HoldSalesIdentifierId { get; set; }
        public string TransactionType { get; set; }
        public string InvoiceNo { get; set; }
        public string PaymentType { get; set; }
        public string TableNumber { get; set; }
        public string SalesPerson { get; set; }
        public string HoldMemo { get; set; }
        public DateTime Date { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTIN { get; set; }
        public string CustomerVAT { get; set; }
        public string CashierUpdated { get; set; }
        public string POSId { get; set; }

        public List<HoldSalesItem> HoldSalesItems { get; set; } = new List<HoldSalesItem>();
    }

    public class HoldSalesItem
    {
        //[Key]
        public string HoldSalesItemIdentifierId { get; set; }
        //public string HoldSalesIdentifierId { get; set; }
        public string CategoryIdentifierId { get; set; }
        public string CategoryName { get; set; }
        public string ItemIdentifierId { get; set; }
        public string ItemDescription { get; set; }
        public string ItemCode { get; set; }
        public string UomIdentifierId { get; set; }
        public string UomName { get; set; }
        public decimal Quantity { get; set; }
        public decimal SalesUnitPrice { get; set; }
        public int TaxType { get; set; }

        //public virtual HoldSales HoldSales { get; set; }
    }

    public class HoldSalesResponseViewModel
    {
        public string Ex { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public bool Success { get; set; }
        public HoldSales Content { get; set; } = new  HoldSales();
    }
}