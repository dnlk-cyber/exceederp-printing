using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HelpDesk.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.HelpDesk.DataSource
{
    [DataObject]
    public class SoftwareLicenseODS
    {
        private ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SoftwareLicenseVM> GetMainteined(string softwareName,string vendor,string status )
        {

            List<SoftwareLicenseVM> softwareLicenseVMs = new List<SoftwareLicenseVM>();

           
            var softwareLicenses = db.SoftwareLicenses.ToList();
            if (!string.IsNullOrEmpty(softwareName))
            {
                softwareLicenses = softwareLicenses.Where(x => x.SoftwareName.ToUpper().Contains(softwareName.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(vendor))
            {
                softwareLicenses = softwareLicenses.Where(x => x.Vendor.ToUpper().Contains(vendor.ToUpper())).ToList();
            }
            if (!string.IsNullOrEmpty(status))
            {
                softwareLicenses = softwareLicenses.Where(x => x.LicenseStatus.ToString()==status).ToList();
            }
           
            foreach (var softwareLicense in softwareLicenses)
            {
                var department = db.MainCompanyStructures.Find(softwareLicense.OrgStructureId);
                var softwareLicenseVM = new SoftwareLicenseVM
                {
                    LicenseID = softwareLicense.LicenseId,
                    AssignedUsers = softwareLicense.AssignedUsers,
                    CommentsOrNotes = softwareLicense.CommentsOrNotes,
                    CostperLicense = softwareLicense.CostperLicense,
                    Structure = department?.Name,
                    PurchaseDate = softwareLicense.PurchaseDate,
                    LicenseExpiryOrRenewalDate = softwareLicense.LicenseExpiryOrRenewalDate,
                    LicenseKey = softwareLicense.LicenseKey,
                    LicenseStatus = softwareLicense.LicenseStatus.ToString(),
                    LicenseType = softwareLicense.LicenseType.ToString(),
                    NumberofLicenses = softwareLicense.NumberofLicenses,
                    NumberofUsers = softwareLicense.NumberofUsers,
                    RenewalNotificationDate = softwareLicense.RenewalNotificationDate,
                    SoftwareName = softwareLicense.SoftwareName,
                    SupportContact = softwareLicense.SupportContact,
                    TotalCost = softwareLicense.TotalCost,
                    UsageLimitations = softwareLicense.UsageLimitations,
                    Vendor = softwareLicense.Vendor,
                    Version = softwareLicense.Version,

                };
               
                softwareLicenseVMs.Add(softwareLicenseVM);
            }

            return softwareLicenseVMs;
        }
    }
}
