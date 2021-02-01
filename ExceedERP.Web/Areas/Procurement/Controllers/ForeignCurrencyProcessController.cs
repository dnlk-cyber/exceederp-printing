﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Procurement.ForeignPurchase;
 using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Procurement.Controllers
{
    public class ForeignCurrencyProcessController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ForeignCurrencyProcesses_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ForeignCurrencyProcess> foreigncurrencyprocesses = db.ForeignCurrencyProcesses;
            DataSourceResult result = foreigncurrencyprocesses.ToDataSourceResult(request, foreignCurrencyProcess => new {
                ForeignCurrencyProcessForeignCurrencyProcessId = foreignCurrencyProcess.ForeignCurrencyProcessId,
                Description = foreignCurrencyProcess.Description
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignCurrencyProcesses_Create([DataSourceRequest]DataSourceRequest request, ForeignCurrencyProcess foreignCurrencyProcess)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignCurrencyProcess
                {
                    Description = foreignCurrencyProcess.Description
                };

                db.ForeignCurrencyProcesses.Add(entity);
                db.SaveChanges();
                foreignCurrencyProcess.ForeignCurrencyProcessId = entity.ForeignCurrencyProcessId;
            }

            return Json(new[] { foreignCurrencyProcess }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignCurrencyProcesses_Update([DataSourceRequest]DataSourceRequest request, ForeignCurrencyProcess foreignCurrencyProcess)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignCurrencyProcess
                {
                    ForeignCurrencyProcessId = foreignCurrencyProcess.ForeignCurrencyProcessId,
                    Description = foreignCurrencyProcess.Description
                };

                db.ForeignCurrencyProcesses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { foreignCurrencyProcess }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForeignCurrencyProcesses_Destroy([DataSourceRequest]DataSourceRequest request, ForeignCurrencyProcess foreignCurrencyProcess)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForeignCurrencyProcess
                {
                    ForeignCurrencyProcessId = foreignCurrencyProcess.ForeignCurrencyProcessId,
                    Description = foreignCurrencyProcess.Description
                };

                db.ForeignCurrencyProcesses.Attach(entity);
                db.ForeignCurrencyProcesses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { foreignCurrencyProcess }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
