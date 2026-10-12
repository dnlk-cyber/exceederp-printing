using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Inventory;
using ExceedERP.Reporting.SharedDataSources.ViewModel;

namespace ExceedERP.Reporting.SharedDataSources
{
    [DataObject]
    public class OrganizationObjectDataSource : Organization
    {
        private ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public Organization GetOrganization()
        {
            Organization record = new Organization();

            try
            {
                var organization = db.Companies.FirstOrDefault(x => x.Category == "0");
                if (organization == null)
                    return record; // Return empty record safely

                record.Name = organization.TradeName;
                record.AmharicName = organization.AmharicName;
                record.Abbreviation = organization.Abbreviation;

                // ✅ Build address safely
                var addresses = db.Addresses
                    .Where(a => a.Reference == organization.MainCompanyId.ToString() &&
                                a.ReferenceType.ToLower() == "company")
                    .ToList();

                if (addresses.Any())
                {
                    record.Address = string.Join(", ",
                        addresses.Select(a => $"{a.Category}: {a.Value}"));
                }

                // ✅ Load logo safely
                var logo = db.Attachments.FirstOrDefault(a =>
                    a.Reference == organization.MainCompanyId.ToString() &&
                    a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    var img = ByteArrayToImage(logo.BinaryFile);
                    if (img != null)
                        record.LogoFileName = img;
                }
          

                //if (logo?.BinaryFile != null && logo.BinaryFile.Length > 0)
                //{
                //    record.LogoFileName = ByteArrayToImage(logo.BinaryFile);
                //}

                // ✅ Load ISO logo safely
                var isoLogo = db.Attachments.FirstOrDefault(a =>
                    a.Reference == organization.MainCompanyId.ToString() &&
                    a.ReferenceType == AttachmentType.ISO_LOGO);

                if (isoLogo?.BinaryFile != null && isoLogo.BinaryFile.Length > 0)
                {
                    record.IsoLogoFileName = ByteArrayToImage(isoLogo.BinaryFile);
                }

                // ✅ Ethiopian date
                record.Date = XAPI.EthiopicDateTime.GetEthiopicDate(
                    DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year) + " E.C";
            }
            catch (Exception ex)
            {
                // You can log this, but it won't crash the report
                System.Diagnostics.Debug.WriteLine("GetOrganization() failed: " + ex.Message);
                // Return partially filled record safely
                return record;
            }

            return record;
        }

      


        public static Image ByteArrayToImage(byte[] byteArrayIn)
        {
            if (byteArrayIn != null)
            {
                MemoryStream ms = new MemoryStream(byteArrayIn);
                Image returnImage = Image.FromStream(ms);
                return returnImage;
            }
            return null;

        }



    }
}