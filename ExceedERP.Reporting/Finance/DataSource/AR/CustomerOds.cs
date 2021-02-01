using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;
//using ExceedERP.Core.Domain.Finance.Report;

namespace ExceedERP.Reporting.Finance.DataSource.AR
{
    [DataObject]
    public class CustomerOds : List<CustomerListViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CustomerListViewModel> GetCustomerList()
        {
            List<CustomerListViewModel> list = new List<CustomerListViewModel> { };

            var customerlist = _db.ARCustomerProfiles.Select(x => new { x.CompanyName, x.CustomerId, x.AccountCode, x.TinNo, x.CustomerType, x.ContactName, x.PhoneNumber, x.Address, x.Country, x.IsActive }).ToList();

            foreach (var item in customerlist)
            {
                var record = new CustomerListViewModel
                {
                    CompanyName = item.CompanyName,
                    ID = item.CustomerId,
                    Account = item.AccountCode,
                    TINNo = item.TinNo,
                    CustomerType = item.CustomerType,
                    ContactName = item.ContactName,
                    PhoneNumber = item.PhoneNumber,
                    Address = item.Address,
                    Country = item.Country,
                    IsActive = item.IsActive ? "Yes" : "No"
                };
                list.Add(record);
            }
            return list;
        }

       
    }
}
