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
using ExceedERP.Core.Domain.Manufacturing.JobCosting.Setting;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.JobCosting.Settings
{
    public class FurnitureUnitController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetAllUnits()
        {
            var data = db.Units;
            return Json(data.Select(s => new { s.UnitId, s.UOM }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult Units_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FurnitureUnit> units = db.FurnitureUnits;
            DataSourceResult result = units.ToDataSourceResult(request, unit => new {
                UnitId = unit.FurnitureUnitId,
                UOM = unit.UOM,
                Description = unit.Description,
                DateCreated = unit.DateCreated,
                LastModified = unit.LastModified,
                CreatedBy = unit.CreatedBy,
                ModifiedBy = unit.ModifiedBy
            });

            return Json(result);
        }

        public ActionResult Units_Create([DataSourceRequest]DataSourceRequest request, FurnitureUnit unit)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureUnit)this.GetObject(unit);
                db.FurnitureUnits.Add(entity);
                db.SaveChanges();
                unit.FurnitureUnitId = entity.FurnitureUnitId;
            }

            return Json(new[] { unit }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Units_Update([DataSourceRequest]DataSourceRequest request, FurnitureUnit unit)
        {
            if (ModelState.IsValid)
            {
                var entity = (FurnitureUnit)this.GetObject(unit);

                db.FurnitureUnits.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { unit }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Units_Destroy([DataSourceRequest]DataSourceRequest request, FurnitureUnit unit)
        {

            if (ModelState.IsValid)
            {
                var entity = (FurnitureUnit)this.GetObject(unit);

                db.FurnitureUnits.Attach(entity);
                db.FurnitureUnits.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { unit }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(FurnitureUnit unit)
        {
            var entity = new FurnitureUnit
            {
                FurnitureUnitId = unit.FurnitureUnitId,
                UOM = unit.UOM,
                Description = unit.Description,
                DateCreated = unit.DateCreated,
                LastModified = unit.LastModified,
                CreatedBy = unit.CreatedBy,
                ModifiedBy = unit.ModifiedBy

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