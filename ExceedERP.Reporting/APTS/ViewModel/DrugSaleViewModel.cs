using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.APTS.ViewModel
{
   public class DrugSaleViewModel
    {
        public string ClientName { get; set; }
        public string CardNo { get; set; }
        public string Sex { get; set; }
        public int Age { get; set; }
        public string HospitalName { get; set; }    
        public List<DrugSaleDetailViewModel> DrugSaleDetailViewModel { get; set; }
        public string Sign { get; set; }
       
    }
}
