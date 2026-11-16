﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    public class TrainingEvaluationCriteriaController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TrainingEvaluationCriterias_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<TrainingEvaluationCriteria> trainingevaluationcriterias = db.TrainingEvaluationCriterias
                .Where(x => x.TrainingEvaluationSettingCategoryId == id && !x.TrainingCourseId.HasValue);
            DataSourceResult result = trainingevaluationcriterias.ToDataSourceResult(request, trainingEvaluationCriteria => new {
                TrainingEvaluationCriteriaId = trainingEvaluationCriteria.TrainingEvaluationCriteriaId,
                TrainingEvaluationSettingCategoryId = trainingEvaluationCriteria.TrainingEvaluationSettingCategoryId,
                Criteria = trainingEvaluationCriteria.Criteria,
                Weight = trainingEvaluationCriteria.Weight,
                Result = trainingEvaluationCriteria.Result,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationCriterias_Create([DataSourceRequest]DataSourceRequest request, TrainingEvaluationCriteria trainingEvaluationCriteria, int id)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new TrainingEvaluationCriteria
                {
                    TrainingEvaluationSettingCategoryId = id,
                    Criteria = trainingEvaluationCriteria.Criteria,
                    Weight = trainingEvaluationCriteria.Weight,
                    Result = trainingEvaluationCriteria.Result,
                };

                db.TrainingEvaluationCriterias.Add(entity);
                db.SaveChanges();
                trainingEvaluationCriteria.TrainingEvaluationCriteriaId = entity.TrainingEvaluationCriteriaId;
            }

            return Json(new[] { trainingEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationCriterias_Update([DataSourceRequest]DataSourceRequest request, TrainingEvaluationCriteria trainingEvaluationCriteria)
        {
            var entity = db.TrainingEvaluationCriterias.SingleOrDefault(x =>
                x.TrainingEvaluationCriteriaId == trainingEvaluationCriteria.TrainingEvaluationCriteriaId && !x.TrainingCourseId.HasValue);
            if (entity == null) ModelState.AddModelError("", "Select a general evaluation criterion.");
            if (ModelState.IsValid)
            {
                entity.TrainingEvaluationSettingCategoryId = trainingEvaluationCriteria.TrainingEvaluationSettingCategoryId;
                entity.Criteria = trainingEvaluationCriteria.Criteria;
                entity.Weight = trainingEvaluationCriteria.Weight;
                entity.Result = trainingEvaluationCriteria.Result;
                db.SaveChanges();
            }

            return Json(new[] { trainingEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationCriterias_Destroy([DataSourceRequest]DataSourceRequest request, TrainingEvaluationCriteria trainingEvaluationCriteria)
        {
            var entity = db.TrainingEvaluationCriterias.SingleOrDefault(x =>
                x.TrainingEvaluationCriteriaId == trainingEvaluationCriteria.TrainingEvaluationCriteriaId && !x.TrainingCourseId.HasValue);
            if (entity == null) ModelState.AddModelError("", "Select a general evaluation criterion.");
            if (ModelState.IsValid)
            {
                db.TrainingEvaluationCriterias.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { trainingEvaluationCriteria }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
