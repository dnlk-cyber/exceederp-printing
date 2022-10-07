using ExceedERP.Reporting.PrintingEstimation.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.ProductionFollowUp.ViewModel
{
   public class ProformaVM : ApprovalPropertyVM
    {
        public string InvoiceNo { get; set; }
        public string Date { get; set; }
        public string EstNo { get; set; }
        public string ValidityDate { get; set; }
        public string DeliveryDate { get; set; }
        public string TermsOfPayment { get; set; }
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string Telephone { get; set; }
        public string Fax { get; set; }
        public string Email { get; set; }
        public string PriceInWord { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Total { get; set; }
        
    }
}
