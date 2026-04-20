using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    
   public class MechanicPerformanceEnum
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PerformanceVM> GetMechanic()
        {
            List<PerformanceVM> list = new List<PerformanceVM>();

            list.Add(new PerformanceVM { Selector = "Technician" });
            list.Add(new PerformanceVM { Selector = "ServiceCenter" });

            return list;
        }

    }

    public class PerformanceVM
    {
        public string Selector { get; set; }
    }
}
