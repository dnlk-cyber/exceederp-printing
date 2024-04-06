using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Global;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{

    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRDepartmentHead)]
    public class ApplicationFormController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static int NoOfPromotion { get; set; }
        HRMInputs inputs = new HRMInputs();
        public static bool IsForOther { get; set; }
        public static int EmployeeId { get; set; }
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var workunits = db.MainCompanyStructures.ToList();
            ViewData["workunits"] = workunits;
            var recruitment = db.RecruitmentVacancy.ToList();
            ViewData["recruitmentVacancy"] = recruitment;
            var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = listt.Where(x => x.IsActive);
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employeName = db.Person_Employee.ToList();
            ViewData["EmployeeName"] = employeName;
            var appTypes = Enum.GetValues(typeof(ApplicationType))
                .Cast<ApplicationType>().ToList();
            var media = db.Lookups.Where(x => x.LookupType == LookUpTypes.MEDIA);
            ViewData["Media"] = media;
            var appTypeses = appTypes.Select(s => new { Text = s.ToString(), Value = s.ToString() });
            ViewData["AppTypes"] = appTypeses;
            var levels = db.Lookups.Where(l => l.LookupType == LookUpTypes.EDUCATION_LEVEL);
            ViewData["EduLevelList"] = levels;
            var ranks = db.Ranks.ToList();
            ViewData["RankList"] = ranks;

            var vacancies = db.VaccancyPlans.Where(x => x.RecruSource == RecruitmentSource.INTERNAL).ToList();
            ViewData["Vacancy"] = vacancies;
            var settings =
                db.Configurations.Where(x => x.Category == "Promotion" && x.Reference == "PROMOTION_PER_YEAR").FirstOrDefault();
            if (settings != null)
            {
                NoOfPromotion = Convert.ToInt32(settings.CurrentValue);
            }
            EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var jobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE).ToList();
            ViewData["JobTitle"] = jobtitles;
        }
        public int SetEmployee(string x)
        {
            IsForOther = false;

            if (x == "true")
            {
                var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);

                IsForOther = true;
                return list.Count();

            }
            return 0;
        }
        public JsonResult GetOrganizations()
        {

            var structres = db.MainCompanyStructures.AsQueryable();
            ViewData["organization"] = structres;
            return Json(structres.Select(s => new { OrgStructureId = s.OrgStructureId, Name = s.Name }), JsonRequestBehavior.AllowGet);
        }
        public static decimal EduLevel { get; set; }
        public static decimal EduField { get; set; }
        public static decimal RelevantExp { get; set; }
        public static decimal RelatedExp { get; set; }
        public static decimal Gender { get; set; }
        public static decimal Disabliy { get; set; }
        public static decimal Profile { get; set; }
        public static decimal Training { get; set; }
        public static decimal Dicipline { get; set; }
        private void SetSettingValues()
        {
            var settings =
                db.Configurations.Where(x => x.Category == "Recruitment" && x.Reference == "Recruitment").ToList();
            if (settings.Any())
            {
                foreach (var s in settings)
                {
                    switch (s.Attribute)
                    {
                        case "EDUCATION_LEVEL":
                            EduLevel = Convert.ToDecimal(s.CurrentValue);
                            break;

                        case "EDUCATIONAL_FIELD":

                            EduField = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "RELATED_EXPERIENCE":

                            RelatedExp = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "RELEVANT_EXPERIENCE":

                            RelevantExp = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "GENDER":

                            Gender = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "DISABILITY":

                            Disabliy = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "PROFILE":

                            Profile = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "TRAINING":

                            Training = Convert.ToDecimal(s.CurrentValue);
                            break;
                        case "DISCIPLINE ":

                            Dicipline = Convert.ToDecimal(s.CurrentValue);
                            break;
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
        public ActionResult ApplicantionFormes_Read([DataSourceRequest]DataSourceRequest request, string id)
        {
            SetSettingValues();
            decimal screenSum = 0;
            var employeeid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            List<int> employeeids = EmployeeHelper.GetSubordinates_FilteredByBranchAlso(employeeid);
            IQueryable<ApplicantionForm> applicarionForm = db.ApplicantionFormes.Where(x => employeeids.Contains(x.EmployeeId));

            //IQueryable<ApplicantionForm> applicantionformes = db.ApplicantionFormes.Where(x => x.EmployeeId == EmployeeId);
            List<ApplicantionForm> applicantRegi = new List<ApplicantionForm>();
            var SettingSum = EduField + EduLevel + RelatedExp + RelevantExp + Disabliy + Gender + Profile + Training + Dicipline;

            //if (id == "Screen")
            //{
            //    foreach (var a in applicarionForm)
            //    {
            //        db = new ExceedDbContext();
            //        var screen =
            //            db.ApplicantScreeningResult.Where(
            //                x => x.ApplicantRegistrationId == a.ApplicantionFormId && x.ReferenceType == "Internal")
            //                .ToList();
            //        if (screen != null)
            //        {

            //            screenSum = Convert.ToDecimal(screen.Sum(x => x.CurrentResult));
            //            a.TotalResult = screenSum;
            //            var exam =
            //                db.ApplicantExamResult.Where(
            //                    x => x.ApplicantRegistrationId == a.ApplicantionFormId && x.ReferenceType == "Internal")
            //                    .ToList();
            //            if (exam != null)
            //            {
            //                decimal exmSum = Convert.ToDecimal(exam.Sum(x => x.CurrentResult));
            //            }
            //            if (screenSum >= SettingSum)
            //            {
            //                applicantRegi.Add(a);


            //            }
            //        }

            //    }
            //    applicarionForm = applicantRegi.AsQueryable();
            //}
            //else
            //{
            //    foreach (var a in applicarionForm)
            //    {
            //        db = new ExceedDbContext();
            //        var screen =
            //            db.ApplicantScreeningResult.Where(x => x.ApplicantRegistrationId == a.ApplicantionFormId && x.ReferenceType == "Internal")
            //                .ToList();
            //        if (screen != null)
            //        {

            //            screenSum = Convert.ToDecimal(screen.Sum(x => x.CurrentResult));
            //            a.TotalResult = screenSum;

            //            applicantRegi.Add(a);
            //        }
            //    }
            //}



            DataSourceResult result = applicarionForm.ToDataSourceResult(request, applicantionForm => new
            {
                ApplicantionFormId = applicantionForm.ApplicantionFormId,
                EmployeeId = applicantionForm.EmployeeId,
                ApplicationType = applicantionForm.ApplicationType,
                VacancyId = applicantionForm.VacancyId,
                TotalResult = applicantionForm.TotalResult,
                Remark = applicantionForm.Remark,
                EmployeeCode = applicantionForm.EmployeeCode,
                LookupId = applicantionForm.LookupId,
                OrgBranchName = applicantionForm.OrgBranchName,
                CreatedBy = applicantionForm.CreatedBy,
                DateCreated = applicantionForm.DateCreated,
                ModifiedBy = applicantionForm.ModifiedBy,
                LastModified = applicantionForm.LastModified,
                VacancyType = applicantionForm.VacancyType,
                IsForOther = applicantionForm.IsForOther,
                OrgStructureId = applicantionForm.OrgStructureId
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
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantionFormes_Create([DataSourceRequest]DataSourceRequest request, ApplicantionForm applicantionForm)
        {
            ModelState.Clear();

            //int vacancyId = Convert.ToInt32(applicantionForm.IDAndTitle.Split('-')[1]);
            //int lookUpId = Convert.ToInt32(applicantionForm.IDAndTitle.Split('-')[0]);

            if (applicantionForm.EmployeeId == 0 && applicantionForm.IsForOther)
            {
                ModelState.AddModelError("", "Employee ID and User are not mapped");
            }

            //var placmenets = db.Placements.Where( x =>
            //            x.AssignmentDate.Year == DateTime.Now.Year &&
            //            x.EmployeeId == applicantionForm.EmployeeId && x.AssignmentType == AssignmentType.PROMOTION).ToList();
            //if (placmenets.Count > NoOfPromotion)
            //{
            //    RedirectToAction("Index");
            //    string messages = "You can not apply for this vacancy. \n You already promoted " + placmenets.Count + " times";
            //    return new JavaScriptResult() { Script = "alert('" + messages + "');" };

            //}

            if (!applicantionForm.IsForOther)
            {
                applicantionForm.EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                if (applicantionForm.EmployeeId == 0)
                {
                    string messages = "Employee and User are not mapped";
                    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                }

                var appForm = db.ApplicantionFormes.FirstOrDefault(x => x.EmployeeId == applicantionForm.EmployeeId && x.VacancyId == applicantionForm.VacancyId);

                if (appForm != null)
                {
                    string messages = "you were apply for this vacancy";
                    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                }
            }
            else
            {
                applicantionForm.EmployeeId = applicantionForm.EmployeeId;
                var appForm = db.ApplicantionFormes.FirstOrDefault(x => x.EmployeeId == applicantionForm.EmployeeId && x.VacancyId == applicantionForm.VacancyId);

                if (appForm != null)
                {
                    string messages = "you were apply for this vacancy";
                    return new JavaScriptResult() { Script = "alert('" + messages + "');" };
                }
            }

            var desciplines = db.DisciplinaryCases.Where(x => x.EmployeeId == applicantionForm.EmployeeId).ToList();
            if (desciplines.Any())
            {
                foreach (var disc in desciplines)
                {
                    var disciplineMeasures = db.EmpDisciplineMeasure.Where(x => x.DisciplinaryCaseId == disc.DisciplinaryCaseId).ToList();
                    foreach (var dm in disciplineMeasures)
                    {
                        var dif = dm.DecisionDate.AddMonths(dm.Month);
                        if (dif > DateTime.Today)
                        {
                            ModelState.AddModelError("", "you can not apply, you have Discipline case");
                        }
                    }
                }
            }

            //var placement = db.Placements.Where(x => x.EmployeeId == applicantionForm.EmployeeId && x.AssignmentType == AssignmentType.PROMOTION);
            //if (placement.Any())
            //{
            //    var dates = placement.Select(x => x.AssignmentDate).ToList();
            //    var maxdate = dates.Max();
               
            //    var days = DateTime.Today.Date.Subtract(maxdate).Days;
            //    var month = days / 30;
            //    var year = (int)month / 12;
            //    if (year < 1)
            //    {
            //        ModelState.AddModelError("", "you are promoted this year you can't promoted morethan once ayear ");
            //    }
            //}

            var profile = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == applicantionForm.EmployeeId);
            if (profile != null)
            {
                var days = DateTime.Today.Date.Subtract(profile.HiredDate).Days;
                var month = days / 30;
                var year = (int)month / 12;
                if (year < 1)
                {
                    ModelState.AddModelError("", "you are new employee you can't be promoted before 1 year internal experience");
                }
            }

            if (ModelState.IsValid)
            {
                var entity = new ApplicantionForm
                {
                    EmployeeId = applicantionForm.EmployeeId,
                    ApplicationType = applicantionForm.ApplicationType,
                    VacancyId = applicantionForm.VacancyId,
                    OrgStructureId = applicantionForm.OrgStructureId,
                    Remark = applicantionForm.Remark,
                    LookupId = applicantionForm.LookupId,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == EmployeeId)?.EmployeeCode,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    ModifiedBy = applicantionForm.ModifiedBy,
                    LastModified = applicantionForm.LastModified,
                    VacancyType = GetType(applicantionForm.VacancyId.ToString()),
                    From = DateTime.Today,
                    To = DateTime.Today
                };

                db.ApplicantionFormes.Add(entity);
                db.SaveChanges();
                applicantionForm.ApplicantionFormId = entity.ApplicantionFormId;

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Application Form", "Success", User.Identity.Name, User.Identity.Name + " add new Application for " + person.EmployeeCode, Modules.HRM);
                }
                #endregion

            }

            #region notification



            var empid = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var superviorId = EmployeeHelper.GetSupervisor(empid);

            NotifModels notif = new NotifModels
            {

                UserId = UserInformationHelper.GetEmployeeUserId(superviorId),
                Title = "Promotion Request",
                Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has request Promotion",
                URL = "~/HRM/ApplicationForm"
            };

            Notify.NotifyUser(notif);

            #endregion


            return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantionFormes_Update([DataSourceRequest]DataSourceRequest request, ApplicantionForm applicantionForm)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ApplicantionForm
                {
                    ApplicantionFormId = applicantionForm.ApplicantionFormId,
                    EmployeeId = applicantionForm.EmployeeId,
                    ApplicationType = applicantionForm.ApplicationType,
                    VacancyId = applicantionForm.VacancyId,
                    OrgStructureId = applicantionForm.OrgStructureId,
                    Remark = applicantionForm.Remark,
                    EmployeeCode = inputs.GetEmployees().FirstOrDefault(x => x.PersonId == EmployeeId)?.EmployeeCode,
                    LookupId = applicantionForm.LookupId,
                    OrgBranchName = applicantionForm.OrgBranchName,
                    CreatedBy = applicantionForm.CreatedBy,
                    DateCreated = applicantionForm.DateCreated,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    VacancyType = GetType(applicantionForm.VacancyId.ToString()),
                };

                db.ApplicantionFormes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Application Form", "Success", User.Identity.Name, User.Identity.Name + " update Application form info of " + person.EmployeeCode, Modules.HRM);

                }
                #endregion
            }

            return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantionFormes_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantionForm applicantionForm)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ApplicantionForm
                {
                    ApplicantionFormId = applicantionForm.ApplicantionFormId,
                    EmployeeId = applicantionForm.EmployeeId,
                    ApplicationType = applicantionForm.ApplicationType,
                    VacancyId = applicantionForm.VacancyId,
                    OrgStructureId = applicantionForm.OrgStructureId,
                    Remark = applicantionForm.Remark,
                    EmployeeCode = applicantionForm.EmployeeCode,
                    LookupId = applicantionForm.LookupId,
                    OrgBranchName = applicantionForm.OrgBranchName,
                    CreatedBy = applicantionForm.CreatedBy,
                    DateCreated = applicantionForm.DateCreated,
                    ModifiedBy = applicantionForm.ModifiedBy,
                    LastModified = applicantionForm.LastModified,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted application form of employee code  " + applicantionForm.EmployeeCode);
                #endregion
                db.ApplicantionFormes.Attach(entity);
                db.ApplicantionFormes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));
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
                        //var lookup = db.Lookups.Where(x => x.LookupId == detail.Media_LookupId).FirstOrDefault();
                        //if (lookup != null)
                        //{
                        //    titleList.Add(lookup);
                        //}
                    }


                }
            }
            return Json(titleList.Select(p => new
            {
                LookupId = p.LookupId,
                Description = p.Description
            }), JsonRequestBehavior.AllowGet);

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
