using ExceedERP.Core.Domain.CMS;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.CMS;
using ExceedERP.Reporting.CMS.Printing.ViewModels;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Reporting.SharedDataSources.ViewModel;
using System.ComponentModel;
using System.Linq;
using System.Web.Hosting;
//using System.Web;

namespace ExceedERP.Reporting.CMS.Printing.DataSource
{
    [DataObject]
    class WelderCertificatePrintDataSource
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public Organization GetOrganization()
        {

            Organization record = new Organization();
            var Organization = db.Companies.Where(x => x.Category == "0").FirstOrDefault();
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
                    db.Attachments.Where(
                        a =>
                            a.Reference == Organization.MainCompanyId.ToString() &&
                            a.ReferenceType == AttachmentType.LOGO).FirstOrDefault();
                if (logo != null)
                {
                    record.LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            return record;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public WeldersQualificationTestCertificateViewModel GetData(string traineeId)
        {

            WeldersQualificationTestCertificateViewModel record = new WeldersQualificationTestCertificateViewModel();


            if (!string.IsNullOrEmpty(traineeId))
            {

                var trainee = db.CmsTrainees.FirstOrDefault(so => so.CmsTraineeId.ToString() == traineeId);

                if (trainee != null)
                {
                    var identification = db.CmsTraineeIdentifications.FirstOrDefault(i => i.CmsTraineeId == trainee.CmsTraineeId);

                    var qualification = db.WelderPerformanceQualification.FirstOrDefault(q => q.CmsTraineeId == trainee.CmsTraineeId);

                    string profilePicPath = HostingEnvironment.MapPath("~/Content/Images/CMS/Profiles/" + trainee.ProfilePicFileName);
                    //string file = System.Web.HttpContext.Current.Server.MapPath("~/Content/Images/CMS/Profiles/" + trainee.ProfilePicFileName);
                    //string mimeType = MimeMapping.GetMimeMapping(file);

                    byte[] profileImageStream = System.IO.File.ReadAllBytes(profilePicPath);

                    string barcodePath = HostingEnvironment.MapPath("~/Content/Images/CMS/Barcodes/" + $"barcode-{trainee.CmsTraineeId}.png");

                    byte[] barcodeReadStream = System.IO.File.ReadAllBytes(barcodePath);

                    record = new WeldersQualificationTestCertificateViewModel
                    {
                        Designation = trainee.CertificateDesignationCode,
                        FullName = trainee.FullName,
                        DateOfBirth = trainee.DateOfBirth,
                        Identification = identification?.IdNumber,
                        MethodOfIdentification = identification?.IdentificationType.ToString().ToLower().Replace('_', ' '),
                        PlaceOfBirth = trainee.Address,
                        RegistrationDate = trainee.RegistrationDate,
                        RegistrationNumber = trainee.RegistrationNumber,
                        ProfilePicture = OrganizationObjectDataSource.ByteArrayToImage(profileImageStream),
                        Barcode = OrganizationObjectDataSource.ByteArrayToImage(barcodeReadStream),

                        WelderPerformanceQualification = qualification,
                        //CertificateValidUntil = qualification.Date.AddYears(3).AddDays(-1) ,
                    };
                    if (qualification != null)
                    {
                        record.CertificateValidUntil = qualification.Date.AddYears(3).AddDays(-1);

                        if (qualification.WeldingType == WeldingType.BW.ToString())
                        {
                            record.PlateOrPipe = qualification.GrooveBaseMetalPlateOrPipe;
                            record.RangeOfPlateOrPipe = qualification.RangeOfGrooveBaseMetalPlateOrPipe;

                            record.ThicknessOfWeldingMetal = qualification.ThicknessOfGrooveBaseMetalPlateOrPipe;
                            record.RangeOfThicknessOfWeldingMetal = qualification.RangeThicknessOfGrooveBaseMetalPlateOrPipe;

                            record.OutsideDiameter = qualification.GroovePipeOrTubularOutsideDiameter;
                            record.RangeOfOutsideDiameter = qualification.RangeOfGroovePipeOrTubularOutsideDiameter;

                        }
                        else if (qualification.WeldingType == WeldingType.FW.ToString())
                        {
                            record.PlateOrPipe = qualification.FilletBaseMetalPlateOrPipe;
                            record.RangeOfPlateOrPipe = qualification.RangeOfFilletBaseMetalPlateOrPipe;

                            record.ThicknessOfWeldingMetal = qualification.ThicknessOfFilletBaseMetalPlateOrPipe;
                            record.RangeOfThicknessOfWeldingMetal = qualification.RangeOfThicknessOfFilletBaseMetalPlateOrPipe;

                            record.OutsideDiameter = qualification.FilletPipeOrTubularOutsideDiameter;
                            record.RangeOfOutsideDiameter = qualification.RangeOfFilletPipeOrTubularOutsideDiameter;

                        }
                    }
                }
                return record;
            }

            return null;
        }
    }
}
