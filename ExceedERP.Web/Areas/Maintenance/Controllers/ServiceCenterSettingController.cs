using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Web.Mvc;
using System.Linq;
using System;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class ServiceCenterSettingController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();

        // GET: Maintenance/ServiceCenterSetting
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request, Guid Id)
        {
            var serviceCenter = db.ServiceCenters.Where(x => x.MaintenanceCenterId == Id);
            DataSourceResult result = serviceCenter.ToDataSourceResult(request, sCenter => new ServiceCenter{
                Name = sCenter.Name,
                Description = sCenter.Description,
                MaintenanceCenterId = sCenter.MaintenanceCenterId,
                ServiceCenterId = sCenter.ServiceCenterId
               
            });

            return Json(result);
        }

        public ActionResult Create([DataSourceRequest]DataSourceRequest request, Guid Id, ServiceCenter serviceCenter)
        {
            ModelState.Remove("MaintenanceCenterId");
            if (ModelState.IsValid)
            {
                serviceCenter.MaintenanceCenterId = Id;
                db.ServiceCenters.Add(serviceCenter);
                db.SaveChanges();
            }
            return Json(new[] { serviceCenter }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update([DataSourceRequest]DataSourceRequest request, ServiceCenter serviceCenter)
        {
            if (ModelState.IsValid)
            {
                db.ServiceCenters.Attach(serviceCenter);
                db.Entry(serviceCenter).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { serviceCenter }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, ServiceCenter serviceCenter)
        {
            if (ModelState.IsValid)
            {
                var model = db.ServiceCenters.Find(serviceCenter.ServiceCenterId);
                db.ServiceCenters.Remove(model);
                
                db.SaveChanges();
            }
            return Json(new[] { serviceCenter }.ToDataSourceResult(request, ModelState));
        }
    }
}