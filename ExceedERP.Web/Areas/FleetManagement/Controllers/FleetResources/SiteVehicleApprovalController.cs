
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FixedAsset.Registrations;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [Authorize]
    public class SiteVehicleApprovalController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();


        
        public ActionResult SiteVehicleApproval_Read([DataSourceRequest]DataSourceRequest request, string EId)
        {

            Guid Id = Guid.Parse(EId);

            List<SiteVehicleApproval> fleetApproval = db.SiteVehicleApproval.Where(x => x.SitesVehicleTransferId == Id).ToList();
            List<SitesVehicleTransfer> Category = db.SitesVehicleTransfer.Where(x => x.SitesVehicleTransferID == Id).ToList();
            
            if ( Category.FirstOrDefault().IsSendForApproval && Category.FirstOrDefault().IsOnlineApproved == false)
            {
                fleetApproval.Add(new SiteVehicleApproval()
                {
                    SitesVehicleTransferId = Id,
                    Remark = string.Empty,
                    Status = MaintenanceConst.Pending,

                });
            }


            return Json(fleetApproval.ToDataSourceResult(request, approval => new SiteVehicleApproval()
            {
                SiteVehicleApprovalID = approval.SiteVehicleApprovalID,

                Remark = approval.Remark,
                Status = approval.Status,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name


            }));
        }



      
       
        public ActionResult SiteVehicle_SendForOnlineApproval(string EId)
        {

            Guid Id = Guid.Parse(EId);
            var SitesVehicleTransfer = db.SitesVehicleTransfer.Where(x => x.SitesVehicleTransferID == Id).FirstOrDefault();
            if (SitesVehicleTransfer != null)
            {
                SitesVehicleTransfer.IsSendForApproval = true;
                SitesVehicleTransfer.Status = MaintenanceConst.WaitingApproval;
                db.SitesVehicleTransfer.Attach(SitesVehicleTransfer);
                db.Entry(SitesVehicleTransfer).State = EntityState.Modified;
                db.SaveChanges();
            }


            return RedirectToActionPermanent("Index", "SitesVehicleTransfer", new { area = "FleetManagement" });
        }
        private void ChangeSiteTransferOnFleet(Guid fleetId, int branchId)
        {
            var fleet = db.Fleets.Find(fleetId);
            var FixedAsset = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == fleet.AssetId);
            var branch = db.Branch.FirstOrDefault(x => x.BranchId == branchId);
            if (fleet != null && branch != null)
            {
                fleet.Location = branch.Name;
                fleet.BranchId = branch.BranchId;
                db.Entry(fleet).State = EntityState.Modified;
                db.SaveChanges();
            }
            if (FixedAsset != null && branch != null)
            {
                FixedAsset.Location = branch.Name;
                FixedAsset.BranchId = branch.BranchId;
                db.Entry(FixedAsset).State = EntityState.Modified;
                db.SaveChanges();
            }
        }
        [AuthorizeRoles(FleetManagementRoles.RentApprover)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SiteVehicle_Approve([DataSourceRequest]DataSourceRequest request, SiteVehicleApproval SiteVehicleApproval, string EId)
        {
            try
            {
                Guid Id = Guid.Parse(EId);
                var entity = db.SitesVehicleTransfer.Where(x => x.SitesVehicleTransferID == Id).FirstOrDefault();
                var fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetsID).FirstOrDefault();
               
                var AssetRegistrations = db.AssetRegistrations.Where(x => x.Tag == fleet.UnitNum).FirstOrDefault();
                if (SiteVehicleApproval.Status == MaintenanceConst.Approved)
                {
                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    entity.Status = MaintenanceConst.Approved;

                    db.SitesVehicleTransfer.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    // Change Branch on Fleet table
                    ChangeSiteTransferOnFleet(entity.FleetsID, entity.BranchId);

                    var Location_Track = db.LocationTrackings.Where(x=>x.IsCurrent).FirstOrDefault();
                    var Branch = db.Branch.Where(x=>x.Name == entity.DestinationSite).FirstOrDefault();
                    if(Location_Track != null && Branch != null && AssetRegistrations != null)
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
                }
                if (SiteVehicleApproval.Status == MaintenanceConst.Rejected)
                {
                    entity.IsSendForApproval = false;

                    db.SitesVehicleTransfer.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var approval = new SiteVehicleApproval()
                {

                    Status = SiteVehicleApproval.Status,
                    SitesVehicleTransferId = Id,
                    Remark = SiteVehicleApproval.Remark,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name

                };

                db.SiteVehicleApproval.Add(approval);
                db.SaveChanges();
                SiteVehicleApproval.SiteVehicleApprovalID = approval.SiteVehicleApprovalID;

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
                db.Fleets.Attach(fleet);
                db.Entry(fleet).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception e)
            {

            }
            return Json(new[] { SiteVehicleApproval }.ToDataSourceResult(request));
        }




    }
}