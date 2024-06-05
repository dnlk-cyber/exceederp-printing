using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.FleetManagement.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using System.Data.Entity;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class DriverAssignmentApprovalController : Controller
    {
        //
        // GET: /FleetManagement/DriverAssignmentApproval/
        ExceedDbContext db = new  ExceedDbContext();

   
        public ActionResult Index()
        {
           
            return View();
        }
         //    [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover)]
        public ActionResult DriverVehicleTransferApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            
            var driveVehicleTransferApprovals = db.DriverVehicleTransferApproval.Where(x => x.DriverAssignmentId == Id);

            var result = driveVehicleTransferApprovals.ToDataSourceResult(request, driveVehicleTransferApproval => new DriverTransferApproval
            {
                DriverAssignmentId = driveVehicleTransferApproval.DriverAssignmentId,
                CreatedBy = driveVehicleTransferApproval.CreatedBy,
                DriverTransferApprovalID = driveVehicleTransferApproval.DriverTransferApprovalID,
                DateCreated = driveVehicleTransferApproval.DateCreated,
                DriverVehicleTransferId = driveVehicleTransferApproval.DriverVehicleTransferId,
                LastModified = driveVehicleTransferApproval.LastModified,
                ModifiedBy = driveVehicleTransferApproval.ModifiedBy,
                OrgBranchName = driveVehicleTransferApproval.OrgBranchName,
                Remark = driveVehicleTransferApproval.Remark,
                Status = driveVehicleTransferApproval.Status
            });
            return Json(result);

        }



        public ActionResult DriverVehicleTransfer_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var driverAssignement = db.DriverAssignments.Where(x => x.DriverAssignmentId == Id).FirstOrDefault();
            if (driverAssignement != null)
            {
                driverAssignement.IsSendForApproval = true;
                driverAssignement.Status = MaintenanceConst.WaitingApproval;
                db.DriverAssignments.Attach(driverAssignement);
                db.Entry(driverAssignement).State = EntityState.Modified;
                db.SaveChanges();
            }
        
            return RedirectToActionPermanent("Index", "DriverAssignment", new { area = "FleetManagement" });
        }

        //[AuthorizeRoles(FleetManagementRoles.DriverVehicleTransferApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DriverVehicleTransfer_Approve([DataSourceRequest]DataSourceRequest request, DriverTransferApproval DriverVehicleTransferApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (DriverVehicleTransferApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.DriverAssignments.Where(x => x.DriverAssignmentId == Id).FirstOrDefault();
                    var fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetId).FirstOrDefault();
                         
                        
                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;
                    entity.Odometer = fleet.CurentOdoMeter;
                    db.DriverAssignments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();


                    fleet.OperatorName = entity.ToOperatorName;
                    db.DriverAssignments.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                }

                var approval = new DriverTransferApproval()
                {

                    Status = DriverVehicleTransferApproval.Status,
                    DriverVehicleTransferId = Id,
                         
                    Remark = DriverVehicleTransferApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    DriverAssignmentId = Id

                };
                
            db.DriverVehicleTransferApproval.Add(approval);
            db.SaveChanges();

            }
            catch (Exception e)
            {

            }
            return Json(new[] { DriverVehicleTransferApproval }.ToDataSourceResult(request));
        }




    }
  }