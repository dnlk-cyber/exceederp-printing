using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using System;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;

namespace ExceedERP.Web.Areas.Fleetmanagement.Controllers
{
    //[AuthorizeRoles(ImportExportRoles.ImportExportExporterUser)]
    public class VehicleDistributionSecondApprovalSettingController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<VehicleDistributionSecondApprovalSetting> poInitiation = db.VehicleDistributionSecondApprovalSetting;
            DataSourceResult result = poInitiation.ToDataSourceResult(request, initiation => new {
                VehicleDistributionSecondApprovalSettingId = initiation.VehicleDistributionSecondApprovalSettingId,
                NeedsSecondApproval = initiation.NeedsSecondApproval,
                Remark = initiation.Remark,
                CreatedBy = initiation.CreatedBy,
                DateCreated = initiation.DateCreated,
                ModifiedBy = initiation.ModifiedBy,
                LastModified = initiation.LastModified,
               
            });

            return Json(result);
        }

        public ActionResult Create([DataSourceRequest]DataSourceRequest request, VehicleDistributionSecondApprovalSetting initiation)
        {
            var existing=db.VehicleDistributionSecondApprovalSetting.FirstOrDefault();
            if(existing!=null)
            {
                ModelState.AddModelError("added","Setting Already Added");
            }

            if (ModelState.IsValid)
            {
                var entity = new VehicleDistributionSecondApprovalSetting();
                entity.NeedsSecondApproval = initiation.NeedsSecondApproval;
                entity.Remark = initiation.Remark;
                entity.DateCreated = DateTime.Now;
                entity.CreatedBy = User.Identity.Name;
                db.VehicleDistributionSecondApprovalSetting.Add(entity);
                db.SaveChanges();
            }

            return Json(new[] { initiation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, VehicleDistributionSecondApprovalSetting initiation)
        {
            if (ModelState.IsValid)
            {
                var entity =db.VehicleDistributionSecondApprovalSetting.FirstOrDefault(x=>x.VehicleDistributionSecondApprovalSettingId== initiation.VehicleDistributionSecondApprovalSettingId);
                entity.NeedsSecondApproval = initiation.NeedsSecondApproval;
                entity.Remark = initiation.Remark;
                entity.LastModified = DateTime.Now;
                entity.ModifiedBy = User.Identity.Name;
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { initiation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, VehicleDistributionSecondApprovalSetting initiation)
        {

            if (ModelState.IsValid)
            {
                var entity = db.VehicleDistributionSecondApprovalSetting.FirstOrDefault(x => x.VehicleDistributionSecondApprovalSettingId == initiation.VehicleDistributionSecondApprovalSettingId);
                db.VehicleDistributionSecondApprovalSetting.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { initiation }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
