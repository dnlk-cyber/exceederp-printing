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
    public class TransactionTypeOds
    {
        private ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetApTransactionType()
        {
            List<string> sourceList = new List<string>
            {
                "Invoice",
                "Payment",
                "Misc Payment"
            };

            return sourceList;
        }

        public List<string> GetArTransactionType()
        {
            List<string> sourceList = new List<string>
            {
                "Invoice",
                "Collection",
                "Misc Collection"

            };

            return sourceList;
        }
    }
}
