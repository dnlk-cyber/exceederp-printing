using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class CompanyStructureVM
    {
        public string Parent { get; set; }
        public List<ChildStructureVM> ChildStructure { get; set; }
    }
    public class ChildStructureVM
    {
        public string Child { get; set; }
    }
}
