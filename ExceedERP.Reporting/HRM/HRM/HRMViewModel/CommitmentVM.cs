using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class CommitmentVM
    {        
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

        public string EmployeeCode { get; set; }
        public string EmployeeName { get; set; }
        public string Type { get; set; }
        public string Reason { get; set; }
        public string CommitmentStartDate { get; set; }
        public string CommitmentEndDate { get; set; }
        public string CommitmentValue { get; set; }
        public List<CosignVM> Cosign { get; set; }
    }
}
