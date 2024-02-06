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
    // [AuthorizeRoles(JobCostingRoles.JobCostingIdleTime)]
    public class FurnitureIdleTimeNonProductiveControllableController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeNonProductiveControllables_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureIdleTimeNonProductiveControllable> idletimenonproductivecontrollables = db.FurnitureIdleTimeNonProductiveControllables;
            DataSourceResult result = idletimenonproductivecontrollables.ToDataSourceResult(request, idleTimeNonProductiveControllable => new {
                FurnitureIdleTimeNonProductiveControllableID = idleTimeNonProductiveControllable.FurnitureIdleTimeNonProductiveControllableID,
                IdleTimeID = idleTimeNonProductiveControllable.IdleTimeID,
                IdleTimeCategoryId = idleTimeNonProductiveControllable.IdleTimeCategoryId,
                CostCenterId = idleTimeNonProductiveControllable.CostCenterId,
                Rate = idleTimeNonProductiveControllable.Rate,
                Value = idleTimeNonProductiveControllable.Value
                
            });

            return Json(result);
        }

        public ActionResult IdleTimeNonProductiveControllables_Create([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeNonProductiveControllable idleTimeNonProductiveControllable, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeNonProductiveControllable)this.GetObject(idleTimeNonProductiveControllable, true, id);
                db.FurnitureIdleTimeNonProductiveControllables.Add(entity);
                db.SaveChanges();
                idleTimeNonProductiveControllable.FurnitureIdleTimeNonProductiveControllableID = entity.FurnitureIdleTimeNonProductiveControllableID;
            }

            return Json(new[] { idleTimeNonProductiveControllable }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeNonProductiveControllables_Update([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeNonProductiveControllable idleTimeNonProductiveControllable)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeNonProductiveControllable)this.GetObject(idleTimeNonProductiveControllable, false);

                db.FurnitureIdleTimeNonProductiveControllables.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeNonProductiveControllable }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeNonProductiveControllables_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeNonProductiveControllable idleTimeNonProductiveControllable)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeNonProductiveControllable)this.GetObject(idleTimeNonProductiveControllable, false);

                db.FurnitureIdleTimeNonProductiveControllables.Attach(entity);
                db.FurnitureIdleTimeNonProductiveControllables.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeNonProductiveControllable }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureIdleTimeNonProductiveControllable idleTimeNonProductiveControllable, bool status, int id = 0)
        {
            if (status)
            {
                idleTimeNonProductiveControllable.IdleTimeID = id;
            }
            var entity = new FurnitureIdleTimeNonProductiveControllable
            {
                FurnitureIdleTimeNonProductiveControllableID = idleTimeNonProductiveControllable.FurnitureIdleTimeNonProductiveControllableID,
                IdleTimeID = idleTimeNonProductiveControllable.IdleTimeID,
                IdleTimeCategoryId = idleTimeNonProductiveControllable.IdleTimeCategoryId,
                CostCenterId = idleTimeNonProductiveControllable.CostCenterId,
                Rate = idleTimeNonProductiveControllable.Rate,
                Value = idleTimeNonProductiveControllable.Value

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
