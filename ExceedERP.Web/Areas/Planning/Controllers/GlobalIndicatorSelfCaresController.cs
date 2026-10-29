using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin,
                    PlanningRoles.Planning_User,
                    HrmConstants.HRDepartmentHead,
                    HrmConstants.HREmployeeSelfService)]
    public class GlobalIndicatorSelfCaresController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["departmentLists"] = db.MainCompanyStructures.Select(x => new
            {
                x.OrgStructureId,
                x.Name
            }).ToList();

            return View();
        }

        public ActionResult GlobalIndicatorSelfCares_Read([DataSourceRequest]DataSourceRequest request)
        {
            var departmentId = PlanningSelfCareHelper.GetCurrentDepartmentId(User.Identity.Name);
            var globalIndicators = PlanningSelfCareHelper.GetDepartmentGlobalIndicators(db, departmentId);

            DataSourceResult result = globalIndicators.ToDataSourceResult(request, globalIndicator => new
            {
                GlobalIndicatorId = globalIndicator.GlobalIndicatorId,
                OrgStructureId = globalIndicator.OrgStructureId,
                Indicator = globalIndicator.Indicator,
                IsActive = globalIndicator.IsActive,
                Description = globalIndicator.Description,
                CreatedBy = globalIndicator.CreatedBy,
                DateCreated = globalIndicator.DateCreated,
                LastModified = globalIndicator.LastModified,
                ModifiedBy = globalIndicator.ModifiedBy,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalIndicatorSelfCares_Create([DataSourceRequest]DataSourceRequest request, GlobalIndicator globalIndicator)
        {
            var departmentId = PlanningSelfCareHelper.GetCurrentDepartmentId(User.Identity.Name);
            if (departmentId == 0)
                ModelState.AddModelError(string.Empty, "Your user is not linked to a current department.");

            if (ModelState.IsValid)
            {
                globalIndicator.OrgStructureId = departmentId;
                globalIndicator.CreatedBy = User.Identity.Name;
                globalIndicator.DateCreated = DateTime.Now;
                globalIndicator.IsActive = true;
                var entity = GetMappedObject(globalIndicator);
                db.GlobalIndicators.Add(entity);
                db.SaveChanges();
                globalIndicator.GlobalIndicatorId = entity.GlobalIndicatorId;

                UserHelper.OperationLog(Request.UserHostAddress, "SelfCare Indicator", "Success", User.Identity.Name, " is Created By  " + User.Identity.Name, Modules.Planning);
            }

            return Json(new[] { globalIndicator }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalIndicatorSelfCares_Update([DataSourceRequest]DataSourceRequest request, GlobalIndicator globalIndicator)
        {
            var departmentId = PlanningSelfCareHelper.GetCurrentDepartmentId(User.Identity.Name);
            var entity = db.GlobalIndicators.Find(globalIndicator.GlobalIndicatorId);

            if (departmentId == 0)
                ModelState.AddModelError(string.Empty, "Your user is not linked to a current department.");
            else if (entity == null || entity.OrgStructureId != departmentId)
                ModelState.AddModelError(string.Empty, "You can update only indicators registered for your department.");

            if (ModelState.IsValid)
            {
                entity.Indicator = globalIndicator.Indicator;
                entity.Description = globalIndicator.Description;
                entity.LastModified = DateTime.Now;
                entity.ModifiedBy = User.Identity.Name;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                UserHelper.OperationLog(Request.UserHostAddress, "SelfCare Indicator", "Success", User.Identity.Name, " is Updated By  " + User.Identity.Name, Modules.Planning);
            }

            return Json(new[] { globalIndicator }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult GlobalIndicatorSelfCares_Destroy([DataSourceRequest]DataSourceRequest request, GlobalIndicator globalIndicator)
        {
            var departmentId = PlanningSelfCareHelper.GetCurrentDepartmentId(User.Identity.Name);
            var entity = db.GlobalIndicators.Find(globalIndicator.GlobalIndicatorId);

            if (departmentId == 0)
                ModelState.AddModelError(string.Empty, "Your user is not linked to a current department.");
            else if (entity == null || entity.OrgStructureId != departmentId)
                ModelState.AddModelError(string.Empty, "You can delete only indicators registered for your department.");
            else if (db.Indicators.Any(x => x.GlobalIndicatorId == entity.GlobalIndicatorId))
                ModelState.AddModelError("exist", "You cannot delete this item because it is used in indicators. Please delete it from those indicators first.");

            if (ModelState.IsValid)
            {
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "     SelfCare Indicator deleted  " + entity.Indicator);
                db.GlobalIndicators.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { globalIndicator }.ToDataSourceResult(request, ModelState));
        }

        private GlobalIndicator GetMappedObject(GlobalIndicator globalIndicator)
        {
            var entity = new GlobalIndicator
            {
                GlobalIndicatorId = globalIndicator.GlobalIndicatorId,
                OrgStructureId = globalIndicator.OrgStructureId,
                Indicator = globalIndicator.Indicator,
                IsActive = globalIndicator.IsActive,
                Description = globalIndicator.Description,
                CreatedBy = globalIndicator.CreatedBy,
                DateCreated = globalIndicator.DateCreated,
                LastModified = globalIndicator.LastModified,
                ModifiedBy = globalIndicator.ModifiedBy,
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