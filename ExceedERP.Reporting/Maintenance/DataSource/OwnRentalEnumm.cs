using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    
   public class OwnRentalEnumm
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<OwnRentalVM> GetIntExt()
        {
            List<OwnRentalVM> list = new List<OwnRentalVM>();

            list.Add(new OwnRentalVM { Selector = "Own" });
            list.Add(new OwnRentalVM { Selector = "Rent" });

            return list;
        }

    }

    public class OwnRentalVM
    {
        public string Selector { get; set; }
    }
}
