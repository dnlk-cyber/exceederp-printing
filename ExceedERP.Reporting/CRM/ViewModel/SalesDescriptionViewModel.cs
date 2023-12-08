using System;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class SalesDescriptionViewModel
    {
        public string BuyerTin { get; set; }
        public decimal   MRC { get; set; }
        public string ReceiptNumber { get; set; }
        public string FsInvoiceNumber { get; set; }
        public DateTime ReceiptDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal Price { get; set; }
        public decimal TotalAmount { get; set; }
        public string ItemCode { get; set; }
        public string ItemName { get; set; }
        public string Description { get; set; }

    }

    public class ReceiptCategory
    {
        // Receipt Category
        public const string CashSalesFiscalReceipt = "Cash Sales Fiscall Receipt";
        public const string CashSalesManualReceipt = "Cash Sales Manual Receipt";
        public const string CreditSalesFiscalReceipt = "Credit Sales Fiscal Receipt";
        public const string CreditSalesManualReceipt = "Credit Sales Manual Receipt";
        //public const string Other = "Other";

    }
    public class PaymentMethod
    {
        public const string CreditPaymentMethod = "CREDIT";
        public const string CashPaymentMethod = "CASH";
    }
}
