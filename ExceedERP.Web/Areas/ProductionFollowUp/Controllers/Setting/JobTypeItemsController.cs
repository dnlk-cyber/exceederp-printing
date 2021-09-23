﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ProductionFollowUp;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.ProductionFollowUp.Controllers.Setting
{
    public class JobTypeItemsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobTypeItems_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<JobTypeItem> jobtypeitems = db.JobTypeItems.Where(x=>x.JobTypeId == id);
            DataSourceResult result = jobtypeitems.ToDataSourceResult(request, jobTypeItem => new {
                JobTypeItemId = jobTypeItem.JobTypeItemId,
                JobTypeId = jobTypeItem.JobTypeId,
                Name = jobTypeItem.Name
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTypeItems_Create([DataSourceRequest]DataSourceRequest request, JobTypeItem jobTypeItem, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobTypeItem
                {
                    JobTypeId = id,
                    Name = jobTypeItem.Name
                };

                db.JobTypeItems.Add(entity);
                db.SaveChanges();
                jobTypeItem.JobTypeItemId = entity.JobTypeItemId;
            }

            return Json(new[] { jobTypeItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTypeItems_Update([DataSourceRequest]DataSourceRequest request, JobTypeItem jobTypeItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobTypeItem
                {
                    JobTypeItemId = jobTypeItem.JobTypeItemId,
                    JobTypeId = jobTypeItem.JobTypeId,
                    Name = jobTypeItem.Name
                };

                db.JobTypeItems.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobTypeItem }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobTypeItems_Destroy([DataSourceRequest]DataSourceRequest request, JobTypeItem jobTypeItem)
        {
            if (ModelState.IsValid)
            {
                var entity = new JobTypeItem
                {
                    JobTypeItemId = jobTypeItem.JobTypeItemId,
                    JobTypeId = jobTypeItem.JobTypeId,
                    Name = jobTypeItem.Name
                };

                db.JobTypeItems.Attach(entity);
                db.JobTypeItems.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobTypeItem }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
