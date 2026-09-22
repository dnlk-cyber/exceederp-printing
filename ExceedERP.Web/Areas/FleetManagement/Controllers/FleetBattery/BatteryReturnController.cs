using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Core.Domain.FleetManagement.BatteryManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System.Data.Entity.Validation;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class BatteryReturnController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/AlertLevel
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BatteryReturn_Read([DataSourceRequest] DataSourceRequest request)
        {
            var BatteryReturn = db.BatteryReturn.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).ToList();

            return Json(BatteryReturn.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BatteryReturn_Create([DataSourceRequest] DataSourceRequest request, BatteryReturn BatteryReturn)
        {
            ModelState.Remove("BatteryReturnId");
            ModelState.Remove("BatteryId");
            try
            {
                var Battery = db.EquipmentBattery.Where(x => x.EquipmentBatteryId.ToString() == BatteryReturn.SerialNo).FirstOrDefault();
                var Fleet = db.Fleets.Where(x => x.FleetsID == Battery.fleetId).FirstOrDefault();
                var entity = new BatteryReturn
                {
                    BatteryReturnId = Guid.NewGuid(),
                    BatteryId = Battery.EquipmentBatteryId,
                    BatteryType = Battery.Type,
                    StoreCode = Battery.StoreCode,
                    Brand = Battery.Make,
                    UnitPrice = Battery.UnitPrice,
                    InstalledDate = Battery.DateCreated,
                    SerialNo = Battery.SerialNo,
                    PlateNumber = Fleet.PlateNumber,
                    KMReading = (int)Fleet.CurentOdoMeter - (int)Battery.Odometer,
                    DescriptionReason = BatteryReturn.DescriptionReason,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    Status = MaintenanceConst.Pending,
                      OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
              
                };
                db.BatteryReturn.Add(entity);
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Battrey Return Registration", "Success", User.Identity.Name, $"created battrey return {entity.BatteryReturnId} ", Modules.FleetManagement);
                #endregion



                BatteryReturn = entity;
            }
            catch ( Exception e)
            {
              
            }
            return Json(new[] { BatteryReturn }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BatteryReturn_Update([DataSourceRequest] DataSourceRequest request, BatteryReturn BatteryReturn)
        {
            if (BatteryReturn != null)
            {
                BatteryReturn.ModifiedBy = User.Identity.Name;
                BatteryReturn.LastModified = DateTime.Now;
                BatteryReturn.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
              
                db.BatteryReturn.Attach(BatteryReturn);
                db.Entry(BatteryReturn).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Battrey Return", "Success", User.Identity.Name, $"updated battrey return {BatteryReturn.BatteryReturnId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { BatteryReturn }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BatteryReturn_Destroy([DataSourceRequest] DataSourceRequest request, BatteryReturn BatteryReturn)
        {
            var BatteryReturns = db.BatteryReturn.Where(x => x.BatteryReturnId == BatteryReturn.BatteryReturnId).FirstOrDefault();
            if (BatteryReturns != null)
            {

                db.BatteryReturn.Attach(BatteryReturns);
                db.BatteryReturn.Remove(BatteryReturns);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted BatteryReturn number " + BatteryReturn.BatteryReturnId);

            }

            return Json(new[] { BatteryReturn }.ToDataSourceResult(request));
        }

        
        public ActionResult GetReturnBatteryInfo(Guid Id)
        {
            string Response = string.Empty;

            var Battery = db.BatteryReturn.Where(x => x.BatteryReturnId == Id && x.IsOnlineApproved == true).FirstOrDefault();
            if (Battery != null)
            {


                Response = Battery.Brand + "_" + Battery.BatteryType + "_" + Battery.KMReading + "_" + Battery.UnitPrice + "_" + DateTime.Parse(Battery.DateCreated.ToString()).Date.ToString("dd/mm/yyyy") + "_" + Battery.StoreCode;
            }

            return Json(Response, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
