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
using ExceedERP.Core.Domain.FleetManagement.FleetInsurances;
using ExceedERP.Core.Domain.Global;

using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Reporting.Fleet.ViewModel;
//using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Core.Domain.Maintenance;
namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetTimesheet
{
    [AuthorizeRoles(FleetManagementRoles.FleetTimeSheetUser, FleetManagementRoles.FleetTimeSheetApprover)]
    public class MachineryWorkingTimeSheetController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        public ActionResult Index(TimeSheetType machinery)
        {
            ViewData["RepairTypes"] = db.RepairType.Select(s => new
            {
                Code = s.RepairTypeID,
                Name = s.Name
            }).AsQueryable();
            ViewBag.setting = db.RequestToInspections
                  .FirstOrDefault()?.Remark ?? " ";
            ViewBag.TimesheetType = TimeSheetType.Machinery;
            ViewData["category"] = CacheData.GetEquipmentIdentityCaches().Select(
    i => new
    {
        Text = i.Name,
        Value = i.Id
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
            ViewBag.machineryType = machinery;
            ViewData["fleet"] = db.Fleets.Select(x => new
            {
                Value = x.FleetsID,
                Text = x.PlateNumber+" "+x.SerialNo + " " + x.UnitNum
            }).ToList();
            ViewData["branchh"] = db.Branch
     .Select(x => new SelectListItem
     {
         Text = x.Name,
         Value = x.BranchId.ToString()
     }).ToList();
            ViewData["branch"] = CacheData.GetBranchCaches().Select(
    i => new
    {
        Name = i.Name,
        BranchId = i.Id
    }).AsQueryable();
            var fetchSettingg = db.RequestToInspections.FirstOrDefault();
            string isECWCC = "Not";
            if (fetchSettingg != null)
            {

                isECWCC = fetchSettingg.Remark;
            }
            ViewBag.IsMIDROC = isECWCC;
            ViewData["operator"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.FullName
            });
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name });
            
            return View();
        }
        //public JsonResult RepopulateTimeSheet()
        //{
        //    var timesheets = db.MachineryWorkingTimeSheets.ToList();
        //    if (timesheets.Any())
        //    {
        //        foreach (var item in timesheets)
        //        {
        //            item.EquipmentCategoryId = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID)?.EquipmentIdentityID;
        //            item.EquipmentTypeId = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID)?.EquipmentTypeID;
        //            item.EquipmentNameId = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID)?.EquipmentNameID;
        //            db.Entry(item).State = EntityState.Modified;
        //        }
        //        db.SaveChanges();
        //    }
        //    return Json("success", JsonRequestBehavior.AllowGet);
        //}
        public ActionResult MachineryWorkingTimeSheets_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var machineryWorkingTimeSheets = db.MachineryWorkingTimeSheets.Where(x => mybraches.Contains(x.BranchId));           
            DataSourceResult result = machineryWorkingTimeSheets.ToDataSourceResult(request, machineryWorkingTimeSheet => new MachineryWorkingTimeSheet
            {
                
                FleetsID = machineryWorkingTimeSheet.FleetsID,
                MachineryWorkingTimeSheetId = machineryWorkingTimeSheet.MachineryWorkingTimeSheetId,
                Date= machineryWorkingTimeSheet.Date,
                MorningTimeStart= machineryWorkingTimeSheet.MorningTimeStart,
                MorningTimeEnd= machineryWorkingTimeSheet.MorningTimeEnd,
                AfternoonTimeStart= machineryWorkingTimeSheet.AfternoonTimeStart,
                AfternoonTimeEnd = machineryWorkingTimeSheet.AfternoonTimeEnd,
                NightTimeStart= machineryWorkingTimeSheet.NightTimeStart,
                NightTimeEnd= machineryWorkingTimeSheet.NightTimeEnd,
                NumberOfHoles= machineryWorkingTimeSheet.NumberOfHoles,
                Unit = machineryWorkingTimeSheet.Unit,
                Volume = machineryWorkingTimeSheet.Volume,
                HoleDepth= machineryWorkingTimeSheet.HoleDepth,
                FuelConsumption= machineryWorkingTimeSheet.FuelConsumption,
                FuelRate = machineryWorkingTimeSheet.FuelRate,
                IdleHours= machineryWorkingTimeSheet.IdleHours,
                IdleReason= machineryWorkingTimeSheet.IdleReason,
                DownHours= machineryWorkingTimeSheet.DownHours,
                DownReason= machineryWorkingTimeSheet.DownReason,
                NetHour= machineryWorkingTimeSheet.NetHour,
                WorkType= machineryWorkingTimeSheet.WorkType,
                Remark = machineryWorkingTimeSheet.Remark,
                BatteryCost = machineryWorkingTimeSheet.BatteryCost,

                FuelType = machineryWorkingTimeSheet.FuelType,
                OilType = machineryWorkingTimeSheet.OilType,
                ExternalOperatorName = machineryWorkingTimeSheet.ExternalOperatorName,
                RentalRate = machineryWorkingTimeSheet.RentalRate,
                IdealRentalRate = machineryWorkingTimeSheet.IdealRentalRate,
                ProcurementSupplierID = machineryWorkingTimeSheet.ProcurementSupplierID,
                OperationAmount = machineryWorkingTimeSheet.OperationAmount,
                IdleRate = machineryWorkingTimeSheet.IdleRate,
                IdleAmount = machineryWorkingTimeSheet.IdleAmount,
                TotalAmountBeforeVAT = machineryWorkingTimeSheet.TotalAmountBeforeVAT,
                VAT = machineryWorkingTimeSheet.VAT,
               
                TotalAmount = machineryWorkingTimeSheet.TotalAmount,
                FuelCost = machineryWorkingTimeSheet.FuelCost,
                ServiceRecharge = machineryWorkingTimeSheet.ServiceRecharge,
                SparePartCost = machineryWorkingTimeSheet.SparePartCost,
                OilCost = machineryWorkingTimeSheet.OilCost,
                GreaseConsumption = machineryWorkingTimeSheet.GreaseConsumption,
                GreaseCost = machineryWorkingTimeSheet.GreaseCost,
                MaintenanceCost = machineryWorkingTimeSheet.MaintenanceCost,
                OilConsumption = machineryWorkingTimeSheet.OilConsumption,
                TotalCost = machineryWorkingTimeSheet.TotalCost,
                NetAmount = machineryWorkingTimeSheet.NetAmount,
                DownHourRate = machineryWorkingTimeSheet.DownHourRate,
                OperationRevenue = machineryWorkingTimeSheet.OperationRevenue,
                Perdium = machineryWorkingTimeSheet.Perdium,
                Salary = machineryWorkingTimeSheet.Salary,
                TireCost = machineryWorkingTimeSheet.TireCost,
                ReasonForDown = machineryWorkingTimeSheet.ReasonForDown,
                ReasonForIdle = machineryWorkingTimeSheet.ReasonForIdle,
                BranchId = machineryWorkingTimeSheet.BranchId,
                BusinessUnitId = machineryWorkingTimeSheet.BusinessUnitId,
                TimeSheetStatus = machineryWorkingTimeSheet.TimeSheetStatus,
                CostSheetApproved = machineryWorkingTimeSheet.CostSheetApproved,
                CostSheetApprovedBy = machineryWorkingTimeSheet.CostSheetApprovedBy,
                CostSheetApprovedDate = machineryWorkingTimeSheet.CostSheetApprovedDate,
                CostSheetSendForAproval = machineryWorkingTimeSheet.CostSheetSendForAproval,
                CreatedBy = machineryWorkingTimeSheet.CreatedBy,
                DateCreated = machineryWorkingTimeSheet.DateCreated,
                LastModified = machineryWorkingTimeSheet.LastModified,
                ModifiedBy = machineryWorkingTimeSheet.ModifiedBy,
                TimeSheetApproved = machineryWorkingTimeSheet.TimeSheetApproved,
                TimeSheetSendForAproval = machineryWorkingTimeSheet.TimeSheetSendForAproval,
                TimeSheetApprovedBy = machineryWorkingTimeSheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = machineryWorkingTimeSheet.TimeSheetApprovedDate,
                TimeSheetSendForApprovalBy = machineryWorkingTimeSheet.TimeSheetSendForApprovalBy,
                TimeSheetSendForApprovalTime = machineryWorkingTimeSheet.TimeSheetSendForApprovalTime,
                OperatorId = machineryWorkingTimeSheet.OperatorId,
                GreaseRate = machineryWorkingTimeSheet.GreaseRate,
                EquipmentCategoryId = machineryWorkingTimeSheet.EquipmentCategoryId,
                EquipmentTypeId = machineryWorkingTimeSheet.EquipmentTypeId,
                EquipmentNameId = machineryWorkingTimeSheet.EquipmentNameId,
                FuelKMR = machineryWorkingTimeSheet.FuelKMR,
                DifferenceEHR = machineryWorkingTimeSheet.DifferenceEHR,
                FinalEHR = machineryWorkingTimeSheet.FinalEHR,
                TotalTime = machineryWorkingTimeSheet.TotalTime,
                InitialEHR = machineryWorkingTimeSheet.InitialEHR


            });
            return Json(result);
        }
        private TimeSpan GetTotalTime(DateTime a, DateTime b,
                              DateTime c, DateTime d,
                              DateTime e, DateTime f)
        {
            return (b - a) + (d - c) + (f - e);
        }
        public ActionResult MachineryWorkingTimeSheets_Create([DataSourceRequest] DataSourceRequest request, MachineryWorkingTimeSheet machineryWorkingTimeSheet)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == machineryWorkingTimeSheet.FleetsID);
            machineryWorkingTimeSheet.EquipmentCategoryId = fleet.EquipmentIdentityID;
            machineryWorkingTimeSheet.EquipmentTypeId = fleet.EquipmentTypeID;
            machineryWorkingTimeSheet.EquipmentNameId = fleet.EquipmentNameID;
            #region valiadtion
            #endregion
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
                            == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date)
                        && OilLubItems.Contains(ii.ItemCode)).ToList();
                machineryWorkingTimeSheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                machineryWorkingTimeSheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                            == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date)
                        && oilItems.Contains(ii.ItemCode)).ToList();
                machineryWorkingTimeSheet.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                machineryWorkingTimeSheet.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                            == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date)
                        && allSparePartItems.Contains(ii.ItemCode)).ToList();
                var issuedBatteryItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date)
                        && allBatteryItems.Contains(ii.ItemCode)).ToList();
                var issuedTyreItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date)
                        && allTyreItems.Contains(ii.ItemCode)).ToList();
                machineryWorkingTimeSheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                machineryWorkingTimeSheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                machineryWorkingTimeSheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == fleet.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date));
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
                machineryWorkingTimeSheet.MaintenanceCost += maintenancePayment;
            }
            #region data
            var operationAmount = machineryWorkingTimeSheet.RentalRate * machineryWorkingTimeSheet.NetHour;
            var idleAmount = machineryWorkingTimeSheet.IdleRate * machineryWorkingTimeSheet.IdleHours;
            var totalAmountBeforeVAT = operationAmount + idleAmount;
            var totalAmount = totalAmountBeforeVAT * machineryWorkingTimeSheet.VAT;
            var fuelCost = machineryWorkingTimeSheet.FuelConsumption * machineryWorkingTimeSheet.FuelRate;
            var totalCost = fuelCost + machineryWorkingTimeSheet.MaintenanceCost + machineryWorkingTimeSheet.GreaseCost + machineryWorkingTimeSheet.ServiceRecharge;

            #endregion
            
            var timeSheets = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.FleetsID == machineryWorkingTimeSheet.FleetsID && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date));
            if (timeSheets != null)
            {
                ModelState.AddModelError("duplicate", "Timesheet for this Plate On the selected day is already added");
            }
            var fetchSettings = db.TimeSheetSetting.ToList();

            if (fetchSettings.Any())
            {
                var fetchSetting = fetchSettings.First();
                var fetchException = db.TimeSheetException.FirstOrDefault(x => x.TimeSheetSettingId == fetchSetting.TimeSheetSettingId && x.BranchId == machineryWorkingTimeSheet.BranchId.ToString());
                if (fetchException == null)
                {
                    if (fetchSetting != null && fetchSetting.IsSet == true)
                    {
                        int hoursDifference = (int)(DateTime.Now - machineryWorkingTimeSheet.Date).TotalHours;

                        if (fetchSetting.SettedHours < hoursDifference)
                        {
                            ModelState.AddModelError("date expired", "The timesheet recording date is passed");
                        }
                    }
                }
            }
            machineryWorkingTimeSheet.NetHour =
                    (decimal)GetTotalTime(
                        machineryWorkingTimeSheet.MorningTimeStart,
                        machineryWorkingTimeSheet.MorningTimeEnd,
                        machineryWorkingTimeSheet.AfternoonTimeStart,
                        machineryWorkingTimeSheet.AfternoonTimeEnd,
                        machineryWorkingTimeSheet.NightTimeStart,
                        machineryWorkingTimeSheet.NightTimeEnd
                    ).TotalHours;
            machineryWorkingTimeSheet.TotalTime = machineryWorkingTimeSheet.NetHour;
            var requestToInspection = db.RequestToInspections.FirstOrDefault();
            var lastTimesheet = db.WagonDrillTimesheets.Where(x => x.FleetsID == machineryWorkingTimeSheet.FleetsID).OrderByDescending(x => x.WagonDrillTimesheetId).FirstOrDefault();
            if (requestToInspection != null && requestToInspection.Remark == "MCG")
            {

                if (machineryWorkingTimeSheet.InitialEHR < 0 || machineryWorkingTimeSheet.FinalEHR < 0 || (machineryWorkingTimeSheet.FinalEHR - machineryWorkingTimeSheet.InitialEHR) < 0 || machineryWorkingTimeSheet.FuelKMR < 0)
                {
                    ModelState.AddModelError("invalid", "Invalid EHR entry");
                }
                machineryWorkingTimeSheet.DownReason = machineryWorkingTimeSheet.ReasonForDown.ToString();
                machineryWorkingTimeSheet.IdleReason = machineryWorkingTimeSheet.ReasonForIdle.ToString();
            }
            fleet.BranchId = machineryWorkingTimeSheet.BranchId;
            db.Entry(fleet).State = EntityState.Modified;
            db.SaveChanges();
            if (ModelState.IsValid)
            {
                var entity = new MachineryWorkingTimeSheet
                {
                    FleetsID = machineryWorkingTimeSheet.FleetsID,
                    MachineryWorkingTimeSheetId = machineryWorkingTimeSheet.MachineryWorkingTimeSheetId,
                    Date = machineryWorkingTimeSheet.Date,
                    MorningTimeStart = machineryWorkingTimeSheet.MorningTimeStart,
                    MorningTimeEnd = machineryWorkingTimeSheet.MorningTimeEnd,
                    AfternoonTimeStart = machineryWorkingTimeSheet.AfternoonTimeStart,
                    AfternoonTimeEnd = machineryWorkingTimeSheet.AfternoonTimeEnd,
                    NightTimeStart = machineryWorkingTimeSheet.NightTimeStart,
                    NightTimeEnd = machineryWorkingTimeSheet.NightTimeEnd,
                    NumberOfHoles = machineryWorkingTimeSheet.NumberOfHoles,
                    Unit = fleet.Unit,
                    Volume = fleet.Volume,

                    TotalTime = machineryWorkingTimeSheet.TotalTime,
                    SparePartCost = machineryWorkingTimeSheet.SparePartCost,
                    OilCost = machineryWorkingTimeSheet.OilCost,
                    GreaseConsumption = machineryWorkingTimeSheet.GreaseConsumption,
                    GreaseCost = machineryWorkingTimeSheet.GreaseCost,
                    MaintenanceCost = machineryWorkingTimeSheet.MaintenanceCost,
                    OilConsumption = machineryWorkingTimeSheet.OilConsumption,

                    HoleDepth = machineryWorkingTimeSheet.HoleDepth,
                    FuelConsumption = machineryWorkingTimeSheet.FuelConsumption,
                    FuelRate = machineryWorkingTimeSheet.FuelRate,
                    IdleHours = machineryWorkingTimeSheet.IdleHours,
                    IdleReason = machineryWorkingTimeSheet.IdleReason,
                    FuelType = machineryWorkingTimeSheet.FuelType,
                    RentalAgreementNum = fleet.RentFromExternalNum,
                    ReasonForDown = machineryWorkingTimeSheet.ReasonForDown,
                    ReasonForIdle = machineryWorkingTimeSheet.ReasonForIdle,
                    OilType = machineryWorkingTimeSheet.OilType,
                    DownHours = machineryWorkingTimeSheet.DownHours,
                    DownReason = machineryWorkingTimeSheet.DownReason,
                    NetHour = machineryWorkingTimeSheet.NetHour,
                    WorkType = machineryWorkingTimeSheet.WorkType,
                    Remark = machineryWorkingTimeSheet.Remark,
                    RentalRate = machineryWorkingTimeSheet.RentalRate,
                    IdealRentalRate = machineryWorkingTimeSheet.IdealRentalRate,
                    ProcurementSupplierID = machineryWorkingTimeSheet.ProcurementSupplierID,
                    OperationAmount = operationAmount,
                    IdleRate = machineryWorkingTimeSheet.IdleRate,
                    IdleAmount = idleAmount,
                    ExternalOperatorName = machineryWorkingTimeSheet.ExternalOperatorName,
                    TotalAmountBeforeVAT = totalAmountBeforeVAT,
                    VAT = machineryWorkingTimeSheet.VAT,
                    TotalAmount = totalAmount,
                    FuelCost = fuelCost,
                    ServiceRecharge = machineryWorkingTimeSheet.ServiceRecharge,
                    TotalCost = totalCost,
                    NetAmount = totalAmount-totalCost,
                    DownHourRate = machineryWorkingTimeSheet.DownHourRate,
                    OperationRevenue = machineryWorkingTimeSheet.OperationRevenue,
                    Perdium = machineryWorkingTimeSheet.Perdium,
                    Salary = machineryWorkingTimeSheet.Salary,
                    TireCost = machineryWorkingTimeSheet.TireCost,
                    BranchId = machineryWorkingTimeSheet.BranchId,
                    BusinessUnitId = machineryWorkingTimeSheet.BusinessUnitId,
                    OperatorId = machineryWorkingTimeSheet.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    GreaseRate = machineryWorkingTimeSheet.GreaseRate,
                    EquipmentCategoryId = machineryWorkingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = machineryWorkingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = machineryWorkingTimeSheet.EquipmentNameId
                };
                entity.InitialEHR = machineryWorkingTimeSheet.InitialEHR;
                entity.FinalEHR = machineryWorkingTimeSheet.FinalEHR;
                entity.DifferenceEHR = machineryWorkingTimeSheet.FinalEHR - machineryWorkingTimeSheet.InitialEHR;
                entity.FuelKMR = machineryWorkingTimeSheet.FuelKMR;
                
                db.MachineryWorkingTimeSheets.Add(entity);
                db.SaveChanges();

                //var fleetSetting = db.RequestToInspections.FirstOrDefault(x => x.Remark != null);

                //if (fleetSetting != null && fleetSetting.Remark == "MIDROC")
                //{
                //    List<MachineryWorkingTimeSheet> timeSheet = db.MachineryWorkingTimeSheets
                //        .Where(x => x.BranchId == entity.BranchId &&
                //                    x.EquipmentCategoryId == entity.EquipmentCategoryId && x.FleetsID != entity.FleetsID)
                //        .GroupBy(x => x.FleetsID)
                //        .Select(g => g.OrderByDescending(x => x.MachineryWorkingTimeSheetId).FirstOrDefault())
                //        .ToList();
                //    var firstObject = timeSheet.First();
                //    var checkDuplicatee = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.FleetsID == firstObject.FleetsID && x.Date == firstObject.Date);
                //    if (checkDuplicatee == null)
                //    {
                //        foreach (var timesht in timeSheet)
                //        {
                //            var newTimeSheet = new MachineryWorkingTimeSheet
                //            {
                //                FleetsID = timesht.FleetsID,
                //                Date = timesht.Date,
                //                MorningTimeStart = timesht.MorningTimeStart,
                //                MorningTimeEnd = timesht.MorningTimeEnd,
                //                AfternoonTimeStart = timesht.AfternoonTimeStart,
                //                AfternoonTimeEnd = timesht.AfternoonTimeEnd,
                //                NightTimeStart = timesht.NightTimeStart,
                //                NightTimeEnd = timesht.NightTimeEnd,
                //                NumberOfHoles = timesht.NumberOfHoles,
                //                HoleDepth = timesht.HoleDepth,
                //                FuelConsumption = timesht.FuelConsumption,
                //                FuelRate = timesht.FuelRate,
                //                GreaseConsumption = timesht.GreaseConsumption,
                //                IdleHours = timesht.IdleHours,
                //                IdleReason = timesht.IdleReason,
                //                FuelType = timesht.FuelType,
                //                OilType = timesht.OilType,
                //                DownHours = timesht.DownHours,
                //                DownReason = timesht.DownReason,
                //                NetHour = timesht.NetHour,
                //                WorkType = timesht.WorkType,
                //                Remark = timesht.Remark,
                //                RentalRate = timesht.RentalRate,
                //                IdealRentalRate = timesht.IdealRentalRate,
                //                ProcurementSupplierID = timesht.ProcurementSupplierID,
                //                OperationAmount = timesht.OperationAmount,
                //                IdleRate = timesht.IdleRate,
                //                IdleAmount = timesht.IdleAmount,
                //                TotalAmountBeforeVAT = timesht.TotalAmountBeforeVAT,
                //                VAT = timesht.VAT,
                //                TotalAmount = timesht.TotalAmount,
                //                FuelCost = timesht.FuelCost,
                //                ServiceRecharge = timesht.ServiceRecharge,
                //                LubricantCost = timesht.LubricantCost,
                //                MaintenanceCost = timesht.MaintenanceCost,
                //                TotalCost = timesht.TotalCost,
                //                NetAmount = timesht.TotalAmount - timesht.TotalCost,
                //                OilConsumption = timesht.OilConsumption,
                //                DownHourRate = timesht.DownHourRate,
                //                OperationRevenue = timesht.OperationRevenue,
                //                Perdium = timesht.Perdium,
                //                Salary = timesht.Salary,
                //                TireCost = timesht.TireCost,
                //                BranchId = timesht.BranchId,
                //                BusinessUnitId = timesht.BusinessUnitId,
                //                OperatorId = timesht.OperatorId,
                //                TimeSheetStatus = MaintenanceConst.Pending,
                //                CreatedBy = User.Identity.Name,
                //                DateCreated = DateTime.Today,
                //                GreaseRate = timesht.GreaseRate,
                //                EquipmentCategoryId = timesht.EquipmentCategoryId,
                //                EquipmentTypeId = timesht.EquipmentTypeId,
                //                EquipmentNameId = timesht.EquipmentNameId
                //            };

                //            db.MachineryWorkingTimeSheets.Add(newTimeSheet);
                //            db.SaveChanges();
                //        }
                //    }
                //}

                machineryWorkingTimeSheet.MachineryWorkingTimeSheetId = entity.MachineryWorkingTimeSheetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Machinery working timesheet", "Success", User.Identity.Name, $"created machinery working timesheet {entity.MachineryWorkingTimeSheetId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { machineryWorkingTimeSheet }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MachineryWorkingTimeSheets_Update([DataSourceRequest] DataSourceRequest request, MachineryWorkingTimeSheet machineryWorkingTimeSheet)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == machineryWorkingTimeSheet.FleetsID);
            machineryWorkingTimeSheet.EquipmentCategoryId = fleet.EquipmentIdentityID;
            machineryWorkingTimeSheet.EquipmentTypeId = fleet.EquipmentTypeID;
            machineryWorkingTimeSheet.EquipmentNameId = fleet.EquipmentNameID;

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
                            == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date)
                        && OilLubItems.Contains(ii.ItemCode)).ToList();
                machineryWorkingTimeSheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                machineryWorkingTimeSheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                            == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date)
                        && oilItems.Contains(ii.ItemCode)).ToList();
                machineryWorkingTimeSheet.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                machineryWorkingTimeSheet.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                            == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date)
                        && allSparePartItems.Contains(ii.ItemCode)).ToList();
                var issuedBatteryItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date)
                        && allBatteryItems.Contains(ii.ItemCode)).ToList();
                var issuedTyreItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date)
                        && allTyreItems.Contains(ii.ItemCode)).ToList();
                machineryWorkingTimeSheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                machineryWorkingTimeSheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                machineryWorkingTimeSheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == fleet.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date));
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
                machineryWorkingTimeSheet.MaintenanceCost += maintenancePayment;
            }
            #region data
            var operationAmount = machineryWorkingTimeSheet.RentalRate * machineryWorkingTimeSheet.NetHour;
            var idleAmount = machineryWorkingTimeSheet.IdleRate * machineryWorkingTimeSheet.IdleHours;
            var totalAmountBeforeVAT = operationAmount + idleAmount;
            var totalAmount = totalAmountBeforeVAT * machineryWorkingTimeSheet.VAT;
            var fuelCost = machineryWorkingTimeSheet.FuelConsumption * machineryWorkingTimeSheet.FuelRate;
            var totalCost = fuelCost + machineryWorkingTimeSheet.MaintenanceCost + machineryWorkingTimeSheet.GreaseCost + machineryWorkingTimeSheet.ServiceRecharge;


            TimeSpan morningTime = machineryWorkingTimeSheet.MorningTimeEnd - machineryWorkingTimeSheet.MorningTimeStart;
            TimeSpan afternoonTime = machineryWorkingTimeSheet.AfternoonTimeEnd - machineryWorkingTimeSheet.AfternoonTimeStart;
            TimeSpan nightTime = machineryWorkingTimeSheet.NightTimeEnd - machineryWorkingTimeSheet.NightTimeStart;

            if (morningTime < TimeSpan.Zero && afternoonTime < TimeSpan.Zero && nightTime < TimeSpan.Zero)
            {
                // Convert time differences to decimal format
                double morningDecimal = morningTime.TotalHours;
                double afternoonDecimal = afternoonTime.TotalHours;
                double nightDecimal = nightTime.TotalHours;

                Console.WriteLine("Morning Time (decimal): " + morningDecimal);
                Console.WriteLine("Afternoon Time (decimal): " + afternoonDecimal);
                Console.WriteLine("Night Time (decimal): " + nightDecimal);
            }

            #endregion
            var timeSheets = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.FleetsID == machineryWorkingTimeSheet.FleetsID&&x.MachineryWorkingTimeSheetId!=machineryWorkingTimeSheet.MachineryWorkingTimeSheetId && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(machineryWorkingTimeSheet.Date));
            if (timeSheets != null)
            {
                ModelState.AddModelError("duplicate", "Timesheet for this Plate On the selected day is already added");
            }
            var requestToInspection = db.RequestToInspections.FirstOrDefault();
            var lastTimesheet = db.WagonDrillTimesheets.Where(x => x.FleetsID == machineryWorkingTimeSheet.FleetsID).OrderByDescending(x => x.WagonDrillTimesheetId).FirstOrDefault();
            if (requestToInspection != null && requestToInspection.Remark == "MCG")
            {

                if (machineryWorkingTimeSheet.InitialEHR < 0 || machineryWorkingTimeSheet.FinalEHR < 0 || (machineryWorkingTimeSheet.FinalEHR - machineryWorkingTimeSheet.InitialEHR) < 0 || machineryWorkingTimeSheet.FuelKMR < 0)
                {
                    ModelState.AddModelError("invalid", "Invalid EHR entry");
                }
                machineryWorkingTimeSheet.DownReason = machineryWorkingTimeSheet.ReasonForDown.ToString();
                machineryWorkingTimeSheet.IdleReason = machineryWorkingTimeSheet.ReasonForIdle.ToString();
                machineryWorkingTimeSheet.DifferenceEHR = machineryWorkingTimeSheet.FinalEHR - machineryWorkingTimeSheet.InitialEHR;
            }
            machineryWorkingTimeSheet.NetHour =
                    (decimal)GetTotalTime(
                        machineryWorkingTimeSheet.MorningTimeStart,
                        machineryWorkingTimeSheet.MorningTimeEnd,
                        machineryWorkingTimeSheet.AfternoonTimeStart,
                        machineryWorkingTimeSheet.AfternoonTimeEnd,
                        machineryWorkingTimeSheet.NightTimeStart,
                        machineryWorkingTimeSheet.NightTimeEnd
                    ).TotalHours;
            machineryWorkingTimeSheet.TotalTime = machineryWorkingTimeSheet.NetHour;
            if (ModelState.IsValid)
            {
                var entity = new MachineryWorkingTimeSheet
                {

                    FleetsID = machineryWorkingTimeSheet.FleetsID,
                    MachineryWorkingTimeSheetId = machineryWorkingTimeSheet.MachineryWorkingTimeSheetId,
                    Date = machineryWorkingTimeSheet.Date,
                    MorningTimeStart = machineryWorkingTimeSheet.MorningTimeStart,
                    MorningTimeEnd = machineryWorkingTimeSheet.MorningTimeEnd,
                    AfternoonTimeStart = machineryWorkingTimeSheet.AfternoonTimeStart,
                    AfternoonTimeEnd = machineryWorkingTimeSheet.AfternoonTimeEnd,
                    NightTimeStart = machineryWorkingTimeSheet.NightTimeStart,
                    NightTimeEnd = machineryWorkingTimeSheet.NightTimeEnd,
                    NumberOfHoles = machineryWorkingTimeSheet.NumberOfHoles,
                    HoleDepth = machineryWorkingTimeSheet.HoleDepth,
                    FuelConsumption = machineryWorkingTimeSheet.FuelConsumption,
                    FuelRate = machineryWorkingTimeSheet.FuelRate,
                    IdleHours = machineryWorkingTimeSheet.IdleHours,
                    IdleReason = machineryWorkingTimeSheet.IdleReason,
                    DownHours = machineryWorkingTimeSheet.DownHours,
                    DownReason = machineryWorkingTimeSheet.DownReason,
                    ReasonForDown = machineryWorkingTimeSheet.ReasonForDown,
                    ReasonForIdle = machineryWorkingTimeSheet.ReasonForIdle,
                    NetHour = machineryWorkingTimeSheet.NetHour,
                    WorkType = machineryWorkingTimeSheet.WorkType,
                    Remark = machineryWorkingTimeSheet.Remark,
                    RentalRate = machineryWorkingTimeSheet.RentalRate,
                    IdealRentalRate = machineryWorkingTimeSheet.IdealRentalRate,
                    ProcurementSupplierID = machineryWorkingTimeSheet.ProcurementSupplierID,
                    OperationAmount = operationAmount,

                    SparePartCost = machineryWorkingTimeSheet.SparePartCost,
                    OilCost = machineryWorkingTimeSheet.OilCost,
                    GreaseConsumption = machineryWorkingTimeSheet.GreaseConsumption,
                    GreaseCost = machineryWorkingTimeSheet.GreaseCost,
                    MaintenanceCost = machineryWorkingTimeSheet.MaintenanceCost,
                    OilConsumption = machineryWorkingTimeSheet.OilConsumption,
                    RentalAgreementNum = fleet.RentFromExternalNum,
                    IdleRate = machineryWorkingTimeSheet.IdleRate,
                    FuelType = machineryWorkingTimeSheet.FuelType,
                    OilType = machineryWorkingTimeSheet.OilType,
                    ExternalOperatorName = machineryWorkingTimeSheet.ExternalOperatorName,
                    IdleAmount = idleAmount,
                    TotalAmountBeforeVAT = totalAmountBeforeVAT,
                    VAT = machineryWorkingTimeSheet.VAT,
                    TotalAmount = totalAmount,
                    FuelCost = fuelCost,
                    ServiceRecharge = machineryWorkingTimeSheet.ServiceRecharge,
                    TotalCost = totalCost,
                    NetAmount = totalAmount - totalCost,

                    DownHourRate = machineryWorkingTimeSheet.DownHourRate,
                    OperationRevenue = machineryWorkingTimeSheet.OperationRevenue,
                    Perdium = machineryWorkingTimeSheet.Perdium,
                    Salary = machineryWorkingTimeSheet.Salary,
                    TireCost = machineryWorkingTimeSheet.TireCost,
                    BranchId = machineryWorkingTimeSheet.BranchId,
                    BusinessUnitId = machineryWorkingTimeSheet.BusinessUnitId,
                    OperatorId = machineryWorkingTimeSheet.OperatorId,
                    GreaseRate = machineryWorkingTimeSheet.GreaseRate,
                    EquipmentCategoryId = machineryWorkingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = machineryWorkingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = machineryWorkingTimeSheet.EquipmentNameId,
                    FuelKMR = machineryWorkingTimeSheet.FuelKMR,
                    DifferenceEHR = machineryWorkingTimeSheet.DifferenceEHR,
                    FinalEHR = machineryWorkingTimeSheet.FinalEHR,
                    InitialEHR = machineryWorkingTimeSheet.InitialEHR
                };
                entity.InitialEHR = machineryWorkingTimeSheet.InitialEHR;
                entity.FinalEHR = machineryWorkingTimeSheet.FinalEHR;
                entity.DifferenceEHR = machineryWorkingTimeSheet.FinalEHR - machineryWorkingTimeSheet.InitialEHR;
                entity.FuelKMR = machineryWorkingTimeSheet.FuelKMR;
                //db.MachineryWorkingTimeSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Working machinery registration", "Success", User.Identity.Name, $"updated machinery working timesheet {entity.MachineryWorkingTimeSheetId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { machineryWorkingTimeSheet }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult InsuranceCompanies_Destroy([DataSourceRequest] DataSourceRequest request, MachineryWorkingTimeSheet machineryWorkingTimeSheet)
        {

            if (ModelState.IsValid)
            {
                var entity = new MachineryWorkingTimeSheet
                {
                    FleetsID = machineryWorkingTimeSheet.FleetsID,
                    MachineryWorkingTimeSheetId = machineryWorkingTimeSheet.MachineryWorkingTimeSheetId,
                    Date = machineryWorkingTimeSheet.Date,
                    MorningTimeStart = machineryWorkingTimeSheet.MorningTimeStart,
                    MorningTimeEnd = machineryWorkingTimeSheet.MorningTimeEnd,
                    AfternoonTimeStart = machineryWorkingTimeSheet.AfternoonTimeStart,
                    AfternoonTimeEnd = machineryWorkingTimeSheet.AfternoonTimeEnd,
                    NightTimeStart = machineryWorkingTimeSheet.NightTimeStart,
                    NightTimeEnd = machineryWorkingTimeSheet.NightTimeEnd,
                    NumberOfHoles = machineryWorkingTimeSheet.NumberOfHoles,
                    HoleDepth = machineryWorkingTimeSheet.HoleDepth,
                    FuelConsumption = machineryWorkingTimeSheet.FuelConsumption,
                    FuelRate = machineryWorkingTimeSheet.FuelRate,
                    GreaseConsumption = machineryWorkingTimeSheet.GreaseConsumption,
                    IdleHours = machineryWorkingTimeSheet.IdleHours,
                    IdleReason = machineryWorkingTimeSheet.IdleReason,
                    DownHours = machineryWorkingTimeSheet.DownHours,
                    DownReason = machineryWorkingTimeSheet.DownReason,
                    NetHour = machineryWorkingTimeSheet.NetHour,
                    WorkType = machineryWorkingTimeSheet.WorkType,
                    Remark = machineryWorkingTimeSheet.Remark,
                    RentalRate = machineryWorkingTimeSheet.RentalRate,
                    FuelType = machineryWorkingTimeSheet.FuelType,
                    OilType = machineryWorkingTimeSheet.OilType,
                    IdealRentalRate = machineryWorkingTimeSheet.IdealRentalRate,
                    ProcurementSupplierID = machineryWorkingTimeSheet.ProcurementSupplierID,
                    OperationAmount = machineryWorkingTimeSheet.OperationAmount,
                    IdleRate = machineryWorkingTimeSheet.IdleRate,
                    IdleAmount = machineryWorkingTimeSheet.IdleAmount,
                    TotalAmountBeforeVAT = machineryWorkingTimeSheet.TotalAmountBeforeVAT,
                    VAT = machineryWorkingTimeSheet.VAT,
                    TotalAmount = machineryWorkingTimeSheet.TotalAmount,
                    FuelCost = machineryWorkingTimeSheet.FuelCost,
                    ServiceRecharge = machineryWorkingTimeSheet.ServiceRecharge,
                    MaintenanceCost = machineryWorkingTimeSheet.MaintenanceCost,
                    TotalCost = machineryWorkingTimeSheet.TotalCost,
                    NetAmount = machineryWorkingTimeSheet.NetAmount,

                    DownHourRate = machineryWorkingTimeSheet.DownHourRate,
                    OperationRevenue = machineryWorkingTimeSheet.OperationRevenue,
                    Perdium = machineryWorkingTimeSheet.Perdium,
                    Salary = machineryWorkingTimeSheet.Salary,
                    TireCost = machineryWorkingTimeSheet.TireCost,

                    OperatorId = machineryWorkingTimeSheet.OperatorId,
                    GreaseRate = machineryWorkingTimeSheet.GreaseRate,
                    EquipmentCategoryId = machineryWorkingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = machineryWorkingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = machineryWorkingTimeSheet.EquipmentNameId
                };

                db.MachineryWorkingTimeSheets.Attach(entity);
                db.MachineryWorkingTimeSheets.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted working machinery timesheet {entity.MachineryWorkingTimeSheetId }");
                #endregion
            }

            return Json(new[] { machineryWorkingTimeSheet }.ToDataSourceResult(request, ModelState));
        }
        public JsonResult GetFleetValues(Guid fleetId)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetId);
            if (fleet != null)
            {
                return Json(fleet, JsonRequestBehavior.AllowGet);
            }
            return Json(null);
        }



        #region Approval
        public JsonResult TimeSheetSendForApproval(int machineryWorkingTimeSheetId)
        {

            var machineryTimeSheet = db.MachineryWorkingTimeSheets.Find(machineryWorkingTimeSheetId);

            if (machineryTimeSheet != null)
            {
                machineryTimeSheet.TimeSheetSendForAproval = true;
                machineryTimeSheet.TimeSheetSendForApprovalTime = DateTime.Now;
                machineryTimeSheet.TimeSheetSendForApprovalBy = User.Identity.Name;
                machineryTimeSheet.TimeSheetStatus = MaintenanceConst.WaitingApproval;

                db.MachineryWorkingTimeSheets.Attach(machineryTimeSheet);
                db.Entry(machineryTimeSheet).State = EntityState.Modified;
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
        public JsonResult voidRecord(int machineryWorkingTimeSheetId)
        {

            var machineryTimeSheet = db.MachineryWorkingTimeSheets.Find(machineryWorkingTimeSheetId);

            if (machineryTimeSheet != null)
            {
                machineryTimeSheet.TimeSheetSendForAproval = false;
                machineryTimeSheet.TimeSheetApproved = false;
                machineryTimeSheet.TimeSheetApprovedDate = DateTime.Now;
                machineryTimeSheet.TimeSheetApprovedBy = User.Identity.Name;
                machineryTimeSheet.TimeSheetStatus = MaintenanceConst.Rejected;

                db.MachineryWorkingTimeSheets.Attach(machineryTimeSheet);
                db.Entry(machineryTimeSheet).State = EntityState.Modified;
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
        public ActionResult MachineryTimeSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int id)
        {

            var approval = db.TimeSheetApprovalNew.Where(x => x.TimeSheetId == id && x.TimeSheetType == TimeSheetType.Machinery);
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
        public ActionResult MachineryTimeSheetApproval_Create([DataSourceRequest] DataSourceRequest request, TimeSheetApprovalNew model, int id)
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
                model.TimeSheetType = TimeSheetType.Machinery;
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var machineryTimeSheet = db.MachineryWorkingTimeSheets.Find(id);
            if (machineryTimeSheet != null)
            {
                machineryTimeSheet.TimeSheetApproved = true;
                machineryTimeSheet.TimeSheetStatus = MaintenanceConst.Approved;
                machineryTimeSheet.TimeSheetApprovedBy = User.Identity.Name;
                machineryTimeSheet.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(machineryTimeSheet).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var machineryTimeSheet = db.MachineryWorkingTimeSheets.Find(id);
            if (machineryTimeSheet != null)
            {
                machineryTimeSheet.TimeSheetApproved = false;
                machineryTimeSheet.TimeSheetSendForAproval = false;
                machineryTimeSheet.TimeSheetStatus = MaintenanceConst.Rejected;
                machineryTimeSheet.TimeSheetApprovedBy = User.Identity.Name;
                machineryTimeSheet.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(machineryTimeSheet).State = EntityState.Modified;
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

            var records = db.MachineryWorkingTimeSheets
                            .Where(x => ids.Contains(x.MachineryWorkingTimeSheetId) && x.TimeSheetSendForAproval == true)
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
                model.TimeSheetId = item.MachineryWorkingTimeSheetId;
                model.TimeSheetType = TimeSheetType.Machinery;
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

            var records = db.MachineryWorkingTimeSheets
                            .Where(x => ids.Contains(x.MachineryWorkingTimeSheetId) && x.TimeSheetApproved == true)
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

            var records = db.MachineryWorkingTimeSheets
                            .Where(x => ids.Contains(x.MachineryWorkingTimeSheetId))
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