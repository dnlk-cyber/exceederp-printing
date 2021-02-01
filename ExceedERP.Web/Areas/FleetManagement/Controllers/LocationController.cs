using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.DataAccess.Context;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class LocationController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/Location/


        public JsonResult LocationByFleet(string Id, [DataSourceRequest] DataSourceRequest request)
        {
            var Location = db.EquipmentLocations.Where(x => x.FleetId.ToString() == Id).Select(e => new

            {
                LocationId = e.EquipmentLocationId,
                Name = e.Name,
                DateCreated = e.DateCreated,
                Odometer = e.Odometer,
                Condition = e.Condition
            }
       ).ToList();


            return Json(Location.ToDataSourceResult(request));
        }



        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Location_Create([DataSourceRequest] DataSourceRequest request, EquipmentLocation EquipmentLocation, string Id)
        {

            ModelState.Clear();

            if (EquipmentLocation != null)
            {
                IFleetService _FleetService = new FleetService();
                var Fleet = _FleetService.GetById(Guid.Parse(Id));
                if (Fleet != null)
                {
                    var Location = new EquipmentLocation();
                    Location.EquipmentLocationId = Guid.NewGuid();
                    Location.Name = EquipmentLocation.Name;
                    Location.Condition = EquipmentLocation.Condition;
                    Location.FleetId = Guid.Parse(Id);
                    Location.CreatedBy = User.Identity.Name;
                    Location.DateCreated = DateTime.Now;
                    Location.Odometer = Fleet.CurentOdoMeter;
                    db.EquipmentLocations.Add(Location);
                    db.SaveChanges();

                    Fleet.Location = Location.Name;
                    _FleetService.Update(Fleet);
                }


            }

            return Json(new[] { EquipmentLocation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Location_Update([DataSourceRequest] DataSourceRequest request, EquipmentLocation Location, string Id)
        {
            if (Location != null)
            {
                Location.EquipmentLocationId = Guid.NewGuid();

                Location.CreatedBy = User.Identity.Name;
                Location.FleetId = Guid.Parse(Id);
                Location.CreatedBy = User.Identity.Name;
                Location.DateCreated = DateTime.Now;

                db.EquipmentLocations.Add(Location);
                db.SaveChanges();

                IFleetService _FleetService = new FleetService();
                var Fleet = _FleetService.GetById(Guid.Parse(Id));
                Fleet.Location = Location.Name;
                _FleetService.Update(Fleet);
            }

            return Json(new[] { Location }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Location_Destroy([DataSourceRequest] DataSourceRequest request, EquipmentLocation Location)
        {
            var LocationObj = db.EquipmentLocations.Where(x => x.EquipmentLocationId == Location.EquipmentLocationId).FirstOrDefault();
            if (Location != null)
            {

                db.EquipmentLocations.Attach(LocationObj);
                db.EquipmentLocations.Remove(LocationObj);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Location number " + Location.EquipmentLocationId);

            }

            return Json(new[] { Location }.ToDataSourceResult(request));
        }

    }
}

