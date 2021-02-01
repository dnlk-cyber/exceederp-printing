using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.APTS.ViewModel
{
    public class DrugSaleWithDrugistViewModel
    {
        public string DrugistName { get; set; }
        public decimal Price { get; set; }
        public decimal Quantity { get; set; }
        public int NoOfPatient { get; set; }
    }
}
