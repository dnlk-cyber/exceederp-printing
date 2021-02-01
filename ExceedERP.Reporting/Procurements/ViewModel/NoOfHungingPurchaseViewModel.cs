using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Procurements.ViewModel
{
   public class NoOfHungingPurchaseViewModel
    {
        public string ItemName { get; set; }
        public string Branch { get; set; }
        public string OpenTender { get; set; }
        public string SelectedTender { get; set; }
        public string Proforma { get; set; }
        public string DirectPurchase { get; set; }
    }
}
