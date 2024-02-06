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
using ExceedERP.Core.Domain.Printing.JobCosting;
using ExceedERP.Core.Domain.printing.JobCosting;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting

{
    // [AuthorizeRoles(JobCostingRoles.JobCostingIdleTime)]
    public class IdleTimeNonProductiveNonControllableController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeNonProductiveNonControllables_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PrintingIdleTimeNonProductiveNonControllable> idletimenonproductivenoncontrollables = db.PrintingIdleTimeNonProductiveNonControllables;
            DataSourceResult result = idletimenonproductivenoncontrollables.ToDataSourceResult(request, idleTimeNonProductiveNonControllable => new {
                IdleTimeNonProductiveNonControllableID = idleTimeNonProductiveNonControllable.PrintingIdleTimeNonProductiveNonControllableID,
                IdleTimeID = idleTimeNonProductiveNonControllable.IdleTimeID,
                IdleTimeCategoryId = idleTimeNonProductiveNonControllable.IdleTimeCategoryId,
                CostCenterId = idleTimeNonProductiveNonControllable.CostCenterId,
                Rate = idleTimeNonProductiveNonControllable.Rate,
                Value = idleTimeNonProductiveNonControllable.Value
            });

            return Json(result);
        }

        public ActionResult IdleTimeNonProductiveNonControllables_Create([DataSourceRequest]DataSourceRequest request, PrintingIdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (PrintingIdleTimeNonProductiveNonControllable)this.GetObject(idleTimeNonProductiveNonControllable, true, id);
                db.PrintingIdleTimeNonProductiveNonControllables.Add(entity);
                db.SaveChanges();
                idleTimeNonProductiveNonControllable.PrintingIdleTimeNonProductiveNonControllableID = entity.PrintingIdleTimeNonProductiveNonControllableID;
            }

            return Json(new[] { idleTimeNonProductiveNonControllable }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeNonProductiveNonControllables_Update([DataSourceRequest]DataSourceRequest request, PrintingIdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable)
        {
            if (ModelState.IsValid)
            {
                var entity = (PrintingIdleTimeNonProductiveNonControllable)this.GetObject(idleTimeNonProductiveNonControllable, false);

                db.PrintingIdleTimeNonProductiveNonControllables.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeNonProductiveNonControllable }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeNonProductiveNonControllables_Destroy([DataSourceRequest]DataSourceRequest request, PrintingIdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable)
        {

            if (ModelState.IsValid)
            {
                var entity = (PrintingIdleTimeNonProductiveNonControllable)this.GetObject(idleTimeNonProductiveNonControllable, false);

                db.PrintingIdleTimeNonProductiveNonControllables.Attach(entity);
                db.PrintingIdleTimeNonProductiveNonControllables.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeNonProductiveNonControllable }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(PrintingIdleTimeNonProductiveNonControllable idleTimeNonProductiveNonControllable, bool status, int id = 0)
        {
            if (status)
            {
                idleTimeNonProductiveNonControllable.IdleTimeID = id;
            }
            var entity = new PrintingIdleTimeNonProductiveNonControllable
            {
                PrintingIdleTimeNonProductiveNonControllableID = idleTimeNonProductiveNonControllable.PrintingIdleTimeNonProductiveNonControllableID,
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
