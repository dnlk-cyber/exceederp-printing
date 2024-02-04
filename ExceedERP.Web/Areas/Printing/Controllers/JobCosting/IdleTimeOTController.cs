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
using ExceedERP.Core.Domain.printing.JobCosting;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting

{
    // [AuthorizeRoles(JobCostingRoles.JobCostingIdleTime)]
    public class IdleTimeOTController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeOTs_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<IdleTimeOT> idletimeots = db.IdleTimeOTs.Where(q=>q.IdleTimeID == id);
            DataSourceResult result = idletimeots.ToDataSourceResult(request, idleTimeOT => new {
                IdleTimeOTID = idleTimeOT.IdleTimeOTID,
                IdleTimeID = idleTimeOT.IdleTimeID,
                CostCenterId = idleTimeOT.CostCenterId,
                Rate = idleTimeOT.Rate,
                Value = idleTimeOT.Value
            });

            return Json(result);
        }

        public ActionResult IdleTimeOTs_Create([DataSourceRequest]DataSourceRequest request, IdleTimeOT idleTimeOT, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (IdleTimeOT)this.GetObject(idleTimeOT, true, id);
                db.IdleTimeOTs.Add(entity);
                db.SaveChanges();
                idleTimeOT.IdleTimeOTID = entity.IdleTimeOTID;
            }

            return Json(new[] { idleTimeOT }.ToDataSourceResult(request, ModelState));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeOTs_Update([DataSourceRequest]DataSourceRequest request, IdleTimeOT idleTimeOT)
        {
            if (ModelState.IsValid)
            {
                var entity = (IdleTimeOT)this.GetObject(idleTimeOT, false);

                db.IdleTimeOTs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeOT }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeOTs_Destroy([DataSourceRequest]DataSourceRequest request, IdleTimeOT idleTimeOT)
        {

            if (ModelState.IsValid)
            {
                var entity = (IdleTimeOT)this.GetObject(idleTimeOT, false);

                db.IdleTimeOTs.Attach(entity);
                db.IdleTimeOTs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeOT }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(IdleTimeOT idleTimeOT, bool status, int id = 0)
        {
            if (status)
            {
                idleTimeOT.IdleTimeID = id;
            }
            var entity = new IdleTimeOT
            {
                IdleTimeOTID = idleTimeOT.IdleTimeOTID,
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
