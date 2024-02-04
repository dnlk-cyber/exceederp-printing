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
    public class IdleTimeProductiveController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeProductives_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<IdleTimeProductive> idletimeproductives = db.IdleTimeProductives.Where(q=>q.IdleTimeID == id);
            DataSourceResult result = idletimeproductives.ToDataSourceResult(request, idleTimeProductive => new {
                IdleTimeProductiveID = idleTimeProductive.IdleTimeProductiveID,
                IdleTimeID = idleTimeProductive.IdleTimeID,
                CostCenterId = idleTimeProductive.CostCenterId,
                Rate = idleTimeProductive.Rate,
                Value = idleTimeProductive.Value
            });

            return Json(result);
        }


        public ActionResult IdleTimeProductives_Create([DataSourceRequest]DataSourceRequest request, IdleTimeProductive idleTimeProductive, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (IdleTimeProductive)this.GetObject(idleTimeProductive, true, id);
                db.IdleTimeProductives.Add(entity);
                db.SaveChanges();
                idleTimeProductive.IdleTimeProductiveID = entity.IdleTimeProductiveID;
            }

            return Json(new[] { idleTimeProductive }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeProductives_Update([DataSourceRequest]DataSourceRequest request, IdleTimeProductive idleTimeProductive)
        {
            if (ModelState.IsValid)
            {
                var entity = (IdleTimeProductive)this.GetObject(idleTimeProductive, false);

                db.IdleTimeProductives.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeProductive }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeProductives_Destroy([DataSourceRequest]DataSourceRequest request, IdleTimeProductive idleTimeProductive)
        {

            if (ModelState.IsValid)
            {
                var entity = (IdleTimeProductive)this.GetObject(idleTimeProductive, false);

                db.IdleTimeProductives.Attach(entity);
                db.IdleTimeProductives.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeProductive }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(IdleTimeProductive idleTimeProductive, bool status, int id = 0)
        {
            if (status)
            {
                idleTimeProductive.IdleTimeID = id;
            }
            var entity = new IdleTimeProductive
            {
                IdleTimeProductiveID = idleTimeProductive.IdleTimeProductiveID,
                IdleTimeID = idleTimeProductive.IdleTimeID,
                CostCenterId = idleTimeProductive.CostCenterId,
                Rate = idleTimeProductive.Rate,
                Value = idleTimeProductive.Value

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
