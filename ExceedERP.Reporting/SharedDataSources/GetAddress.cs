using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class GetAddress
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AddressVm> GetAddressInfo()
        {
            List<AddressVm> addressList = new List<AddressVm>();

            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            if (Organization != null)
            {
               
                var address =
                    db.Addresses.Where(a =>
                            a.Reference == Organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {
                        AddressVm addr = new AddressVm();
                        var fax = address.FirstOrDefault(x=>x.Category == AddressType.FAX && x.Value != null);                       
                            addr.Fax = "Fax: " + fax.Value;

                        var tel = address.FirstOrDefault(x => x.Category == AddressType.OFFICE_PHONE && x.Value != null);                       
                            addr.Tel = "Tel: " + tel.Value;

                        var POBox = address.FirstOrDefault(x => x.Category == AddressType.POBOX && x.Value != null);                       
                            addr.POBox = "P.O.Box: " + POBox.Value;

                        var email = address.FirstOrDefault(x => x.Category == AddressType.OFFICE_EMAIL && x.Value != null);                        
                            addr.Email = "E-mail: " + email.Value;

                        var locatCountry = address.FirstOrDefault(x => x.Category == AddressType.COUNTRY && x.Value != null);
                        var locatCity = address.FirstOrDefault(x => x.Category == AddressType.CITYPROVINCE && x.Value != null);
                        if(locatCity != null && locatCountry != null)
                        {
                            addr.Location = locatCity.Value + ", " + locatCountry.Value;
                        }
                      
                        //if (a.Category == AddressType.CITYPROVINCE)
                        //{
                        //    addr.City =  a.Value;
                        //}
                        //if (a.Category == AddressType.COUNTRY)
                        //{
                        //    addr.Country = a.Value;
                        //}
                    }

                }
            }
                return null;
        }
    }

    public class AddressVm
    {
        public string Tel { get; set; }
        public string Fax { get; set; }
        public string POBox { get; set; }
        public string Email { get; set; }
        public string Location { get; set; }
        public string City { get; set;  }
        public string Country { get; set; }
        public string WebSite { get; set; }
    }
}
