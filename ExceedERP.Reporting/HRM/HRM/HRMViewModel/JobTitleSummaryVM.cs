using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
  
    public class JobTitleSummaryVM
    {
        public string Code { get; set; }
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string Email { get; set; }
        public string Tel { get; set; }
        public string WebSite { get; set; }
        public string Fax { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }
    public class BusinessUnitBRanchSummaryVM
    {
        public string Code { get; set; }
        public string BusinessUnitName { get; set; }
        public int NumberofBranchs { get; set; }
       
    }

}
