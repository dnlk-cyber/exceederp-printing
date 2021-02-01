using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.Core.Domain.Finance.AR.SalesOrder;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Reporting.Finance.DataSource.Sales
{

    [DataObject]
    public class SalesOrderCustomerObjectDataSource
    {
        readonly ExceedDbContext _db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> GetCustomer()
        {
            List<string> customerList = new List<string>();
     
            var distinctSalesOrderCustomers = _db.ArSalesOrders.OrderBy(x=>x.CustomerName).Select(x=>x.CustomerName).Distinct().ToList();
            customerList.AddRange(distinctSalesOrderCustomers);
            return customerList;
        }
    }
}
