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
    public class ForcastedIncomeStatementController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            return View();
        }

        public ActionResult ForcastedIncomeStatements_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ForcastedIncomeStatement> forcastedincomestatements = db.ForcastedIncomeStatements;
            DataSourceResult result = forcastedincomestatements.ToDataSourceResult(request, forcastedIncomeStatement => new {
                ForcastedIncomeStatementId = forcastedIncomeStatement.ForcastedIncomeStatementId,
                GlFiscalYearId = forcastedIncomeStatement.GlFiscalYearId,
                Name = forcastedIncomeStatement.Name,
                First = forcastedIncomeStatement.First,
                Second = forcastedIncomeStatement.Second,
                Third = forcastedIncomeStatement.Third,
                Fourth = forcastedIncomeStatement.Fourth,
                FromAccCode = forcastedIncomeStatement.FromAccCode,
                ToAccCode = forcastedIncomeStatement.ToAccCode,
                OrgBranchName = forcastedIncomeStatement.OrgBranchName,
                DateCreated = forcastedIncomeStatement.DateCreated,
                LastModified = forcastedIncomeStatement.LastModified,
                CreatedBy = forcastedIncomeStatement.CreatedBy,
                ModifiedBy = forcastedIncomeStatement.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForcastedIncomeStatements_Create([DataSourceRequest]DataSourceRequest request, ForcastedIncomeStatement forcastedIncomeStatement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForcastedIncomeStatement
                {
                    Name =forcastedIncomeStatement.Name,
                    GlFiscalYearId = forcastedIncomeStatement.GlFiscalYearId,
                    First = forcastedIncomeStatement.First,
                    Second = forcastedIncomeStatement.Second,
                    Third = forcastedIncomeStatement.Third,
                    Fourth = forcastedIncomeStatement.Fourth,
                    FromAccCode = forcastedIncomeStatement.FromAccCode,
                    ToAccCode = forcastedIncomeStatement.ToAccCode,
                    OrgBranchName = forcastedIncomeStatement.OrgBranchName,
                    DateCreated = forcastedIncomeStatement.DateCreated,
                    LastModified = forcastedIncomeStatement.LastModified,
                    CreatedBy = forcastedIncomeStatement.CreatedBy,
                    ModifiedBy = forcastedIncomeStatement.ModifiedBy
                };

                db.ForcastedIncomeStatements.Add(entity);
                db.SaveChanges();
                forcastedIncomeStatement.ForcastedIncomeStatementId = entity.ForcastedIncomeStatementId;
            }

            return Json(new[] { forcastedIncomeStatement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForcastedIncomeStatements_Update([DataSourceRequest]DataSourceRequest request, ForcastedIncomeStatement forcastedIncomeStatement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForcastedIncomeStatement
                {
                    Name = forcastedIncomeStatement.Name,
                    GlFiscalYearId = forcastedIncomeStatement.GlFiscalYearId,
                    ForcastedIncomeStatementId = forcastedIncomeStatement.ForcastedIncomeStatementId,
                    First = forcastedIncomeStatement.First,
                    Second = forcastedIncomeStatement.Second,
                    Third = forcastedIncomeStatement.Third,
                    Fourth = forcastedIncomeStatement.Fourth,
                    FromAccCode = forcastedIncomeStatement.FromAccCode,
                    ToAccCode = forcastedIncomeStatement.ToAccCode,
                    OrgBranchName = forcastedIncomeStatement.OrgBranchName,
                    DateCreated = forcastedIncomeStatement.DateCreated,
                    LastModified = forcastedIncomeStatement.LastModified,
                    CreatedBy = forcastedIncomeStatement.CreatedBy,
                    ModifiedBy = forcastedIncomeStatement.ModifiedBy
                };

                db.ForcastedIncomeStatements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { forcastedIncomeStatement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ForcastedIncomeStatements_Destroy([DataSourceRequest]DataSourceRequest request, ForcastedIncomeStatement forcastedIncomeStatement)
        {
            if (ModelState.IsValid)
            {
                var entity = new ForcastedIncomeStatement
                {
                    Name = forcastedIncomeStatement.Name,
                    GlFiscalYearId = forcastedIncomeStatement.GlFiscalYearId,
                    ForcastedIncomeStatementId = forcastedIncomeStatement.ForcastedIncomeStatementId,
                    First = forcastedIncomeStatement.First,
                    Second = forcastedIncomeStatement.Second,
                    Third = forcastedIncomeStatement.Third,
                    Fourth = forcastedIncomeStatement.Fourth,
                    FromAccCode = forcastedIncomeStatement.FromAccCode,
                    ToAccCode = forcastedIncomeStatement.ToAccCode,
                    OrgBranchName = forcastedIncomeStatement.OrgBranchName,
                    DateCreated = forcastedIncomeStatement.DateCreated,
                    LastModified = forcastedIncomeStatement.LastModified,
                    CreatedBy = forcastedIncomeStatement.CreatedBy,
                    ModifiedBy = forcastedIncomeStatement.ModifiedBy
                };

                db.ForcastedIncomeStatements.Attach(entity);
                db.ForcastedIncomeStatements.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { forcastedIncomeStatement }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
