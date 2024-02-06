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
using ExceedERP.Core.Domain.Manufacturing.JobCosting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting

{
    //[AuthorizeRoles(JobCostingRoles.JobCostingJobCostUser)]
    public class FurnitureJobChangeOfOrderController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult JobChangeOfOrders_Read([DataSourceRequest]DataSourceRequest request,int id)
        {
            var jobCost = db.JobCosts.Find(id);
            if(jobCost!=null)
            {

            
            var jobchangeoforders = db.ChangeOrders.Where(q=>q.JobId==jobCost.JobId).ToList();
                var jobChanges = new List<FurnitureJobChangeOfOrder>();
                
                foreach(var joborder in jobchangeoforders)
                {
                    var jobChange = new FurnitureJobChangeOfOrder();

                    jobChange.FurnitureChangeOrderId = joborder.ChangeOrderId;
                jobChange.AdditionalChange = joborder.Quantity;
                    jobChange.ChargeAmount = joborder.Total;
                    jobChange.Reason = joborder.Reason;
                    jobChange.DateCreated = joborder.DateCreated;
                    jobChange.LastModified = joborder.LastModified;
                    jobChange.CreatedBy = joborder.CreatedBy;
                    jobChange.ModifiedBy = joborder.ModifiedBy;
                    jobChanges.Add(jobChange);
                }
            DataSourceResult result = jobChanges.ToDataSourceResult(request, jobChangeOfOrder => new FurnitureJobChangeOfOrder
            { 
                FurnitureJobChangeOfOrderId = jobChangeOfOrder.FurnitureChangeOrderId,
                FurnitureJobCostId = id,
                FurnitureChangeOrderId = jobChangeOfOrder.FurnitureChangeOrderId,
                AdditionalChange = jobChangeOfOrder.AdditionalChange,
                ChargeAmount = jobChangeOfOrder.ChargeAmount,
                Reason = jobChangeOfOrder.Reason,
                DateCreated = jobChangeOfOrder.DateCreated,
                LastModified = jobChangeOfOrder.LastModified,
                CreatedBy = jobChangeOfOrder.CreatedBy,
                ModifiedBy = jobChangeOfOrder.ModifiedBy
            });

            return Json(result,JsonRequestBehavior.AllowGet);
            }
            return Json(request);

        }
        public ActionResult JobChangeOfOrders_Create([DataSourceRequest]DataSourceRequest request, FurnitureJobChangeOfOrder jobChangeOfOrder, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobChangeOfOrder)this.GetObject(jobChangeOfOrder, true, id);
                db.FurnitureJobChangeOfOrders.Add(entity);
                db.SaveChanges();
                jobChangeOfOrder.FurnitureJobChangeOfOrderId = entity.FurnitureJobChangeOfOrderId;
            }

            return Json(new[] { jobChangeOfOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobChangeOfOrders_Update([DataSourceRequest]DataSourceRequest request, FurnitureJobChangeOfOrder jobChangeOfOrder)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobChangeOfOrder)this.GetObject(jobChangeOfOrder, false);

                db.FurnitureJobChangeOfOrders.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { jobChangeOfOrder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JobChangeOfOrders_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureJobChangeOfOrder jobChangeOfOrder)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureJobChangeOfOrder)this.GetObject(jobChangeOfOrder, false);

                db.FurnitureJobChangeOfOrders.Attach(entity);
                db.FurnitureJobChangeOfOrders.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { jobChangeOfOrder }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureJobChangeOfOrder jobChangeOfOrder, bool status, int id = 0)
        {
            if (status)
            {
                jobChangeOfOrder.FurnitureJobCostId = id;
            }
            var entity = new FurnitureJobChangeOfOrder
            {
                FurnitureJobChangeOfOrderId = jobChangeOfOrder.FurnitureJobChangeOfOrderId,
                FurnitureJobCostId = jobChangeOfOrder.FurnitureJobCostId,
                FurnitureChangeOrderId = jobChangeOfOrder.FurnitureChangeOrderId,
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
