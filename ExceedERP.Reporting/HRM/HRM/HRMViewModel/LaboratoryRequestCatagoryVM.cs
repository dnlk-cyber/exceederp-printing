using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class LaboratoryRequestCatagoryVM
    {
        public string Catagory { get; set; }
        public List<LaboratoryRequestTestsVM> LaboratoryRequestTests { get; set; }
    }
}
