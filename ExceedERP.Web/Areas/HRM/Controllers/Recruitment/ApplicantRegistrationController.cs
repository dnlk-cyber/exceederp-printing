using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Models;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using System.Data;
using System.IO;
using System.Data.OleDb;
using System.Web.Hosting;
using System.Net.Http.Headers;
using System.Configuration;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class ApplicantRegistrationController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string FullName { get; set; }
        public static decimal EduLevel { get; set; }
        public static decimal EduField { get; set; }
        public static decimal RelevantExp { get; set; }
        public static decimal RelatedExp { get; set; }
        public static decimal Gender { get; set; }
        public static decimal Disabliy { get; set; }
        public static decimal Profile { get; set; }
        public static decimal Training { get; set; }
        public static decimal Dicipline { get; set; }
        public static DateTime RegistnDate { get; set; }
        public static DateTime EndDate { get; set; }
        public static string RegistnDateAm { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {

            var context = new ApplicationDbContext();
            var allUsers = context.Users.ToList();
            var name = allUsers.Where(X => X.UserName == User.Identity.Name).FirstOrDefault();
            if (name != null)
            {
                FullName = name.FirstName + " " + name.MiddleName + " " + name.LastName;
            }
            GetEducationFiled();
            GetEducationLevel();
            var media = db.Lookups.Where(x => x.LookupType == LookUpTypes.MEDIA);
            ViewData["Media"] = media;     
            ViewData["Vacancy"] = db.VaccancyPlans.ToList();
            ViewData["recruitmentVacancy"] = db.RecruitmentVacancy.ToList();
            var jobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            ViewData["JobTitle"] = jobtitles;
            ViewData["branch"] = db.Branch.ToList();
            ViewData["EducationLevel"] = db.Lookups.Where(x=>x.LookupType == LookUpTypes.EDUCATION_LEVEL).ToList();
            ViewData["EducationField"] = db.Lookups.Where(x => x.LookupType == LookUpTypes.EDUCATION_FIELD).ToList();
            ViewData["ExperienceOrgType"] = db.ExperienceOrganizationTypeSettings.ToList();
            ViewData["ExperienceJobTitle"] = db.ExperienceJobTitles.ToList();
            ViewData["Lookups"] = db.Lookups.Where(x=>x.LookupType == LookUpTypes.JOB_TITLE).ToList();
        }
        public JsonResult GetJobTitle(string VacancyId)
        {
            db = new ExceedDbContext();
            var recruitVacancy = db.RecruitmentVacancy.Where(x => x.VacancyId.ToString() == VacancyId).ToList();
            List<Vacancy> titleList = new List<Vacancy>();
            if (recruitVacancy.Any())
            {
                titleList = new List<Vacancy>();
                foreach (var p in recruitVacancy)
                {
                    var detail = db.VaccancyPlans.Where(x => x.VacancyId == p.VacancyId).FirstOrDefault();
                    if (detail != null)
                    {
                        titleList.Add(detail);
                    }


                }
            }
            return Json(titleList.Select(p => new
            {
                LookupId = p.VacancyId,
                Description = p.JobTitle
            }), JsonRequestBehavior.AllowGet);

        }
        private bool CheckAppRegnValidation(int id)
        {
            var ErrorMessage = "";


            #region ApplicantScreeningResult
            //var ApplicantScreenResult = db.ApplicantScreenResult.Where(x => x.ApplicantScreeningId == id && x.ReferenceType == "External").FirstOrDefault();
            //if (ApplicantScreenResult != null)
            //{

            //    ErrorMessage = "You cannot delete this Applicant .Please delete Applicant Screening result information first.";
            //    return false;


            //}
            #endregion
            #region ApplicantExamResult
            //var ApplicantExamResult = db.ApplicantExamResult.Where(x => x.ApplicantScreeningId == id && x.ReferenceType == "External").FirstOrDefault();
            //if (ApplicantExamResult != null)
            //{

            //    ErrorMessage = "You cannot delete this Applicant .Please delete Applicant Exam result information first.";
            //    return false;


            //}
            #endregion
            #region ApplicantTraining
            var ApplicantTraining = db.ApplicantTraining.Where(x => x.ApplicantRegistrationId == id).FirstOrDefault();
            if (ApplicantTraining != null)
            {

                ErrorMessage = "You cannot delete this Applicant .Please delete Applicant Training information first.";
                return false;


            }
            #endregion
            #region ApplicantEducation
            var ApplicantEducation = db.ApplicantEducation.Where(x => x.ApplicantRegistrationId == id).FirstOrDefault();
            if (ApplicantEducation != null)
            {

                ErrorMessage = "You cannot delete this Applicant .Please delete Applicant Education information first.";
                return false;


            }
            #endregion
            return true;
        }
        public void SetDate(string name, string value)
        {
            if (name == "RegistrationDateAm")
            {
                RegistnDateAm = value;
                var splitDate = RegistnDateAm.Split('/');
                RegistnDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }


        }
        private void SetSettingValues()
        {
            var settings =
                db.Configurations.Where(x => x.Category == "Recruitment_External" && x.Reference == "Recruitment").ToList();
            if (settings.Any())
            {
                foreach (var s in settings)
                {
                    if (s.Attribute == "EDUCATION_LEVEL")
                    {
                        EduLevel = Convert.ToDecimal(s.CurrentValue);
                    }
                    else if (s.Attribute == "EDUCATIONAL_FIELD")
                    {
                        EduField = Convert.ToDecimal(s.CurrentValue);
                    }
                    else if (s.Attribute == "RELATED_EXPERIENCE")
                    {
                        RelatedExp = Convert.ToDecimal(s.CurrentValue);
                    }
                    else if (s.Attribute == "RELEVANT_EXPERIENCE")
                    {
                        RelevantExp = Convert.ToDecimal(s.CurrentValue);
                    }
                    else if (s.Attribute == "GENDER")
                    {
                        Gender = Convert.ToDecimal(s.CurrentValue);
                    }
                    else if (s.Attribute == "DISABILITY")
                    {
                        Disabliy = Convert.ToDecimal(s.CurrentValue);
                    }
                    else if (s.Attribute == "PROFILE")
                    {
                        Profile = Convert.ToDecimal(s.CurrentValue);
                    }
                    else if (s.Attribute == "TRAINING")
                    {
                        Training = Convert.ToDecimal(s.CurrentValue);
                    }
                    else if (s.Attribute == "DISCIPLINE ")
                    {
                        Dicipline = Convert.ToDecimal(s.CurrentValue);
                    }
                }

            }
        }
        public JsonResult GetEducationFiled()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_FIELD).AsQueryable();
            ViewData["EduFiledList"] = lookups;
            return Json(lookups.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetEducationLevel()
        {
            var lookups = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL).AsQueryable();
            ViewData["EduLevelList"] = lookups;
            return Json(lookups.Select(p => new { LookupId = p.LookupId, Description = p.Description }), JsonRequestBehavior.AllowGet);
        }
        public int GetBranch(int id)
        {
            var vacancy = db.VaccancyPlans.FirstOrDefault(x=>x.VacancyId == id);
            if (vacancy != null)
            {
                var branch = db.Branch.FirstOrDefault(x=>x.BranchId == vacancy.BranchId);
                if(branch != null)
                {
                    return branch.BranchId;
                }
            }
            return 0;
        }
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult ApplicantRegistration_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
               IQueryable<ApplicantRegistration> applicantregistration = db.ApplicantRegistration;


            DataSourceResult result = applicantregistration.ToDataSourceResult(request, applicantRegistration => new
            {
                ApplicantRegistrationId = applicantRegistration.ApplicantRegistrationId,
                VacancyId = applicantRegistration.VacancyId,
                TotalResult = applicantRegistration.TotalResult,
                OrgBranchName = applicantRegistration.OrgBranchName,
                DateCreated = applicantRegistration.DateCreated,
                LastModified = applicantRegistration.LastModified,
                CreatedBy = applicantRegistration.CreatedBy,
                ModifiedBy = applicantRegistration.ModifiedBy,
                VacancyType = applicantRegistration.VacancyType,
                MediaVacancyCode = applicantRegistration.MediaVacancyCode,
                VacancyCode = applicantRegistration.VacancyCode,
                JobtitleName = applicantRegistration.JobtitleName,
                IsTransfered = applicantRegistration.IsTransfered,
                BranchId = GetBranch(applicantRegistration.VacancyId)

            });

            return Json(result);
        }
        private VacancyType GetType(string code)
        {
            var vacancy = db.VaccancyPlans.Where(x => x.VaccancyCode == code).FirstOrDefault();
            if (vacancy != null)
            {
                return vacancy.VacancyType;
            }
            else
            {
                return VacancyType.PERMANENT;
            }
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantRegistration_Create([DataSourceRequest]DataSourceRequest request, ApplicantRegistration applicantRegistration)
        {
            ModelState.Clear();

            int vacancyId = 0;
            string jobTitle = "";
            string vacancyCode = "";
            var mediaVacancyCode = "";           

            vacancyCode = db.VaccancyPlans.Find(applicantRegistration.VacancyId)?.VaccancyCode;
            mediaVacancyCode = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == applicantRegistration.VacancyId)?.MediaVacancyCode;

            var vac = db.VaccancyPlans.Find(applicantRegistration.VacancyId);
            if (vac != null)
            {
                var look = db.Lookups.FirstOrDefault(x => x.LookupId == vac.JobTitle);
                if (look != null)
                {
                    jobTitle = look.Description;
                }
            }

            if (ModelState.IsValid)
            {
                var entity = new ApplicantRegistration
                {
                    VacancyId = applicantRegistration.VacancyId,

                    IsTransfered = false,
                    JobtitleName = jobTitle,
                    VacancyCode = vacancyCode,
                    MediaVacancyCode = mediaVacancyCode,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    LastModified = applicantRegistration.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = applicantRegistration.ModifiedBy,
                    VacancyType = GetType(applicantRegistration.VacancyId.ToString()),               
                   
                };

                db.ApplicantRegistration.Add(entity);
                db.SaveChanges();
                applicantRegistration.ApplicantRegistrationId = entity.ApplicantRegistrationId;


            }
            return Json(new[] { applicantRegistration }.ToDataSourceResult(request, ModelState));
        }


        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantRegistration_Update([DataSourceRequest]DataSourceRequest request, ApplicantRegistration applicantRegistration)
        {
            ModelState.Clear();

            int vacancyId = 0;
            string jobTitle = "";
            string vacancyCode = "";
            var mediaVacancyCode = "";
          

            vacancyCode = db.VaccancyPlans.Find(applicantRegistration.VacancyId)?.VaccancyCode;
            mediaVacancyCode = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == applicantRegistration.VacancyId)?.MediaVacancyCode;

            var vac = db.VaccancyPlans.Find(applicantRegistration.VacancyId);
            if (vac != null)
            {
                var look = db.Lookups.FirstOrDefault(x => x.LookupId == vac.JobTitle);
                if(look != null)
                {
                    jobTitle = look.Description;
                }
               
            }

            if (ModelState.IsValid)
            {
                var applicant = db.ApplicantRegistration.Find(applicantRegistration.ApplicantRegistrationId);
                if (applicant != null)
                {
                    applicant.VacancyId = applicantRegistration.VacancyId;
                    applicant.IsTransfered = applicantRegistration.IsTransfered;
                    applicant.JobtitleName = jobTitle;
                    applicant.MediaVacancyCode = mediaVacancyCode;
                    applicant.VacancyCode = vacancyCode;
                    applicant.OrgBranchName = applicantRegistration.OrgBranchName;
                    applicant.DateCreated = applicantRegistration.DateCreated;
                    applicant.LastModified = applicantRegistration.LastModified;
                    applicant.CreatedBy = applicantRegistration.CreatedBy;
                    applicant.ModifiedBy = User.Identity.Name;
                    applicant.VacancyType = applicantRegistration.VacancyType;

                    db.Entry(applicant).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                //var entity = new ApplicantRegistration
                //{
                //    VacancyId = vacancyId,
                //    IsTransfered = false,
                //    JobtitleName = applicantRegistration.JobtitleName,
                //    RegisterBy = FullName,
                //    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                //    DateCreated = DateTime.Now,
                //    LastModified = applicantRegistration.LastModified,
                //    CreatedBy = User.Identity.Name,
                //    ModifiedBy = applicantRegistration.ModifiedBy,
                //    VacancyType = GetType(applicantRegistration.VacancyId.ToString()),
                //    RegistrationDate = DateTime.Now
                //};

                //db.ApplicantRegistration.Attach(entity);
                //db.Entry(entity).State = EntityState.Modified;
                //db.SaveChanges();

            }

            return Json(new[] { applicantRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantRegistration_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantRegistration applicantRegistration)
        {
            ModelState.Clear();

            if (!DeleteApplicantUnderThisVacancy(applicantRegistration.ApplicantRegistrationId))
            {
                ModelState.AddModelError("", "Can't delete this vacancy");
            }
            if (ModelState.IsValid)
            {
                var entity = new ApplicantRegistration
                {
                    ApplicantRegistrationId = applicantRegistration.ApplicantRegistrationId,
                    VacancyId = applicantRegistration.VacancyId,
                    IsTransfered = false,
                    OrgBranchName = applicantRegistration.OrgBranchName,
                    DateCreated = applicantRegistration.DateCreated,
                    LastModified = applicantRegistration.LastModified,
                    CreatedBy = applicantRegistration.CreatedBy,
                    ModifiedBy = applicantRegistration.ModifiedBy,
                    VacancyType = applicantRegistration.VacancyType,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted vacancy id from applicant registration" + applicantRegistration.VacancyId);
                #endregion
                db.ApplicantRegistration.Attach(entity);
                db.ApplicantRegistration.Remove(entity);
                db.SaveChanges();
            }


            return Json(new[] { applicantRegistration }.ToDataSourceResult(request, ModelState));
        }
        public bool DeleteApplicantUnderThisVacancy(int id)
        {
            var applicants = db.ApplicantDetails.Where(x => x.ApplicantRegistrationId == id).ToList();
            foreach (var app in applicants)
            {
                db.ApplicantDetails.Attach(app);
                db.ApplicantDetails.Remove(app);
                db.SaveChanges();
            }

            var checkApplicants = db.ApplicantDetails.Where(x => x.ApplicantRegistrationId == id).Count();
            if (checkApplicants > 0)
            {
                return false;
            }
            return true;
        }
        public ActionResult ImportApplicantsFromExcel([DataSourceRequest]DataSourceRequest request, IEnumerable<HttpPostedFileBase> file, string id)
        {

            if (file != null)
            {
                string fileNamePrefix = DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day.ToString() + DateTime.Now.Hour + DateTime.Now.Minute;

                //return new JavaScriptResult() { Script = "alert('" + "Data Copied Successfully!!" + "');" };
                DataSet ds = new DataSet();
                #region import

                foreach (HttpPostedFileBase fileBase in file)
                {
                    var postedFileBase = Request.Files["file"];
                    if (postedFileBase != null)
                    {
                        string fileExtension = Path.GetExtension(postedFileBase.FileName);
                        if (fileExtension == ".xls" || fileExtension == ".xlsx" || fileExtension == ".xlsm")
                        {
                            string fileLocation = Server.MapPath("~/App_Data/Import/") + fileNamePrefix + "-" +
                                                  DateTime.Now.Day.ToString() + "-" + DateTime.Now.Month.ToString() + "-" + DateTime.Now.Year.ToString() + User.Identity.Name + postedFileBase.FileName;
                            //if (System.IO.File.Exists(fileLocation))
                            //{
                            //    System.IO.File.Delete(fileLocation);
                            //}
                            postedFileBase.SaveAs(fileLocation);
                            var excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";

                            if (fileExtension == ".xls")
                            {
                                excelConnectionString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=2\"";
                            }
                            //connection String for xlsx file format.
                            else if (fileExtension == ".xlsx")
                            {

                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }
                            else if (fileExtension == ".xlsm")
                            {

                                excelConnectionString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + fileLocation +
                                                        ";Extended Properties=\"Excel 12.0;HDR=Yes;IMEX=2\"";
                            }
                            //Create Connection to Excel work book and add oledb namespace
                            OleDbConnection excelConnection = new OleDbConnection(excelConnectionString);
                            excelConnection.Open();
                            DataTable dt = new DataTable();

                            dt = excelConnection.GetOleDbSchemaTable(OleDbSchemaGuid.Tables, null);
                            if (dt == null)
                            {
                                return null;
                            }

                            String[] excelSheets = new String[dt.Rows.Count];
                            int t = 0;
                            //excel data saves in temp file here.
                            foreach (DataRow row in dt.Rows)
                            {
                                excelSheets[t] = row["TABLE_NAME"].ToString();
                                t++;
                            }
                            OleDbConnection excelConnection1 = new OleDbConnection(excelConnectionString);
                            string query = string.Format("Select * from [{0}]", excelSheets[0]);
                            using (OleDbDataAdapter dataAdapter = new OleDbDataAdapter(query, excelConnection1))
                            {
                                dataAdapter.Fill(ds);
                            }

                        }
                    }
                }
                #endregion



                if (ModelState.IsValid)
                {
                    if (ds.Tables[0].Rows.Count > 0)
                    {


                        for (int j = 0; j <= ds.Tables[0].Rows.Count - 1; j++)
                        {
                            int vacancyId = 0;
                            string jobTitle = "";
                            string vacancyCode = ds.Tables[0].Rows[j].ItemArray[7].ToString().Trim().ToLower();
                            var vacancy = db.VaccancyPlans.FirstOrDefault(x => x.VaccancyCode.Trim().ToLower() == vacancyCode);
                            if (vacancy == null)
                            {
                                //   ModelState.AddModelError("", "you should enter correct vacancy code");
                                string message = "you should enter correct vacancy code at line "+ j;
                                return new JavaScriptResult() { Script = "alert('" + message + "');" };
                            }
                            else
                            {
                                vacancyId = vacancy.VacancyId;
                                
                                    var look = db.Lookups.FirstOrDefault(x => x.LookupId == vacancy.JobTitle);
                                if(look != null)
                                {
                                    jobTitle = look.Description;
                                }
                                  
                                
                                var applicantReg = db.ApplicantRegistration.FirstOrDefault(x => x.VacancyId == vacancyId);
                                if (applicantReg == null)
                                {
                                    ApplicantRegistration applicantRegistration = new ApplicantRegistration();

                                    applicantRegistration.VacancyId = vacancyId;
                                    applicantRegistration.IsTransfered = false;
                                    applicantRegistration.JobtitleName = jobTitle;
                                    applicantRegistration.DateCreated = DateTime.Now;
                                    applicantRegistration.LastModified = applicantRegistration.LastModified;
                                    applicantRegistration.CreatedBy = User.Identity.Name;
                                    applicantRegistration.ModifiedBy = User.Identity.Name;
                                    applicantRegistration.VacancyType = vacancy.VacancyType;

                                    db.ApplicantRegistration.Add(applicantRegistration);
                                    //  db.Entry(applicantRegistration).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                else
                                {
                                    ApplicantDetail applicantDetail = new ApplicantDetail();
                                    Gender gender = Core.Domain.Common.Gender.MALE;
                                    if (ds.Tables[0].Rows[j].ItemArray[2].ToString().Trim().ToLower() == "female" || ds.Tables[0].Rows[j].ItemArray[2].ToString().Trim().ToLower() == "f")
                                    {
                                        gender = Core.Domain.Common.Gender.FEMALE;
                                    }

                                    applicantDetail.ApplicantRegistrationId = applicantReg.ApplicantRegistrationId;
                                    applicantDetail.ApplicantName = ds.Tables[0].Rows[j].ItemArray[0].ToString().Trim();
                                    applicantDetail.Age = Convert.ToInt32(ds.Tables[0].Rows[j].ItemArray[1].ToString().Trim());
                                    applicantDetail.Sex = gender;
                                    applicantDetail.Address = ds.Tables[0].Rows[j].ItemArray[3].ToString().Trim();
                                    //applicantDetail.Education = ds.Tables[0].Rows[j].ItemArray[4].ToString().Trim();
                                    //applicantDetail.Experiance = Convert.ToDecimal(ds.Tables[0].Rows[j].ItemArray[5].ToString().Trim());
                                    applicantDetail.Training = ds.Tables[0].Rows[j].ItemArray[6].ToString().Trim();
                                    applicantDetail.VacancyCode = vacancy.VaccancyCode;

                                    db.ApplicantDetails.Add(applicantDetail);
                                    db.SaveChanges();
                                }

                            }

                        }
                    }
                }

            }

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported applicant registration");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported applicant registration");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        //public bool CheckApplicant(int id)
        //{
        //    IQueryable<ApplicantRegistration> applicantregistration = db.ApplicantRegistration.Where(x => x.ApplicantRegistrationId == id);
        //    decimal screenSum = 0;
        //    var SettingSum = EduField + EduLevel + RelatedExp + RelevantExp + Disabliy + Gender + Profile + Training + Dicipline;

        //    foreach (var a in applicantregistration)
        //    {
        //        db = new ExceedDbContext();
        //        screenSum = 0;
        //        var screen =
        //            db.ApplicantScreenResult.Where(
        //                x => x.ApplicantScreeningId == a.ApplicantRegistrationId && x.ReferenceType == "External")
        //                .ToList();
        //        if (screen != null)
        //        {

        //            screenSum = Convert.ToDecimal(screen.Sum(x => x.CurrentResult));
        //            a.TotalResult = screenSum;
        //            var exam =
        //         db.ApplicantExamResult.Where(
        //             x => x.ApplicantScreeningId == a.ApplicantRegistrationId && x.ReferenceType == "External")
        //             .ToList();
        //            if (exam != null)
        //            {
        //                decimal exmSum = Convert.ToDecimal(exam.Sum(x => x.CurrentResult));
        //            }
        //            if (screenSum >= SettingSum)
        //            {
        //                return true;


        //            }
        //        }

        //    }
        //    return false;


        //}
        [HttpPost, ValidateInput(false)]
        public ActionResult ApplyForVacancy(HttpPostedFileBase file, string idh)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                int applicantDetailId = int.Parse(idh);

                var applicantDetail = db.ApplicantDetails.FirstOrDefault(x => x.ApplicantDetailId == applicantDetailId);
                if (applicantDetail != null && file != null)
                {
                    #region File Validation
                    // Ensure the file is valid and not empty
                    if (file.ContentLength > 0)
                    {
                        // You can add additional validation if needed, e.g., checking the file size.
                        // Remove the extension restrictions to allow all file types
                    }
                    else
                    {
                        ModelState.AddModelError("", "Uploaded file is empty.");
                        return RedirectToAction("Index");  // Adjust as needed
                    }
                    #endregion

                    #region Upload File
                    try
                    {
                        // Use the file name without extension and add timestamp
                        string fileName = Path.GetFileNameWithoutExtension(file.FileName);
                        string fileExtension = Path.GetExtension(file.FileName);
                        fileName = DateTime.Now.ToString("yyyyMMdd") + "-" + fileName.Trim() + fileExtension;
                        string path = HttpContext.Server.MapPath("~/Content/HRM/Recruitment/ApplicationRegistraion/");
                        // Get Upload path from Web.Config file AppSettings
                        //string uploadPath = ConfigurationManager.AppSettings["UserFilePath"];
                        //if (string.IsNullOrEmpty(uploadPath))
                        //{
                        //    throw new ConfigurationErrorsException("UserFilePath is missing in the configuration.");
                        //}

                        // Combine paths correctly and create directory if needed
                        string filePath = Path.Combine(path, fileName);
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }

                        // Save the file to the server
                        file.SaveAs(filePath);

                        // Update applicant details
                        applicantDetail.FilePath = filePath;
                        applicantDetail.FileName = fileName;

                        db.Entry(applicantDetail).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        // Log the exception as needed
                        ModelState.AddModelError("", "Error saving the file: " + ex.Message);
                        return RedirectToAction("Index");  // Adjust to handle the error appropriately
                    }
                    #endregion
                }
                else
                {
                    ModelState.AddModelError("", "Invalid applicant details or file not provided.");
                    return RedirectToAction("Index");
                }
            }

            return RedirectToAction("Index");
        }

        //public ActionResult ApplyForVacancy(HttpPostedFileBase file, string idh)
        //{
        //    ModelState.Clear();

        //    if (ModelState.IsValid)
        //    {
        //       int applicantDetailId = int.Parse(idh);

        //        var applicantDetail = db.ApplicantDetails.FirstOrDefault(x=>x.ApplicantDetailId == applicantDetailId);
        //        if (applicantDetail != null) {
        //            #region Upload File

        //            //Use Namespace called :  System.IO  
        //            string FileName = Path.GetFileNameWithoutExtension(file.FileName);

        //            //To Get File Extension  
        //            string FileExtension = Path.GetExtension(file.FileName);

        //            //Add Current Date To Attached File Name  
        //            FileName = DateTime.Now.ToString("yyyyMMdd") + "-" + FileName.Trim() + FileExtension;

        //            //Get Upload path from Web.Config file AppSettings.  
        //            string UploadPath = ConfigurationManager.AppSettings["UserFilePath"].ToString();

        //            //Its Create complete path to store in server.  
        //            var FilePath = UploadPath + FileName;

        //            //To copy and save file into server.  
        //             file.SaveAs(FilePath);
        //            #endregion

        //            #region  Update

        //            applicantDetail.FilePath = FilePath;
        //            applicantDetail.FileName = FileName;

        //            db.Entry(applicantDetail).State = System.Data.Entity.EntityState.Modified;
        //            db.SaveChanges();

        //            #endregion

        //        }

        //    }

        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
