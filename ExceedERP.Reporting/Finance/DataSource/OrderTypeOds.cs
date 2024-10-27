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
    public class OrderTypeOds: IDisposable
    {
        private readonly ExceedDbContext _db;
        public OrderTypeOds()
        {
            _db = new ExceedDbContext();
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetOrderType()
        {
            return new List<string>
            {
                "Cash",
                "Credit"
            };
        }

        public void Dispose()
        {
            _db?.Dispose();
        }

    }
}
