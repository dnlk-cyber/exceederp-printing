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
    public class EquipmentDistributionController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
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
                var businessUnits = CacheData.GetBusinessUnitCaches().Select(
                 i => new
                 {
                     Name = i.Name,
                     Id = i.Id
                 }).AsQueryable();
                ViewData["BusinessUnits"] = businessUnits;
            }
            ViewData["Fleets"] = db.Fleets.Select(s => new {
                Id = s.FleetsID,
                s.OrgBranchName,
                Name = s.PlateNumber + "-" + s.ChasisNumber + "-" + s.SerialNo
            });

            ViewData["FleetsAssignedEquip"] = db.Fleets.Select(s => new
            {
                Id = s.FleetsID,
                Name = s.PlateNumber
            });

           
            ViewData["EquipmentCategories"] = CacheData.GetEquipmentIdentityCaches().Select(
     i => new
     {
         Name = i.Name.Trim(),
         Id = i.Id.ToString()
     }).AsQueryable();
            ViewData["EquipmentTypes"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Name = i.Name,
            Id = i.Id
        }).AsQueryable();
            ViewData["EquipmentNames"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Name = i.Name,
            Id = i.Id
        }).AsQueryable();
            ViewData["Employee"] = CacheData.GetEmployeeCaches().Select(s => new {
                Id = s.Id,
                Name = s.Name
            });
            ViewData["Operators"] = db.Operator.Select(s => new {
                Id = s.OperatorID,
                Name = s.EmployeeId + " " + s.FullName
            });

        //    ViewData["EquipmentName"] = CacheData.GetEquipmentNameCaches().Select(
        //i => new
        //{
        //    Name = i.Name,
        //    Id = i.Id
        //}).AsQueryable();

            PopulateViewData();
            PopulateViewData2();
            return View();
        }
        private void PopulateViewData2()
        {

            var categories = db.ItemCategorys;
            var items = db.InventoryItems;
            var costCenter = CacheData.GetWorkUnitCaches().AsQueryable();

            var branches = CacheData.GetBranchCaches().Select(
                             i => new
                             {
                                 Name = i.Name,
                                 BranchId = i.Id
                             }).AsQueryable();
            ViewData["Branches"] = branches;

            var inventoryPeriods = db.InventoryPeriods;
            var selectedinventoryPeriods = inventoryPeriods.Select(
             i => new
             {
                 Name = i.Name,
                 GLPeriodId = i.InventoryPeriodId
             }).AsQueryable();
            ViewData["inventoryPeriods"] = selectedinventoryPeriods;


            var selecteditems = items.Select(
                i => new
                {
                    Name = i.ItemCode + "_" + i.Name,
                    ItemCode = i.ItemCode
                });

            ViewData["Items"] = selecteditems;
            ViewData["Category"] = categories;
            ViewData["defaultCategory"] = categories.FirstOrDefault();

            var store = db.InventoryStores.AsQueryable();
            ViewData["storesInCompany"] = store;



            //var employee = db.Person_Employee;
            //var selectedEmployee = employee.Select(
            //    i => new
            //    {
            //        Name = i.FirstNameEnglish + " " + i.MiddleNameEnglish + " " + i.LastNameEnglish,
            //        Code = i.EmployeeId
            //    }).AsQueryable();
            //ViewData["personEmployees"] = selectedEmployee;

            //var selectedcostCenter = 
            ViewData["costCenters"] = CacheData.GetWorkUnitCaches().Select(
                i => new
                {
                    Name = i.Name,
                    Code = i.Id
                }).AsQueryable();

        }
        private void PopulateViewData()
        {
            var selectedEmployee = CacheData.GetEmployeeCaches().Select(
                i => new
                {
                    Name = i.Name,
                    Code = i.Id
                }).AsQueryable();
            ViewData["personEmployees"] = selectedEmployee;
            // var branches = db.Branch;
            var structure = db.MainCompanyStructures;

            //var subAccountSeg = db.GlSegmentSetups.FirstOrDefault(x => x.SegmentType == DynamicSegmentTypes.SubAccount);
            //var subAccounts = db.GlChartOfAccountSegments.Where(x => x.GLSegmentSetupId == subAccountSeg.GLSegmentSetupId);
            ////var chartOfAccountSubAccounts = subAccounts;
            //var subaccounts = chartOfAccountSubAccounts.Select(
            //    i => new
            //    {
            //        Name = i.Description,
            //        Code = i.Values
            //    }).AsQueryable();


            //ViewData["SubAccounts"] = subaccounts;

            ViewData["Structure"] = structure;

            // ViewData["Branches"] = branches;
            var items = db.ApSetupItems.Select(s => new
            {
                s.ApSetupItemId,
                s.Name
            });
            ViewData["ApSetUpItems"] = items;
        }
        public ActionResult VehicleDistribution_Read([DataSourceRequest] DataSourceRequest request)
        {
            //FleetMaintenaceHelper FleetMaintenaceHelper = new FleetMaintenaceHelper();
            //var VehicleDistribution = _VehicleDistributionService.GetAll().Where(x=>x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(v => new  
            //    {
            string userBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
            IQueryable<VehicleDistribution> vehicleDistributions = db.VehicleDistribution
              .Where(x => x.OrgBranchName == userBranch&&x.ParentId==null);
            DataSourceResult result = vehicleDistributions.ToDataSourceResult(request, v => new
            {

                RequestID = v.RequestID,
                AssignedQuantity = v.AssignedQuantity,
                EmployeeId = v.EmployeeId,
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
        //    public ActionResult VehicleDistributionHistory_Read([DataSourceRequest] DataSourceRequest request,string id)
        //{
        //    //FleetMaintenaceHelper FleetMaintenaceHelper = new FleetMaintenaceHelper();
        //    //var VehicleDistribution = _VehicleDistributionService.GetAll().Where(x=>x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Select(v => new  
        //    //    {
        //    var FetchFleet = db.Fleets.FirstOrDefault(x => x.FleetsID.ToString() == id);

        //   string userBranch = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
        //    IQueryable<VehicleDistribution> vehicleDistributions = db.VehicleDistribution
        //      .Where(x => x.OrgBranchName == userBranch&&x.PlateNumber==FetchFleet.PlateNumber);
        //    DataSourceResult result = vehicleDistributions.ToDataSourceResult(request, v => new VehicleDistributionVM
        //    {

        //        RequestID = v.RequestID,
        //        AssignedQuantity = v.AssignedQuantity,
        //        EmployeeId = v.EmployeeId,
        //        RequestDate = v.RequestDate,
        //        ReasonForTravel = v.ReasonForTravel,
        //        DistrubtionNumber = v.DistrubtionNumber,
        //        StartingDate = v.StartingDate,
        //        ToDate = v.ToDate,
        //        RequestNumber = v.RequestNumber,
        //        Status = v.Status,
        //        VehicleDistributionID = v.VehicleDistributionID,
        //        FromBranchName = v.FromBranchName,
        //        FromStructureName = v.FromStructureName,
        //        EquipmentStatus = v.EquipmentStatus,
        //        Reference = v.Reference,
        //        OperatorName = GetOperatorById(
        //                    db.AssignedEquipment
        //                      .Where(x => x.VehicleDemandId.ToString() == v.RequestID.ToString())
        //                      .Select(x => x.OperatorId.ToString())
        //                      .FirstOrDefault()
        //                )


        //    }
        //        );

        //    return Json(result);
        //}
        private string GetOperatorById(string operatorSelector)
        {
            if (string.IsNullOrEmpty(operatorSelector)) return string.Empty;

            var fetchOperatorName = db.Operator
                .FirstOrDefault(x => x.OperatorID.ToString() == operatorSelector)?.FullName;

            return fetchOperatorName ?? string.Empty;

        }
        public ActionResult VehicleReturn_Read([DataSourceRequest] DataSourceRequest request)
        {
            var VehicleDistribution = db.VehicleDistribution.ToList().Where(x => x.OrgBranchName == UserInformationHelper.GetUserDefaultBranch(User.Identity.Name)).Where(x => x.IsOnlineApproved == true).Select(v => new VehicleDistribution
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

        [AuthorizeRoles(FleetManagementRoles.ResourceRequestApprover)]

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
                //int equipmentDistribution = _VehicleDistributionService.GetAll().Count;//OrderByDescending(x => x.RequestNumber).FirstOrDefault();
                int equipmentDistribution = db.VehicleDistribution.ToList().Count;//OrderByDescending(x => x.RequestNumber).FirstOrDefault();

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
        public JsonResult ToggleIsExternal(string id, bool newIsExternal)
        {
            var record = db.VehicleDistribution.FirstOrDefault(x => x.VehicleDistributionID.ToString() == id);
            if (record != null)
            {
                record.IsExternal = newIsExternal;
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();
                return Json(new { success = true });
            }

            return Json(new { success = false, message = "Record not found." });
        }

        public JsonResult Void(string id)
        {
            Guid Id = Guid.Parse(id);
            var record = db.VehicleDistribution.Find(Id);
            if (record != null && record.IsOnlineApproved && !record.IsVoid)
            {
                record.Status = Enum.GetName(typeof(Core.Domain.FleetManagement.FleetOperationStatuses), 10);
                record.IsVoid = true;
                record.IsSendForApproval = false;
                record.IsOnlineApproved = false;
                record.VoidBy = User.Identity.Name;
                record.VoidTime = DateTime.Now;
                db.VehicleDistribution.Attach(record);
                db.Entry(record).State = EntityState.Modified;
                db.SaveChanges();



                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Vehicle Distribution Voided", "Success", User.Identity.Name, $"void Vehicle Distribution {id} ", Modules.Inventory);
                #endregion

                var formattedData = new
                {
                    Message = "Void Successful.",
                    Status = "OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            if (record == null)
            {
                var formattedData = new
                {
                    Message = "The record is not found.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (record.IsVoid)
            {
                var formattedData = new
                {
                    Message = "The record is already void.You can not void again.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }

            if (!record.IsOnlineApproved)
            {
                var formattedData = new
                {
                    Message = "The record is not approved hence you can modify by editing.",
                    Status = "NOT_OK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }


            return null;
        }
        //public JsonResult Void(int id)
        //{
        //    var record = db.VehicleDistribution.Find(id);
        //    if (record != null && record.IsOnlineApproved && !record.IsVoid)
        //    {


        //            record.IsSendForApproval = false;
        //            record.IsOnlineApproved = false;

        //          //  record.OnlineApprovedBy = User.Identity.Name;
        //            record.Status = MaintenanceConst.Rejected;
        //        record.IsVoid = true;
        //        record.VoidBy = User.Identity.Name; ;
        //        record.IsVoid = true;
        //            //db.VehicleDistribution.Attach(record);
        //            db.Entry(record).State = EntityState.Modified;
        //            db.SaveChanges();
        //            var returnMessage = new
        //            {
        //                Message = "Successfully Sent!",

        //            };
        //            return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);

        //        }

        //        var jsonData = new
        //        {
        //            Message = "Error Sending!",

        //        };
        //        return Json(jsonData.Message, JsonRequestBehavior.AllowGet);

        //    }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
