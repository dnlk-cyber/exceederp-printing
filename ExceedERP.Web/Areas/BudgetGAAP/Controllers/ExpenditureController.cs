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
using ExceedERP.Core.Domain.Budget;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.BudgetGAAP.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetUser)] 
    public class ExpenditureController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            return View();
        }

        public ActionResult Expenditures_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Expenditure> expenditures = db.Expenditures;
            DataSourceResult result = expenditures.ToDataSourceResult(request, expenditure => new {
                ExpenditureId = expenditure.ExpenditureId,
                Name = expenditure.Name,
                GlFiscalYearId = expenditure.GlFiscalYearId,
                First = expenditure.First,
                Second = expenditure.Second,
                Third = expenditure.Third,
                Fourth = expenditure.Fourth,
                FromAccCode = expenditure.FromAccCode,
                ToAccCode = expenditure.ToAccCode,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Expenditures_Create([DataSourceRequest]DataSourceRequest request, Expenditure expenditure)
        {
            if (ModelState.IsValid)
            {
                var entity = new Expenditure
                {
                    Name = expenditure.Name,
                    GlFiscalYearId = expenditure.GlFiscalYearId,
                    First = expenditure.First,
                    Second = expenditure.Second,
                    Third = expenditure.Third,
                    Fourth = expenditure.Fourth,
                    FromAccCode = expenditure.FromAccCode,
                    ToAccCode = expenditure.ToAccCode,
                };

                db.Expenditures.Add(entity);
                db.SaveChanges();
                expenditure.ExpenditureId = entity.ExpenditureId;
            }

            return Json(new[] { expenditure }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Expenditures_Update([DataSourceRequest]DataSourceRequest request, Expenditure expenditure)
        {
            if (ModelState.IsValid)
            {
                var entity = new Expenditure
                {
                   
                    ExpenditureId = expenditure.ExpenditureId,
                    Name = expenditure.Name,
                    GlFiscalYearId = expenditure.GlFiscalYearId,
                    First = expenditure.First,
                    Second = expenditure.Second,
                    Third = expenditure.Third,
                    Fourth = expenditure.Fourth,
                    FromAccCode = expenditure.FromAccCode,
                    ToAccCode = expenditure.ToAccCode,
                };

                db.Expenditures.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { expenditure }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Expenditures_Destroy([DataSourceRequest]DataSourceRequest request, Expenditure expenditure)
        {
            if (ModelState.IsValid)
            {
                var entity = new Expenditure
                {
                    ExpenditureId = expenditure.ExpenditureId,
                    Name = expenditure.Name,
                    GlFiscalYearId = expenditure.GlFiscalYearId,
                    First = expenditure.First,
                    Second = expenditure.Second,
                    Third = expenditure.Third,
                    Fourth = expenditure.Fourth,
                    FromAccCode = expenditure.FromAccCode,
                    ToAccCode = expenditure.ToAccCode,
                };

                db.Expenditures.Attach(entity);
                db.Expenditures.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { expenditure }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
