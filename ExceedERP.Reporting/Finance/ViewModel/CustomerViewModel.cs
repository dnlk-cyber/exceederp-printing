using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Finance.ViewModel
{
    public class CustomerListViewModel
    {
        [Display(Name = " Company Name")]
        public string CompanyName { get; set; }
        [Display(Name = "Customer ID")]
        public string ID { get; set; }
        public string Account { get; set; }
        public string TINNo { get; set; }
        [Display(Name = "Customer Type")]
        public string CustomerType { get; set; }
        [Display(Name = "Contact Name")]
        public string ContactName { get; set; }
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }
        [Display(Name = "Address")]
        public string Address { get; set; }
        [Display(Name = "Country")]
        public string Country { get; set; }
        public string IsActive { get; set; }
    }

    public class PartnerListViewModel
    {
        public string Code { get; set; }
        public string Name { get; set; }
        public int PartnerId { get; set; }
        public string Country { get; set; }
        public string IsActive { get; set; }
    }
}
