using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.CRM.Printing.ViewModel
{
    public  class QuotePrintViewModel : Organization
    {

        [DisplayName("# Quote")]
        public int QuoteNumber { get; set; }

        [DisplayName("Customer")]
        public string Consignee { get; set; }
        public string CustomerType { get; set; }
        public string CustomerAddress { get; set; }
        public string SalesPerson { get; set; }
        public string PaymentMethod { get; set; }
        
        [DisplayName("Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime QuoteDate { get; set; }
       
        public decimal GrandTotal { get; set; }

        public string Status { get; set; }


        public string RequestedBy { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }

        public bool IsOnlineApproved { get; set; }
        public string OnlineApprovedBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OnlineApprovedTime { get; set; }
        public bool IsOnlineTransferred { get; set; }
        public string IsOnlineTransferredBy { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? OnlineTransferredTime { get; set; }
       
        public string SendForApprovalBy { get; set; }
        public DateTime? SendForApprovalTime { get; set; }
        public DateTime? CheckedTime { get; set; }

        public DateTime? DueDate { get; internal set; }
        public string Store { get; internal set; }
        public decimal SubTotal { get; internal set; }
        public string TotalAmountInWords { get; internal set; }
        public decimal TotalTax { get; internal set; }
        public decimal TotalWithholding { get; internal set; }
        public decimal NetPay { get; internal set; }
        public string NetPayDisplayText { get; internal set; }
        public decimal TotalVATWithholdingAmount { get; internal set; }

        public string PlateNo { get; set; }
        public string Equipment { get; set; }
        public string BanckAccount { get; set; }
        public string VATNo { get; set; }
        public string TINNo { get; set; }

        public List<QuotePrintLineItemViewModel> LineItemList { get; set; } = new List<QuotePrintLineItemViewModel>();
        public List<QuotePrintServiceLineItemViewModel> ServiceLineItemList { get; set; } = new List<QuotePrintServiceLineItemViewModel>();
       
    }

    public class QuotePrintLineItemViewModel
    {
        [DisplayName("Product Name")]
        public string ProductName { get; set; }
        //public string Variety { get; set; }
        //public string Class { get; set; }
        public string UOM { get; set; }

        public decimal Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal SubTotoal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal WithholdingAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class QuotePrintServiceLineItemViewModel
    {
        public string ServiceName { get; set; }

        public decimal UnitAmount { get; set; }
        public decimal SubTotoal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal WithholdingAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
