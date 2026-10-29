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
    public class QuickFixPlansController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateLookups();
            return View();
        }

        public ActionResult Plans_Read(int? glFiscalYearId)
        {
            if (!glFiscalYearId.HasValue)
            {
                return Json(Enumerable.Empty<object>(), JsonRequestBehavior.AllowGet);
            }

            var plans = db.Plans
                .Where(x => x.GlFiscalYearId == glFiscalYearId.Value)
                .OrderBy(x => x.Description)
                .Select(x => new
                {
                    x.PlanId,
                    Name = x.Description
                })
                .ToList();

            var planIds = plans.Select(x => x.PlanId).ToList();
            var annualPlans = db.PlanningDetails
                .Where(x => planIds.Contains(x.GetBSCPlan.PlanId))
                .GroupBy(x => x.GetBSCPlan.PlanId)
                .Select(x => new
                {
                    PlanId = x.Key,
                    AnnualPlan = x.Sum(y => y.Plan)
                })
                .ToDictionary(x => x.PlanId, x => x.AnnualPlan);

            var items = plans.Select(x => new
            {
                x.PlanId,
                x.Name,
                AnnualPlan = annualPlans.ContainsKey(x.PlanId) ? annualPlans[x.PlanId] : 0
            });

            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public ActionResult QuickFixPlans_Read([DataSourceRequest] DataSourceRequest request)
        {
            var result = db.QuickFixPlans.ToDataSourceResult(request, item => new
            {
                item.QuickFixPlanId,
                item.GlFiscalYearId,
                item.PlanId,
                item.AnnualPlan,
                item.UpToQuarter,
                item.PlanUpToQuarter,
                item.Actual,
                item.PreviousPlanRemainingQuarter,
                item.RevisedPlanRemainingQuarter,
                item.DateCreated,
                item.CreatedBy,
                item.LastModified,
                item.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuickFixPlans_Create(
            [DataSourceRequest] DataSourceRequest request,
            QuickFixPlan item)
        {
            var annualPlan = ValidateQuickFixPlan(item);

            if (ModelState.IsValid)
            {
                var entity = new QuickFixPlan
                {
                    GlFiscalYearId = item.GlFiscalYearId,
                    PlanId = item.PlanId,
                    AnnualPlan = annualPlan,
                    UpToQuarter = item.UpToQuarter,
                    PlanUpToQuarter = item.PlanUpToQuarter,
                    Actual = item.Actual,
                    PreviousPlanRemainingQuarter = item.PreviousPlanRemainingQuarter,
                    RevisedPlanRemainingQuarter = item.RevisedPlanRemainingQuarter,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.QuickFixPlans.Add(entity);
                db.SaveChanges();

                item.QuickFixPlanId = entity.QuickFixPlanId;
                item.AnnualPlan = entity.AnnualPlan;
                item.DateCreated = entity.DateCreated;
                item.CreatedBy = entity.CreatedBy;

                LogOperation(entity.QuickFixPlanId, "created");
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QuickFixPlans_Update(
            [DataSourceRequest] DataSourceRequest request,
            QuickFixPlan item)
        {
            var annualPlan = ValidateQuickFixPlan(item);

            if (ModelState.IsValid)
            {
                var entity = db.QuickFixPlans.Find(item.QuickFixPlanId);
                if (entity == null)
                {
                    ModelState.AddModelError("", "The selected quick-fix plan no longer exists.");
                }
                else
                {
                    entity.GlFiscalYearId = item.GlFiscalYearId;
                    entity.PlanId = item.PlanId;
                    entity.AnnualPlan = annualPlan;
                    entity.UpToQuarter = item.UpToQuarter;
                    entity.PlanUpToQuarter = item.PlanUpToQuarter;
                    entity.Actual = item.Actual;
                    entity.PreviousPlanRemainingQuarter = item.PreviousPlanRemainingQuarter;
                    entity.RevisedPlanRemainingQuarter = item.RevisedPlanRemainingQuarter;
                    entity.LastModified = DateTime.Now;
                    entity.ModifiedBy = User.Identity.Name;

                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    item.AnnualPlan = entity.AnnualPlan;
                    item.LastModified = entity.LastModified;
                    item.ModifiedBy = entity.ModifiedBy;

                    LogOperation(entity.QuickFixPlanId, "updated");
                }
            }

            return Json(new[] { item }.ToDataSourceResult(request, ModelState));
        }

        private decimal ValidateQuickFixPlan(QuickFixPlan item)
        {
            var plan = db.Plans.FirstOrDefault(x => x.PlanId == item.PlanId);

            if (!db.GLFiscalYears.Any(x => x.GlFiscalYearId == item.GlFiscalYearId))
            {
                ModelState.AddModelError("GlFiscalYearId", "Please select a valid fiscal year.");
            }

            if (plan == null)
            {
                ModelState.AddModelError("PlanId", "Please select a valid annual plan.");
                return 0;
            }

            if (plan.GlFiscalYearId != item.GlFiscalYearId)
            {
                ModelState.AddModelError(
                    "PlanId",
                    "The selected annual plan does not belong to the selected fiscal year.");
            }

            var duplicateExists = db.QuickFixPlans.Any(x =>
                x.QuickFixPlanId != item.QuickFixPlanId &&
                x.GlFiscalYearId == item.GlFiscalYearId &&
                x.PlanId == item.PlanId &&
                x.UpToQuarter == item.UpToQuarter);

            if (duplicateExists)
            {
                ModelState.AddModelError(
                    "PlanId",
                    "A quick-fix plan already exists for this activity and up-to quarter.");
            }

            var bscPlanIds = db.BSCPlans
                .Where(x => x.PlanId == item.PlanId)
                .Select(x => x.BSCPlanId);

            return db.PlanningDetails
                .Where(x => bscPlanIds.Contains(x.BSCPlanId))
                .Select(x => (decimal?)x.Plan)
                .Sum() ?? 0;
        }

        private void PopulateLookups()
        {
            ViewData["FiscalYears"] = db.GLFiscalYears
                .OrderByDescending(x => x.DateFrom)
                .Select(x => new { x.GlFiscalYearId, x.Name })
                .ToList();

            ViewData["Plans"] = db.Plans
                .OrderBy(x => x.Description)
                .Select(x => new
                {
                    x.PlanId,
                    Name = x.Description
                })
                .ToList();

            ViewData["Quarters"] = new[]
            {
                new { Value = 1, Text = "Quarter 1" },
                new { Value = 2, Text = "Quarter 2" },
                new { Value = 3, Text = "Quarter 3" }
            };
        }

        private void LogOperation(int id, string action)
        {
            UserHelper.OperationLog(
                Request.UserHostAddress,
                "Quick-Fix Plan",
                "Success",
                User.Identity.Name,
                "Quick-fix plan " + id + " was " + action + " by " + User.Identity.Name,
                Modules.Planning);
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
