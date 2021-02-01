﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Dashboard;
 using ExceedERP.Core.Domain.Finance;
 using ExceedERP.DataAccess.Context;
 using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Finance.Controllers
{
     [AuthorizeRoles(FinanceConstants.FinanceAdmin)]
    public class FinanceDashboardController : Controller
    {
        private readonly ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult FinanceDashboards_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<FinanceDashboard> financedashboards = _db.FinanceDashboards;
            DataSourceResult result = financedashboards.ToDataSourceResult(request, financeDashboard => new {
                FinanceDashboardId = financeDashboard.FinanceDashboardId,
                Category = financeDashboard.Category,
                Name = financeDashboard.Name
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinanceDashboards_Create([DataSourceRequest]DataSourceRequest request, FinanceDashboard financeDashboard)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinanceDashboard
                {
                    Category = financeDashboard.Category,
                    Name = financeDashboard.Name
                };

                _db.FinanceDashboards.Add(entity);
                _db.SaveChanges();
                financeDashboard.FinanceDashboardId = entity.FinanceDashboardId;
            }

            return Json(new[] { financeDashboard }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinanceDashboards_Update([DataSourceRequest]DataSourceRequest request, FinanceDashboard financeDashboard)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinanceDashboard
                {
                    FinanceDashboardId = financeDashboard.FinanceDashboardId,
                    Category = financeDashboard.Category,
                    Name = financeDashboard.Name
                };

                _db.FinanceDashboards.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();
            }

            return Json(new[] { financeDashboard }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult FinanceDashboards_Destroy([DataSourceRequest]DataSourceRequest request, FinanceDashboard financeDashboard)
        {
            if (ModelState.IsValid)
            {
                var entity = new FinanceDashboard
                {
                    FinanceDashboardId = financeDashboard.FinanceDashboardId,
                    Category = financeDashboard.Category,
                    Name = financeDashboard.Name
                };

                _db.FinanceDashboards.Attach(entity);
                _db.FinanceDashboards.Remove(entity);
                _db.SaveChanges();
            }

            return Json(new[] { financeDashboard }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
