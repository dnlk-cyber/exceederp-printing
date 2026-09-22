using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
     //[AuthorizeRoles(FleetManagementRoles.FleetAdmin)]
    public class MaintenanceTypeController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/MaintenanceType/
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MaintenanceType_Read([DataSourceRequest] DataSourceRequest request)
        {
            var MaintenanceType = db.MaintenanceType.ToList(); 
            return Json(MaintenanceType.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaintenanceType_Create([DataSourceRequest] DataSourceRequest request, MaintenanceType MaintenanceType)
        {
            ModelState.Remove("MaintenanceTypeId");
            List<MaintenanceType> MaintenanceTypes = db.MaintenanceType.Where(f => f.Name == MaintenanceType.Name).ToList();
            if (MaintenanceTypes.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("ExistingName", " Maintenance Types with Name Already Exists ");
            }
            else
            {

                if (MaintenanceType != null)
                {
                    MaintenanceType.MaintenanceTypeId = Guid.NewGuid();
                     MaintenanceType.CreatedBy = User.Identity.Name;
                    MaintenanceType.DateCreated = DateTime.Now;

                    db.MaintenanceType.Add(MaintenanceType);
                    db.SaveChanges();
                }
            }

            return Json(new[] { MaintenanceType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaintenanceType_Update([DataSourceRequest] DataSourceRequest request, MaintenanceType MaintenanceType)
        {
            if (ModelState.IsValid)
            {
                if (MaintenanceType != null)
                {
                    MaintenanceType.ModifiedBy = User.Identity.Name;
                    MaintenanceType.LastModified = DateTime.Now;

                    db.MaintenanceType.Attach(MaintenanceType);
                    db.Entry(MaintenanceType).State = EntityState.Modified;
                    db.SaveChanges();

                }
            }

            return Json(new[] { MaintenanceType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MaintenanceType_Destroy([DataSourceRequest] DataSourceRequest request, MaintenanceType MaintenanceType)
        {
            var MaintenanceTypes = db.MaintenanceType.Where(x=>x.MaintenanceTypeId == MaintenanceType.MaintenanceTypeId).FirstOrDefault();
            if (MaintenanceTypes != null)
            {
                db.MaintenanceType.Attach(MaintenanceTypes);
                db.MaintenanceType.Remove(MaintenanceTypes);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Maintenance Type " + MaintenanceType.MaintenanceTypeId);

            }

            return Json(new[] { MaintenanceType }.ToDataSourceResult(request));
        }
        
        public ActionResult MaintenanceTypeDDBind()
        {

            var fleets = db.MaintenanceType.Select(s => new MaintenanceType
            {
                Name = s.Name,
                MaintenanceTypeId = s.MaintenanceTypeId
            });

            return Json(fleets, JsonRequestBehavior.AllowGet);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}


