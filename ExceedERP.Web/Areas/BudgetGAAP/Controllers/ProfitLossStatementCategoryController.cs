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
    [AuthorizeRoles(BudgetRoles.BudgetAdmin)] 
    public class ProfitLossStatementCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProfitLossStatementCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProfitLossStatementCategory> profitlossstatementcategories = db.ProfitLossStatementCategories;
            DataSourceResult result = profitlossstatementcategories.ToDataSourceResult(request, profitLossStatementCategory => new {
                ProfitLossStatementCategoryId = profitLossStatementCategory.ProfitLossStatementCategoryId,
                Name = profitLossStatementCategory.Name,
                ProfitLostType = profitLossStatementCategory.ProfitLostType,
                OrgBranchName = profitLossStatementCategory.OrgBranchName,
                DateCreated = profitLossStatementCategory.DateCreated,
                LastModified = profitLossStatementCategory.LastModified,
                CreatedBy = profitLossStatementCategory.CreatedBy,
                ModifiedBy = profitLossStatementCategory.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProfitLossStatementCategories_Create([DataSourceRequest]DataSourceRequest request, ProfitLossStatementCategory profitLossStatementCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProfitLossStatementCategory
                {
                    Name = profitLossStatementCategory.Name,
                    ProfitLostType = profitLossStatementCategory.ProfitLostType,
                    OrgBranchName = profitLossStatementCategory.OrgBranchName,
                    DateCreated = profitLossStatementCategory.DateCreated,
                    LastModified = profitLossStatementCategory.LastModified,
                    CreatedBy = profitLossStatementCategory.CreatedBy,
                    ModifiedBy = profitLossStatementCategory.ModifiedBy
                };

                db.ProfitLossStatementCategories.Add(entity);
                db.SaveChanges();
                profitLossStatementCategory.ProfitLossStatementCategoryId = entity.ProfitLossStatementCategoryId;
            }

            return Json(new[] { profitLossStatementCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProfitLossStatementCategories_Update([DataSourceRequest]DataSourceRequest request, ProfitLossStatementCategory profitLossStatementCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProfitLossStatementCategory
                {
                    ProfitLossStatementCategoryId = profitLossStatementCategory.ProfitLossStatementCategoryId,
                    Name = profitLossStatementCategory.Name,
                    ProfitLostType = profitLossStatementCategory.ProfitLostType,
                    OrgBranchName = profitLossStatementCategory.OrgBranchName,
                    DateCreated = profitLossStatementCategory.DateCreated,
                    LastModified = profitLossStatementCategory.LastModified,
                    CreatedBy = profitLossStatementCategory.CreatedBy,
                    ModifiedBy = profitLossStatementCategory.ModifiedBy
                };

                db.ProfitLossStatementCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { profitLossStatementCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProfitLossStatementCategories_Destroy([DataSourceRequest]DataSourceRequest request, ProfitLossStatementCategory profitLossStatementCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProfitLossStatementCategory
                {
                    ProfitLossStatementCategoryId = profitLossStatementCategory.ProfitLossStatementCategoryId,
                    Name = profitLossStatementCategory.Name,
                    ProfitLostType = profitLossStatementCategory.ProfitLostType,
                    OrgBranchName = profitLossStatementCategory.OrgBranchName,
                    DateCreated = profitLossStatementCategory.DateCreated,
                    LastModified = profitLossStatementCategory.LastModified,
                    CreatedBy = profitLossStatementCategory.CreatedBy,
                    ModifiedBy = profitLossStatementCategory.ModifiedBy
                };

                db.ProfitLossStatementCategories.Attach(entity);
                db.ProfitLossStatementCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { profitLossStatementCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
