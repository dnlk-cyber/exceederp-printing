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
    public class PerformanceModifierSubContractAdministrationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Modifier_Read([DataSourceRequest]DataSourceRequest request, int pIdReadSubCtrct)
        {
            IQueryable<PerformanceModifierSubContractAdministration> modifiers = db.PerformanceModifierSubContractAdministration.Where(x => x.MonthlyProjectPerformanceModifierId == pIdReadSubCtrct);
            DataSourceResult result = modifiers.ToDataSourceResult(request, modifier => new
            {
                PerformanceModifierSubContractAdministrationId = modifier.PerformanceModifierSubContractAdministrationId,
                MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                Description = modifier.Description,
                MaxAllowablePoints = modifier.MaxAllowablePoints,
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
        public ActionResult Modifier_Create([DataSourceRequest]DataSourceRequest request, PerformanceModifierSubContractAdministration modifier, int pIdCreateSubCtrct)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierSubContractAdministration
                {
                    PerformanceModifierSubContractAdministrationId = modifier.PerformanceModifierSubContractAdministrationId,
                    MonthlyProjectPerformanceModifierId = pIdCreateSubCtrct,
                    Description = modifier.Description,
                    MaxAllowablePoints = modifier.MaxAllowablePoints,
                    Points = modifier.MaxAllowablePoints != null && modifier.Points > modifier.MaxAllowablePoints ?
                                        modifier.MaxAllowablePoints : modifier.Points,

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = modifier.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = modifier.ModifiedBy
                };

                db.PerformanceModifierSubContractAdministration.Add(entity);
                db.SaveChanges();
                modifier.PerformanceModifierSubContractAdministrationId = entity.PerformanceModifierSubContractAdministrationId;

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifier(entity.MonthlyProjectPerformanceModifierId);
            }
            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Update([DataSourceRequest]DataSourceRequest request, PerformanceModifierSubContractAdministration modifier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierSubContractAdministration
                {
                    PerformanceModifierSubContractAdministrationId = modifier.PerformanceModifierSubContractAdministrationId,
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    Description = modifier.Description,
                    MaxAllowablePoints = modifier.MaxAllowablePoints,
                    Points = modifier.MaxAllowablePoints != null && modifier.Points > modifier.MaxAllowablePoints? 
                                        modifier.MaxAllowablePoints: modifier.Points,

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.PerformanceModifierSubContractAdministration.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifier(entity.MonthlyProjectPerformanceModifierId);

            }

            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Destroy([DataSourceRequest]DataSourceRequest request, PerformanceModifierSubContractAdministration modifier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierSubContractAdministration
                {
                    PerformanceModifierSubContractAdministrationId = modifier.PerformanceModifierSubContractAdministrationId,
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    Description = modifier.Description,
                    MaxAllowablePoints = modifier.MaxAllowablePoints,
                    Points = modifier.Points,

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = modifier.LastModified,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = modifier.ModifiedBy
                };

                db.PerformanceModifierSubContractAdministration.Attach(entity);
                db.PerformanceModifierSubContractAdministration.Remove(entity);
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