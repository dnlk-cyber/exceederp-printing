using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.HRM.Payroll.ViewModel;

namespace ExceedERP.Reporting.HRM.Payroll.DataSource
{
    [DataObject]
    public class EmployeeStatusOds : List<string>
    {
       

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetStatus()
        {
            List<string> list = new List<string> { };

            list.Add("Active");
            list.Add("InActive");

            return list;
            
        }

    }
}