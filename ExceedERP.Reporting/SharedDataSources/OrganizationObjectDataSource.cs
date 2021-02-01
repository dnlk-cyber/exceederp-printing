using System;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
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
            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            if (Organization != null)
            {
                record.Name = Organization.TradeName;
                record.AmharicName = Organization.AmharicName;
                record.Abbreviation = Organization.Abbreviation;

                var address =
                    db.Addresses.Where(a =>
                        a.Reference == Organization.MainCompanyId.ToString() &&
                        a.ReferenceType.ToLower() == "company").ToList();
                if (address.Any())
                {
                    foreach (var a in address)
                    {

                        if (record.Address == null)
                        {
                            record.Address = a.Category + ":- " + a.Value;
                        }
                        else
                        {
                            record.Address += ", " + a.Category + "-" + a.Value;
                        }


                    }

                }
                var logo =
                    db.Attachments.FirstOrDefault(a => a.Reference == Organization.MainCompanyId.ToString() &&
                                                       a.ReferenceType == AttachmentType.LOGO);
                if (logo != null)
                {
                    record.LogoFileName = ByteArrayToImage(logo.BinaryFile);
                }

                record.Date =
                    XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
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