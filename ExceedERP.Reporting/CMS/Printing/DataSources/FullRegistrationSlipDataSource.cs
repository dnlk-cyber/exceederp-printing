using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.CMS;
using ExceedERP.Reporting.CMS.Printing.ViewModels;
using ExceedERP.Reporting.SharedDataSources;
using System.ComponentModel;
using System.Linq;
using System.Web.Hosting;

namespace ExceedERP.Reporting.CMS.Printing.DataSource
{
    [DataObject]
    class FullRegistrationSlipDataSource
    {
        private readonly ExceedDbContext db = new ExceedDbContext();



        [DataObjectMethod(DataObjectMethodType.Select)]
        public FullRegistrationSlipViewModel GetRegistrationData(string traineeId)
        {

            FullRegistrationSlipViewModel record = new FullRegistrationSlipViewModel();

            if (!string.IsNullOrEmpty(traineeId))
            {

                var trainee = db.CmsTrainees.FirstOrDefault(so => so.CmsTraineeId.ToString() == traineeId);

                if (trainee != null)
                {

                    string profilePicPath = HostingEnvironment.MapPath("~/Content/Images/CMS/Profiles/" + trainee.ProfilePicFileName);

                    byte[] profileImageStream = System.IO.File.ReadAllBytes(profilePicPath);

                    string barcodePath = HostingEnvironment.MapPath("~/Content/Images/CMS/Barcodes/" + $"barcode-{trainee.CmsTraineeId}.png");

                    byte[] barcodeReadStream = System.IO.File.ReadAllBytes(barcodePath);

                    var identificationObject = CMSGenericHelper.GetMethodOfIdentification(trainee.CmsTraineeId);

                    record = new FullRegistrationSlipViewModel
                    {
                        FullName = trainee.FullName,
                        ParticipantNumber = CMSGenericHelper.GetTraineeSystemGeneratedIdNumber(trainee.CmsTraineeId),
                        Requirement = trainee.Requirement,
                        IdNumber = identificationObject?.IdNumber,
                        IdExpiryDate = identificationObject?.DateOfExpiry,
                        CompanyName = trainee.CompanyName,
                        ProfilePicture = OrganizationObjectDataSource.ByteArrayToImage(profileImageStream),
                        Barcode = OrganizationObjectDataSource.ByteArrayToImage(barcodeReadStream),

                        Type = (int)trainee.Type,

                        City = CMSGenericHelper.GetCityById(trainee.CityId),
                        Address = trainee.Address,
                        DateOfBirth = trainee.DateOfBirth,
                        PlaceOfBirth = trainee.PlaceOfBirth,

                        OrderNumber = trainee.CmsTraineeId.ToString(),
                        ZipCode = trainee.ZipCode,
                        CourseShortCode = CMSGenericHelper.GetCourseCode(trainee.CMSCourseId),
                    };

                    var courses = db.CMSTraineeCourses.Where(c => c.CmsTraineeId == trainee.CmsTraineeId).ToList();
                    foreach (var course in courses)
                    {
                        var courseRecord = new RegistrationSlipCourseViewModel
                        {
                            Code = course.Course.Code,
                            Name = course.Course.Name,
                            StartOfTraining = course.StartOfTraining,
                            EndOfTraining = course.EndOfTraining,
                            PlaceOfTrainingOrExamination = course.PlaceOfTrainingOrExamination,
                            TargetAmount = course.TargetAmount
                        };
                        record.CourseList.Add(courseRecord);
                    }
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
