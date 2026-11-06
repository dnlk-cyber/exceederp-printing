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
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System.Data.Entity;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    // Stage 3 of the site vehicle transfer workflow: fleet reviews approved distributions and
    // accepts/approves them (SiteVehicleApproval tab or AcceptTransfer for MCG), which moves the vehicle.
    [AuthorizeRoles(FleetManagementRoles.RentApprover, FleetManagementRoles.FleetAdmin, FleetManagementRoles.ResourceApprover)]
    public class SitesVehicleTransferFleetController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();

        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers;
            var fetchSettingg = db.RequestToInspections.FirstOrDefault();
            ViewBag.Setting = fetchSettingg?.Remark ?? "";
            ViewData["Employee"] = CacheData.GetEmployeeCaches().Select(i => new { Name = i.Name, Code = i.Id }).AsQueryable();
            ViewData["Branch"] = CacheData.GetBranchCaches().Select(i => new { Name = i.Name, BranchId = i.Id }).AsQueryable();
            ViewData["EquipmentCategory"] = CacheData.GetEquipmentIdentityCaches().Select(i => new { Text = i.Name, Value = i.Id }).AsQueryable();
            ViewData["EquipmentType"] = CacheData.GetEquipmentTypeCaches().Select(i => new { Text = i.Name, Value = i.Id }).AsQueryable();
            ViewData["EquipmentName"] = CacheData.GetEquipmentNameCaches().Select(i => new { Text = i.Name, Value = i.Id }).AsQueryable();
            ViewData["Operator"] = db.Operator.Select(s => new { Value = s.OperatorID, Text = s.EmployeeCode + " " + s.FullName }).ToList();
            return View();
        }

        public ActionResult Fleet_Read([DataSourceRequest] DataSourceRequest request)
        {
            IQueryable<SitesVehicleTransfer> SVT = db.SitesVehicleTransfer.Where(x => x.IsDistributionApproved);
            DataSourceResult Result = SVT.ToDataSourceResult(request, s => new
            {
                SitesVehicleTransferID = s.SitesVehicleTransferID,
                RequestNumber = s.RequestNumber,
                OrgBranchName = s.OrgBranchName,
                EquipmentIdentityID = s.EquipmentIdentityID,
                EquipmentTypeID = s.EquipmentTypeID,
                EquipmentNameID = s.EquipmentNameID,
                FleetsID = s.FleetsID,
                PlateNo = s.Fleet.PlateNumber,
                InitialSite = s.InitialSite,
                BranchId = s.BranchId,
                DestinationSite = s.DestinationSite,
                OperatorId = s.OperatorId,
                SecondOperatorId = s.SecondOperatorId,
                StartingDate = s.StartingDate,
                ToDate = s.ToDate,
                TransferReason = s.TransferReason,
                Status = s.Status,
                RequestedBy = s.RequestedBy,
                RequestApprovedBy = s.RequestApprovedBy,
                RequestReceivedBy = s.RequestReceivedBy,
                DistributionApprovedBy = s.DistributionApprovedBy,
                DistributionApprovedDate = s.DistributionApprovedDate,
                IsSendForApproval = s.IsSendForApproval,
                IsOnlineApproved = s.IsOnlineApproved,
                OnlineApprovedBy = s.OnlineApprovedBy,
                OnlineApprovedTime = s.OnlineApprovedTime,
                DateCreated = s.DateCreated
            });
            return Json(Result, JsonRequestBehavior.AllowGet);
        }

        // MCG-mode acceptance: equivalent of a fleet approval without the approval grid
        [HttpPost]
        public JsonResult AcceptTransfer(Guid id)
        {
            var entity = db.SitesVehicleTransfer.FirstOrDefault(x => x.SitesVehicleTransferID == id);
            var fleet = entity?.FleetsID != null ? db.Fleets.FirstOrDefault(x => x.FleetsID == entity.FleetsID) : null;
            if (entity == null || fleet == null || !entity.IsDistributionApproved)
                return Json(new { Message = "Transfer is not ready for fleet acceptance", Status = "Error" });
            if (entity.IsOnlineApproved)
                return Json(new { Message = "Transfer already accepted", Status = "Error" });

            entity.IsOnlineApproved = true;
            entity.OnlineApprovedBy = User.Identity.Name;
            entity.OnlineApprovedTime = DateTime.Now;
            entity.Status = SitesVehicleTransferStatus.Approved;
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();

            db.SiteVehicleApproval.Add(new SiteVehicleApproval
            {
                Status = "Approved",
                SitesVehicleTransferId = id,
                Remark = "Approved",
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name
            });
            db.SaveChanges();

            var assetRegistration = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == fleet.AssetId);
            var branch = db.Branch.FirstOrDefault(x => x.BranchId == entity.BranchId);
            if (assetRegistration != null && branch != null)
            {
                var currentTrack = db.LocationTrackings.FirstOrDefault(x => x.IsCurrent && x.AssetRegistrationId == assetRegistration.AssetRegistrationId);
                if (currentTrack != null)
                {
                    currentTrack.EndDateDate = DateTime.Now;
                    currentTrack.IsCurrent = false;
                    db.Entry(currentTrack).State = EntityState.Modified;
                }
                db.LocationTrackings.Add(new LocationTracking
                {
                    AssetRegistrationId = assetRegistration.AssetRegistrationId,
                    StartDate = DateTime.Now,
                    IsCurrent = true,
                    BranchId = branch.BranchId,
                    LocationType = LocationType.NotSet
                });
                assetRegistration.Location = branch.Name;
                assetRegistration.BranchId = branch.BranchId;
                db.Entry(assetRegistration).State = EntityState.Modified;
                db.SaveChanges();
            }

            db.EquipmentLocations.Add(new EquipmentLocation
            {
                EquipmentLocationId = Guid.NewGuid(),
                Condition = fleet.Condition,
                FleetId = fleet.FleetsID,
                Odometer = fleet.CurentOdoMeter,
                Name = entity.DestinationSite,
                CreatedBy = User.Identity.Name,
                DateCreated = DateTime.Now
            });
            if (branch != null)
            {
                fleet.Location = branch.Name;
                fleet.BranchId = branch.BranchId;
                db.Entry(fleet).State = EntityState.Modified;
            }
            db.SaveChanges();

            return Json(new { Message = "Accepted Successfully", Status = "OK" });
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
