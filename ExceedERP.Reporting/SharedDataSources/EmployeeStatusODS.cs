using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
   public class EmployeeStatusODS
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetStatus()
        {

            List<string> str = new List<string>();
        
            str.Add("Active");
            str.Add("InActive");

            return str;
        }
    }
}
