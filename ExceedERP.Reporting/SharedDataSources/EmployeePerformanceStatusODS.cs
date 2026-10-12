using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
   public class EmployeePerformanceStatusODS
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetEmployeePerformanceStatus()
        {

            List<string> str = new List<string>();
        
            str.Add("In-Progress");
            str.Add("Not-Evaluated");

            return str;
        }
    }
}
