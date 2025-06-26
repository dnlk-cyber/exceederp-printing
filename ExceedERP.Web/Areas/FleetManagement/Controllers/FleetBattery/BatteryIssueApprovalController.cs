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
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.BatteryManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class BatteryIssueApprovalController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();


        [AuthorizeRoles(FleetManagementRoles.ResourceApprover, FleetManagementRoles.ResourceApprover, FleetManagementRoles.ResourceApprover)]
        public ActionResult BatteryIssueApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<BatteryApproval> BatteryApproval = db.BatteryApproval.Where(x => x.BatteryId == Id).ToList();
            List<BatteryIssue> Category = db.BatteryIssue.Where(x => x.BatteryIssueId == Id).ToList();
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

        public ActionResult BatteryIssue_SendForOnlineApproval(Guid Id)
        {
             
            var BatteryIssue = db.BatteryIssue.Where(x => x.BatteryIssueId == Id).FirstOrDefault();
            if (BatteryIssue != null)
            {
                BatteryIssue.IsSendForApproval = true;
                BatteryIssue.Status = MaintenanceConst.WaitingApproval;
                db.BatteryIssue.Attach(BatteryIssue);
                db.Entry(BatteryIssue).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "BatteryIssue", new { area = "FleetManagement" });
        }

        [AuthorizeRoles(FleetManagementRoles.RentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult BatteryIssue_Approve([DataSourceRequest]DataSourceRequest request, BatteryApproval BatteryIssueApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (BatteryIssueApproval.Status == ApprovalStatuses.Approve)
                {
                    var entity = db.BatteryIssue.Where(x => x.BatteryIssueId == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;
                    db.BatteryIssue.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    var EquipmentBattery = db.EquipmentBattery.Where(x => x.EquipmentBatteryId == entity.EquipmentBatteryId).FirstOrDefault();
                   
                    EquipmentBattery.Isattached = true;

                    db.EquipmentBattery.Attach(EquipmentBattery);
                    db.Entry(EquipmentBattery).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Educational Level Registration", "Success", User.Identity.Name, $"created issue {EquipmentBattery.EquipmentBatteryId} ", Modules.FleetManagement);
                    #endregion

                    var BatteryReturn = db.BatteryReturn.Where(x => x.BatteryReturnId == entity.BatteryReturnId).FirstOrDefault();

                    BatteryReturn.IsOnlineTransferred = true;

                    db.BatteryReturn.Attach(BatteryReturn);
                    db.Entry(BatteryReturn).State = EntityState.Modified;
                    db.SaveChanges();

                }
                if (BatteryIssueApproval.Status == ApprovalStatuses.Reject)
                {
                    var entity = db.BatteryIssue.Where(x => x.BatteryIssueId == Id).FirstOrDefault();

                    entity.IsSendForApproval = false;

                    db.BatteryIssue.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                    #region operation log
                    UserHelper.OperationLog(Request.UserHostAddress, "Educational Level Registration", "Success", User.Identity.Name, $"created issue {entity.EquipmentBatteryId} ", Modules.FleetManagement);
                    #endregion
                }
                var approval = new BatteryApproval()
                {

                    BatteryApprovalId = Guid.NewGuid(),
                    BatteryId = Id,
                    Remark = BatteryIssueApproval.Remark,
                    DateCreated = DateTime.Now,
                    Status = BatteryIssueApproval.Status,
                    CreatedBy = User.Identity.Name

                };

                db.BatteryApproval.Add(approval);
                db.SaveChanges();
                BatteryIssueApproval.BatteryApprovalId = approval.BatteryApprovalId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Educational Level Registration", "Success", User.Identity.Name, $"created issue {approval.BatteryApprovalId} ", Modules.FleetManagement);
                #endregion


            }
            catch (Exception e)
            {

                string ee = e.Message;
            }
            return Json(new[] { BatteryIssueApproval }.ToDataSourceResult(request));
        }




    }
}
