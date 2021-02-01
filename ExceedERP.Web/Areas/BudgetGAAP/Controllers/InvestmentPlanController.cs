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
    public class InvestmentPlanController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["FiscalYear"] = db.GLFiscalYears.ToList();
            return View();
        }

        public ActionResult InvestmentPlans_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<InvestmentPlan> investmentplans = db.InvestmentPlans;
            DataSourceResult result = investmentplans.ToDataSourceResult(request, investmentPlan => new {
                InvestmentPlanId = investmentPlan.InvestmentPlanId,
                GlFiscalYearId = investmentPlan.GlFiscalYearId,
                Name = investmentPlan.Name, 
                First = investmentPlan.First,
                Second = investmentPlan.Second,
                Third = investmentPlan.Third,
                Fourth = investmentPlan.Fourth,
                FromAccCode = investmentPlan.FromAccCode,
                ToAccCode = investmentPlan.ToAccCode,
                OrgBranchName = investmentPlan.OrgBranchName,
                DateCreated = investmentPlan.DateCreated,
                LastModified = investmentPlan.LastModified,
                CreatedBy = investmentPlan.CreatedBy,
                ModifiedBy = investmentPlan.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvestmentPlans_Create([DataSourceRequest]DataSourceRequest request, InvestmentPlan investmentPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new InvestmentPlan
                {
                    Name = investmentPlan.Name,
                    GlFiscalYearId = investmentPlan.GlFiscalYearId,
                    First = investmentPlan.First,
                    Second = investmentPlan.Second,
                    Third = investmentPlan.Third,
                    Fourth = investmentPlan.Fourth,
                    FromAccCode = investmentPlan.FromAccCode,
                    ToAccCode = investmentPlan.ToAccCode,
                    OrgBranchName = investmentPlan.OrgBranchName,
                    DateCreated = investmentPlan.DateCreated,
                    LastModified = investmentPlan.LastModified,
                    CreatedBy = investmentPlan.CreatedBy,
                    ModifiedBy = investmentPlan.ModifiedBy
                };

                db.InvestmentPlans.Add(entity);
                db.SaveChanges();
                investmentPlan.InvestmentPlanId = entity.InvestmentPlanId;
            }

            return Json(new[] { investmentPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvestmentPlans_Update([DataSourceRequest]DataSourceRequest request, InvestmentPlan investmentPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new InvestmentPlan
                {
                    Name = investmentPlan.Name,
                    GlFiscalYearId = investmentPlan.GlFiscalYearId,
                    InvestmentPlanId = investmentPlan.InvestmentPlanId,
                    First = investmentPlan.First,
                    Second = investmentPlan.Second,
                    Third = investmentPlan.Third,
                    Fourth = investmentPlan.Fourth,
                    FromAccCode = investmentPlan.FromAccCode,
                    ToAccCode = investmentPlan.ToAccCode,
                    OrgBranchName = investmentPlan.OrgBranchName,
                    DateCreated = investmentPlan.DateCreated,
                    LastModified = investmentPlan.LastModified,
                    CreatedBy = investmentPlan.CreatedBy,
                    ModifiedBy = investmentPlan.ModifiedBy
                };

                db.InvestmentPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { investmentPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvestmentPlans_Destroy([DataSourceRequest]DataSourceRequest request, InvestmentPlan investmentPlan)
        {
            if (ModelState.IsValid)
            {
                var entity = new InvestmentPlan
                {
                    Name = investmentPlan.Name,
                    GlFiscalYearId = investmentPlan.GlFiscalYearId,
                    InvestmentPlanId = investmentPlan.InvestmentPlanId,
                    First = investmentPlan.First,
                    Second = investmentPlan.Second,
                    Third = investmentPlan.Third,
                    Fourth = investmentPlan.Fourth,
                    FromAccCode = investmentPlan.FromAccCode,
                    ToAccCode = investmentPlan.ToAccCode,
                    OrgBranchName = investmentPlan.OrgBranchName,
                    DateCreated = investmentPlan.DateCreated,
                    LastModified = investmentPlan.LastModified,
                    CreatedBy = investmentPlan.CreatedBy,
                    ModifiedBy = investmentPlan.ModifiedBy
                };

                db.InvestmentPlans.Attach(entity);
                db.InvestmentPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { investmentPlan }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
