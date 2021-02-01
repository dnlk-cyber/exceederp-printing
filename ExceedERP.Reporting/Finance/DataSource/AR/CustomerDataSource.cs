using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Finance.ViewModel;

namespace ExceedERP.Reporting.Finance.DataSource.AR
{
    [DataObject]
    public class CustomerDataSource : List<CustomerListViewModel>
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CustomerListViewModel> GetCustomerList(string custype)
        {
            List<CustomerListViewModel> list = new List<CustomerListViewModel> { };

            var customerlist = _db.ARCustomerProfiles
                .Select(x => new { x.CompanyName, x.CustomerId, x.AccountCode, x.TinNo, x.CustomerType, x.ContactName, x.PhoneNumber,x.Address,x.Country,x.IsActive }).ToList();
            if (!String.IsNullOrEmpty(custype))
            {
                customerlist = customerlist.Where(x => x.CustomerType.ToUpper() == custype.ToUpper())
                    .ToList();
            }
            foreach (var customer in customerlist)
            {
                var record = new CustomerListViewModel
                {
                    CompanyName = customer.CompanyName,
                    ID = customer.CustomerId,
                    Account = customer.AccountCode,
                    TINNo = customer.TinNo,
                    CustomerType = customer.CustomerType,
                    ContactName = customer.ContactName,
                    PhoneNumber = customer.PhoneNumber,
                    Address = customer.Address,
                    Country = customer.Country,
                    IsActive = customer.IsActive ? "Yes" : "No"
                };
                list.Add(record);
            }
            return list;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<string> CustomerCategory()
        {

            List<string> list = new List<string>();
            list.AddRange(_db.ARCustomerTypes.Select(x => x.CustomerTypeName).ToList());
            return list;
        }
    }
}
