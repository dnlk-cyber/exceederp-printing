using System;

using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class AdvanceMeasureController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AdvanceMeasures_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AdvanceMeasure> advancemeasures = db.AdvanceMeasures;
            DataSourceResult result = advancemeasures.ToDataSourceResult(request, advanceMeasure => new {
                AdvanceMeasureId = advanceMeasure.AdvanceMeasureId,
                Description = advanceMeasure.Description,
                DateCreated = advanceMeasure.DateCreated,
                LastModified = advanceMeasure.LastModified,
                CreatedBy = advanceMeasure.CreatedBy,
                ModifiedBy = advanceMeasure.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvanceMeasures_Create([DataSourceRequest]DataSourceRequest request, AdvanceMeasure advanceMeasure)
        {
            ModelState.Remove("AdvanceMeasureId");
            if (ModelState.IsValid)
            {
                var entity = new AdvanceMeasure
                {
                    AdvanceMeasureId = Guid.NewGuid(),
                    Description = advanceMeasure.Description,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.AdvanceMeasures.Add(entity);
                db.SaveChanges();
                advanceMeasure.AdvanceMeasureId = entity.AdvanceMeasureId;
            }

            return Json(new[] { advanceMeasure }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvanceMeasures_Update([DataSourceRequest]DataSourceRequest request, AdvanceMeasure advanceMeasure)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdvanceMeasure
                {
                    AdvanceMeasureId = advanceMeasure.AdvanceMeasureId,
                    Description = advanceMeasure.Description,
                    DateCreated = advanceMeasure.DateCreated,
                    CreatedBy = advanceMeasure.CreatedBy,
                    LastModified = DateTime.Now,
                  
                    ModifiedBy = User.Identity.Name
                };

                db.AdvanceMeasures.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { advanceMeasure }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AdvanceMeasures_Destroy([DataSourceRequest]DataSourceRequest request, AdvanceMeasure advanceMeasure)
        {
            if (ModelState.IsValid)
            {
                var entity = new AdvanceMeasure
                {
                    AdvanceMeasureId = advanceMeasure.AdvanceMeasureId,
                    Description = advanceMeasure.Description,
                    DateCreated = advanceMeasure.DateCreated,
                    LastModified = advanceMeasure.LastModified,
                    CreatedBy = advanceMeasure.CreatedBy,
                    ModifiedBy = advanceMeasure.ModifiedBy
                };

                db.AdvanceMeasures.Attach(entity);
                db.AdvanceMeasures.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { advanceMeasure }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
