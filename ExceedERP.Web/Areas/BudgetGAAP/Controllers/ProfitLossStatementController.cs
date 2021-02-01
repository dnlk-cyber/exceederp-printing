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
using ExceedERP.Helpers.Common;  

namespace ExceedERP.Web.Areas.BudgetGAAP.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetUser)] 
    public class ProfitLossStatementController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["ProfitLossStatementCategory"] = db.ProfitLossStatementCategories.ToList();
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
           // ViewData["Type"] = EnumExtension.EnumToSelectList(typeof(Enums.ProfitLostType));

            return View();
        }

        public ActionResult ProfitLossStatements_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<ProfitLossStatement> profitlossstatements = db.ProfitLossStatements.AsNoTracking().ToList();
            DataSourceResult result = profitlossstatements.ToDataSourceResult(request, profitLossStatement => new {
                ProfitLossStatementId = profitLossStatement.ProfitLossStatementId,
                Name = profitLossStatement.Name,
                ProfitLossStatementCategoryId = profitLossStatement.ProfitLossStatementCategoryId,
                GlFiscalYearId = profitLossStatement.GlFiscalYearId,
                CategoryType = profitLossStatement.ProfitLossStatementCategory.ProfitLostType,
                First = profitLossStatement.First,
                Second = profitLossStatement.Second,
                Third = profitLossStatement.Third,
                Fourth = profitLossStatement.Fourth,
                FromAccCode = profitLossStatement.FromAccCode,
                ToAccCode = profitLossStatement.ToAccCode,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProfitLossStatements_Create([DataSourceRequest]DataSourceRequest request, ProfitLossStatement profitLossStatement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProfitLossStatement
                {
                    Name = profitLossStatement.Name,
                    ProfitLossStatementCategoryId = profitLossStatement.ProfitLossStatementCategoryId,
                    GlFiscalYearId = profitLossStatement.GlFiscalYearId,
                    First = profitLossStatement.First,
                    Second = profitLossStatement.Second,
                    Third = profitLossStatement.Third,
                    Fourth = profitLossStatement.Fourth,
                    FromAccCode = profitLossStatement.FromAccCode,
                    ToAccCode = profitLossStatement.ToAccCode,
                };

                db.ProfitLossStatements.Add(entity);
                db.SaveChanges();
                profitLossStatement.ProfitLossStatementId = entity.ProfitLossStatementId;
            }

            return Json(new[] { profitLossStatement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProfitLossStatements_Update([DataSourceRequest]DataSourceRequest request, ProfitLossStatement profitLossStatement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProfitLossStatement
                {
                    Name = profitLossStatement.Name,
                    ProfitLossStatementCategoryId = profitLossStatement.ProfitLossStatementCategoryId,
                    GlFiscalYearId = profitLossStatement.GlFiscalYearId,
                    ProfitLossStatementId = profitLossStatement.ProfitLossStatementId,
                    First = profitLossStatement.First,
                    Second = profitLossStatement.Second,
                    Third = profitLossStatement.Third,
                    Fourth = profitLossStatement.Fourth,
                    FromAccCode = profitLossStatement.FromAccCode,
                    ToAccCode = profitLossStatement.ToAccCode,
                };

                db.ProfitLossStatements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { profitLossStatement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProfitLossStatements_Destroy([DataSourceRequest]DataSourceRequest request, ProfitLossStatement profitLossStatement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProfitLossStatement
                {
                    Name = profitLossStatement.Name,
                    ProfitLossStatementCategoryId = profitLossStatement.ProfitLossStatementCategoryId,
                    ProfitLossStatementId = profitLossStatement.ProfitLossStatementId,
                    GlFiscalYearId = profitLossStatement.GlFiscalYearId,
                    First = profitLossStatement.First,
                    Second = profitLossStatement.Second,
                    Third = profitLossStatement.Third,
                    Fourth = profitLossStatement.Fourth,
                    FromAccCode = profitLossStatement.FromAccCode,
                    ToAccCode = profitLossStatement.ToAccCode,
                };

                db.ProfitLossStatements.Attach(entity);
                db.ProfitLossStatements.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { profitLossStatement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
