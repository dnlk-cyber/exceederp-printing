﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Budget.GAAP;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Budget;

namespace ExceedERP.Web.Areas.BudgetGAAP.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetUser)] 
    public class CostOfServiceSoldController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            return View();
        }

        public ActionResult CostOfServiceSolds_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CostOfServiceSold> costofservicesolds = db.CostOfServiceSolds;
            DataSourceResult result = costofservicesolds.ToDataSourceResult(request, costOfServiceSold => new {
                CostOfServiceSoldId = costOfServiceSold.CostOfServiceSoldId,
                Name = costOfServiceSold.Name,
                GlFiscalYearId = costOfServiceSold.GlFiscalYearId,
                First = costOfServiceSold.First,
                Second = costOfServiceSold.Second,
                Third = costOfServiceSold.Third,
                Fourth = costOfServiceSold.Fourth,
                FromAccCode = costOfServiceSold.FromAccCode,
                ToAccCode = costOfServiceSold.ToAccCode,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostOfServiceSolds_Create([DataSourceRequest]DataSourceRequest request, CostOfServiceSold costOfServiceSold)
        {
            if (ModelState.IsValid)
            {
                var entity = new CostOfServiceSold
                {
                    Name = costOfServiceSold.Name,
                    GlFiscalYearId = costOfServiceSold.GlFiscalYearId,
                    First = costOfServiceSold.First,
                    Second = costOfServiceSold.Second,
                    Third = costOfServiceSold.Third,
                    Fourth = costOfServiceSold.Fourth,
                    FromAccCode = costOfServiceSold.FromAccCode,
                    ToAccCode = costOfServiceSold.ToAccCode,
                };

                db.CostOfServiceSolds.Add(entity);
                db.SaveChanges();
                costOfServiceSold.CostOfServiceSoldId = entity.CostOfServiceSoldId;
            }

            return Json(new[] { costOfServiceSold }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostOfServiceSolds_Update([DataSourceRequest]DataSourceRequest request, CostOfServiceSold costOfServiceSold)
        {
            if (ModelState.IsValid)
            {
                var entity = new CostOfServiceSold
                {
                    Name = costOfServiceSold.Name,
                    CostOfServiceSoldId = costOfServiceSold.CostOfServiceSoldId,
                    GlFiscalYearId = costOfServiceSold.GlFiscalYearId,
                    First = costOfServiceSold.First,
                    Second = costOfServiceSold.Second,
                    Third = costOfServiceSold.Third,
                    Fourth = costOfServiceSold.Fourth,
                    FromAccCode = costOfServiceSold.FromAccCode,
                    ToAccCode = costOfServiceSold.ToAccCode,
                };

                db.CostOfServiceSolds.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { costOfServiceSold }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CostOfServiceSolds_Destroy([DataSourceRequest]DataSourceRequest request, CostOfServiceSold costOfServiceSold)
        {
            if (ModelState.IsValid)
            {
                var entity = new CostOfServiceSold
                {
                    CostOfServiceSoldId = costOfServiceSold.CostOfServiceSoldId,
                    GlFiscalYearId = costOfServiceSold.GlFiscalYearId,
                    Name = costOfServiceSold.Name,
                    First = costOfServiceSold.First,
                    Second = costOfServiceSold.Second,
                    Third = costOfServiceSold.Third,
                    Fourth = costOfServiceSold.Fourth,
                    FromAccCode = costOfServiceSold.FromAccCode,
                    ToAccCode = costOfServiceSold.ToAccCode,
                };

                db.CostOfServiceSolds.Attach(entity);
                db.CostOfServiceSolds.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { costOfServiceSold }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
