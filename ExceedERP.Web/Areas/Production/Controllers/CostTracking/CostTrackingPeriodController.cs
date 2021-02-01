using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.CostTracking
{
    public class CostTrackingPeriodController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();

        public ActionResult Periods_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<CostTrackingPeriod> periods = db.CostTrackingPeriods.Where(p => p.ProductionProjectId == id);
            DataSourceResult result = periods.ToDataSourceResult(request, period => new
            {
                CostTrackingPeriodId = period.CostTrackingPeriodId,
                ProductionProjectId = period.ProductionProjectId,
                ProductionPeriodId = period.ProductionPeriodId,
                Remark = period.Remark,


                OrgBranchName = period.OrgBranchName,
                DateCreated = period.DateCreated,
                LastModified = period.LastModified,
                CreatedBy = period.CreatedBy,
                ModifiedBy = period.ModifiedBy,
            });

            return Json(result);
        }

        private bool PeriodValueExists(int projId, CostTrackingPeriod record)
        {
            var valueExists = false;
            var temp = db.CostTrackingPeriods.FirstOrDefault(p => p.ProductionProjectId == projId && p.ProductionPeriodId == record.ProductionPeriodId);
            if (temp != null)
            {
                valueExists = true;
            }

            return valueExists;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Period_Create([DataSourceRequest]DataSourceRequest request, CostTrackingPeriod period, int id)
        {
            if (ModelState.IsValid)
            {

                if (PeriodValueExists(id, period))
                {
                    ModelState.AddModelError("", "Period Exists");
                }
                else
                {
                    var entity = new CostTrackingPeriod
                    {
                        CostTrackingPeriodId = period.CostTrackingPeriodId,
                        ProductionProjectId = id,
                        ProductionPeriodId = period.ProductionPeriodId,
                        Remark = period.Remark,

                        OrgBranchName = period.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = period.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = period.ModifiedBy,

                    };

                    db.CostTrackingPeriods.Add(entity);
                    db.SaveChanges();

                    period.CostTrackingPeriodId = entity.CostTrackingPeriodId;
                }

            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Period_Update([DataSourceRequest]DataSourceRequest request, CostTrackingPeriod period)
        {
            if (ModelState.IsValid)
            {
                var entity = new CostTrackingPeriod
                {
                    CostTrackingPeriodId = period.CostTrackingPeriodId,
                    ProductionProjectId = period.ProductionProjectId,
                    ProductionPeriodId = period.ProductionPeriodId,
                    Remark = period.Remark,

                    OrgBranchName = period.OrgBranchName,
                    DateCreated = period.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = period.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.CostTrackingPeriods.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Period_Destroy([DataSourceRequest]DataSourceRequest request, CostTrackingPeriod period)
        {
            if (ModelState.IsValid)
            {
                var entity = new CostTrackingPeriod
                {
                    CostTrackingPeriodId = period.CostTrackingPeriodId,
                    ProductionProjectId = period.ProductionProjectId,
                    ProductionPeriodId = period.ProductionPeriodId,
                    Remark = period.Remark,

                    OrgBranchName = period.OrgBranchName,
                    DateCreated = period.DateCreated,
                    LastModified = period.LastModified,
                    CreatedBy = period.CreatedBy,
                    ModifiedBy = period.ModifiedBy,

                };

                db.CostTrackingPeriods.Attach(entity);
                db.CostTrackingPeriods.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}