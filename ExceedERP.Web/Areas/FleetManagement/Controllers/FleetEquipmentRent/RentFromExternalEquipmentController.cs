using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetEquipmentRent
{
    public class RentFromExternalEquipmentController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/RentFromExternalEquipment
        public ActionResult Index()
        {
            return View();
        }

        public JsonResult RentFromExternalEquipment_Read([DataSourceRequest] DataSourceRequest request, Guid Id)
        {
            var rentFromExternalEquipment = db.RentFromExternalEquipments.Where(x => x.RentFromExternalId == Id);
           
            return Json(rentFromExternalEquipment.ToDataSourceResult(request));
        }


        public JsonResult RentFromExternalEquipment_Create([DataSourceRequest] DataSourceRequest request, RentFromExternalEquipment rentFromExternalEquipment, Guid Id)
        {
            rentFromExternalEquipment.RentFromExternalId = Id;
            ModelState.Remove("RentFromExternalId");
            if (ModelState.IsValid)
            {
                UpdateFleet(rentFromExternalEquipment.FleetId, Id, rentFromExternalEquipment);
                rentFromExternalEquipment.RentFromExternalId = Id;
                db.RentFromExternalEquipments.Add(rentFromExternalEquipment);
                db.SaveChanges();
            }
            return Json(new[] { rentFromExternalEquipment }.ToDataSourceResult(request));
        }
        public JsonResult RentFromExternalEquipment_Update([DataSourceRequest] DataSourceRequest request, RentFromExternalEquipment rentFromExternalEquipment)
        {
            if (ModelState.IsValid)
            {
                db.Entry(rentFromExternalEquipment).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { rentFromExternalEquipment }.ToDataSourceResult(request));
        }
        public JsonResult RentFromExternalEquipment_Delete([DataSourceRequest] DataSourceRequest request, RentFromExternalEquipment rentFromExternalEquipment)
        {
            if (ModelState.IsValid)
            {
                var equipment = db.RentFromExternalEquipments.Find(rentFromExternalEquipment.RentFromExternalEquipmentId);
                db.RentFromExternalEquipments.Remove(equipment);
                db.SaveChanges();
            }
            return Json(new[] { rentFromExternalEquipment }.ToDataSourceResult(request));
        }

        private void UpdateFleet(Guid fleetId, Guid rentId, RentFromExternalEquipment rentEquipment)
        {
            
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetId);
            fleet.RentId = rentId;
            fleet.RentalRate = rentEquipment.RentalRate;
            fleet.IdealRentalRate = rentEquipment.IdleRentalRate;
            fleet.RentType = rentEquipment.RentType;
            db.Entry(fleet).State = EntityState.Modified;
            db.SaveChanges();
        }
    }
}