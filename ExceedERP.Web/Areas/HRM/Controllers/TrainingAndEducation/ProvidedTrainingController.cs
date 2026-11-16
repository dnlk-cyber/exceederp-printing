using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.HRM.TrainingEducation;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{

    [AuthorizeRoles(HrmConstants.HRTrainingUser, HrmConstants.HRTrainingApprover)]
    public class ProvidedTrainingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: Common/ProvidedTraining/ProvidedTrainings_Read
        public ActionResult ProvidedTrainings_Read([DataSourceRequest] DataSourceRequest request,int mainCompanyId)
        {
            var trainings = db.ProvidedTrainings
                .Where(x => x.MainCompanyId == mainCompanyId)
                .Select(x => new
                {
                    x.ProvidedTrainingId,
                    x.TrainingName,
                    x.Description,
                    x.Duration,
                    x.DurationUnit,
                    x.IsActive,
                    x.MainCompanyId
                });

            return Json(
                trainings.ToDataSourceResult(request),
                JsonRequestBehavior.AllowGet);
        }

        // POST: Common/ProvidedTraining/ProvidedTrainings_Create
        [HttpPost]
        public ActionResult ProvidedTrainings_Create([DataSourceRequest] DataSourceRequest request, ProvidedTraining providedTraining,int mainCompanyId)
        {
            if (providedTraining != null && ModelState.IsValid)
            {
                providedTraining.MainCompanyId = mainCompanyId;

                db.ProvidedTrainings.Add(providedTraining);
                db.SaveChanges();
            }

            return Json(new[] { providedTraining }.ToDataSourceResult(request, ModelState));
        }

        // POST: Common/ProvidedTraining/ProvidedTrainings_Update
        [HttpPost]
        public ActionResult ProvidedTrainings_Update( [DataSourceRequest] DataSourceRequest request,ProvidedTraining model)
        {
            if (model != null && ModelState.IsValid)
            {
                var entity = db.ProvidedTrainings.FirstOrDefault(x =>x.ProvidedTrainingId == model.ProvidedTrainingId);

                if (entity != null)
                {
                    entity.TrainingName = model.TrainingName;
                    entity.Description = model.Description;
                    entity.Duration = model.Duration;
                    entity.DurationUnit = model.DurationUnit;
                    entity.IsActive = model.IsActive;

                    db.SaveChanges();
                }
            }

            return Json(
                new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // POST: Common/ProvidedTraining/ProvidedTrainings_Destroy
        [HttpPost]
        public ActionResult ProvidedTrainings_Destroy( [DataSourceRequest] DataSourceRequest request, ProvidedTraining model)
        {
            if (model != null)
            {
                var entity = db.ProvidedTrainings .FirstOrDefault(x => x.ProvidedTrainingId == model.ProvidedTrainingId);

                if (entity != null)
                {
                    db.ProvidedTrainings.Remove(entity);
                    db.SaveChanges();
                }
            }

            return Json(
                new[] { model }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
