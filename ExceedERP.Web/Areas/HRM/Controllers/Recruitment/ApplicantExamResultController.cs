using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
  
    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
    public class ApplicantExamResultController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        private void Configartion(int applicationId)
        {
            ApplicantExamResult applicantScreeningResult = new ApplicantExamResult();
            List<ApplicantExamResult> applicantScreeningResultes = new List<ApplicantExamResult>();
            var setingvalues = db.Configurations.Where(x => x.Reference == "Recruitment" && x.Category == "Recruitment_External").ToList();
            var recruitmentSetting = Enum.GetValues(typeof(RecruitmentSetting))
                           .Cast<RecruitmentSetting>()
                           .ToList();
            if (recruitmentSetting.Any())
            {
                foreach (var p in recruitmentSetting)
                {
                    if (p.ToString() != "THEORY_EXAM" && p.ToString() != "PRACTICAL_EXAM" && p.ToString() != "INTERVIEW")
                    {
                        continue;
                    }

                    applicantScreeningResult = new ApplicantExamResult();
                   // applicantScreeningResult.Criteria = p.ToString();
                    if (setingvalues.Any())
                    {
                        var oneValue = setingvalues.Where(x => x.Attribute == p.ToString()).FirstOrDefault();
                        if (oneValue != null)
                        {
                            applicantScreeningResult.SettingValue = Convert.ToDecimal(oneValue.CurrentValue);
                        }
                    }
                  //  applicantScreeningResult.ReferenceType = "External";
                    applicantScreeningResult.CurrentResult = 0;
                 //   applicantScreeningResult.Total = 0;
                    applicantScreeningResult.ApplicantScreeningId = applicationId;
                    applicantScreeningResultes.Add(applicantScreeningResult);

                }
            }
            db = new ExceedDbContext();
            db.ApplicantExamResult.AddRange(applicantScreeningResultes);
            db.SaveChanges();
        }
        
    
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult ApplicantExamResult_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ApplicantExamResult> applicantexamresult = db.ApplicantExamResult.Where(x => x.ApplicantScreeningId == id);
           
            DataSourceResult result = applicantexamresult.ToDataSourceResult(request, applicantExamResult => new
            {
                ApplicantResultId = applicantExamResult.ApplicantResultId,
                ApplicantScreeningId = applicantExamResult.ApplicantScreeningId,
                SettingValue = applicantExamResult.SettingValue,
                CurrentResult = applicantExamResult.CurrentResult,
                ActualValue = applicantExamResult.ActualValue,
                ExamType = applicantExamResult.ExamType,
                Remark = applicantExamResult.Remark,
            });

            return Json(result);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult PrepareExternalExamResults([DataSourceRequest] DataSourceRequest request, int id)
        {
            var externalScreening = from applicant in db.ApplicantScreenings
                                    join screening in db.ApplicantScreenResults on applicant.ApplicantScreenResultId equals screening.ApplicantScreenResultId
                                    join vacancy in db.VaccancyPlans on screening.VacancyId equals vacancy.VacancyId
                                    where applicant.ApplicantScreeningId == id && vacancy.RecruSource == RecruitmentSource.EXTERNAL
                                    select applicant.ApplicantScreeningId;
            if (!externalScreening.Any())
                return ApplicantExamResult_Read(request, id);

            var examSettings = GetExternalExamSettings(db);
            var existingTypes = db.ApplicantExamResult.Where(x => x.ApplicantScreeningId == id).Select(x => x.ExamType).ToList();
            if (examSettings.Keys.Except(existingTypes).Any())
            {
                using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
                {
                    AddMissingExamRows(db, id, examSettings);
                    db.SaveChanges();
                    transaction.Commit();
                }
            }

            return ApplicantExamResult_Read(request, id);
        }

        internal static IReadOnlyDictionary<ExamType, decimal> GetExternalExamSettings(ExceedDbContext context)
        {
            var settings = context.InternalRecruitmentCriteriaTypeSettings
                .Where(x => x.ExamSource == RecruitmentSource.EXTERNAL)
                .OrderBy(x => x.InternalRecruitmentCriteriaTypeSettingId)
                .Select(x => new { x.ExamType, x.SettingVlaue }).ToList();
            if (settings.Count == 0)
            {
                settings = context.ExamTypeSettings.Where(x => x.ExamSource == RecruitmentSource.EXTERNAL)
                    .OrderBy(x => x.ExamTypeSettingId)
                    .Select(x => new { x.ExamType, x.SettingVlaue }).ToList();
            }

            return settings.GroupBy(x => x.ExamType).ToDictionary(x => x.Key, x => x.First().SettingVlaue);
        }

        internal static void AddMissingExamRows(ExceedDbContext context, int applicantScreeningId, IReadOnlyDictionary<ExamType, decimal> examSettings)
        {
            var existingTypes = context.ApplicantExamResult.Where(x => x.ApplicantScreeningId == applicantScreeningId)
                .Select(x => x.ExamType).ToList();
            foreach (var examType in examSettings.Keys.Except(existingTypes))
            {
                context.ApplicantExamResult.Add(new ApplicantExamResult
                {
                    ApplicantScreeningId = applicantScreeningId,
                    ExamType = examType,
                    SettingValue = examSettings[examType]
                });
            }
        }
       
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExamResult_Create([DataSourceRequest]DataSourceRequest request, ApplicantExamResult applicantExamResult, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var settingValue = db.InternalRecruitmentCriteriaTypeSettings.FirstOrDefault(x=>x.ExamType == applicantExamResult.ExamType && x.ExamSource == RecruitmentSource.EXTERNAL);
                if(settingValue != null)
                {
                    applicantExamResult.SettingValue = settingValue.SettingVlaue;
                }
                var entity = new ApplicantExamResult
                {
                    ApplicantScreeningId = id,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    ActualValue = applicantExamResult.ActualValue,
                    ExamType = applicantExamResult.ExamType,
                    Remark = applicantExamResult.Remark,
                };

                db.ApplicantExamResult.Add(entity);
                db.SaveChanges();
                applicantExamResult.ApplicantResultId = entity.ApplicantResultId;
            }

            return Json(new[] { applicantExamResult }.ToDataSourceResult(request, ModelState));
        }
     
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExamResult_Update([DataSourceRequest]DataSourceRequest request, ApplicantExamResult applicantExamResult)
        {
            var result = db.ApplicantExamResult.Find(applicantExamResult.ApplicantResultId);
            if (result == null)
            {
                ModelState.AddModelError("", "The exam result could not be found. Refresh the grid and try again.");
                return Json(new[] { applicantExamResult }.ToDataSourceResult(request, ModelState));
            }

            if (applicantExamResult.CurrentResult < 0)
                ModelState.AddModelError(nameof(applicantExamResult.CurrentResult), "Enter a score of zero or higher.");
            if (result.SettingValue > 0 && applicantExamResult.CurrentResult > result.SettingValue)
                ModelState.AddModelError(nameof(applicantExamResult.CurrentResult), "The score cannot exceed the setting value.");

            if (!ModelState.IsValid)
                return Json(new[] { applicantExamResult }.ToDataSourceResult(request, ModelState));

            result.CurrentResult = applicantExamResult.CurrentResult;
            result.ActualValue = applicantExamResult.ActualValue;
            result.Remark = applicantExamResult.Remark;
            db.SaveChanges();

            var dataResult = new[] { result }.ToDataSourceResult(request, ModelState);
            var screening = db.ApplicantScreenings.Find(result.ApplicantScreeningId);
            if (screening == null)
                return Json(dataResult);

            var screeningResult = db.ApplicantScreenResults.Find(screening.ApplicantScreenResultId);
            var total = ApplicantScreeningResultController.GetTotalResult(db, result.ApplicantScreeningId);
            return Json(new
            {
                dataResult.Data,
                dataResult.Total,
                dataResult.Errors,
                result.ApplicantScreeningId,
                ApplicantTotal = total,
                PassOrFail = ApplicantScreeningResultController.PassOrFail(db, screening, screeningResult?.VacancyId ?? 0, total)
            });
        }
     
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExamResult_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantExamResult applicantExamResult)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApplicantExamResult
                {
                    ApplicantResultId = applicantExamResult.ApplicantResultId,
                    ApplicantScreeningId = applicantExamResult.ApplicantScreeningId,
                    SettingValue = applicantExamResult.SettingValue,
                    CurrentResult = applicantExamResult.CurrentResult,
                    ActualValue = applicantExamResult.ActualValue,
                    ExamType = applicantExamResult.ExamType,
                    Remark = applicantExamResult.Remark,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted applicant exam result information of applicant" + applicantExamResult.ApplicantScreeningId);
                #endregion
                db.ApplicantExamResult.Attach(entity);
                db.ApplicantExamResult.Remove(entity);
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
