using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Mega.ViewModel
{
   public class ProformaVM
    {
        public string CustomerName { get; set; }
        public string ContactName { get; set; }
        public string ContactPhone { get; set; }
        public string PriceInWord { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Vat { get; set; }
        public decimal Total { get; set; }
        
    }
}
