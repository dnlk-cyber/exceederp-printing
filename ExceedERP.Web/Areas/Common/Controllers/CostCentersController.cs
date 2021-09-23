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

namespace ExceedERP.Web.Areas.Common.Controllers
{
    public class CostCentersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CostCenters_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CostCenter> costcenters = db.CostCenters;
            DataSourceResult result = costcenters.ToDataSourceResult(request, costCenter => new {
                CostCenterId = costCenter.CostCenterId,
                CostCenterName = costCenter.CostCenterName,
                Description = costCenter.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostCenters_Create([DataSourceRequest]DataSourceRequest request, CostCenter costCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new CostCenter
                {
                    CostCenterName = costCenter.CostCenterName,
                    Description = costCenter.Description
                };

                db.CostCenters.Add(entity);
                db.SaveChanges();
                costCenter.CostCenterId = entity.CostCenterId;
            }

            return Json(new[] { costCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostCenters_Update([DataSourceRequest]DataSourceRequest request, CostCenter costCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new CostCenter
                {
                    CostCenterId = costCenter.CostCenterId,
                    CostCenterName = costCenter.CostCenterName,
                    Description = costCenter.Description
                };

                db.CostCenters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { costCenter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostCenters_Destroy([DataSourceRequest]DataSourceRequest request, CostCenter costCenter)
        {
            if (ModelState.IsValid)
            {
                var entity = new CostCenter
                {
                    CostCenterId = costCenter.CostCenterId,
                    CostCenterName = costCenter.CostCenterName,
                    Description = costCenter.Description
                };

                db.CostCenters.Attach(entity);
                db.CostCenters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { costCenter }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
