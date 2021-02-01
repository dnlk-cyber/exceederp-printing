﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
 using ExceedERP.Core.Domain.Finance.Report;
 using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Finance.Report.GAAP;
 using ExceedERP.DataAccess.Context;
 using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.Finance.Controllers.Setting
{
    public class RevenueByTypeController : Controller
    {
        private ExceedDbContext _db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.Count = _db.RevenueBySalesTypes.Count();
            return View();
        }

        public ActionResult RevenueBySalesTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RevenueBySalesType> revenueBySalesTypes = _db.RevenueBySalesTypes;
            DataSourceResult result = revenueBySalesTypes.ToDataSourceResult(request, revenueBySalesType => new {
                RevenueBySalesTypeId = revenueBySalesType.RevenueBySalesTypeId,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueBySalesTypes_Create([DataSourceRequest]DataSourceRequest request, RevenueBySalesType revenueBySalesType)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueBySalesType
                {
                };

                _db.RevenueBySalesTypes.Add(entity);
                _db.SaveChanges();
                revenueBySalesType.RevenueBySalesTypeId = entity.RevenueBySalesTypeId;

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance revenue by type", "Success", User.Identity.Name, $"created revenue by sales type {entity.RevenueBySalesTypeId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { revenueBySalesType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueBySalesTypes_Update([DataSourceRequest]DataSourceRequest request, RevenueBySalesType revenueBySalesType)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueBySalesType
                {
                    RevenueBySalesTypeId = revenueBySalesType.RevenueBySalesTypeId,
                };

                _db.RevenueBySalesTypes.Attach(entity);
                _db.Entry(entity).State = EntityState.Modified;
                _db.SaveChanges();

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Finance revenue by type", "Success", User.Identity.Name, $"updated revenue by sales type {entity.RevenueBySalesTypeId}", Modules.Finance);
                #endregion
            }

            return Json(new[] { revenueBySalesType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RevenueBySalesTypes_Destroy([DataSourceRequest]DataSourceRequest request, RevenueBySalesType revenueBySalesType)
        {
            if (ModelState.IsValid)
            {
                var entity = new RevenueBySalesType
                {
                    RevenueBySalesTypeId = revenueBySalesType.RevenueBySalesTypeId,
                };

                _db.RevenueBySalesTypes.Attach(entity);
                _db.RevenueBySalesTypes.Remove(entity);
                _db.SaveChanges();

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted revenue by type of sales {revenueBySalesType.RevenueBySalesTypeId}");
                #endregion
            }

            return Json(new[] { revenueBySalesType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            _db.Dispose();
            base.Dispose(disposing);
        }
    }
}
