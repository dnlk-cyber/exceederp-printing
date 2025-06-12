using ExceedERP.Reporting.CRM.ViewModel;
using System;
using System.Collections.Generic;

namespace ExceedERP.Reporting.CRM.Printing.ViewModel
{
    public class SalesInvoicePrintViewMode : OrganizationFullAddress
    {
        public int InvoiceNumber { get; set; }

        public string OrderNumber { get; set; }

        public string Consignee { get; set; }
        public string CustomerAddress { get; set; }
        public string PaymentMethod { get; set; }
        public string Remark { get; set; }
        public string SalesPerson { get; set; }
        public string Store { get; set; }
     
        public string CustomerType { get; set; }

        public string InvoiceDate { get; set; }
        public DateTime OrderDate { get; set; }
        public decimal OrderQuantity { get; set; }
        

        public string Status { get; set; }

        public bool IsOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        public DateTime? OnlineApprovedTime { get; set; }
        public bool IsOnlineTransferred { get; set; }
        public string IsOnlineTransferredBy { get; set; }
        public DateTime? OnlineTransferredTime { get; set; }
        public string SendForApprovalBy { get; set; }
        public DateTime? SendForApprovalTime { get; set; }

        public string RequestedBy { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }

        public string PaymentTerm { get; set; }
        public string CustomerTinNumber { get; internal set; }
        public string TotalAmountInWords { get; internal set; }
        public string Branch { get; internal set; }
        public string Shop { get; internal set; }
        public string PrintStatus { get; internal set; }
        public string ReceiptReference { get; internal set; }
        public string FsInvoiceNumber { get; internal set; }

        public decimal TotalAmount { get; internal set; }
        public decimal TotalTax { get; internal set; }
        public decimal TotalWithholding { get; internal set; }
        public decimal NetPay { get; internal set; }
        public List<SalesInvoicePrintLineItemViewModel> LineItemList { get; set; } = new List<SalesInvoicePrintLineItemViewModel>();
        public List<SalesInvoicePrintServiceLineItemViewModel> ServiceLineItemList { get; set; } = new List<SalesInvoicePrintServiceLineItemViewModel>();
        public string NetPayDisplayText { get; internal set; }
        public decimal TotalVATWithholdingAmount { get; internal set; }
    }

    public class SalesInvoicePrintLineItemViewModel
    {
        public string ProductName { get; set; }
        public string UOM { get; set; }

        public decimal Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal SubTotoal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal WithholdingAmount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal VatWithholdingAmount { get; internal set; }
    }

    public class SalesInvoicePrintServiceLineItemViewModel
    {
        public string ServiceName { get; set; }

        public decimal UnitAmount { get; set; }
        public decimal SubTotoal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal WithholdingAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}

