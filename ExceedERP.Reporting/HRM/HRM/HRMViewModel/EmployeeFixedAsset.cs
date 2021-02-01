using System.Collections.Generic;
using System.Drawing;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class EmployeeFixedAsset
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Department { get; set; }
        public string JobTitle { get; set; }
       public string Branch { get; set; }

        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string EnglishName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }
        public List<UserCardItemsList> UserCardItemsList { get; set; }
    }
}