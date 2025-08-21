using ExceedERP.Core.Domain.Printing;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Printing.ViewModel
{
    public class PrintingProformaViewModel : PrintingCommonViewModel
    {
        public int ProformaNumber { get; set; }
        public string CustomerName { get; set; }
        public string DeliveryPeriod { get; set; }
        public string PriceValidityPeriod { get; set; }
        public string PaymentType { get; set; }
        public string ModeOfPayment { get; set; }
        public string Remark { get; set; }
        public decimal TransportCost { get; set; }
        public decimal TotalTransportCost { get; set; }
        public string VAT { get; set; }
        public decimal VATAmount { get; set; }

    }
    public class PrintingProformaItemsViewModel
    {

        public PrintJobType JobType { get; set; }
        public string JobDescription { get; set; }
        public decimal Quantity { get; set; }
        public int NoOfPage { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal TotalPrice { get; set; }
        public string VAT { get; set; }
        public string TransportVAT { get; set; }
        public decimal TransportCost { get; set; }
        public decimal SellingPrice { get; set; }
        public decimal SellingPriceIncludingTransCost { get; set; }
        public string Remark { get; set; }
        public string PaperSize { get; set; }
        public string BindingType { get; set; }
        public string TextPaperType { get; set; }
        public string CoverPaperType { get; set; }
        public int ColorText { get; set; }
        public int ColorCover { get; set; }
    }
    public class PrintTotalViewModel
    {
        public decimal Total { get; set; }
    }
    
}
