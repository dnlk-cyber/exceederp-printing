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
    public class FurnitureIdleTimeOTController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeOTs_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureIdleTimeOT> idletimeots = db.FurnitureIdleTimeOTs.Where(q=>q.IdleTimeID == id);
            DataSourceResult result = idletimeots.ToDataSourceResult(request, idleTimeOT => new {
                FurnitureIdleTimeOTID = idleTimeOT.FurnitureIdleTimeOTID,
                IdleTimeID = idleTimeOT.IdleTimeID,
                CostCenterId = idleTimeOT.CostCenterId,
                Rate = idleTimeOT.Rate,
                Value = idleTimeOT.Value
            });

            return Json(result);
        }

        public ActionResult IdleTimeOTs_Create([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeOT idleTimeOT, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeOT)this.GetObject(idleTimeOT, true, id);
                db.FurnitureIdleTimeOTs.Add(entity);
                db.SaveChanges();
                idleTimeOT.FurnitureIdleTimeOTID = entity.FurnitureIdleTimeOTID;
            }

            return Json(new[] { idleTimeOT }.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeOTs_Update([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeOT idleTimeOT)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeOT)this.GetObject(idleTimeOT, false);

                db.FurnitureIdleTimeOTs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeOT }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeOTs_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeOT idleTimeOT)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeOT)this.GetObject(idleTimeOT, false);

                db.FurnitureIdleTimeOTs.Attach(entity);
                db.FurnitureIdleTimeOTs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeOT }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureIdleTimeOT idleTimeOT, bool status, int id = 0)
        {
            if (status)
            {
                idleTimeOT.IdleTimeID = id;
            }
            var entity = new FurnitureIdleTimeOT
            {
                FurnitureIdleTimeOTID = idleTimeOT.FurnitureIdleTimeOTID,
                IdleTimeID   = idleTimeOT.IdleTimeID,
                CostCenterId = idleTimeOT.CostCenterId,
                Rate = idleTimeOT.Rate,
                Value = idleTimeOT.Value

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
