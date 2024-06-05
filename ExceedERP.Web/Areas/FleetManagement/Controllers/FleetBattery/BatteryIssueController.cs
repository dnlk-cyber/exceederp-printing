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
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class BatteryIssueController : Controller
    {
     
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/AlertLevel
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult BatteryIssue_Read([DataSourceRequest] DataSourceRequest request)
        {
            var BatteryIssue = db.BatteryIssue.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).ToList();

            return Json(BatteryIssue.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BatteryIssue_Create([DataSourceRequest] DataSourceRequest request, BatteryIssue BatteryIssue)
        {
            ModelState.Remove("BatteryIssueId");

            try
            { 
            var Battery = db.BatteryReturn.Where(x => x.BatteryReturnId.ToString() == BatteryIssue.SerialNo).FirstOrDefault();
            var Fleet = db.Fleets.Where(x => x.PlateNumber == BatteryIssue.PlateNumber).FirstOrDefault();
       
            var entitybattery = new EquipmentBattery
            {
                EquipmentBatteryId = Guid.NewGuid(),
                 fleetId = Fleet.FleetsID,
                Type = Battery.BatteryType,
                Make = Battery.Brand,
                MakeCountry = Battery.MakeCountry,
                Condition = Battery.Condition,
                      Isattached = false,
                SerialNo = Battery.SerialNo,
                StoreCode = Battery.StoreCode,
                UnitPrice = Battery.UnitPrice,
                Odometer =decimal.Parse( Fleet.CurentOdoMeter.ToString()),
                  DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name,
                 OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)
              
            };
            db.EquipmentBattery.Add(entitybattery);
            db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Battery issue registration", "Success", User.Identity.Name, $"created battrey issue {entitybattery.EquipmentBatteryId} ", Modules.FleetManagement);
                #endregion

                var entity = new BatteryIssue
            {
                BatteryIssueId = Guid.NewGuid(),
                BatteryReturnId = Battery.BatteryReturnId,
                EquipmentBatteryId = entitybattery.EquipmentBatteryId,
                BatteryType = Battery.BatteryType,
                SerialNo = Battery.SerialNo,
                UnitPrice = Battery.UnitPrice,
                StoreCode = Battery.StoreCode,
                Brand = Battery.Brand,
                InstalledDate = Battery.InstalledDate,
                    KMReading = Battery.KMReading,
                DescriptionReason = BatteryIssue.DescriptionReason,
                PlateNumber = Fleet.PlateNumber,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name,
                Status = MaintenanceConst.Pending
                
            };
            db.BatteryIssue.Add(entity);
            db.SaveChanges();

            #region operation log
            UserHelper.OperationLog(Request.UserHostAddress, "Battrey issue", "Success", User.Identity.Name, $"updated battrey issue {entity.BatteryIssueId} ", Modules.FleetManagement);
            #endregion



            }
            catch (Exception e)
            {
                
            }
            return Json(new[] { BatteryIssue }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BatteryIssue_Update([DataSourceRequest] DataSourceRequest request, BatteryIssue BatteryIssue)
        {
            if (BatteryIssue != null)
            {
                BatteryIssue.ModifiedBy = User.Identity.Name;
                BatteryIssue.LastModified = DateTime.Now;
                BatteryIssue.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
              
                db.BatteryIssue.Attach(BatteryIssue);
                db.Entry(BatteryIssue).State = EntityState.Modified;
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted battrey issue number {BatteryIssue.BatteryIssueId }");
                #endregion
            }

            return Json(new[] { BatteryIssue }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BatteryIssue_Destroy([DataSourceRequest] DataSourceRequest request, BatteryIssue BatteryIssue)
        {
            var BatteryIssues = db.BatteryIssue.Where(x => x.BatteryIssueId == BatteryIssue.BatteryIssueId).FirstOrDefault();
            if (BatteryIssues != null)
            {

                db.BatteryIssue.Attach(BatteryIssues);
                db.BatteryIssue.Remove(BatteryIssues);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted BatteryIssue number " + BatteryIssue.BatteryIssueId);

            }

            return Json(new[] { BatteryIssue }.ToDataSourceResult(request));
        }


        [AllowAnonymous]
        public ActionResult BatteryForIssue()
        {

            var Batterys = db.BatteryReturn.Where(x => x.IsOnlineApproved == true && x.IsOnlineTransferred == false &&  x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)             ).Select(e => new

            {
                SerialNo = e.SerialNo,
                Value = e.BatteryReturnId,
                Text = e.SerialNo + "-" + e.Brand,


            }).ToList();
            var Issues = db.BatteryIssue.ToList();

            foreach (var Issue in Issues)
            {
                var Battery = Batterys.Where(x => x.SerialNo == Issue.SerialNo).FirstOrDefault();
                if(Battery != null)
                {
                    Batterys.Remove(Battery);
                }
            }

            return Json(Batterys, JsonRequestBehavior.AllowGet);
        }

      


    }
}
