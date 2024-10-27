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
    public class TransactionTypeOds : IDisposable
    {
        private readonly ExceedDbContext _db;
        public TransactionTypeOds()
        {
            _db = new ExceedDbContext();
        }

       

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetApTransactionType()
        {
           return new List<string>
            {
                "Invoice",
                "Payment",
                "Misc Payment"
            };
        }

        public List<string> GetArTransactionType()
        {
            return new List<string>
            {
                "Invoice",
                "Collection",
                "Misc Collection"

            };
        }

        public void Dispose()
        {
            _db?.Dispose();
        }
    }
}
