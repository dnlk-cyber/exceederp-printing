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
using ExceedERP.Core.Domain.Dashboard;

namespace ExceedERP.Web.Areas.BudgetGAAP.Controllers
{
    [AuthorizeRoles(BudgetRoles.BudgetUser)] 
    public class BalanceSheetController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
           
            ViewData["BalanceSheetCategory"] = db.BalanceSheetCategories.ToList();
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();

            return View();
        }

        public ActionResult BalanceSheets_Read([DataSourceRequest]DataSourceRequest request)
        {
            List<BalanceSheet> balancesheets = db.BalanceSheets.ToList();
            DataSourceResult result = balancesheets.ToDataSourceResult(request, balanceSheet => new {
                BalanceSheetId = balanceSheet.BalanceSheetId,
                Name = balanceSheet.Name,
                BalanceSheetCategoryId = balanceSheet.BalanceSheetCategoryId,
                GlFiscalYearId = balanceSheet.GlFiscalYearId,
                CategoryType = balanceSheet.BalanceSheetCategory.BalanceSheetType,
                First = balanceSheet.First,
                Second = balanceSheet.Second,
                Third = balanceSheet.Third,
                Fourth = balanceSheet.Fourth,
                Total = balanceSheet.First + balanceSheet.Second +balanceSheet.Third + balanceSheet.Fourth,
                FromAccCode = balanceSheet.FromAccCode,
                ToAccCode = balanceSheet.ToAccCode,
                OrgBranchName = balanceSheet.OrgBranchName,
                DateCreated = balanceSheet.DateCreated,
                LastModified = balanceSheet.LastModified,
                CreatedBy = balanceSheet.CreatedBy,
                ModifiedBy = balanceSheet.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BalanceSheets_Create([DataSourceRequest]DataSourceRequest request, BalanceSheet balanceSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new BalanceSheet
                {
                    Name = balanceSheet.Name,
                    BalanceSheetCategoryId = balanceSheet.BalanceSheetCategoryId,
                    GlFiscalYearId = balanceSheet.GlFiscalYearId,
                    First = balanceSheet.First,
                    Second = balanceSheet.Second,
                    Third = balanceSheet.Third,
                    Fourth = balanceSheet.Fourth,
                    FromAccCode = balanceSheet.FromAccCode,
                    ToAccCode = balanceSheet.ToAccCode,
                    OrgBranchName = balanceSheet.OrgBranchName,
                    DateCreated = balanceSheet.DateCreated,
                    LastModified = balanceSheet.LastModified,
                    CreatedBy = balanceSheet.CreatedBy,
                    ModifiedBy = balanceSheet.ModifiedBy
                };

                db.BalanceSheets.Add(entity);
                db.SaveChanges();
                balanceSheet.BalanceSheetId = entity.BalanceSheetId;
            }

            return Json(new[] { balanceSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BalanceSheets_Update([DataSourceRequest]DataSourceRequest request, BalanceSheet balanceSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new BalanceSheet
                {
                    BalanceSheetId = balanceSheet.BalanceSheetId,
                    Name = balanceSheet.Name,
                    BalanceSheetCategoryId = balanceSheet.BalanceSheetCategoryId,
                    GlFiscalYearId = balanceSheet.GlFiscalYearId,
                    First = balanceSheet.First,
                    Second = balanceSheet.Second,
                    Third = balanceSheet.Third,
                    Fourth = balanceSheet.Fourth,
                    FromAccCode = balanceSheet.FromAccCode,
                    ToAccCode = balanceSheet.ToAccCode,
                    OrgBranchName = balanceSheet.OrgBranchName,
                    DateCreated = balanceSheet.DateCreated,
                    LastModified = balanceSheet.LastModified,
                    CreatedBy = balanceSheet.CreatedBy,
                    ModifiedBy = balanceSheet.ModifiedBy
                };

                db.BalanceSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { balanceSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BalanceSheets_Destroy([DataSourceRequest]DataSourceRequest request, BalanceSheet balanceSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new BalanceSheet
                {
                    BalanceSheetId = balanceSheet.BalanceSheetId,
                    Name = balanceSheet.Name,
                    BalanceSheetCategoryId = balanceSheet.BalanceSheetCategoryId,
                    GlFiscalYearId = balanceSheet.GlFiscalYearId,
                    First = balanceSheet.First,
                    Second = balanceSheet.Second,
                    Third = balanceSheet.Third,
                    Fourth = balanceSheet.Fourth,
                    FromAccCode = balanceSheet.FromAccCode,
                    ToAccCode = balanceSheet.ToAccCode,
                    OrgBranchName = balanceSheet.OrgBranchName,
                    DateCreated = balanceSheet.DateCreated,
                    LastModified = balanceSheet.LastModified,
                    CreatedBy = balanceSheet.CreatedBy,
                    ModifiedBy = balanceSheet.ModifiedBy
                };

                db.BalanceSheets.Attach(entity);
                db.BalanceSheets.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { balanceSheet }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
