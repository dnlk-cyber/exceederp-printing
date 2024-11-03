using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class InternalApplicantExamResultController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            HRMInputs inputs = new HRMInputs();
            var listt = inputs.GetEmployees(User.Identity.Name);
            ViewData["EmployeeList"] = listt.Where(x => x.IsActive);

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
            List<Vacancy> list = new List<Vacancy>();
            var vacancies = db.RecruitmentVacancy.ToList();
            if (vacancies.Any())
            {
                var recruit = db.VaccancyPlans.ToList();
                var vac = new Vacancy();
                foreach (var r in recruit)
                {
                    foreach (var v in vacancies)
                    {
                        var vacancydetail = db.VaccancyPlans.Where(x => x.VacancyId == v.VacancyId).FirstOrDefault();
                        if (vacancydetail != null)
                        {
                            if (vacancydetail.RecruSource == RecruitmentSource.INTERNAL)
                            {
                                if (v.VacancyId == r.VacancyId)
                                {
                                    vac = new Vacancy();
                                    vac = vacancydetail;
                                    list.Add(vac);
                                }
                            }
                        }
                    }
                }

            }
            ViewData["Vacancy"] = list;
        }
       
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult InternalApplicantExamResult_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<InternalApplicantExamResult> internalApplicantexamresult = db.InternalApplicantExamResults.Where(x => x.InternalApplicantScreeningId == id);
            //if (!internalApplicantexamresult.Any())
            //{
            //    Configartion(id);
            //    applicantexamresult = db.ApplicantExamResult.Where(x => x.ApplicantScreeningId == id && x.ReferenceType == "Internal");
            //}
            DataSourceResult result = internalApplicantexamresult.ToDataSourceResult(request, applicantExamResult => new
            {
                InternalApplicantExamResultId = applicantExamResult.InternalApplicantExamResultId,
                InternalApplicantScreeningId = applicantExamResult.InternalApplicantScreeningId,
                SettingValue = applicantExamResult.SettingValue,
                CurrentResult = applicantExamResult.CurrentResult,
                Total = applicantExamResult.Total,
                ExamType = applicantExamResult.ExamType,
                Remark = applicantExamResult.Remark,
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantExamResult_Create([DataSourceRequest]DataSourceRequest request, InternalApplicantExamResult applicantExamResult, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (applicantExamResult.SettingValue < applicantExamResult.CurrentResult)
                {
                    return new JavaScriptResult() { Script = "alert('" + " The result must be less or equal to the setting value!!" + "');" };
                }
                var entity = new InternalApplicantExamResult
                {
                    InternalApplicantScreeningId = id,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    Total = applicantExamResult.Total,
                    ExamType = applicantExamResult.ExamType,
                    Remark = applicantExamResult.Remark,
                };

                db.InternalApplicantExamResults.Add(entity);
                db.SaveChanges();
                applicantExamResult.InternalApplicantExamResultId = entity.InternalApplicantExamResultId;
            }

            return Json(new[] { applicantExamResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantExamResult_Update([DataSourceRequest]DataSourceRequest request, InternalApplicantExamResult applicantExamResult)
        {
            if (ModelState.IsValid)
            {
                if (applicantExamResult.SettingValue < applicantExamResult.CurrentResult)
                {
                    return new JavaScriptResult() { Script = "alert('" + " The result must be less or equal to the setting value!!" + "');" };
                }
                var entity = new InternalApplicantExamResult
                {
                    InternalApplicantExamResultId = applicantExamResult.InternalApplicantExamResultId,
                    InternalApplicantScreeningId = applicantExamResult.InternalApplicantScreeningId,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    Total = applicantExamResult.Total,
                    ExamType = applicantExamResult.ExamType,
                    Remark = applicantExamResult.Remark,
                };

                db.InternalApplicantExamResults.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { applicantExamResult }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantExamResult_Destroy([DataSourceRequest]DataSourceRequest request, InternalApplicantExamResult applicantExamResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalApplicantExamResult
                {
                    InternalApplicantExamResultId = applicantExamResult.InternalApplicantExamResultId,
                    InternalApplicantScreeningId = applicantExamResult.InternalApplicantScreeningId,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    Total = applicantExamResult.Total,
                    ExamType = applicantExamResult.ExamType,
                    Remark = applicantExamResult.Remark,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted internal applicant information of applicant  " + applicantExamResult.InternalApplicantScreeningId);
                #endregion
                db.InternalApplicantExamResults.Attach(entity);
                db.InternalApplicantExamResults.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { applicantExamResult }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
