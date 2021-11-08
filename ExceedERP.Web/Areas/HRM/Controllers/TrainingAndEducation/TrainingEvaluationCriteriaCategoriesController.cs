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
    public class TrainingEvaluationCriteriaCategoriesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult TrainingEvaluationCriteriaCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<TrainingEvaluationCriteriaCategory> trainingevaluationcriteriacategories = db.TrainingEvaluationCriteriaCategories;
            DataSourceResult result = trainingevaluationcriteriacategories.ToDataSourceResult(request, trainingEvaluationCriteriaCategory => new {
                TrainingEvaluationSettingCategoryId = trainingEvaluationCriteriaCategory.TrainingEvaluationSettingCategoryId,
                Name = trainingEvaluationCriteriaCategory.Name
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationCriteriaCategories_Create([DataSourceRequest]DataSourceRequest request, TrainingEvaluationCriteriaCategory trainingEvaluationCriteriaCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new TrainingEvaluationCriteriaCategory
                {
                    Name = trainingEvaluationCriteriaCategory.Name
                };

                db.TrainingEvaluationCriteriaCategories.Add(entity);
                db.SaveChanges();
                trainingEvaluationCriteriaCategory.TrainingEvaluationSettingCategoryId = entity.TrainingEvaluationSettingCategoryId;
            }

            return Json(new[] { trainingEvaluationCriteriaCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationCriteriaCategories_Update([DataSourceRequest]DataSourceRequest request, TrainingEvaluationCriteriaCategory trainingEvaluationCriteriaCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new TrainingEvaluationCriteriaCategory
                {
                    TrainingEvaluationSettingCategoryId = trainingEvaluationCriteriaCategory.TrainingEvaluationSettingCategoryId,
                    Name = trainingEvaluationCriteriaCategory.Name
                };

                db.TrainingEvaluationCriteriaCategories.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { trainingEvaluationCriteriaCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult TrainingEvaluationCriteriaCategories_Destroy([DataSourceRequest]DataSourceRequest request, TrainingEvaluationCriteriaCategory trainingEvaluationCriteriaCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new TrainingEvaluationCriteriaCategory
                {
                    TrainingEvaluationSettingCategoryId = trainingEvaluationCriteriaCategory.TrainingEvaluationSettingCategoryId,
                    Name = trainingEvaluationCriteriaCategory.Name
                };

                db.TrainingEvaluationCriteriaCategories.Attach(entity);
                db.TrainingEvaluationCriteriaCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { trainingEvaluationCriteriaCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
