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
    public class StrategicGoalsController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult StrategicGoals_Read(
            [DataSourceRequest] DataSourceRequest request,
            int strategicPlanId)
        {
            var goals = db.StrategicGoals.Where(x => x.StrategicPlanId == strategicPlanId);
            return Json(goals.ToDataSourceResult(request, goal => new
            {
                goal.StrategicGoalId,
                goal.StrategicPlanId,
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
        public ActionResult StrategicGoals_Create(
            [DataSourceRequest] DataSourceRequest request,
            StrategicGoal goal,
            int parentStrategicPlanId)
        {
            if (!db.StrategicPlans.Any(x => x.StrategicPlanId == parentStrategicPlanId))
            {
                ModelState.AddModelError("StrategicPlanId", "The selected strategic plan no longer exists.");
            }

            if (ModelState.IsValid)
            {
                var entity = new StrategicGoal
                {
                    StrategicPlanId = parentStrategicPlanId,
                    Name = goal.Name,
                    Description = goal.Description,
                    Remark = goal.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.StrategicGoals.Add(entity);
                db.SaveChanges();
                goal.StrategicGoalId = entity.StrategicGoalId;
                goal.StrategicPlanId = entity.StrategicPlanId;
            }

            return Json(new[] { goal }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StrategicGoals_Update(
            [DataSourceRequest] DataSourceRequest request,
            StrategicGoal goal)
        {
            var entity = db.StrategicGoals.Find(goal.StrategicGoalId);
            if (entity == null)
            {
                ModelState.AddModelError(string.Empty, "The selected strategic goal no longer exists.");
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
            }

            return Json(new[] { goal }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StrategicGoals_Destroy(
            [DataSourceRequest] DataSourceRequest request,
            StrategicGoal goal)
        {
            var entity = db.StrategicGoals.Find(goal.StrategicGoalId);
            if (entity != null)
            {
                db.StrategicGoals.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { goal }.ToDataSourceResult(request, ModelState));
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
