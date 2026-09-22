using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class BatteryUsageHistoryController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        //
        // GET: /FleetManagement/BatteryUsageHistory/
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BatteryUsageHistory_Read([DataSourceRequest] DataSourceRequest request)
        {
            var EquipmentBatteryLog = db.EquipmentBatteryLog.Select(s => new {
            
                Condition = s.Condition,
                Efficency = s.Efficency,
                FinalOdoMeter =s.FinalOdoMeter,
                IntialOdoMeter = s.IntialOdoMeter,
                Make= s.Make,
                MakeCountry= s.MakeCountry,
                ProjectName = s.ProjectName,
                SerialNo = s.SerialNo,
                Size = s.Size,
                StoreCode = s.StoreCode,
                Type = s.Type,
                UnitPrice = s.UnitPrice,
                Name=s.Name,//GetFleetDetail(s.FleetId),
                Reason = (s.FinalOdoMeter- s.IntialOdoMeter).ToString()
            
            }).ToList();

            return Json(EquipmentBatteryLog.ToDataSourceResult(request));
        }
	  
        public string GetFleetDetail(Guid Id)
        {
            var Fleet = db.Fleets.Where(x => x.FleetsID == Id).FirstOrDefault();
            if(Fleet!= null)
            {
                return Fleet.UnitNum + " - " + Fleet.PlateNumber;
            }
            return string.Empty;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}