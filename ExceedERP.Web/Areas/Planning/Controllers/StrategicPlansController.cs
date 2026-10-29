using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
    public class StrategicPlansController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["GlFiscalYears"] = db.GLFiscalYears
                .OrderBy(x => x.DateFrom)
                .Select(x => new
                {
                    x.GlFiscalYearId,
                    x.Name
                })
                .ToList();
            ViewData["departmentLists"] = db.MainCompanyStructures
                .Select(x => new
                {
                    x.OrgStructureId,
                    x.Name
                })
                .ToList();

            return View();
        }

        public ActionResult StrategicPlans_Read([DataSourceRequest] DataSourceRequest request)
        {
            var strategicPlans = db.StrategicPlans;
            var result = strategicPlans.ToDataSourceResult(request, strategicPlan => new
            {
                strategicPlan.StrategicPlanId,
                strategicPlan.FromGlFiscalYearId,
                strategicPlan.ToGlFiscalYearId,
                strategicPlan.IsDepartnental,
                strategicPlan.IsPersonal,
                strategicPlan.Description,
                strategicPlan.DateCreated,
                strategicPlan.CreatedBy,
                strategicPlan.LastModified,
                strategicPlan.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StrategicPlans_Create(
            [DataSourceRequest] DataSourceRequest request,
            StrategicPlan strategicPlan)
        {
            ValidateStrategicPlan(strategicPlan);

            if (ModelState.IsValid)
            {
                strategicPlan.IsDepartnental = true;
                strategicPlan.DateCreated = DateTime.Now.Date;
                strategicPlan.CreatedBy = User.Identity.Name;

                var entity = GetMappedObject(strategicPlan);
                db.StrategicPlans.Add(entity);
                db.SaveChanges();
                strategicPlan.StrategicPlanId = entity.StrategicPlanId;
                SynchronizeAnnualGoals(entity);

                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Strategic Plan",
                    "Success",
                    User.Identity.Name,
                    GetFiscalYearRange(strategicPlan) + " is created by " + User.Identity.Name,
                    Modules.Planning);
            }

            return Json(new[] { strategicPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StrategicPlans_Update(
            [DataSourceRequest] DataSourceRequest request,
            StrategicPlan strategicPlan)
        {
            ValidateStrategicPlan(strategicPlan);

            if (ModelState.IsValid)
            {
                strategicPlan.LastModified = DateTime.Now.Date;
                strategicPlan.ModifiedBy = User.Identity.Name;

                var entity = GetMappedObject(strategicPlan);
                db.StrategicPlans.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                SynchronizeAnnualGoals(entity);

                UserHelper.OperationLog(
                    Request.UserHostAddress,
                    "Strategic Plan",
                    "Success",
                    User.Identity.Name,
                    GetFiscalYearRange(strategicPlan) + " is updated by " + User.Identity.Name,
                    Modules.Planning);
            }

            return Json(new[] { strategicPlan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StrategicPlans_Destroy(
            [DataSourceRequest] DataSourceRequest request,
            StrategicPlan strategicPlan)
        {
            var entity = db.StrategicPlans.Find(strategicPlan.StrategicPlanId);
            if (entity != null)
            {
                if (db.Objectives.Any(x => x.StrategicPlanId == entity.StrategicPlanId))
                {
                    ModelState.AddModelError("exists", "You can't delete this strategic plan before deleting its objectives.");
                    return Json(new[] { strategicPlan }.ToDataSourceResult(request, ModelState));
                }

                if (db.StrategicGoals.Any(x => x.StrategicPlanId == entity.StrategicPlanId))
                {
                    ModelState.AddModelError("exists", "You can't delete this strategic plan before deleting its strategic goals.");
                    return Json(new[] { strategicPlan }.ToDataSourceResult(request, ModelState));
                }

                UserHelper.DeleteLog(
                    Request.UserHostAddress,
                    "Delete",
                    "Success",
                    User.Identity.Name,
                    "Strategic plan " + GetFiscalYearRange(entity));

                var annualGoals = db.AnnualGoals
                    .Where(x => x.StrategicPlanId == entity.StrategicPlanId)
                    .ToList();
                db.AnnualGoals.RemoveRange(annualGoals);
                db.StrategicPlans.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { strategicPlan }.ToDataSourceResult(request, ModelState));
        }

        private void ValidateStrategicPlan(StrategicPlan strategicPlan)
        {
            var fromFiscalYear = db.GLFiscalYears.Find(strategicPlan.FromGlFiscalYearId);
            var toFiscalYear = db.GLFiscalYears.Find(strategicPlan.ToGlFiscalYearId);

            if (fromFiscalYear == null)
            {
                ModelState.AddModelError("FromGlFiscalYearId", "Please select a valid starting fiscal year.");
            }

            if (toFiscalYear == null)
            {
                ModelState.AddModelError("ToGlFiscalYearId", "Please select a valid ending fiscal year.");
            }

            if (fromFiscalYear == null || toFiscalYear == null)
            {
                return;
            }

            if (toFiscalYear.DateFrom <= fromFiscalYear.DateFrom)
            {
                ModelState.AddModelError("ToGlFiscalYearId", "End fiscal year must be after the starting fiscal year.");
                return;
            }

            var fiscalYearCount = db.GLFiscalYears.Count(x =>
                x.DateFrom >= fromFiscalYear.DateFrom && x.DateFrom <= toFiscalYear.DateFrom);

            if (fiscalYearCount < 3)
            {
                ModelState.AddModelError("ToGlFiscalYearId", "A strategic plan must cover at least three fiscal years.");
            }

            var duplicateExists = db.StrategicPlans.Any(x =>
                x.FromGlFiscalYearId == strategicPlan.FromGlFiscalYearId &&
                x.ToGlFiscalYearId == strategicPlan.ToGlFiscalYearId &&
                x.StrategicPlanId != strategicPlan.StrategicPlanId);

            if (duplicateExists)
            {
                ModelState.AddModelError("exists", "This fiscal-year range is already registered.");
            }
        }

        private string GetFiscalYearRange(StrategicPlan strategicPlan)
        {
            var fromName = db.GLFiscalYears
                .Where(x => x.GlFiscalYearId == strategicPlan.FromGlFiscalYearId)
                .Select(x => x.Name)
                .FirstOrDefault();
            var toName = db.GLFiscalYears
                .Where(x => x.GlFiscalYearId == strategicPlan.ToGlFiscalYearId)
                .Select(x => x.Name)
                .FirstOrDefault();

            return (fromName ?? strategicPlan.FromGlFiscalYearId.ToString()) + " - " +
                   (toName ?? strategicPlan.ToGlFiscalYearId.ToString());
        }

        private void SynchronizeAnnualGoals(StrategicPlan strategicPlan)
        {
            var fromDate = db.GLFiscalYears
                .Where(x => x.GlFiscalYearId == strategicPlan.FromGlFiscalYearId)
                .Select(x => x.DateFrom)
                .First();
            var toDate = db.GLFiscalYears
                .Where(x => x.GlFiscalYearId == strategicPlan.ToGlFiscalYearId)
                .Select(x => x.DateFrom)
                .First();
            var fiscalYearIds = db.GLFiscalYears
                .Where(x => x.DateFrom >= fromDate && x.DateFrom <= toDate)
                .Select(x => x.GlFiscalYearId)
                .ToList();
            var existingGoals = db.AnnualGoals
                .Where(x => x.StrategicPlanId == strategicPlan.StrategicPlanId)
                .ToList();
            var existingFiscalYearIds = existingGoals.Select(x => x.GlFiscalYearId).ToList();

            foreach (var fiscalYearId in fiscalYearIds.Except(existingFiscalYearIds))
            {
                db.AnnualGoals.Add(new AnnualGoal
                {
                    StrategicPlanId = strategicPlan.StrategicPlanId,
                    GlFiscalYearId = fiscalYearId,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }

            db.AnnualGoals.RemoveRange(
                existingGoals.Where(x => !fiscalYearIds.Contains(x.GlFiscalYearId)));
            db.SaveChanges();
        }

        private static StrategicPlan GetMappedObject(StrategicPlan strategicPlan)
        {
            return new StrategicPlan
            {
                StrategicPlanId = strategicPlan.StrategicPlanId,
                FromGlFiscalYearId = strategicPlan.FromGlFiscalYearId,
                ToGlFiscalYearId = strategicPlan.ToGlFiscalYearId,
                IsDepartnental = strategicPlan.IsDepartnental,
                IsPersonal = strategicPlan.IsPersonal,
                Description = strategicPlan.Description,
                DateCreated = strategicPlan.DateCreated,
                CreatedBy = strategicPlan.CreatedBy,
                LastModified = strategicPlan.LastModified,
                ModifiedBy = strategicPlan.ModifiedBy
            };
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }
}
