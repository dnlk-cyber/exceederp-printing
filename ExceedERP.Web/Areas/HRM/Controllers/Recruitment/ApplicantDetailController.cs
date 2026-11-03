using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.AccessControl;
using System.Security.Principal;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class ApplicantDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/ApplicantDetail
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult ApplicantDetail_Read([DataSourceRequest]DataSourceRequest request, int id, string vid)
        {
            IQueryable<ApplicantDetail> applicantdtail = db.ApplicantDetails.Where(x => x.ApplicantRegistrationId == id);
            DataSourceResult result = applicantdtail.ToDataSourceResult(request, detail => new
            {
                ApplicantName = detail.ApplicantName,
                Age = detail.Age,
                Sex = detail.Sex,
                Address = detail.Address,
                Language = detail.Language,
                Training = detail.Training,
                ApplicantDetailId = detail.ApplicantDetailId,
                ApplicantRegistrationId = detail.ApplicantRegistrationId,
                VacancyCode = detail.VacancyCode,
                Result = GetResult(detail.ApplicantRegistrationId, detail.ApplicantDetailId),
                Description = detail.Description,
                FilePath = detail.FilePath,
                FileName = detail.FileName,
                DocumentFile = detail.DocumentFile
            });

            return Json(result);
        }
        public ActionResult DownloadFile(string documentId)
        {

            var id = int.Parse(documentId);
            var attachment =
                    db.ApplicantDetails.FirstOrDefault(x => x.ApplicantDetailId == id);

            if (attachment != null && attachment.FileName != null)
            {
                //string pathTest = Server.MapPath("~/UserFilePath/");

                string path = attachment.FilePath;//Server.MapPath("~/UserFilePath/") + attachment.FileName;

                byte[] bytes = System.IO.File.ReadAllBytes(path);

                return File(bytes, "application/octet-stream", attachment.FileName);
            }
            //ModelState.AddModelError("","Can't download empty file");
            //return Json( ModelState);
            return null;
        }

        public decimal GetResult(int regId, int aid)
        {
            decimal settingValue = 0;
            decimal result = 0;
            var examResults = db.ApplicantExamResult.Where(x => x.ApplicantScreeningId == regId && x.ApplicantResultId == aid).ToList();
            if (examResults.Any())
            {
                settingValue = examResults.Sum(x => x.SettingValue);
                result = examResults.Sum(x => x.CurrentResult);
            }
            return result;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantDetail_Create([DataSourceRequest] DataSourceRequest request,HttpPostedFileBase DocumentFile,ApplicantDetail applicant,int id,string vid)
        {
            // Validate age range
            if (applicant.Age < 18 || applicant.Age > 60)
            {
                ModelState.AddModelError("", "Please enter a valid age (18–60).");
                return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
            }

            // Find registration once
            var registration = db.ApplicantRegistration.Find(id);
            if (registration == null)
            {
                ModelState.AddModelError("", "Applicant registration for this vacancy was not found.");
                return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
            }
            applicant.ApplicantRegistrationId = id;
            applicant.VacancyCode = vid;
            // Proceed only if model is valid
            if (!ModelState.IsValid)
                return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));

            // Create new applicant record
            var entity = new ApplicantDetail
            {
                ApplicantRegistrationId = id,
                ApplicantName = applicant.ApplicantName,
                Age = applicant.Age,
                Sex = applicant.Sex,
                Address = applicant.Address,
                Description = applicant.Description,
                Training = applicant.Training,
                VacancyCode = vid,
                Language = applicant.Language,
            };

            db.ApplicantDetails.Add(entity);
            db.SaveChanges();

            applicant.ApplicantDetailId = entity.ApplicantDetailId;

            // Update registration count efficiently (no double COUNT)
            registration.NumberOfApplicant = db.ApplicantDetails
                .Count(x => x.ApplicantRegistrationId == id && !string.IsNullOrEmpty(x.ApplicantName));

            db.Entry(registration).State = EntityState.Modified;
            db.SaveChanges();

            return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantDetail_Update([DataSourceRequest] DataSourceRequest request, ApplicantDetail applicant)
        {
            if (!ModelState.IsValid)
                return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));

            var entity = db.ApplicantDetails.Find(applicant.ApplicantDetailId);
            if (entity == null)
            {
                ModelState.AddModelError("", "Applicant record not found.");
                return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
            }

            // Update fields
            entity.ApplicantName = applicant.ApplicantName;
            entity.Age = applicant.Age;
            entity.Sex = applicant.Sex;
            entity.Address = applicant.Address;
            entity.Description = applicant.Description;
            entity.Training = applicant.Training;
            entity.VacancyCode = applicant.VacancyCode;
            entity.Language = applicant.Language;

            db.SaveChanges();

            // ✅ Update NumberOfApplicant count
            var registration = db.ApplicantRegistration.Find(entity.ApplicantRegistrationId);
            if (registration != null)
            {
                registration.NumberOfApplicant = db.ApplicantDetails
                    .Count(x => x.ApplicantRegistrationId == entity.ApplicantRegistrationId && !string.IsNullOrEmpty(x.ApplicantName));
                db.Entry(registration).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantDetail_Destroy([DataSourceRequest] DataSourceRequest request, ApplicantDetail applicant)
        {
            if (!ModelState.IsValid)
                return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));

            var entity = db.ApplicantDetails.Find(applicant.ApplicantDetailId);
            if (entity == null)
            {
                ModelState.AddModelError("", "Applicant record not found.");
                return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
            }

            var registrationId = entity.ApplicantRegistrationId;

            db.ApplicantDetails.Remove(entity);
            db.SaveChanges();

            // ✅ Update NumberOfApplicant count after deletion
            var registration = db.ApplicantRegistration.Find(registrationId);
            if (registration != null)
            {
                registration.NumberOfApplicant = db.ApplicantDetails
                    .Count(x => x.ApplicantRegistrationId == registrationId && !string.IsNullOrEmpty(x.ApplicantName));
                db.Entry(registration).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
        }
        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ApplicantDetail_Create([DataSourceRequest]DataSourceRequest request, HttpPostedFileBase DocumentFile, ApplicantDetail applicant, int id, string vid)
        //{
        //    if (applicant.Age < 18 || applicant.Age > 60)
        //    {
        //        ModelState.AddModelError("", "you have to enter valid Age");
        //        return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));

        //    }
        //    applicant.ApplicantRegistrationId = id;
        //    var applicationRegistration = db.ApplicantRegistration.Find(id);
        //    if (applicationRegistration == null)
        //    {
        //        ModelState.AddModelError("", "Applicant registration for this vacancy was not found.");
        //        return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));

        //    }
        //    if (ModelState.IsValid)
        //    {

        //        var entity = new ApplicantDetail
        //        {
        //            ApplicantRegistrationId = id,
        //            ApplicantName = applicant.ApplicantName,
        //            Age = applicant.Age,
        //            Sex = applicant.Sex,
        //            Address = applicant.Address,
        //            Description = applicant.Description,
        //            //Education = applicant.Education,
        //            Training = applicant.Training,
        //            //Experiance = applicant.Experiance,
        //            VacancyCode = vid,
        //            Language = applicant.Language,
        //        };
        //        db.ApplicantDetails.Add(entity);

        //        db.SaveChanges();
        //        applicant.ApplicantDetailId = entity.ApplicantDetailId;

        //        applicationRegistration.NumberOfApplicant =
        //            (applicationRegistration.NumberOfApplicant ?? 0)
        //            + db.ApplicantDetails
        //                .Where(x => x.ApplicantRegistrationId == id && !string.IsNullOrEmpty(x.ApplicantName))
        //                .Count();
        //        db.Entry(applicationRegistration).State = EntityState.Modified;
        //        db.SaveChanges();

        //    }

        //    return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
        //}

        //public ActionResult ApplicantDetail_Update([DataSourceRequest]DataSourceRequest request, ApplicantDetail applicant)
        //{
        //    if (ModelState.IsValid)
        //    {

        //        var entity = new ApplicantDetail
        //        {
        //            ApplicantDetailId = applicant.ApplicantDetailId,
        //            ApplicantRegistrationId = applicant.ApplicantRegistrationId,
        //            ApplicantName = applicant.ApplicantName,
        //            Age = applicant.Age,
        //            Sex = applicant.Sex,
        //            Address = applicant.Address,
        //            Description = applicant.Description,

        //            //Education = applicant.Education,
        //            Training = applicant.Training,
        //            //Experiance = applicant.Experiance,
        //            VacancyCode = applicant.VacancyCode,
        //            Language = applicant.Language,
        //        };

        //        // db.Applicant.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();
        //    }


        //    return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult ApplicantDetail_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantDetail applicant)
        //{
        //    if (ModelState.IsValid)
        //    {


        //        var entity = new ApplicantDetail
        //        {
        //            ApplicantDetailId = applicant.ApplicantDetailId,
        //            ApplicantRegistrationId = applicant.ApplicantRegistrationId,
        //            ApplicantName = applicant.ApplicantName,
        //            Age = applicant.Age,
        //            Sex = applicant.Sex,
        //            Address = applicant.Address,
        //            Description = applicant.Description,
        //            //Education = applicant.Education,
        //            Training = applicant.Training,
        //            //Experiance = applicant.Experiance,
        //            VacancyCode = applicant.VacancyCode,
        //            Language = applicant.Language,
        //            //  LanguageForDisplay = applicant.LanguageForDisplay,
        //        };

        //        db.ApplicantDetails.Attach(entity);
        //        db.ApplicantDetails.Remove(entity);
        //        db.SaveChanges();
        //    }

        //    return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
