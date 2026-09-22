using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetResources
{
    
    public class AssignedEquipmentsController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        // GET: FleetManagement/AssignedEquipments
        //public ActionResult Index(FleetReturnType type)
        public ActionResult Index()
        {

          //  ViewBag.Type = type == FleetReturnType.R ? "Fleet Return" : "Assigned Equipment";
          
            ViewData["EquipmentNames"] = CacheData.GetEquipmentNameCaches().Select(
       i => new
       {
           Id = i.Name,
           Name = i.Id
       }).AsQueryable();
            ViewData["FleetsAssignedEquip"] = db.Fleets.Select(s => new
            {
                Id = s.FleetsID,
                Name = s.PlateNumber
            });
            ViewData["Operators"] = db.Operator.Select(s => new {
                Id = s.OperatorID,
                Name = s.FullName
            });
            
            return View();
        }
        
        public ActionResult AssignedEquipment_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<AssignedEquipment> assignedEquips;
            var companySetting = db.RequestToInspections.FirstOrDefault();
            if (companySetting != null && companySetting.Remark == "MCG")
            {
                assignedEquips = db.AssignedEquipment.Where(x => x.IsAssigned || x.Returned==true);
                //assignedEquips = db.AssignedEquipment.Where(x =>x.IsArrived);
            }
            else
            {
                assignedEquips = db.AssignedEquipment.Where(x => x.IsAssigned);
            }
            var result = assignedEquips.ToDataSourceResult(request, model => new AssignedEquipment
            {
                AssignedEquipmentID = model.AssignedEquipmentID,
                AssignedDate = model.AssignedDate,
                EquipmentNameID = model.EquipmentNameID,
                FleetId = model.FleetId,
                OperatorId = model.OperatorId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                ArrivalPlace = model.ArrivalPlace,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                Destination = model.Destination,
                EndOdometer = model.EndOdometer,
                EquipmentStatus = model.EquipmentStatus,
                IsArrived = model.IsArrived,
                IsAssigned = model.IsAssigned,
                IsOnlineApproved = model.IsOnlineApproved,
                IsOnlineTransferred = model.IsOnlineTransferred,
                IsOnlineTransferredBy = model.IsOnlineTransferredBy,
                IsSendForApproval = model.IsSendForApproval,
                IsVoid = model.IsVoid,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                NetOdometer = model.NetOdometer,
                NumberHourPerKm = model.NumberHourPerKm,
                NumberofPersonOrTon = model.NumberofPersonOrTon,
                OnlineApprovedBy = model.OnlineApprovedBy,
                OnlineApprovedTime = model.OnlineApprovedTime,
                OnlineTransferredTime = model.OnlineTransferredTime,
                OrgBranchName = model.OrgBranchName,
                Quantity = model.Quantity,
                Remark = model.Remark,
                RequestDesc = model.RequestDesc,
                RequestedDate = model.RequestedDate,
                RequestedEquipmentId = model.RequestedEquipmentId,
                RequestNumber = model.RequestNumber,
                ReturnDate = model.ReturnDate,
                Returned = model.Returned,
                SendForApprovalBy = model.SendForApprovalBy,
                SendForApprovalTime = model.SendForApprovalTime,
                StartOdometr = model.StartOdometr,
                Status = model.Status,
                VehicleDemandId = model.VehicleDemandId,
                VoidBy = model.VoidBy,
                VoidTime = model.VoidTime,
                
                EquipmentName = model.EquipmentName,
                
                
            });
            return Json(result);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}