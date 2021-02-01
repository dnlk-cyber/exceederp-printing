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

namespace ExceedERP.Web.Areas.Construction.Controllers.Performance
{
    public class PeriodMappingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/PeriodMapping
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PeriodMapping_Read([DataSourceRequest]DataSourceRequest request, int projectIdRead)
        {
            IQueryable<PeriodMapping> mappings = db.PeriodMapping.Where(pm => pm.ProjectInformationId == projectIdRead);
            DataSourceResult result = mappings.ToDataSourceResult(request, period => new
            {
                PeriodMappingId = period.PeriodMappingId,
                ProjectInformationId = period.ProjectInformationId,
                ConstructionPeriodId = period.ConstructionPeriodId,
                Remark = period.Remark,
            });

            return Json(result);
        }

        private bool CheckExistance(int periodId, int projectId)
        {
            var period = db.PeriodMapping.Where(pm => pm.ConstructionPeriodId == periodId && pm.ProjectInformationId == projectId).FirstOrDefault();
            if (period != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PeriodMapping_Create([DataSourceRequest]DataSourceRequest request, PeriodMapping period, int projectIdCreate)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(period.ConstructionPeriodId, projectIdCreate))
                {
                    ModelState.AddModelError("", "Value Exists!!");
                }
                else
                {
                    var entity = new PeriodMapping
                    {
                        PeriodMappingId = period.PeriodMappingId,
                        ProjectInformationId = projectIdCreate,
                        ConstructionPeriodId = period.ConstructionPeriodId,
                        Remark = period.Remark,
                    };

                    db.PeriodMapping.Add(entity);
                    db.SaveChanges();
                    period.PeriodMappingId = entity.PeriodMappingId;

                }
            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PeriodMapping_Update([DataSourceRequest]DataSourceRequest request, PeriodMapping period)
        {
            if (ModelState.IsValid)
            {
                var entity = new PeriodMapping
                {
                    PeriodMappingId = period.PeriodMappingId,
                    ProjectInformationId = period.ProjectInformationId,
                    ConstructionPeriodId = period.ConstructionPeriodId,
                    Remark = period.Remark,
                };

                db.PeriodMapping.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { period }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PeriodMapping_Destroy([DataSourceRequest]DataSourceRequest request, PeriodMapping period)
        {
            if (ModelState.IsValid)
            {
                var entity = new PeriodMapping
                {
                    PeriodMappingId = period.PeriodMappingId,
                    ProjectInformationId = period.ProjectInformationId,
                    ConstructionPeriodId = period.ConstructionPeriodId,
                    Remark = period.Remark,
                };

                db.PeriodMapping.Attach(entity);
                db.PeriodMapping.Remove(entity);
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