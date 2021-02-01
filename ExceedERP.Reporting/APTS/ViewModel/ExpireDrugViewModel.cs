using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.APTS.ViewModel
{
   public class ExpireDrugViewModel
    {
        public string DrugCode { get; set; }
        public string DrugName { get; set; }
        public string ExpireDate { get; set; }
        public string EntryDate { get; set; }
        public decimal DateLeft { get; set; }
        public decimal Amount { get; set; }
    }
}
