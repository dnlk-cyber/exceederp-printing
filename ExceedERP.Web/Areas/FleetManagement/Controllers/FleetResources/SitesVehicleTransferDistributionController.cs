using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using System.Data.Entity;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FixedAsset.Registrations;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    // Stage 2 of the site vehicle transfer workflow: the request receiver picks an approved request
    // by request number and assigns a specific vehicle, then it is checked and approved before going to fleet.
    [AuthorizeRoles(FleetManagementRoles.ResourceDistributor, FleetManagementRoles.ResourceDistributionChecker, FleetManagementRoles.ResourceDistributionApprover)]
    public class SitesVehicleTransferDistributionController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {
            Populate();
            return View();
        }
        public ActionResult FleetIndex()
        {
            Populate();
            return View();
        }
        private void Populate()
        {
            ViewData["Branch"] = CacheData.GetBranchCaches().Select(i => new { Name = i.Name, BranchId = i.Id }).AsQueryable();
            ViewData["EquipmentCategory"] = CacheData.GetEquipmentIdentityCaches().Select(i => new { Text = i.Name, Value = i.Id }).AsQueryable();
            ViewData["EquipmentType"] = CacheData.GetEquipmentTypeCaches().Select(i => new { Text = i.Name, Value = i.Id }).AsQueryable();
            ViewData["EquipmentName"] = CacheData.GetEquipmentNameCaches().Select(i => new { Text = i.Name, Value = i.Id }).AsQueryable();
            ViewData["Operator"] = db.Operator.Select(s => new { Value = s.OperatorID, Text = s.EmployeeCode + " " + s.FullName }).ToList();

        }
        public ActionResult Distribution_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<SitesVehicleTransfer> SVT = db.SitesVehicleTransfer.Where(x => x.IsRequestApproved);
            DataSourceResult Result = SVT.ToDataSourceResult(request, s => new
            {
                SitesVehicleTransferID = s.SitesVehicleTransferID,
                RequestNumber = s.RequestNumber,
                RequestDate = s.RequestDate,
                RequestedBy = s.RequestedBy,
                OrgBranchName = s.OrgBranchName,
                EquipmentIdentityID = s.EquipmentIdentityID,
                EquipmentTypeID = s.EquipmentTypeID,
                EquipmentNameID = s.EquipmentNameID,
                FleetsID = s.FleetsID,
                PlateNo = s.Fleet != null ? s.Fleet.PlateNumber : null,
                InitialSite = s.InitialSite,
                BranchId = s.BranchId,
                DestinationSite = s.DestinationSite,
                StartingDate = s.StartingDate,
                ToDate = s.ToDate,
                TransferReason = s.TransferReason,
                OdoMeter = s.OdoMeter,
                OperatorId = s.OperatorId,
                SecondOperatorId = s.SecondOperatorId,
                FuelAmount = s.FuelAmount,
                BatteryStatus = s.BatteryStatus,
                EquipmentCondition = s.EquipmentCondition,
                TransferMode = s.TransferMode,
                Hydrolic = s.Hydrolic,
                BatteryAmpere = s.BatteryAmpere,
                BatteryQuantity = s.BatteryQuantity,
                Description = s.Description,
                Status = s.Status,
                Remark = s.Remark,
                IsRequestReceived = s.IsRequestReceived,
                RequestReceivedBy = s.RequestReceivedBy,
                RequestReceivedDate = s.RequestReceivedDate,
                IsDistributionChecked = s.IsDistributionChecked,
                DistributionCheckedBy = s.DistributionCheckedBy,
                DistributionCheckedDate = s.DistributionCheckedDate,
                IsDistributionApproved = s.IsDistributionApproved,
                DistributionApprovedBy = s.DistributionApprovedBy,
                DistributionApprovedDate = s.DistributionApprovedDate,
                IsAccepted = s.IsAccepted,
                AcceptedBy = s.AcceptedBy,
                AcceptedTime = s.AcceptedTime,
                IsSendForApproval = s.IsSendForApproval,
                SendForApprovalBy = s.SendForApprovalBy,
                SendForApprovalTime = s.SendForApprovalTime,
                IsOnlineApproved = s.IsOnlineApproved,
                OnlineApprovedBy = s.OnlineApprovedBy,
                OnlineApprovedTime = s.OnlineApprovedTime,
                CreatedBy = s.CreatedBy,
                DateCreated = s.DateCreated,
                ModifiedBy = s.ModifiedBy,
                LastModified = s.LastModified
            });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Fleet_Distribution_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<SitesVehicleTransfer> SVT = db.SitesVehicleTransfer.Where(x => x.IsDistributionApproved);
            DataSourceResult Result = SVT.ToDataSourceResult(request, s => new
            {
                SitesVehicleTransferID = s.SitesVehicleTransferID,
                RequestNumber = s.RequestNumber,
                RequestDate = s.RequestDate,
                RequestedBy = s.RequestedBy,
                OrgBranchName = s.OrgBranchName,
                EquipmentIdentityID = s.EquipmentIdentityID,
                EquipmentTypeID = s.EquipmentTypeID,
                EquipmentNameID = s.EquipmentNameID,
                FleetsID = s.FleetsID,
                PlateNo = s.Fleet != null ? s.Fleet.PlateNumber : null,
                InitialSite = s.InitialSite,
                BranchId = s.BranchId,
                DestinationSite = s.DestinationSite,
                StartingDate = s.StartingDate,
                ToDate = s.ToDate,
                TransferReason = s.TransferReason,
                OdoMeter = s.OdoMeter,
                OperatorId = s.OperatorId,
                SecondOperatorId = s.SecondOperatorId,
                FuelAmount = s.FuelAmount,
                BatteryStatus = s.BatteryStatus,
                EquipmentCondition = s.EquipmentCondition,
                TransferMode = s.TransferMode,
                Hydrolic = s.Hydrolic,
                BatteryAmpere = s.BatteryAmpere,
                BatteryQuantity = s.BatteryQuantity,
                Description = s.Description,
                Status = s.Status,
                Remark = s.Remark,
                IsRequestReceived = s.IsRequestReceived,
                RequestReceivedBy = s.RequestReceivedBy,
                RequestReceivedDate = s.RequestReceivedDate,
                IsDistributionChecked = s.IsDistributionChecked,
                DistributionCheckedBy = s.DistributionCheckedBy,
                DistributionCheckedDate = s.DistributionCheckedDate,
                IsDistributionApproved = s.IsDistributionApproved,
                DistributionApprovedBy = s.DistributionApprovedBy,
                DistributionApprovedDate = s.DistributionApprovedDate,
                IsAccepted = s.IsAccepted,
                AcceptedBy = s.AcceptedBy,
                AcceptedTime = s.AcceptedTime,
                IsSendForApproval = s.IsSendForApproval,
                SendForApprovalBy = s.SendForApprovalBy,
                SendForApprovalTime = s.SendForApprovalTime,
                IsOnlineApproved = s.IsOnlineApproved,
                OnlineApprovedBy = s.OnlineApprovedBy,
                OnlineApprovedTime = s.OnlineApprovedTime,
                CreatedBy = s.CreatedBy,
                DateCreated = s.DateCreated,
                ModifiedBy = s.ModifiedBy,
                LastModified = s.LastModified
            });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetRequestDetail(Guid id)
        {
            var s = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            if (s == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var category = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == s.EquipmentIdentityID);
            var type = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == s.EquipmentTypeID);
            var name = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID);
            return Json(new
            {
                s.RequestNumber,
                RequestDate = s.RequestDate?.ToString("yyyy-MM-dd"),
                s.RequestedBy,
                RequestBranch = s.OrgBranchName,
                EquipmentCategory = category?.EquipmentCategoryName,
                EquipmentType = type?.EquipmentTypeName,
                EquipmentName = name?.Name,
                s.EquipmentNameID,
                s.DestinationSite,
                StartingDate = s.StartingDate?.ToString("yyyy-MM-dd"),
                ToDate = s.ToDate?.ToString("yyyy-MM-dd"),
                s.TransferReason,
                s.RequestApprovedBy
            }, JsonRequestBehavior.AllowGet);
        }

        // Assigns (or re-assigns) a vehicle to an approved request; the row itself is the request.
        [AuthorizeRoles(FleetManagementRoles.ResourceDistributor)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Distribution_Update([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer model)
        {
            ModelState.Clear();
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == model.SitesVehicleTransferID);
            var fleet = model.FleetsID.HasValue ? db.Fleets.FirstOrDefault(x => x.FleetsID == model.FleetsID) : null;

            if (entity == null || !entity.IsRequestApproved)
                ModelState.AddModelError("", "Only approved requests can be assigned a vehicle");
            if (entity.IsDistributionChecked)
                ModelState.AddModelError("", "Assignment can no longer be edited");
            if (fleet == null)
                ModelState.AddModelError("FleetsID", "Plate No is required");
            if (entity != null && fleet.EquipmentNameID != entity.EquipmentNameID)
                ModelState.AddModelError("FleetsID", "Selected vehicle does not match the requested equipment name");
            if (entity != null && fleet.BranchId == entity.BranchId)
               ModelState.AddModelError("FleetsID", "Initial and Destination Site must be different");

            if (ModelState.IsValid)
            {
                var previousFleetId = entity.FleetsID;
                var isFirstAssignment = !entity.IsRequestReceived;
                entity.FleetsID = model.FleetsID;
                entity.InitialSite = db.Branch.FirstOrDefault(x=>x.BranchId==fleet.BranchId)?.Name??"";
                entity.OdoMeter = string.IsNullOrWhiteSpace(model.OdoMeter) ? fleet.CurentOdoMeter.ToString() : model.OdoMeter;
                entity.OperatorId = model.OperatorId;
                entity.SecondOperatorId = model.SecondOperatorId;
                entity.FuelAmount = model.FuelAmount;
                entity.BatteryStatus = model.BatteryStatus;
                entity.DestinationSite = db.Branch.FirstOrDefault(x => x.BranchId == model.BranchId)?.Name ?? "";
                entity.EquipmentCondition = model.EquipmentCondition;
                entity.TransferMode = model.TransferMode;
                entity.Hydrolic = model.Hydrolic;
                entity.BatteryAmpere = model.BatteryAmpere;
                entity.BatteryQuantity = model.BatteryQuantity;
                entity.Description = model.Description;
                entity.IsRequestReceived = true;
                if (isFirstAssignment || string.IsNullOrEmpty(entity.RequestReceivedBy))
                {
                    entity.RequestReceivedBy = User.Identity.Name;
                    entity.RequestReceivedDate = DateTime.Now;
                }
                entity.Status = SitesVehicleTransferStatus.VehicleAssigned;
                entity.Remark = null;
                entity.ModifiedBy = User.Identity.Name;
                entity.LastModified = DateTime.Now;
                
                    try
                    {
                        db.Entry(entity).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    catch (System.Data.Entity.Infrastructure.DbUpdateException ex)
                    {
                        var sqlEx = ex.InnerException;

                        while (sqlEx.InnerException != null)
                        {
                            sqlEx = sqlEx.InnerException;
                        }

                        throw new Exception(sqlEx.Message);
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine(ex.ToString());
                        throw;
                    }

                    if (previousFleetId != fleet.FleetsID)
                {
                    SnapshotTyres(entity);
                }
                    fleet.Location = model.DestinationSite;
                    fleet.BranchId = model.BranchId;
                    db.Entry(fleet).State = EntityState.Modified;
                    db.SaveChanges();
                //UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer distribution", "Success", User.Identity.Name, $"assigned {fleet.PlateNumber} to request {entity.RequestNumber}", Modules.FleetManagement);
                //entity.SitesVehicleTransferID = fleet;
               // model = entity;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        // Record the tyres attached to the vehicle at the time of assignment
        private void SnapshotTyres(SitesVehicleTransfer entity)
        {
            var existing = db.SiteVechicleTransferTyreDetailes.Where(x => x.SiteVechicleTransferId == entity.SitesVehicleTransferID).ToList();
            db.SiteVechicleTransferTyreDetailes.RemoveRange(existing);
            var tyres = db.EquipmentTyres.Where(x => x.fleetId == entity.FleetsID && x.Isattached).ToList();
            foreach (var tyre in tyres)
            {
                db.SiteVechicleTransferTyreDetailes.Add(new SiteVechicleTransferTyreDetail
                {
                    SiteVechicleTransferId = entity.SitesVehicleTransferID,
                    SerialNo = tyre.SerialNo,
                    Type = tyre.Type,
                    Description = tyre.Make + " " + tyre.Condition,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now
                });
            }
            db.SaveChanges();
        }

        [AuthorizeRoles(FleetManagementRoles.ResourceDistributionChecker)]
        [HttpPost]
        public JsonResult CheckDistribution(Guid id)
        {
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            if (entity == null || !entity.IsRequestReceived || entity.IsDistributionChecked)
                return Json(new { Message = "A vehicle must be assigned before checking", Status = "Error" });

            entity.IsDistributionChecked = true;
            entity.DistributionCheckedBy = User.Identity.Name;
            entity.DistributionCheckedDate = DateTime.Now;
            entity.Status = SitesVehicleTransferStatus.DistributionChecked;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
           // UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer distribution", "Success", User.Identity.Name, $"checked distribution {entity.RequestNumber}", Modules.FleetManagement);
            return Json(new { Message = "Distribution Checked", Status = "OK" });
        }

        [AuthorizeRoles(FleetManagementRoles.ResourceDistributionApprover)]
        [HttpPost]
        public JsonResult ApproveDistribution(Guid id)
        {
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            if (entity == null || !entity.IsDistributionChecked || entity.IsDistributionApproved)
                return Json(new { Message = "Distribution must be checked before approval", Status = "Error" });

            entity.IsDistributionApproved = true;
            entity.DistributionApprovedBy = User.Identity.Name;
            entity.DistributionApprovedDate = DateTime.Now;
            // Hand over to the fleet stage (Accept -> Send For Approval -> Approve)
            entity.Status = SitesVehicleTransferStatus.WaitingFleetApproval;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            //UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer distribution", "Success", User.Identity.Name, $"approved distribution {entity.RequestNumber}", Modules.FleetManagement);
            return Json(new { Message = "Distribution Approved and sent to Fleet", Status = "OK" });
        }

        [AuthorizeRoles(FleetManagementRoles.ResourceDistributionChecker, FleetManagementRoles.ResourceDistributionApprover)]
        [HttpPost]
        public JsonResult RejectDistribution(Guid id, string remark)
        {
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            if (entity == null || entity.IsDistributionApproved)
                return Json(new { Message = "Approved distribution cannot be rejected", Status = "Error" });

            // Send back to the receiver so another vehicle can be assigned
            entity.IsRequestReceived = false;
            entity.IsDistributionChecked = false;
            entity.DistributionCheckedBy = null;
            entity.DistributionCheckedDate = null;
            entity.Status = SitesVehicleTransferStatus.Rejected;
            entity.Remark = remark;
            entity.ModifiedBy = User.Identity.Name;
            entity.LastModified = DateTime.Now;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            //UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer distribution", "Success", User.Identity.Name, $"rejected distribution {entity.RequestNumber}", Modules.FleetManagement);
            return Json(new { Message = "Distribution Rejected", Status = "OK" });
        }
        // Fleet stage step 1: acknowledge the distribution-approved transfer
        [HttpPost]
        public JsonResult AcceptTransfer(Guid id)
        {
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            if (entity == null || !entity.IsDistributionApproved || entity.IsAccepted)
                return Json(new { Message = "Only distribution approved transfers can be accepted", Status = "Error" });

            entity.IsAccepted = true;
            entity.AcceptedBy = User.Identity.Name;
            entity.AcceptedTime = DateTime.Now;
            entity.Status = SitesVehicleTransferStatus.Accepted;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer distribution", "Success", User.Identity.Name, $"accepted transfer {entity.RequestNumber}", Modules.FleetManagement);
            return Json(new { Message = "Accepted Successfully", Status = "OK" });
        }

        // Fleet stage step 2: only available once accepted
        [HttpPost]
        public JsonResult SendForApproval(Guid id)
        {
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            if (entity == null || !entity.IsAccepted || entity.IsSendForApproval)
                return Json(new { Message = "Transfer must be accepted before sending for approval", Status = "Error" });

            entity.IsSendForApproval = true;
            entity.SendForApprovalBy = User.Identity.Name;
            entity.SendForApprovalTime = DateTime.Now;
            entity.Status = SitesVehicleTransferStatus.WaitingApproval;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer distribution", "Success", User.Identity.Name, $"sent transfer {entity.RequestNumber} for approval", Modules.FleetManagement);
            return Json(new { Message = "Sent For Approval", Status = "OK" });
        }

        // Fleet stage step 3: final approval moves the vehicle to the destination site
        [HttpPost]
        public JsonResult ApproveTransfer(Guid id)
        {
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            if (entity == null || !entity.IsSendForApproval || entity.IsOnlineApproved)
                return Json(new { Message = "Transfer must be sent for approval before approving", Status = "Error" });

            try
            {
                var fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetsID).FirstOrDefault();

                var AssetRegistrations = db.AssetRegistrations.Where(x => x.AssetRegistrationId == fleet.AssetId).FirstOrDefault();

                entity.IsOnlineApproved = true;
                entity.OnlineApprovedBy = User.Identity.Name;
                entity.Status = SitesVehicleTransferStatus.Approved;
                entity.OnlineApprovedTime = DateTime.Now;

                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                // Change Branch on Fleet table
                //ChangeSiteTransferOnFleet(entity.FleetsID, entity.BranchId);

                var Location_Track = db.LocationTrackings.Where(x => x.IsCurrent).FirstOrDefault();
                var Branch = db.Branch.Where(x => x.Name == entity.DestinationSite).FirstOrDefault();
                if (Location_Track != null && Branch != null && AssetRegistrations != null)
                {
                    Location_Track.EndDateDate = DateTime.Now;
                    db.LocationTrackings.Attach(Location_Track);
                    db.Entry(Location_Track).State = EntityState.Modified;
                    db.SaveChanges();


                    var CurrentLocation = new LocationTracking()
                    {
                        AssetRegistrationId = AssetRegistrations.AssetRegistrationId,
                        StartDate = DateTime.Now,
                        IsCurrent = true,
                        BranchId = Branch.BranchId,
                        LocationType = LocationType.NotSet,


                    };
                    db.LocationTrackings.Add(CurrentLocation);
                    db.SaveChanges();
                }


                var approval = new SiteVehicleApproval()
                {

                    Status = "Approved",
                    SitesVehicleTransferId = id,
                    Remark = "Approved",
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.SiteVehicleApproval.Add(approval);
                db.SaveChanges();


                var Location = new EquipmentLocation()
                {
                    EquipmentLocationId = Guid.NewGuid(),
                    Condition = fleet.Condition,
                    FleetId = fleet.FleetsID,
                    Odometer = fleet.CurentOdoMeter,
                    Name = entity.DestinationSite,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now

                };
                db.EquipmentLocations.Add(Location);
                db.SaveChanges();



                fleet.Location = db.Branch.Find(entity.BranchId)?.Name ?? "";
                fleet.BranchId = entity.BranchId;
                //db.Fleets.Attach(fleet);
                db.Entry(fleet).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                return Json(new { Message = "Error occurred while approving transfer", Status = "Error" });
            }
            UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer distribution", "Success", User.Identity.Name, $"approved transfer {entity.RequestNumber}", Modules.FleetManagement);
            return Json(new { Message = "Approved Successfully", Status = "OK" });
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
