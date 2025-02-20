using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HelpDesk.ViewModel
{
   public class SoftwareLicenseVM
    {
        public string LicenseID { get; set; }
        public string SoftwareName { get; set; }
        public string Version { get; set; }
        public string Vendor { get; set; }
        public string LicenseType { get; set; }
        public string LicenseKey { get; set; }
        public DateTime PurchaseDate { get; set; }
        public DateTime LicenseExpiryOrRenewalDate { get; set; }
        public int NumberofLicenses { get; set; }
        public int NumberofUsers { get; set; }
        public decimal CostperLicense { get; set; }
        public decimal TotalCost { get; set; }
        public string Structure { get; set; }
        public int AssignedUsers { get; set; }
        public string LicenseStatus { get; set; }
        public string UsageLimitations { get; set; }
        public DateTime? RenewalNotificationDate { get; set; }
        public string SupportContact { get; set; }
        public string CommentsOrNotes { get; set; }

    }
}
