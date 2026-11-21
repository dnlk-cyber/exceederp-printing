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
        HRMInputs inputs = new HRMInputs();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            var workunits = db.MainCompanyStructures.ToList();
            ViewData["workunits"] = workunits;
            ViewData["ApplicationVacancyCodes"] = db.VaccancyPlans.Select(x => new { x.VacancyId, x.VaccancyCode }).ToList();
            var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = listt.Where(x => x.IsActive);
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
            var vacancyJobTitles = db.VaccancyPlans.Select(x => x.JobTitle);
            var jobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE || x.LookupType == LookUpTypes.OLD_JOB_TITLE
                || vacancyJobTitles.Contains(x.LookupId)).ToList();
            ViewData["JobTitle"] = jobtitles;
        }
        public int SetEmployee(string x)
        {

            if (x == "true")
            {
                var list = UserInformationHelper.GetEmployeesByReportsTo(User.Identity.Name);

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
            var currentEmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employeeIds = ApplicationEmployeeIds(currentEmployeeId);
            var rows = db.ApplicantionFormes.Where(x => employeeIds.Contains(x.EmployeeId));
            var result = rows.ToDataSourceResult(request, form => new
            {
                form.ApplicantionFormId,
                form.EmployeeId,
                form.ApplicationType,
                form.VacancyId,
                form.TotalResult,
                form.Remark,
                form.EmployeeCode,
                form.LookupId,
                form.OrgBranchName,
                form.CreatedBy,
                form.DateCreated,
                form.ModifiedBy,
                form.LastModified,
                form.VacancyType,
                IsForOther = form.EmployeeId != currentEmployeeId,
                form.OrgStructureId
            });
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantionFormes_Create([DataSourceRequest]DataSourceRequest request, ApplicantionForm applicantionForm)
        {
            var currentEmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employee = ResolveApplicationEmployee(applicantionForm, currentEmployeeId);
            ApplicationVacancy.Apply(db, applicantionForm, ModelState);
            CheckDuplicate(applicantionForm);

            if (employee != null)
            {
                var cases = db.DisciplinaryCases.Where(x => x.EmployeeId == employee.EmployeeId)
                    .Select(x => x.DisciplinaryCaseId);
                var measures = db.EmpDisciplineMeasure.Where(x => cases.Contains(x.DisciplinaryCaseId))
                    .Select(x => new { x.DecisionDate, x.Month }).ToList();
                if (measures.Any(x => x.DecisionDate.AddMonths(x.Month) > DateTime.Today))
                    ModelState.AddModelError("", "The employee has an active disciplinary measure.");

                if ((DateTime.Today - employee.HiredDate).Days < 360)
                    ModelState.AddModelError("", "The employee needs one year of internal experience to apply.");
            }

            if (!ModelState.IsValid)
                return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));

            var entity = new ApplicantionForm
            {
                EmployeeId = employee.EmployeeId,
                EmployeeCode = employee.EmployeeCode,
                IsForOther = applicantionForm.IsForOther,
                ApplicationType = applicantionForm.ApplicationType,
                VacancyId = applicantionForm.VacancyId,
                OrgStructureId = applicantionForm.OrgStructureId,
                LookupId = applicantionForm.LookupId,
                VacancyType = applicantionForm.VacancyType,
                Remark = applicantionForm.Remark,
                OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now,
                From = DateTime.Today,
                To = DateTime.Today
            };
            db.ApplicantionFormes.Add(entity);
            db.SaveChanges();
            applicantionForm.ApplicantionFormId = entity.ApplicantionFormId;

            UserHelper.OperationLog(Request.UserHostAddress, "Application Form", "Success", User.Identity.Name,
                User.Identity.Name + " added an application for " + employee.EmployeeCode, Modules.HRM);
            var supervisor = UserInformationHelper.GetEmployeeUserId(EmployeeHelper.GetSupervisor(currentEmployeeId));
            if (!string.IsNullOrWhiteSpace(supervisor))
            {
                Notify.NotifyUser(new NotifModels
                {
                    UserId = supervisor,
                    Title = "Promotion Request",
                    Message = $"{UserInformationHelper.GetEmployeeFullName(User.Identity.Name)} submitted an application.",
                    URL = "~/HRM/ApplicationForm"
                });
            }

            return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantionFormes_Update([DataSourceRequest]DataSourceRequest request, ApplicantionForm applicantionForm)
        {
            var currentEmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var employeeIds = ApplicationEmployeeIds(currentEmployeeId);
            var entity = db.ApplicantionFormes.SingleOrDefault(x => x.ApplicantionFormId == applicantionForm.ApplicantionFormId
                && employeeIds.Contains(x.EmployeeId));
            if (entity == null)
            {
                ModelState.AddModelError("", "This application is unavailable.");
                return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));
            }

            var employee = ResolveApplicationEmployee(applicantionForm, currentEmployeeId);
            ApplicationVacancy.Apply(db, applicantionForm, ModelState, entity.VacancyId);
            CheckDuplicate(applicantionForm, entity.ApplicantionFormId);

            if (ModelState.IsValid)
            {
                entity.EmployeeId = employee.EmployeeId;
                entity.EmployeeCode = employee.EmployeeCode;
                entity.IsForOther = applicantionForm.IsForOther;
                entity.VacancyId = applicantionForm.VacancyId;
                entity.OrgStructureId = applicantionForm.OrgStructureId;
                entity.LookupId = applicantionForm.LookupId;
                entity.VacancyType = applicantionForm.VacancyType;
                entity.Remark = applicantionForm.Remark;
                entity.ModifiedBy = User.Identity.Name;
                entity.LastModified = DateTime.Now;
                db.SaveChanges();
                UserHelper.OperationLog(Request.UserHostAddress, "Application Form", "Success", User.Identity.Name,
                    User.Identity.Name + " updated an application for " + employee.EmployeeCode, Modules.HRM);
            }

            return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantionFormes_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantionForm applicantionForm)
        {
            var employeeIds = ApplicationEmployeeIds(UserInformationHelper.GetEmployeeId(User.Identity.Name));
            var entity = db.ApplicantionFormes.SingleOrDefault(x => x.ApplicantionFormId == applicantionForm.ApplicantionFormId
                && employeeIds.Contains(x.EmployeeId));
            if (entity == null)
            {
                ModelState.AddModelError("", "This application is unavailable.");
            }
            else
            {
                db.ApplicantionFormes.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name,
                    "Deleted application for " + entity.EmployeeCode);
            }

            return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));
        }

        private static List<int> ApplicationEmployeeIds(int currentEmployeeId)
        {
            if (currentEmployeeId <= 0)
                return new List<int>();

            var employeeIds = EmployeeHelper.GetSubordinates_FilteredByHead(currentEmployeeId);
            if (!employeeIds.Contains(currentEmployeeId))
                employeeIds.Add(currentEmployeeId);
            return employeeIds;
        }

        private Person_Employee ResolveApplicationEmployee(ApplicantionForm form, int currentEmployeeId)
        {
            if (!form.IsForOther)
            {
                form.EmployeeId = currentEmployeeId;
                ModelState.Remove(nameof(form.EmployeeId));
            }

            var employeeIds = ApplicationEmployeeIds(currentEmployeeId);
            var employee = db.Person_Employee.SingleOrDefault(x => x.EmployeeId == form.EmployeeId
                && employeeIds.Contains(x.EmployeeId) && x.IsActive);
            if (employee == null)
                ModelState.AddModelError(nameof(form.EmployeeId), "Select an employee you can apply for, or check your employee account.");
            else
                form.EmployeeCode = employee.EmployeeCode;
            return employee;
        }

        private void CheckDuplicate(ApplicantionForm form, int currentApplicationId = 0)
        {
            if (db.ApplicantionFormes.Any(x => x.EmployeeId == form.EmployeeId && x.VacancyId == form.VacancyId
                && x.ApplicantionFormId != currentApplicationId))
            {
                ModelState.AddModelError("", "This employee already has an application for the vacancy.");
            }
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
