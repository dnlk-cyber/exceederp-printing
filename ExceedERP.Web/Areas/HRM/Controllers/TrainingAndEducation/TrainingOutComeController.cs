using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    //[Authorize(Roles = HR_Training_User + "," + HR_Admin)]
    [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRAdmin)]

    public class TrainingOutComeController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRAdmin)]
        public ActionResult TrainingStatuses_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<TrainingStatus> trainingstatuses = db.TrainingStatuses.Where(x => x.TraineeId == id);
            DataSourceResult result = trainingstatuses.ToDataSourceResult(request, trainingStatus => new
            {
                TrainingStatusId = trainingStatus.TrainingStatusId,
                TrainingOutCome = trainingStatus.TrainingOutCome,
                TraineeId = trainingStatus.TraineeId,
                Remark = trainingStatus.Remark
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingStatuses_Create([DataSourceRequest]DataSourceRequest request, TrainingStatus trainingStatus, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new TrainingStatus
                {
                    TrainingOutCome = trainingStatus.TrainingOutCome,
                    Trainee = trainingStatus.Trainee,
                    TraineeId = id,
                    Remark = trainingStatus.Remark
                };

                db.TrainingStatuses.Add(entity);
                db.SaveChanges();
                trainingStatus.TrainingStatusId = entity.TrainingStatusId;
            }

            return Json(new[] { trainingStatus }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRTrainingUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingStatuses_Update([DataSourceRequest]DataSourceRequest request, TrainingStatus trainingStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new TrainingStatus
                {
                    TrainingStatusId = trainingStatus.TrainingStatusId,
                    TraineeId = trainingStatus.TraineeId,
                    TrainingOutCome = trainingStatus.TrainingOutCome,
                    Remark = trainingStatus.Remark
                };

                db.TrainingStatuses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { trainingStatus }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingStatuses_Destroy([DataSourceRequest]DataSourceRequest request, TrainingStatus trainingStatus)
        {
            if (ModelState.IsValid)
            {
                var entity = new TrainingStatus
                {
                    TrainingStatusId = trainingStatus.TrainingStatusId,
                    TrainingOutCome = trainingStatus.TrainingOutCome,
                    TraineeId = trainingStatus.TraineeId,
                    Remark = trainingStatus.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted training outcome information of trainee id" + trainingStatus.TraineeId);
                #endregion
                db.TrainingStatuses.Attach(entity);
                db.TrainingStatuses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { trainingStatus }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
