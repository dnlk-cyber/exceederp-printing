using ExceedERP.ApplicationServices.FleetManagement.FleetResource;
using Kendo.Mvc.UI;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Helpers.Fleet;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using ExceedERP.Core.Domain.Finance.GL.Dynamic;
using ExceedERP.Core.Domain.Finance.AP;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.Inventory;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceRequestApprover)]
    public class EquipmentDistributionAdditionalController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        IVehicleDistributionService _VehicleDistributionService;
        ExceedDbContext db = new ExceedDbContext();

       
        //
        // GET: /FleetManagement/VehicleDistribution/
        public ActionResult Index()
        {
            
            return View();
        }
        public ActionResult VehicleDistribution_Read([DataSourceRequest] DataSourceRequest request,string id)
        {
            //FleetMaintenaceHelper FleetMaintenaceHelper = new FleetMaintenaceHelper();
            //var VehicleDistribution = _VehicleDistributionService.GetAll().Where(x=>x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(v => new  
            //    {
            string userBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            IQueryable<VehicleDistribution> vehicleDistributions = db.VehicleDistribution
              .Where(x => x.OrgBranchName == userBranch&&x.ParentId.ToString()==id);
            DataSourceResult result = vehicleDistributions.ToDataSourceResult(request, v => new
            {

                RequestID = v.RequestID,
                AssignedQuantity = v.AssignedQuantity,
                EmployeeId = v.EmployeeId,
                ParentId=v.ParentId,
                RequestDate = v.RequestDate,
                ReasonForTravel = v.ReasonForTravel,
                Type = v.Type,
                DistrubtionNumber = v.DistrubtionNumber,
                StartingDate = v.StartingDate,
                ToDate = v.ToDate,
                RequestNumber = v.RequestNumber,
                Status = v.Status,
                IsVoid = v.IsVoid,
                IsExternal = v.IsExternal,
                VehicleDistributionID = v.VehicleDistributionID,
                FromBranchName = v.FromBranchName,
                FromStructureName = v.FromStructureName,
                EquipmentStatus = v.EquipmentStatus,
                Reference = v.Reference,
                IsOnlineApproved = v.IsOnlineApproved,
                IsSendForApproval = v.IsSendForApproval,
                CreatedBy = v.CreatedBy,
                DateCreated = v.DateCreated,
                LastModified = v.LastModified,
                ModifiedBy = v.ModifiedBy,

                Remark = v.Remark //FleetMaintenaceHelper.GetFleetDistributionDuration(v)


            }
                );

            return Json(result);
        }
       
       
       
        [AuthorizeRoles(FleetManagementRoles.ResourceRequestApprover)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDistribution_Create([DataSourceRequest] DataSourceRequest request, VehicleDistribution VehicleDistribution,string id)
        {
            IRentRequestService _rentRequestService = new RentRequestService();
            RentRequest rentRequest;
            if (VehicleDistribution.StartingDate > VehicleDistribution.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("DateIncorrect", "Start Date is Above End Date");
            }
            else
            {

                rentRequest = _rentRequestService.GetAll()
                     .Where(x => x.RentRequestID == VehicleDistribution.RequestID)
                     .FirstOrDefault();
                int RequestNumber = 1;
                //int equipmentDistribution = _VehicleDistributionService.GetAll().Count;//OrderByDescending(x => x.RequestNumber).FirstOrDefault();
                int equipmentDistribution = db.VehicleDistribution.ToList().Count;//OrderByDescending(x => x.RequestNumber).FirstOrDefault();

                RequestNumber = int.Parse(equipmentDistribution.ToString()) + 1;

                if (rentRequest != null)
                {
                    VehicleDistribution.VehicleDistributionID = Guid.NewGuid();
                    VehicleDistribution.ParentId = id;
                    VehicleDistribution.DistrubtionNumber = rentRequest.RequestNumber;
                    VehicleDistribution.StartingDate = rentRequest.StartingDate;
                    VehicleDistribution.ToDate = rentRequest.ToDate;
                    VehicleDistribution.FromStructureName = rentRequest.FromStructureName;
                    VehicleDistribution.FromBranchName = rentRequest.FromBranchName;
                    VehicleDistribution.RequestNumber = RequestNumber;
                    VehicleDistribution.Type = ExceedERP.Core.Domain.FleetManagement.FleetResource.VehicleDistribution.RequestType.Rent;
                    VehicleDistribution.Reference = rentRequest.RequestNumber.ToString();
                    VehicleDistribution.Status = MaintenanceConst.Editing;
                    VehicleDistribution.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                    VehicleDistribution.EquipmentStatus = MaintenanceConst.Pending;


                    if (VehicleDistribution != null)
                    {
                        VehicleDistribution.CreatedBy = User.Identity.Name;
                        VehicleDistribution.DateCreated = DateTime.Now;

                        // _VehicleDistributionService.Insert(VehicleDistribution);
                        
                        try
                        {
                            db.VehicleDistribution.Add(VehicleDistribution);
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
                        rentRequest.IsOnlineApproved = true;
                        rentRequest.OnlineApprovedBy = User.Identity.Name;
                        rentRequest.OnlineApprovedTime = DateTime.Now;
                        rentRequest.Status = MaintenanceConst.Forwarded;
                        _rentRequestService.Update(rentRequest);
                    }
                }

                else if (rentRequest == null)
                {

                    IVehicleDemandService _VehicleDemandService = new VehicleDemandService();
                    VehicleDemand vehicleDemand =db.VehicleDemand.Where(x => x.VehicleDemandId == VehicleDistribution.RequestID)
                                                    .FirstOrDefault();


                    if (vehicleDemand != null)
                    {

                        VehicleDistribution.VehicleDistributionID = Guid.NewGuid();
                        VehicleDistribution.DistrubtionNumber = vehicleDemand.RequestNumber;
                        VehicleDistribution.ReasonForTravel = vehicleDemand.ReasonForTravel;
                        VehicleDistribution.EmployeeId = vehicleDemand.EmployeeId;
                        VehicleDistribution.RequestDate = vehicleDemand.RequestDate;
                        VehicleDistribution.IsAssigned = vehicleDemand.IsAssigned;
                        VehicleDistribution.StartingDate = vehicleDemand.StartingDate;
                        VehicleDistribution.ToDate = vehicleDemand.ToDate;
                        VehicleDistribution.FromStructureName = vehicleDemand.FromStructureName;
                        VehicleDistribution.FromBranchName = vehicleDemand.FromBranchName;
                        VehicleDistribution.RequestNumber = RequestNumber;
                        VehicleDistribution.ParentId = id;
                        VehicleDistribution.Reference = vehicleDemand.RequestNumber.ToString();
                        VehicleDistribution.Type = ExceedERP.Core.Domain.FleetManagement.FleetResource.VehicleDistribution.RequestType.Company;
                        VehicleDistribution.Status = MaintenanceConst.Editing;
                        VehicleDistribution.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                        VehicleDistribution.EquipmentStatus = MaintenanceConst.Pending;



                        if (VehicleDistribution != null)
                        {
                            VehicleDistribution.CreatedBy = User.Identity.Name;
                            VehicleDistribution.DateCreated = DateTime.Now;
                            //    _VehicleDistributionService.Insert(VehicleDistribution);
                            try
                            {
                                db.VehicleDistribution.Add(VehicleDistribution);
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
                            vehicleDemand.IsOnlineApproved = true;
                            vehicleDemand.OnlineApprovedBy = User.Identity.Name;
                            vehicleDemand.OnlineApprovedTime = DateTime.Now;

                            vehicleDemand.Status = MaintenanceConst.Forwarded;
                            _VehicleDemandService.Update(vehicleDemand);
                        }
                    }

                }
            }
            return Json(new[] { VehicleDistribution }.ToDataSourceResult(request));
        }

        [AuthorizeRoles(FleetManagementRoles.ResourceRequestApprover)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDistribution_Update([DataSourceRequest] DataSourceRequest request,
      VehicleDistribution VehicleDistribution)
        {
            if (VehicleDistribution.StartingDate > VehicleDistribution.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("DateIncorrect", "Start Date is Above End Date");
            }
            else
            {

                if (VehicleDistribution != null)
                {
                    VehicleDistribution.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);

                    _VehicleDistributionService.Update(VehicleDistribution);
                }
            }

            return Json(new[] { VehicleDistribution }.ToDataSourceResult(request));
        }
        [AuthorizeRoles(FleetManagementRoles.ResourceRequestApprover)]

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDistribution_Destroy([DataSourceRequest] DataSourceRequest request, VehicleDistribution vehicleDistribution)
        {
            var VehicleDistribution = _VehicleDistributionService.GetById(vehicleDistribution.VehicleDistributionID);
            if (VehicleDistribution != null)
            {
                VehicleDistribution.Status = MaintenanceConst.Approved;
                VehicleDistribution.ModifiedBy = User.Identity.Name;
                VehicleDistribution.EquipmentStatus = MaintenanceConst.Finished;
                VehicleDistribution.IsOnlineApproved = true;
                VehicleDistribution.OnlineApprovedBy = User.Identity.Name;
                VehicleDistribution.OnlineApprovedTime = DateTime.Now;

                _VehicleDistributionService.Update(VehicleDistribution);

                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Equipment Distribution number " + vehicleDistribution.VehicleDistributionID);

            }

            return Json(new[] { VehicleDistribution }.ToDataSourceResult(request));
        }


        //Ajax method
       

       
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
