﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Manufacturing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.ProductionFollowUp.Setting


{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class FurnitureJobCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Parents"] = db.FurnitureJobCategories.Where(q=>q.HaveParent== false).ToList();
            ViewData["Taxes"] = db.GLTaxRates.Select(s=>new {
            Code = s.GLTaxRateID,
            Name = s.TaxName}).ToList();
            return View();
        }

        public JsonResult GetJobTypes()
        {
            var data = db.FurnitureJobCategories;
            return Json(data.Select(s => new { s.FurnitureJobCategoryId, s.JobName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult JobCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureJobCategory> jobcategories = db.FurnitureJobCategories;
            DataSourceResult result = jobcategories.ToDataSourceResult(request, jobCategory => new {
                JobCategoryId = jobCategory.FurnitureJobCategoryId,
                JobName = jobCategory.JobName,
                JobCode = jobCategory.JobCode,
                HaveParent = jobCategory.HaveParent,
                ParentId = jobCategory.FurnitureParentId,
                AccountNumber = jobCategory.AccountNumber,
                GLTaxRateID = jobCategory.GLTaxRateID,
                Description = jobCategory.Description,
                DateCreated = jobCategory.DateCreated,
                LastModified = jobCategory.LastModified,
                CreatedBy = jobCategory.CreatedBy,
                ModifiedBy = jobCategory.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult JobCategories_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobCategory jobCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobCategory)this.GetObject(jobCategory);
                db.FurnitureJobCategories.Add(entity);
                db.SaveChanges();
                jobCategory.FurnitureJobCategoryId = entity.FurnitureJobCategoryId;
            }

            return Json(new[] { jobCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobCategories_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobCategory jobCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobCategory)this.GetObject(jobCategory);

                db.FurnitureJobCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobCategories_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobCategory jobCategory)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobCategory)this.GetObject(jobCategory);

                db.FurnitureJobCategories.Attach(entity);
                db.FurnitureJobCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobCategory }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureJobCategory jobCategory)
        {
            var entity = new FurnitureJobCategory
            {
                FurnitureJobCategoryId = jobCategory.FurnitureJobCategoryId,
                JobName = jobCategory.JobName,
                JobCode = jobCategory.JobCode,
                HaveParent = jobCategory.HaveParent,
                FurnitureParentId = jobCategory.FurnitureParentId,
                GLTaxRateID = jobCategory.GLTaxRateID,
                AccountNumber = jobCategory.AccountNumber,
                Description = jobCategory.Description,
                DateCreated = jobCategory.DateCreated,
                LastModified = jobCategory.LastModified,
                CreatedBy = jobCategory.CreatedBy,
                ModifiedBy = jobCategory.ModifiedBy

            };
            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
