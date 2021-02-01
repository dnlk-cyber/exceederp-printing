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
    public class PerformanceModifierBudgetRequestController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Modifier_Read([DataSourceRequest]DataSourceRequest request, int pIdReadBReq)
        {
            IQueryable<PerformanceModifierBudgetRequest> modifiers = db.PerformanceModifierBudgetRequest.Where(x => x.MonthlyProjectPerformanceModifierId == pIdReadBReq);
            DataSourceResult result = modifiers.ToDataSourceResult(request, modifier => new
            {
                PerformanceModifierBudgetRequestId = modifier.PerformanceModifierBudgetRequestId,
                MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                ForecastedBudget = modifier.ForecastedBudget,
                RequestedBudget = modifier.RequestedBudget,
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
        public ActionResult Modifier_Create([DataSourceRequest]DataSourceRequest request, PerformanceModifierBudgetRequest modifier, int pIdCreateBReq)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierBudgetRequest
                {
                    //PerformanceModifierBudgetRequestId = modifier.PerformanceModifierBudgetRequestId,
                    MonthlyProjectPerformanceModifierId = pIdCreateBReq,
                    ForecastedBudget = modifier.ForecastedBudget,
                    RequestedBudget = modifier.RequestedBudget,
                    Deviation = this.CalculateDeviation(modifier),
                    Points = modifier.Points,

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = modifier.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = modifier.ModifiedBy
                };
                entity.Points = entity.Deviation > 15 ? -2 : 0;

                db.PerformanceModifierBudgetRequest.Add(entity);
                db.SaveChanges();
                modifier.PerformanceModifierBudgetRequestId = entity.PerformanceModifierBudgetRequestId;

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifier(entity.MonthlyProjectPerformanceModifierId);

            }
            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Update([DataSourceRequest]DataSourceRequest request, PerformanceModifierBudgetRequest modifier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierBudgetRequest
                {
                    PerformanceModifierBudgetRequestId = modifier.PerformanceModifierBudgetRequestId,
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    ForecastedBudget = modifier.ForecastedBudget,
                    RequestedBudget = modifier.RequestedBudget,
                    Deviation = this.CalculateDeviation(modifier),
                    Points = modifier.Points,


                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.Points = entity.Deviation > 15 ? -2 : 0;


                db.PerformanceModifierBudgetRequest.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifier(entity.MonthlyProjectPerformanceModifierId);

            }

            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Destroy([DataSourceRequest]DataSourceRequest request, PerformanceModifierBudgetRequest modifier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierBudgetRequest
                {
                    PerformanceModifierBudgetRequestId = modifier.PerformanceModifierBudgetRequestId,
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    ForecastedBudget = modifier.ForecastedBudget,
                    RequestedBudget = modifier.RequestedBudget,
                    Deviation = modifier.Deviation,
                    Points = modifier.Points,

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = modifier.LastModified,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = modifier.ModifiedBy
                };

                db.PerformanceModifierBudgetRequest.Attach(entity);
                db.PerformanceModifierBudgetRequest.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifier(entity.MonthlyProjectPerformanceModifierId);

            }

            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        private decimal CalculateDeviation(PerformanceModifierBudgetRequest model)
        {
            var dv = new decimal();
            if (model.ForecastedBudget > 0 && model.RequestedBudget > 0)
            {
                dv = (Math.Abs((model.ForecastedBudget - model.RequestedBudget) / model.ForecastedBudget)) * 100;
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