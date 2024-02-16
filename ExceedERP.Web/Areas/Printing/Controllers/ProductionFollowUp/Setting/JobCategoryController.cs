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
using ExceedERP.Core.Domain.Printing.ProductionFollowUp;

namespace ExceedERP.Web.Areas.Printing.Controllers.ProductionFollowUp.Setting

{
    //[AuthorizeRoles(ProductionFollowUpRoles.ProductionFollowUpAdmin)]
    public class JobCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Parents"] = db.JobCategories.Where(q=>q.HaveParent== false).ToList();

            ViewData["Taxes"] = db.GLTaxRates.Select(s => new
            {
                Code = s.GLTaxRateID,
                Name = s.TaxName
            }).ToList();

            return View();
        }

        public JsonResult GetJobTypes()
        {
            var data = db.JobCategories;
            return Json(data.Select(s => new { s.JobCategoryId, s.JobName }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult JobCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<JobCategory> jobcategories = db.JobCategories;
            DataSourceResult result = jobcategories.ToDataSourceResult(request, jobCategory => new {
                JobCategoryId = jobCategory.JobCategoryId,
                JobName = jobCategory.JobName,
                JobCode = jobCategory.JobCode,
                HaveParent = jobCategory.HaveParent,
                ParentId = jobCategory.ParentId,
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

        public ActionResult JobCategories_Create([DataSourceRequest]DataSourceRequest request, JobCategory jobCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = (JobCategory)this.GetObject(jobCategory);
                db.JobCategories.Add(entity);
                db.SaveChanges();
                jobCategory.JobCategoryId = entity.JobCategoryId;
            }

            return Json(new[] { jobCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobCategories_Update([DataSourceRequest]DataSourceRequest request, JobCategory jobCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = (JobCategory)this.GetObject(jobCategory);

                db.JobCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobCategories_Destroy([DataSourceRequest]DataSourceRequest request, JobCategory jobCategory)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobCategory)this.GetObject(jobCategory);

                db.JobCategories.Attach(entity);
                db.JobCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobCategory }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(JobCategory jobCategory)
        {
            var entity = new JobCategory
            {
                JobCategoryId = jobCategory.JobCategoryId,
                JobName = jobCategory.JobName,
                JobCode = jobCategory.JobCode,
                HaveParent = jobCategory.HaveParent,
                ParentId = jobCategory.ParentId,
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
