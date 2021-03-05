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
    class WeldersPerformanceQualificationPrintDataSource
    {
        private readonly ExceedDbContext db = new ExceedDbContext();



        [DataObjectMethod(DataObjectMethodType.Select)]
        public WeldersPerformanceQualificationPrintViewModel GetData(string traineeId)
        {

            WeldersPerformanceQualificationPrintViewModel record = new WeldersPerformanceQualificationPrintViewModel();


            if (!string.IsNullOrEmpty(traineeId))
            {

                var trainee = db.CmsTrainees.FirstOrDefault(so => so.CmsTraineeId.ToString() == traineeId);

                if (trainee != null)
                {
                    var identification = db.CmsTraineeIdentifications.FirstOrDefault(i => i.CmsTraineeId == trainee.CmsTraineeId);

                    var qualification = db.WelderPerformanceQualification.FirstOrDefault(q => q.CmsTraineeId == trainee.CmsTraineeId);

                    string profilePicPath = HostingEnvironment.MapPath("~/Content/Images/CMS/Profiles/" + trainee.ProfilePicFileName);
                    
                    byte[] profileImageStream = System.IO.File.ReadAllBytes(profilePicPath);

                    string barcodePath = HostingEnvironment.MapPath("~/Content/Images/CMS/Barcodes/" + $"barcode-{trainee.CmsTraineeId}.png");
                   
                    byte[] barcodeReadStream = System.IO.File.ReadAllBytes(barcodePath);

                    record = new WeldersPerformanceQualificationPrintViewModel
                    {
                        FullName = trainee.FullName,
                        //DateOfBirth = trainee.DateOfBirth,
                        //Identification = identification?.IdNumber,
                        //MethodOfIdentification = identification?.IdentificationType.ToString().ToLower().Replace('_', ' '),
                        //PlaceOfBirth = trainee.Address,
                        RegistrationDate = trainee.RegistrationDate,
                        RegistrationNumber = trainee.RegistrationNumber,
                        ProfilePicture = OrganizationObjectDataSource.ByteArrayToImage(profileImageStream),
                        Barcode = OrganizationObjectDataSource.ByteArrayToImage(barcodeReadStream),
                        
                        WelderPerformanceQualification = qualification,
                        //CertificateValidUntil = qualification.Date.AddYears(3).AddDays(-1),
                    };
                }
                return record;
            }

            return null;
        }


       


        
    }
}
