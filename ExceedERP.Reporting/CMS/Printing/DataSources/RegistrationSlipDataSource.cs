using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.CMS;
using ExceedERP.Reporting.CMS.Printing.ViewModels;
using ExceedERP.Reporting.SharedDataSources;
using System.ComponentModel;
using System.Linq;
using System.Web.Hosting;
//using System.Web;

namespace ExceedERP.Reporting.CMS.Printing.DataSource
{
    [DataObject]
    class RegistrationSlipDataSource
    {
        private readonly ExceedDbContext db = new ExceedDbContext();



        [DataObjectMethod(DataObjectMethodType.Select)]
        public RegistrationSlipViewModel GetRegistrationData(string traineeId)
        {

            RegistrationSlipViewModel record = new RegistrationSlipViewModel();


            //var logo =
            //    db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
            //            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
            //if (logo != null)
            //{
            //    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
            //}
            //}

            


            if (!string.IsNullOrEmpty(traineeId))
            {

                var trainee = db.CmsTrainees.FirstOrDefault(so => so.CmsTraineeId.ToString() == traineeId);

                if (trainee != null)
                {

                    string profilePicPath = HostingEnvironment.MapPath("~/Content/Images/CMS/Profiles/" + trainee.ProfilePicFileName);
                    //string file = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/CMS/Profiles/" + trainee.ProfilePicFileName);
                    //string mimeType = MimeMapping.GetMimeMapping(file);

                    byte[] profileImageStream = System.IO.File.ReadAllBytes(profilePicPath);

                    string barcodePath = HostingEnvironment.MapPath("~/Content/Images/CMS/Barcodes/" + $"barcode-{trainee.CmsTraineeId}.png");
                   
                    byte[] barcodeReadStream = System.IO.File.ReadAllBytes(barcodePath);

                    record = new RegistrationSlipViewModel
                    {
                        FullName = trainee.FullName,

                        MobilePhone = trainee.MobilePhoneNumber,
                        RegistrationDate = trainee.RegistrationDate,
                        RegistrationNumber = trainee.RegistrationNumber,
                        ProfilePicture = OrganizationObjectDataSource.ByteArrayToImage(profileImageStream),
                        Barcode = OrganizationObjectDataSource.ByteArrayToImage(barcodeReadStream),
                        Program = CMSGenericHelper.GetProgramById(trainee.CMSProgramId),
                        Course = CMSGenericHelper.GetCourseById(trainee.CMSCourseId),
                        CompanyName = trainee.CompanyName,
                        Type =(int)trainee.Type
                    };
                }
                return record;
            }

            return null;
        }


       


        //private string GetCustomerCategory(string catId)
        //{
        //    return db.OrganizationCategories.FirstOrDefault(c => c.OrganizationCategoryID.ToString() == catId)?.Description;
        //}
        //[DataObjectMethod(DataObjectMethodType.Select)]
        //public ShipmentReportHeaderViewModel GetReportHeaderData()
        //{
        //    var db = new ExceedDbContext();

        //    var headerVM = new ShipmentReportHeaderViewModel();
        //    string tradeName = "";

        //    string AmharicName = "";
        //    string Name = "";
        //    string Address = "";
        //    Image LogoFileName = null;
        //    var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

        //    if (organization != null)
        //    {
        //        Name = organization.TradeName;
        //        AmharicName = organization.AmharicName;
        //        var address =
        //            db.Addresses.Where(
        //                a =>
        //                    a.Reference == organization.MainCompanyId.ToString() &&
        //                    a.ReferenceType.ToLower() == "company").ToList();
        //        if (address.Any())
        //        {
        //            foreach (var a in address)
        //            {

        //                if (Address == null)
        //                {
        //                    Address = a.Category + ":- " + a.Value;
        //                }
        //                else
        //                {
        //                    Address += ", " + a.Category + "-" + a.Value;
        //                }

        //            }

        //        }
        //        var logo =
        //            db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
        //                    a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
        //        if (logo != null)
        //        {
        //            LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
        //        }
        //    }

        //    headerVM.AmharicName = AmharicName;
        //    headerVM.Name = Name;

        //    headerVM.LogoFileName = LogoFileName;
        //    headerVM.Address = Address;
        //    return headerVM;
        //}
    }
}
