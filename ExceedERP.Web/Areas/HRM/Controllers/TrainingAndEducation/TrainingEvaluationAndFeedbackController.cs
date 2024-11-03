using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{

    public class TrainingEvaluationAndFeedbackController : BaseController
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
            ViewData["PeriodList"] = db.GLFiscalYears.ToList();
            ViewData["EmployeeList"] = inputs.GetEmployees(User.Identity.Name);
            ViewData["CourseList"] = db.Lookups.ToList();
            ViewData["ProviderList"] = db.Companies.ToList();
            ViewData["Criterias"] = db.TrainingEvaluationCriterias.ToList();
        }
        public ActionResult CourseTrackings_Read([DataSourceRequest]DataSourceRequest request)
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

            var trainees = db.Trainee.Where(x => x.EmployeeId == empId).ToList().Select(x => x.CourseTrackingId);

            IQueryable<CourseTracking> coursetrackings = db.CourseTrackings.Where(x => trainees.Contains(x.CourseTrackingId));

            DataSourceResult result = coursetrackings.ToDataSourceResult(request, courseTracking => new
            {
                CourseTrackingId = courseTracking.CourseTrackingId,
                GlFiscalYearId = courseTracking.GlFiscalYearId,
                NoOfTrainee = courseTracking.NoOfTrainee,
                TrainingPlanId = courseTracking.TrainingPlanId,
                LookupId = courseTracking.LookupId,
                From = courseTracking.From,
                To = courseTracking.To,
                Cost = courseTracking.Cost,
                TrainingType = courseTracking.TrainingType,
                MainCompanyId = courseTracking.MainCompanyId,
                Remark = courseTracking.Remark,
                FromAmharic = courseTracking.FromAmharic,
                ToAmharic = courseTracking.ToAmharic,
                DateCreated = courseTracking.DateCreated,
                LastModified = courseTracking.LastModified,
                CreatedBy = courseTracking.CreatedBy,
                ModifiedBy = courseTracking.ModifiedBy,
                OrgBranchName = courseTracking.OrgBranchName,
                TrainingLocation = courseTracking.TrainingLocation,
                TrainingTerm = courseTracking.TrainingTerm,
                IsOnJob = courseTracking.IsOnJob,
            });

            return Json(result);
        }

        public ActionResult TrainingEvaluation_For_CourseTracking_Read([DataSourceRequest]DataSourceRequest request, int traineeId)
        {
            List<TrainingEvaluation> trainigEvaluations = new List<TrainingEvaluation>();

            var trainee = db.Trainee.FirstOrDefault(x=>x.TraineeId == traineeId);
            if(trainee != null)
            {
                trainigEvaluations = db.TrainingEvaluations.Where(x => x.CourseTrackingId == trainee.CourseTrackingId && x.EmployeeId == trainee.EmployeeId).ToList();
            }
          

            DataSourceResult result = trainigEvaluations.ToDataSourceResult(request, eval => new
            {
                TrainingEvaluationId = eval.TrainingEvaluationId,
                EmployeeId = eval.EmployeeId,
                CriteriaId = eval.CriteriaId,
                CourseTrackingId = eval.CourseTrackingId,
                Score = eval.Score,
            });

            return Json(result);
        }
        public ActionResult TrainingFeedback_For_CourseTracking_Read([DataSourceRequest]DataSourceRequest request, int traineeId)
        {
            List<TrainingFeedback> trainingFeedbacks = new List<TrainingFeedback>();

            var trainee = db.Trainee.FirstOrDefault(x => x.TraineeId == traineeId);
            if (trainee != null)
            {
                trainingFeedbacks = db.TrainingFeedbacks.Where(x => x.CourseTrackingId == trainee.CourseTrackingId && x.EmployeeId == trainee.EmployeeId).ToList();
            }

            DataSourceResult result = trainingFeedbacks.ToDataSourceResult(request, feed => new
            {
                TrainingFeedbackId = feed.TrainingFeedbackId,
                EmployeeId = feed.EmployeeId,
                CourseTrackingId = feed.CourseTrackingId,
                FeedBack = feed.FeedBack,
            });

            return Json(result);
        }

        public ActionResult TrainingFeedback_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var trainingEvalueations = db.TrainingFeedbacks.Where(x => x.CourseTrackingId == id).ToList();
            DataSourceResult result = trainingEvalueations.ToDataSourceResult(request, training => new
            {
                TrainingFeedbackId = training.TrainingFeedbackId,
                CourseTrackingId = training.CourseTrackingId,
                EmployeeId = training.EmployeeId,
                FeedBack = training.FeedBack,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingFeedback_Create([DataSourceRequest]DataSourceRequest request, TrainingFeedback training, int id)
        {
            ModelState.Clear();


            if (ModelState.IsValid)
            {
                training.EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                var entity = new TrainingFeedback
                {
                    CourseTrackingId = id,
                    EmployeeId = training.EmployeeId,
                    FeedBack = training.FeedBack,
                };

                db.TrainingFeedbacks.Add(entity);
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Education ", "Success", User.Identity.Name, User.Identity.Name + " add Education info for " + person.EmployeeCode, Modules.HRM);

                }
                #endregion

            }

            return Json(new[] { training }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingFeedback_Update([DataSourceRequest]DataSourceRequest request, TrainingFeedback training)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {

                var entity = new TrainingFeedback
                {
                    TrainingFeedbackId = training.TrainingFeedbackId,
                    CourseTrackingId = training.CourseTrackingId,
                    EmployeeId = training.EmployeeId,
                    FeedBack = training.FeedBack,
                };


                db.TrainingFeedbacks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                    if (name != null)
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Education ", "Success", User.Identity.Name, name.EmployeeCode + " update Education info of " + person.EmployeeCode, Modules.HRM);

                    }
                }
                #endregion

            }

            return Json(new[] { training }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingFeedback_Destroy([DataSourceRequest]DataSourceRequest request, TrainingFeedback training)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new TrainingFeedback
                {
                    TrainingFeedbackId = training.TrainingFeedbackId,
                    CourseTrackingId = training.CourseTrackingId,
                    EmployeeId = training.EmployeeId,
                    FeedBack = training.FeedBack,
                };

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted educational information of employee " + training.EmployeeId);
                #endregion

                db.TrainingFeedbacks.Attach(entity);
                db.TrainingFeedbacks.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { training }.ToDataSourceResult(request, ModelState));
        }
        /// <summary>
        /// ///////
        /// </summary>
        /// <param name="disposing"></param>
        /// 
        public ActionResult TrainingEvaluation_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var trainingEvalueations = db.TrainingEvaluations.Where(x => x.CourseTrackingId == id).ToList();
            DataSourceResult result = trainingEvalueations.ToDataSourceResult(request, training => new
            {
                TrainingEvaluationId = training.TrainingEvaluationId,
                CourseTrackingId = training.CourseTrackingId,
                EmployeeId = training.EmployeeId,
                CriteriaId = training.CriteriaId,
                Score = training.Score,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluation_Create([DataSourceRequest]DataSourceRequest request, TrainingEvaluation training, int id)
        {
            ModelState.Clear();

            var check = db.TrainingEvaluations.FirstOrDefault(x => x.CourseTrackingId == id && x.CriteriaId == training.CriteriaId);

            if (check != null)
            {
                ModelState.AddModelError("", "You set this criteria before");
            }

            if (ModelState.IsValid)
            {
                training.EmployeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                var entity = new TrainingEvaluation
                {
                    CourseTrackingId = id,
                    TrainingEvaluationId = training.TrainingEvaluationId,
                    EmployeeId = training.EmployeeId,
                    CriteriaId = training.CriteriaId,
                    Score = training.Score,
                };

                db.TrainingEvaluations.Add(entity);
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    UserHelper.OperationLog(Request.UserHostAddress, "Education ", "Success", User.Identity.Name, User.Identity.Name + " add Education info for " + person.EmployeeCode, Modules.HRM);

                }
                #endregion

            }

            return Json(new[] { training }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluation_Update([DataSourceRequest]DataSourceRequest request, TrainingEvaluation training)
        {
            ModelState.Clear();

            var check = db.TrainingEvaluations.FirstOrDefault(x => x.CourseTrackingId == training.CourseTrackingId && x.CriteriaId == training.CriteriaId && x.TrainingEvaluationId != training.TrainingEvaluationId);

            if (check != null)
            {
                ModelState.AddModelError("", "You set this criteria before");
            }

            if (ModelState.IsValid)
            {
                var entity = new TrainingEvaluation
                {
                    TrainingEvaluationId = training.TrainingEvaluationId,
                    CourseTrackingId = training.CourseTrackingId,
                    EmployeeId = training.EmployeeId,
                    CriteriaId = training.CriteriaId,
                    Score = training.Score,
                };


                db.TrainingEvaluations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var person = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == entity.EmployeeId);
                if (person != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
                    if (name != null)
                    {
                        UserHelper.OperationLog(Request.UserHostAddress, "Education ", "Success", User.Identity.Name, name.EmployeeCode + " update Education info of " + person.EmployeeCode, Modules.HRM);

                    }
                }
                #endregion

            }

            return Json(new[] { training }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluation_Destroy([DataSourceRequest]DataSourceRequest request, TrainingEvaluation training)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new TrainingEvaluation
                {
                    TrainingEvaluationId = training.TrainingEvaluationId,
                    CourseTrackingId = training.CourseTrackingId,
                    EmployeeId = training.EmployeeId,
                    CriteriaId = training.CriteriaId,
                    Score = training.Score,
                };

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted educational information of employee " + training.EmployeeId);
                #endregion

                db.TrainingEvaluations.Attach(entity);
                db.TrainingEvaluations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { training }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
