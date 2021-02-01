using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using System.Data.Entity;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class EquipmentBatteryController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/EquipmentBattery/


        public string GetDuration(DateTime? dt)
        {

            TimeSpan Duration = (DateTime.Now - DateTime.Parse(dt.ToString()));
            return Duration.TotalHours + " Hours";
        }
        public ActionResult BatteryByFleet_Read([DataSourceRequest] DataSourceRequest request, string Id)
        {
            List<EquipmentBattery> tires = new List<EquipmentBattery>();
            var Batterys = db.EquipmentBattery.Where(x => x.fleetId.ToString() == Id && x.Isattached == true).ToList();
            foreach (var e in Batterys)
            {
                string Duration = GetDuration(e.DateCreated);
                EquipmentBattery Battery = new EquipmentBattery()
                {
                    EquipmentBatteryId = e.EquipmentBatteryId,
                    Make = e.Make,
                    Type = e.Type,
                    SerialNo = e.SerialNo,
                    Odometer = e.Odometer,
                    CreatedBy = e.CreatedBy,

                    Condition = e.Condition,
                    LastModified = e.LastModified,
                    ModifiedBy = e.ModifiedBy,
                    DateCreated = e.DateCreated,
                    UnitPrice = e.UnitPrice

                };
                tires.Add(e);
            }

            return Json(tires.ToDataSourceResult(request));

        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentBattery_Create([DataSourceRequest] DataSourceRequest request, EquipmentBattery EquipmentBattery, Guid Id)
        {
            var Battery = db.EquipmentBattery.Where(x => x.SerialNo == EquipmentBattery.SerialNo).ToList();

            var Fleet = db.Fleets.Where(x => x.FleetsID == Id).FirstOrDefault();
            if (Fleet != null)
            {
                EquipmentBattery.Odometer = Fleet.CurentOdoMeter;
            }
            if(Battery.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("DuplicateSerialNo", " Serial Number Already Registered!");
                return Json(new[] { EquipmentBattery }.ToDataSourceResult(request, ModelState));
            }
            var Batterys = db.EquipmentBattery.Where(x => x.SerialNo == EquipmentBattery.StoreCode).ToList();
            if (Batterys.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("DuplicateSerialNo", " Store Code Already Registered!");
                return Json(new[] { EquipmentBattery }.ToDataSourceResult(request, ModelState));
            }

            if (EquipmentBattery != null)
            {

                EquipmentBattery.EquipmentBatteryId = Guid.NewGuid();
                EquipmentBattery.fleetId = Id;
                EquipmentBattery.Isattached = true;
                EquipmentBattery.DateCreated = DateTime.Now;
                EquipmentBattery.CreatedBy = User.Identity.Name;


                db.EquipmentBattery.Add(EquipmentBattery);
                db.SaveChanges();


            }


            return Json(new[] { EquipmentBattery }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentBattery_Update([DataSourceRequest] DataSourceRequest request, EquipmentBattery EquipmentBattery)
        {
            var Battery = db.EquipmentBattery.Where(x => x.SerialNo == EquipmentBattery.SerialNo && x.EquipmentBatteryId != EquipmentBattery.EquipmentBatteryId).ToList();
            if (Battery.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("DuplicateSerialNo", " Serial Number Already Registered!");
                return Json(new[] { EquipmentBattery }.ToDataSourceResult(request, ModelState));
            }
            var Batterys = db.EquipmentBattery.Where(x => x.SerialNo == EquipmentBattery.StoreCode).ToList();
            if (Batterys.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("DuplicateSerialNo", " Store Code Already Registered!");
                return Json(new[] { EquipmentBattery }.ToDataSourceResult(request, ModelState));
            }

            if (EquipmentBattery != null)
            {
                EquipmentBattery.LastModified = DateTime.Now;
                EquipmentBattery.ModifiedBy = User.Identity.Name;

                db.EquipmentBattery.Attach(EquipmentBattery);
                db.Entry(EquipmentBattery).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { EquipmentBattery }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EquipmentBattery_Destroy([DataSourceRequest] DataSourceRequest request, EquipmentBattery EquipmentBattery)
        {
            var EquipmentBatteryObj = db.EquipmentBattery.Where(x => x.EquipmentBatteryId == EquipmentBattery.EquipmentBatteryId).FirstOrDefault();
            if (EquipmentBattery != null)
            {

                db.EquipmentBattery.Attach(EquipmentBatteryObj);
                db.EquipmentBattery.Remove(EquipmentBatteryObj);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted EquipmentBattery number " + EquipmentBattery.EquipmentBatteryId);

            }

            return Json(new[] { EquipmentBattery }.ToDataSourceResult(request));
        }


        [AllowAnonymous]
        public ActionResult BatteryByFleet(string Id)
        {

            var Batterys = db.EquipmentBattery.Where(x => x.fleetId.ToString() == Id && x.Isattached == true).Select(e => new

            {
                Value = e.SerialNo,
                Text = e.SerialNo + "-" + e.Make,


            }).ToList();

            return Json(Batterys, JsonRequestBehavior.AllowGet);
        }




        [AllowAnonymous]
        public ActionResult BatteryByFleetByPlate(string Id)
        {
            var Equipment = db.Fleets.Where(x => x.PlateNumber == Id).FirstOrDefault();
            if (Equipment != null)
            {
                var Batterys = db.EquipmentBattery.Where(x => x.fleetId == Equipment.FleetsID && x.Isattached == true).Select(e => new

                {
                    Value = e.EquipmentBatteryId,
                    Text = e.SerialNo + "-" + e.Make,


                }).ToList();
                return Json(Batterys, JsonRequestBehavior.AllowGet);
            }
            return Json(string.Empty, JsonRequestBehavior.AllowGet);

        }
        [AllowAnonymous]
        public ActionResult GetBatteryInfo(Guid Id)
        {
            string Response = string.Empty;
            decimal OdomenterDiff = 0;
            var Battery = db.EquipmentBattery.Where(x => x.EquipmentBatteryId == Id && x.Isattached == true).FirstOrDefault();
            if (Battery != null)
            {
                var Fleet = db.Fleets.Where(x => x.FleetsID == Battery.fleetId).FirstOrDefault();
                if (Fleet != null)
                {
                    OdomenterDiff = decimal.Parse(Fleet.CurentOdoMeter.ToString()) - Battery.Odometer;

                    Response = Battery.Make + "_" + Battery.Type + "_" + OdomenterDiff + "_" + Battery.UnitPrice + "_" + DateTime.Parse(Battery.DateCreated.ToString()).Date.ToString("dd/mm/yyyy") + "_" + Battery.StoreCode;
                }
            }
            return Json(Response, JsonRequestBehavior.AllowGet);
        }

    }
}