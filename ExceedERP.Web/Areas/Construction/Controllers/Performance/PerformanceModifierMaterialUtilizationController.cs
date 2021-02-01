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
    public class PerformanceModifierMaterialUtilizationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult Modifier_Read([DataSourceRequest]DataSourceRequest request, int pIdReadMUt)
        {
            IQueryable<PerformanceModifierMaterialUtilization> modifiers = db.PerformanceModifierMaterialUtilization.Where(x => x.MonthlyProjectPerformanceModifierId == pIdReadMUt);
            DataSourceResult result = modifiers.ToDataSourceResult(request, modifier => new
            {
                PerformanceModifierMaterialUtilizationId = modifier.PerformanceModifierMaterialUtilizationId,
                MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                ItemCategoryCode = modifier.ItemCategoryCode,
                ItemCode = modifier.ItemCode,
                UtilizedAmount = modifier.UtilizedAmount,

                OrgBranchName = modifier.OrgBranchName,
                DateCreated = modifier.DateCreated,
                LastModified = modifier.LastModified,
                CreatedBy = modifier.CreatedBy,
                ModifiedBy = modifier.ModifiedBy
            });

            return Json(result);
        }
        private bool MaterialExists(int modifierId, string itemCode) {
            var valueExists = false;
            var mat = db.PerformanceModifierMaterialUtilization
                .FirstOrDefault(mu => mu.MonthlyProjectPerformanceModifierId == modifierId && mu.ItemCode == itemCode);
            if(mat != null)
            {
                valueExists = true;
            }
            return valueExists;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Create([DataSourceRequest]DataSourceRequest request, PerformanceModifierMaterialUtilization modifier, int pIdCreateMUt)
        {
            if (ModelState.IsValid)
            {
                if(MaterialExists(pIdCreateMUt, modifier.ItemCode))
                {
                    ModelState.AddModelError("", "Material Exists");
                }
                else
                {
                    var entity = new PerformanceModifierMaterialUtilization
                    {
                        PerformanceModifierMaterialUtilizationId = modifier.PerformanceModifierMaterialUtilizationId,
                        MonthlyProjectPerformanceModifierId = pIdCreateMUt,
                        ItemCategoryCode = modifier.ItemCategoryCode,
                        ItemCode = modifier.ItemCode,
                        UtilizedAmount = modifier.UtilizedAmount,

                        OrgBranchName = modifier.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = modifier.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = modifier.ModifiedBy
                    };

                    db.PerformanceModifierMaterialUtilization.Add(entity);
                    db.SaveChanges();
                    modifier.PerformanceModifierMaterialUtilizationId = entity.PerformanceModifierMaterialUtilizationId;

                    PerformanceEvnHelper.UpdateMonthlyPerformanceModifierAfterModifyingMaterialUtilization(entity.MonthlyProjectPerformanceModifierId);

                }

            }
            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Update([DataSourceRequest]DataSourceRequest request, PerformanceModifierMaterialUtilization modifier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierMaterialUtilization
                {
                    PerformanceModifierMaterialUtilizationId = modifier.PerformanceModifierMaterialUtilizationId,
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    ItemCategoryCode = modifier.ItemCategoryCode,
                    ItemCode = modifier.ItemCode,
                    UtilizedAmount = modifier.UtilizedAmount,


                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.PerformanceModifierMaterialUtilization.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifierAfterModifyingMaterialUtilization(entity.MonthlyProjectPerformanceModifierId);

            }

            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Destroy([DataSourceRequest]DataSourceRequest request, PerformanceModifierMaterialUtilization modifier)
        {
            if (ModelState.IsValid)
            {
                var entity = new PerformanceModifierMaterialUtilization
                {
                    PerformanceModifierMaterialUtilizationId = modifier.PerformanceModifierMaterialUtilizationId,
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    ItemCategoryCode = modifier.ItemCategoryCode,
                    ItemCode = modifier.ItemCode,
                    UtilizedAmount = modifier.UtilizedAmount,

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = modifier.LastModified,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = modifier.ModifiedBy
                };

                db.PerformanceModifierMaterialUtilization.Attach(entity);
                db.PerformanceModifierMaterialUtilization.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyPerformanceModifierAfterModifyingMaterialUtilization(entity.MonthlyProjectPerformanceModifierId);

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