using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Performance
{
    public class PerformanceModifierBudgetUtilizationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Modifier_Read([DataSourceRequest]DataSourceRequest request, int pIdReadBUt)
        {
            IQueryable<PerformanceModifierBudgetUtilization> modifiers = db.PerformanceModifierBudgetUtilization.Where(x => x.MonthlyProjectPerformanceModifierId == pIdReadBUt);
            DataSourceResult result = modifiers.ToDataSourceResult(request, modifier => new
            {
                PerformanceModifierBudgetUtilizationId = modifier.PerformanceModifierBudgetUtilizationId,
                MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                RequestedBudget = modifier.RequestedBudget,
                UtilizedBudget = modifier.UtilizedBudget,
                Deviation = modifier.Deviation,
                Points = modifier.Points,

                OrgBranchName = modifier.OrgBranchName,
                DateCreated = modifier.DateCreated,
                LastModified = modifier.LastModified,
                CreatedBy = modifier.CreatedBy,
                ModifiedBy = modifier.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Create([DataSourceRequest]DataSourceRequest request, PerformanceModifierBudgetUtilization modifier, int pIdCreateBUt)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierBudgetUtilization
                {
                    PerformanceModifierBudgetUtilizationId = modifier.PerformanceModifierBudgetUtilizationId,
                    MonthlyProjectPerformanceModifierId = pIdCreateBUt,
                    RequestedBudget = modifier.RequestedBudget,
                    UtilizedBudget = modifier.UtilizedBudget,
                    Deviation = this.CalculateDeviation(modifier),
                    Points = modifier.Points,

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = modifier.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = modifier.ModifiedBy
                };
                entity.Points = entity.Deviation > 15 ? -3 : 0;

                db.PerformanceModifierBudgetUtilization.Add(entity);
                db.SaveChanges();
                modifier.PerformanceModifierBudgetUtilizationId = entity.PerformanceModifierBudgetUtilizationId;

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifier(entity.MonthlyProjectPerformanceModifierId);

            }
            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Update([DataSourceRequest]DataSourceRequest request, PerformanceModifierBudgetUtilization modifier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierBudgetUtilization
                {
                    PerformanceModifierBudgetUtilizationId = modifier.PerformanceModifierBudgetUtilizationId,
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    RequestedBudget = modifier.RequestedBudget,
                    UtilizedBudget = modifier.UtilizedBudget,
                    Deviation = this.CalculateDeviation(modifier),
                    Points = modifier.Points,


                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.Points = entity.Deviation > 15 ? -3 : 0;

                db.PerformanceModifierBudgetUtilization.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifier(entity.MonthlyProjectPerformanceModifierId);

            }

            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Destroy([DataSourceRequest]DataSourceRequest request, PerformanceModifierBudgetUtilization modifier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierBudgetUtilization
                {
                    PerformanceModifierBudgetUtilizationId = modifier.PerformanceModifierBudgetUtilizationId,
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    RequestedBudget = modifier.RequestedBudget,
                    UtilizedBudget = modifier.UtilizedBudget,
                    Deviation = modifier.Deviation,
                    Points = modifier.Points,

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = modifier.LastModified,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = modifier.ModifiedBy
                };

                db.PerformanceModifierBudgetUtilization.Attach(entity);
                db.PerformanceModifierBudgetUtilization.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifier(entity.MonthlyProjectPerformanceModifierId);

            }

            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }
        private decimal CalculateDeviation(PerformanceModifierBudgetUtilization model)
        {
            var dv = new decimal();
            if (model.RequestedBudget > 0 && model.UtilizedBudget > 0)
            {
                dv = (Math.Abs((model.RequestedBudget - model.UtilizedBudget) / model.RequestedBudget)) * 100;
            }
            return dv;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}