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
    public class PerformanceModifierHealthAndSafetyController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Modifier_Read([DataSourceRequest]DataSourceRequest request, int pIdReadSafety)
        {
            IQueryable<PerformanceModifierHealthAndSafety> modifiers = db.PerformanceModifierHealthAndSafety.Where(x => x.MonthlyProjectPerformanceModifierId == pIdReadSafety);
            DataSourceResult result = modifiers.ToDataSourceResult(request, modifier => new
            {
                PerformanceModifierHealthAndSafetyId = modifier.PerformanceModifierHealthAndSafetyId,
                MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                InjuryLevel = modifier.InjuryLevel,
                InjuryCount = modifier.InjuryCount,
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
        public ActionResult Modifier_Create([DataSourceRequest]DataSourceRequest request, PerformanceModifierHealthAndSafety modifier, int pIdCreateSafety)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierHealthAndSafety
                {
                    //PerformanceModifierHealthAndSafetyId = modifier.PerformanceModifierHealthAndSafetyId,
                    MonthlyProjectPerformanceModifierId = pIdCreateSafety,
                    InjuryLevel = modifier.InjuryLevel,
                    InjuryCount = modifier.InjuryCount,
                    Points = PerformanceEvnHelper.GetHealthAndSafetyScore(modifier),

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = modifier.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = modifier.ModifiedBy
                };

                db.PerformanceModifierHealthAndSafety.Add(entity);
                db.SaveChanges();
                modifier.PerformanceModifierHealthAndSafetyId = entity.PerformanceModifierHealthAndSafetyId;

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifier(entity.MonthlyProjectPerformanceModifierId);

            }
            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Update([DataSourceRequest]DataSourceRequest request, PerformanceModifierHealthAndSafety modifier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierHealthAndSafety
                {
                    PerformanceModifierHealthAndSafetyId = modifier.PerformanceModifierHealthAndSafetyId,
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    InjuryLevel = modifier.InjuryLevel,
                    InjuryCount = modifier.InjuryCount,
                    Points = PerformanceEvnHelper.GetHealthAndSafetyScore(modifier),


                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.PerformanceModifierHealthAndSafety.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifier(entity.MonthlyProjectPerformanceModifierId);

            }

            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Destroy([DataSourceRequest]DataSourceRequest request, PerformanceModifierHealthAndSafety modifier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierHealthAndSafety
                {
                    PerformanceModifierHealthAndSafetyId = modifier.PerformanceModifierHealthAndSafetyId,
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    InjuryLevel = modifier.InjuryLevel,
                    InjuryCount = modifier.InjuryCount,
                    Points = modifier.Points,

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = modifier.LastModified,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = modifier.ModifiedBy
                };

                db.PerformanceModifierHealthAndSafety.Attach(entity);
                db.PerformanceModifierHealthAndSafety.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifier(entity.MonthlyProjectPerformanceModifierId);

            }

            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}