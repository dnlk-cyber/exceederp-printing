using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    public class EquipmentRateController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/EquipmentRate
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EquipmentRent_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var equipementRate = db.EquipmentRates.Where(x => x.FleetRentalRateId == id);
            var result = equipementRate.ToDataSourceResult(request, s => new EquipmentRate
            {
                FleetRentalRateId = s.FleetRentalRateId,
                CapacityFrom = s.CapacityFrom,
                CapacityTo = s.CapacityTo,
                DistanceFrom = s.DistanceFrom,
                DistanceTo = s.DistanceTo,
                EquipmentCatagoryId = s.EquipmentCatagoryId,
                EquipmentRateId = s.EquipmentRateId,
                EquipmentTypeId = s.EquipmentTypeId,
                
                ManufacturingYearFrom = s.ManufacturingYearFrom,
                ManufacturingYearTo = s.ManufacturingYearTo,
                NumberOfSeatsFrom = s.NumberOfSeatsFrom,
                NumberOfSeatsTo = s.NumberOfSeatsTo,
                Rate = s.Rate,
                Remark = s.Remark,
                SprayingType = s.SprayingType,
                RoadType = s.RoadType,
                DumpTruckTypeOfWork = s.DumpTruckTypeOfWork,
                UoM = s.UoM,
                

            });
            return Json(result);
        }

        public ActionResult EquipmentRate_Create([DataSourceRequest] DataSourceRequest request, EquipmentRate model, int id)
        {
            if (ModelState.IsValid)
            {
                model.FleetRentalRateId = id;
                db.EquipmentRates.Add(model);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult EquipmentRate_Update([DataSourceRequest] DataSourceRequest request, EquipmentRate model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult EquipmentRate_Delete([DataSourceRequest] DataSourceRequest request, EquipmentRate model)
        {
            if (ModelState.IsValid)
            {
                var equipmentRate = db.EquipmentRates.Find(model.FleetRentalRateId);
                db.EquipmentRates.Remove(equipmentRate);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}