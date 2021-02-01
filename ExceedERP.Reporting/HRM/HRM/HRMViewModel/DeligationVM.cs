using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class DeligationVM
    {
        public string FromEmployee { get; set; }
        public string BranchFrom { get; set; }
        public string DepartmentFrom { get; set; }
        public string JobTitleFrom { get; set; }
        public string ToEmployee { get; set; }
        public string BranchTo { get; set; }
        public string DepartmentTo { get; set; }
        public string JobTitleTo { get; set; }
        public string Reason { get; set; }
        public string Period { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
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
        public string Date { get; set; }
    }
}
