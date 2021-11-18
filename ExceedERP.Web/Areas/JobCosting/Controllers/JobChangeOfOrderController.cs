﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.JobCosting;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.JobCosting.Controllers
{
    public class JobChangeOfOrderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobChangeOfOrders_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            IQueryable<JobChangeOfOrder> jobchangeoforders = db.JobChangeOfOrders.Where(q=>q.JobCostId==id);
            DataSourceResult result = jobchangeoforders.ToDataSourceResult(request, jobChangeOfOrder => new {
                JobChangeOfOrderId = jobChangeOfOrder.JobChangeOfOrderId,
                JobCostId = jobChangeOfOrder.JobCostId,
                ChangeOrderId = jobChangeOfOrder.ChangeOrderId,
                AdditionalChange = jobChangeOfOrder.AdditionalChange,
                ChargeAmount = jobChangeOfOrder.ChargeAmount,
                Reason = jobChangeOfOrder.Reason,
                DateCreated = jobChangeOfOrder.DateCreated,
                LastModified = jobChangeOfOrder.LastModified,
                CreatedBy = jobChangeOfOrder.CreatedBy,
                ModifiedBy = jobChangeOfOrder.ModifiedBy
            });

            return Json(result);
        }
        public ActionResult JobChangeOfOrders_Create([DataSourceRequest]DataSourceRequest request, JobChangeOfOrder jobChangeOfOrder, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobChangeOfOrder)this.GetObject(jobChangeOfOrder, true, id);
                db.JobChangeOfOrders.Add(entity);
                db.SaveChanges();
                jobChangeOfOrder.JobChangeOfOrderId = entity.JobChangeOfOrderId;
            }

            return Json(new[] { jobChangeOfOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobChangeOfOrders_Update([DataSourceRequest]DataSourceRequest request, JobChangeOfOrder jobChangeOfOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = (JobChangeOfOrder)this.GetObject(jobChangeOfOrder, false);

                db.JobChangeOfOrders.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobChangeOfOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobChangeOfOrders_Destroy([DataSourceRequest]DataSourceRequest request, JobChangeOfOrder jobChangeOfOrder)
        {

            if (ModelState.IsValid)
            {
                var entity = (JobChangeOfOrder)this.GetObject(jobChangeOfOrder, false);

                db.JobChangeOfOrders.Attach(entity);
                db.JobChangeOfOrders.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobChangeOfOrder }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(JobChangeOfOrder jobChangeOfOrder, bool status, int id = 0)
        {
            if (status)
            {
                jobChangeOfOrder.JobCostId = id;
            }
            var entity = new JobChangeOfOrder
            {
                JobChangeOfOrderId = jobChangeOfOrder.JobChangeOfOrderId,
                JobCostId = jobChangeOfOrder.JobCostId,
                ChangeOrderId = jobChangeOfOrder.ChangeOrderId,
                AdditionalChange = jobChangeOfOrder.AdditionalChange,
                ChargeAmount = jobChangeOfOrder.ChargeAmount,
                Reason = jobChangeOfOrder.Reason,
                DateCreated = jobChangeOfOrder.DateCreated,
                LastModified = jobChangeOfOrder.LastModified,
                CreatedBy = jobChangeOfOrder.CreatedBy,
                ModifiedBy = jobChangeOfOrder.ModifiedBy

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
