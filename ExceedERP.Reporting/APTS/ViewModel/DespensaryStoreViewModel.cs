using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.APTS.ViewModel
{
   public class DespensaryStoreViewModel
    {
        public string DrugName { get; set; }
        public string DrugCode { get; set; }
        public string Unit { get; set; }
        public decimal CurrentAmount { get; set; }
        public decimal NewReceived { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
