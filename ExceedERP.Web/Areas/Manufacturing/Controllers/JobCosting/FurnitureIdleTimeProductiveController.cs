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
    public class FurnitureIdleTimeProductiveController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult IdleTimeProductives_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<FurnitureIdleTimeProductive> idletimeproductives = db.FurnitureIdleTimeProductives.Where(q=>q.IdleTimeID == id);
            DataSourceResult result = idletimeproductives.ToDataSourceResult(request, idleTimeProductive => new {
                FurnitureIdleTimeProductiveID = idleTimeProductive.FurnitureIdleTimeProductiveID,
                IdleTimeID = idleTimeProductive.IdleTimeID,
                CostCenterId = idleTimeProductive.CostCenterId,
                Rate = idleTimeProductive.Rate,
                Value = idleTimeProductive.Value
            });

            return Json(result);
        }


        public ActionResult IdleTimeProductives_Create([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeProductive idleTimeProductive, int id)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeProductive)this.GetObject(idleTimeProductive, true, id);
                db.FurnitureIdleTimeProductives.Add(entity);
                db.SaveChanges();
                idleTimeProductive.FurnitureIdleTimeProductiveID = entity.FurnitureIdleTimeProductiveID;
            }

            return Json(new[] { idleTimeProductive }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeProductives_Update([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeProductive idleTimeProductive)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeProductive)this.GetObject(idleTimeProductive, false);

                db.FurnitureIdleTimeProductives.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { idleTimeProductive }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult IdleTimeProductives_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureIdleTimeProductive idleTimeProductive)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureIdleTimeProductive)this.GetObject(idleTimeProductive, false);

                db.FurnitureIdleTimeProductives.Attach(entity);
                db.FurnitureIdleTimeProductives.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { idleTimeProductive }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureIdleTimeProductive idleTimeProductive, bool status, int id = 0)
        {
            if (status)
            {
                idleTimeProductive.IdleTimeID = id;
            }
            var entity = new FurnitureIdleTimeProductive
            {
                FurnitureIdleTimeProductiveID = idleTimeProductive.FurnitureIdleTimeProductiveID,
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
