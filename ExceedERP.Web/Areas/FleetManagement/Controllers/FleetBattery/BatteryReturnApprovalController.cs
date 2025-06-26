using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using System.Data.Entity;
using ExceedERP.Core.Domain.Common;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.BatteryManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class BatteryReturnApprovalController : Controller
    {
        //
        ExceedDbContext db = new ExceedDbContext();


        [AuthorizeRoles(FleetManagementRoles.ResourceApprover, FleetManagementRoles.ResourceApprover, FleetManagementRoles.ResourceApprover)]
        public ActionResult BatteryReturnApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<BatteryApproval> BatteryApproval = db.BatteryApproval.Where(x => x.BatteryId == Id).ToList();
            List<BatteryReturn> Category = db.BatteryReturn.Where(x => x.BatteryReturnId == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                BatteryApproval.Add(new BatteryApproval()
                {
                    BatteryId = Id,
                    Remark = string.Empty,
                    Status = ApprovalStatuses.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                });
            }


            return Json(BatteryApproval.ToDataSourceResult(request, approval => new BatteryApproval()
            {
                BatteryApprovalId = approval.BatteryApprovalId,
                BatteryId = Id,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        [AuthorizeRoles(FleetManagementRoles.RentRequestor, FleetManagementRoles.RentUser)]

        public ActionResult BatteryReturn_SendForOnlineApproval(Guid Id)
        {
             
            var BatteryReturn = db.BatteryReturn.Where(x => x.BatteryReturnId == Id).FirstOrDefault();
            if (BatteryReturn != null)
            {
                BatteryReturn.IsSendForApproval = true;
                BatteryReturn.Status = MaintenanceConst.WaitingApproval;
                db.BatteryReturn.Attach(BatteryReturn);
                db.Entry(BatteryReturn).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Battrey Return", "Success", User.Identity.Name, $"created issue {BatteryReturn.BatteryReturnId} ", Modules.FleetManagement);
                #endregion
            }


            return RedirectToActionPermanent("Index", "BatteryReturn", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.RentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BatteryReturn_Approve([DataSourceRequest]DataSourceRequest request, BatteryApproval BatteryReturnApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (BatteryReturnApproval.Status == ApprovalStatuses.Approve)
                {
                 
                    var entity = db.BatteryReturn.Where(x => x.BatteryReturnId == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;
                    db.BatteryReturn.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    

                    var EquipmentBattery = db.EquipmentBattery.Where(x => x.EquipmentBatteryId == entity.BatteryId).FirstOrDefault();
                    var Fleet = db.Fleets.Where(x => x.FleetsID == EquipmentBattery.fleetId).FirstOrDefault();
               
                    EquipmentBattery.Isattached = false;
                    EquipmentBattery.DateCreated = DateTime.Now;
                    
                    db.EquipmentBattery.Attach(EquipmentBattery);
                    db.Entry(EquipmentBattery).State = EntityState.Modified;
                    db.SaveChanges();
                    

                    var tyrelog = new EquipmentBatteryLog
                    {
                        EquipmentBatteryLogId = Guid.NewGuid(),
                        FleetId = Fleet.FleetsID,
                        StoreCode = EquipmentBattery.StoreCode,
                        Condition = EquipmentBattery.Condition,
                        CreatedBy = User.Identity.Name,
                        IntialOdoMeter = EquipmentBattery.Odometer,
                        FinalOdoMeter = decimal.Parse(Fleet.CurentOdoMeter.ToString()),
                        DateCreated = DateTime.Now,
                        Make = entity.Brand,
                        ProjectName = Fleet.Location,
                        Reason = entity.DescriptionReason,
                        SerialNo = entity.SerialNo.ToString(),
                        Name = Fleet.UnitNum+" - "+Fleet.PlateNumber,
                        Type = entity.BatteryType,
                        UnitPrice = entity.UnitPrice,
                        Efficency = (decimal.Parse(Fleet.CurentOdoMeter.ToString()) - EquipmentBattery.Odometer) / entity.UnitPrice

                    };
                    db.EquipmentBatteryLog.Add(tyrelog);
                    db.SaveChanges();
                   
                }

                if (BatteryReturnApproval.Status == ApprovalStatuses.Reject)
                {
                    var entity = db.BatteryReturn.Where(x => x.BatteryReturnId == Id).FirstOrDefault();

                    entity.IsSendForApproval = false;

                    db.BatteryReturn.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                   

                }

                var approval = new BatteryApproval()
                {

                    BatteryApprovalId = Guid.NewGuid(),
                    BatteryId = Id,
                    Remark = BatteryReturnApproval.Remark,
                    DateCreated = DateTime.Now,
                    Status = BatteryReturnApproval.Status,
                    CreatedBy = User.Identity.Name

                };

                db.BatteryApproval.Add(approval);
                db.SaveChanges();
                BatteryReturnApproval.BatteryApprovalId = approval.BatteryApprovalId;
                

            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { BatteryReturnApproval }.ToDataSourceResult(request));
        }




    }
}
