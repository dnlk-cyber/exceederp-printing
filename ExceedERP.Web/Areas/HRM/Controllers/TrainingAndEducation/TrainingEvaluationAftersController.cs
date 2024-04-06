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
    public class TrainingEvaluationAftersController : BaseController
    {
        // GET: HRM/TrainingAssessment
        private ExceedDbContext db = new ExceedDbContext();

      
      
        public ActionResult TrainingEvaluationAfters_Read([DataSourceRequest]DataSourceRequest request,int reqId)
        {
            IQueryable<TrainingEvaluationAfter> trainingNeedRequests = db.TrainingEvaluationAfters.Where(x=>x.TrainingNeedRequestId == reqId);//.Where(x => mybraches.Contains(x.OrgBranchName)&&x.IsTransfered==false);
            DataSourceResult result = trainingNeedRequests.ToDataSourceResult(request, trainingNeedRequest => new
            {
                TrainingEvaluationAfterId = trainingNeedRequest.TrainingEvaluationAfterId,
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
        public ActionResult TrainingEvaluationAfters_Create([DataSourceRequest]DataSourceRequest request, TrainingEvaluationAfter trainingNeedRequest, int reqId)
        {
            ModelState.Clear();
           
            if (ModelState.IsValid)
            {
                trainingNeedRequest.TrainingNeedRequestId = reqId;
                var entity = GetMappedObject(trainingNeedRequest);
                db.TrainingEvaluationAfters.Add(entity);
                db.SaveChanges();
                trainingNeedRequest.TrainingNeedRequestId = entity.TrainingNeedRequestId;
            }

            return Json(new[] { trainingNeedRequest }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationAfters_Update([DataSourceRequest]DataSourceRequest request, TrainingEvaluationAfter trainingNeedRequest)
        {
            ModelState.Clear();
        
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(trainingNeedRequest);
                db.TrainingEvaluationAfters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { trainingNeedRequest }.ToDataSourceResult(request, ModelState));
        }
    
        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationAfters_Destroy([DataSourceRequest]DataSourceRequest request, TrainingEvaluationAfter trainingNeedRequest)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(trainingNeedRequest);
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted need assessment information of course " + trainingNeedRequest.TrainingNeedRequestId);
                #endregion
                db.TrainingEvaluationAfters.Attach(entity);
                db.TrainingEvaluationAfters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { trainingNeedRequest }.ToDataSourceResult(request, ModelState));
        }
        private TrainingEvaluationAfter GetMappedObject(TrainingEvaluationAfter trainingNeedRequest)
        {
            var entity = new TrainingEvaluationAfter
            {
                TrainingEvaluationAfterId = trainingNeedRequest.TrainingEvaluationAfterId,
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
        public JsonResult GenerateEvaluationAfter(int reqId)
        {
            //List<TrainingEvaluationBefore> trainingEvaluationBefores = new List<TrainingEvaluationBefore>();
            var categories = db.TrainingEvaluationCriteriaCategories.ToList();
            int count = 0;
            foreach (var category in categories)
            {
                var criterias = db.TrainingEvaluationCriterias.Where(x => x.TrainingEvaluationSettingCategoryId == category.TrainingEvaluationSettingCategoryId).ToList();
                foreach (var criteria in criterias)
                {
                    var exist = db.TrainingEvaluationAfters.Where(x => x.TrainingEvaluationCriteriaId == criteria.TrainingEvaluationCriteriaId
                                                                        && x.TrainingEvaluationSettingCategoryId == criteria.TrainingEvaluationSettingCategoryId
                                                                        && x.TrainingNeedRequestId == reqId).Any();
                    if (!exist)
                    {



                        var entity = new TrainingEvaluationAfter
                        {
                            TrainingNeedRequestId = reqId,
                            TrainingEvaluationSettingCategoryId = criteria.TrainingEvaluationSettingCategoryId,
                            Name = category.Name,
                            TrainingEvaluationCriteriaId = criteria.TrainingEvaluationCriteriaId,
                            Criteria = criteria.Criteria,
                            Weight = criteria.Weight,
                            Result = criteria.Result

                        };
                        db.TrainingEvaluationAfters.Add(entity);
                        db.SaveChanges();
                        entity.TrainingEvaluationAfterId = entity.TrainingEvaluationAfterId;
                        count++;
                    }
                }
            }
            if (count > 0)
            {
                var formattedData = new
                {
                    Message = "It has been successfully generated.",
                    Status = "Okay"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            var formattedData1 = new
            {
                Message = "no new record found.",
                Status = "not-okay"
            };
            return Json(formattedData1, JsonRequestBehavior.AllowGet);

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
