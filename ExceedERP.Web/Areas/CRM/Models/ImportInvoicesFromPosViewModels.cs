using System;
using System.Collections.Generic;

namespace ExceedERP.Web.Areas.CRM.Models
{
    //public class ImportInvoicesFromPosViewModels
    //{
    //}
    public class ConfirmPaymentViewModel
    {
        public ConfirmPaymentViewModel()
        {
            this.Items = new List<ConfirmPaymentItemViewModel>();
        }
        public string TransactionType { get; set; }
        public DateTime SoldDate { get; set; }
        public string FsInvoiceNo { get; set; }
        public string EJNo { get; set; }
        public string PaymentType { get; set; }
        public string CheckNo { get; set; }
        public string MachineID { get; set; }
        public string CustomerName { get; set; }
        public string CustomerTIN { get; set; }
        public string InvoiceNumber { get; set; }
        public string Memo { get; set; }

        public decimal TotalAdditionalAmount { get; set; }
        public decimal TotalDiscountAmount { get; set; }
        public decimal TotalBeforeTax { get; set; }
        public decimal TotalTax { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal WithHoldingTAXAmount { get; set; }
        public string UserName { get; set; }
        public string IsVANSales { get; set; }

        public virtual ICollection<ConfirmPaymentItemViewModel> Items { get; set; }

        //public List<ConfirmPaymentItemViewModel> SalesDataDetails = new List<ConfirmPaymentItemViewModel>();
    }
    public class ConfirmPaymentItemViewModel
    {
        public string ItemIdentifierID { get; set; }
        public string ItemName { get; set; }
        public decimal ItemTax { get; set; }
        public decimal ItemUnitPrice { get; set; }
        public string UomIdentifierID { get; set; }
        public string ItemUom { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal TotalItemAddition { get; set; }
        public decimal TotalItemBeforeTax { get; set; }
        public decimal TotalItemDiscount { get; set; }
        public decimal TotalItemPrice { get; set; }
    }
    public class ConfirmPaymentStatusViewModel
    {
        public string Ex { get; set; }
        public string Message { get; set; }
        public bool Success { get; set; }
        public string Content { get; set; }

    }
}