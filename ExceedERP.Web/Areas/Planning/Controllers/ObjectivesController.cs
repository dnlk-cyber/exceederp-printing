using System;
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
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
    public class ObjectivesController : Controller
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
            var uoms = db.UoMs.Select(x => x.UoMName).ToList();
            var existingUnits = db.Indicators.Where(x => !string.IsNullOrEmpty(x.Unit)).Select(x => x.Unit).Distinct().ToList();
            ViewData["UoMs"] = uoms.Union(existingUnits).Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x).Select(x => new
            {
                Name = x,
                Code = x
            }).ToList();

            return View();
        }

        public ActionResult Objectives_Read([DataSourceRequest]DataSourceRequest request, int? strategicPlanId)
        {
            IQueryable<Objective> objectives = db.Objectives;
            if (strategicPlanId.HasValue)
            {
                objectives = objectives.Where(x => x.StrategicPlanId == strategicPlanId.Value);
            }

            DataSourceResult result = objectives.ToDataSourceResult(request, objective => new {
                ObjectiveId = objective.ObjectiveId,
                StrategicPlanId = objective.StrategicPlanId,
                OrgStructureId = objective.OrgStructureId,
                Name = objective.Name,
                Weight = objective.Weight,
                Description = objective.Description,
                Remark = objective.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Objectives_Create([DataSourceRequest]DataSourceRequest request, Objective objective)
        {
            return CreateObjective(request, objective, objective.StrategicPlanId);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StrategicPlanObjectives_Create(
            [DataSourceRequest] DataSourceRequest request,
            Objective objective,
            int parentStrategicPlanId)
        {
            return CreateObjective(request, objective, parentStrategicPlanId);
        }

        private ActionResult CreateObjective(
            DataSourceRequest request,
            Objective objective,
            int? strategicPlanId)
        {
            if (strategicPlanId.HasValue && !db.StrategicPlans.Any(x => x.StrategicPlanId == strategicPlanId.Value))
            {
                ModelState.AddModelError("StrategicPlanId", "The selected strategic plan no longer exists.");
            }

            if (ModelState.IsValid)
            {
                objective.StrategicPlanId = strategicPlanId;
                var entity = GetMappedObject(objective);
                db.Objectives.Add(entity);
                db.SaveChanges();
                objective.ObjectiveId = entity.ObjectiveId;
                objective.StrategicPlanId = entity.StrategicPlanId;
            }

            return Json(new[] { objective }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Objectives_Update([DataSourceRequest]DataSourceRequest request, Objective objective)
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
        public ActionResult Objectives_Destroy([DataSourceRequest]DataSourceRequest request, Objective objective)
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
                StrategicPlanId = objective.StrategicPlanId,
                OrgStructureId = objective.OrgStructureId,
                Name = objective.Name,
                Weight = objective.Weight,
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
