using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Data.Entity;
using System.Linq;
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
        public ActionResult ApplicantDetail_Create([DataSourceRequest]DataSourceRequest request, HttpPostedFileBase DocumentFile, ApplicantDetail applicant, int id, string vid)
        {
            if (applicant.Age < 18 || applicant.Age > 60)
            {
                ModelState.AddModelError("", "you have to enter valid Age");
            }
            if (ModelState.IsValid)
            {

                var entity = new ApplicantDetail
                {
                    ApplicantRegistrationId = id,
                    ApplicantName = applicant.ApplicantName,
                    Age = applicant.Age,
                    Sex = applicant.Sex,
                    Address = applicant.Address,
                    //Education = applicant.Education,
                    Training = applicant.Training,
                    //Experiance = applicant.Experiance,
                    VacancyCode = vid,
                    Language = applicant.Language,
                };

                db.ApplicantDetails.Add(entity);
                db.SaveChanges();
                applicant.ApplicantDetailId = entity.ApplicantDetailId;

            }

            return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantDetail_Update([DataSourceRequest]DataSourceRequest request, ApplicantDetail applicant)
        {
            if (ModelState.IsValid)
            {

                var entity = new ApplicantDetail
                {
                    ApplicantDetailId = applicant.ApplicantDetailId,
                    ApplicantRegistrationId = applicant.ApplicantRegistrationId,
                    ApplicantName = applicant.ApplicantName,
                    Age = applicant.Age,
                    Sex = applicant.Sex,
                    Address = applicant.Address,
                    //Education = applicant.Education,
                    Training = applicant.Training,
                    //Experiance = applicant.Experiance,
                    VacancyCode = applicant.VacancyCode,
                    Language = applicant.Language,
                };

                // db.Applicant.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }


            return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantDetail_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantDetail applicant)
        {
            if (ModelState.IsValid)
            {


                var entity = new ApplicantDetail
                {
                    ApplicantDetailId = applicant.ApplicantDetailId,
                    ApplicantRegistrationId = applicant.ApplicantRegistrationId,
                    ApplicantName = applicant.ApplicantName,
                    Age = applicant.Age,
                    Sex = applicant.Sex,
                    Address = applicant.Address,
                    //Education = applicant.Education,
                    Training = applicant.Training,
                    //Experiance = applicant.Experiance,
                    VacancyCode = applicant.VacancyCode,
                    Language = applicant.Language,
                    //  LanguageForDisplay = applicant.LanguageForDisplay,
                };

                db.ApplicantDetails.Attach(entity);
                db.ApplicantDetails.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { applicant }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
