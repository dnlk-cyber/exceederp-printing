using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System; 
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Global;
using System.Data.Entity;
using ExceedERP.Core.Domain.FixedAsset.Registrations;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceRequestor)] 
    public class SitesVehicleTransferController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        ISitesVehicleTransferService _SitesVehicleTransferService;

        public SitesVehicleTransferController()
        {
            _SitesVehicleTransferService = new SitesVehicleTransferService();
        }
        //
        // GET: /FleetManagement/SitesVehicleTransfer/
        public ActionResult Index()
        {
            ViewData["Customer"] = db.OrganizationCustomers;
            var fetchSettingg = db.RequestToInspections.FirstOrDefault();
            ViewBag.Setting = fetchSettingg?.Remark ?? "";
            ViewData["Employee"] = CacheData.GetEmployeeCaches().Select(
                i => new
                {
                    Name = i.Name,
                    Code = i.Id
                }).AsQueryable();
            ViewData["VehiclePartDamageType"] = db.VehiclePartDamageType;
            ViewData["VehicleParts"] = db.VehiclePart;
            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(s => new {
                Id = s.Id,
                Name = s.Name
            }).AsQueryable();
            ViewData["Fleets"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber ?? "" + "/" + s.SerialNo ?? ""
            });
            ViewData["Branch"] = CacheData.GetBranchCaches().Select(
         i => new
         {
             Name = i.Name,
             BranchId = i.Id
         }).AsQueryable();
            ViewData["Operator"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.EmployeeCode+" "+s.FullName
            }).AsQueryable();
            ViewData["OperatorTwo"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.EmployeeCode + " " + s.FullName
            });

            ViewData["EquipmentCategory"] = CacheData.GetEquipmentIdentityCaches().Select(
    i => new
    {
        Text = i.Name,
        Value = i.Id
    }).AsQueryable();
            ViewData["EquipmentType"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["EquipmentName"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            return View();
        }


        public ActionResult SitesVehicleTransfer_Read([DataSourceRequest] DataSourceRequest request)
        {
            var defaultBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

            // Now use it in your EF query
            IQueryable<SitesVehicleTransfer> SVT = db.SitesVehicleTransfer
                .Where(x => x.OrgBranchName == defaultBranch);
            DataSourceResult Result= SVT.ToDataSourceResult(request,s => new
            {
                Description = s.Description,
                FleetsID = s.FleetsID,
                PlateNo = s.Fleet.PlateNumber,
                ToDate = s.ToDate,
                StartingDate = s.StartingDate,
                DestinationSite = s.DestinationSite,
                InitialSite = s.InitialSite,
                OperatorId = s.OperatorId,
                SecondOperatorId = s.SecondOperatorId,
                //EquipmentTypeId = s.Fleet.EquipmentIdentityID.ToString(),
                Remark = s.Fleet.PlateNumber + " -- " + s.Fleet.EquipmentName,
                SitesVehicleTransferID = s.SitesVehicleTransferID  ,
                IsOnlineApproved = s.IsOnlineApproved,
                IsSendForApproval = s.IsSendForApproval,
                CreatedBy = s.CreatedBy,
                DateCreated = s.DateCreated,
                ModifiedBy = s.ModifiedBy,
                LastModified = s.LastModified,
                Status = s.Status,
                BranchId = s.BranchId,
                EquipmentNameID = s.EquipmentNameID,
                EquipmentIdentityID = s.EquipmentIdentityID,
                EquipmentTypeID = s.EquipmentTypeID,
                
                   

            });

            return Json(Result,JsonRequestBehavior.AllowGet);
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SitesVehicleTransfer_Create([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer sitesVehicleTransfer)
        {
            ModelState.Remove("SitesVehicleTransferID");
            ModelState.Remove("InitialSite");
            ModelState.Remove("EquipmentIdentityID");
            ModelState.Remove("EquipmentTypeID");
            ModelState.Remove("EquipmentNameID");
           
            Fleets fleet = db.Fleets.Where(x => x.FleetsID == sitesVehicleTransfer.FleetsID).FirstOrDefault();

            if (sitesVehicleTransfer.StartingDate > sitesVehicleTransfer.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date");
            }
            else if (sitesVehicleTransfer.BranchId == fleet.BranchId)
            {
                ModelState.Clear();
                ModelState.AddModelError("SameSite", "Intial and Destination Site Must be Different");
            }
            else
            {
                if (ModelState.IsValid)
                {
                    if (sitesVehicleTransfer != null)
                    {
                        sitesVehicleTransfer.SitesVehicleTransferID = Guid.NewGuid();
                        sitesVehicleTransfer.EquipmentIdentityID = fleet.EquipmentIdentityID;
                        sitesVehicleTransfer.EquipmentTypeID = fleet.EquipmentTypeID;
                        sitesVehicleTransfer.EquipmentNameID = fleet.EquipmentNameID;
                        sitesVehicleTransfer.OperatorId = sitesVehicleTransfer.OperatorId;
                        sitesVehicleTransfer.SecondOperatorId = sitesVehicleTransfer.SecondOperatorId;
                        sitesVehicleTransfer.InitialSite = fleet.Location;
                        sitesVehicleTransfer.DestinationSite = sitesVehicleTransfer.DestinationSite;
                        sitesVehicleTransfer.CreatedBy = User.Identity.Name;
                        sitesVehicleTransfer.DateCreated = DateTime.Now;
                        sitesVehicleTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        bool IsInserted = _SitesVehicleTransferService.Insert(sitesVehicleTransfer);
                        //ChangeSiteTransferOnFleet(sitesVehicleTransfer.FleetsID, sitesVehicleTransfer.BranchId);

                        var EquipmentTyres = db.EquipmentTyres.Where(x => x.fleetId == sitesVehicleTransfer.FleetsID && x.Isattached).ToList();
                        fleet.Location = db.Branch.FirstOrDefault(x => x.BranchId == sitesVehicleTransfer.BranchId)?.Name ?? "";
                        var AssetRegistration = db.AssetRegistrations.FirstOrDefault(x => x.AssetRegistrationId == fleet.AssetId);
                        if (AssetRegistration != null && fleet != null)
                        {
                            AssetRegistration.Location = fleet.Location;
                            db.Entry(AssetRegistration).State = EntityState.Modified;
                            db.Entry(fleet).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                       

                        foreach (var EquipmentTyre in EquipmentTyres)
                        {

                            var TyreDetail = new SiteVechicleTransferTyreDetail()
                            {
                                SiteVechicleTransferId = sitesVehicleTransfer.SitesVehicleTransferID,
                                SerialNo = EquipmentTyre.SerialNo,
                                Type = EquipmentTyre.Type,
                                Description = EquipmentTyre.Make + " " + EquipmentTyre.Condition,
                                CreatedBy = User.Identity.Name,
                                DateCreated = DateTime.Now
                            };
                            db.SiteVechicleTransferTyreDetailes.Add(TyreDetail);
                            db.SaveChanges();
                            #region operation log
                            UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer registration", "Success", User.Identity.Name, $"created site vehicle transfer {TyreDetail.SiteVechicleTransferTyreDetailId}", Modules.FleetManagement);
                            #endregion


                        }
                    }
                }
            }
            return Json(new[] { sitesVehicleTransfer }.ToDataSourceResult(request, ModelState));
        }
       
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SitesVehicleTransfer_Update([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer SitesVehicleTransfer)
        {
            ModelState.Remove("Num");
            ModelState.Remove("FuelAmount");
            ModelState.Remove("BatteryStatus");
            ModelState.Remove("EquipmentCondition");
            ModelState.Remove("TransferMode");
            ModelState.Remove("Hydrolic");
            if (SitesVehicleTransfer.StartingDate > SitesVehicleTransfer.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Start Date is Above End Date");
            }
            
            else
            {
                if (ModelState.IsValid)
                {
                    if (SitesVehicleTransfer != null)
                    {
                        SitesVehicleTransfer.ModifiedBy = User.Identity.Name;
                        SitesVehicleTransfer.LastModified = DateTime.Now;
                        SitesVehicleTransfer.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                        _SitesVehicleTransferService.Update(SitesVehicleTransfer);
                        //ChangeSiteTransferOnFleet(SitesVehicleTransfer.FleetsID, SitesVehicleTransfer.BranchId);
                        #region operation log
                        UserHelper.OperationLog(Request.UserHostAddress, "Site vehicle transfer registration", "Success", User.Identity.Name, $"updated site vehicle transfer {SitesVehicleTransfer.SitesVehicleTransferID}", Modules.FleetManagement);
                        #endregion
                    }

                }
            }

            return Json(new[] { SitesVehicleTransfer }.ToDataSourceResult(request,ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SitesVehicleTransfer_Destroy([DataSourceRequest] DataSourceRequest request, SitesVehicleTransfer sitesVehicleTransfer)
        {
            var SitesVehicleTransfer = _SitesVehicleTransferService.GetById(sitesVehicleTransfer.SitesVehicleTransferID);
            if (SitesVehicleTransfer != null)
            {
                _SitesVehicleTransferService.Delete(SitesVehicleTransfer);
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, " Delete site equipment transfer number " + sitesVehicleTransfer.SitesVehicleTransferID);
           
            }

            return Json(new[] { SitesVehicleTransfer }.ToDataSourceResult(request));
        }
        public JsonResult AcceptTransfer(Guid id)
        {
            try
            {
                var entity = db.SitesVehicleTransfer.Where(x => x.SitesVehicleTransferID == id).FirstOrDefault();
                var fleet = db.Fleets.Where(x => x.FleetsID == entity.FleetsID).FirstOrDefault();

                var AssetRegistrations = db.AssetRegistrations.Where(x => x.AssetRegistrationId == fleet.AssetId).FirstOrDefault();
                
                    entity.IsOnlineApproved = true;
                    entity.OnlineApprovedBy = User.Identity.Name;
                    entity.OnlineApprovedTime = DateTime.Now;
                    //entity.Status = MaintenanceConst.Approved;

                    //db.SitesVehicleTransfer.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    // Change Branch on Fleet table
                    ChangeSiteTransferOnFleet(entity.FleetsID, entity.BranchId);

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
                db.Fleets.Attach(fleet);
                db.Entry(fleet).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception e)
            {

            }
            var formattedData = new
            {
                Message = "Accepted Successfully",
                Status = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}