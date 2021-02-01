using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.SharedDataSources
{
   
    [DataObject]
    public class QuarterListDataSource
    {
       
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetQuarters()
        {
            List<string> quarters = new List<string> {"1","2","3","4"};

            return quarters;
        }

    }
}
