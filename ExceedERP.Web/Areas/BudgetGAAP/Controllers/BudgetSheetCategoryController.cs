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
    public class BudgetSheetCategoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BalanceSheetCategories_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<BudgetSheetCategory> balancesheetcategories = db.BalanceSheetCategories;
            DataSourceResult result = balancesheetcategories.ToDataSourceResult(request, balanceSheetCategory => new {
                BalanceSheetCategoryId = balanceSheetCategory.BalanceSheetCategoryId,
                BalanceSheetType = balanceSheetCategory.BalanceSheetType,
                Name = balanceSheetCategory.Name,
                OrgBranchName = balanceSheetCategory.OrgBranchName,
                DateCreated = balanceSheetCategory.DateCreated,
                LastModified = balanceSheetCategory.LastModified,
                CreatedBy = balanceSheetCategory.CreatedBy,
                ModifiedBy = balanceSheetCategory.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BalanceSheetCategories_Create([DataSourceRequest]DataSourceRequest request, BudgetSheetCategory balanceSheetCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetSheetCategory
                {
                    BalanceSheetType = balanceSheetCategory.BalanceSheetType,
                    Name = balanceSheetCategory.Name,
                    OrgBranchName = balanceSheetCategory.OrgBranchName,
                    DateCreated = balanceSheetCategory.DateCreated,
                    LastModified = balanceSheetCategory.LastModified,
                    CreatedBy = balanceSheetCategory.CreatedBy,
                    ModifiedBy = balanceSheetCategory.ModifiedBy
                };

                db.BalanceSheetCategories.Add(entity);
                db.SaveChanges();
                balanceSheetCategory.BalanceSheetCategoryId = entity.BalanceSheetCategoryId;
            }

            return Json(new[] { balanceSheetCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BalanceSheetCategories_Update([DataSourceRequest]DataSourceRequest request, BudgetSheetCategory balanceSheetCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetSheetCategory
                {
                    BalanceSheetCategoryId = balanceSheetCategory.BalanceSheetCategoryId,
                    BalanceSheetType = balanceSheetCategory.BalanceSheetType,
                    Name = balanceSheetCategory.Name,
                    OrgBranchName = balanceSheetCategory.OrgBranchName,
                    DateCreated = balanceSheetCategory.DateCreated,
                    LastModified = balanceSheetCategory.LastModified,
                    CreatedBy = balanceSheetCategory.CreatedBy,
                    ModifiedBy = balanceSheetCategory.ModifiedBy
                };

                db.BalanceSheetCategories.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { balanceSheetCategory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BalanceSheetCategories_Destroy([DataSourceRequest]DataSourceRequest request, BudgetSheetCategory balanceSheetCategory)
        {
            if (ModelState.IsValid)
            {
                var entity = new BudgetSheetCategory
                {
                    BalanceSheetCategoryId = balanceSheetCategory.BalanceSheetCategoryId,
                    BalanceSheetType = balanceSheetCategory.BalanceSheetType,
                    Name = balanceSheetCategory.Name,
                    OrgBranchName = balanceSheetCategory.OrgBranchName,
                    DateCreated = balanceSheetCategory.DateCreated,
                    LastModified = balanceSheetCategory.LastModified,
                    CreatedBy = balanceSheetCategory.CreatedBy,
                    ModifiedBy = balanceSheetCategory.ModifiedBy
                };

                db.BalanceSheetCategories.Attach(entity);
                db.BalanceSheetCategories.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { balanceSheetCategory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
