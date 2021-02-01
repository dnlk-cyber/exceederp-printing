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

namespace ExceedERP.Web.Areas.Construction.Controllers.Tracking
{
    public class EarnedValueManagementPeriodController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
       
        public ActionResult Periods_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<EarnedValueManagementPeriod> periods = db.EarnedValueManagementPeriods.Where(pm => pm.ProjectInformationId == id);
            DataSourceResult result = periods.ToDataSourceResult(request, period => new
            {
                EarnedValueManagementPeriodId = period.EarnedValueManagementPeriodId,
                ProjectInformationId = period.ProjectInformationId,
                ConstructionPeriodId = period.ConstructionPeriodId,
                Remark = period.Remark,

                OrgBranchName = period.OrgBranchName,
                DateCreated = period.DateCreated,
                LastModified = period.LastModified,
                CreatedBy = period.CreatedBy,
                ModifiedBy = period.ModifiedBy,
            });

            return Json(result);
        }

        private bool PeriodExists(int periodId, int projectId)
        {
            var period = db.EarnedValueManagementPeriods.Where(p => p.ConstructionPeriodId == periodId && 
                        p.ProjectInformationId == projectId)
                        .FirstOrDefault();
            if (period != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Period_Create([DataSourceRequest]DataSourceRequest request, EarnedValueManagementPeriod period, int id)
        {
            if (ModelState.IsValid)
            {
                if (PeriodExists(period.ConstructionPeriodId, id))
                {
                    ModelState.AddModelError("", "Value Exists!!");
                }
                else
                {
                    var entity = new EarnedValueManagementPeriod
                    {
                        EarnedValueManagementPeriodId = period.EarnedValueManagementPeriodId,
                        ProjectInformationId = id,
                        ConstructionPeriodId = period.ConstructionPeriodId,
                        Remark = period.Remark,

                        OrgBranchName = period.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = period.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = period.ModifiedBy
                    };

                    db.EarnedValueManagementPeriods.Add(entity);
                    db.SaveChanges();
                    period.EarnedValueManagementPeriodId = entity.EarnedValueManagementPeriodId;

                }
            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Period_Update([DataSourceRequest]DataSourceRequest request, EarnedValueManagementPeriod period)
        {
            if (ModelState.IsValid)
            {
                var entity = new EarnedValueManagementPeriod
                {
                    EarnedValueManagementPeriodId = period.EarnedValueManagementPeriodId,
                    ProjectInformationId = period.ProjectInformationId,
                    ConstructionPeriodId = period.ConstructionPeriodId,
                    Remark = period.Remark,

                    OrgBranchName = period.OrgBranchName,
                    DateCreated = period.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = period.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.EarnedValueManagementPeriods.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Period_Destroy([DataSourceRequest]DataSourceRequest request, EarnedValueManagementPeriod period)
        {
            if (ModelState.IsValid)
            {
                var entity = new EarnedValueManagementPeriod
                {
                    EarnedValueManagementPeriodId = period.EarnedValueManagementPeriodId,
                    ProjectInformationId = period.ProjectInformationId,
                    ConstructionPeriodId = period.ConstructionPeriodId,
                    Remark = period.Remark,

                    OrgBranchName = period.OrgBranchName,
                    DateCreated = period.DateCreated,
                    LastModified = period.LastModified,
                    CreatedBy = period.CreatedBy,
                    ModifiedBy = period.ModifiedBy
                };

                db.EarnedValueManagementPeriods.Attach(entity);
                db.EarnedValueManagementPeriods.Remove(entity);
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