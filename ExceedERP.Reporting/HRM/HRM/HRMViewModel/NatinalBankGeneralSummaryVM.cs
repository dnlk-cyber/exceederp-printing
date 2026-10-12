using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HRM.HRM.HRMViewModel
{
   public class NatinalBankGeneralSummaryVM
    {
        public string InstitutionName { get; set; }
        public string ReportType { get; set; } // e.g. "Semi Annual"
        public string PeriodEnding { get; set; }
        public string Region { get; set; }
        public string City { get; set; }
        public string SubCityOrWoreda { get; set; }
        public string Kebele { get; set; }
        public string MailingAddress { get; set; }

        public string Telephone { get; set; }
        public string Fax { get; set; }

        public string Website { get; set; }
        public string Email { get; set; }
        public HeadOfficeViewModel HeadOffice { get; set; }
        public List<BranchNetworkItemViewModel> BranchNetworks { get; set; }
        public string OperatingRegions { get; set; } // simple string or list if you want later


    }
    public class HeadOfficeViewModel
    {
        public AddressViewModel Address { get; set; }
    }
    public class BranchNetworkItemViewModel
    {
        public string BusinessUnit { get; set; }
        public int NumberOfBranches { get; set; }
        public int NumberOfSubBranches { get; set; }
    }

    public class AddressViewModel
    {
        public string Region { get; set; }
        public string City { get; set; }
        public string SubCityOrWoreda { get; set; }
        public string Kebele { get; set; }
        public string MailingAddress { get; set; }

        public string Telephone { get; set; }
        public string Fax { get; set; }

        public string Website { get; set; }
        public string Email { get; set; }
    }

}
