using System.Drawing;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
    public class ClearanceLetterData : AddressVm
    {
        public string FullName { get; set; }
        public string JobTitle { get; set; }
        public string Department { get; set; }
        public decimal Salary { get; set; }
        public string Reason { get; set; }
        public string TerminationDate { get; set; }
        public string Punishment { get; set; }
        public string Status { get; set; }
        public string Name { get; set; }
        public string AmharicName { get; set; }
        public string Abbreviation { get; set; }
        public string Address { get; set; }
        public Image LogoFileName { get; set; }
        public string Email { get; set; }
        public string WebSite { get; set; }
        public string City { get; set; }
        public string Fax { get; set; }
        public string Pobox { get; set; }
        public string Tel { get; set; }
        public byte[] SignatureImage { get; set; }
    }
}