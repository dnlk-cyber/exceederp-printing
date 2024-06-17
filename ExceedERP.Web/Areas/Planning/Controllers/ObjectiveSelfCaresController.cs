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
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin)]
    public class ObjectiveSelfCaresController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            var departmentLists = db.MainCompanyStructures.Select(x => new
            {
                x.OrgStructureId,
                x.Name
            }).ToList();
            ViewData["departmentLists"] = departmentLists;
            ViewData["GlobalIndicators"] = db.GlobalIndicators.Select(x => new { x.GlobalIndicatorId, x.Indicator });

            return View();

        }
        
        public ActionResult ObjectiveSelfCares_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<Objective> objectives = db.Objectives;
            DataSourceResult result = objectives.ToDataSourceResult(request, objective => new {
                ObjectiveId = objective.ObjectiveId,
                OrgStructureId = objective.OrgStructureId,
                Name = objective.Name,
                Description = objective.Description,
                Remark = objective.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]   
        public ActionResult ObjectiveSelfCares_Create([DataSourceRequest]DataSourceRequest request, Objective objective)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(objective);
                db.Objectives.Add(entity);
                db.SaveChanges();
                objective.ObjectiveId = entity.ObjectiveId;
            }

            return Json(new[] { objective }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ObjectiveSelfCares_Update([DataSourceRequest]DataSourceRequest request, Objective objective)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(objective);
                db.Objectives.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { objective }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ObjectiveSelfCares_Destroy([DataSourceRequest]DataSourceRequest request, Objective objective)
        {
            var indicators = db.Indicators.Where(x => x.ObjectiveId == objective.ObjectiveId).ToList();
            if (indicators.Any())
            {
                ModelState.AddModelError("exists", "You Can't delete this! you have to delete child Data first");
            }
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(objective);
                db.Objectives.Attach(entity);
                db.Objectives.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { objective }.ToDataSourceResult(request, ModelState));
        }
        private Objective GetMappedObject(Objective objective)
        {
            var entity = new Objective
            {
                ObjectiveId = objective.ObjectiveId,
                OrgStructureId = objective.OrgStructureId,
                Name = objective.Name,
                Description = objective.Description,
                Remark = objective.Remark
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
