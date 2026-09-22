using ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class ExternalComponentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ExternalComponent_Read([DataSourceRequest] DataSourceRequest request, Guid jobOrderID)
        {
            var ExternalComponent = db.ExternalComponent.Where(x => x.JobId == jobOrderID).ToList();

            return Json(ExternalComponent.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalComponent_Create([DataSourceRequest] DataSourceRequest request, ExternalComponent ExternalComponent, Guid jobOrderID)
        {
            ModelState.Remove("ExternalComponentId");
            ModelState.Remove("JobId");
            IJobOrderService _JobOrderService = new JobOrderService();
            IFleetService _FleetService = new FleetService();

            var Job = db.JobOrder.Where(x => x.JobOrderID == jobOrderID).FirstOrDefault();
            //var Fleet = _FleetService.GetAll().Where(x => x.FleetsID == Job.FleetsID).Select(s => new Fleets
            //{
            //    UnitNum = s.UnitNum,
            //    FleetPhoto = s.EquipmentType.CategoryName,
            //    ChasisNumber = s.ChasisNumber,
            //    CurentOdoMeter = s.CurentOdoMeter,
            //}
            //    ).FirstOrDefault();
            //if (Fleet != null)
            //{
                ExternalComponent.ExternalComponentId = Guid.NewGuid();
                ExternalComponent.JobId = jobOrderID;
                //ExternalComponent.ChasisNo = Fleet.ChasisNumber;
                //ExternalComponent.Code = Fleet.UnitNum;
                //ExternalComponent.EquipmentType = Fleet.FleetPhoto;
                //ExternalComponent.KMReading = Fleet.CurentOdoMeter.ToString();
                ExternalComponent.DateCreated = DateTime.Now;
                ExternalComponent.CreatedBy = User.Identity.Name;

                db.ExternalComponent.Add(ExternalComponent);
                db.SaveChanges();
            //}

            return Json(new[] { ExternalComponent }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalComponent_Update([DataSourceRequest] DataSourceRequest request, ExternalComponent ExternalComponent, Guid jobOrderID)
        {
            if (ExternalComponent != null)
            {
                ExternalComponent.ModifiedBy = User.Identity.Name;
                ExternalComponent.LastModified = DateTime.Now;
                ExternalComponent.JobId = jobOrderID;
                db.ExternalComponent.Attach(ExternalComponent);
                db.Entry(ExternalComponent).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { ExternalComponent }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ExternalComponent_Destroy([DataSourceRequest] DataSourceRequest request, ExternalComponent ExternalComponent)
        {
            var ExternalComponents = db.ExternalComponent.Where(x => x.ExternalComponentId == ExternalComponent.ExternalComponentId).FirstOrDefault();
            if (ExternalComponents != null)
            {

                db.ExternalComponent.Attach(ExternalComponents);
                db.ExternalComponent.Remove(ExternalComponents);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted ExternalComponent number " + ExternalComponent.ExternalComponentId);

            }

            return Json(new[] { ExternalComponent }.ToDataSourceResult(request));
        }
   protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
