using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.CRM.ViewModel
{
    public class SalesQuote : Organization
        {
        [DisplayName("Quote Number")]
        public int QuoteNumber { get; set; }

        [DisplayName("Customer")]
        public string Consignee { get; set; }
          [DisplayName("Product Name")]
        public string ProductName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime IssuedDate { get; set; }
        public decimal Quantity { get; set; }
        public decimal SubTotoal { get; set; }
        public decimal Tax { get; set; }
        public decimal GrandTotal { get; set; }

        public string UOM { get; set; }

        public string Status { get; set; }


    
      
        }
    }