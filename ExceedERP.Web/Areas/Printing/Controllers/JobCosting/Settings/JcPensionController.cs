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
using ExceedERP.Core.Domain.printing.JobCosting.Setting;

namespace ExceedERP.Web.Areas.Printing.Controllers.JobCosting.Settings
{
    public class JcPensionController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public JsonResult GetAllJcPensions()
        {
            var data = db.JcPensions;
            return Json(data.Select(s => new { s.JcPensionId, s.Rate }), JsonRequestBehavior.AllowGet);
        }
        public ActionResult JcPensions_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<JcPension> JcPensions = db.JcPensions;
            DataSourceResult result = JcPensions.ToDataSourceResult(request, JcPension => new {
                JcPensionId = JcPension.JcPensionId,
                Rate = JcPension.Rate,
                Description = JcPension.Description
            });

            return Json(result);
        }

        public ActionResult JcPensions_Create([DataSourceRequest]DataSourceRequest request, JcPension JcPension)
        {
            if (ModelState.IsValid)
            {
                var entity = (JcPension)this.GetObject(JcPension);
                db.JcPensions.Add(entity);
                db.SaveChanges();
                JcPension.JcPensionId = entity.JcPensionId;
            }

            return Json(new[] { JcPension }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JcPensions_Update([DataSourceRequest]DataSourceRequest request, JcPension JcPension)
        {
            if (ModelState.IsValid)
            {
                var entity = (JcPension)this.GetObject(JcPension);

                db.JcPensions.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { JcPension }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult JcPensions_Destroy([DataSourceRequest]DataSourceRequest request, JcPension JcPension)
        {

            if (ModelState.IsValid)
            {
                var entity = (JcPension)this.GetObject(JcPension);

                db.JcPensions.Attach(entity);
                db.JcPensions.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { JcPension }.ToDataSourceResult(request, ModelState));
        }

        private object GetObject(JcPension JcPension)
        {
            var entity = new JcPension
            {
                JcPensionId = JcPension.JcPensionId,
                Rate = JcPension.Rate,
                Description = JcPension.Description

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