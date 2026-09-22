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
//using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    [AuthorizeRoles(FleetManagementRoles.FleetTimeSheetUser, FleetManagementRoles.FleetTimeSheetApprover)]
    public class WagonDrillTimesheetController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        public ActionResult Index(TimeSheetType wagonDrill)
        {
            ViewBag.TimesheetType = TimeSheetType.WagonDrill;
            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(
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
            ViewData["type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["name"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();

            ViewBag.wagonDrill = wagonDrill;
            ViewData["fleet"] = db.Fleets.Select(x => new
            {
                Value = x.FleetsID,
                Text = x.PlateNumber + " " + x.SerialNo + " " + x.Model,
            });
            var fetchSettingg = db.RequestToInspections.FirstOrDefault();
            ViewBag.setting = (fetchSettingg != null) ? fetchSettingg.Remark : "";
            string isECWCC = "Not";
            if (fetchSettingg != null)
            {

                isECWCC = fetchSettingg.Remark;
            }
            ViewBag.IsMIDROC = isECWCC;
            ViewData["branchh"] = db.Branch
     .Select(x => new SelectListItem
     {
         Text = x.Name,
         Value = x.BranchId.ToString()
     }).ToList();
            ViewBag.IsECWCC = isECWCC;
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
            
            return View();
        }
        public JsonResult RepopulateTimeSheet()
        {
            var timesheets = db.WagonDrillTimesheets.ToList();
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
        public ActionResult WagonDrillTimesheets_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var wagonDrillTimesheets = db.WagonDrillTimesheets.Where(x => mybraches.Contains(x.BranchId));
            DataSourceResult result = wagonDrillTimesheets.ToDataSourceResult(request, wagonDrillTimesheet => new WagonDrillTimesheet
            {
                WagonDrillTimesheetId = wagonDrillTimesheet.WagonDrillTimesheetId,
                FleetsID = wagonDrillTimesheet.FleetsID,
                ProcurementSupplierID = wagonDrillTimesheet.ProcurementSupplierID,
                Date = wagonDrillTimesheet.Date,
                WorkItem = wagonDrillTimesheet.WorkItem,
                OperationNetWorkingHour = wagonDrillTimesheet.OperationNetWorkingHour,
                OperationDrilledHoleDepth = wagonDrillTimesheet.OperationDrilledHoleDepth,
                NumberOfHolesDrilled = wagonDrillTimesheet.NumberOfHolesDrilled,
                OperationRateForMeter = wagonDrillTimesheet.OperationRateForMeter,
                OperationAmount=wagonDrillTimesheet.OperationAmount,
                IdleAmount=wagonDrillTimesheet.IdleAmount,
                TotalAmountBeforeVAT=wagonDrillTimesheet.TotalAmountBeforeVAT,
                TotalAmount=wagonDrillTimesheet.TotalAmount,
                FuelCost=wagonDrillTimesheet.FuelCost,
                TotalCost=wagonDrillTimesheet.TotalCost,
                NetAmount=wagonDrillTimesheet.NetAmount,
                IdleHour = wagonDrillTimesheet.IdleHour,
                BatteryCost = wagonDrillTimesheet.BatteryCost,
                IdleRateHour = wagonDrillTimesheet.IdleRateHour,
                DownTime = wagonDrillTimesheet.DownTime,
                VAT = wagonDrillTimesheet.VAT,
                ReasonForDown = wagonDrillTimesheet.ReasonForDown,
                ReasonForIdle = wagonDrillTimesheet.ReasonForIdle,
                Unit = wagonDrillTimesheet.Unit,
                Volume = wagonDrillTimesheet.Volume,
                ExternalOperatorName = wagonDrillTimesheet.ExternalOperatorName,
                FuelInLiter = wagonDrillTimesheet.FuelInLiter,
                FuelRate = wagonDrillTimesheet.FuelRate,
                ServiceCharge = wagonDrillTimesheet.ServiceCharge,
                TireCost = wagonDrillTimesheet.TireCost,
                DateCreated = wagonDrillTimesheet.DateCreated,
                CreatedBy = wagonDrillTimesheet.CreatedBy,
                ModifiedBy = wagonDrillTimesheet.ModifiedBy,
                LastModified = wagonDrillTimesheet.LastModified,
                Perdium = wagonDrillTimesheet.Perdium,
                Salary = wagonDrillTimesheet.Salary,
                BranchId = wagonDrillTimesheet.BranchId,
                FuelType = wagonDrillTimesheet.FuelType,
                FuelKMR = wagonDrillTimesheet.FuelKMR,
                SparePartCost = wagonDrillTimesheet.SparePartCost,
                OilCost = wagonDrillTimesheet.OilCost,
                GreaseConsumption = wagonDrillTimesheet.GreaseConsumption,
                GreaseCost = wagonDrillTimesheet.GreaseCost,
                MaintenanceCost = wagonDrillTimesheet.MaintenanceCost,
                OilConsumption = wagonDrillTimesheet.OilConsumption,
                DifferenceEHR = wagonDrillTimesheet.DifferenceEHR,
                FinalEHR = wagonDrillTimesheet.FinalEHR,
                InitialEHR = wagonDrillTimesheet.InitialEHR,
                MorningTimeStart = wagonDrillTimesheet.MorningTimeStart,
                MorningTimeEnd = wagonDrillTimesheet.MorningTimeEnd,
                AfternoonTimeStart = wagonDrillTimesheet.AfternoonTimeStart,
                AfternoonTimeEnd = wagonDrillTimesheet.AfternoonTimeEnd,
                NightTimeStart = wagonDrillTimesheet.NightTimeStart,
                NightTimeEnd = wagonDrillTimesheet.NightTimeEnd,
                TotalTime = wagonDrillTimesheet.TotalTime,
                OilType = wagonDrillTimesheet.OilType,
                BusinessUnitId = wagonDrillTimesheet.BusinessUnitId,
                CostSheetApproved = wagonDrillTimesheet.CostSheetApproved,
                CostSheetApprovedBy = wagonDrillTimesheet.CostSheetApprovedBy,
                CostSheetApprovedDate = wagonDrillTimesheet.CostSheetApprovedDate,
                CostSheetSendForAproval = wagonDrillTimesheet.CostSheetSendForAproval,
                TimeSheetStatus = wagonDrillTimesheet.TimeSheetStatus,
                TimeSheetApproved = wagonDrillTimesheet.TimeSheetApproved,
                TimeSheetApprovedBy = wagonDrillTimesheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = wagonDrillTimesheet.TimeSheetApprovedDate,
                TimeSheetSendForAproval =   wagonDrillTimesheet.TimeSheetSendForAproval,
                OperatorId = wagonDrillTimesheet.OperatorId,
                DownReason = wagonDrillTimesheet.DownReason,
                IdleReason = wagonDrillTimesheet.IdleReason,
                EquipmentCategoryId = wagonDrillTimesheet.EquipmentCategoryId,
                EquipmentTypeId = wagonDrillTimesheet.EquipmentTypeId,
                EquipmentNameId = wagonDrillTimesheet.EquipmentNameId
            });
            return Json(result);
        }
        public ActionResult WagonDrillTimesheets_Create([DataSourceRequest] DataSourceRequest request, WagonDrillTimesheet wagonDrillTimesheet)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == wagonDrillTimesheet.FleetsID);
            wagonDrillTimesheet.EquipmentCategoryId = fleet.EquipmentIdentityID;
            wagonDrillTimesheet.EquipmentTypeId = fleet.EquipmentTypeID;
            wagonDrillTimesheet.EquipmentNameId = fleet.EquipmentNameID;
            var timeSheets = db.WagonDrillTimesheets.FirstOrDefault(x => x.FleetsID == wagonDrillTimesheet.FleetsID && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(wagonDrillTimesheet.Date));
            if (timeSheets != null)
            {
                ModelState.AddModelError("duplicate", "Timesheet for this Plate On the selected day is already added");
            }
            var fetchSettings = db.TimeSheetSetting.ToList();
            if (fetchSettings.Any())
            {
                var fetchSetting = fetchSettings.First();
                var fetchException = db.TimeSheetException.FirstOrDefault(x => x.TimeSheetSettingId == fetchSetting.TimeSheetSettingId && x.BranchId == wagonDrillTimesheet.BranchId.ToString());
                if (fetchException == null)
                {
                    if (fetchSetting != null && fetchSetting.IsSet == true)
                    {
                        int hoursDifference = (int)(DateTime.Now - wagonDrillTimesheet.Date).TotalHours;

                        if (fetchSetting.SettedHours < hoursDifference)
                        {
                            ModelState.AddModelError("date expired", "The timesheet recording date is passed");
                        }
                    }
                }
            }
            var requestToInspection = db.RequestToInspections.FirstOrDefault();
            var lastTimesheet = db.WagonDrillTimesheets.Where(x => x.FleetsID == wagonDrillTimesheet.FleetsID).OrderByDescending(x => x.WagonDrillTimesheetId).FirstOrDefault();
            if (requestToInspection!=null&&requestToInspection.Remark=="MCG")
            {

                if (wagonDrillTimesheet.InitialEHR < 0 || wagonDrillTimesheet.FinalEHR < 0 || (wagonDrillTimesheet.FinalEHR - wagonDrillTimesheet.InitialEHR) < 0 || wagonDrillTimesheet.FuelKMR < 0)
                {
                    ModelState.AddModelError("invalid", "Invalid EHR entry");
                }
                wagonDrillTimesheet.ReasonForDown = wagonDrillTimesheet.DownReason.ToString();
                wagonDrillTimesheet.ReasonForIdle = wagonDrillTimesheet.IdleReason.ToString();
                if (wagonDrillTimesheet.MorningTimeStart == null || wagonDrillTimesheet.MorningTimeEnd == null)
                {
                    wagonDrillTimesheet.MorningTimeStart = DateTime.Now;
                    wagonDrillTimesheet.MorningTimeEnd = DateTime.Now;
                }
                if (wagonDrillTimesheet.AfternoonTimeStart == null || wagonDrillTimesheet.AfternoonTimeEnd == null)
                {
                    wagonDrillTimesheet.AfternoonTimeStart = DateTime.Now;
                    wagonDrillTimesheet.AfternoonTimeEnd = DateTime.Now;
                }
                if (wagonDrillTimesheet.NightTimeStart == null || wagonDrillTimesheet.NightTimeEnd == null)
                {
                    wagonDrillTimesheet.NightTimeStart = DateTime.Now;
                    wagonDrillTimesheet.NightTimeEnd = DateTime.Now;
                }
                wagonDrillTimesheet.OperationNetWorkingHour =
                    (decimal)GetTotalTime(
                        wagonDrillTimesheet.MorningTimeStart.Value,
                        wagonDrillTimesheet.MorningTimeEnd.Value,
                        wagonDrillTimesheet.AfternoonTimeStart.Value,
                        wagonDrillTimesheet.AfternoonTimeEnd.Value,
                        wagonDrillTimesheet.NightTimeStart.Value,
                        wagonDrillTimesheet.NightTimeEnd.Value
                    ).TotalHours;
                wagonDrillTimesheet.TotalTime = wagonDrillTimesheet.OperationNetWorkingHour;
                if (wagonDrillTimesheet.MorningTimeEnd == wagonDrillTimesheet.MorningTimeStart)
                {
                    wagonDrillTimesheet.MorningTimeStart = null;
                    wagonDrillTimesheet.MorningTimeEnd = null;
                }
                if (wagonDrillTimesheet.AfternoonTimeStart == wagonDrillTimesheet.AfternoonTimeEnd)
                {
                    wagonDrillTimesheet.AfternoonTimeEnd = null;
                    wagonDrillTimesheet.AfternoonTimeStart = null;
                }
                if (wagonDrillTimesheet.NightTimeEnd == wagonDrillTimesheet.NightTimeStart)
                {
                    wagonDrillTimesheet.NightTimeStart = null;
                    wagonDrillTimesheet.NightTimeEnd = null;
                }

            }
            fleet.BranchId = wagonDrillTimesheet.BranchId;
            db.Entry(fleet).State = EntityState.Modified;
            db.SaveChanges();
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
                            == DbFunctions.TruncateTime(wagonDrillTimesheet.Date)
                        && OilLubItems.Contains(ii.ItemCode)).ToList();
                wagonDrillTimesheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                wagonDrillTimesheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                            == DbFunctions.TruncateTime(wagonDrillTimesheet.Date)
                        && oilItems.Contains(ii.ItemCode)).ToList();
                wagonDrillTimesheet.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                wagonDrillTimesheet.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                            == DbFunctions.TruncateTime(wagonDrillTimesheet.Date)
                        && allSparePartItems.Contains(ii.ItemCode)).ToList();
                var issuedBatteryItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(wagonDrillTimesheet.Date)
                        && allBatteryItems.Contains(ii.ItemCode)).ToList();
                var issuedTyreItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(wagonDrillTimesheet.Date)
                        && allTyreItems.Contains(ii.ItemCode)).ToList();
                wagonDrillTimesheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                wagonDrillTimesheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                wagonDrillTimesheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == fleet.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(wagonDrillTimesheet.Date));
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
                wagonDrillTimesheet.MaintenanceCost += maintenancePayment;
            }
            if (ModelState.IsValid)
            {
                var entity = new WagonDrillTimesheet
                {
                    WagonDrillTimesheetId = wagonDrillTimesheet.WagonDrillTimesheetId,
                    FleetsID = wagonDrillTimesheet.FleetsID,
                    ProcurementSupplierID = wagonDrillTimesheet.ProcurementSupplierID,
                    Date = wagonDrillTimesheet.Date,
                    WorkItem = wagonDrillTimesheet.WorkItem,
                    OperationNetWorkingHour = wagonDrillTimesheet.OperationNetWorkingHour,
                    OperationDrilledHoleDepth = wagonDrillTimesheet.OperationDrilledHoleDepth,
                    NumberOfHolesDrilled = wagonDrillTimesheet.NumberOfHolesDrilled,
                    OperationRateForMeter = wagonDrillTimesheet.OperationRateForMeter,
                    IdleHour = wagonDrillTimesheet.IdleHour,
                    IdleRateHour = wagonDrillTimesheet.IdleRateHour,
                    DownTime = wagonDrillTimesheet.DownTime,
                    VAT = wagonDrillTimesheet.VAT,
                    FuelInLiter = wagonDrillTimesheet.FuelInLiter,
                    FuelRate = wagonDrillTimesheet.FuelRate,
                    ServiceCharge = wagonDrillTimesheet.ServiceCharge,
                    TireCost = wagonDrillTimesheet.TireCost,
                    FuelType = wagonDrillTimesheet.FuelType,
                    OilType = wagonDrillTimesheet.OilType,
                    DateCreated = DateTime.Now.Date,
                    CreatedBy = User.Identity.Name,
                    Unit =fleet.Unit,
                    Volume =fleet.Volume,
                    TotalTime = wagonDrillTimesheet.TotalTime,
                    RentalAgreementNum = fleet.RentFromExternalNum,
                    SparePartCost = wagonDrillTimesheet.SparePartCost,
                    OilCost = wagonDrillTimesheet.OilCost,
                    GreaseConsumption = wagonDrillTimesheet.GreaseConsumption,
                    GreaseCost = wagonDrillTimesheet.GreaseCost,
                    MaintenanceCost = wagonDrillTimesheet.MaintenanceCost,
                    OilConsumption = wagonDrillTimesheet.OilConsumption,

                    DownReason = wagonDrillTimesheet.DownReason,
                    IdleReason = wagonDrillTimesheet.IdleReason,                    
                    ExternalOperatorName = wagonDrillTimesheet.ExternalOperatorName,
                    MorningTimeEnd = wagonDrillTimesheet.MorningTimeEnd,
                    MorningTimeStart = wagonDrillTimesheet.MorningTimeStart,
                    AfternoonTimeEnd = wagonDrillTimesheet.AfternoonTimeEnd,
                    AfternoonTimeStart = wagonDrillTimesheet.AfternoonTimeStart,
                    NightTimeEnd = wagonDrillTimesheet.NightTimeEnd,
                    NightTimeStart = wagonDrillTimesheet.NightTimeStart,
                    ModifiedBy = wagonDrillTimesheet.ModifiedBy,
                    LastModified = wagonDrillTimesheet.LastModified,
                    OperationAmount = wagonDrillTimesheet.OperationAmount,
                    IdleAmount = wagonDrillTimesheet.IdleAmount,
                    TotalAmountBeforeVAT = wagonDrillTimesheet.TotalAmountBeforeVAT,
                    TotalAmount = wagonDrillTimesheet.TotalAmount,
                    FuelCost = wagonDrillTimesheet.FuelCost,
                    TotalCost = wagonDrillTimesheet.TotalCost,
                    NetAmount = wagonDrillTimesheet.NetAmount,
                    Perdium = wagonDrillTimesheet.Perdium,
                    Salary = wagonDrillTimesheet.Salary,
                    BranchId = wagonDrillTimesheet.BranchId,
                    BusinessUnitId = wagonDrillTimesheet.BusinessUnitId,
                    OperatorId = wagonDrillTimesheet.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    ReasonForDown = wagonDrillTimesheet.ReasonForDown,
                    ReasonForIdle = wagonDrillTimesheet.ReasonForIdle,
                    EquipmentCategoryId = wagonDrillTimesheet.EquipmentCategoryId,
                    EquipmentTypeId = wagonDrillTimesheet.EquipmentTypeId,
                    EquipmentNameId = wagonDrillTimesheet.EquipmentNameId,
                    
                };
                
                    entity.InitialEHR = wagonDrillTimesheet.InitialEHR;
                    entity.FinalEHR = wagonDrillTimesheet.FinalEHR;
                    entity.DifferenceEHR = wagonDrillTimesheet.FinalEHR - wagonDrillTimesheet.InitialEHR;
                    entity.FuelKMR = wagonDrillTimesheet.FuelKMR;
                
               
                db.WagonDrillTimesheets.Add(entity);
                db.SaveChanges();

                //var fleetSetting = db.RequestToInspections.FirstOrDefault(x => x.Remark != null);

                //if (fleetSetting != null && fleetSetting.Remark == "MIDROC")
                //{
                //    List<WagonDrillTimesheet> timeSheet = db.WagonDrillTimesheets
                //        .Where(x => x.BranchId == entity.BranchId &&
                //                    x.EquipmentCategoryId == entity.EquipmentCategoryId && x.FleetsID != entity.FleetsID)
                //        .GroupBy(x => x.FleetsID)
                //        .Select(g => g.OrderByDescending(x => x.WagonDrillTimesheetId).FirstOrDefault())
                //        .ToList();
                //    var firstObject = timeSheet.First();
                //    var checkDuplicatee = db.WagonDrillTimesheets.FirstOrDefault(x => x.FleetsID == firstObject.FleetsID && x.Date == firstObject.Date);
                //    if (checkDuplicatee == null        
                //    {
                //        foreach (var timesht in timeSheet)
                //        {
                //            var newTimeSheet = new WagonDrillTimesheet
                //            {
                //                WagonDrillTimesheetId = timesht.WagonDrillTimesheetId,
                //                FleetsID = timesht.FleetsID,
                //                ProcurementSupplierID = timesht.ProcurementSupplierID,
                //                Date = timesht.Date,
                //                WorkItem = timesht.WorkItem,
                //                OperationNetWorkingHour = timesht.OperationNetWorkingHour,
                //                OperationDrilledHoleDepth = timesht.OperationDrilledHoleDepth,
                //                NumberOfHolesDrilled = timesht.NumberOfHolesDrilled,
                //                OperationRateForMeter = timesht.OperationRateForMeter,
                //                IdleHour = timesht.IdleHour,
                //                IdleRateHour = timesht.IdleRateHour,
                //                DownTime = timesht.DownTime,
                //                VAT = timesht.VAT,
                //                FuelInLiter = timesht.FuelInLiter,
                //                FuelRate = timesht.FuelRate,
                //                ServiceCharge = timesht.ServiceCharge,
                //                LubricantCost = timesht.LubricantCost,
                //                MaintenanceCost = timesht.MaintenanceCost,
                //                TireCost = timesht.TireCost,
                //                FuelType = timesht.FuelType,
                //                OilType = timesht.OilType,
                //                DateCreated = DateTime.Now.Date,
                //                CreatedBy = User.Identity.Name,
                //                ModifiedBy = timesht.ModifiedBy,
                //                LastModified = timesht.LastModified,
                //                OperationAmount = timesht.OperationAmount,
                //                IdleAmount = timesht.IdleAmount,
                //                TotalAmountBeforeVAT = timesht.TotalAmountBeforeVAT,
                //                TotalAmount = timesht.TotalAmount,
                //                FuelCost = timesht.FuelCost,
                //                TotalCost = timesht.TotalCost,
                //                NetAmount = timesht.NetAmount,
                //                Perdium = timesht.Perdium,
                //                Salary = timesht.Salary,
                //                BranchId = timesht.BranchId,
                //                BusinessUnitId = timesht.BusinessUnitId,
                //                OperatorId = timesht.OperatorId,
                //                TimeSheetStatus = MaintenanceConst.Pending,
                //                ReasonForDown = timesht.ReasonForDown,
                //                ReasonForIdle = timesht.ReasonForIdle,
                //                EquipmentCategoryId = timesht.EquipmentCategoryId,
                //                EquipmentTypeId = timesht.EquipmentTypeId,
                //                EquipmentNameId = timesht.EquipmentNameId,
                //            };

                //            db.WagonDrillTimesheets.Add(newTimeSheet);
                //            db.SaveChanges();
                //        }
                //    }
                //}

                wagonDrillTimesheet.WagonDrillTimesheetId = entity.WagonDrillTimesheetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dry loading timesheet registration", "Success", User.Identity.Name, $"created dry loading timesheet {entity.WagonDrillTimesheetId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { wagonDrillTimesheet }.ToDataSourceResult(request, ModelState));
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
        public ActionResult WagonDrillTimesheets_Update([DataSourceRequest] DataSourceRequest request, WagonDrillTimesheet wagonDrillTimesheet)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == wagonDrillTimesheet.FleetsID);
            var timeSheets = db.WagonDrillTimesheets.FirstOrDefault(x => x.FleetsID == wagonDrillTimesheet.FleetsID && x.WagonDrillTimesheetId != wagonDrillTimesheet.WagonDrillTimesheetId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(wagonDrillTimesheet.Date));
            if (timeSheets != null)
            {
                ModelState.AddModelError("duplicate", "Timesheet for this Plate On the selected day is already added");
            }
            var lastTimesheet = db.WagonDrillTimesheets.Where(x => x.FleetsID == wagonDrillTimesheet.FleetsID && x.WagonDrillTimesheetId != wagonDrillTimesheet.WagonDrillTimesheetId).OrderByDescending(x => x.WagonDrillTimesheetId).FirstOrDefault();

            var requestToInspection = db.RequestToInspections.FirstOrDefault();
            if (requestToInspection != null && requestToInspection.Remark == "MCG")
            {

                if (wagonDrillTimesheet.InitialEHR < 0 || wagonDrillTimesheet.FinalEHR < 0 || (wagonDrillTimesheet.FinalEHR - wagonDrillTimesheet.InitialEHR) < 0 || wagonDrillTimesheet.FuelKMR < 0)
                {
                    ModelState.AddModelError("invalid", "Invalid EHR entry");
                }
                //if (lastTimesheet != null && wagonDrillTimesheet.FuelKMR <= lastTimesheet.FuelKMR)
                //{
                //    ModelState.AddModelError("invalid", "Invalid Fuel KMR entry");
                //}
                //if (lastTimesheet != null && wagonDrillTimesheet.FinalEHR <= lastTimesheet.FinalEHR)
                //{
                //    ModelState.AddModelError("invalid", "Invalid EHR entry");
                //}
                //if (lastTimesheet != null && wagonDrillTimesheet.FuelKMR <= lastTimesheet.FuelKMR)
                //{
                //    ModelState.AddModelError("invalid", "Invalid Fuel KMR entry");
                //}
                //if (lastTimesheet != null && wagonDrillTimesheet.FinalEHR <= lastTimesheet.FinalEHR)
                //{
                //    ModelState.AddModelError("invalid", "Invalid EHR entry");
                //}
                if (wagonDrillTimesheet.MorningTimeStart == null || wagonDrillTimesheet.MorningTimeEnd == null)
                {
                    wagonDrillTimesheet.MorningTimeStart = DateTime.Now;
                    wagonDrillTimesheet.MorningTimeEnd = DateTime.Now;
                }
                if (wagonDrillTimesheet.AfternoonTimeStart == null || wagonDrillTimesheet.AfternoonTimeEnd == null)
                {
                    wagonDrillTimesheet.AfternoonTimeStart = DateTime.Now;
                    wagonDrillTimesheet.AfternoonTimeEnd = DateTime.Now;
                }
                if (wagonDrillTimesheet.NightTimeStart == null || wagonDrillTimesheet.NightTimeEnd == null)
                {
                    wagonDrillTimesheet.NightTimeStart = DateTime.Now;
                    wagonDrillTimesheet.NightTimeEnd = DateTime.Now;
                }
                wagonDrillTimesheet.OperationNetWorkingHour =
                    (decimal)GetTotalTime(
                        wagonDrillTimesheet.MorningTimeStart.Value,
                        wagonDrillTimesheet.MorningTimeEnd.Value,
                        wagonDrillTimesheet.AfternoonTimeStart.Value,
                        wagonDrillTimesheet.AfternoonTimeEnd.Value,
                        wagonDrillTimesheet.NightTimeStart.Value,
                        wagonDrillTimesheet.NightTimeEnd.Value
                    ).TotalHours;
                wagonDrillTimesheet.TotalTime = wagonDrillTimesheet.OperationNetWorkingHour;
                if (wagonDrillTimesheet.MorningTimeEnd == wagonDrillTimesheet.MorningTimeStart)
                {
                    wagonDrillTimesheet.MorningTimeStart = null;
                    wagonDrillTimesheet.MorningTimeEnd = null;
                }
                if (wagonDrillTimesheet.AfternoonTimeStart == wagonDrillTimesheet.AfternoonTimeEnd)
                {
                    wagonDrillTimesheet.AfternoonTimeEnd = null;
                    wagonDrillTimesheet.AfternoonTimeStart = null;
                }
                if (wagonDrillTimesheet.NightTimeEnd == wagonDrillTimesheet.NightTimeStart)
                {
                    wagonDrillTimesheet.NightTimeStart = null;
                    wagonDrillTimesheet.NightTimeEnd = null;
                }

            }

            if (ModelState.IsValid)
            {
                var entity = new WagonDrillTimesheet
                {
                    WagonDrillTimesheetId = wagonDrillTimesheet.WagonDrillTimesheetId,
                    FleetsID = wagonDrillTimesheet.FleetsID,
                    ProcurementSupplierID = wagonDrillTimesheet.ProcurementSupplierID,
                    Date = wagonDrillTimesheet.Date,
                    WorkItem = wagonDrillTimesheet.WorkItem,
                    OperationNetWorkingHour = wagonDrillTimesheet.OperationNetWorkingHour,
                    MorningTimeEnd = wagonDrillTimesheet.MorningTimeEnd,
                    MorningTimeStart = wagonDrillTimesheet.MorningTimeStart,
                    AfternoonTimeEnd = wagonDrillTimesheet.AfternoonTimeEnd,
                    AfternoonTimeStart = wagonDrillTimesheet.AfternoonTimeStart,
                    NightTimeEnd = wagonDrillTimesheet.NightTimeEnd,
                    NightTimeStart = wagonDrillTimesheet.NightTimeStart,
                    OperationDrilledHoleDepth = wagonDrillTimesheet.OperationDrilledHoleDepth,
                    NumberOfHolesDrilled = wagonDrillTimesheet.NumberOfHolesDrilled,
                    OperationRateForMeter = wagonDrillTimesheet.OperationRateForMeter,
                    IdleHour = wagonDrillTimesheet.IdleHour,
                    IdleRateHour = wagonDrillTimesheet.IdleRateHour,
                    DownTime = wagonDrillTimesheet.DownTime,
                    VAT = wagonDrillTimesheet.VAT,
                    FuelInLiter = wagonDrillTimesheet.FuelInLiter,
                    FuelRate = wagonDrillTimesheet.FuelRate,
                    TotalTime = wagonDrillTimesheet.TotalTime,

                    SparePartCost = wagonDrillTimesheet.SparePartCost,
                    OilCost = wagonDrillTimesheet.OilCost,
                    GreaseConsumption = wagonDrillTimesheet.GreaseConsumption,
                    GreaseCost = wagonDrillTimesheet.GreaseCost,
                    MaintenanceCost = wagonDrillTimesheet.MaintenanceCost,
                    OilConsumption = wagonDrillTimesheet.OilConsumption,
                    RentalAgreementNum = fleet.RentFromExternalNum,

                    ServiceCharge = wagonDrillTimesheet.ServiceCharge,
                    TireCost = wagonDrillTimesheet.TireCost,
                    DateCreated = wagonDrillTimesheet.DateCreated,
                    FuelType = wagonDrillTimesheet.FuelType,
                    OilType = wagonDrillTimesheet.OilType,
                    CreatedBy = wagonDrillTimesheet.CreatedBy,
                    ModifiedBy =User.Identity.Name,
                    DownReason = wagonDrillTimesheet.DownReason,
                    IdleReason = wagonDrillTimesheet.IdleReason,
                    LastModified =DateTime.Now.Date,
                    OperationAmount = wagonDrillTimesheet.OperationAmount,
                    IdleAmount = wagonDrillTimesheet.IdleAmount,
                    TotalAmountBeforeVAT = wagonDrillTimesheet.TotalAmountBeforeVAT,
                    TotalAmount = wagonDrillTimesheet.TotalAmount,
                    FuelCost = wagonDrillTimesheet.FuelCost,
                    TotalCost = wagonDrillTimesheet.TotalCost,
                    NetAmount = wagonDrillTimesheet.NetAmount,
                    Perdium = wagonDrillTimesheet.Perdium,
                    ExternalOperatorName = wagonDrillTimesheet.ExternalOperatorName,
                    Salary = wagonDrillTimesheet.Salary,
                    BranchId = wagonDrillTimesheet.BranchId,
                    BusinessUnitId = wagonDrillTimesheet.BusinessUnitId,
                    OperatorId = wagonDrillTimesheet.OperatorId,
                    ReasonForDown = wagonDrillTimesheet.ReasonForDown,
                    ReasonForIdle = wagonDrillTimesheet.ReasonForIdle,
                    EquipmentCategoryId = wagonDrillTimesheet.EquipmentCategoryId,
                    EquipmentTypeId = wagonDrillTimesheet.EquipmentTypeId,
                    EquipmentNameId = wagonDrillTimesheet.EquipmentNameId,
                    
                };
               
                    entity.InitialEHR = wagonDrillTimesheet.InitialEHR;
                    entity.FinalEHR = wagonDrillTimesheet.FinalEHR;
                    entity.DifferenceEHR = wagonDrillTimesheet.FinalEHR - wagonDrillTimesheet.InitialEHR;
                    entity.FuelKMR = wagonDrillTimesheet.FuelKMR;
                
               
                db.WagonDrillTimesheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Wagon drill timesheet update", "Success", User.Identity.Name, $"updated wagon drill timesheet {entity.WagonDrillTimesheetId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { wagonDrillTimesheet }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult WagonDrillTimesheets_Destroy([DataSourceRequest] DataSourceRequest request, WagonDrillTimesheet wagonDrillTimesheet)
        {
            if (ModelState.IsValid)
            {
                var entity = new WagonDrillTimesheet
                {
                    WagonDrillTimesheetId = wagonDrillTimesheet.WagonDrillTimesheetId,
                    FleetsID = wagonDrillTimesheet.FleetsID,
                    ProcurementSupplierID = wagonDrillTimesheet.ProcurementSupplierID,
                    Date = wagonDrillTimesheet.Date,
                    WorkItem = wagonDrillTimesheet.WorkItem,
                    OperationNetWorkingHour = wagonDrillTimesheet.OperationNetWorkingHour,
                    OperationDrilledHoleDepth = wagonDrillTimesheet.OperationDrilledHoleDepth,
                    NumberOfHolesDrilled = wagonDrillTimesheet.NumberOfHolesDrilled,
                    OperationRateForMeter = wagonDrillTimesheet.OperationRateForMeter,
                    IdleHour = wagonDrillTimesheet.IdleHour,
                    IdleRateHour = wagonDrillTimesheet.IdleRateHour,
                    DownTime = wagonDrillTimesheet.DownTime,
                    VAT = wagonDrillTimesheet.VAT,
                    FuelInLiter = wagonDrillTimesheet.FuelInLiter,
                    FuelRate = wagonDrillTimesheet.FuelRate,
                    FuelType = wagonDrillTimesheet.FuelType,
                    OilType = wagonDrillTimesheet.OilType,
                    ServiceCharge = wagonDrillTimesheet.ServiceCharge,
                    MaintenanceCost = wagonDrillTimesheet.MaintenanceCost,
                    TireCost = wagonDrillTimesheet.TireCost,
                    DateCreated = wagonDrillTimesheet.DateCreated,
                    CreatedBy = wagonDrillTimesheet.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now.Date,
                    DownReason = wagonDrillTimesheet.DownReason,
                    IdleReason = wagonDrillTimesheet.IdleReason,
                    OperationAmount = wagonDrillTimesheet.OperationAmount,
                    IdleAmount = wagonDrillTimesheet.IdleAmount,
                    TotalAmountBeforeVAT = wagonDrillTimesheet.TotalAmountBeforeVAT,
                    TotalAmount = wagonDrillTimesheet.TotalAmount,
                    FuelCost = wagonDrillTimesheet.FuelCost,
                    TotalCost = wagonDrillTimesheet.TotalCost,
                    NetAmount = wagonDrillTimesheet.NetAmount,
                    Perdium = wagonDrillTimesheet.Perdium,
                    Salary = wagonDrillTimesheet.Salary,
                    BranchId = wagonDrillTimesheet.BranchId,
                    BusinessUnitId = wagonDrillTimesheet.BusinessUnitId,
                    OperatorId = wagonDrillTimesheet.OperatorId,
                    ReasonForDown = wagonDrillTimesheet.ReasonForDown,
                    ReasonForIdle = wagonDrillTimesheet.ReasonForIdle,
                    EquipmentCategoryId = wagonDrillTimesheet.EquipmentCategoryId,
                    EquipmentTypeId = wagonDrillTimesheet.EquipmentTypeId,
                    EquipmentNameId = wagonDrillTimesheet.EquipmentNameId
                };
                db.WagonDrillTimesheets.Attach(entity);
                db.WagonDrillTimesheets.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted wagon drill timesheet {entity.WagonDrillTimesheetId }");
                #endregion
            }
            return Json(new[] { wagonDrillTimesheet }.ToDataSourceResult(request, ModelState));
        }



        //-------------------------------------approve---------------------------------------//


        #region Approval
        public JsonResult TimeSheetSendForApproval(int id)
        {

            var wagonDrillTimesheets = db.WagonDrillTimesheets.Find(id);

            if (wagonDrillTimesheets != null)
            {
                wagonDrillTimesheets.TimeSheetSendForAproval = true;
                wagonDrillTimesheets.TimeSheetSendForApprovalTime = DateTime.Now;
                wagonDrillTimesheets.TimeSheetSendForApprovalBy = User.Identity.Name;
                wagonDrillTimesheets.TimeSheetStatus = MaintenanceConst.WaitingApproval;

                db.WagonDrillTimesheets.Attach(wagonDrillTimesheets);
                db.Entry(wagonDrillTimesheets).State = EntityState.Modified;
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

            var wagonDrillTimesheets = db.WagonDrillTimesheets.Find(id);

            if (wagonDrillTimesheets != null)
            {
                wagonDrillTimesheets.TimeSheetSendForAproval = false;
                wagonDrillTimesheets.TimeSheetApproved = false;
                wagonDrillTimesheets.TimeSheetApprovedDate = DateTime.Now;
                wagonDrillTimesheets.TimeSheetApprovedBy = User.Identity.Name;
                wagonDrillTimesheets.TimeSheetStatus = MaintenanceConst.Rejected;

                db.WagonDrillTimesheets.Attach(wagonDrillTimesheets);
                db.Entry(wagonDrillTimesheets).State = EntityState.Modified;
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
        public ActionResult WagonDrillTimeSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.TimeSheetApprovalNew.Where(x => x.TimeSheetId == id && x.TimeSheetType == TimeSheetType.WagonDrill);
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
        public ActionResult WagonDrillTimeSheetApproval_Create([DataSourceRequest] DataSourceRequest request, TimeSheetApprovalNew model, int id)
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
                model.TimeSheetType = TimeSheetType.WagonDrill;
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var wagonDrillTimesheets = db.WagonDrillTimesheets.Find(id);
            if (wagonDrillTimesheets != null)
            {
                wagonDrillTimesheets.TimeSheetApproved = true;
                wagonDrillTimesheets.TimeSheetStatus = MaintenanceConst.Approved;
                wagonDrillTimesheets.TimeSheetApprovedBy = User.Identity.Name;
                wagonDrillTimesheets.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(wagonDrillTimesheets).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var wagonDrillTimesheets = db.WagonDrillTimesheets.Find(id);
            if (wagonDrillTimesheets != null)
            {
                wagonDrillTimesheets.TimeSheetApproved = false;
                wagonDrillTimesheets.TimeSheetSendForAproval = false;
                wagonDrillTimesheets.TimeSheetStatus = MaintenanceConst.Rejected;
                wagonDrillTimesheets.TimeSheetApprovedBy = User.Identity.Name;
                wagonDrillTimesheets.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(wagonDrillTimesheets).State = EntityState.Modified;
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

            var records = db.WagonDrillTimesheets
                            .Where(x => ids.Contains(x.WagonDrillTimesheetId) && x.TimeSheetSendForAproval == true)
                            .ToList();
            if (!records.Any())
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
                model.TimeSheetId = item.WagonDrillTimesheetId;
                model.TimeSheetType = TimeSheetType.WagonDrill;
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();
            }


            return Json(new
            {
                Message = " record(s) approved successfully.",
                Status = "OK"
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

            var records = db.WagonDrillTimesheets
                            .Where(x => ids.Contains(x.WagonDrillTimesheetId) && x.TimeSheetApproved == true)
                            .ToList();
            if (!records.Any())
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
                Status = "OK"
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

            var records = db.WagonDrillTimesheets
                            .Where(x => ids.Contains(x.WagonDrillTimesheetId))
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