﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Construction;
﻿using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class UnitRateController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult UnitRate_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<UnitRate> unitrate = db.UnitRate;
            DataSourceResult result = unitrate.ToDataSourceResult(request, unitRate => new
            {
                UnitRateId = unitRate.UnitRateId,
                Diameter = unitRate.Diameter,
                UnitWeight = unitRate.UnitWeight,
                Remark = unitRate.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnitRate_Create([DataSourceRequest]DataSourceRequest request, UnitRate unitRate)
        {
            if (ModelState.IsValid)
            {
                var entity = new UnitRate
                {
                    Diameter = unitRate.Diameter,
                    UnitWeight = unitRate.UnitWeight,
                    Remark = unitRate.Remark
                };

                db.UnitRate.Add(entity);
                db.SaveChanges();
                unitRate.UnitRateId = entity.UnitRateId;
            }

            return Json(new[] { unitRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnitRate_Update([DataSourceRequest]DataSourceRequest request, UnitRate unitRate)
        {
            if (ModelState.IsValid)
            {
                var entity = new UnitRate
                {
                    UnitRateId = unitRate.UnitRateId,
                    Diameter = unitRate.Diameter,
                    UnitWeight = unitRate.UnitWeight,
                    Remark = unitRate.Remark
                };

                db.UnitRate.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { unitRate }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult UnitRate_Destroy([DataSourceRequest]DataSourceRequest request, UnitRate unitRate)
        {
            if (ModelState.IsValid)
            {
                var entity = new UnitRate
                {
                    UnitRateId = unitRate.UnitRateId,
                    Diameter = unitRate.Diameter,
                    UnitWeight = unitRate.UnitWeight,
                    Remark = unitRate.Remark
                };

                db.UnitRate.Attach(entity);
                db.UnitRate.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { unitRate }.ToDataSourceResult(request, ModelState));
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
