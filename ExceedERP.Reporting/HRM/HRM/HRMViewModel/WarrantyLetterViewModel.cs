using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class WarrantyLetterViewModel
    {
        public string Name { get; set; }
        public int EmployeeId { get; set; }
        public string Company { get; set; }
        public string Adress { get; set; }
        public decimal Salary { get; set; }
        public string SalaryInWord { get; set; }
        public string RequestDate { get; set; }
        public string Jobtitle { get; set; }
        public string EmployeeStatus { get; set; }

        public string TradeName { get; set; }
        public string AmharicName { get; set; }
        public string Abbreviation { get; set; }
        public Image LogoFileName { get; set; }
        public string OrgAddress { get; set; }

        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }
        public string Address { get; set; }
        public string TempOne { get; set; }
    }
}
