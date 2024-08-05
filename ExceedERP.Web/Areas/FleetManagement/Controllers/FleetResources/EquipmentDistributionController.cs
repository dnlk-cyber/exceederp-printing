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

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
      [AuthorizeRoles(FleetManagementRoles.ResourceRequestor, FleetManagementRoles.ResourceRequestApprover)] 
    public class EquipmentDistributionController : Controller
    {
        IVehicleDistributionService _VehicleDistributionService;
        ExceedDbContext db = new ExceedDbContext();

        public EquipmentDistributionController()
        {
            _VehicleDistributionService = new VehicleDistributionService();
        }
        //
        // GET: /FleetManagement/VehicleDistribution/
        public ActionResult Index()
        {
            if (GlobalHelper.IsMultipleBusinessUnit())
            {
                var businessUnits = db.BusinessUnits.ToList();
                ViewData["BusinessUnits"] = businessUnits;
            }
            ViewData["Fleets"] = db.Fleets.ToList().Select(s => new {
                Id = s.FleetsID,
                s.OrgBranchName,
                Name = s.PlateNumber + "-" +s.ChasisNumber + "-" + s.SerialNo
            });

            ViewData["FleetsAssignedEquip"] = db.Fleets.Select(s => new
            {
                Id = s.FleetsID,
                Name = s.PlateNumber
            });

            ViewData["EquipmentCategories"] = db.EquipmentIdentity.ToList().Select(s => new {
                Id = s.EquipmentIdentityID,
                Name = s.EquipmentCategoryName
            });
            ViewData["EquipmentTypes"] = db.EquipmentType.ToList().Select(s => new {
                Id = s.EquipmentTypeID,
                Name = s.EquipmentTypeName
            });
            ViewData["EquipmentNames"] = db.EquipmentName.ToList().Select(s => new {
                Id = s.EquipmentNameID,
                Name = s.Name
            });
            ViewData["Employee"] = db.Person_Employee.ToList().Select(s => new {
                Id = s.EmployeeId,
                Name = s.FirstNameEnglish + " " + s.MiddleNameEnglish + " " + s.LastNameEnglish
            });
            ViewData["Operators"] = db.Operator.ToList().Select(s => new {
                Id = s.OperatorID,
                Name = s.FullName
            });

            ViewData["EquipmentName"] = db.EquipmentName.Select(s => new
            {
                Id = s.EquipmentNameID,
                Name = s.Name
            });

            PopulateViewData();
            PopulateViewData2();
            return View();
        }
        private void PopulateViewData2()
        {

            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var costCenter = db.BranchCostCenter;
            var branches = db.Branch;


            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;

            ViewData["Branches"] = branches;
            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                }).ToList();

            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();

            var store = db.InventoryStores.AsQueryable();
            ViewData["storesInCompany"] = store;



            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;

            var selectedcostCenter = costCenter.Select(
               i => new
               {
                   Name = i.MainCompanyStructure.Name,
                   Code = i.OrgStructureId
               }).AsQueryable();

            ViewData["costCenters"] = selectedcostCenter;

        }
        private void PopulateViewData()
        {
            var employee = db.Person_Employee;
            var selectedEmployee = employee.Select(
                i => new
                {
                    Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
                    Code = i.EmployeeId
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
            var branches = db.Branch;
            var structure = db.MainCompanyStructures;

            var subAccountSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
            var subAccounts = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == subAccountSeg.GLSegmentSetupId);
            var chartOfAccountSubAccounts = subAccounts;
            var subaccounts = chartOfAccountSubAccounts.Select(
                i => new
                {
                    Name = i.Description,
                    Code = i.Values
                }).AsQueryable();


            ViewData["SubAccounts"] = subaccounts;

            ViewData["Structure"] = structure;

            ViewData["Branches"] = branches;
            var items = db.ApSetupItems.Select(s => new
            {
                s.ApSetupItemId,
                s.Name
            });
            ViewData["ApSetUpItems"] = items;
        }
        public ActionResult VehicleDistribution_Read([DataSourceRequest] DataSourceRequest request)
        {
            FleetMaintenaceHelper FleetMaintenaceHelper = new FleetMaintenaceHelper();
            var VehicleDistribution = _VehicleDistributionService.GetAll().Where(x=>x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(v => new  
                {

                    RequestID = v.RequestID,
                    AssignedQuantity = v.AssignedQuantity,
                    EmployeeId=v.EmployeeId,
                    RequestDate=v.RequestDate,
                    ReasonForTravel =v.ReasonForTravel,
                    Type = v.Type,
                    DistrubtionNumber = v.DistrubtionNumber,
                    StartingDate = v.StartingDate,
                    ToDate = v.ToDate,
                    RequestNumber = v.RequestNumber,
                    Status = v.Status,
                    VehicleDistributionID = v.VehicleDistributionID,
                    FromBranchName = v.FromBranchName,
                    FromStructureName = v.FromStructureName,
                    EquipmentStatus=v.EquipmentStatus,
                    Reference=v.Reference,
                    IsOnlineApproved =  v.IsOnlineApproved,
                    IsSendForApproval = v.IsSendForApproval,
                    CreatedBy = v.CreatedBy,
                    DateCreated = v.DateCreated,
                    LastModified = v.LastModified,
                    ModifiedBy= v.ModifiedBy,
                    Remark = FleetMaintenaceHelper.GetFleetDistributionDuration(v)
                   

                }
                );

            return Json(VehicleDistribution.ToDataSourceResult(request));
        }

        public ActionResult VehicleReturn_Read([DataSourceRequest] DataSourceRequest request)
        {
            var VehicleDistribution = _VehicleDistributionService.GetAll().ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Where(x => x.IsOnlineApproved == true).Select(v => new VehicleDistribution
            {

                RequestID = v.RequestID,
                AssignedQuantity = v.AssignedQuantity,
                Type = v.Type,
                DistrubtionNumber = v.DistrubtionNumber,
                StartingDate = v.StartingDate,
                ToDate = v.ToDate,
                RequestNumber = v.RequestNumber,
                Status = v.Status,
                VehicleDistributionID = v.VehicleDistributionID,
                FromBranchName = v.FromBranchName,
                FromStructureName = v.FromStructureName,
                EquipmentStatus = v.EquipmentStatus,
                Reference = v.Reference

            }
                );

            return Json(VehicleDistribution.ToDataSourceResult(request));
        }

        [AuthorizeRoles( FleetManagementRoles.ResourceRequestApprover)] 

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDistribution_Create([DataSourceRequest] DataSourceRequest request, VehicleDistribution VehicleDistribution)
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
                int equipmentDistribution = _VehicleDistributionService.GetAll().Count;//OrderByDescending(x => x.RequestNumber).FirstOrDefault();
        
                    RequestNumber = int.Parse(equipmentDistribution.ToString()) + 1;
                
                if (rentRequest != null)
                {
                    VehicleDistribution.VehicleDistributionID = Guid.NewGuid();

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
                        _VehicleDistributionService.Insert(VehicleDistribution);
                      
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
                    VehicleDemand vehicleDemand = _VehicleDemandService.GetAll()
                                                    .Where(x => x.VehicleDemandId == VehicleDistribution.RequestID)
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
                        VehicleDistribution.Reference = vehicleDemand.RequestNumber.ToString();
                        VehicleDistribution.Type = ExceedERP.Core.Domain.FleetManagement.FleetResource.VehicleDistribution.RequestType.Company;
                        VehicleDistribution.Status = MaintenanceConst.Editing;
                        VehicleDistribution.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
               
                        VehicleDistribution.EquipmentStatus = MaintenanceConst.Pending;
                     
                        
          
                        if (VehicleDistribution != null)
                        {
                            VehicleDistribution.CreatedBy = User.Identity.Name;
                            VehicleDistribution.DateCreated = DateTime.Now;
                            _VehicleDistributionService.Insert(VehicleDistribution);
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

              [AuthorizeRoles( FleetManagementRoles.ResourceRequestApprover)] 

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult VehicleDistribution_Update([DataSourceRequest] DataSourceRequest request,
            VehicleDistribution VehicleDistribution)
        {
            if (VehicleDistribution.StartingDate > VehicleDistribution.ToDate)
            {
                ModelState.Clear();
                ModelState.AddModelError("DateIncorrect", "Start Date is Above End Date");
            }            else
            {

                if (VehicleDistribution != null)
                {
                    VehicleDistribution.OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                
                    _VehicleDistributionService.Update(VehicleDistribution);
                }
            }

            return Json(new[] { VehicleDistribution }.ToDataSourceResult(request));
        }
              [AuthorizeRoles( FleetManagementRoles.ResourceRequestApprover)] 

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
        public JsonResult GetDriver(Guid fleetId)
        {
            
            var driverAssignment = db.DriverAssignments.FirstOrDefault(x => x.FleetId == fleetId);
            if (driverAssignment != null)
            {
                var driver = db.Operator.FirstOrDefault(x => x.FleetsID == driverAssignment.FleetId);
                if (driver == null) return Json(null);
                var result = new
                {
                    FullName = driver.FullName,
                };
                return Json(result, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
           
        }

        public JsonResult VehicleDistribution_SendForOnlineApproval(string vehicleDistributionID)
        {
            var message = new
            {
                ErrorMessage = "You need to assign equipment before sending it for approval.",
                SuccessMessage = "Successfully sent for approval",
                Error = "Error"
            };
            Guid Id = Guid.Parse(vehicleDistributionID);
            var vehicleDistrubution = db.VehicleDistribution.FirstOrDefault(x => x.VehicleDistributionID == Id);
            var requestEquipment = db.RequestedEquipments.FirstOrDefault(x => x.RequestNumber == vehicleDistrubution.RequestID);
            if (vehicleDistrubution != null)
            {
                var assignedEquipments = db.AssignedEquipment.Where(x => x.RequestedEquipmentId == requestEquipment.RequestedEquipmentId);
                if (!assignedEquipments.Any()) return Json(message.ErrorMessage, JsonRequestBehavior.AllowGet);
               
                vehicleDistrubution.IsSendForApproval = true;
                vehicleDistrubution.Status = MaintenanceConst.WaitingApproval;

                db.VehicleDistribution.Attach(vehicleDistrubution);
                db.Entry(vehicleDistrubution).State = EntityState.Modified;
                db.SaveChanges();
                return Json(message.SuccessMessage, JsonRequestBehavior.AllowGet);
               
            }
            return Json(message.Error);
            
        }
    }
}
