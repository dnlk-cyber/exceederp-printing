using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CMS;
using ExceedERP.DataAccess.Context;
using System.IO;
using System.Drawing;
//using IronBarCode;

namespace ExceedERP.Web.Areas.CMS.Controllers
{
    public class TraineesController : Controller
    {
        //private string imageInitial = "~/Content/Images/CMS/Profiles/";
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Programs"] = db.CMSPrograms.Select(p => new { p.CMSProgramId, p.Name }).ToList();
            ViewData["Courses"] = db.CMSCourses.Select(p => new { p.CMSCourseId, p.Name }).ToList();
            ViewData["Cities"] = db.Cities.Select(c => new { c.Name, c.CityId }).ToList();

            if (db.CmsTrainees.Any(t => string.IsNullOrEmpty(t.Status)))
            {
                UpdateEmptyStatus();
            }

            return View();
        }
        private void UpdateEmptyStatus()
        {
           foreach (var trainee in db.CmsTrainees.Where(t => string.IsNullOrEmpty(t.Status)).ToList())
            {
                if(trainee.Type == TraineeType.Individual)
                {
                    trainee.Status = TraineeStatusConstants.NewTrainee;
                }
                else
                {
                    trainee.Status = CompanyStatusConstants.NewCompany;
                }

                db.CmsTrainees.Attach(trainee);
                db.Entry(trainee).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        public ActionResult ShowProfilePhoto(int id)
        {
            var currentTrainee = db.CmsTrainees.Find(id);
            if (currentTrainee != null)
            {
                String file = Server.MapPath("~/Content/Images/CMS/Profiles/" + currentTrainee.ProfilePicFileName);
                String mimeType = MimeMapping.GetMimeMapping(file);

                byte[] stream = System.IO.File.ReadAllBytes(file);
                return File(stream, mimeType);

                //return "~/Content/Images/CMS/Profiles/" + currentTrainee.ProfilePicFileName;
            }
            //var imageData = _db.Pacientes.Find(id).Foto;

            //if (imageData != null)
            //{
            //    return File(imageData, "image/jpg");
            //}
            return null;
        }
        public ActionResult Trainees_Read([DataSourceRequest]DataSourceRequest request, TraineeType traineeType)
        {
            IQueryable<CmsTrainee> trainees = db.CmsTrainees.Where(t => t.Type == traineeType);
            DataSourceResult result = trainees.ToDataSourceResult(request, trainee => new CmsTrainee
            {
                CmsTraineeId = trainee.CmsTraineeId,
                Type = trainee.Type,
                RegistrationDate = trainee.RegistrationDate,
                RegistrationNumber = trainee.RegistrationNumber,
                CMSProgramId = trainee.CMSProgramId,
                CMSCourseId = trainee.CMSCourseId,
                FirstName = trainee.FirstName,
                LastName = trainee.LastName,
                MiddleName = trainee.MiddleName,
                FullName = trainee.FullName,
                Nationality = trainee.Nationality,
                CompanyName = trainee.CompanyName,
                Region = trainee.Region,
                Address = trainee.Address,
                CityId = trainee.CityId,
                DateOfBirth = trainee.DateOfBirth,
                PhoneNumber = trainee.PhoneNumber,
                MobilePhoneNumber = trainee.MobilePhoneNumber,
                Email = trainee.Email,
                PreferedDate = trainee.PreferedDate,
                TrainingVenue = trainee.TrainingVenue,
                ProfilePicFileName = string.IsNullOrEmpty(trainee.ProfilePicFileName) ?
                "user-avatar.png" : trainee.ProfilePicFileName,
                BarcodeImageName = $"barcode-{trainee.CmsTraineeId}.png",
                Status = trainee.Status,

                DateCreated = trainee.DateCreated,
                LastModified = trainee.LastModified,
                CreatedBy = trainee.CreatedBy,
                ModifiedBy = trainee.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Trainees_Create([DataSourceRequest]DataSourceRequest request, CmsTrainee trainee, TraineeType traineeType)
        {
            if (ModelState.IsValid)
            {
                var entity = new CmsTrainee
                {
                    Type = traineeType,
                    RegistrationDate = trainee.RegistrationDate,
                    RegistrationNumber = trainee.RegistrationNumber,
                    CMSProgramId = trainee.CMSProgramId,
                    CMSCourseId = trainee.CMSCourseId,
                    FirstName = trainee.FirstName,
                    LastName = trainee.LastName,
                    MiddleName = trainee.MiddleName,
                    FullName = trainee.FullName,
                    Nationality = trainee.Nationality,
                    CompanyName = trainee.CompanyName,
                    Region = trainee.Region,
                    Address = trainee.Address,
                    CityId = trainee.CityId,
                    DateOfBirth = trainee.DateOfBirth,
                    PhoneNumber = trainee.PhoneNumber,
                    MobilePhoneNumber = trainee.MobilePhoneNumber,
                    Email = trainee.Email,
                    PreferedDate = trainee.PreferedDate,
                    TrainingVenue = trainee.TrainingVenue,
                    ProfilePicFileName = trainee.ProfilePicFileName,
                    Status = trainee.Status,

                    DateCreated = DateTime.Now,
                    LastModified = trainee.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = trainee.ModifiedBy
                };
                entity.FullName = $"{entity.FirstName} {entity.MiddleName} {entity.LastName}";

                db.CmsTrainees.Add(entity);
                db.SaveChanges();

                var result = GenerateTraineeRegistrationNumber(entity);
                if (result.IsSuccessful)
                {
                    entity.RegistrationNumber = result.TraineeIdNumberComponent.CombinedIdNumber;

                    db.CmsTrainees.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    trainee.RegistrationNumber = entity.RegistrationNumber;

                    CreateBarcodeOnTraineeRegistration(entity, result.TraineeIdNumberComponent.SystemId);
                }

                trainee.CmsTraineeId = entity.CmsTraineeId;
            }

            return Json(new[] { trainee }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Trainees_Update([DataSourceRequest]DataSourceRequest request, CmsTrainee trainee)
        {
            if (ModelState.IsValid)
            {
                var entity = new CmsTrainee
                {
                    CmsTraineeId = trainee.CmsTraineeId,
                    Type = trainee.Type,
                    RegistrationDate = trainee.RegistrationDate,
                    RegistrationNumber = trainee.RegistrationNumber,
                    CMSProgramId = trainee.CMSProgramId,
                    CMSCourseId = trainee.CMSCourseId,
                    FirstName = trainee.FirstName,
                    LastName = trainee.LastName,
                    MiddleName = trainee.MiddleName,
                    FullName = trainee.FullName,
                    Nationality = trainee.Nationality,
                    CompanyName = trainee.CompanyName,
                    Region = trainee.Region,
                    Address = trainee.Address,
                    CityId = trainee.CityId,
                    DateOfBirth = trainee.DateOfBirth,
                    PhoneNumber = trainee.PhoneNumber,
                    MobilePhoneNumber = trainee.MobilePhoneNumber,
                    Email = trainee.Email,
                    PreferedDate = trainee.PreferedDate,
                    TrainingVenue = trainee.TrainingVenue,
                    ProfilePicFileName = trainee.ProfilePicFileName,
                    Status = trainee.Status,

                    DateCreated = trainee.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = trainee.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.FullName = $"{entity.FirstName} {entity.MiddleName} {entity.LastName}";

                db.CmsTrainees.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { trainee }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Trainees_Destroy([DataSourceRequest]DataSourceRequest request, CmsTrainee trainee)
        {
            if (ModelState.IsValid)
            {
                var entity = new CmsTrainee
                {
                    CmsTraineeId = trainee.CmsTraineeId,
                    Type = trainee.Type,
                    RegistrationDate = trainee.RegistrationDate,
                    RegistrationNumber = trainee.RegistrationNumber,
                    CMSProgramId = trainee.CMSProgramId,
                    CMSCourseId = trainee.CMSCourseId,
                    FirstName = trainee.FirstName,
                    LastName = trainee.LastName,
                    MiddleName = trainee.MiddleName,
                    FullName = trainee.FullName,
                    Nationality = trainee.Nationality,
                    CompanyName = trainee.CompanyName,
                    Region = trainee.Region,
                    Address = trainee.Address,
                    CityId = trainee.CityId,
                    DateOfBirth = trainee.DateOfBirth,
                    PhoneNumber = trainee.PhoneNumber,
                    MobilePhoneNumber = trainee.MobilePhoneNumber,
                    Email = trainee.Email,
                    PreferedDate = trainee.PreferedDate,
                    TrainingVenue = trainee.TrainingVenue,
                    ProfilePicFileName = trainee.ProfilePicFileName,
                    Status = trainee.Status,

                    DateCreated = trainee.DateCreated,
                    LastModified = trainee.LastModified,
                    CreatedBy = trainee.CreatedBy,
                    ModifiedBy = trainee.ModifiedBy
                };

                db.CmsTrainees.Attach(entity);
                db.CmsTrainees.Remove(entity);
                db.SaveChanges();

                DeleteProfileAndBarcodeImagesFromFileSystemOnTraineeDelete(entity);
            }

            return Json(new[] { trainee }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        #region local private methods
        private void CreateBarcodeOnTraineeRegistration(CmsTrainee trainee, string systemId)
        {

            string path = HttpContext.Server.MapPath("~/Content/Images/CMS/Barcodes/");
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            string fullFilePath = path + "barcode-" + trainee.CmsTraineeId + ".png";


            BarcodeLib.Barcode linearBarcode = new BarcodeLib.Barcode();

            linearBarcode.EncodedType = BarcodeLib.TYPE.EAN13;
            linearBarcode.BarWidth = 1;
            linearBarcode.Height = 250;
            linearBarcode.IncludeLabel = true;
            linearBarcode.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
            linearBarcode.ImageFormat = System.Drawing.Imaging.ImageFormat.Png;

            linearBarcode.Encode(BarcodeLib.TYPE.CODE39, systemId) /*= trainee.RegistrationNumber*/;

            linearBarcode.SaveImage(fullFilePath, BarcodeLib.SaveTypes.PNG);

            //BarcodeLib.Barcode.DoEncode(BarcodeLib.TYPE.CODE39, trainee.RegistrationNumber).Save(fullFilePath);

        }
        private void DeleteProfileAndBarcodeImagesFromFileSystemOnTraineeDelete(CmsTrainee trainee)
        {
            string path = HttpContext.Server.MapPath("~/Content/Images/CMS/Barcodes/");
            string fullFilePath = path + "barcode-" + trainee.CmsTraineeId + ".png";
            if (System.IO.File.Exists(fullFilePath))
            {
                System.IO.File.Delete(fullFilePath);
            }
            string profilesPicPath = HttpContext.Server.MapPath("~/Content/Images/CMS/Profiles/" + trainee.ProfilePicFileName);
            if (System.IO.File.Exists(profilesPicPath))
            {
                System.IO.File.Delete(profilesPicPath);
            }

        }
        #endregion

        #region public methods
        #region Generate Id 
        private TraineeIdGenerationStatusViewModel GenerateTraineeRegistrationNumber(CmsTrainee trainee)
        {
            TraineeIdGenerationStatusViewModel model = new TraineeIdGenerationStatusViewModel();

            var defaultPrefix = db.RegistrationNumberPrefixes.FirstOrDefault(p => p.IsDefault);
            var defaultISO = db.ISOTypes.FirstOrDefault(i => i.IsDefault);

            if (defaultPrefix != null && defaultISO != null)
            {
                string decimalPlaces = "D7";
                // 9,999,999
                if (trainee.CmsTraineeId > 9999999)
                {
                    decimalPlaces = "D10";
                }
                //9,999,999,999
                else if (trainee.CmsTraineeId > 1000000000)
                {
                    decimalPlaces = "D13";
                }
                var sequenceNumber = trainee.CmsTraineeId.ToString(decimalPlaces);
                var systemId = Guid.NewGuid().ToString("N").ToUpper();

                // define id 
                // WTTC-565857-ISO9606-1-78744444AY455555LLL7478.000001
                var completeId = $"{defaultPrefix.PrefixValue}-{defaultISO.Name}-{systemId}.{sequenceNumber}";

                var entity = new TraineeIdNumberComponent
                {
                    CombinedIdNumber = completeId,
                    PrefixValue = defaultPrefix.PrefixValue,
                    RegistrationNumberPrefixId = defaultPrefix.RegistrationNumberPrefixId,
                    ISOType = defaultISO.Name,
                    ISOTypeId = defaultISO.ISOTypeId,

                    SequenceNumber = sequenceNumber,
                    SystemId = systemId,
                    CmsTraineeId = trainee.CmsTraineeId,


                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    LastModified = null,
                    ModifiedBy = null
                };
                db.TraineeIdNumberComponents.Add(entity);
                db.SaveChanges();

                model.IsSuccessful = true;
                model.TraineeIdNumberComponent = entity;

            }
            else
            {
                if (defaultISO == null)
                {
                    model.ErrorMessage = "There is no Default ISO defined.";
                    model.IsSuccessful = false;
                }
                if (defaultPrefix == null)
                {
                    model.ErrorMessage = "There is no Default Prefix defined.";
                    model.IsSuccessful = false;
                }
            }
            return model;
        }
        #endregion
        public void ProfilePicture_Upload(int id, IEnumerable<HttpPostedFileBase> files)
        {
            if (ModelState.IsValid)
            {
                if (files != null && files.Count() > 0)
                {
                    foreach (var file in files)
                    {
                        string path = HttpContext.Server.MapPath("~/Content/Images/CMS/Profiles/");
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        var newFileName = "profile-" + id + Path.GetExtension(file.FileName);
                        var filePath = path + newFileName;
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                        file.SaveAs(filePath);

                        var currentTrainee = db.CmsTrainees.Find(id);
                        if (currentTrainee != null)
                        {
                            currentTrainee.ProfilePicFileName = newFileName;

                            db.CmsTrainees.Attach(currentTrainee);
                            db.Entry(currentTrainee).State = EntityState.Modified;
                            db.SaveChanges();
                        }


                    }
                }


            }

        }

        public JsonResult RegenerateBarcode(int traineeId)
        {
            var trainee = db.CmsTrainees.Find(traineeId);
            if (trainee == null)
            {
                return Json(new { Message = "Trainee not found", Status = "Error" });
            }
            var idComponent = db.TraineeIdNumberComponents.FirstOrDefault(c => c.CmsTraineeId == trainee.CmsTraineeId);
            if (idComponent == null)
            {
                return Json(new { Message = "System Id component was not found", Status = "Error" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                string path = HttpContext.Server.MapPath("~/Content/Images/CMS/Barcodes/");
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string fullFilePath = path + "barcode-" + trainee.CmsTraineeId + ".png";

                if (System.IO.File.Exists(fullFilePath))
                {
                    System.IO.File.Delete(fullFilePath);
                }

                BarcodeLib.Barcode linearBarcode = new BarcodeLib.Barcode();

                linearBarcode.EncodedType = BarcodeLib.TYPE.EAN13;
                linearBarcode.BarWidth = 1;
                linearBarcode.Height = 250;
                linearBarcode.IncludeLabel = true;
                linearBarcode.LabelPosition = BarcodeLib.LabelPositions.BOTTOMCENTER;
                linearBarcode.ImageFormat = System.Drawing.Imaging.ImageFormat.Png;

                linearBarcode.Encode(BarcodeLib.TYPE.CODE39, idComponent.SystemId);

                linearBarcode.SaveImage(fullFilePath, BarcodeLib.SaveTypes.PNG);

                return Json(new { Message = $"Barcode regenerated for Trainee with registration number: {trainee.RegistrationNumber}", Status = "Ok" });
            }
        }
        #endregion
    }
}
