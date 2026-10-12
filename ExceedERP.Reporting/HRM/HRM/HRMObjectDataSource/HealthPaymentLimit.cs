using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMObjectDataSource
{
    [DataObject]
    public class HealthPaymentLimit
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetPaymentLimitLists()
        {
            List<string> list = new List<string>();

            list.Add("On Limit");
            list.Add("Above Limit");

            return list;
        }
    }
}
