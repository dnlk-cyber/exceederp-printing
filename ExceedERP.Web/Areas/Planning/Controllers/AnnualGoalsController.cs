using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
    public class AnnualGoalsController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult AnnualGoals_Read(
            [DataSourceRequest] DataSourceRequest request,
            int strategicPlanId)
        {
            SynchronizeFiscalYears(strategicPlanId);

            var goals = db.AnnualGoals
                .Where(x => x.StrategicPlanId == strategicPlanId)
                .OrderBy(x => x.GlFiscalYear.DateFrom);

            return Json(goals.ToDataSourceResult(request, goal => new
            {
                goal.AnnualGoalId,
                goal.StrategicPlanId,
                goal.GlFiscalYearId,
                goal.Name,
                goal.Description,
                goal.Remark,
                goal.DateCreated,
                goal.CreatedBy,
                goal.LastModified,
                goal.ModifiedBy
            }));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AnnualGoals_Update(
            [DataSourceRequest] DataSourceRequest request,
            AnnualGoal goal)
        {
            var entity = db.AnnualGoals.Find(goal.AnnualGoalId);
            if (entity == null)
            {
                ModelState.AddModelError(string.Empty, "The selected annual goal no longer exists.");
            }

            if (ModelState.IsValid)
            {
                entity.Name = goal.Name;
                entity.Description = goal.Description;
                entity.Remark = goal.Remark;
                entity.LastModified = DateTime.Now;
                entity.ModifiedBy = User.Identity.Name;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                goal.StrategicPlanId = entity.StrategicPlanId;
                goal.GlFiscalYearId = entity.GlFiscalYearId;
            }

            return Json(new[] { goal }.ToDataSourceResult(request, ModelState));
        }

        private void SynchronizeFiscalYears(int strategicPlanId)
        {
            var strategicPlan = db.StrategicPlans.Find(strategicPlanId);
            if (strategicPlan == null)
            {
                return;
            }

            var fromDate = db.GLFiscalYears
                .Where(x => x.GlFiscalYearId == strategicPlan.FromGlFiscalYearId)
                .Select(x => (DateTime?)x.DateFrom)
                .FirstOrDefault();
            var toDate = db.GLFiscalYears
                .Where(x => x.GlFiscalYearId == strategicPlan.ToGlFiscalYearId)
                .Select(x => (DateTime?)x.DateFrom)
                .FirstOrDefault();

            if (!fromDate.HasValue || !toDate.HasValue)
            {
                return;
            }

            var fiscalYearIds = db.GLFiscalYears
                .Where(x => x.DateFrom >= fromDate.Value && x.DateFrom <= toDate.Value)
                .Select(x => x.GlFiscalYearId)
                .ToList();
            var existingGoals = db.AnnualGoals
                .Where(x => x.StrategicPlanId == strategicPlanId)
                .ToList();
            var existingFiscalYearIds = existingGoals.Select(x => x.GlFiscalYearId).ToList();

            foreach (var fiscalYearId in fiscalYearIds.Except(existingFiscalYearIds))
            {
                db.AnnualGoals.Add(new AnnualGoal
                {
                    StrategicPlanId = strategicPlanId,
                    GlFiscalYearId = fiscalYearId,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }

            db.AnnualGoals.RemoveRange(existingGoals.Where(x => !fiscalYearIds.Contains(x.GlFiscalYearId)));
            db.SaveChanges();
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
