using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Inventory.DataSource.Print.ViewModel
{
    public class InspectorVm
    {
        
        public DateTime InspectionDate { get; set; }
        public string Employeename { get; set; }
        public bool Accepted { get; set; }
        public string Remark { get; set; }
    }
}
