﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin)]
    public class ExcutivePlanKPIsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var departmentLists = db.MainCompanyStructures.Select(x => new
            {
                x.OrgStructureId,
                x.Name
            }).ToList();
            var indicators = db.Indicators.Select(I => new
            {
                I.IndicatorId,
                I.Name,

            }).ToList();
            ViewData["Indicators"] = indicators;

            ViewData["departmentLists"] = departmentLists;
            return View();
        }

        public ActionResult ExcutivePlanKPIs_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ExcutivePlanKPI> excutivePlans = db.ExcutivePlanKPIs;
            DataSourceResult result = excutivePlans.ToDataSourceResult(request, excutivePlan => new {
                ExcutivePlanKPIId = excutivePlan.ExcutivePlanKPIId,
                IndicatorId = excutivePlan.IndicatorId,
                DateCreated = excutivePlan.DateCreated,
                CreatedBy = excutivePlan.CreatedBy,
                LastModified = excutivePlan.LastModified,
                ModifiedBy = excutivePlan.ModifiedBy,

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExcutivePlanKPIs_Create([DataSourceRequest]DataSourceRequest request, ExcutivePlanKPI excutivePlan)
        {
            if( db.ExcutivePlanKPIs.Any(x => x.IndicatorId == excutivePlan.IndicatorId))
            {
                ModelState.AddModelError("", "Registered already");
            }

            if (ModelState.IsValid)
            {
                excutivePlan.DateCreated = DateTime.Now;
                excutivePlan.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(excutivePlan);
                db.ExcutivePlanKPIs.Add(entity);
                db.SaveChanges();
                excutivePlan.ExcutivePlanKPIId = entity.ExcutivePlanKPIId;
            }

            return Json(new[] { excutivePlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExcutivePlanKPIs_Update([DataSourceRequest]DataSourceRequest request, ExcutivePlanKPI excutivePlan)
        {
            if (db.ExcutivePlanKPIs.Any(x => x.IndicatorId == excutivePlan.IndicatorId))
            {
                ModelState.AddModelError("", "Registered already");
            }
            if (ModelState.IsValid)
            {
                excutivePlan.LastModified = DateTime.Now;
                excutivePlan.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(excutivePlan);
                db.ExcutivePlanKPIs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { excutivePlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExcutivePlanKPIs_Destroy([DataSourceRequest]DataSourceRequest request, ExcutivePlanKPI excutivePlan)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(excutivePlan);
                db.ExcutivePlanKPIs.Attach(entity);
                db.ExcutivePlanKPIs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { excutivePlan }.ToDataSourceResult(request, ModelState));
        }
        private ExcutivePlanKPI GetMappedObject(ExcutivePlanKPI excutivePlan)
        {
            var entity = new ExcutivePlanKPI
            {
                ExcutivePlanKPIId = excutivePlan.ExcutivePlanKPIId,
                IndicatorId = excutivePlan.IndicatorId,
                DateCreated = excutivePlan.DateCreated,
                CreatedBy = excutivePlan.CreatedBy,
                LastModified = excutivePlan.LastModified,
                ModifiedBy = excutivePlan.ModifiedBy,
               
            };

            return entity;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
