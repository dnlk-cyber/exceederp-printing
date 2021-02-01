using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Finance.DataSource
{
   
    [DataObject]
    public class OrderTypeOds
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetOrderType()
        {
            List<string> sourceList = new List<string>
            {
                "Cash",
                "Credit"
            };

            return sourceList;
        }

       
    }
}
