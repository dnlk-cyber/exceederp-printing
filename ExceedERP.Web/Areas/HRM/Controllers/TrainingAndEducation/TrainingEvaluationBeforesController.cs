using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.HRM.Controllers.Company;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.HRM.HRMPlan;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    public class TrainingEvaluationBeforesController : BaseController
    {
        // GET: HRM/TrainingAssessment
        private ExceedDbContext db = new ExceedDbContext();

      
      
        public ActionResult TrainingEvaluationBefores_Read([DataSourceRequest]DataSourceRequest request,int requestId)
        {
            IQueryable<TrainingEvaluationBefore> trainingNeedRequests = db.TrainingEvaluationBefores.Where(x=>x.TrainingNeedRequestId == requestId);//.Where(x => mybraches.Contains(x.OrgBranchName)&&x.IsTransfered==false);
            DataSourceResult result = trainingNeedRequests.ToDataSourceResult(request, trainingNeedRequest => new
            {
                TrainingEvaluationBeforeId = trainingNeedRequest.TrainingEvaluationBeforeId,
                TrainingNeedRequestId = trainingNeedRequest.TrainingNeedRequestId,
                Name = trainingNeedRequest.Name,
                Criteria = trainingNeedRequest.Criteria,
                TrainingEvaluationCriteriaId = trainingNeedRequest.TrainingEvaluationCriteriaId,
                TrainingEvaluationSettingCategoryId = trainingNeedRequest.TrainingEvaluationSettingCategoryId,
                Weight = trainingNeedRequest.Weight,
                Result = trainingNeedRequest.Result
            });

            return Json(result);
        }
      
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationBefores_Create([DataSourceRequest]DataSourceRequest request, TrainingEvaluationBefore trainingNeedRequest, int assesId)
        {
            ModelState.Clear();
           
            if (ModelState.IsValid)
            {
                trainingNeedRequest.TrainingNeedRequestId = assesId;
                var entity = GetMappedObject(trainingNeedRequest);
                db.TrainingEvaluationBefores.Add(entity);
                db.SaveChanges();
                trainingNeedRequest.TrainingNeedRequestId = entity.TrainingNeedRequestId;
            }

            return Json(new[] { trainingNeedRequest }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationBefores_Update([DataSourceRequest]DataSourceRequest request, TrainingEvaluationBefore trainingNeedRequest)
        {
            ModelState.Clear();
        
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(trainingNeedRequest);
                db.TrainingEvaluationBefores.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { trainingNeedRequest }.ToDataSourceResult(request, ModelState));
        }
    
        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationBefores_Destroy([DataSourceRequest]DataSourceRequest request, TrainingEvaluationBefore trainingNeedRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(trainingNeedRequest);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted need assessment information of course " + trainingNeedRequest.TrainingNeedRequestId);
                #endregion
                db.TrainingEvaluationBefores.Attach(entity);
                db.TrainingEvaluationBefores.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { trainingNeedRequest }.ToDataSourceResult(request, ModelState));
        }
        private TrainingEvaluationBefore GetMappedObject(TrainingEvaluationBefore trainingNeedRequest)
        {
            var entity = new TrainingEvaluationBefore
            {
                TrainingEvaluationBeforeId = trainingNeedRequest.TrainingEvaluationBeforeId,
                TrainingNeedRequestId = trainingNeedRequest.TrainingNeedRequestId,
                Name = trainingNeedRequest.Name,
                Criteria = trainingNeedRequest.Criteria,
                TrainingEvaluationCriteriaId = trainingNeedRequest.TrainingEvaluationCriteriaId,
                TrainingEvaluationSettingCategoryId = trainingNeedRequest.TrainingEvaluationSettingCategoryId,
                Weight = trainingNeedRequest.Weight,
                Result = trainingNeedRequest.Result

            };
            return entity;
        }
        [HttpPost, ValidateAntiForgeryToken, AuthorizeRoles(HrmConstants.HRTrainingUser)]
        public JsonResult GenerateEvaluation(int requestId)
        {
            using (var transaction = db.Database.BeginTransaction(System.Data.IsolationLevel.Serializable))
            {
                var trainingRequest = db.TrainingNeedRequests.Find(requestId);
                if (trainingRequest == null || !(
                    from detail in db.NeedAssessmentSummaryDetails
                    join summary in db.NeedAssessmentSummary on detail.AssesmentSummaryId equals summary.NeedAssessmentSummaryId
                    where detail.AssesmentId == trainingRequest.AssesmentId && summary.LookupId == trainingRequest.LookupId
                        && summary.IsThirdSendForApproval && summary.IsApproved && !summary.IsRejected
                    select summary.NeedAssessmentSummaryId).Any())
                    return Json(new { Message = "Select a request in an approved training summary.", Status = "not-okay" });

                var courseId = trainingRequest.LookupId;
                var hasCourseCriteria = db.TrainingEvaluationCriterias.Any(x => x.TrainingCourseId == courseId);
                var existing = db.TrainingEvaluationBefores.Where(x => x.TrainingNeedRequestId == requestId)
                    .Select(x => x.TrainingEvaluationCriteriaId);
                var entries = (from criterion in db.TrainingEvaluationCriterias
                               join category in db.TrainingEvaluationCriteriaCategories
                                   on criterion.TrainingEvaluationSettingCategoryId equals category.TrainingEvaluationSettingCategoryId
                               where (hasCourseCriteria ? criterion.TrainingCourseId == courseId : !criterion.TrainingCourseId.HasValue)
                                   && !existing.Contains(criterion.TrainingEvaluationCriteriaId)
                               select new { Criterion = criterion, category.Name }).ToList();
                db.TrainingEvaluationBefores.AddRange(entries.Select(entry => new TrainingEvaluationBefore
                {
                    TrainingNeedRequestId = requestId,
                    TrainingEvaluationSettingCategoryId = entry.Criterion.TrainingEvaluationSettingCategoryId,
                    Name = entry.Name,
                    TrainingEvaluationCriteriaId = entry.Criterion.TrainingEvaluationCriteriaId,
                    Criteria = entry.Criterion.Criteria,
                    Weight = entry.Criterion.Weight,
                    Result = entry.Criterion.Result
                }));
                db.SaveChanges();
                transaction.Commit();
                return Json(new
                {
                    Message = entries.Count > 0 ? "Evaluation criteria generated." : "No new evaluation criteria found.",
                    Status = entries.Count > 0 ? "Okay" : "not-okay"
                });
            }
        }

        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Recruitment plan ");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Recruitment plan ");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
