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
    public class LightVehicleTimeSheetController : Controller
    {

        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        public ActionResult Index(TimeSheetType lightVehicle)
        {
           
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
           
            ViewBag.machineryType = lightVehicle;
            ViewData["fleet"] = db.Fleets.Select(x => new { x.FleetsID, Name = x.PlateNumber + "-" + x.SerialNo });
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name });
            
            ViewData["operator"] = db.Operator.Select(s => new
            {
                Value = s.OperatorID,
                Text = s.FullName
            });
            var fetchSettingg = db.RequestToInspections.FirstOrDefault();
            ViewBag.Setting = fetchSettingg?.Remark ?? "";
            string isECWCC = "Not";
            if (fetchSettingg != null)
            {

                isECWCC = fetchSettingg.Remark;
            }
         
           ViewBag.IsMIDROC = isECWCC;
            ViewBag.TimesheetType = TimeSheetType.LightVehicle;
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
            ViewData["businessunit"] = CacheData.GetBusinessUnitCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();

            return View();
        }
        public JsonResult RepopulateTimeSheet()
        {
            var timesheets = db.LightVehicleTimeSheets.ToList();
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
        public ActionResult LightVehicleTimeSheets_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var lightVehicleTimesheet = db.LightVehicleTimeSheets.Where(x => mybraches.Contains(x.BranchId));           
            DataSourceResult result = lightVehicleTimesheet.ToDataSourceResult(request, model => new LightVehicleTimeSheet
            {
                
                FleetsID = model.FleetsID,
                LightVehicleTimeSheetId = model.LightVehicleTimeSheetId,
                Date= model.Date,
                MorningTimeStart= model.MorningTimeStart,
                MorningTimeEnd= model.MorningTimeEnd,
                AfternoonTimeStart= model.AfternoonTimeStart,
                AfternoonTimeEnd = model.AfternoonTimeEnd,
                NightTimeStart= model.NightTimeStart,
                NightTimeEnd= model.NightTimeEnd,
                NumberOfHoles= model.NumberOfHoles,
                HoleDepth= model.HoleDepth,
                DifferenceKMR = model.DifferenceKMR,
                FinalKMR = model.FinalKMR,
                InitialKMR = model.InitialKMR,
                FuelKMR = model.FuelKMR,
                FuelConsumption= model.FuelConsumption,
                FuelRate = model.FuelRate,
                IdleHours= model.IdleHours,
                OilConsumption = model.OilConsumption,
                BatteryCost = model.BatteryCost,
                OilCost = model.OilCost,
                SparePartCost = model.SparePartCost,
                GreaseCost = model.GreaseCost,
                GreaseConsumption = model.GreaseConsumption,
                MaintenanceCost = model.MaintenanceCost,
                ExternalOperatorName = model.ExternalOperatorName,
                IdleReason= model.IdleReason,
                DownHours= model.DownHours,
                DownReason= model.DownReason,
                FuelType = model.FuelType,
                TotalTime = model.TotalTime,
                OilType = model.OilType,
                IsECWC =checkIsECWC(),
                NetHour = model.NetHour,
                WorkType = model.WorkType,
                Remark = model.Remark,
                RentalRate = model.RentalRate,
                IdealRentalRate = model.IdealRentalRate,
                ProcurementSupplierID = model.ProcurementSupplierID,
                OperationAmount = model.OperationAmount,
                IdleRate = model.IdleRate,
                IdleAmount = model.IdleAmount,
                TotalAmountBeforeVAT = model.TotalAmountBeforeVAT,
                VAT = model.VAT,
                TotalAmount = model.TotalAmount,
                FuelCost = model.FuelCost,
                ServiceRecharge = model.ServiceRecharge,
                
                TotalCost = model.TotalCost,
                NetAmount = model.NetAmount,

                DownHourRate = model.DownHourRate,
                OperationRevenue = model.OperationRevenue,
                Perdium = model.Perdium,
                ReasonForDown = model.ReasonForDown,
                ReasonForIdle = model.ReasonForIdle,
                Salary = model.Salary,
                TireCost = model.TireCost,
                BranchId = model.BranchId,
                BusinessUnitId = model.BusinessUnitId,
                TimeSheetStatus = model.TimeSheetStatus,
                CostSheetApproved = model.CostSheetApproved,
                CostSheetApprovedBy = model.CostSheetApprovedBy,
                CostSheetApprovedDate = model.CostSheetApprovedDate,
                CostSheetSendForAproval = model.CostSheetSendForAproval,
                CreatedBy = model.CreatedBy,
                DateCreated = model.DateCreated,
                LastModified = model.LastModified,
                ModifiedBy = model.ModifiedBy,
                TimeSheetApproved = model.TimeSheetApproved,
                TimeSheetSendForAproval = model.TimeSheetSendForAproval,
                TimeSheetApprovedBy = model.TimeSheetApprovedBy,
                TimeSheetApprovedDate = model.TimeSheetApprovedDate,

                OperatorId = model.OperatorId,
                EquipmentCategoryId = model.EquipmentCategoryId,
                EquipmentTypeId = model.EquipmentTypeId,
                EquipmentNameId = model.EquipmentNameId
            });
            return Json(result);
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
        private bool checkIsECWC()
        {
            bool x = false;
            var fetchSetting = db.RequestToInspections.FirstOrDefault();
            if(fetchSetting!=null&&fetchSetting.Remark=="ECWC")
            {
                x = true;
            }
               return(x);
        }
        public ActionResult LightVehicleTimeSheets_Create([DataSourceRequest] DataSourceRequest request, LightVehicleTimeSheet model)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == model.FleetsID);
            model.EquipmentCategoryId = fleet.EquipmentIdentityID;
            model.EquipmentTypeId = fleet.EquipmentTypeID;
            model.EquipmentNameId = fleet.EquipmentNameID;
            var timeSheets = db.LightVehicleTimeSheets.FirstOrDefault(x => x.FleetsID == model.FleetsID && DbFunctions.TruncateTime(x.Date) == DbFunctions.TruncateTime(model.Date));
            if (timeSheets != null)
            {
                ModelState.AddModelError("duplicate", "Timesheet for this Plate On the selected day is already added");
            }
            var checkDuplicate = db.LightVehicleTimeSheets.FirstOrDefault(x => x.FleetsID == model.FleetsID && x.Date == model.Date);
            if(checkDuplicate!=null)
            {
                ModelState.AddModelError("Duplicate", "Timesheet for this machine is already registered on this date");
            }
            var setting = db.RequestToInspections.FirstOrDefault();
            var lastTimesheet = db.LightVehicleTimeSheets.Where(x => x.FleetsID == model.FleetsID).OrderByDescending(x => x.LightVehicleTimeSheetId).FirstOrDefault();

            if (setting != null && setting.Remark == "MCG")
            {
                if (model.InitialKMR <0 || model.FinalKMR < 0 || (model.FinalKMR - model.InitialKMR) < 0 || model.FuelKMR < 0)
                {
                    ModelState.AddModelError("invalid", "Invalid KMR entry");
                }

                model.DifferenceKMR = model.FinalKMR - model.InitialKMR;
                model.DownReason =model.ReasonForDown.ToString();
                    model.IdleReason = model.ReasonForIdle.ToString();
                
            }
                 model.NetHour =
                    (decimal)GetTotalTime(
                        model.MorningTimeStart,
                        model.MorningTimeEnd,
                        model.AfternoonTimeStart,
                        model.AfternoonTimeEnd,
                        model.NightTimeStart,
                        model.NightTimeEnd
                    ).TotalHours;
            model.TotalTime += model.NetHour;
            #region valiadtion
            #endregion
            #region data
            var operationAmount = model.RentalRate * model.NetHour;
            var idleAmount = model.IdleRate * model.IdleHours;
            var totalAmountBeforeVAT = operationAmount + idleAmount;
            var totalAmount = totalAmountBeforeVAT * model.VAT;
            var fuelCost = model.FuelConsumption * model.FuelRate;
            var totalCost = model.OilCost+fuelCost + model.MaintenanceCost + model.GreaseCost + model.ServiceRecharge;

            #endregion
           
            var fetchSettings = db.TimeSheetSetting.ToList();

            if (fetchSettings.Any())
            {
                var fetchSetting = fetchSettings.First();
                var fetchException = db.TimeSheetException.FirstOrDefault(x => x.TimeSheetSettingId == fetchSetting.TimeSheetSettingId && x.BranchId == model.BranchId.ToString());
                if (fetchException == null)
                {
                    if (fetchSetting != null && fetchSetting.IsSet == true)
                    {
                        int hoursDifference = (int)(DateTime.Now - model.Date).TotalHours;

                        if (fetchSetting.SettedHours < hoursDifference)
                        {
                            ModelState.AddModelError("date expired", "The timesheet recording date is passed");
                        }
                    }
                }
            }
            if (model.BranchId != 0 && fleet != null)
            {
                fleet.BranchId = model.BranchId;
                db.Entry(fleet).State = EntityState.Modified;
                db.SaveChanges();
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
                            == DbFunctions.TruncateTime(model.Date)
                        && OilLubItems.Contains(ii.ItemCode)).ToList();
                model.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                model.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                            == DbFunctions.TruncateTime(model.Date)
                        && oilItems.Contains(ii.ItemCode)).ToList();
                model.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                model.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                            == DbFunctions.TruncateTime(model.Date)
                        && allSparePartItems.Contains(ii.ItemCode)).ToList();
                var issuedBatteryItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(model.Date)
                        && allBatteryItems.Contains(ii.ItemCode)).ToList();
                var issuedTyreItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(model.Date)
                        && allTyreItems.Contains(ii.ItemCode)).ToList();
                model.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                model.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                model.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == fleet.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(model.Date));
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
                model.MaintenanceCost += maintenancePayment;
            }

            if (ModelState.IsValid)
            {
                var entity = new LightVehicleTimeSheet
                {
                    FleetsID = model.FleetsID,
                    
                    Date = model.Date,
                    MorningTimeStart = model.MorningTimeStart,
                    MorningTimeEnd = model.MorningTimeEnd,
                    AfternoonTimeStart = model.AfternoonTimeStart,
                    AfternoonTimeEnd = model.AfternoonTimeEnd,
                    NightTimeStart = model.NightTimeStart,
                    NightTimeEnd = model.NightTimeEnd,
                    InitialKMR=model.InitialKMR,
                    TotalTime = model.TotalTime,
                    FinalKMR=model.FinalKMR,
                    DifferenceKMR=model.DifferenceKMR,
                    FuelKMR=model.FuelKMR,
                    NumberOfHoles = model.NumberOfHoles,
                    HoleDepth = model.HoleDepth,
                    FuelConsumption = model.FuelConsumption,
                    FuelRate = model.FuelRate,
                    IdleHours = model.IdleHours,
                    RentalAgreementNum = fleet.RentFromExternalNum,
                    IdleReason = model.IdleReason,
                    DownHours = model.DownHours,
                    FuelType = model.FuelType,
                    OilType = model.OilType,
                    ReasonForDown = model.ReasonForDown,
                    ReasonForIdle = model.ReasonForIdle,
                    ExternalOperatorName = model.ExternalOperatorName,
                    DownReason = model.DownReason,
                    NetHour = model.NetHour,
                    WorkType = model.WorkType,
                    Remark = model.Remark,
                    RentalRate = model.RentalRate,
                    IdealRentalRate = model.IdealRentalRate,
                    ProcurementSupplierID = model.ProcurementSupplierID,
                    OperationAmount = operationAmount,
                    IdleRate = model.IdleRate,
                    IdleAmount = idleAmount,
                    TotalAmountBeforeVAT = totalAmountBeforeVAT,
                    VAT = model.VAT,
                    TotalAmount = totalAmount,
                    FuelCost = fuelCost,
                    ServiceRecharge = model.ServiceRecharge,
                    GreaseCost = model.GreaseCost,
                    GreaseConsumption = model.GreaseConsumption,
                    OilCost = model.OilCost,
                    OilConsumption = model.OilConsumption,
                    SparePartCost = model.SparePartCost,
                    MaintenanceCost = model.MaintenanceCost,
                    TotalCost = totalCost,
                    NetAmount = totalAmount-totalCost,

                    DownHourRate = model.DownHourRate,
                    OperationRevenue = model.OperationRevenue,
                    Perdium = model.Perdium,
                    Salary = model.Salary,
                    TireCost = model.TireCost,
                    BranchId = model.BranchId,
                    BusinessUnitId = model.BusinessUnitId,
                    OperatorId = model.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    EquipmentCategoryId = model.EquipmentCategoryId,
                    EquipmentTypeId = model.EquipmentTypeId,
                    EquipmentNameId = model.EquipmentNameId
                };

                entity.InitialKMR = model.InitialKMR;
                entity.FinalKMR = model.FinalKMR;
                entity.DifferenceKMR = model.FinalKMR - model.InitialKMR;
                entity.FuelKMR = model.FuelKMR;
                db.LightVehicleTimeSheets.Add(entity);
                db.SaveChanges();
                //var fleetSetting = db.RequestToInspections.FirstOrDefault(x=>x.Remark!=null);

                //if(fleetSetting!=null&&fleetSetting.Remark=="MIDROC")
                //{ 
                //List<LightVehicleTimeSheet> timeSheet = db.LightVehicleTimeSheets
                //    .Where(x => x.BranchId == entity.BranchId &&
                //                x.EquipmentCategoryId == entity.EquipmentCategoryId&&x.FleetsID!=entity.FleetsID)
                //    .GroupBy(x => x.FleetsID)
                //    .Select(g => g.OrderByDescending(x => x.LightVehicleTimeSheetId).FirstOrDefault())
                //    .ToList();
                //var firstObject = timeSheet.First();
                //var checkDuplicatee = db.LightVehicleTimeSheets.FirstOrDefault(x => x.FleetsID == firstObject.FleetsID && x.Date == firstObject.Date);
                //    if (checkDuplicatee == null)
                //    {
                //        foreach (var timesht in timeSheet)
                //        {
                //            var newTimeSheet = new LightVehicleTimeSheet
                //            {
                //                FleetsID = timesht.FleetsID,

                //                Date = entity.Date,
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
                //                DownHours = timesht.DownHours,
                //                FuelType = timesht.FuelType,
                //                OilType = timesht.OilType,
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
                //                EquipmentCategoryId = timesht.EquipmentCategoryId,
                //                EquipmentTypeId = timesht.EquipmentTypeId,
                //                EquipmentNameId = timesht.EquipmentNameId
                //            };

                //            db.LightVehicleTimeSheets.Add(newTimeSheet);
                //            db.SaveChanges();
                //        }
                //    }
                //}
                model.LightVehicleTimeSheetId = entity.LightVehicleTimeSheetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Light Vehicle timesheet", "Success", User.Identity.Name, $"created light vehicle working timesheet {entity.LightVehicleTimeSheetId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LightVehicleTimeSheets_Update([DataSourceRequest] DataSourceRequest request, LightVehicleTimeSheet model)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == model.FleetsID);
            model.EquipmentCategoryId = fleet.EquipmentIdentityID;
            model.EquipmentTypeId = fleet.EquipmentTypeID;
            model.EquipmentNameId = fleet.EquipmentNameID;
            #region data
            var operationAmount = model.RentalRate * model.NetHour;
            var idleAmount = model.IdleRate * model.IdleHours;
            var totalAmountBeforeVAT = operationAmount + idleAmount;
            var totalAmount = totalAmountBeforeVAT * model.VAT;
            var fuelCost = model.FuelConsumption * model.FuelRate;

            var lastTimesheet = db.LightVehicleTimeSheets.Where(x => x.FleetsID == model.FleetsID&&x.LightVehicleTimeSheetId!=model.LightVehicleTimeSheetId).OrderByDescending(x => x.LightVehicleTimeSheetId).FirstOrDefault();
            var setting = db.RequestToInspections.FirstOrDefault();
            if (setting != null && setting.Remark == "MCG")
            {
                model.TotalTime = model.IdleRate * model.IdleHours * 0.01m;
                if (model.InitialKMR <0 || model.FinalKMR < 0 || (model.FinalKMR - model.InitialKMR) < 0 || model.FuelKMR < 0)
                {
                    ModelState.AddModelError("invalid", "Invalid KMR entry");
                }
                model.DifferenceKMR = model.FinalKMR - model.InitialKMR;
                model.DownReason = model.ReasonForDown.ToString();
                model.IdleReason = model.ReasonForIdle.ToString();
            }
            model.NetHour =
                    (decimal)GetTotalTime(
                        model.MorningTimeStart,
                        model.MorningTimeEnd,
                        model.AfternoonTimeStart,
                        model.AfternoonTimeEnd,
                        model.NightTimeStart,
                        model.NightTimeEnd
                    ).TotalHours;
            model.TotalTime += model.NetHour;
            TimeSpan morningTime = model.MorningTimeEnd - model.MorningTimeStart;
            TimeSpan afternoonTime = model.AfternoonTimeEnd - model.AfternoonTimeStart;
            TimeSpan nightTime = model.NightTimeEnd - model.NightTimeStart;

            if (morningTime < TimeSpan.Zero && afternoonTime < TimeSpan.Zero && nightTime < TimeSpan.Zero)
            {
                // Convert time differences to decimal format
                double morningDecimal = morningTime.TotalHours;
                double afternoonDecimal = afternoonTime.TotalHours;
                double nightDecimal = nightTime.TotalHours;
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
                            == DbFunctions.TruncateTime(model.Date)
                        && OilLubItems.Contains(ii.ItemCode)).ToList();
                model.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                model.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                            == DbFunctions.TruncateTime(model.Date)
                        && oilItems.Contains(ii.ItemCode)).ToList();
                model.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                model.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                            == DbFunctions.TruncateTime(model.Date)
                        && !allSparePartItems.Contains(ii.ItemCode)).ToList();
                var issuedBatteryItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(model.Date)
                        && allBatteryItems.Contains(ii.ItemCode)).ToList();
                var issuedTyreItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(model.Date)
                        && allTyreItems.Contains(ii.ItemCode)).ToList();
                model.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                model.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                model.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == fleet.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(model.Date));
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
                model.MaintenanceCost += maintenancePayment;
            }

            #endregion
            if (ModelState.IsValid)
            {
                var totalCost = model.SparePartCost + model.TireCost + model.BatteryCost + model.OilCost + fuelCost + model.MaintenanceCost + model.GreaseCost + model.ServiceRecharge;
                var ExistingTimesheet = db.LightVehicleTimeSheets.FirstOrDefault(x => x.LightVehicleTimeSheetId == model.LightVehicleTimeSheetId);                
                    ExistingTimesheet.Date = model.Date;
                    ExistingTimesheet.MorningTimeStart = model.MorningTimeStart;
                    ExistingTimesheet.MorningTimeEnd = model.MorningTimeEnd;
                    ExistingTimesheet.AfternoonTimeStart = model.AfternoonTimeStart;
                    ExistingTimesheet.AfternoonTimeEnd = model.AfternoonTimeEnd;
                    ExistingTimesheet.NightTimeStart = model.NightTimeStart;
                    ExistingTimesheet.NightTimeEnd = model.NightTimeEnd;
                    ExistingTimesheet.NumberOfHoles = model.NumberOfHoles;
                    ExistingTimesheet.HoleDepth = model.HoleDepth;
                    ExistingTimesheet.FuelConsumption = model.FuelConsumption;
                    ExistingTimesheet.FuelRate = model.FuelRate;
                    ExistingTimesheet.TotalTime = model.TotalTime;
                    ExistingTimesheet.FuelKMR = model.FuelKMR;
                    ExistingTimesheet.GreaseConsumption = model.GreaseConsumption;
                    ExistingTimesheet.IdleHours = model.IdleHours;
                    ExistingTimesheet.IdleReason = model.IdleReason;
                    ExistingTimesheet.DownHours = model.DownHours;
                    ExistingTimesheet.DownReason = model.DownReason;
                    ExistingTimesheet.NetHour = model.NetHour;
                    ExistingTimesheet.WorkType = model.WorkType;
                    ExistingTimesheet.ReasonForDown = model.ReasonForDown;
                    ExistingTimesheet.ReasonForIdle = model.ReasonForIdle;
                    ExistingTimesheet.Remark = model.Remark;
                    ExistingTimesheet.ExternalOperatorName = model.ExternalOperatorName;
                    ExistingTimesheet.RentalRate = model.RentalRate;
                    ExistingTimesheet.IdealRentalRate = model.IdealRentalRate;
                    ExistingTimesheet.ProcurementSupplierID = model.ProcurementSupplierID;
                    ExistingTimesheet.OperationAmount = operationAmount;
                    ExistingTimesheet.IdleRate = model.IdleRate;
                    ExistingTimesheet.IdleAmount = idleAmount;
                    ExistingTimesheet.TotalAmountBeforeVAT = totalAmountBeforeVAT;
                ExistingTimesheet.RentalAgreementNum = fleet.RentFromExternalNum;
                    ExistingTimesheet.VAT = model.VAT;
                    ExistingTimesheet.TotalAmount = totalAmount;
                    ExistingTimesheet.FuelCost = fuelCost;
                    ExistingTimesheet.ServiceRecharge = model.ServiceRecharge;
                    ExistingTimesheet.GreaseCost = model.GreaseCost;
                    ExistingTimesheet.GreaseConsumption = model.GreaseConsumption;
                    ExistingTimesheet.BatteryCost = model.BatteryCost;
                    ExistingTimesheet.OilCost = model.OilCost;
                    ExistingTimesheet.OilConsumption = model.OilConsumption;
                    ExistingTimesheet.SparePartCost = model.SparePartCost;
                    ExistingTimesheet.MaintenanceCost = model.MaintenanceCost;
                    ExistingTimesheet.TotalCost = totalCost;
                    ExistingTimesheet.NetAmount = totalAmount - totalCost;
                    ExistingTimesheet.FuelType = model.FuelType;
                    ExistingTimesheet.OilType = model.OilType;
                    ExistingTimesheet.DownHourRate = model.DownHourRate;
                    ExistingTimesheet.OperationRevenue = model.OperationRevenue;
                    ExistingTimesheet.Perdium = model.Perdium;
                    ExistingTimesheet.Salary = model.Salary;
                    ExistingTimesheet.TireCost = model.TireCost;
                    ExistingTimesheet.BranchId = model.BranchId;
                    ExistingTimesheet.BusinessUnitId = model.BusinessUnitId;
                    ExistingTimesheet.OperatorId = model.OperatorId;
                    ExistingTimesheet.EquipmentCategoryId = model.EquipmentCategoryId;
                    ExistingTimesheet.EquipmentTypeId = model.EquipmentTypeId;
                    ExistingTimesheet.EquipmentNameId = model.EquipmentNameId;


                ExistingTimesheet.InitialKMR = model.InitialKMR;
                ExistingTimesheet.FinalKMR = model.FinalKMR;
                ExistingTimesheet.DifferenceKMR = model.FinalKMR - model.InitialKMR;
                ExistingTimesheet.FuelKMR = model.FuelKMR;
              
               

                db.Entry(ExistingTimesheet).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Light Vehicle registration", "Success", User.Identity.Name, $"updated machinery working timesheet {ExistingTimesheet.LightVehicleTimeSheetId}", Modules.FleetManagement);
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }


        public ActionResult LightVehicleTimeSheets_Destroy([DataSourceRequest] DataSourceRequest request, LightVehicleTimeSheet model)
        {
            if (ModelState.IsValid)
            {
                var lightVehicleTimeSheet = db.LightVehicleTimeSheets.Find(model.LightVehicleTimeSheetId);
                db.LightVehicleTimeSheets.Attach(lightVehicleTimeSheet);
                db.LightVehicleTimeSheets.Remove(lightVehicleTimeSheet);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted working light Vehicle timesheet {model.LightVehicleTimeSheetId }");
                #endregion
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
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

                //db.LightVehicleTimeSheets.Attach(lightVehicle);
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
                lightVehicle.TimeSheetSendForAproval = false;
                lightVehicle.TimeSheetStatus = MaintenanceConst.Rejected;
                lightVehicle.TimeSheetApprovedBy = User.Identity.Name;
                lightVehicle.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(lightVehicle).State = EntityState.Modified;
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

            var records = db.LightVehicleTimeSheets
                            .Where(x => ids.Contains(x.LightVehicleTimeSheetId) && x.TimeSheetSendForAproval == true)
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
                model.TimeSheetId = item.LightVehicleTimeSheetId;
                model.TimeSheetType = TimeSheetType.LightVehicle;
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

            var records = db.LightVehicleTimeSheets
                            .Where(x => ids.Contains(x.LightVehicleTimeSheetId) && x.TimeSheetApproved == true)
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

            var records = db.LightVehicleTimeSheets
                            .Where(x => ids.Contains(x.LightVehicleTimeSheetId))
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