using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceApprover)]
    public class DriverVehicleTransferApprovalController : Controller
    {
        //
        // GET: /FleetManagement/DriverVehicleTransferApproval/
        ExceedDbContext db = new ExceedDbContext();
        IDriverVehicleTransferService _DriverVehicleTransfersService;
        private IOperatorService _operatorService;
        private IFleetService _FleetService;

        public DriverVehicleTransferApprovalController()
        {
            _FleetService = new FleetService();
            _DriverVehicleTransfersService = new DriverVehicleTransferService();
            _operatorService = new OperatorService();
        }

        //    [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceApprover)]
        public ActionResult DriverVehicleTransferApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<DriverTransferApproval> fleetApproval = db.DriverVehicleTransferApproval.Where(x => x.DriverVehicleTransferId == Id).ToList();
            List<DriverVehicleTransfer> Category = db.DriverVehicleTransfer.Where(x => x.DriverVehicleTransferID == Id).ToList();
            bool SendForApproval = Category.FirstOrDefault().IsSendForApproval;
            if (SendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new DriverTransferApproval()
                {
                    DriverVehicleTransferId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name


                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new DriverTransferApproval()
            {
                DriverTransferApprovalID = approval.DriverTransferApprovalID,
                DriverVehicleTransferId = approval.DriverVehicleTransferId,
                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



        public ActionResult DriverVehicleTransfer_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var DriverVehicleTransfer = db.DriverVehicleTransfer.Where(x => x.DriverVehicleTransferID == Id).FirstOrDefault();
            if (DriverVehicleTransfer != null)
            {
                DriverVehicleTransfer.IsSendForApproval = true;
                DriverVehicleTransfer.Status = MaintenanceConst.WaitingApproval;
                db.DriverVehicleTransfer.Attach(DriverVehicleTransfer);
                db.Entry(DriverVehicleTransfer).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "DriverTransfer", new { area = "FleetManagement" });
        }

        //        [AuthorizeRoles(FleetManagementRoles.DriverVehicleTransferApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DriverVehicleTransfer_Approve([DataSourceRequest]DataSourceRequest request, DriverTransferApproval DriverVehicleTransferApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                if (DriverVehicleTransferApproval.Status == MaintenanceConst.Approved)
                {
                    var entity = db.DriverVehicleTransfer.Where(x => x.DriverVehicleTransferID == Id).FirstOrDefault();

                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;

                    db.DriverVehicleTransfer.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();


                    Fleets fromFleet =
                        _FleetService.GetAll()
                            .Where(x => x.FleetsID == entity.FromEquipment)
                            .FirstOrDefault();
                    Fleets toFleet =
                      _FleetService.GetAll()
                            .Where(x => x.FleetsID == entity.ToEquipment)
                            .FirstOrDefault();

                    fromFleet.OperatorName = entity.ToOperatorId;
                    fromFleet.LastModified = DateTime.Now;

                    toFleet.OperatorName = entity.FromOperatorId;
                    toFleet.LastModified = DateTime.Now;

                    _FleetService.Update(fromFleet);

                    _FleetService.Update(toFleet);

                    var AssignmentFrom = new DriverAssignment
                    {

                        DriverAssignmentId = Guid.NewGuid(),
                        FleetId = fromFleet.FleetsID,
                        EquipmentName = fromFleet.PlateNumber + " - " + fromFleet.EquipmentName,
                        FromOperatorName = fromFleet.OperatorName,
                        ToOperatorName = toFleet.OperatorName,
                        CreatedBy = User.Identity.Name,
                        Status = MaintenanceConst.Pending,
                        StartingDate = entity.StartingDate,
                        ToDate = entity.ToDate,
                        Odometer = fromFleet.CurentOdoMeter,
                        DateCreated = DateTime.Now,

                    };
                    db.DriverAssignments.Add(AssignmentFrom);
                    db.SaveChanges();

                    var AssignmentTo = new DriverAssignment
                    {

                        DriverAssignmentId = Guid.NewGuid(),
                        FleetId = toFleet.FleetsID,
                        EquipmentName = toFleet.PlateNumber + " - " + toFleet.EquipmentName,
                        FromOperatorName = toFleet.OperatorName,
                        ToOperatorName = fromFleet.OperatorName,
                        CreatedBy = User.Identity.Name,
                        Status = MaintenanceConst.Pending,
                        StartingDate = entity.StartingDate,
                        ToDate = entity.ToDate,
                        Odometer = toFleet.CurentOdoMeter,
                        DateCreated = DateTime.Now,

                    };
                    db.DriverAssignments.Add(AssignmentTo);
                    db.SaveChanges();
                }
                if (DriverVehicleTransferApproval.Status == MaintenanceConst.Rejected)
                {
                    var entity = db.DriverVehicleTransfer.Where(x => x.DriverVehicleTransferID == Id).FirstOrDefault();


                    entity.IsSendForApproval = false;

                    db.DriverVehicleTransfer.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new DriverTransferApproval()
                {

                    Status = DriverVehicleTransferApproval.Status,
                    DriverVehicleTransferId = Id,
                    Remark = DriverVehicleTransferApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.DriverVehicleTransferApproval.Add(approval);
                db.SaveChanges();
                DriverVehicleTransferApproval.DriverTransferApprovalID = approval.DriverTransferApprovalID;

            }
            catch (Exception e)
            {

             
            }
            return Json(new[] { DriverVehicleTransferApproval }.ToDataSourceResult(request));
        }

    }
}