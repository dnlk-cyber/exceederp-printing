using System.Data.Entity;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using ExceedERP.Web.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using ExceedERP.Helpers.Common;  
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.Global;

using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    [AuthorizeRoles(FleetManagementRoles.FleetTimeSheetUser, FleetManagementRoles.FleetTimeSheetApprover)]
    public class DumpTruckTimeSheetController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        ExceedDbContext db = new ExceedDbContext();
       
public ActionResult Index(TimeSheetType dumpTruck)
        {

            var fetchSetting = db.RequestToInspections.FirstOrDefault();
            string isECWC = (fetchSetting != null && fetchSetting.Remark == "ECWC") ? "ECWC" : "Not";
            ViewBag.TimesheetType = TimeSheetType.DumpTruck;
            ViewData["IsECWC"] = isECWC;
            var fetchSettingg = db.RequestToInspections.FirstOrDefault();
            ViewBag.setting = (fetchSetting != null) ? fetchSetting.Remark : "";
            string isECWCC = "Not";
            if (fetchSettingg != null)
            {

                isECWCC = fetchSettingg.Remark;
            }
            ViewBag.IsMIDROC = isECWCC;
            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(
     i => new
     {
         Text = i.Name,
         Value = i.Id
     }).AsQueryable();
            ViewBag.setting = db.RequestToInspections
                  .FirstOrDefault()?.Remark ?? " ";
            ViewData["type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["RepairTypes"] = db.RepairType.Select(s => new
            {
                Code = s.RepairTypeID,
                Name = s.Name
            }).AsQueryable();
            ViewData["name"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["branchh"] = db.Branch
     .Select(x => new SelectListItem
     {
         Text = x.Name,
         Value = x.BranchId.ToString()
     }).ToList();
            ViewData["Fleets"] = CacheData.GetFleetsSecondCache().Select(x => new
            {
                Value = x.FleetsID,
                Text = x.PlateNumber
            });
          
            ViewData["operator"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.FullName
            });
            ViewData["branch"] = CacheData.GetBranchCaches().Select(
       i => new
       {
           Name = i.Name,
           BranchId = i.Id
       }).AsQueryable();
            ViewData["businessunit"] = CacheData.GetBusinessUnitCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();

            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name });

            ViewBag.dumpTruck = dumpTruck;
            
            return View();
        }


        public JsonResult RepopulateTimeSheet()
        {
            var timesheets = db.DumpTruckTimeSheets.ToList();
            if (timesheets.Any())
            {
                foreach (var item in timesheets)
                {
                    item.EquipmentCategoryId = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID)?.EquipmentIdentityID;
                    item.EquipmentTypeId = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID)?.EquipmentTypeID;
                    item.EquipmentNameId = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID)?.EquipmentNameID;
                    db.Entry(item).State = EntityState.Modified;
                }
                db.SaveChanges();
            }
            return Json("success", JsonRequestBehavior.AllowGet);
        }
        public ActionResult DumpTruckTimeSheets_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var dumpTruckTimeSheets = db.DumpTruckTimeSheets.Where(x => mybraches.Contains(x.BranchId));
            
            DataSourceResult result = dumpTruckTimeSheets.ToDataSourceResult(request, dumpTruckTimeSheet => new DumpTruckTimeSheet
            {
                DumpTruckTimeSheetId = dumpTruckTimeSheet.DumpTruckTimeSheetId,
                FleetsID = dumpTruckTimeSheet.FleetsID,
                ProcurementSupplierID = dumpTruckTimeSheet.ProcurementSupplierID,
                Date = dumpTruckTimeSheet.Date,
                TypeOfWorkExcuted=dumpTruckTimeSheet.TypeOfWorkExcuted,
                LoadingLocationFrom=dumpTruckTimeSheet.LoadingLocationFrom,
                LoadingLOactionTo=dumpTruckTimeSheet.LoadingLOactionTo,
                Capacity=dumpTruckTimeSheet.Capacity,
                ActualDistance=dumpTruckTimeSheet.ActualDistance,
                NumberOfTrip=dumpTruckTimeSheet.NumberOfTrip,
                TotalHaulage = dumpTruckTimeSheet.TotalHaulage,
                FuelConsumption=dumpTruckTimeSheet.FuelConsumption,
                IdleHour= dumpTruckTimeSheet.IdleHour,
                MorningTimeStart = dumpTruckTimeSheet.MorningTimeStart,
                MorningTimeEnd = dumpTruckTimeSheet.MorningTimeEnd,
                AfternoonTimeStart = dumpTruckTimeSheet.AfternoonTimeStart,
                AfternoonTimeEnd = dumpTruckTimeSheet.AfternoonTimeEnd,
                NightTimeStart = dumpTruckTimeSheet.NightTimeStart,
                NightTimeEnd = dumpTruckTimeSheet.NightTimeEnd,
                DifferenceKMR = dumpTruckTimeSheet.DifferenceKMR,
                FinalKMR = dumpTruckTimeSheet.FinalKMR,
                InitialKMR = dumpTruckTimeSheet.InitialKMR,
                TotalTime = dumpTruckTimeSheet.TotalTime,
                FuelKMR = dumpTruckTimeSheet.FuelKMR,

                SparePartCost = dumpTruckTimeSheet.SparePartCost,
                OilCost = dumpTruckTimeSheet.OilCost,
                GreaseConsumption = dumpTruckTimeSheet.GreaseConsumption,
                GreaseCost = dumpTruckTimeSheet.GreaseCost,
                MaintenanceCost = dumpTruckTimeSheet.MaintenanceCost,
                OilConsumption = dumpTruckTimeSheet.OilConsumption,

                ReasonForIdle =dumpTruckTimeSheet.ReasonForIdle,
                DownHour = dumpTruckTimeSheet.DownHour,
                ReasonForDown = dumpTruckTimeSheet.ReasonForDown,
                DateCreated = dumpTruckTimeSheet.DateCreated,
                CreatedBy = dumpTruckTimeSheet.CreatedBy,
                DownReason = dumpTruckTimeSheet.DownReason,
                IdleReason = dumpTruckTimeSheet.IdleReason,
                FuelType = dumpTruckTimeSheet.FuelType,
                Unit = dumpTruckTimeSheet.Unit,
                Volume = dumpTruckTimeSheet.Volume,
                OilType = dumpTruckTimeSheet.OilType,
                ModifiedBy = dumpTruckTimeSheet.ModifiedBy,
                LastModified = dumpTruckTimeSheet.LastModified,
                RentalRate=dumpTruckTimeSheet.RentalRate,
                IdealRentalRate=dumpTruckTimeSheet.IdealRentalRate,
                FuelRate = dumpTruckTimeSheet.FuelRate,
                NetworkingHour = dumpTruckTimeSheet.NetworkingHour,
                Perdium = dumpTruckTimeSheet.Perdium,
                Salary = dumpTruckTimeSheet.Salary,
                ExternalOperatorName = dumpTruckTimeSheet.ExternalOperatorName,
                TireCost = dumpTruckTimeSheet.TireCost,
                LoadingStatus = dumpTruckTimeSheet.LoadingStatus,
                BranchId = dumpTruckTimeSheet.BranchId,
                BusinessUnitId = dumpTruckTimeSheet.BusinessUnitId,
                CostSheetApproved = dumpTruckTimeSheet.CostSheetApproved,
                CostSheetApprovedBy = dumpTruckTimeSheet.CostSheetApprovedBy,
                CostSheetApprovedDate = dumpTruckTimeSheet.CostSheetApprovedDate,
                CostSheetSendForAproval = dumpTruckTimeSheet.CostSheetSendForAproval,
                TimeSheetStatus = dumpTruckTimeSheet.TimeSheetStatus,
                TimeSheetApproved = dumpTruckTimeSheet.TimeSheetApproved,
                TimeSheetApprovedBy = dumpTruckTimeSheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = dumpTruckTimeSheet.TimeSheetApprovedDate,
                TimeSheetSendForAproval = dumpTruckTimeSheet.TimeSheetSendForAproval,
                OperatorId = dumpTruckTimeSheet.OperatorId,
                EquipmentCategoryId = dumpTruckTimeSheet.EquipmentCategoryId,
                EquipmentTypeId = dumpTruckTimeSheet.EquipmentTypeId,
                EquipmentNameId = dumpTruckTimeSheet.EquipmentNameId

            });

            return Json(result);
        }
        public ActionResult DumpTruckTimeSheets_Create([DataSourceRequest] DataSourceRequest request, DumpTruckTimeSheet dumpTruckTimeSheet)
        {
            dumpTruckTimeSheet.TotalTime = 0;
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == dumpTruckTimeSheet.FleetsID);
            dumpTruckTimeSheet.EquipmentCategoryId = fleet.EquipmentIdentityID;
            dumpTruckTimeSheet.EquipmentTypeId = fleet.EquipmentTypeID;
            dumpTruckTimeSheet.EquipmentNameId = fleet.EquipmentNameID;
            var fetchSettings = db.TimeSheetSetting.ToList();
            if (fetchSettings.Any())
            {
                var fetchSetting = fetchSettings.First();
                var fetchException = db.TimeSheetException.FirstOrDefault(x => x.TimeSheetSettingId == fetchSetting.TimeSheetSettingId && x.BranchId == dumpTruckTimeSheet.BranchId.ToString());
                if (fetchException == null)
                {
                    if (fetchSetting != null && fetchSetting.IsSet == true)
                    {
                        int hoursDifference = (int)(DateTime.Now - dumpTruckTimeSheet.Date).TotalHours;

                        if (fetchSetting.SettedHours < hoursDifference)
                        {
                            ModelState.AddModelError("date expired", "The timesheet recording date is passed");
                        }
                    }
                }
            }
            var companySetting = db.RequestToInspections.FirstOrDefault();
            if (companySetting != null)
            {
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
                var plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.PlateNumber)
                    .FirstOrDefault();
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                                    .Where(x => x.FleetsID == fleet.FleetsID)
                                    .Select(x => x.SerialNo)
                                    .FirstOrDefault();
                }
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.ChasisNumber)
                    .FirstOrDefault();
                }
                var lubItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date)
                        && OilLubItems.Contains(ii.ItemCode)).ToList();
                dumpTruckTimeSheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                dumpTruckTimeSheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
                plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.PlateNumber)
                    .FirstOrDefault();
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                                    .Where(x => x.FleetsID == fleet.FleetsID)
                                    .Select(x => x.SerialNo)
                                    .FirstOrDefault();
                }
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.ChasisNumber)
                    .FirstOrDefault();
                }
                var issuedOilItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date)
                        && oilItems.Contains(ii.ItemCode)).ToList();
                dumpTruckTimeSheet.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                dumpTruckTimeSheet.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
               plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.PlateNumber)
                    .FirstOrDefault();
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                                    .Where(x => x.FleetsID == fleet.FleetsID)
                                    .Select(x => x.SerialNo)
                                    .FirstOrDefault();
                }
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.ChasisNumber)
                    .FirstOrDefault();
                }
                var issuedSpareItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date)
                        && allSparePartItems.Contains(ii.ItemCode)).ToList();
                var issuedBatteryItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date)
                        && allBatteryItems.Contains(ii.ItemCode)).ToList();
                var issuedTyreItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date)
                        && allTyreItems.Contains(ii.ItemCode)).ToList();
                dumpTruckTimeSheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                dumpTruckTimeSheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                dumpTruckTimeSheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == fleet.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date));
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
                dumpTruckTimeSheet.MaintenanceCost += maintenancePayment;
            }

            var timeSheets = db.DumpTruckTimeSheets.FirstOrDefault(x => x.FleetsID == dumpTruckTimeSheet.FleetsID && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date));
             if(timeSheets!=null)
            {
                ModelState.AddModelError("duplicate", "Timesheet for this Plate On the selected day is already added");
            }
            var setting = db.RequestToInspections.FirstOrDefault();
            var lastTimesheet = db.LightVehicleTimeSheets.Where(x => x.FleetsID == dumpTruckTimeSheet.FleetsID).OrderByDescending(x => x.LightVehicleTimeSheetId).FirstOrDefault();

            if (setting != null && setting.Remark == "MCG")
            {
                if (dumpTruckTimeSheet.InitialKMR < 0 || dumpTruckTimeSheet.FinalKMR < 0 || (dumpTruckTimeSheet.FinalKMR - dumpTruckTimeSheet.InitialKMR) < 0 || dumpTruckTimeSheet.FuelKMR < 0)
                {
                    ModelState.AddModelError("invalid", "Invalid KMR entry");
                }

                dumpTruckTimeSheet.ReasonForDown = dumpTruckTimeSheet.DownReason.ToString();
                dumpTruckTimeSheet.ReasonForIdle = dumpTruckTimeSheet.IdleReason.ToString();



               if (dumpTruckTimeSheet.MorningTimeStart==null||dumpTruckTimeSheet.MorningTimeEnd==null)
                {
                    dumpTruckTimeSheet.MorningTimeStart = DateTime.Now;
                    dumpTruckTimeSheet.MorningTimeEnd = DateTime.Now;
                }if(dumpTruckTimeSheet.AfternoonTimeStart==null||dumpTruckTimeSheet.AfternoonTimeEnd==null)
                {
                    dumpTruckTimeSheet.AfternoonTimeStart = DateTime.Now;
                    dumpTruckTimeSheet.AfternoonTimeEnd = DateTime.Now;
                }if(dumpTruckTimeSheet.NightTimeStart==null|| dumpTruckTimeSheet.NightTimeEnd==null)
                {
                    dumpTruckTimeSheet.NightTimeStart = DateTime.Now;
                    dumpTruckTimeSheet.NightTimeEnd = DateTime.Now;
                }
                dumpTruckTimeSheet.NetworkingHour =
                    (decimal)GetTotalTime(
                        dumpTruckTimeSheet.MorningTimeStart.Value,
                        dumpTruckTimeSheet.MorningTimeEnd.Value,
                        dumpTruckTimeSheet.AfternoonTimeStart.Value,
                        dumpTruckTimeSheet.AfternoonTimeEnd.Value,
                        dumpTruckTimeSheet.NightTimeStart.Value,
                        dumpTruckTimeSheet.NightTimeEnd.Value
                    ).TotalHours;
                if(dumpTruckTimeSheet.MorningTimeEnd == dumpTruckTimeSheet.MorningTimeStart)
                    {
                        dumpTruckTimeSheet.MorningTimeStart = null;
                        dumpTruckTimeSheet.MorningTimeEnd = null;
                    }
                if (dumpTruckTimeSheet.AfternoonTimeStart == dumpTruckTimeSheet.AfternoonTimeEnd)
                {
                    dumpTruckTimeSheet.AfternoonTimeEnd = null;
                    dumpTruckTimeSheet.AfternoonTimeStart = null;
                }
                if (dumpTruckTimeSheet.NightTimeEnd == dumpTruckTimeSheet.NightTimeStart)
                {
                    dumpTruckTimeSheet.NightTimeStart = null;
                    dumpTruckTimeSheet.NightTimeEnd = null;
                }
                
                    if (dumpTruckTimeSheet.InitialKMR < 0 || dumpTruckTimeSheet.FinalKMR < 0 || (dumpTruckTimeSheet.FinalKMR - dumpTruckTimeSheet.InitialKMR) < 0 || dumpTruckTimeSheet.FuelKMR < 0)
                    {
                        ModelState.AddModelError("invalid", "Invalid KMR entry");
                    }


                dumpTruckTimeSheet.TotalTime = dumpTruckTimeSheet.NetworkingHour + dumpTruckTimeSheet.IdealRentalRate * dumpTruckTimeSheet.IdleHour * 0.01m;
                dumpTruckTimeSheet.DifferenceKMR = dumpTruckTimeSheet.FinalKMR - dumpTruckTimeSheet.InitialKMR;
            }
            fleet.BranchId = dumpTruckTimeSheet.BranchId;
            db.Entry(fleet).State = EntityState.Modified;
            db.SaveChanges();
            
            if (ModelState.IsValid)
            {

                var entity = new DumpTruckTimeSheet
                {
                    DumpTruckTimeSheetId = dumpTruckTimeSheet.DumpTruckTimeSheetId,
                    FleetsID = dumpTruckTimeSheet.FleetsID,
                    ProcurementSupplierID = dumpTruckTimeSheet.ProcurementSupplierID,
                    Date = dumpTruckTimeSheet.Date,
                    Unit = fleet.Unit,
                    Volume = fleet.Volume,
                    RentalAgreementNum = fleet.RentFromExternalNum,
                    TotalTime = dumpTruckTimeSheet.TotalTime,
                    TypeOfWorkExcuted = dumpTruckTimeSheet.TypeOfWorkExcuted,
                    IdleReason = dumpTruckTimeSheet.IdleReason,
                    DownReason = dumpTruckTimeSheet.DownReason,
                    LoadingLocationFrom = dumpTruckTimeSheet.LoadingLocationFrom,
                    LoadingLOactionTo = dumpTruckTimeSheet.LoadingLOactionTo,
                    Capacity = dumpTruckTimeSheet.Capacity,
                    ActualDistance = dumpTruckTimeSheet.ActualDistance,
                    NumberOfTrip = dumpTruckTimeSheet.NumberOfTrip,
                    TotalHaulage = dumpTruckTimeSheet.TotalHaulage,
                    FuelConsumption = dumpTruckTimeSheet.FuelConsumption,
                    IdleHour = dumpTruckTimeSheet.IdleHour,
                    ReasonForIdle = dumpTruckTimeSheet.ReasonForIdle,
                    DownHour = dumpTruckTimeSheet.DownHour,
                    LoadingStatus = dumpTruckTimeSheet.LoadingStatus,
                    ReasonForDown = dumpTruckTimeSheet.ReasonForDown,
                    FuelType = dumpTruckTimeSheet.FuelType,
                    OilType = dumpTruckTimeSheet.OilType,
                    DateCreated = DateTime.Now,
                    ExternalOperatorName = dumpTruckTimeSheet.ExternalOperatorName,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = dumpTruckTimeSheet.ModifiedBy,
                    LastModified = dumpTruckTimeSheet.LastModified,
                    RentalRate = dumpTruckTimeSheet.RentalRate,
                    IdealRentalRate = dumpTruckTimeSheet.IdealRentalRate,
                    // added

                    SparePartCost = dumpTruckTimeSheet.SparePartCost,
                    OilCost = dumpTruckTimeSheet.OilCost,
                    GreaseConsumption = dumpTruckTimeSheet.GreaseConsumption,
                    GreaseCost = dumpTruckTimeSheet.GreaseCost,
                    MaintenanceCost = dumpTruckTimeSheet.MaintenanceCost,
                    OilConsumption = dumpTruckTimeSheet.OilConsumption,

                    MorningTimeStart = dumpTruckTimeSheet.MorningTimeStart,
                    MorningTimeEnd = dumpTruckTimeSheet.MorningTimeEnd,
                    AfternoonTimeStart = dumpTruckTimeSheet.AfternoonTimeStart,
                    AfternoonTimeEnd = dumpTruckTimeSheet.AfternoonTimeEnd,
                    NightTimeStart = dumpTruckTimeSheet.NightTimeStart,
                    NightTimeEnd = dumpTruckTimeSheet.NightTimeEnd,
                    InitialKMR = dumpTruckTimeSheet.InitialKMR,
                    FinalKMR = dumpTruckTimeSheet.FinalKMR,
                    DifferenceKMR = dumpTruckTimeSheet.DifferenceKMR,
                    FuelKMR = dumpTruckTimeSheet.FuelKMR,
                    FuelRate = dumpTruckTimeSheet.FuelRate,
                    NetworkingHour = dumpTruckTimeSheet.NetworkingHour,
                    Perdium = dumpTruckTimeSheet.Perdium,
                    Salary = dumpTruckTimeSheet.Salary,
                    TireCost = dumpTruckTimeSheet.TireCost,
                    BranchId = dumpTruckTimeSheet.BranchId,
                    BusinessUnitId = dumpTruckTimeSheet.BusinessUnitId,
                    OperatorId = dumpTruckTimeSheet.OperatorId,
                    EquipmentCategoryId = dumpTruckTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = dumpTruckTimeSheet.EquipmentTypeId,
                    EquipmentNameId = dumpTruckTimeSheet.EquipmentNameId
                };

                db.DumpTruckTimeSheets.Add(entity);
                db.SaveChanges();
                //var fleetSetting = db.RequestToInspections.FirstOrDefault(x => x.Remark != null);

                //if (fleetSetting != null && fleetSetting.Remark == "MIDROC")
                //{
                //    List<DumpTruckTimeSheet> timeSheet = db.DumpTruckTimeSheets
                //        .Where(x => x.BranchId == entity.BranchId &&
                //                    x.EquipmentCategoryId == entity.EquipmentCategoryId && x.FleetsID != entity.FleetsID)
                //        .GroupBy(x => x.FleetsID)
                //        .Select(g => g.OrderByDescending(x => x.DumpTruckTimeSheetId).FirstOrDefault())
                //        .ToList();
                //    var firstObject = timeSheet.First();
                //    var checkDuplicatee = db.DumpTruckTimeSheets.FirstOrDefault(x => x.FleetsID == firstObject.FleetsID && x.Date == firstObject.Date);
                //    if (checkDuplicatee == null)
                //    {
                //        foreach (var timesht in timeSheet)
                //        {
                //            var newTimeSheet = new DumpTruckTimeSheet
                //            {
                //                DumpTruckTimeSheetId = timesht.DumpTruckTimeSheetId,
                //                FleetsID = timesht.FleetsID,
                //                ProcurementSupplierID = timesht.ProcurementSupplierID,
                //                Date = entity.Date,
                //                TypeOfWorkExcuted = timesht.TypeOfWorkExcuted,
                //                LoadingLocationFrom = timesht.LoadingLocationFrom,
                //                LoadingLOactionTo = timesht.LoadingLOactionTo,
                //                Capacity = timesht.Capacity,
                //                ActualDistance = timesht.ActualDistance,
                //                NumberOfTrip = timesht.NumberOfTrip,
                //                TotalHaulage = timesht.TotalHaulage,
                //                FuelConsumption = timesht.FuelConsumption,
                //                OilConsumption = timesht.OilConsumption,
                //                GreaseConsumption = timesht.GreaseConsumption,
                //                IdleHour = timesht.IdleHour,
                //                ReasonForIdle = timesht.ReasonForIdle,
                //                DownHour = timesht.DownHour,
                //                LoadingStatus = timesht.LoadingStatus,
                //                ReasonForDown = timesht.ReasonForDown,
                //                FuelType = timesht.FuelType,
                //                OilType = timesht.OilType,
                //                DateCreated = DateTime.Now,
                //                CreatedBy = User.Identity.Name,
                //                RentalRate = timesht.RentalRate,
                //                IdealRentalRate = timesht.IdealRentalRate,
                //                // added
                //                LubricantCost = timesht.LubricantCost,
                //                FuelRate = timesht.FuelRate,
                //                MaintenanceCost = timesht.MaintenanceCost,
                //                NetworkingHour = timesht.NetworkingHour,
                //                Perdium = timesht.Perdium,
                //                Salary = timesht.Salary,
                //                TireCost = timesht.TireCost,
                //                BranchId = timesht.BranchId,
                //                BusinessUnitId = timesht.BusinessUnitId,
                //                OperatorId = timesht.OperatorId,
                //                EquipmentCategoryId = timesht.EquipmentCategoryId,
                //                EquipmentTypeId = timesht.EquipmentTypeId,
                //                EquipmentNameId = timesht.EquipmentNameId
                //            };

                //            db.DumpTruckTimeSheets.Add(newTimeSheet);
                //            db.SaveChanges();
                //        }
                //    }
                //}
                dumpTruckTimeSheet.DumpTruckTimeSheetId = entity.DumpTruckTimeSheetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dump truck timesheet registration", "Success", User.Identity.Name, $"created dump truck timesheet {entity.DumpTruckTimeSheetId} ", Modules.FleetManagement);
                #endregion

            }

            return Json(new[] { dumpTruckTimeSheet }.ToDataSourceResult(request, ModelState));
        }
        private TimeSpan GetTotalTime(
     DateTime? a, DateTime? b,
     DateTime? c, DateTime? d,
     DateTime? e, DateTime? f)
        {
            TimeSpan total = TimeSpan.Zero;

            if (a.HasValue && b.HasValue)
                total += (b.Value - a.Value);

            if (c.HasValue && d.HasValue)
                total += (d.Value - c.Value);

            if (e.HasValue && f.HasValue)
                total += (f.Value - e.Value);

            return total;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DumpTruckTimeSheets_Update([DataSourceRequest] DataSourceRequest request, DumpTruckTimeSheet dumpTruckTimeSheet)
        {
            dumpTruckTimeSheet.TotalTime = 0;
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == dumpTruckTimeSheet.FleetsID);
            dumpTruckTimeSheet.EquipmentCategoryId = fleet.EquipmentIdentityID;
            dumpTruckTimeSheet.EquipmentTypeId = fleet.EquipmentTypeID;
            dumpTruckTimeSheet.EquipmentNameId = fleet.EquipmentNameID;
            var companySetting = db.RequestToInspections.FirstOrDefault();
            if (companySetting != null)
            {
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
                var plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.PlateNumber)
                    .FirstOrDefault();
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                                    .Where(x => x.FleetsID == fleet.FleetsID)
                                    .Select(x => x.SerialNo)
                                    .FirstOrDefault();
                }
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.ChasisNumber)
                    .FirstOrDefault();
                }
                var lubItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date)
                        && OilLubItems.Contains(ii.ItemCode)).ToList();
                dumpTruckTimeSheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                dumpTruckTimeSheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
                plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.PlateNumber)
                    .FirstOrDefault();
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                                    .Where(x => x.FleetsID == fleet.FleetsID)
                                    .Select(x => x.SerialNo)
                                    .FirstOrDefault();
                }
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.ChasisNumber)
                    .FirstOrDefault();
                }
                var issuedOilItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date)
                        && oilItems.Contains(ii.ItemCode)).ToList();
                dumpTruckTimeSheet.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                dumpTruckTimeSheet.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
                plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.PlateNumber)
                    .FirstOrDefault();
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                                    .Where(x => x.FleetsID == fleet.FleetsID)
                                    .Select(x => x.SerialNo)
                                    .FirstOrDefault();
                }
                if (plateNo == null)
                {
                    plateNo = db.Fleets
                    .Where(x => x.FleetsID == fleet.FleetsID)
                    .Select(x => x.ChasisNumber)
                    .FirstOrDefault();
                }
                var issuedSpareItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date)
                        && allSparePartItems.Contains(ii.ItemCode)).ToList();
                var issuedBatteryItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date)
                        && allBatteryItems.Contains(ii.ItemCode)).ToList();
                var issuedTyreItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date)
                        && allTyreItems.Contains(ii.ItemCode)).ToList();
                dumpTruckTimeSheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                dumpTruckTimeSheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                dumpTruckTimeSheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == fleet.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date));
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
                dumpTruckTimeSheet.MaintenanceCost += maintenancePayment;
            }
            var timeSheets = db.DumpTruckTimeSheets.FirstOrDefault(x => x.FleetsID == dumpTruckTimeSheet.FleetsID&&x.DumpTruckTimeSheetId!=dumpTruckTimeSheet.DumpTruckTimeSheetId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(dumpTruckTimeSheet.Date));
            if (timeSheets != null)
            {
                ModelState.AddModelError("duplicate", "Timesheet for this Plate On the selected day is already added");
            }
            var setting = db.RequestToInspections.FirstOrDefault();
            var lastTimesheet = db.LightVehicleTimeSheets.Where(x => x.FleetsID == dumpTruckTimeSheet.FleetsID).OrderByDescending(x => x.LightVehicleTimeSheetId).FirstOrDefault();

            if (setting != null && setting.Remark == "MCG")
            {
                if (dumpTruckTimeSheet.InitialKMR <0 || dumpTruckTimeSheet.FinalKMR < 0 || (dumpTruckTimeSheet.FinalKMR - dumpTruckTimeSheet.InitialKMR) < 0 || dumpTruckTimeSheet.FuelKMR < 0)
                {
                    ModelState.AddModelError("invalid", "Invalid KMR entry");
                }

                dumpTruckTimeSheet.ReasonForDown = dumpTruckTimeSheet.DownReason.ToString();
                dumpTruckTimeSheet.ReasonForIdle = dumpTruckTimeSheet.IdleReason.ToString();

                if (dumpTruckTimeSheet.MorningTimeStart== null || dumpTruckTimeSheet.MorningTimeEnd == null)
                {
                    dumpTruckTimeSheet.MorningTimeStart = DateTime.Now;
                    dumpTruckTimeSheet.MorningTimeEnd = DateTime.Now;
                }
                if (dumpTruckTimeSheet.AfternoonTimeStart == null || dumpTruckTimeSheet.AfternoonTimeEnd == null)
                {
                    dumpTruckTimeSheet.AfternoonTimeStart = DateTime.Now;
                    dumpTruckTimeSheet.AfternoonTimeEnd = DateTime.Now;
                }
                if (dumpTruckTimeSheet.NightTimeStart == null || dumpTruckTimeSheet.NightTimeEnd == null)
                {
                    dumpTruckTimeSheet.NightTimeStart = DateTime.Now;
                    dumpTruckTimeSheet.NightTimeEnd = DateTime.Now;
                }
                dumpTruckTimeSheet.NetworkingHour =
                    (decimal)GetTotalTime(
                        dumpTruckTimeSheet.MorningTimeStart.Value,
                        dumpTruckTimeSheet.MorningTimeEnd.Value,
                        dumpTruckTimeSheet.AfternoonTimeStart.Value,
                        dumpTruckTimeSheet.AfternoonTimeEnd.Value,
                        dumpTruckTimeSheet.NightTimeStart.Value,
                        dumpTruckTimeSheet.NightTimeEnd.Value
                    ).TotalHours;
                if (dumpTruckTimeSheet.MorningTimeEnd == dumpTruckTimeSheet.MorningTimeStart)
                {
                    dumpTruckTimeSheet.MorningTimeStart = null;
                    dumpTruckTimeSheet.MorningTimeEnd = null;
                }
                if (dumpTruckTimeSheet.AfternoonTimeStart == dumpTruckTimeSheet.AfternoonTimeEnd)
                {
                    dumpTruckTimeSheet.AfternoonTimeEnd = null;
                    dumpTruckTimeSheet.AfternoonTimeStart = null;
                }
                if (dumpTruckTimeSheet.NightTimeEnd == dumpTruckTimeSheet.NightTimeStart)
                {
                    dumpTruckTimeSheet.NightTimeStart = null;
                    dumpTruckTimeSheet.NightTimeEnd = null;
                }
                dumpTruckTimeSheet.TotalTime = dumpTruckTimeSheet.NetworkingHour + dumpTruckTimeSheet.IdealRentalRate * dumpTruckTimeSheet.IdleHour * 0.01m;
                dumpTruckTimeSheet.DifferenceKMR = dumpTruckTimeSheet.FinalKMR - dumpTruckTimeSheet.InitialKMR;
            }
            if (ModelState.IsValid)
            {
                var entity = new DumpTruckTimeSheet
                {
                    DumpTruckTimeSheetId = dumpTruckTimeSheet.DumpTruckTimeSheetId,
                    FleetsID = dumpTruckTimeSheet.FleetsID,
                    ProcurementSupplierID = dumpTruckTimeSheet.ProcurementSupplierID,
                    Date = dumpTruckTimeSheet.Date,
                    TypeOfWorkExcuted = dumpTruckTimeSheet.TypeOfWorkExcuted,
                    IdleReason = dumpTruckTimeSheet.IdleReason,
                    DownReason = dumpTruckTimeSheet.DownReason,
                    LoadingLocationFrom = dumpTruckTimeSheet.LoadingLocationFrom,
                    LoadingLOactionTo = dumpTruckTimeSheet.LoadingLOactionTo,
                    Capacity = dumpTruckTimeSheet.Capacity,
                    ActualDistance = dumpTruckTimeSheet.ActualDistance,
                    NumberOfTrip = dumpTruckTimeSheet.NumberOfTrip,
                    TotalHaulage = dumpTruckTimeSheet.TotalHaulage,
                    FuelConsumption = dumpTruckTimeSheet.FuelConsumption,
                    IdleHour = dumpTruckTimeSheet.IdleHour,
                    ReasonForIdle = dumpTruckTimeSheet.ReasonForIdle,
                    DownHour = dumpTruckTimeSheet.DownHour,
                    ReasonForDown = dumpTruckTimeSheet.ReasonForDown,
                    DateCreated = dumpTruckTimeSheet.DateCreated,
                    CreatedBy = dumpTruckTimeSheet.CreatedBy,
                    ExternalOperatorName = dumpTruckTimeSheet.ExternalOperatorName,
                    ModifiedBy = User.Identity.Name,
                    FuelType = dumpTruckTimeSheet.FuelType,
                    BatteryCost = dumpTruckTimeSheet.BatteryCost,

                    SparePartCost = dumpTruckTimeSheet.SparePartCost,
                    OilCost = dumpTruckTimeSheet.OilCost,
                    GreaseConsumption = dumpTruckTimeSheet.GreaseConsumption,
                    GreaseCost = dumpTruckTimeSheet.GreaseCost,
                    MaintenanceCost = dumpTruckTimeSheet.MaintenanceCost,
                    OilConsumption = dumpTruckTimeSheet.OilConsumption,

                    OilType = dumpTruckTimeSheet.OilType,
                    LastModified = DateTime.Now,
                    RentalRate = dumpTruckTimeSheet.RentalRate,
                    IdealRentalRate = dumpTruckTimeSheet.IdealRentalRate,
                    MorningTimeStart = dumpTruckTimeSheet.MorningTimeStart,
                    MorningTimeEnd = dumpTruckTimeSheet.MorningTimeEnd,
                    AfternoonTimeStart = dumpTruckTimeSheet.AfternoonTimeStart,
                    AfternoonTimeEnd = dumpTruckTimeSheet.AfternoonTimeEnd,
                    NightTimeStart = dumpTruckTimeSheet.NightTimeStart,
                    NightTimeEnd = dumpTruckTimeSheet.NightTimeEnd,
                    InitialKMR = dumpTruckTimeSheet.InitialKMR,
                    FinalKMR = dumpTruckTimeSheet.FinalKMR,
                    DifferenceKMR = dumpTruckTimeSheet.DifferenceKMR,
                    FuelKMR = dumpTruckTimeSheet.FuelKMR,
                    RentalAgreementNum = fleet.RentFromExternalNum,
                    FuelRate = dumpTruckTimeSheet.FuelRate,
                    NetworkingHour = dumpTruckTimeSheet.NetworkingHour,
                    Perdium = dumpTruckTimeSheet.Perdium,
                    LoadingStatus = dumpTruckTimeSheet.LoadingStatus,
                    Salary = dumpTruckTimeSheet.Salary,
                    TireCost = dumpTruckTimeSheet.TireCost,
                    BranchId = dumpTruckTimeSheet.BranchId,
                    BusinessUnitId = dumpTruckTimeSheet.BusinessUnitId,
                    OperatorId = dumpTruckTimeSheet.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    EquipmentCategoryId = dumpTruckTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = dumpTruckTimeSheet.EquipmentTypeId,
                    EquipmentNameId = dumpTruckTimeSheet.EquipmentNameId
                };

                db.DumpTruckTimeSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dump truck timesheet update", "Success", User.Identity.Name, $"updated dump truck timesheet {entity.DumpTruckTimeSheetId} ", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { dumpTruckTimeSheet }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult DumpTruckTimeSheets_Destroy([DataSourceRequest] DataSourceRequest request, DumpTruckTimeSheet dumpTruckTimeSheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new DumpTruckTimeSheet
                {
                    DumpTruckTimeSheetId = dumpTruckTimeSheet.DumpTruckTimeSheetId,
                    FleetsID = dumpTruckTimeSheet.FleetsID,
                    ProcurementSupplierID = dumpTruckTimeSheet.ProcurementSupplierID,
                    Date = dumpTruckTimeSheet.Date,
                    TypeOfWorkExcuted = dumpTruckTimeSheet.TypeOfWorkExcuted,
                    LoadingLocationFrom = dumpTruckTimeSheet.LoadingLocationFrom,
                    LoadingLOactionTo = dumpTruckTimeSheet.LoadingLOactionTo,
                    Capacity = dumpTruckTimeSheet.Capacity,
                    ActualDistance = dumpTruckTimeSheet.ActualDistance,
                    NumberOfTrip = dumpTruckTimeSheet.NumberOfTrip,
                    TotalHaulage = dumpTruckTimeSheet.TotalHaulage,
                    FuelConsumption = dumpTruckTimeSheet.FuelConsumption,
                    OilConsumption = dumpTruckTimeSheet.OilConsumption,
                    GreaseConsumption = dumpTruckTimeSheet.GreaseConsumption,
                    IdleHour = dumpTruckTimeSheet.IdleHour,
                    ReasonForIdle = dumpTruckTimeSheet.ReasonForIdle,
                    DownHour = dumpTruckTimeSheet.DownHour,
                    ReasonForDown = dumpTruckTimeSheet.ReasonForDown,
                    DateCreated = dumpTruckTimeSheet.DateCreated,
                    CreatedBy = dumpTruckTimeSheet.CreatedBy,
                    ModifiedBy = dumpTruckTimeSheet.ModifiedBy,
                    LastModified = dumpTruckTimeSheet.LastModified,
                    RentalRate = dumpTruckTimeSheet.RentalRate,
                    IdealRentalRate = dumpTruckTimeSheet.IdealRentalRate,
                    FuelType = dumpTruckTimeSheet.FuelType,
                    OilType = dumpTruckTimeSheet.OilType,
                    FuelRate = dumpTruckTimeSheet.FuelRate,
                    MaintenanceCost = dumpTruckTimeSheet.MaintenanceCost,
                    NetworkingHour = dumpTruckTimeSheet.NetworkingHour,
                    Perdium = dumpTruckTimeSheet.Perdium,
                    Salary = dumpTruckTimeSheet.Salary,
                    TireCost = dumpTruckTimeSheet.TireCost,
                    BranchId = dumpTruckTimeSheet.BranchId,
                    BusinessUnitId = dumpTruckTimeSheet.BusinessUnitId,
                    OperatorId = dumpTruckTimeSheet.OperatorId,
                    EquipmentCategoryId = dumpTruckTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = dumpTruckTimeSheet.EquipmentTypeId,
                    EquipmentNameId = dumpTruckTimeSheet.EquipmentNameId
                };

                db.DumpTruckTimeSheets.Attach(entity);
                db.DumpTruckTimeSheets.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted dump truck timesheet {entity.DumpTruckTimeSheetId }");
                #endregion
            }

            return Json(new[] { dumpTruckTimeSheet }.ToDataSourceResult(request, ModelState));
        }


        //---------------------------------- Approval Section --------------------------------------//








        #region Approval
        public JsonResult TimeSheetSendForApproval(int id)
        {

            var dumpTruckTimeSheet = db.DumpTruckTimeSheets.Find(id);

            if (dumpTruckTimeSheet != null)
            {
                dumpTruckTimeSheet.TimeSheetSendForAproval = true;
                dumpTruckTimeSheet.TimeSheetSendForApprovalTime = DateTime.Now;
                dumpTruckTimeSheet.TimeSheetSendForApprovalBy = User.Identity.Name;
                dumpTruckTimeSheet.TimeSheetStatus = MaintenanceConst.WaitingApproval;

                db.DumpTruckTimeSheets.Attach(dumpTruckTimeSheet);
                db.Entry(dumpTruckTimeSheet).State = EntityState.Modified;
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
        public JsonResult VoidTimeSheet(int id)
        {

            var dumpTruckTimeSheet = db.DumpTruckTimeSheets.Find(id);

            if (dumpTruckTimeSheet != null)
            {
                dumpTruckTimeSheet.TimeSheetSendForAproval = false;
                dumpTruckTimeSheet.TimeSheetApproved = false;
                dumpTruckTimeSheet.TimeSheetApprovedDate = DateTime.Now;
                dumpTruckTimeSheet.TimeSheetApprovedBy = User.Identity.Name;
                dumpTruckTimeSheet.TimeSheetStatus = MaintenanceConst.Rejected;

                db.DumpTruckTimeSheets.Attach(dumpTruckTimeSheet);
                db.Entry(dumpTruckTimeSheet).State = EntityState.Modified;
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
        public ActionResult DumpTruckTimeSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.TimeSheetApprovalNew.Where(x => x.TimeSheetId == id && x.TimeSheetType == TimeSheetType.DumpTruck);
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
                TimeSheetApprovalNewId = model.TimeSheetApprovalNewId,
                TimeSheetType = model.TimeSheetType
            });
            return Json(result);
        }
        [AuthorizeRoles(FleetManagementRoles.FleetTimeSheetApprover)]
        public ActionResult DumpTruckTimeSheetApproval_Create([DataSourceRequest] DataSourceRequest request, TimeSheetApprovalNew model, int id)
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
                model.TimeSheetType = TimeSheetType.DumpTruck;
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var dumpTruckTimeSheet = db.DumpTruckTimeSheets.Find(id);
            if (dumpTruckTimeSheet != null)
            {
                dumpTruckTimeSheet.TimeSheetApproved = true;
                dumpTruckTimeSheet.TimeSheetStatus = MaintenanceConst.Approved;
                dumpTruckTimeSheet.TimeSheetApprovedBy = User.Identity.Name;
                dumpTruckTimeSheet.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(dumpTruckTimeSheet).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var dumpTruckTimeSheet = db.DumpTruckTimeSheets.Find(id);
            if (dumpTruckTimeSheet != null)
            {
                dumpTruckTimeSheet.TimeSheetApproved = false;
                dumpTruckTimeSheet.TimeSheetSendForAproval = false;
                dumpTruckTimeSheet.TimeSheetStatus = MaintenanceConst.Rejected;
                dumpTruckTimeSheet.TimeSheetApprovedBy = User.Identity.Name;
                dumpTruckTimeSheet.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(dumpTruckTimeSheet).State = EntityState.Modified;
                db.SaveChanges();

            }
        }

        #endregion
        [HttpPost]
        public ActionResult ApproveSelected(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "No records selected."
                });
            }

            var records = db.DumpTruckTimeSheets
                            .Where(x => ids.Contains(x.DumpTruckTimeSheetId)&&x.TimeSheetSendForAproval==true)
                            .ToList();
             if(!records.Any())
            {
               var formattedData = new
                {
                    Message = "Send The Records For approval First",
                    Status = "NOTOK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            foreach (var item in records)
            {
                item.TimeSheetApproved = true;
                item.TimeSheetStatus = MaintenanceConst.Approved;
                item.TimeSheetApprovedBy = User.Identity.Name;
                item.TimeSheetApprovedDate = DateTime.Now;

                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
                var model = new TimeSheetApprovalNew();
                     model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.Status = "Approved";
                model.Remark = "Approved By Multi Approval";
                model.TimeSheetId = item.DumpTruckTimeSheetId;
                model.TimeSheetType = TimeSheetType.DumpTruck;
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();
            }
            
           

            return Json(new
            {
                Message = " record(s) approved successfully.",
                Status="OK"
            });
        }
        public ActionResult RejectSelected(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "No records selected."
                });
            }

            var records = db.DumpTruckTimeSheets
                            .Where(x => ids.Contains(x.DumpTruckTimeSheetId)&&x.TimeSheetApproved==true)
                            .ToList();
             if(!records.Any())
            {
               var formattedData = new
                {
                    Message = "Approve the records First",
                    Status = "NOTOK"
                };
                return Json(formattedData, JsonRequestBehavior.AllowGet);
            }
            foreach (var item in records)
            {
                item.TimeSheetApproved = false;
                item.TimeSheetSendForAproval = false;
                item.TimeSheetStatus = MaintenanceConst.Rejected;
                item.TimeSheetApprovedBy = User.Identity.Name;
                item.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(item).State = EntityState.Modified;
            }
            
            db.SaveChanges();

            return Json(new
            {
                Message = " record(s) Rejected successfully.",
                Status="OK"
            });
        }
        public ActionResult SendForApprovalSelected(List<int> ids)
        {
            if (ids == null || !ids.Any())
            {
                return Json(new
                {
                    success = false,
                    message = "No records selected."
                });
            }

            var records = db.DumpTruckTimeSheets
                            .Where(x => ids.Contains(x.DumpTruckTimeSheetId))
                            .ToList();

            foreach (var item in records)
            {
                item.TimeSheetSendForAproval = true;
                item.TimeSheetStatus = MaintenanceConst.WaitingApproval;
                item.TimeSheetSendForApprovalBy = User.Identity.Name;
                item.TimeSheetSendForApprovalTime = DateTime.Now;
                db.Entry(item).State = EntityState.Modified;
            }

            db.SaveChanges();

            return Json(new
            {
                Message = " record(s) send for approval successfully.",
                Status = "OK"
            });
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}