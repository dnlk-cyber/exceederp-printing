using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Reporting.CRM.Printing.ViewModel
{
    public  class PriceEstimationPrintViewModel
    {

        [DisplayName("# Quote")]
        public string QuoteNumber { get; set; }

        [DisplayName("Customer")]
        public string Customer { get; set; }
        
        public DateTime Date { get; set; }

        public string PlateNo { get; set; }
        public string Equipment { get; set; }

        public decimal GrandTotal { get; set; }
        public string TotalInWords { get; set; }

        public List<PriceEstimationPrintLineItemViewModel> LineItemList { get; set; } = new List<PriceEstimationPrintLineItemViewModel>();
        public List<PriceEstimationPrintServiceLineItemViewModel> ServiceLineItemList { get; set; } = new List<PriceEstimationPrintServiceLineItemViewModel>();
        public decimal SubTotal { get; internal set; }
        public decimal TotalTax { get; internal set; }
        public decimal TotalWithholding { get; internal set; }
        public decimal NetPay { get; internal set; }

        public string PreparedBy { get; set; }
        public string CheckedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string NetPayDisplayText { get; internal set; }

        public string BanckAccount { get; set; }
        public string VATNo { get; set; }
        public string TINNo { get; set; }
    }

    public class PriceEstimationPrintLineItemViewModel
    {
        [DisplayName("Product Name")]
        public string ProductName { get; set; }

        public decimal Quantity { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal SubTotoal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal WithholdingAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }

    public class PriceEstimationPrintServiceLineItemViewModel
    {
        public string ServiceName { get; set; }

        public decimal UnitAmount { get; set; }
        public decimal SubTotoal { get; set; }
        public decimal TaxAmount { get; set; }
        public decimal WithholdingAmount { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
