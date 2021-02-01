using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers
{
    public class ConstructionSubPeriodController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult SubPeriods_Read([DataSourceRequest]DataSourceRequest request, int id)
        {

            IQueryable<ConstructionSubPeriod> periods = db.ConstructionSubPeriod.Where(c => c.ConstructionPeriodId == id);

            DataSourceResult result = periods.ToDataSourceResult(request, period => new
            {
                ConstructionSubPeriodId = period.ConstructionSubPeriodId,
                ConstructionPeriodId = period.ConstructionPeriodId,
                Name = period.Name,
                DateFrom = period.DateFrom,
                DateTo = period.DateTo,

                OrgBranchName = period.OrgBranchName,
                DateCreated = period.DateCreated,
                LastModified = period.LastModified,
                CreatedBy = period.CreatedBy,
                ModifiedBy = period.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubPeriod_Create([DataSourceRequest]DataSourceRequest request, ConstructionSubPeriod period, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConstructionSubPeriod
                {
                    ConstructionSubPeriodId = period.ConstructionSubPeriodId,
                    ConstructionPeriodId = id,
                    Name = period.Name,
                    DateFrom = period.DateFrom,
                    DateTo = period.DateTo,

                    OrgBranchName = period.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = period.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = period.ModifiedBy
                };

                db.ConstructionSubPeriod.Add(entity);
                db.SaveChanges();
                period.ConstructionSubPeriodId = entity.ConstructionSubPeriodId;
            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubPeriod_Update([DataSourceRequest]DataSourceRequest request, ConstructionSubPeriod period)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConstructionSubPeriod
                {
                    ConstructionSubPeriodId = period.ConstructionSubPeriodId,
                    ConstructionPeriodId = period.ConstructionPeriodId,
                    Name = period.Name,
                    DateFrom = period.DateFrom,
                    DateTo = period.DateTo,

                    OrgBranchName = period.OrgBranchName,
                    DateCreated = period.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = period.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                };

                db.ConstructionSubPeriod.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubPeriod_Destroy([DataSourceRequest]DataSourceRequest request, ConstructionSubPeriod period)
        {
            if (ModelState.IsValid)
            {
                var entity = new ConstructionSubPeriod
                {
                    ConstructionSubPeriodId = period.ConstructionSubPeriodId,
                    ConstructionPeriodId = period.ConstructionPeriodId,
                    Name = period.Name,
                    DateFrom = period.DateFrom,
                    DateTo = period.DateTo,

                    OrgBranchName = period.OrgBranchName,
                    DateCreated = period.DateCreated,
                    LastModified = period.LastModified,
                    CreatedBy = period.CreatedBy,
                    ModifiedBy = period.ModifiedBy
                };

                db.ConstructionSubPeriod.Attach(entity);
                db.ConstructionSubPeriod.Remove(entity);
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