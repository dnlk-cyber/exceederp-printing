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
using ExceedERP.Core.Domain.Manufacturing.JobCosting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting

{
    // [AuthorizeRoles(JobCostingRoles.JobCostingIdleTime)]
    public class FurnitureIdleTimeNonProductiveNonControllableController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeNonProductiveNonControllables_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureIdleTimeNonProductiveNonControllable> idletimenonproductivenoncontrollables = db.FurnitureIdleTimeNonProductiveNonControllables;
            DataSourceResult result = idletimenonproductivenoncontrollables.ToDataSourceResult(request, idleTimeNonProductiveNonControllable => new {
                IdleTimeNonProductiveNonControllableID = idleTimeNonProductiveNonControllable.FurnitureIdleTimeNonProductiveNonControllableID,
                IdleTimeID = idleTimeNonProductiveNonControllable.IdleTimeID,
                IdleTimeCategoryId = idleTimeNonProductiveNonControllable.IdleTimeCategoryId,
                CostCenterId = idleTimeNonProductiveNonControllable.CostCenterId,
                Rate = idleTimeNonProductiveNonControllable.Rate,
                Value = idleTimeNonProductiveNonControllable.Value
            });

            return Json(result);
        }

        public ActionResult IdleTimeNonProductiveNonControllables_Create([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeNonProductiveNonControllable)this.GetObject(idleTimeNonProductiveNonControllable, true, id);
                db.FurnitureIdleTimeNonProductiveNonControllables.Add(entity);
                db.SaveChanges();
                idleTimeNonProductiveNonControllable.FurnitureIdleTimeNonProductiveNonControllableID = entity.FurnitureIdleTimeNonProductiveNonControllableID;
            }

            return Json(new[] { idleTimeNonProductiveNonControllable }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeNonProductiveNonControllables_Update([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeNonProductiveNonControllable)this.GetObject(idleTimeNonProductiveNonControllable, false);

                db.FurnitureIdleTimeNonProductiveNonControllables.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeNonProductiveNonControllable }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeNonProductiveNonControllables_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeNonProductiveNonControllable)this.GetObject(idleTimeNonProductiveNonControllable, false);

                db.FurnitureIdleTimeNonProductiveNonControllables.Attach(entity);
                db.FurnitureIdleTimeNonProductiveNonControllables.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeNonProductiveNonControllable }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureIdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable, bool status, int id = 0)
        {
            if (status)
            {
                idleTimeNonProductiveNonControllable.IdleTimeID = id;
            }
            var entity = new FurnitureIdleTimeNonProductiveNonControllable
            {
                FurnitureIdleTimeNonProductiveNonControllableID = idleTimeNonProductiveNonControllable.FurnitureIdleTimeNonProductiveNonControllableID,
                IdleTimeID = idleTimeNonProductiveNonControllable.IdleTimeID,
                IdleTimeCategoryId = idleTimeNonProductiveNonControllable.IdleTimeCategoryId,
                CostCenterId = idleTimeNonProductiveNonControllable.CostCenterId,
                Rate = idleTimeNonProductiveNonControllable.Rate,
                Value = idleTimeNonProductiveNonControllable.Value

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
