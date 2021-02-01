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
    public class OrganizationWithVacancyObjectDataSource : Organization
    {
        private ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public Organization GetOrganization(string vacancyCod, string note,bool header)
        {

            Organization record = new Organization();
            var Organization = db.Companies.FirstOrDefault(x => x.Category == "0");
            if (header)
            {
                if (Organization != null)
                {

                    record.Name = Organization.TradeName;
                    record.AmharicName = Organization.AmharicName;
                    record.Abbreviation = Organization.Abbreviation;
                    record.Note = note;
                    var address =
                        db.Addresses.Where(a =>
                            a.Reference == Organization.MainCompanyId.ToString() &&
                            a.ReferenceType.ToLower() == "company").ToList();
                    if (address.Any())
                    {
                        ///////////

                        foreach (var a in address)
                        {
                            if (a.Category == AddressType.KIFLEKETEMA)
                            {
                                record.Address = "Subcity: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.KEBELE)
                            {
                                record.Address += "Kebele: " + a.Value + " ";
                            }
                            if (a.Category == AddressType.CITYPROVINCE)
                            {
                                if (record.Address == null)
                                {
                                    record.Address += a.Value + ", Ethiopia";
                                }
                            }
                            else if (a.Category == AddressType.POBOX)
                            {

                                record.POBox = "P.O.Box: " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_EMAIL)
                            {

                                record.Email = "Email:  " + a.Value;

                            }
                            else if (a.Category == AddressType.WEBSITE)
                            {

                                record.WebSite = "Website: " + a.Value;

                            }
                            else if (a.Category == AddressType.FAX)
                            {

                                record.Fax = "Fax:   " + a.Value;

                            }
                            else if (a.Category == AddressType.OFFICE_PHONE)
                            {

                                record.Tel = "Tel:   " + a.Value;

                            }


                        }


                        ////////////
                        //foreach (var a in address)
                        //{

                        //    if (record.Address == null)
                        //    {
                        //        record.Address = a.Category + ":- " + a.Value;
                        //    }
                        //    else
                        //    {
                        //        record.Address += ", " + a.Category + "-" + a.Value;
                        //    }

                        //}

                    }
                    var logo =
                        db.Attachments.Where(
                            a =>
                                a.Reference == Organization.MainCompanyId.ToString() &&
                                a.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
                    if (logo != null)
                    {
                        record.LogoFileName = ByteArrayToImage(logo.BinaryFile);
                    }
                }
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