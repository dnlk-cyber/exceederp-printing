using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin,
                     HrmConstants.HRDepartmentHead,
                     HrmConstants.HREmployeeSelfService)]
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
            ViewData["GlobalIndicators"] = PlanningSelfCareHelper.GetDepartmentGlobalIndicatorList(db, User.Identity.Name);
            var uoms = db.UoMs.Select(x => x.UoMName).ToList();
            var existingUnits = db.Indicators.Where(x => !string.IsNullOrEmpty(x.Unit)).Select(x => x.Unit).Distinct().ToList();
            ViewData["UoMs"] = uoms.Union(existingUnits).Where(x => !string.IsNullOrWhiteSpace(x)).OrderBy(x => x).Select(x => new
            {
                Name = x,
                Code = x
            }).ToList();

            return View();

        }
        
        public ActionResult ObjectiveSelfCares_Read([DataSourceRequest]DataSourceRequest request)
        {
            var employeeId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            var workUnitId = EmployeeHelper.GetEmployeeWorkUnitId(employeeId);
            IQueryable<Objective> objectives = db.Objectives.Where(x=>x.OrgStructureId==workUnitId);
            DataSourceResult result = objectives.ToDataSourceResult(request, objective => new {
                ObjectiveId = objective.ObjectiveId,
                OrgStructureId = objective.OrgStructureId,
                Name = objective.Name,
                Weight = objective.Weight,
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
