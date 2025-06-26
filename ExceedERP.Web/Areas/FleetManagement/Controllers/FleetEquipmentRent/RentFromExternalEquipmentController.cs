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
            var equipmentTypes = db.EquipmentType.AsNoTracking().ToList();
            var model = db.RentFromExternalEquipments.Where(x => x.RentFromExternalId == Id).AsEnumerable();
            foreach (var equipment in model)
            {
                equipment.EquipmentType = equipmentTypes.Find(t => t.EquipmentTypeID == equipment.EquipmentTypeID)?.EquipmentTypeName;
            }

            return Json(model.ToDataSourceResult(request));
        }
        public JsonResult Rate_Read([DataSourceRequest] DataSourceRequest request, int Id)
        {
            var rates = db.RentFromExternalEquipmentRates.Where(x => x.RentFromExternalEquipmentId == Id);

            return Json(rates.ToDataSourceResult(request));
        }


        public JsonResult RentFromExternalEquipment_Create([DataSourceRequest] DataSourceRequest request, RentFromExternalEquipment model, Guid Id)
        {
            model.RentFromExternalId = Id;
            ModelState.Remove("RentFromExternalId");
            if (ModelState.IsValid)
            {
                UpdateFleet(model.FleetId, Id, model);
                model.DateCreated = DateTime.Today;
                model.CreatedBy = User.Identity.Name;
                model.RentFromExternalId = Id;
                db.RentFromExternalEquipments.Add(model);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request));
        }

        public JsonResult Rate_Create([DataSourceRequest] DataSourceRequest request, RentFromExternalEquipmentRate model, int Id)
        {
            if (model.RoadType != null)
            {
                var fetchFromSetting = db.EquipmentRates.FirstOrDefault(x => x.DistanceFrom == model.DistanceFrom && x.DistanceTo == model.DistanceTo && x.RoadType == model.RoadType);
                if (fetchFromSetting == null)
                    ModelState.AddModelError("Check the distance interval", "Check the distance interval from setting");
                if (fetchFromSetting != null)
                {
                    model.IdleRentalRate = fetchFromSetting.IdleRate;
                    model.RentalRate = fetchFromSetting.Rate;
                }
            }
            if(model.DumpTruckTypeOfWork!=null)
            {
                var fetchFromSetting = db.EquipmentRates.FirstOrDefault(x => x.DistanceFrom == model.DistanceFrom && x.DistanceTo == model.DistanceTo && x.DumpTruckTypeOfWork == model.DumpTruckTypeOfWork);
                if (fetchFromSetting == null)
                    ModelState.AddModelError("Check the distance interval", "Check the distance interval from setting");
                if (fetchFromSetting != null)
                {
                    model.IdleRentalRate = fetchFromSetting.IdleRate;
                    model.RentalRate = fetchFromSetting.Rate;
                }
            }
            if (ModelState.IsValid)
            {
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Today;
                model.RentFromExternalEquipmentId = Id;
                db.RentFromExternalEquipmentRates.Add(model);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request));
        }
        public JsonResult RentFromExternalEquipment_Update([DataSourceRequest] DataSourceRequest request, RentFromExternalEquipment model)
        {

            if (ModelState.IsValid)
            {
                model.LastModified = DateTime.Today;
                model.ModifiedBy = User.Identity.Name;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request));
        }
        public JsonResult Rate_Update([DataSourceRequest] DataSourceRequest request, RentFromExternalEquipmentRate model)
        {
            if (model.RoadType != null)
            {
                var fetchFromSetting = db.EquipmentRates.FirstOrDefault(x => x.DistanceFrom == model.DistanceFrom && x.DistanceTo == model.DistanceTo && x.RoadType == model.RoadType);
                if (fetchFromSetting == null)
                    ModelState.AddModelError("Check the distance interval", "Check the distance interval from setting");
                if (fetchFromSetting != null)
                {
                    model.IdleRentalRate = fetchFromSetting.IdleRate;
                    model.RentalRate = fetchFromSetting.Rate;
                }
            }

            if (model.DumpTruckTypeOfWork != null)
            {
                var fetchFromSetting = db.EquipmentRates.FirstOrDefault(x => x.DistanceFrom == model.DistanceFrom && x.DistanceTo == model.DistanceTo && x.DumpTruckTypeOfWork == model.DumpTruckTypeOfWork);
                if (fetchFromSetting == null)
                    ModelState.AddModelError("Check the distance interval", "Check the distance interval from setting");
                if (fetchFromSetting != null)
                {
                    model.IdleRentalRate = fetchFromSetting.IdleRate;
                    model.RentalRate = fetchFromSetting.Rate;
                }
            }
            if (ModelState.IsValid)
            {
                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Today;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request));
        }
        public JsonResult RentFromExternalEquipment_Delete([DataSourceRequest] DataSourceRequest request, RentFromExternalEquipment model)
        {
            if (ModelState.IsValid)
            {
                var equipment = db.RentFromExternalEquipments.Find(model.RentFromExternalEquipmentId);
                db.RentFromExternalEquipments.Remove(equipment);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request));
        }
        public JsonResult Rate_Delete([DataSourceRequest] DataSourceRequest request, RentFromExternalEquipmentRate model)
        {
            if (ModelState.IsValid)
            {
                var rate = db.RentFromExternalEquipmentRates.Find(model.RentFromExternalEquipmentRateId);
                db.RentFromExternalEquipmentRates.Remove(rate);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request));
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
        public JsonResult IsDumpTruck(int id)
        {
            var FetchExternalEquipment = db.RentFromExternalEquipments.FirstOrDefault(x => x.RentFromExternalEquipmentId == id);
            if(FetchExternalEquipment!=null)
            {
                FetchExternalEquipment.IsDumpTruck = true;
                db.Entry(FetchExternalEquipment).State = EntityState.Modified;
                db.SaveChanges();
            }
            var succussMessage =new
            {
               Message="Truck Type Changed",
               Status="OK"
            };
            return Json(succussMessage,JsonRequestBehavior.AllowGet);
        }
        public JsonResult IsNotDumpTruck(int id)
        {
            var FetchExternalEquipment = db.RentFromExternalEquipments.FirstOrDefault(x => x.RentFromExternalEquipmentId == id);
            if (FetchExternalEquipment != null)
            {
                FetchExternalEquipment.IsDumpTruck = false;
                db.Entry(FetchExternalEquipment).State = EntityState.Modified;
                db.SaveChanges();
            }
            var succussMessage = new
            {
                Message = "Truck Type Changed",
                Status = "OK"
            };
            return Json(succussMessage, JsonRequestBehavior.AllowGet);
        }
    }
}