using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.ViewModel
{
   public class SalvageStoreViewModel
    {

         public string EquipmentName { get; set; }
         public string SparePart { get; set; }
         public Nullable<int> VoucherNo { get; set; }
        public Nullable<DateTime> Date { get; set; }

        public string Description { get; set; }
     


      
    }
}
