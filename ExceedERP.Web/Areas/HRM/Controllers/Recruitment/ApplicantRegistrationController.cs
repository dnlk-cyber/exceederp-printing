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
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.Core.Domain.HRM.EmployeeDetail;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]

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
        string ErrorMessage = "";
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

            //  List<Vacancy> list = new List<Vacancy>();
            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.EXTERNAL).ToList();

            ViewData["Vacancy"] = vacancies;

            var recruitment = db.RecruitmentVacancy.ToList();

            ViewData["recruitmentVacancy"] = recruitment;

            var jobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            ViewData["JobTitle"] = jobtitles;
        }
        public JsonResult GetJobTitle(string VacancyId)
        {
            db = new ExceedDbContext();
            var plans = db.RecruitmentVacancy.Where(x => x.VacancyId.ToString() == VacancyId).ToList();
            List<Lookup> titleList = new List<Lookup>();
            if (plans.Any())
            {
                titleList = new List<Lookup>();
                foreach (var p in plans)
                {
                    var detail = db.VaccancyPlans.Where(x => x.VacancyId == p.VacancyId).FirstOrDefault();
                    if (detail != null)
                    {
                        var lookup = db.Lookups.Where(x => x.LookupId == detail.LookupId).FirstOrDefault();
                        if (lookup != null)
                        {
                            titleList.Add(lookup);
                        }
                    }


                }
            }
            return Json(titleList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        private bool CheckAppRegnValidation(int id)
        {
            ErrorMessage = "";


            #region ApplicantScreeningResult
            var ApplicantScreeningResult = db.ApplicantScreeningResult.Where(x => x.ApplicantRegistrationId == id && x.ReferenceType == "External").FirstOrDefault();
            if (ApplicantScreeningResult != null)
            {

                ErrorMessage = "You cannot delete this Applicant .Please delete Applicant Screening result information first.";
                return false;


            }
            #endregion
            #region ApplicantExamResult
            var ApplicantExamResult = db.ApplicantExamResult.Where(x => x.ApplicantRegistrationId == id && x.ReferenceType == "External").FirstOrDefault();
            if (ApplicantExamResult != null)
            {

                ErrorMessage = "You cannot delete this Applicant .Please delete Applicant Exam result information first.";
                return false;


            }
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

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult ApplicantRegistration_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            SetSettingValues();
            var VacancyCode = "";
            var mediaCode = "";
            decimal screenSum = 0;
            List<string> mybraches = UserInformationHelper.GetUserBranchs(User.Identity.Name);
            IQueryable<ApplicantRegistration> applicantregistration = db.ApplicantRegistration.Where(x => mybraches.Contains(x.OrgBranchName)).OrderByDescending(x => x.RegistrationDate);
            List<ApplicantRegistration> applicantRegi = new List<ApplicantRegistration>();
            var SettingSum = EduField + EduLevel + RelatedExp + RelevantExp + Disabliy + Gender + Profile + Training + Dicipline;


            if (id == "Screen")
            {
                foreach (var a in applicantregistration)
                {
                    db = new ExceedDbContext();
                    screenSum = 0;
                    var screen =
                        db.ApplicantScreeningResult.Where(
                            x => x.ApplicantRegistrationId == a.ApplicantRegistrationId && x.ReferenceType == "External")
                            .ToList();
                    if (screen != null)
                    {

                        screenSum = Convert.ToDecimal(screen.Sum(x => x.CurrentResult));
                        a.TotalResult = screenSum;
                        var exam =
                     db.ApplicantExamResult.Where(
                         x => x.ApplicantRegistrationId == a.ApplicantRegistrationId && x.ReferenceType == "External")
                         .ToList();
                        if (exam != null)
                        {
                            decimal exmSum = Convert.ToDecimal(exam.Sum(x => x.CurrentResult));
                        }
                        if (screenSum >= SettingSum)
                        {
                            applicantRegi.Add(a);


                        }
                    }

                }
                applicantregistration = applicantRegi.AsQueryable();
            }
            else
            {
                foreach (var a in applicantregistration)
                {
                    db = new ExceedDbContext();
                    screenSum = 0;
                    var screen =
                        db.ApplicantScreeningResult.Where(
                            x => x.ApplicantRegistrationId == a.ApplicantRegistrationId && x.ReferenceType == "External")
                            .ToList();
                    if (screen.Any())
                    {

                        screenSum = Convert.ToDecimal(screen.Sum(x => x.CurrentResult));
                        a.TotalResult = screenSum;
                        var exam =
                  db.ApplicantExamResult.Where(
                      x => x.ApplicantRegistrationId == a.ApplicantRegistrationId && x.ReferenceType == "External")
                      .ToList();
                        if (exam.Any())
                        {
                            decimal exmSum = Convert.ToDecimal(exam.Sum(x => x.CurrentResult)); ;
                            a.TotalResult = a.TotalResult + exmSum;
                        }
                        applicantRegi.Add(a);
                    }

                    var dd = db.VaccancyPlans.Find(a.VacancyId);
                    VacancyCode = dd.VaccancyCode;
                    var vacancyPlanforMedia = db.VaccancyPlans.FirstOrDefault(x => x.VacancyId == a.VacancyId);
                    var recruitmentMediaCode = db.RecruitmentVacancy.FirstOrDefault(x => x.VacancyId == vacancyPlanforMedia.VacancyId);
                    if (recruitmentMediaCode != null)
                    {
                        mediaCode = recruitmentMediaCode.MediaVacancyCode;
                    }


                }
            }


            DataSourceResult result = applicantregistration.ToDataSourceResult(request, applicantRegistration => new
            {
                ApplicantRegistrationId = applicantRegistration.ApplicantRegistrationId,
                VacancyId = applicantRegistration.VacancyId,
                ApplicantName = applicantRegistration.ApplicantName,
                //MediaVacancyCode = applicantRegistration.MediaVacancyCode,
                FirstName = applicantRegistration.FirstName,
                MiddleName = applicantRegistration.MiddleName,
                LastName = applicantRegistration.LastName,
                Age = applicantRegistration.Age,
                LookupId = applicantRegistration.LookupId,
                Sex = applicantRegistration.Sex,
                RegistrationDate = applicantRegistration.RegistrationDate,
                RegistrationPalce = applicantRegistration.RegistrationPalce,
                RegisterBy = applicantRegistration.RegisterBy,
                TotalResult = applicantRegistration.TotalResult,
                RegistrationDateAm = applicantRegistration.RegistrationDateAm,
                OrgBranchName = applicantRegistration.OrgBranchName,
                DateCreated = applicantRegistration.DateCreated,
                LastModified = applicantRegistration.LastModified,
                CreatedBy = applicantRegistration.CreatedBy,
                ModifiedBy = applicantRegistration.ModifiedBy,
                VacancyType = applicantRegistration.VacancyType,
                MediaVacancyCode = mediaCode,
                VacancyCode = VacancyCode,
                IsTransfered = applicantRegistration.IsTransfered


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

            int vacancyId = Convert.ToInt32(applicantRegistration.IDAndTitle.Split('-')[1]);
            int lookUpId = Convert.ToInt32(applicantRegistration.IDAndTitle.Split('-')[0]);



            if (ModelState.IsValid)
            {
                if (applicantRegistration.Age < 18)
                {
                    ModelState.AddModelError("", "The applicant age must be more than 18!!");
                }
                else if (RegistnDateAm == null)
                {
                    ModelState.AddModelError("", "Registration date is mandatory!!");
                }
                else if (RegistnDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Registration date should be less or equal to the current date!!");
                }

                else
                {

                    var entity = new ApplicantRegistration
                    {


                        VacancyId = vacancyId,
                        FirstName = applicantRegistration.FirstName,
                        MiddleName = applicantRegistration.MiddleName,
                        LastName = applicantRegistration.LastName,
                        IsTransfered = false,
                        ApplicantName = applicantRegistration.FirstName + " " + applicantRegistration.MiddleName + " " + applicantRegistration.LastName,
                        JobtitleName = applicantRegistration.JobtitleName,
                        Age = applicantRegistration.Age,
                        Sex = applicantRegistration.Sex,
                        LookupId = lookUpId,
                        RegistrationDate = RegistnDate,
                        RegistrationPalce = applicantRegistration.RegistrationPalce,
                        RegisterBy = FullName,
                        RegistrationDateAm = RegistnDateAm,
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

                    #region operation log
                    
                        UserHelper.OperationLog(Request.UserHostAddress, "Applicant Registration ", "Success", User.Identity.Name, User.Identity.Name + " add new applicant Info called " + entity.ApplicantName + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                    
                    #endregion

                }
            }
            return Json(new[] { applicantRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Editing_Create([DataSourceRequest] DataSourceRequest request, [Bind(Prefix = "models")]IEnumerable<ApplicantRegistration> transfer)
        {
            ModelState.Clear();
            //var results = new List<Vacancy>();
            Person_Employee personEmployee = new Person_Employee();
            Education education;
            Experience experience = new Experience();
            Training training = new Training();
            Address address = new Address();
            List<Person_Employee> personEmployeeList = new List<Person_Employee>();


            if (transfer != null && ModelState.IsValid)
            {
                string code = "NewEmploye " + DateTime.Now.Year;


                foreach (var applicantRegistration in transfer)
                {
                    if (!applicantRegistration.IsTransfered)
                    {

                        if (applicantRegistration.Check)
                        {
                            personEmployee = new Person_Employee();

                            personEmployee.EmployeeCode = code;
                            personEmployee.IsActive = false;
                            personEmployee.Title = 0;
                            personEmployee.FirstName = applicantRegistration.FirstName;
                            personEmployee.MiddleName = applicantRegistration.MiddleName;
                            personEmployee.LastName = applicantRegistration.LastName;
                            personEmployee.FirstNameEnglish = "Not Available";
                            personEmployee.MiddleNameEnglish = "Not Available";
                            personEmployee.LastNameEnglish = "Not Available";
                            personEmployee.BirthPlace = "Not Available";
                            personEmployee.Gender = applicantRegistration.Sex;
                            personEmployee.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                            personEmployee.CreatedBy = User.Identity.Name;
                            personEmployee.DateCreated = DateTime.Now;
                            personEmployee.LastModified = applicantRegistration.LastModified;
                            personEmployee.CreatedBy = User.Identity.Name;
                            personEmployee.ModifiedBy = applicantRegistration.ModifiedBy;
                            personEmployee.HiredDate = DateTime.Now;
                            personEmployee.DateOfBirth = DateTime.Now;


                            db.Person_Employee.Add(personEmployee);
                            db.SaveChanges();

                            #region detailInfo


                            personEmployee.Education = new List<Education>();
                            var educationInfo = db.ApplicantEducation.Where(x => x.ApplicantRegistrationId == applicantRegistration.ApplicantRegistrationId).ToList();
                            foreach (var ed in educationInfo)
                            {

                                education = new Education();
                                var splitDateto = ed.ToAmahric.Split('/');
                                RegistnDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateto[0]), Convert.ToInt32(splitDateto[1]), Convert.ToInt32(splitDateto[2]));

                                education.EmployeeId = personEmployee.EmployeeId;
                                education.OrganizationName = "Not Available";
                                education.OrgAddress = "Not Available";
                                education.EduField = ed.EducField;
                                education.EduLevel = ed.EduLevel;
                                education.OrganizationName = ed.EducationInstitution;
                                education.From = DateTime.Now;
                                education.To = RegistnDate;
                                education.ToEAmharic = ed.ToAmahric;
                                education.FromEAmharic = "Not Available";

                                //education.FromEAmharic=ed.


                                db.Education.Add(education);
                                db.SaveChanges();

                            }

                            personEmployee.Experience = new List<Experience>();
                            var experienceInfo = db.ApplicantExperience.Where(x => x.ApplicantRegistrationId == applicantRegistration.ApplicantRegistrationId).ToList();
                            foreach (var ex in experienceInfo)
                            {
                                experience = new Experience();
                                var splitDatefrom = ex.FromAmharic.Split('/');
                                RegistnDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDatefrom[0]), Convert.ToInt32(splitDatefrom[1]), Convert.ToInt32(splitDatefrom[2]));
                                var splitDateto = ex.ToAmahric.Split('/');
                                EndDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDateto[0]), Convert.ToInt32(splitDateto[1]), Convert.ToInt32(splitDateto[2]));

                                experience.EmployeeId = personEmployee.EmployeeId;
                                experience.Organozation = ex.OrganizationName;
                                experience.OrgFullAddress = "Not Available";
                                experience.Responsibility = ex.Postion;
                                experience.From = ex.From;
                                experience.TotalDuration = Convert.ToInt32(ex.Experience);
                                experience.From = RegistnDate;// DateTime.Now;
                                experience.To = EndDate;//DateTime.Now;
                                experience.FromExAmharic = ex.FromAmharic;
                                experience.ToExAmharic = ex.ToAmahric;
                                experience.ClearanceDateTime = DateTime.Now;

                                db.Experiences.Add(experience);
                                db.SaveChanges();
                            }

                            personEmployee.Training = new List<Training>();
                            var trainingInfo = db.ApplicantTraining.Where(x => x.ApplicantRegistrationId == applicantRegistration.ApplicantRegistrationId).ToList();
                            foreach (var tr in trainingInfo)
                            {
                                training = new Training();

                                training.EmployeeId = personEmployee.EmployeeId;
                                training.TrainingName = tr.TrainingName;
                                training.OrganizationName = "Not Available";
                                training.OrgAddress = "Not Available";
                                training.OrgBranchName = "Not Available";
                                training.From = DateTime.Now;
                                training.To = DateTime.Now;

                                db.EmpTrainingEducations.Add(training);
                                db.SaveChanges();
                            }
                            //var addressInfo = db.Addresses.ToList();
                            //foreach (var ad in addressInfo)
                            //{
                            //    address = new Address();
                            //    address.Category = ad.Category;
                            //    address.Value = ad.Value;
                            //    address.Remark = ad.Remark;
                            //}
                            #endregion

                        }

                        personEmployeeList.Add(personEmployee);

                        var updateStatus = db.ApplicantRegistration.Find(applicantRegistration.ApplicantRegistrationId);
                        updateStatus.IsTransfered = true;
                        db.Entry(updateStatus).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        ModelState.AddModelError("", "This Employee is Transfered Before");
                    }

                }
            }


            return Json(personEmployeeList.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantRegistration_Update([DataSourceRequest]DataSourceRequest request, ApplicantRegistration applicantRegistration)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (RegistnDateAm == null)
                {
                    RegistnDateAm = applicantRegistration.RegistrationDateAm;
                    var splitDate = RegistnDateAm.Split('/');
                    RegistnDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (applicantRegistration.Age < 18)
                {
                    ModelState.AddModelError("", "The applicant age must be more than 18!!");
                }
                else if (RegistnDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Registration date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new ApplicantRegistration
                    {
                        ApplicantRegistrationId = applicantRegistration.ApplicantRegistrationId,
                        VacancyId = applicantRegistration.VacancyId,
                        FirstName = applicantRegistration.FirstName,
                        MiddleName = applicantRegistration.MiddleName,
                        LastName = applicantRegistration.LastName,
                        IsTransfered = false,
                        ApplicantName = applicantRegistration.FirstName + " " + applicantRegistration.MiddleName + " " + applicantRegistration.LastName,
                        Age = applicantRegistration.Age,
                        Sex = applicantRegistration.Sex,
                        RegistrationDate = RegistnDate,
                        RegistrationPalce = applicantRegistration.RegistrationPalce,
                        RegisterBy = FullName,
                        RegistrationDateAm = RegistnDateAm,
                        LookupId = applicantRegistration.LookupId,
                        OrgBranchName = applicantRegistration.OrgBranchName,
                        DateCreated = applicantRegistration.DateCreated,
                        LastModified = DateTime.Now,
                        CreatedBy = applicantRegistration.CreatedBy,
                        ModifiedBy = User.Identity.Name,
                        VacancyType = GetType(applicantRegistration.VacancyId.ToString()),
                    };

                    db.ApplicantRegistration.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log
                    
                        UserHelper.OperationLog(Request.UserHostAddress, "Applicant Registration ", "Success", User.Identity.Name, User.Identity.Name + " update applicant Info of " + entity.ApplicantName + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                    
                    #endregion

                }
            }

            return Json(new[] { applicantRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantRegistration_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantRegistration applicantRegistration)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (!CheckAppRegnValidation(applicantRegistration.ApplicantRegistrationId))
                {
                    ModelState.AddModelError("", ErrorMessage);
                }
                else
                {
                    var entity = new ApplicantRegistration
                    {
                        ApplicantRegistrationId = applicantRegistration.ApplicantRegistrationId,
                        VacancyId = applicantRegistration.VacancyId,
                        FirstName = applicantRegistration.FirstName,
                        MiddleName = applicantRegistration.MiddleName,
                        LastName = applicantRegistration.LastName,
                        IsTransfered = false,
                        ApplicantName = applicantRegistration.ApplicantName,
                        Age = applicantRegistration.Age,
                        Sex = applicantRegistration.Sex,
                        LookupId = applicantRegistration.LookupId,
                        RegistrationDate = applicantRegistration.RegistrationDate,
                        RegistrationPalce = applicantRegistration.RegistrationPalce,
                        RegisterBy = applicantRegistration.RegisterBy,
                        RegistrationDateAm = applicantRegistration.RegistrationDateAm,
                        OrgBranchName = applicantRegistration.OrgBranchName,
                        DateCreated = applicantRegistration.DateCreated,
                        LastModified = applicantRegistration.LastModified,
                        CreatedBy = applicantRegistration.CreatedBy,
                        ModifiedBy = applicantRegistration.ModifiedBy,
                        VacancyType = applicantRegistration.VacancyType,
                    };
                    #region delete log
                    UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted applicant  information of applicant" + applicantRegistration.ApplicantName);
                    #endregion
                    db.ApplicantRegistration.Attach(entity);
                    db.ApplicantRegistration.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(new[] { applicantRegistration }.ToDataSourceResult(request, ModelState));
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
        public bool CheckApplicant(int id)
        {
            IQueryable<ApplicantRegistration> applicantregistration = db.ApplicantRegistration.Where(x => x.ApplicantRegistrationId == id);
            decimal screenSum = 0;
            var SettingSum = EduField + EduLevel + RelatedExp + RelevantExp + Disabliy + Gender + Profile + Training + Dicipline;

            foreach (var a in applicantregistration)
            {
                db = new ExceedDbContext();
                screenSum = 0;
                var screen =
                    db.ApplicantScreeningResult.Where(
                        x => x.ApplicantRegistrationId == a.ApplicantRegistrationId && x.ReferenceType == "External")
                        .ToList();
                if (screen != null)
                {

                    screenSum = Convert.ToDecimal(screen.Sum(x => x.CurrentResult));
                    a.TotalResult = screenSum;
                    var exam =
                 db.ApplicantExamResult.Where(
                     x => x.ApplicantRegistrationId == a.ApplicantRegistrationId && x.ReferenceType == "External")
                     .ToList();
                    if (exam != null)
                    {
                        decimal exmSum = Convert.ToDecimal(exam.Sum(x => x.CurrentResult));
                    }
                    if (screenSum >= SettingSum)
                    {
                        return true;


                    }
                }

            }
            return false;


        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
