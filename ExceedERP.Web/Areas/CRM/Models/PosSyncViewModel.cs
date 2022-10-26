using System.Collections.Generic;

namespace ExceedERP.Web.Areas.CRM.Models
{
    public class PosSyncViewModel
    {
        public string Ex { get; set; }
        public string Message { get; set; }
        public string Status { get; set; }
        public bool Success { get; set; }

        public List<PosSalesDataViewModel> Content { get; set; } = new List<PosSalesDataViewModel>();
    }
    public class PosSalesDataViewModel
    {
        public string CheckNo { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTIN { get; set; }
        public string EJNo { get; set; }
        public string FsInvoiceNo { get; set; }
        public string InvoiceNumber { get; set; }
        public string MachineID { get; set; }
        public string Memo { get; set; }
        public string PaymentType { get; set; }

        public List<PosSalesDataDetailViewModel> SalesDataDetails { get; set; } = new List<PosSalesDataDetailViewModel>();

    }
    public class PosSalesDataDetailViewModel
    {
        public string ItemIdentifierId { get; set; }
        public string ItemName { get; set; }
        public decimal ItemTax { get; set; }
        public decimal ItemUnitPrice { get; set; }
        public string ItemUom { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalItemAddition { get; set; }
        public decimal TotalItemBeforeTax { get; set; }
        public decimal TotalItemDiscount { get; set; }
        public decimal TotalItemPrice { get; set; }
        public string UomIdentifierId { get; set; }
    }
}