using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.CRM.ViewModel;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

namespace ExceedERP.Reporting.CRM.DataSource
{
    [DataObject]
    public class CustomerListOds
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<CustomerViewModel> GetCustomers()
        {
            List<CustomerViewModel> list = new List<CustomerViewModel>();

            var customers = db.OrganizationCustomers.ToList();

            foreach (var c in customers)
            {
                var record = new CustomerViewModel
                {
                    CustomerId = c.OrganizationCustomerID,
                    Name = c.TradeName,
                    CustomerAccount = "",
                    CustomerType = GetCustomerCategory( c.Category),
                    ContactName = "",
                    CustomerStatus = c.IsActive ? "Active" : "Inactive",
                    TIN = c.TinNo
                };
                var customerAccounts = db.AccountMaps.Where(m => m.ReferenceType == "Organization_Customer" && m.Reference == c.OrganizationCustomerID.ToString()).ToList();
                if (customerAccounts.Any())
                {
                    foreach(var ac in customerAccounts)
                    {
                        record.CustomerAccount += ac.AccountID + " ";
                    }
                }
                var addresses = db.Addresses.Where(ad => ad.ReferenceType == "Organization_Customer" && ad.Reference == c.OrganizationCustomerID.ToString()).ToList();
                if (addresses.Any())
                {
                    var contactPersonAddress = addresses.FirstOrDefault(a => a.Category == Core.Domain.Common.AddressType.CONTACT_PERSON);
                    if(contactPersonAddress != null)
                    {
                        record.ContactName = contactPersonAddress.Value;
                    }
                }
                list.Add(record);
            }

            return list;
        }
        private string GetCustomerCategory(string id)
        {
            return db.OrganizationCategories.FirstOrDefault(c => c.OrganizationCategoryID.ToString() == id)?.Description;
        }
    }
}
