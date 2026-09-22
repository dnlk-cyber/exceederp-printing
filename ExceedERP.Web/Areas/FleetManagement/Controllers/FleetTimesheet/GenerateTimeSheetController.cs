using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
//using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetTimesheet
{
    [AuthorizeRoles(FleetManagementRoles.FleetTimeSheetUser, FleetManagementRoles.FleetTimeSheetApprover)]
    public class GenerateTimeSheetController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        public ActionResult Index(TimeSheetType lightVehicle)
        {


            return View();
        }


        public JsonResult AutoGenerateTimeSheet(int id, DateTime date)
        {
            List<LightVehicleTimeSheet> timeSheet = new List<LightVehicleTimeSheet>();
            var timeSheets = db.LightVehicleTimeSheets
                    .Where(x => x.BranchId == id)
                    .GroupBy(x => x.FleetsID).ToList();
            foreach (var g in timeSheets)
            {                
                var latest = g.OrderByDescending(x => x.Date).FirstOrDefault();
                timeSheet.Add(latest);
            }
            var allNonFuelItems= db.NonFuelItemSettings.ToList();
            var allInventoryItems = db.InventoryItems.ToList();
            var OilLubCategories = allNonFuelItems.Any()?allNonFuelItems.Where(x => x.IsLubricant == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var OilLubItems = allInventoryItems.Any()? allInventoryItems.Where(x => OilLubCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allSparePartCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsSparePart == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allSparePartItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allSparePartCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allBatteryCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsBattery == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allBatteryItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allBatteryCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allTyreCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsTire == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allTyreItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allTyreCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var OilCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsOil == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var oilItems = allInventoryItems.Any() ? allInventoryItems.Where(x => OilCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();

            var terminatedFleetIds = db.Fleets.Where(x => x.IsContractTerminated).Select(x=>x.FleetsID).ToList();
            var AllFleetsInBranch = db.Fleets.Where(x => x.BranchId == id&&!x.IsContractTerminated).ToList();
            if (AllFleetsInBranch.Any() && timeSheet.Any())
            {
                var selectedFleetIds = timeSheet.Select(x => x.FleetsID).ToList();
                if (selectedFleetIds.Any())
                {
                    AllFleetsInBranch = AllFleetsInBranch.Where(x => !selectedFleetIds.Contains(x.FleetsID)).ToList();
                    if (AllFleetsInBranch.Any())
                    {
                        var groupedBranchFleets = AllFleetsInBranch.GroupBy(x => x.EquipmentTypeID);
                        foreach (var group in groupedBranchFleets)
                        {
                            var first = group.First();
                            var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID);
                            if (equipmentType.TimeSheetType == TimeSheetType.LightVehicle)
                            {
                                foreach (var g in group)
                                {
                                    var newTimeSheet = new LightVehicleTimeSheet
                                    {
                                        Date = date,
                                        FleetsID = g.FleetsID,
                                        MorningTimeStart = DateTime.Now,
                                        MorningTimeEnd = DateTime.Now,
                                        AfternoonTimeStart = DateTime.Now,
                                        AfternoonTimeEnd = DateTime.Now,
                                        NightTimeStart = DateTime.Now,
                                        NightTimeEnd = DateTime.Now,
                                        DateCreated = DateTime.Now.Date,
                                        CreatedBy = User.Identity.Name,
                                        ModifiedBy = g.ModifiedBy,
                                        LastModified = g.LastModified,
                                        BranchId = g.BranchId,
                                        BusinessUnitId = 1,
                                        RentalAgreementNum = g.RentFromExternalNum,
                                        OperatorId = g.OperatorId,
                                        TimeSheetStatus = MaintenanceConst.Pending,
                                        EquipmentCategoryId = g.EquipmentIdentityID,
                                        EquipmentTypeId = g.EquipmentTypeID,
                                        EquipmentNameId = g.EquipmentNameID,
                                    };
                                    db.LightVehicleTimeSheets.Add(newTimeSheet);
                                    db.SaveChanges();
                                    timeSheet.Add(newTimeSheet);
                                }
                            }
                        }

                    }
                }

            }

            if (!timeSheet.Any())
            {
                var AllFleets = db.Fleets.Where(x => x.BranchId == id&& !x.IsContractTerminated).ToList();
                var groupedFleets = AllFleets.GroupBy(x => x.EquipmentTypeID);
                foreach (var group in groupedFleets)
                {
                    var first = group.First();
                    var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID);
                    if (equipmentType.TimeSheetType == TimeSheetType.LightVehicle)
                    {
                        foreach (var timesht in group)
                        {
                            var newTimeSheet = new LightVehicleTimeSheet
                            {
                                Date = date,
                                FleetsID = timesht.FleetsID,
                                MorningTimeStart = DateTime.Now,
                                MorningTimeEnd = DateTime.Now,
                                AfternoonTimeStart = DateTime.Now,
                                AfternoonTimeEnd = DateTime.Now,
                                NightTimeStart = DateTime.Now,
                                NightTimeEnd = DateTime.Now,
                                DateCreated = DateTime.Now.Date,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = timesht.ModifiedBy,
                                LastModified = timesht.LastModified,
                                BranchId = timesht.BranchId,
                                BusinessUnitId = 1,
                                RentalAgreementNum = timesht.RentFromExternalNum,
                                OperatorId = timesht.OperatorId,
                                TimeSheetStatus = MaintenanceConst.Pending,
                                EquipmentCategoryId = timesht.EquipmentIdentityID,
                                EquipmentTypeId = timesht.EquipmentTypeID,
                                EquipmentNameId = timesht.EquipmentNameID,
                            };

                            db.LightVehicleTimeSheets.Add(newTimeSheet);
                            db.SaveChanges();
                        }
                    }
                }
            }
            var RFleetIds = timeSheet.Select(x => x.FleetsID).ToList();
            var Fleets = db.Fleets.Where(x => RFleetIds.Contains(x.FleetsID)&& !x.IsContractTerminated).ToList();

            var today = date.Date;

            var fleetIds = timeSheet
                .Where(x => x.Date != null && x.Date.Date == today)   // compare only the date
                .Select(x => x.FleetsID).ToList();
            var registeredIds = db.LightVehicleTimeSheets.Where(x => RFleetIds.Contains(x.FleetsID) && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date)).Select(x => x.FleetsID).ToList();
            timeSheet = timeSheet.Where(x => !registeredIds.Contains(x.FleetsID)).ToList();
            timeSheet = timeSheet.Where(x => !terminatedFleetIds.Contains(x.FleetsID)).ToList();
            foreach (var timesht in timeSheet)
            {
                var dbFleet = db.Fleets.FirstOrDefault(x => x.FleetsID == timesht.FleetsID);
                var newTimeSheet = new LightVehicleTimeSheet
                {
                    Date = date,
                    FleetsID = timesht.FleetsID,
                    MorningTimeStart = timesht.MorningTimeStart,
                    MorningTimeEnd = timesht.MorningTimeStart,
                    AfternoonTimeStart = timesht.MorningTimeStart,
                    AfternoonTimeEnd = timesht.MorningTimeStart,
                    NightTimeStart = timesht.MorningTimeStart,
                    NightTimeEnd = timesht.MorningTimeStart,
                    DateCreated = DateTime.Now.Date,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = timesht.ModifiedBy,
                    LastModified = timesht.LastModified,
                    BranchId = timesht.BranchId,
                    RentalAgreementNum = dbFleet.RentFromExternalNum,
                    BusinessUnitId = timesht.BusinessUnitId,
                    OperatorId = timesht.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    EquipmentCategoryId = timesht.EquipmentCategoryId,
                    EquipmentTypeId = timesht.EquipmentTypeId,
                    EquipmentNameId = timesht.EquipmentNameId,
                };
                var companySetting = db.RequestToInspections.FirstOrDefault();
                if (companySetting != null)
                {

                    
                    var plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var lubItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && OilLubItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    newTimeSheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
                   
                    var issuedOilItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && oilItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    newTimeSheet.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
                   
                    var issuedSpareItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && allSparePartItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var issuedBatteryItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(date)
                        && allBatteryItems.Contains(ii.ItemCode)).ToList();
                    var issuedTyreItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && allTyreItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                    newTimeSheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == timesht.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(date));
                    decimal maintenancePayment = 0;

                    if (jobOrder != null)
                    {
                        string maintenanceCenterId = jobOrder.MaintenanceCenterId.ToString();
                        var maintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID.ToString() == maintenanceCenterId);
                        var ServiceCenterJobs = db.ServiceCenterJob.Where(x => x.JobOrderID == jobOrder.JobOrderID).ToList();
                        foreach (var ServiceCenterJob in ServiceCenterJobs)
                        {
                            if (maintenanceCenter != null && maintenanceCenter.IsExternal)
                            {
                                var externalServicePayment = db.ExternalServiceCenterPayments.Where(x => x.ServiceCenterJobId == ServiceCenterJob.ServiceCenterJobId).ToList();
                                maintenancePayment += externalServicePayment.Sum(x => (decimal?)x.Total) ?? 0;

                            }
                            else
                            {
                                var StoreRequests = db.StoreRequisitions.Where(x => x.ServiceJobOrderNo == ServiceCenterJob.ServiceCenterJobId.ToString()).Select(x => x.StoreRequisitionID).ToList();
                                if (StoreRequests.Any())
                                {
                                    var IssuedIds = db.Issues.Where(x => StoreRequests.Contains(x.StoreRequisitionID)).Select(x => x.IssueID).ToList();
                                    if (IssuedIds.Any())
                                    {
                                        var IssuedItems = db.IssueItems.Where(x => IssuedIds.Contains(x.IssueID)).ToList();
                                        foreach (var Item in IssuedItems)
                                        {
                                            maintenancePayment += Item.UnitPrice * Item.IssuedQuantity;
                                        }
                                    }

                                }
                            }
                            var crewIds = db.ServiceCrewWorkLogs.Where(x => x.JobId == ServiceCenterJob.ServiceCenterJobId).ToList();
                            foreach (var crew in crewIds)
                            {
                                decimal companyCost = db.RepairType.FirstOrDefault(x => x.RepairTypeID.ToString() == crew.RepiarTypeId)?.TotalAmountCompany ?? 0;
                                maintenancePayment += companyCost;
                            }
                        }

                    }
                    newTimeSheet.MaintenanceCost += maintenancePayment;
                }
                db.LightVehicleTimeSheets.Add(newTimeSheet);
                db.SaveChanges();
            }
            GenerateDryLoading(id, date);
            GenerateDumpTruck(id, date);
            GenerateMachinery(id, date);
            GenerateWagonDrill(id, date);
            var formattedData = new
            {
                Message = "Timesheets Created",
                Result = "OK"
            };
            return Json(formattedData, JsonRequestBehavior.AllowGet);
        }
        private void GenerateMachinery(int id, DateTime date)
        {
            
            List<MachineryWorkingTimeSheet> timeSheet = new List<MachineryWorkingTimeSheet>();
            var timeSheets = db.MachineryWorkingTimeSheets
                    .Where(x => x.BranchId == id)
                    .GroupBy(x => x.FleetsID).ToList();
            foreach (var g in timeSheets)
            {
                var latest = g.OrderByDescending(x => x.Date).FirstOrDefault();
                timeSheet.Add(latest);
            }
            var allNonFuelItems = db.NonFuelItemSettings.ToList();
            var allInventoryItems = db.InventoryItems.ToList();
            var OilLubCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsLubricant == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var OilLubItems = allInventoryItems.Any() ? allInventoryItems.Where(x => OilLubCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allSparePartCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsSparePart == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allSparePartItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allSparePartCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allBatteryCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsBattery == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allBatteryItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allBatteryCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allTyreCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsTire == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allTyreItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allTyreCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var OilCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsOil == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var oilItems = allInventoryItems.Any() ? allInventoryItems.Where(x => OilCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();

            var terminatedFleetIds = db.Fleets.Where(x => x.IsContractTerminated).Select(x => x.FleetsID).ToList();
            var AllFleetsInBranch = db.Fleets.Where(x => x.BranchId == id&&!x.IsContractTerminated).ToList();
            if (AllFleetsInBranch.Any() && timeSheet.Any())
            {
                var selectedFleetIds = timeSheet.Select(x => x.FleetsID).ToList();
                if (selectedFleetIds.Any())
                {
                    AllFleetsInBranch = AllFleetsInBranch.Where(x => !selectedFleetIds.Contains(x.FleetsID)).ToList();
                    if (AllFleetsInBranch.Any())
                    {
                        var groupedBranchFleets = AllFleetsInBranch.GroupBy(x => x.EquipmentTypeID);
                        foreach (var group in groupedBranchFleets)
                        {
                            var first = group.First();
                            var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID);
                            if (equipmentType.TimeSheetType == TimeSheetType.Machinery)
                            {
                                foreach (var g in group)
                                {
                                    var newTimeSheet = new MachineryWorkingTimeSheet
                                    {
                                        Date = date,
                                        FleetsID = g.FleetsID,
                                        MorningTimeStart = DateTime.Now,
                                        MorningTimeEnd = DateTime.Now,
                                        AfternoonTimeStart = DateTime.Now,
                                        AfternoonTimeEnd = DateTime.Now,
                                        NightTimeStart = DateTime.Now,
                                        NightTimeEnd = DateTime.Now,
                                        DateCreated = DateTime.Now.Date,
                                        CreatedBy = User.Identity.Name,
                                        ModifiedBy = g.ModifiedBy,
                                        LastModified = g.LastModified,
                                        RentalAgreementNum = g.RentFromExternalNum,
                                        BranchId = g.BranchId,
                                        BusinessUnitId = 1,
                                        OperatorId = g.OperatorId,
                                        TimeSheetStatus = MaintenanceConst.Pending,
                                        EquipmentCategoryId = g.EquipmentIdentityID,
                                        EquipmentTypeId = g.EquipmentTypeID,
                                        EquipmentNameId = g.EquipmentNameID,
                                    };
                                    db.MachineryWorkingTimeSheets.Add(newTimeSheet);
                                    db.SaveChanges();
                                    timeSheet.Add(newTimeSheet);
                                }
                            }
                        }

                    }
                }

            }
            var today = date.Date;
            if (!timeSheet.Any())
            {
                var AllFleets = db.Fleets.Where(x => x.BranchId == id&&!x.IsContractTerminated).ToList();
                var groupedFleets = AllFleets.GroupBy(x => x.EquipmentTypeID);
                foreach (var group in groupedFleets)
                {
                    var first = group.First();
                    var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID);
                    if (equipmentType.TimeSheetType == TimeSheetType.Machinery)
                    {
                        foreach (var timesht in group)
                        {
                            var newTimeSheet = new MachineryWorkingTimeSheet
                            {
                                Date = date,
                                FleetsID = timesht.FleetsID,
                                MorningTimeStart = DateTime.Now,
                                MorningTimeEnd = DateTime.Now,
                                AfternoonTimeStart = DateTime.Now,
                                AfternoonTimeEnd = DateTime.Now,
                                NightTimeStart = DateTime.Now,
                                NightTimeEnd = DateTime.Now,
                                RentalAgreementNum = timesht.RentFromExternalNum,
                                DateCreated = DateTime.Now.Date,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = timesht.ModifiedBy,
                                LastModified = timesht.LastModified,
                                BranchId = timesht.BranchId,
                                BusinessUnitId = 1,
                                OperatorId = timesht.OperatorId,
                                TimeSheetStatus = MaintenanceConst.Pending,
                                EquipmentCategoryId = timesht.EquipmentIdentityID,
                                EquipmentTypeId = timesht.EquipmentTypeID,
                                EquipmentNameId = timesht.EquipmentNameID,
                            };

                            db.MachineryWorkingTimeSheets.Add(newTimeSheet);
                            db.SaveChanges();
                        }
                    }
                }
            }
            var RFleetIds = timeSheet.Select(x => x.FleetsID).ToList();
            var Fleets = db.Fleets.Where(x => RFleetIds.Contains(x.FleetsID)).ToList();
            var fleetIds = timeSheet
                .Where(x => x.Date.ToString() != null && x.Date.Date == today)   // compare only the date
                .Select(x => x.FleetsID).ToList();
            var registeredIds = db.MachineryWorkingTimeSheets.Where(x => RFleetIds.Contains(x.FleetsID) && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date)).Select(x => x.FleetsID).ToList();

            timeSheet = timeSheet.Where(x => !registeredIds.Contains(x.FleetsID)).ToList();
            timeSheet = timeSheet.Where(x => !terminatedFleetIds.Contains(x.FleetsID)).ToList();

            foreach (var timesht in timeSheet)
            {
                var dbFleet = db.Fleets.FirstOrDefault(x => x.FleetsID == timesht.FleetsID);
                var newTimeSheet = new MachineryWorkingTimeSheet
                {
                    FleetsID = timesht.FleetsID,
                    Date = date,
                    MorningTimeStart = timesht.MorningTimeStart,
                    MorningTimeEnd = timesht.MorningTimeStart,
                    AfternoonTimeStart = timesht.MorningTimeStart,
                    AfternoonTimeEnd = timesht.MorningTimeStart,
                    NightTimeStart = timesht.MorningTimeStart,
                    NightTimeEnd = timesht.MorningTimeStart,
                    DateCreated = DateTime.Now.Date,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = timesht.ModifiedBy,
                    RentalAgreementNum = dbFleet.RentFromExternalNum,
                    LastModified = timesht.LastModified,
                    BranchId = timesht.BranchId,
                    BusinessUnitId = timesht.BusinessUnitId,
                    OperatorId = timesht.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    EquipmentCategoryId = timesht.EquipmentCategoryId,
                    EquipmentTypeId = timesht.EquipmentTypeId,
                    EquipmentNameId = timesht.EquipmentNameId,
                };
                var companySetting = db.RequestToInspections.FirstOrDefault();
                if (companySetting != null)
                {
                   
                    var plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var lubItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && OilLubItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    newTimeSheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
                    plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var issuedOilItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && oilItems.Contains(ii.ItemCode)).ToList();
                    timesht.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    timesht.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var allFuelItems = db.FuelItemSettings.Select(x => x.ItemCode).ToList();
                    plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var issuedSpareItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && !allFuelItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var issuedBatteryItems = db.IssueItems
               .Where(ii =>
                       ii.Issue.PlateNo == plateNo
                       && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                           == DbFunctions.TruncateTime(date)
                       && allBatteryItems.Contains(ii.ItemCode)).ToList();
                    var issuedTyreItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && allTyreItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                    newTimeSheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == timesht.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(date));
                    decimal maintenancePayment = 0;

                    if (jobOrder != null)
                    {
                        string maintenanceCenterId = jobOrder.MaintenanceCenterId.ToString();
                        var maintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID.ToString() == maintenanceCenterId);
                        var ServiceCenterJobs = db.ServiceCenterJob.Where(x => x.JobOrderID == jobOrder.JobOrderID).ToList();
                        foreach (var ServiceCenterJob in ServiceCenterJobs)
                        {
                            if (maintenanceCenter != null && maintenanceCenter.IsExternal)
                            {
                                var externalServicePayment = db.ExternalServiceCenterPayments.Where(x => x.ServiceCenterJobId == ServiceCenterJob.ServiceCenterJobId).ToList();
                                maintenancePayment += externalServicePayment.Sum(x => (decimal?)x.Total) ?? 0;

                            }
                            else
                            {
                                var StoreRequests = db.StoreRequisitions.Where(x => x.ServiceJobOrderNo == ServiceCenterJob.ServiceCenterJobId.ToString()).Select(x => x.StoreRequisitionID).ToList();
                                if (StoreRequests.Any())
                                {
                                    var IssuedIds = db.Issues.Where(x => StoreRequests.Contains(x.StoreRequisitionID)).Select(x => x.IssueID).ToList();
                                    if (IssuedIds.Any())
                                    {
                                        var IssuedItems = db.IssueItems.Where(x => IssuedIds.Contains(x.IssueID)).ToList();
                                        foreach (var Item in IssuedItems)
                                        {
                                            maintenancePayment += Item.UnitPrice * Item.IssuedQuantity;
                                        }
                                    }

                                }
                            }
                            var crewIds = db.ServiceCrewWorkLogs.Where(x => x.JobId == ServiceCenterJob.ServiceCenterJobId).ToList();
                            foreach (var crew in crewIds)
                            {
                                decimal companyCost = db.RepairType.FirstOrDefault(x => x.RepairTypeID.ToString() == crew.RepiarTypeId)?.TotalAmountCompany ?? 0;
                                maintenancePayment += companyCost;
                            }
                        }

                    }
                    newTimeSheet.MaintenanceCost += maintenancePayment;
                }
                db.MachineryWorkingTimeSheets.Add(newTimeSheet);

            }
            db.SaveChanges();
        }
        private void GenerateDumpTruck(int id, DateTime date)
        {
            
            List<DumpTruckTimeSheet> timeSheet = new List<DumpTruckTimeSheet>();
            var timeSheets = db.DumpTruckTimeSheets
                    .Where(x => x.BranchId == id)
                    .GroupBy(x => x.FleetsID).ToList();
            foreach (var g in timeSheets)
            {
                var latest = g.OrderByDescending(x => x.Date).FirstOrDefault();
                timeSheet.Add(latest);
            }
            var allNonFuelItems = db.NonFuelItemSettings.ToList();
            var allInventoryItems = db.InventoryItems.ToList();
            var OilLubCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsLubricant == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var OilLubItems = allInventoryItems.Any() ? allInventoryItems.Where(x => OilLubCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allSparePartCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsSparePart == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allSparePartItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allSparePartCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allBatteryCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsBattery == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allBatteryItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allBatteryCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allTyreCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsTire == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allTyreItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allTyreCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var OilCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsOil == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var oilItems = allInventoryItems.Any() ? allInventoryItems.Where(x => OilCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();

            var terminatedFleetIds = db.Fleets.Where(x => x.IsContractTerminated).Select(x => x.FleetsID).ToList();
            var AllFleetsInBranch = db.Fleets.Where(x => x.BranchId == id&&!x.IsContractTerminated).ToList();
            if (AllFleetsInBranch.Any() && timeSheet.Any())
            {
                var selectedFleetIds = timeSheet.Select(x => x.FleetsID).ToList();
                if (selectedFleetIds.Any())
                {
                    AllFleetsInBranch = AllFleetsInBranch.Where(x => !selectedFleetIds.Contains(x.FleetsID)).ToList();
                    if (AllFleetsInBranch.Any())
                    {
                        var groupedBranchFleets = AllFleetsInBranch.GroupBy(x => x.EquipmentTypeID);
                        foreach (var group in groupedBranchFleets)
                        {
                            var first = group.First();
                            var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID);
                            if (equipmentType.TimeSheetType == TimeSheetType.DumpTruck)
                            {
                                foreach (var g in group)
                                {
                                    var newTimeSheet = new DumpTruckTimeSheet
                                    {
                                        Date = date,
                                        FleetsID = g.FleetsID,
                                        MorningTimeStart = DateTime.Now,
                                        MorningTimeEnd = DateTime.Now,
                                        AfternoonTimeStart = DateTime.Now,
                                        AfternoonTimeEnd = DateTime.Now,
                                        NightTimeStart = DateTime.Now,
                                        NightTimeEnd = DateTime.Now,
                                        DateCreated = DateTime.Now.Date,
                                        CreatedBy = User.Identity.Name,
                                        ModifiedBy = g.ModifiedBy,
                                        LastModified = g.LastModified,
                                        BranchId = g.BranchId,
                                        RentalAgreementNum = g.RentFromExternalNum,
                                        BusinessUnitId = 1,
                                        OperatorId = g.OperatorId,
                                        TimeSheetStatus = MaintenanceConst.Pending,
                                        EquipmentCategoryId = g.EquipmentIdentityID,
                                        EquipmentTypeId = g.EquipmentTypeID,
                                        EquipmentNameId = g.EquipmentNameID,
                                    };
                                    db.DumpTruckTimeSheets.Add(newTimeSheet);
                                    db.SaveChanges();
                                    timeSheet.Add(newTimeSheet);
                                }
                            }
                        }

                    }
                }

            }
            var today = date.Date;
            if (!timeSheet.Any())
            {
                var AllFleets = db.Fleets.Where(x => x.BranchId == id&&!x.IsContractTerminated).ToList();
                var groupedFleets = AllFleets.GroupBy(x => x.EquipmentTypeID);
                foreach (var group in groupedFleets)
                {
                    var first = group.First();
                    var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID);
                    if (equipmentType.TimeSheetType == TimeSheetType.DumpTruck)
                    {
                        foreach (var timesht in group)
                        {
                            var newTimeSheet = new DumpTruckTimeSheet
                            {
                                Date = date, // use the target date
                                FleetsID = timesht.FleetsID,
                                FuelConsumption = 0,
                                OilConsumption = 0,
                                GreaseConsumption = 0,
                                IdleHour = 0,
                                ReasonForIdle = "Reason",
                                DownHour = 0,
                                ReasonForDown = "Reason",
                                RentalRate = 0,
                                IdealRentalRate = 0,

                                // Metadata
                                DateCreated = DateTime.Now,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = timesht.ModifiedBy,
                                LastModified = timesht.LastModified,
                                BranchId = timesht.BranchId,
                                BusinessUnitId = 1,
                                RentalAgreementNum = timesht.RentFromExternalNum,
                                OperatorId = timesht.OperatorId,
                                TimeSheetStatus = MaintenanceConst.Pending,
                                EquipmentCategoryId = timesht.EquipmentIdentityID,
                                EquipmentTypeId = timesht.EquipmentTypeID,
                                EquipmentNameId = timesht.EquipmentNameID,
                            };

                            db.DumpTruckTimeSheets.Add(newTimeSheet);
                            db.SaveChanges();
                        }
                    }
                }
            }
            var RFleetIds = timeSheet.Select(x => x.FleetsID).ToList();
    //        var Fleets = db.Fleets.Where(x => RFleetIds.Contains(x.FleetsID)).ToList();
    //        var fleetIds = timeSheet
    //.Where(x => x.Date.ToString()!=null && x.Date.Date == date.Date)
    //.Select(x => x.FleetsID)
    //.ToList();
            var registeredIds = db.DumpTruckTimeSheets.Where(x => RFleetIds.Contains(x.FleetsID) && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date)).Select(x=>x.FleetsID).ToList();
            timeSheet = timeSheet.Where(x => !registeredIds.Contains(x.FleetsID)).ToList();
            timeSheet = timeSheet.Where(x => !terminatedFleetIds.Contains(x.FleetsID)).ToList();
            foreach (var timesht in timeSheet)
            {
                var dbFleet = db.Fleets.FirstOrDefault(x => x.FleetsID == timesht.FleetsID);
                var newTimeSheet = new DumpTruckTimeSheet
                {

                    Date = date, // use the target date
                    FleetsID = timesht.FleetsID,
                    FuelConsumption = timesht.FuelConsumption,
                    OilConsumption = timesht.OilConsumption,
                    GreaseConsumption = timesht.GreaseConsumption,
                    IdleHour = timesht.IdleHour,
                    ReasonForIdle = timesht.ReasonForIdle,
                    DownHour = timesht.DownHour,
                    ReasonForDown = timesht.ReasonForDown,
                    RentalRate = timesht.RentalRate,
                    IdealRentalRate = timesht.IdealRentalRate,

                    // Metadata
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = timesht.ModifiedBy,
                    LastModified = timesht.LastModified,
                    BranchId = timesht.BranchId,
                    RentalAgreementNum = dbFleet.RentFromExternalNum,
                    BusinessUnitId = timesht.BusinessUnitId,
                    OperatorId = timesht.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    EquipmentCategoryId = timesht.EquipmentCategoryId,
                    EquipmentTypeId = timesht.EquipmentTypeId,
                    EquipmentNameId = timesht.EquipmentNameId,
                };
                var companySetting = db.RequestToInspections.FirstOrDefault();
                if (companySetting != null)
                {
                  
                    var plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var lubItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && OilLubItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    newTimeSheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
                    plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var issuedOilItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && oilItems.Contains(ii.ItemCode)).ToList();
                    timesht.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    timesht.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
                    plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var issuedSpareItems = db.IssueItems
                     .Where(ii =>
                             ii.Issue.PlateNo == plateNo
                             && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                 == DbFunctions.TruncateTime(date)
                             && allSparePartItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var issuedBatteryItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(date)
                        && allBatteryItems.Contains(ii.ItemCode)).ToList();
                    var issuedTyreItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && allTyreItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                    newTimeSheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == timesht.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(date));
                    decimal maintenancePayment = 0;

                    if (jobOrder != null)
                    {
                        string maintenanceCenterId = jobOrder.MaintenanceCenterId.ToString();
                        var maintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID.ToString() == maintenanceCenterId);
                        var ServiceCenterJobs = db.ServiceCenterJob.Where(x => x.JobOrderID == jobOrder.JobOrderID).ToList();
                        foreach (var ServiceCenterJob in ServiceCenterJobs)
                        {
                            if (maintenanceCenter != null && maintenanceCenter.IsExternal)
                            {
                                var externalServicePayment = db.ExternalServiceCenterPayments.Where(x => x.ServiceCenterJobId == ServiceCenterJob.ServiceCenterJobId).ToList();
                                maintenancePayment += externalServicePayment.Sum(x => (decimal?)x.Total) ?? 0;

                            }
                            else
                            {
                                var StoreRequests = db.StoreRequisitions.Where(x => x.ServiceJobOrderNo == ServiceCenterJob.ServiceCenterJobId.ToString()).Select(x => x.StoreRequisitionID).ToList();
                                if (StoreRequests.Any())
                                {
                                    var IssuedIds = db.Issues.Where(x => StoreRequests.Contains(x.StoreRequisitionID)).Select(x => x.IssueID).ToList();
                                    if (IssuedIds.Any())
                                    {
                                        var IssuedItems = db.IssueItems.Where(x => IssuedIds.Contains(x.IssueID)).ToList();
                                        foreach (var Item in IssuedItems)
                                        {
                                            maintenancePayment += Item.UnitPrice * Item.IssuedQuantity;
                                        }
                                    }

                                }
                            }
                            var crewIds = db.ServiceCrewWorkLogs.Where(x => x.JobId == ServiceCenterJob.ServiceCenterJobId).ToList();
                            foreach (var crew in crewIds)
                            {
                                decimal companyCost = db.RepairType.FirstOrDefault(x => x.RepairTypeID.ToString() == crew.RepiarTypeId)?.TotalAmountCompany ?? 0;
                                maintenancePayment += companyCost;
                            }
                        }

                    }
                    newTimeSheet.MaintenanceCost += maintenancePayment;
                }
                db.DumpTruckTimeSheets.Add(newTimeSheet);

            }
            db.SaveChanges();
        }
        private void GenerateDryLoading(int id, DateTime date)
        {
            
            List<DryLoadingTimeSheet> timeSheet = new List<DryLoadingTimeSheet>();
            var timeSheets = db.DryLoadingTimeSheets
                    .Where(x => x.BranchId == id)
                    .GroupBy(x => x.FleetsID).ToList();
            foreach (var g in timeSheets)
            {
                var latest = g.OrderByDescending(x => x.TripStartDate).FirstOrDefault();
                timeSheet.Add(latest);
            }
            var allNonFuelItems = db.NonFuelItemSettings.ToList();
            var allInventoryItems = db.InventoryItems.ToList();
            var OilLubCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsLubricant == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var OilLubItems = allInventoryItems.Any() ? allInventoryItems.Where(x => OilLubCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allSparePartCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsSparePart == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allSparePartItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allSparePartCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allBatteryCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsBattery == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allBatteryItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allBatteryCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allTyreCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsTire == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allTyreItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allTyreCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var OilCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsOil == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var oilItems = allInventoryItems.Any() ? allInventoryItems.Where(x => OilCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();

            var terminatedFleetIds = db.Fleets.Where(x => x.IsContractTerminated).Select(x => x.FleetsID).ToList();
            var AllFleetsInBranch = db.Fleets.Where(x => x.BranchId == id&&!x.IsContractTerminated).ToList();
            if (AllFleetsInBranch.Any() && timeSheet.Any())
            {
                var selectedFleetIds = timeSheet.Select(x => x.FleetsID).ToList();
                if (selectedFleetIds.Any())
                {
                    AllFleetsInBranch = AllFleetsInBranch.Where(x => !selectedFleetIds.Contains(x.FleetsID)).ToList();
                    if (AllFleetsInBranch.Any())
                    {
                        var groupedBranchFleets = AllFleetsInBranch.GroupBy(x => x.EquipmentTypeID);
                        foreach (var group in groupedBranchFleets)
                        {
                            var first = group.First();
                            var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID);
                            if (equipmentType.TimeSheetType == TimeSheetType.DryLoading)
                            {
                                foreach (var g in group)
                                {
                                    var newTimeSheet = new DryLoadingTimeSheet
                                    {

                                        FleetsID = g.FleetsID,
                                        TripStartDate = DateTime.Now,
                                        TripArrivalDate = DateTime.Now,

                                        TripStartPlace = "To Be Edited",
                                        TriplArrivalPlace = "To Be Edited",
                                        WorkItem = "To Be Edited",
                                        WorkingHour = 0,
                                        Asphalt = 0,
                                        Gravel = 0,
                                        TotalKmTravelled = 0,
                                        FuelInLiter = 0,
                                        RentalAgreementNum = g.RentFromExternalNum,
                                        PerDiem = 0,
                                        IdleHour = 0,
                                        ReasonForIdle = "To Be Edited",
                                        DateCreated = DateTime.Now.Date,
                                        CreatedBy = User.Identity.Name,
                                        ModifiedBy = g.ModifiedBy,
                                        LastModified = DateTime.Now.Date,
                                        BranchId = g.BranchId,
                                        BusinessUnitId = 1,
                                        OperatorId = g.OperatorId,
                                        TimeSheetStatus = MaintenanceConst.Pending,
                                        EquipmentCategoryId = g.EquipmentIdentityID,
                                        EquipmentTypeId = g.EquipmentTypeID,
                                        EquipmentNameId = g.EquipmentNameID,

                                    };
                                    db.DryLoadingTimeSheets.Add(newTimeSheet);
                                    db.SaveChanges();
                                    timeSheet.Add(newTimeSheet);
                                }
                            }
                        }

                    }
                }

            }
            var today = date.Date;
            if (!timeSheet.Any())
            {
                var AllFleets = db.Fleets.Where(x => x.BranchId == id&&!x.IsContractTerminated).ToList();
                var groupedFleets = AllFleets.GroupBy(x => x.EquipmentTypeID);
                foreach (var group in groupedFleets)
                {
                    var first = group.First();
                    var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID);
                    if (equipmentType.TimeSheetType == TimeSheetType.DryLoading)
                    {
                        foreach (var timesht in group)
                        {
                            var newTimeSheet = new DryLoadingTimeSheet
                            {

                                FleetsID = timesht.FleetsID,
                                TripStartDate = DateTime.Now,
                                TripArrivalDate = DateTime.Now,

                                TripStartPlace = "To Be Edited",
                                TriplArrivalPlace = "To Be Edited",
                                WorkItem = "To Be Edited",
                                WorkingHour = 0,
                                Asphalt = 0,
                                Gravel = 0,
                                TotalKmTravelled = 0,
                                FuelInLiter = 0,
                                PerDiem = 0,
                                RentalAgreementNum = timesht.RentFromExternalNum,
                                IdleHour = 0,
                                ReasonForIdle = "To Be Edited",
                                DateCreated = DateTime.Now.Date,
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = timesht.ModifiedBy,
                                LastModified = timesht.LastModified,
                                BranchId = timesht.BranchId,
                                BusinessUnitId = 1,
                                OperatorId = timesht.OperatorId,
                                TimeSheetStatus = MaintenanceConst.Pending,
                                EquipmentCategoryId = timesht.EquipmentIdentityID,
                                EquipmentTypeId = timesht.EquipmentTypeID,
                                EquipmentNameId = timesht.EquipmentNameID,

                            };
                            db.DryLoadingTimeSheets.Add(newTimeSheet);
                        }
                        db.SaveChanges();

                    }
                }
            }
            var RFleetIds = timeSheet.Select(x => x.FleetsID).ToList();
            var Fleets = db.Fleets.Where(x => RFleetIds.Contains(x.FleetsID)).ToList();
            var fleetIds = timeSheet
                .Where(x => x.TripStartDate.ToString() != null && x.TripStartDate.Date == today)   // compare only the date
                .Select(x => x.FleetsID).ToList();
            var registeredIds = db.DryLoadingTimeSheets.Where(x => RFleetIds.Contains(x.FleetsID) && DbFunctions.TruncateTime(x.TripStartDate) == DbFunctions.TruncateTime(date)).Select(x => x.FleetsID).ToList();
            timeSheet = timeSheet.Where(x => !registeredIds.Contains(x.FleetsID)).ToList();
            timeSheet = timeSheet.Where(x => !terminatedFleetIds.Contains(x.FleetsID)).ToList();
            foreach (var timesht in timeSheet)
            {
                var dbFleet = db.Fleets.FirstOrDefault(x => x.FleetsID == timesht.FleetsID);
                var newTimeSheet = new DryLoadingTimeSheet
                {

                    FleetsID = timesht.FleetsID,
                    TripStartDate = timesht.TripStartDate,
                    TripArrivalDate = timesht.TripStartDate,

                    TripStartPlace = timesht.TripStartPlace,
                    TriplArrivalPlace = timesht.TriplArrivalPlace,
                    WorkItem = timesht.WorkItem,
                    WorkingHour = timesht.WorkingHour,
                    Asphalt = timesht.Asphalt,
                    Gravel = timesht.Gravel,
                    TotalKmTravelled = timesht.TotalKmTravelled,
                    FuelInLiter = timesht.FuelInLiter,
                    PerDiem = timesht.PerDiem,
                    RentalAgreementNum = dbFleet.RentFromExternalNum,
                    IdleHour = timesht.IdleHour,
                    ReasonForIdle = timesht.ReasonForIdle,
                    DateCreated = DateTime.Now.Date,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = timesht.ModifiedBy,
                    LastModified = timesht.LastModified,
                    BranchId = timesht.BranchId,
                    BusinessUnitId = timesht.BusinessUnitId,
                    OperatorId = timesht.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    EquipmentCategoryId = timesht.EquipmentCategoryId,
                    EquipmentTypeId = timesht.EquipmentTypeId,
                    EquipmentNameId = timesht.EquipmentNameId,
                };
                var companySetting = db.RequestToInspections.FirstOrDefault();
                if (companySetting != null)
                {
                    
                    var plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var lubItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && OilLubItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    newTimeSheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
                    plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var issuedOilItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && oilItems.Contains(ii.ItemCode)).ToList();
                    timesht.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    timesht.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
                    plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var issuedSpareItems = db.IssueItems
                     .Where(ii =>
                             ii.Issue.PlateNo == plateNo
                             && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                 == DbFunctions.TruncateTime(date)
                             && allSparePartItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var issuedBatteryItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(date)
                        && allBatteryItems.Contains(ii.ItemCode)).ToList();
                    var issuedTyreItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && allTyreItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                    newTimeSheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == timesht.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(date));
                    decimal maintenancePayment = 0;

                    if (jobOrder != null)
                    {
                        string maintenanceCenterId = jobOrder.MaintenanceCenterId.ToString();
                        var maintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID.ToString() == maintenanceCenterId);
                        var ServiceCenterJobs = db.ServiceCenterJob.Where(x => x.JobOrderID == jobOrder.JobOrderID).ToList();
                        foreach (var ServiceCenterJob in ServiceCenterJobs)
                        {
                            if (maintenanceCenter != null && maintenanceCenter.IsExternal)
                            {
                                var externalServicePayment = db.ExternalServiceCenterPayments.Where(x => x.ServiceCenterJobId == ServiceCenterJob.ServiceCenterJobId).ToList();
                                maintenancePayment += externalServicePayment.Sum(x => (decimal?)x.Total) ?? 0;

                            }
                            else
                            {
                                var StoreRequests = db.StoreRequisitions.Where(x => x.ServiceJobOrderNo == ServiceCenterJob.ServiceCenterJobId.ToString()).Select(x => x.StoreRequisitionID).ToList();
                                if (StoreRequests.Any())
                                {
                                    var IssuedIds = db.Issues.Where(x => StoreRequests.Contains(x.StoreRequisitionID)).Select(x => x.IssueID).ToList();
                                    if (IssuedIds.Any())
                                    {
                                        var IssuedItems = db.IssueItems.Where(x => IssuedIds.Contains(x.IssueID)).ToList();
                                        foreach (var Item in IssuedItems)
                                        {
                                            maintenancePayment += Item.UnitPrice * Item.IssuedQuantity;
                                        }
                                    }

                                }
                            }
                            var crewIds = db.ServiceCrewWorkLogs.Where(x => x.JobId == ServiceCenterJob.ServiceCenterJobId).ToList();
                            foreach (var crew in crewIds)
                            {
                                decimal companyCost = db.RepairType.FirstOrDefault(x => x.RepairTypeID.ToString() == crew.RepiarTypeId)?.TotalAmountCompany ?? 0;
                                maintenancePayment += companyCost;
                            }
                        }

                    }
                    newTimeSheet.MaintenanceCost += maintenancePayment;
                }
                db.DryLoadingTimeSheets.Add(newTimeSheet);

            }
            db.SaveChanges();
        }
        private void GenerateWagonDrill(int id, DateTime date)
        {
            
            List<WagonDrillTimesheet> timeSheet = new List<WagonDrillTimesheet>();
            var timeSheets = db.WagonDrillTimesheets
                    .Where(x => x.BranchId == id)
                    .GroupBy(x => x.FleetsID).ToList();
            foreach (var g in timeSheets)
            {
                var latest = g.OrderByDescending(x => x.Date).FirstOrDefault();
                timeSheet.Add(latest);
            }
            var allNonFuelItems = db.NonFuelItemSettings.ToList();
            var allInventoryItems = db.InventoryItems.ToList();
            var OilLubCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsLubricant == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var OilLubItems = allInventoryItems.Any() ? allInventoryItems.Where(x => OilLubCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allSparePartCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsSparePart == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allSparePartItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allSparePartCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allBatteryCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsBattery == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allBatteryItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allBatteryCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var allTyreCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsTire == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var allTyreItems = allInventoryItems.Any() ? allInventoryItems.Where(x => allTyreCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();
            var OilCategories = allNonFuelItems.Any() ? allNonFuelItems.Where(x => x.IsOil == true).Select(x => x.ItemCategoryCode).ToList() : new List<string>();
            var oilItems = allInventoryItems.Any() ? allInventoryItems.Where(x => OilCategories.Contains(x.ItemCategoryCode)).Select(x => x.ItemCode).ToList() : new List<string>();

            var terminatedFleetIds = db.Fleets.Where(x => x.IsContractTerminated).Select(x => x.FleetsID).ToList();
            var AllFleetsInBranch = db.Fleets.Where(x => x.BranchId == id&&!x.IsContractTerminated).ToList();
            if (AllFleetsInBranch.Any() && timeSheet.Any())
            {
                var selectedFleetIds = timeSheet.Select(x => x.FleetsID).ToList();
                if (selectedFleetIds.Any())
                {
                    AllFleetsInBranch = AllFleetsInBranch.Where(x => !selectedFleetIds.Contains(x.FleetsID)).ToList();
                    if (AllFleetsInBranch.Any())
                    {
                        var groupedBranchFleets = AllFleetsInBranch.GroupBy(x => x.EquipmentTypeID);
                        foreach (var group in groupedBranchFleets)
                        {
                            var first = group.First();
                            var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID);
                            if (equipmentType.TimeSheetType == TimeSheetType.WagonDrill)
                            {
                                foreach (var g in group)
                                {
                                    var newTimeSheet = new WagonDrillTimesheet
                                    {
                                        FleetsID = g.FleetsID,
                                        Date = date,
                                        LabourCost = 0,
                                        SparePartCost = 0,
                                        BasicSalary = 0,
                                        OverTime = 0,
                                        BatteryCost = 0,
                                        OperationNetWorkingHour = 0,
                                        OperationDrilledHoleDepth = 0,
                                        NumberOfHolesDrilled = 0,
                                        OperationRateForMeter = 0,
                                        RentalAgreementNum = g.RentFromExternalNum,
                                        OperationAmount = 0,
                                        IdleHour = 0,
                                        IdleRateHour = 0,
                                        IdleAmount = 0,
                                        DownTime = 0,
                                        TotalAmountBeforeVAT = 0,
                                        VAT = 0,
                                        TotalAmount = 0,
                                        FuelInLiter = 0,
                                        FuelRate = 0,
                                        FuelCost = 0,
                                        ServiceCharge = 0,
                                        MaintenanceCost = 0,
                                        TireCost = 0,
                                        TotalCost = 0,
                                        NetAmount = 0,
                                        Perdium = 0,
                                        Salary = 0,
                                        RateInMeter = 0,

                                        // Required string fields
                                        WorkItem = "To be edited",
                                        FuelType = "To be edited",
                                        OilType = "To be edited",
                                        CreatedBy = User.Identity.Name,
                                        ModifiedBy = g.ModifiedBy,
                                        LastModified = DateTime.Now.Date,

                                        BranchId = g.BranchId,
                                        BusinessUnitId = 1,
                                        OperatorId = g.OperatorId,
                                        TimeSheetStatus = MaintenanceConst.Pending,
                                        EquipmentCategoryId = g.EquipmentIdentityID,
                                        EquipmentTypeId = g.EquipmentTypeID,
                                        EquipmentNameId = g.EquipmentNameID,
                                    };
                                    db.WagonDrillTimesheets.Add(newTimeSheet);
                                    db.SaveChanges();
                                    timeSheet.Add(newTimeSheet);
                                }
                            }
                        }

                    }
                }

            }
            var today = date.Date;
            if (!timeSheet.Any())
            {
                var AllFleets = db.Fleets.Where(x => x.BranchId == id&&!x.IsContractTerminated).ToList();
                var groupedFleets = AllFleets.GroupBy(x => x.EquipmentTypeID);
                foreach (var group in groupedFleets)
                {
                    var first = group.First();
                    var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == first.EquipmentTypeID);
                    if (equipmentType.TimeSheetType == TimeSheetType.WagonDrill)
                    {
                        foreach (var timesht in group)
                        {
                            var newTimeSheet = new WagonDrillTimesheet
                            {
                                FleetsID = timesht.FleetsID,
                                Date = date,
                                LabourCost = 0,
                                SparePartCost = 0,
                                BasicSalary = 0,
                                OverTime = 0,
                                BatteryCost = 0,
                                OperationNetWorkingHour = 0,
                                OperationDrilledHoleDepth = 0,
                                NumberOfHolesDrilled = 0,
                                OperationRateForMeter = 0,
                                OperationAmount = 0,
                                IdleHour = 0,
                                IdleRateHour = 0,
                                RentalAgreementNum = timesht.RentFromExternalNum,
                                IdleAmount = 0,
                                DownTime = 0,
                                TotalAmountBeforeVAT = 0,
                                VAT = 0,
                                TotalAmount = 0,
                                FuelInLiter = 0,
                                FuelRate = 0,
                                FuelCost = 0,
                                ServiceCharge = 0,
                                MaintenanceCost = 0,
                                TireCost = 0,
                                TotalCost = 0,
                                NetAmount = 0,
                                Perdium = 0,
                                Salary = 0,
                                RateInMeter = 0,

                                // Required string fields
                                WorkItem = "To be edited",
                                FuelType = "To be edited",
                                OilType = "To be edited",
                                CreatedBy = User.Identity.Name,
                                ModifiedBy = timesht.ModifiedBy,
                                LastModified = timesht.LastModified,

                                BranchId = timesht.BranchId,
                                BusinessUnitId = 1,
                                OperatorId = timesht.OperatorId,
                                TimeSheetStatus = MaintenanceConst.Pending,
                                EquipmentCategoryId = timesht.EquipmentIdentityID,
                                EquipmentTypeId = timesht.EquipmentTypeID,
                                EquipmentNameId = timesht.EquipmentNameID,
                            };

                            db.WagonDrillTimesheets.Add(newTimeSheet);
                        }
                        db.SaveChanges();

                    }
                }
            }
            var RFleetIds = timeSheet.Select(x => x.FleetsID).ToList();
            var Fleets = db.Fleets.Where(x => RFleetIds.Contains(x.FleetsID)).ToList();
            var fleetIds = timeSheet
                .Where(x => x.Date.ToString() != null && x.Date.Date == today)   // compare only the date
                .Select(x => x.FleetsID).ToList();
            var registeredIds = db.WagonDrillTimesheets.Where(x => RFleetIds.Contains(x.FleetsID) && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(date)).Select(x => x.FleetsID).ToList();
            timeSheet = timeSheet.Where(x => !registeredIds.Contains(x.FleetsID)).ToList();
            timeSheet = timeSheet.Where(x => !terminatedFleetIds.Contains(x.FleetsID)).ToList();
            foreach (var timesht in timeSheet)
            {
                var dbFleet = db.Fleets.FirstOrDefault(x => x.FleetsID == timesht.FleetsID);
                var newTimeSheet = new WagonDrillTimesheet
                {
                    FleetsID = timesht.FleetsID,
                    Date = date,
                    LabourCost = timesht.LabourCost,
                    SparePartCost = timesht.SparePartCost,
                    BasicSalary = timesht.BasicSalary,
                    OverTime = timesht.OverTime,
                    BatteryCost = timesht.BatteryCost,
                    OperationNetWorkingHour = timesht.OperationNetWorkingHour,
                    OperationDrilledHoleDepth = timesht.OperationDrilledHoleDepth,
                    NumberOfHolesDrilled = timesht.NumberOfHolesDrilled,
                    OperationRateForMeter = timesht.OperationRateForMeter,
                    OperationAmount = timesht.OperationAmount,
                    IdleHour = timesht.IdleHour,
                    IdleRateHour = timesht.IdleRateHour,
                    IdleAmount = timesht.IdleAmount,
                    DownTime = timesht.DownTime,
                    TotalAmountBeforeVAT = timesht.TotalAmountBeforeVAT,
                    VAT = timesht.VAT,
                    TotalAmount = timesht.TotalAmount,
                    FuelInLiter = timesht.FuelInLiter,
                    FuelRate = timesht.FuelRate,
                    FuelCost = timesht.FuelCost,
                    ServiceCharge = timesht.ServiceCharge,
                    MaintenanceCost = timesht.MaintenanceCost,
                    TireCost = timesht.TireCost,
                    TotalCost = timesht.TotalCost,
                    NetAmount = timesht.NetAmount,
                    Perdium = timesht.Perdium,
                    RentalAgreementNum = dbFleet.RentFromExternalNum,
                    Salary = timesht.Salary,
                    RateInMeter = timesht.RateInMeter,

                    // Required string fields
                    WorkItem = timesht.WorkItem,
                    FuelType = timesht.FuelType,
                    OilType = timesht.OilType,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = timesht.ModifiedBy,
                    LastModified = timesht.LastModified,

                    BranchId = timesht.BranchId,
                    BusinessUnitId = timesht.BusinessUnitId,
                    OperatorId = timesht.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    EquipmentCategoryId = timesht.EquipmentCategoryId,
                    EquipmentTypeId = timesht.EquipmentTypeId,
                    EquipmentNameId = timesht.EquipmentNameId,
                };
                var companySetting = db.RequestToInspections.FirstOrDefault();
                if (companySetting != null)
                {
                    var plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var lubItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && OilLubItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    newTimeSheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
                    plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var issuedOilItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && oilItems.Contains(ii.ItemCode)).ToList();
                    timesht.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    timesht.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
                    plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.PlateNumber)
                        .FirstOrDefault();
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                                        .Where(x => x.FleetsID == timesht.FleetsID)
                                        .Select(x => x.SerialNo)
                                        .FirstOrDefault();
                    }
                    if (plateNo == null)
                    {
                        plateNo = db.Fleets
                        .Where(x => x.FleetsID == timesht.FleetsID)
                        .Select(x => x.ChasisNumber)
                        .FirstOrDefault();
                    }
                    var issuedSpareItems = db.IssueItems
                     .Where(ii =>
                             ii.Issue.PlateNo == plateNo
                             && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                 == DbFunctions.TruncateTime(date)
                             && allSparePartItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var issuedBatteryItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(date)
                        && allBatteryItems.Contains(ii.ItemCode)).ToList();
                    var issuedTyreItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(date)
                            && allTyreItems.Contains(ii.ItemCode)).ToList();
                    newTimeSheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                    newTimeSheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == timesht.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(date));
                    decimal maintenancePayment = 0;

                    if (jobOrder != null)
                    {
                        string maintenanceCenterId = jobOrder.MaintenanceCenterId.ToString();
                        var maintenanceCenter = db.MaintenanceCenter.FirstOrDefault(x => x.MaintenanceCenterID.ToString() == maintenanceCenterId);
                        var ServiceCenterJobs = db.ServiceCenterJob.Where(x => x.JobOrderID == jobOrder.JobOrderID).ToList();
                        foreach (var ServiceCenterJob in ServiceCenterJobs)
                        {
                            if (maintenanceCenter != null && maintenanceCenter.IsExternal)
                            {
                                var externalServicePayment = db.ExternalServiceCenterPayments.Where(x => x.ServiceCenterJobId == ServiceCenterJob.ServiceCenterJobId).ToList();
                                maintenancePayment += externalServicePayment.Sum(x => (decimal?)x.Total) ?? 0;

                            }
                            else
                            {
                                var StoreRequests = db.StoreRequisitions.Where(x => x.ServiceJobOrderNo == ServiceCenterJob.ServiceCenterJobId.ToString()).Select(x => x.StoreRequisitionID).ToList();
                                if (StoreRequests.Any())
                                {
                                    var IssuedIds = db.Issues.Where(x => StoreRequests.Contains(x.StoreRequisitionID)).Select(x => x.IssueID).ToList();
                                    if (IssuedIds.Any())
                                    {
                                        var IssuedItems = db.IssueItems.Where(x => IssuedIds.Contains(x.IssueID)).ToList();
                                        foreach (var Item in IssuedItems)
                                        {
                                            maintenancePayment += Item.UnitPrice * Item.IssuedQuantity;
                                        }
                                    }

                                }
                            }
                            var crewIds = db.ServiceCrewWorkLogs.Where(x => x.JobId == ServiceCenterJob.ServiceCenterJobId).ToList();
                            foreach (var crew in crewIds)
                            {
                                decimal companyCost = db.RepairType.FirstOrDefault(x => x.RepairTypeID.ToString() == crew.RepiarTypeId)?.TotalAmountCompany ?? 0;
                                maintenancePayment += companyCost;
                            }
                        }

                    }
                    newTimeSheet.MaintenanceCost += maintenancePayment;
                }
                db.WagonDrillTimesheets.Add(newTimeSheet);

            }
            db.SaveChanges();
        }

        #region Approval
        public JsonResult VoidTimeSheet(int lightVehicleTimeSheetId)
        {

            var lightVehicle = db.LightVehicleTimeSheets.Find(lightVehicleTimeSheetId);

            if (lightVehicle != null)
            {
                lightVehicle.TimeSheetSendForAproval = false;
                lightVehicle.TimeSheetApproved = false;
                lightVehicle.TimeSheetApprovedDate = DateTime.Now;
                lightVehicle.TimeSheetApprovedBy = User.Identity.Name;
                lightVehicle.TimeSheetStatus = MaintenanceConst.Rejected;

                db.LightVehicleTimeSheets.Attach(lightVehicle);
                db.Entry(lightVehicle).State = EntityState.Modified;
                db.SaveChanges();
                var returnMessage = new
                {
                    Message = "Successfully Sent!",

                };
                return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);

            }

            var jsonData = new
            {
                Message = "Error Sending!",

            };
            return Json(jsonData.Message, JsonRequestBehavior.AllowGet);

        }
        public JsonResult TimeSheetSendForApproval(int lightVehicleTimeSheetId)
        {

            var lightVehicle = db.LightVehicleTimeSheets.Find(lightVehicleTimeSheetId);

            if (lightVehicle != null)
            {
                lightVehicle.TimeSheetSendForAproval = true;
                lightVehicle.TimeSheetSendForApprovalTime = DateTime.Now;
                lightVehicle.TimeSheetSendForApprovalBy = User.Identity.Name;
                lightVehicle.TimeSheetStatus = MaintenanceConst.WaitingApproval;

                db.LightVehicleTimeSheets.Attach(lightVehicle);
                db.Entry(lightVehicle).State = EntityState.Modified;
                db.SaveChanges();
                var returnMessage = new
                {
                    Message = "Successfully Sent!",

                };
                return Json(returnMessage.Message, JsonRequestBehavior.AllowGet);

            }

            var jsonData = new
            {
                Message = "Error Sending!",

            };
            return Json(jsonData.Message, JsonRequestBehavior.AllowGet);

        }
        public ActionResult LightVehicleTimeSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.TimeSheetApprovalNew.Where(x => x.TimeSheetId == id && x.TimeSheetType == TimeSheetType.LightVehicle);
            DataSourceResult result = approval.ToDataSourceResult(request, model => new TimeSheetApprovalNew
            {
                TimeSheetId = model.TimeSheetId,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                IsApproved = model.IsApproved,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                Remark = model.Remark,
                Status = model.Status,
                TimeSheetType = model.TimeSheetType,
                TimeSheetApprovalNewId = model.TimeSheetApprovalNewId
            });
            return Json(result);
        }
        [AuthorizeRoles(FleetManagementRoles.FleetTimeSheetApprover)]
        public ActionResult LightVehicleTimeSheetApproval_Create([DataSourceRequest] DataSourceRequest request, TimeSheetApprovalNew model, int id)
        {
            if (ModelState.IsValid)
            {
                if (model.Status == MaintenanceConst.Approved)
                {
                    Approved(id);
                    model.IsApproved = true;

                }
                else
                {
                    Rejected(id);
                    model.IsApproved = false;
                }
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.TimeSheetId = id;
                model.TimeSheetType = TimeSheetType.LightVehicle;
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var lightVehicle = db.LightVehicleTimeSheets.Find(id);
            if (lightVehicle != null)
            {
                lightVehicle.TimeSheetApproved = true;
                lightVehicle.TimeSheetStatus = MaintenanceConst.Approved;
                lightVehicle.TimeSheetApprovedBy = User.Identity.Name;
                lightVehicle.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(lightVehicle).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var lightVehicle = db.LightVehicleTimeSheets.Find(id);
            if (lightVehicle != null)
            {
                lightVehicle.TimeSheetApproved = false;
                lightVehicle.TimeSheetStatus = MaintenanceConst.Rejected;
                lightVehicle.TimeSheetApprovedBy = User.Identity.Name;
                lightVehicle.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(lightVehicle).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        #endregion



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}