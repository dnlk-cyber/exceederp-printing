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
using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Reporting.Fleet.ViewModel;
//using ExceedERP.Reporting.Fleet.ViewModel;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    [AuthorizeRoles(FleetManagementRoles.FleetTimeSheetUser, FleetManagementRoles.FleetTimeSheetApprover)]
    public class DryLoadingTimeSheetsController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index(TimeSheetType dryLoading)
        {
            ViewBag.setting = db.RequestToInspections
                  .FirstOrDefault()?.Remark ?? " ";
            ViewBag.TimesheetType = TimeSheetType.DryLoading;
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
            var fetchSetting = db.RequestToInspections.FirstOrDefault();
            string isECWC = "Not";
                if(fetchSetting!=null)
            {
                
                    isECWC = fetchSetting.Remark;
            }
            ViewBag.IsECWC = isECWC;
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
            ViewData["fleet"] = CacheData.GetFleetsSecondCache().Select(x => new { x.FleetsID, x.PlateNumber });
            ViewData["ProcurementSupplier"] = db.ProcurementSuppliers.Select(x => new { x.ProcurementSupplierID, x.Name });
            ViewBag.dryLoading = dryLoading;
           

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
            ViewData["branchh"] = db.Branch
     .Select(x => new SelectListItem
     {
         Text = x.Name,
         Value = x.BranchId.ToString()
     })
     .ToList();

            ViewData["businessunit"] = CacheData.GetBusinessUnitCaches().Select(
         i => new
         {
             Name = i.Name,
             Id = i.Id
         }).AsQueryable();
            ViewData["ItemCategory"] = db.ItemCategorys.Select(x => new
            {
                Value = x.ItemCategoryCode,
                Text = x.Name
            }).AsQueryable();
            ViewData["ItemCode"] = db.InventoryItems.Select(x => new
            {
                Value = x.ItemCode,
                Text = x.Name
            }).AsQueryable();

            return View();
        }
        public JsonResult RepopulateTimeSheet()
        {
            var timesheets = db.DryLoadingTimeSheets.ToList();
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
        public ActionResult DryLoadingTimeSheets_Read([DataSourceRequest] DataSourceRequest request)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            
            //var dryLoadingTimeSheets = db.DryLoadingTimeSheets.Where(x => mybraches.Contains(x.BranchId));
            var dryLoadingTimeSheets = db.DryLoadingTimeSheets;
            DataSourceResult result = dryLoadingTimeSheets.ToDataSourceResult(request, dryLoadingTimeSheet => new DryLoadingTimeSheet
            {
                DryLoadingTimeSheetId = dryLoadingTimeSheet.DryLoadingTimeSheetId,
                ProcurementSupplierID = dryLoadingTimeSheet.ProcurementSupplierID,
                FleetsID = dryLoadingTimeSheet.FleetsID,
                WorkItem = dryLoadingTimeSheet.WorkItem,
                WorkingHour = dryLoadingTimeSheet.WorkingHour,
                TripStartPlace = dryLoadingTimeSheet.TripStartPlace,
                TriplArrivalPlace = dryLoadingTimeSheet.TriplArrivalPlace,
                TripStartDate = dryLoadingTimeSheet.TripStartDate,
                TripArrivalDate = dryLoadingTimeSheet.TripArrivalDate,
                Asphalt = dryLoadingTimeSheet.Asphalt,
                Gravel = dryLoadingTimeSheet.Gravel,
                TotalKmTravelled = dryLoadingTimeSheet.TotalKmTravelled,
                FuelInLiter = dryLoadingTimeSheet.FuelInLiter,
                PerDiem = dryLoadingTimeSheet.PerDiem,
                FuelType = dryLoadingTimeSheet.FuelType,
                OilType = dryLoadingTimeSheet.OilType,
                IdleHour = dryLoadingTimeSheet.IdleHour,
                ReasonForIdle = dryLoadingTimeSheet.ReasonForIdle,
                DateCreated = dryLoadingTimeSheet.DateCreated,
                CreatedBy = dryLoadingTimeSheet.CreatedBy,
                ModifiedBy = dryLoadingTimeSheet.ModifiedBy,
                LastModified = dryLoadingTimeSheet.LastModified,
                DownReason = dryLoadingTimeSheet.DownReason,
                AsphaltRate = dryLoadingTimeSheet.AsphaltRate,
                FuelRate = dryLoadingTimeSheet.FuelRate,
                BatteryCost = dryLoadingTimeSheet.BatteryCost,
                GravelRate = dryLoadingTimeSheet.GravelRate,
                LoadedType = dryLoadingTimeSheet.LoadedType,
                LoadingUnloadingCost = dryLoadingTimeSheet.LoadingUnloadingCost,
                LubricantCost = dryLoadingTimeSheet.LubricantCost,
                SparePartCost = dryLoadingTimeSheet.SparePartCost,
                OilCost = dryLoadingTimeSheet.OilCost,
                GreaseConsumption = dryLoadingTimeSheet.GreaseConsumption,
                GreaseCost = dryLoadingTimeSheet.GreaseCost,
                MaintenanceCost = dryLoadingTimeSheet.MaintenanceCost,
                OilConsumption = dryLoadingTimeSheet.OilConsumption,
                NetworkingHour = dryLoadingTimeSheet.NetworkingHour,
                NoOfTripe = dryLoadingTimeSheet.NoOfTripe,
                Perdium = dryLoadingTimeSheet.Perdium,
                ReasonForDown = dryLoadingTimeSheet.ReasonForDown,
                IdleReason = dryLoadingTimeSheet.IdleReason,
                ProcurementSupplier = dryLoadingTimeSheet.ProcurementSupplier,
                Salary = dryLoadingTimeSheet.Salary,
                TireCost = dryLoadingTimeSheet.TireCost,
                BranchId = dryLoadingTimeSheet.BranchId,
                BusinessUnitId = dryLoadingTimeSheet.BusinessUnitId,
                Category = dryLoadingTimeSheet.Category,
                CostSheetApproved = dryLoadingTimeSheet.CostSheetApproved,
                CostSheetApprovedBy = dryLoadingTimeSheet.CostSheetApprovedBy,
                CostSheetApprovedDate = dryLoadingTimeSheet.CostSheetApprovedDate,
                CostSheetSendForAproval = dryLoadingTimeSheet.CostSheetSendForAproval,

                TimeSheetStatus = dryLoadingTimeSheet.TimeSheetStatus,

                TimeSheetApproved = dryLoadingTimeSheet.TimeSheetApproved,
                TimeSheetApprovedBy = dryLoadingTimeSheet.TimeSheetApprovedBy,
                TimeSheetApprovedDate = dryLoadingTimeSheet.TimeSheetApprovedDate,
                TimeSheetSendForAproval = dryLoadingTimeSheet.TimeSheetSendForAproval,
                OperatorId = dryLoadingTimeSheet.OperatorId,
                Downhour = dryLoadingTimeSheet.Downhour,
               EquipmentCategoryId = dryLoadingTimeSheet.EquipmentCategoryId,
               EquipmentTypeId = dryLoadingTimeSheet.EquipmentTypeId,
               EquipmentNameId = dryLoadingTimeSheet.EquipmentNameId

            });
            return Json(result);
        }
       
        public ActionResult DryLoadingTimeSheets_Create([DataSourceRequest] DataSourceRequest request, DryLoadingTimeSheet dryLoadingTimeSheet)
        {
            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == dryLoadingTimeSheet.FleetsID);
            dryLoadingTimeSheet.EquipmentCategoryId = fleet.EquipmentIdentityID;
            dryLoadingTimeSheet.EquipmentTypeId = fleet.EquipmentTypeID;
            dryLoadingTimeSheet.EquipmentNameId = fleet.EquipmentNameID;
            var timeSheets = db.DryLoadingTimeSheets.FirstOrDefault(x => x.FleetsID == dryLoadingTimeSheet.FleetsID && DbFunctions.TruncateTime(x.TripStartDate) == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate));
            if (timeSheets != null)
            {
                ModelState.AddModelError("duplicate", "Timesheet for this Plate On the selected day is already added");
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

            var fetchSettings = db.TimeSheetSetting.ToList();

            if (fetchSettings.Any())
            {
                var fetchSetting = fetchSettings.First();
                var fetchException = db.TimeSheetException.FirstOrDefault(x => x.TimeSheetSettingId == fetchSetting.TimeSheetSettingId && x.BranchId == dryLoadingTimeSheet.BranchId.ToString());
                if (fetchException == null)
                {
                    if (fetchSetting != null && fetchSetting.IsSet == true)
                    {

                        int hoursDifference = (int)(DateTime.Now - dryLoadingTimeSheet.TripStartDate).TotalHours;

                        if (fetchSetting.SettedHours < hoursDifference)
                        {
                            ModelState.AddModelError("date expired", "The timesheet recording date is passed");
                        }
                    }
                }
            }
            fleet.BranchId = dryLoadingTimeSheet.BranchId;
            db.Entry(fleet).State = EntityState.Modified;
            db.SaveChanges();
            var companySetting = db.RequestToInspections.FirstOrDefault();
            if (companySetting != null && companySetting.Remark == "MCG")
            {
                dryLoadingTimeSheet.DownReason = dryLoadingTimeSheet.ReasonForDown.ToString();
                dryLoadingTimeSheet.ReasonForIdle = dryLoadingTimeSheet.IdleReason.ToString();
            }
                if (companySetting != null)
                {
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
                                == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate)
                            && OilLubItems.Contains(ii.ItemCode)).ToList();
                dryLoadingTimeSheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                dryLoadingTimeSheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                                == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate)
                            && oilItems.Contains(ii.ItemCode)).ToList();
                dryLoadingTimeSheet.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                dryLoadingTimeSheet.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                                == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate)
                            && allSparePartItems.Contains(ii.ItemCode)).ToList();
                    var issuedBatteryItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate)
                            && allBatteryItems.Contains(ii.ItemCode)).ToList();
                    var issuedTyreItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate)
                            && allTyreItems.Contains(ii.ItemCode)).ToList();
                dryLoadingTimeSheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                dryLoadingTimeSheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                dryLoadingTimeSheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == fleet.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate));
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
                dryLoadingTimeSheet.MaintenanceCost += maintenancePayment;
                }
                if (ModelState.IsValid)
            {
                var entity = new DryLoadingTimeSheet
                {
                    DryLoadingTimeSheetId = dryLoadingTimeSheet.DryLoadingTimeSheetId,
                    ProcurementSupplierID = dryLoadingTimeSheet.ProcurementSupplierID,
                    FleetsID = dryLoadingTimeSheet.FleetsID,
                    WorkItem = dryLoadingTimeSheet.WorkItem,
                    WorkingHour = dryLoadingTimeSheet.WorkingHour,
                    TripStartPlace = dryLoadingTimeSheet.TripStartPlace,
                    TriplArrivalPlace = dryLoadingTimeSheet.TriplArrivalPlace,
                    TripStartDate = dryLoadingTimeSheet.TripStartDate,
                    TripArrivalDate = dryLoadingTimeSheet.TripArrivalDate,
                    Asphalt = dryLoadingTimeSheet.Asphalt,
                    Gravel = dryLoadingTimeSheet.Gravel,
                    SparePartCost = dryLoadingTimeSheet.SparePartCost,
                    OilCost = dryLoadingTimeSheet.OilCost,
                    GreaseConsumption = dryLoadingTimeSheet.GreaseConsumption,
                    GreaseCost = dryLoadingTimeSheet.GreaseCost,
                    MaintenanceCost = dryLoadingTimeSheet.MaintenanceCost,
                    OilConsumption = dryLoadingTimeSheet.OilConsumption,
                    RentalAgreementNum=fleet.RentFromExternalNum,
                    TotalKmTravelled = dryLoadingTimeSheet.TotalKmTravelled,
                    FuelInLiter = dryLoadingTimeSheet.FuelInLiter,
                    PerDiem = dryLoadingTimeSheet.PerDiem,
                    ReasonForDown = dryLoadingTimeSheet.ReasonForDown,
                    IdleReason = dryLoadingTimeSheet.IdleReason,
                    IdleHour = dryLoadingTimeSheet.IdleHour,
                    ReasonForIdle = dryLoadingTimeSheet.ReasonForIdle,
                    DateCreated = DateTime.Today,
                    ExternalOperatorName = dryLoadingTimeSheet.ExternalOperatorName,
                    CreatedBy = User.Identity.Name,
                    FuelType = dryLoadingTimeSheet.FuelType,
                    OilType = dryLoadingTimeSheet.OilType,
                    ModifiedBy = dryLoadingTimeSheet.ModifiedBy,
                    LastModified = dryLoadingTimeSheet.LastModified,
                    DownReason = dryLoadingTimeSheet.DownReason,
                    AsphaltRate = dryLoadingTimeSheet.AsphaltRate,
                    FuelRate = dryLoadingTimeSheet.FuelRate,
                    GravelRate = dryLoadingTimeSheet.GravelRate,
                    LoadedType = dryLoadingTimeSheet.LoadedType,
                    LoadingUnloadingCost = dryLoadingTimeSheet.LoadingUnloadingCost,
                    LubricantCost = dryLoadingTimeSheet.LubricantCost,
                    NetworkingHour = dryLoadingTimeSheet.NetworkingHour,
                    NoOfTripe = dryLoadingTimeSheet.NoOfTripe,
                    Perdium = dryLoadingTimeSheet.Perdium,
                    ProcurementSupplier = dryLoadingTimeSheet.ProcurementSupplier,
                    Salary = dryLoadingTimeSheet.Salary,
                    TireCost = dryLoadingTimeSheet.TireCost,
                    BranchId = dryLoadingTimeSheet.BranchId,
                    BusinessUnitId = dryLoadingTimeSheet.BusinessUnitId,
                    Category = dryLoadingTimeSheet.Category,
                    OperatorId = dryLoadingTimeSheet.OperatorId,
                    TimeSheetStatus = MaintenanceConst.Pending,
                    Downhour = dryLoadingTimeSheet.Downhour,
                    EquipmentCategoryId = dryLoadingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = dryLoadingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = dryLoadingTimeSheet.EquipmentNameId
                };
                db.DryLoadingTimeSheets.Add(entity);
                db.SaveChanges();
                
                dryLoadingTimeSheet.DryLoadingTimeSheetId = entity.DryLoadingTimeSheetId;
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dry loading timesheet registration", "Success", User.Identity.Name, $"created dry loading timesheet {entity.DryLoadingTimeSheetId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { dryLoadingTimeSheet }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DryLoadingTimeSheets_Update([DataSourceRequest] DataSourceRequest request, DryLoadingTimeSheet dryLoadingTimeSheet)
        {

            var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == dryLoadingTimeSheet.FleetsID);
            dryLoadingTimeSheet.EquipmentCategoryId = fleet.EquipmentIdentityID;
            dryLoadingTimeSheet.EquipmentTypeId = fleet.EquipmentTypeID;
            dryLoadingTimeSheet.EquipmentNameId = fleet.EquipmentNameID;
            var companySetting = db.RequestToInspections.FirstOrDefault();
            if (companySetting != null && companySetting.Remark == "MCG")
            {
                dryLoadingTimeSheet.DownReason = dryLoadingTimeSheet.ReasonForDown.ToString();
                dryLoadingTimeSheet.ReasonForIdle = dryLoadingTimeSheet.IdleReason.ToString();
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

            if (companySetting != null)
                {
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
                                == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate)
                            && OilLubItems.Contains(ii.ItemCode)).ToList();
                    dryLoadingTimeSheet.GreaseConsumption = lubItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    dryLoadingTimeSheet.GreaseCost = lubItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                                == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate)
                            && oilItems.Contains(ii.ItemCode)).ToList();
                    dryLoadingTimeSheet.OilConsumption = issuedOilItems.Sum(x => (decimal?)x.IssuedQuantity) ?? 0;
                    dryLoadingTimeSheet.OilCost = issuedOilItems.Sum(x => (decimal?)x.Total) ?? 0;
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
                                == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate)
                            && allSparePartItems.Contains(ii.ItemCode)).ToList();
                    var issuedBatteryItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate)
                            && allBatteryItems.Contains(ii.ItemCode)).ToList();
                    var issuedTyreItems = db.IssueItems
                    .Where(ii =>
                            ii.Issue.PlateNo == plateNo
                            && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                                == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate)
                            && allTyreItems.Contains(ii.ItemCode)).ToList();
                    dryLoadingTimeSheet.SparePartCost = issuedSpareItems.Sum(x => (decimal?)x.Total) ?? 0;
                    dryLoadingTimeSheet.BatteryCost = issuedBatteryItems.Sum(x => (decimal?)x.Total) ?? 0;
                    dryLoadingTimeSheet.TireCost = issuedTyreItems.Sum(x => (decimal?)x.Total) ?? 0;
                    var jobOrder = db.JobOrder.FirstOrDefault(x => x.FleetsID == fleet.FleetsID && x.Status == "Delivered" && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime) == DbFunctions.TruncateTime(dryLoadingTimeSheet.TripStartDate));
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
                    dryLoadingTimeSheet.MaintenanceCost += maintenancePayment;
                }
                if (ModelState.IsValid)
            {
                var totalCost = dryLoadingTimeSheet.SparePartCost + dryLoadingTimeSheet.TireCost + dryLoadingTimeSheet.BatteryCost + dryLoadingTimeSheet.OilCost + dryLoadingTimeSheet.FuelInLiter* dryLoadingTimeSheet.FuelRate + dryLoadingTimeSheet.MaintenanceCost + dryLoadingTimeSheet.GreaseCost;

                var entity = new DryLoadingTimeSheet
                {
                    DryLoadingTimeSheetId = dryLoadingTimeSheet.DryLoadingTimeSheetId,
                    ProcurementSupplierID = dryLoadingTimeSheet.ProcurementSupplierID,
                    WorkItem = dryLoadingTimeSheet.WorkItem,
                    FleetsID = dryLoadingTimeSheet.FleetsID,
                    WorkingHour = dryLoadingTimeSheet.WorkingHour,
                    TripStartPlace = dryLoadingTimeSheet.TripStartPlace,
                    TriplArrivalPlace = dryLoadingTimeSheet.TriplArrivalPlace,
                    TripStartDate = dryLoadingTimeSheet.TripStartDate,
                    TripArrivalDate = dryLoadingTimeSheet.TripArrivalDate,
                    Asphalt = dryLoadingTimeSheet.Asphalt,
                    Gravel = dryLoadingTimeSheet.Gravel,
                    TotalKmTravelled = dryLoadingTimeSheet.TotalKmTravelled,
                    FuelInLiter = dryLoadingTimeSheet.FuelInLiter,
                    PerDiem = dryLoadingTimeSheet.PerDiem,
                    IdleHour = dryLoadingTimeSheet.IdleHour,
                    SparePartCost = dryLoadingTimeSheet.SparePartCost,
                    OilCost = dryLoadingTimeSheet.OilCost,
                    GreaseConsumption = dryLoadingTimeSheet.GreaseConsumption,
                    GreaseCost = dryLoadingTimeSheet.GreaseCost,
                    MaintenanceCost = dryLoadingTimeSheet.MaintenanceCost,
                    OilConsumption = dryLoadingTimeSheet.OilConsumption,
                    ReasonForDown = dryLoadingTimeSheet.ReasonForDown,
                    IdleReason = dryLoadingTimeSheet.IdleReason,
                    ReasonForIdle = dryLoadingTimeSheet.ReasonForIdle,
                    DateCreated = dryLoadingTimeSheet.DateCreated,
                    CreatedBy = dryLoadingTimeSheet.CreatedBy,
                    FuelType = dryLoadingTimeSheet.FuelType,
                    OilType = dryLoadingTimeSheet.OilType,
                    RentalAgreementNum = fleet.RentFromExternalNum,
                    ModifiedBy = User.Identity.Name,
                    ExternalOperatorName = dryLoadingTimeSheet.ExternalOperatorName,
                    LastModified = DateTime.Today,
                    AsphaltRate = dryLoadingTimeSheet.AsphaltRate,
                    FuelRate = dryLoadingTimeSheet.FuelRate,
                    GravelRate = dryLoadingTimeSheet.GravelRate,
                    LoadedType = dryLoadingTimeSheet.LoadedType,
                    LoadingUnloadingCost = dryLoadingTimeSheet.LoadingUnloadingCost,
                    LubricantCost = dryLoadingTimeSheet.LubricantCost,
                    NetworkingHour = dryLoadingTimeSheet.NetworkingHour,
                    NoOfTripe = dryLoadingTimeSheet.NoOfTripe,
                    Perdium = dryLoadingTimeSheet.Perdium,
                    ProcurementSupplier = dryLoadingTimeSheet.ProcurementSupplier,
                    Salary = dryLoadingTimeSheet.Salary,
                    TireCost = dryLoadingTimeSheet.TireCost,
                    BranchId = dryLoadingTimeSheet.BranchId,
                    BusinessUnitId = dryLoadingTimeSheet.BusinessUnitId,
                    Category = dryLoadingTimeSheet.Category,
                    OperatorId = dryLoadingTimeSheet.OperatorId,
                    Downhour = dryLoadingTimeSheet.Downhour,
                    EquipmentCategoryId = dryLoadingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = dryLoadingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = dryLoadingTimeSheet.EquipmentNameId
                };
                db.DryLoadingTimeSheets.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Dry loading timesheet update", "Success", User.Identity.Name, $"updated dry loading timesheet {entity.DryLoadingTimeSheetId} ", Modules.FleetManagement);
                #endregion
            }
            return Json(new[] { dryLoadingTimeSheet }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DryLoadingTimeSheets_Destroy([DataSourceRequest] DataSourceRequest request, DryLoadingTimeSheet dryLoadingTimeSheet)
        {

            if (ModelState.IsValid)
            {
                var entity = new DryLoadingTimeSheet
                {
                    DryLoadingTimeSheetId = dryLoadingTimeSheet.DryLoadingTimeSheetId,
                    ProcurementSupplierID = dryLoadingTimeSheet.ProcurementSupplierID,
                    FleetsID = dryLoadingTimeSheet.FleetsID,
                    WorkItem = dryLoadingTimeSheet.WorkItem,
                    WorkingHour = dryLoadingTimeSheet.WorkingHour,
                    TripStartPlace = dryLoadingTimeSheet.TripStartPlace,
                    TriplArrivalPlace = dryLoadingTimeSheet.TriplArrivalPlace,
                    TripStartDate = dryLoadingTimeSheet.TripStartDate,
                    TripArrivalDate = dryLoadingTimeSheet.TripArrivalDate,
                    Asphalt = dryLoadingTimeSheet.Asphalt,
                    Gravel = dryLoadingTimeSheet.Gravel,
                    TotalKmTravelled = dryLoadingTimeSheet.TotalKmTravelled,
                    FuelInLiter = dryLoadingTimeSheet.FuelInLiter,
                    PerDiem = dryLoadingTimeSheet.PerDiem,
                    IdleHour = dryLoadingTimeSheet.IdleHour,
                    ReasonForIdle = dryLoadingTimeSheet.ReasonForIdle,
                    DateCreated = dryLoadingTimeSheet.DateCreated,
                    CreatedBy = dryLoadingTimeSheet.CreatedBy,
                    ModifiedBy = dryLoadingTimeSheet.ModifiedBy,
                    LastModified = dryLoadingTimeSheet.LastModified,
                    FuelType = dryLoadingTimeSheet.FuelType,
                    ExternalOperatorName = dryLoadingTimeSheet.ExternalOperatorName,
                    OilType = dryLoadingTimeSheet.OilType,
                    AsphaltRate = dryLoadingTimeSheet.AsphaltRate,
                    FuelRate = dryLoadingTimeSheet.FuelRate,
                    GravelRate = dryLoadingTimeSheet.GravelRate,
                    LoadedType = dryLoadingTimeSheet.LoadedType,
                    LoadingUnloadingCost = dryLoadingTimeSheet.LoadingUnloadingCost,
                    LubricantCost = dryLoadingTimeSheet.LubricantCost,
                    MaintenanceCost = dryLoadingTimeSheet.MaintenanceCost,
                    NetworkingHour = dryLoadingTimeSheet.NetworkingHour,
                    NoOfTripe = dryLoadingTimeSheet.NoOfTripe,
                    Perdium = dryLoadingTimeSheet.Perdium,
                    ProcurementSupplier = dryLoadingTimeSheet.ProcurementSupplier,
                    Salary = dryLoadingTimeSheet.Salary,
                    OilConsumption = dryLoadingTimeSheet.OilConsumption,
                    TireCost = dryLoadingTimeSheet.TireCost,
                    BranchId = dryLoadingTimeSheet.BranchId,
                    BusinessUnitId = dryLoadingTimeSheet.BusinessUnitId,
                    Category = dryLoadingTimeSheet.Category,
                    OperatorId = dryLoadingTimeSheet.OperatorId,
                    Downhour = dryLoadingTimeSheet.Downhour,
                    EquipmentCategoryId = dryLoadingTimeSheet.EquipmentCategoryId,
                    EquipmentTypeId = dryLoadingTimeSheet.EquipmentTypeId,
                    EquipmentNameId = dryLoadingTimeSheet.EquipmentNameId
                };

                db.DryLoadingTimeSheets.Attach(entity);
                db.DryLoadingTimeSheets.Remove(entity);
                db.SaveChanges();
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, $"Deleted dry loading timesheet {entity.DryLoadingTimeSheetId }");
                #endregion
            }

            return Json(new[] { dryLoadingTimeSheet }.ToDataSourceResult(request, ModelState));
        }


        #region Approval
        public JsonResult TimeSheetSendForApprovalDryLoad(int id)
        {

            var dryLoadingTimeSheet = db.DryLoadingTimeSheets.Find(id);

            if (dryLoadingTimeSheet != null)
            {
                dryLoadingTimeSheet.TimeSheetSendForAproval = true;
                dryLoadingTimeSheet.TimeSheetSendForApprovalTime = DateTime.Now;
                dryLoadingTimeSheet.TimeSheetSendForApprovalBy = User.Identity.Name;
                dryLoadingTimeSheet.TimeSheetStatus = MaintenanceConst.WaitingApproval;

                db.DryLoadingTimeSheets.Attach(dryLoadingTimeSheet);
                db.Entry(dryLoadingTimeSheet).State = EntityState.Modified;
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
        public JsonResult GetOperator(Guid FleetId)
        {
            var fetchOperator = db.DriverAssignments.FirstOrDefault(x => x.FleetId == FleetId);

            if (fetchOperator != null)
            {
                var fetchOperators = db.Operator.FirstOrDefault(x => x.OperatorID == fetchOperator.OperatorId);
                return Json(new
                {
                    text = fetchOperators.FullName,
                    value = fetchOperators.OperatorID
                }, JsonRequestBehavior.AllowGet);
            }

            return Json(null, JsonRequestBehavior.AllowGet);
        }
    
        public JsonResult TimeSheetVoidDryLoad(int id)
        {

            var dryLoadingTimeSheet = db.DryLoadingTimeSheets.Find(id);

            if (dryLoadingTimeSheet != null)
            {
               

                dryLoadingTimeSheet.TimeSheetSendForAproval = false;
                dryLoadingTimeSheet.TimeSheetApproved = false;
                dryLoadingTimeSheet.TimeSheetSendForApprovalTime = DateTime.Now;
                dryLoadingTimeSheet.TimeSheetApprovedBy = User.Identity.Name;
                dryLoadingTimeSheet.TimeSheetStatus = MaintenanceConst.Rejected;

                db.DryLoadingTimeSheets.Attach(dryLoadingTimeSheet);
                db.Entry(dryLoadingTimeSheet).State = EntityState.Modified;
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
        public ActionResult DryLoadingTimeSheetApproval_Read([DataSourceRequest] DataSourceRequest request, int dryLoadId)
        {

            var approval = db.TimeSheetApprovalNew.Where(x => x.TimeSheetId == dryLoadId && x.TimeSheetType == TimeSheetType.DryLoading);
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
        public ActionResult DryLoadingTimeSheetApproval_Create([DataSourceRequest] DataSourceRequest request, TimeSheetApprovalNew model, int dryLoadId)
        {
            if (ModelState.IsValid)
            {
                if (model.Status == MaintenanceConst.Approved)
                {
                    Approved(dryLoadId);
                    model.IsApproved = true;

                }
                else
                {
                    Rejected(dryLoadId);
                    model.IsApproved = false;
                }
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                model.TimeSheetId = dryLoadId;
                model.TimeSheetType = TimeSheetType.DryLoading;
                db.TimeSheetApprovalNew.Add(model);
                db.SaveChanges();

            }


            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void Approved(int id)
        {
            var dryLoading = db.DryLoadingTimeSheets.Find(id);
            if (dryLoading != null)
            {
                dryLoading.TimeSheetApproved = true;
                dryLoading.TimeSheetSendForAproval = true;
                dryLoading.TimeSheetStatus = MaintenanceConst.Approved;
                dryLoading.TimeSheetApprovedBy = User.Identity.Name;
                dryLoading.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(dryLoading).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        private void Rejected(int id)
        {
            var dryLoading = db.DryLoadingTimeSheets.Find(id);
            if (dryLoading != null)
            {
                dryLoading.TimeSheetApproved = false;
                dryLoading.TimeSheetSendForAproval = false;
                dryLoading.TimeSheetStatus = MaintenanceConst.Rejected;
                dryLoading.TimeSheetApprovedBy = User.Identity.Name;
                dryLoading.TimeSheetApprovedDate = DateTime.Now;
                db.Entry(dryLoading).State = EntityState.Modified;
                db.SaveChanges();

            }
        }
        public ActionResult GetFuelConsumption([DataSourceRequest]DataSourceRequest request,Guid fleetId, DateTime date)
        {
            var allFuelItems = db.FuelItemSettings.Where(x => x.IsLubricant == false&&x.IsOil==false).Select(x => x.ItemCode).ToList();
            var plateNo = db.Fleets
                .Where(x => x.FleetsID == fleetId)
                .Select(x => x.PlateNumber)
                .FirstOrDefault();
            if (plateNo == null)
            {
                plateNo = db.Fleets
                                .Where(x => x.FleetsID == fleetId)
                                .Select(x => x.SerialNo)
                                .FirstOrDefault();
            }
            if(plateNo==null)
            {
                plateNo = db.Fleets
                .Where(x => x.FleetsID == fleetId)
                .Select(x => x.ChasisNumber)
                .FirstOrDefault();
            }
                var fuelItems = db.IssueItems
                .Where(ii =>
                        ii.Issue.PlateNo == plateNo
                        && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                            == DbFunctions.TruncateTime(date)
                        && allFuelItems.Contains(ii.ItemCode))
                .Select(ii => new ItemConsumptionVm
                {
                    IssueID = ii.IssueID,
                    //IssueDate = ii.Issue.IssueDate,
                    PlateNo = ii.Issue.PlateNo,
                    ItemCode = ii.InventoryItem.Name,
                    ItemCategoryCode = ii.ItemCategory.Name,
                    IssuedQuantity = ii.IssuedQuantity,
                    UnitPrice = ii.UnitPrice,
                    Total = ii.Total,
                });
            return Json(fuelItems.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }
        public ActionResult GetOilConsumption([DataSourceRequest]DataSourceRequest request, Guid fleetId, DateTime date)
        {
            var OilLubItems = db.NonFuelItemSettings.Where(x=>x.IsLubricant==true||x.IsOil==true).Select(x=>x.ItemCode).ToList();
            var plateNo = db.Fleets
                .Where(x => x.FleetsID == fleetId)
                .Select(x => x.PlateNumber)
                .FirstOrDefault();
            if (plateNo == null)
            {
                plateNo = db.Fleets
                                .Where(x => x.FleetsID == fleetId)
                                .Select(x => x.SerialNo)
                                .FirstOrDefault();
            }
            if (plateNo == null)
            {
                plateNo = db.Fleets
                .Where(x => x.FleetsID == fleetId)
                .Select(x => x.ChasisNumber)
                .FirstOrDefault();
            }
            var fuelItems = db.IssueItems
            .Where(ii =>
                    ii.Issue.PlateNo == plateNo
                    && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                        == DbFunctions.TruncateTime(date)
                    && OilLubItems.Contains(ii.ItemCode))
            .Select(ii => new ItemConsumptionVm
            {
                IssueID = ii.IssueID,
                //IssueDate = ii.Issue.IssueDate,
                PlateNo = ii.Issue.PlateNo,
                ItemCode = ii.InventoryItem.Name,
                ItemCategoryCode = ii.ItemCategory.Name,
                IssuedQuantity = ii.IssuedQuantity,
                UnitPrice = ii.UnitPrice,
                Total = ii.Total,
            });
            return Json(fuelItems.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
        }public ActionResult GetSparePartConsumption([DataSourceRequest]DataSourceRequest request, Guid fleetId, DateTime date)
        {
            var outOfSpareItems = db.FuelItemSettings.Select(x=>x.ItemCode).ToList();
            var plateNo = db.Fleets
                .Where(x => x.FleetsID == fleetId)
                .Select(x => x.PlateNumber)
                .FirstOrDefault();
            if (plateNo == null)
            {
                plateNo = db.Fleets
                                .Where(x => x.FleetsID == fleetId)
                                .Select(x => x.SerialNo)
                                .FirstOrDefault();
            }
            if (plateNo == null)
            {
                plateNo = db.Fleets
                .Where(x => x.FleetsID == fleetId)
                .Select(x => x.ChasisNumber)
                .FirstOrDefault();
            }
            var fuelItems = db.IssueItems
            .Where(ii =>
                    ii.Issue.PlateNo == plateNo
                    && DbFunctions.TruncateTime(ii.Issue.IssueDate)
                        == DbFunctions.TruncateTime(date)
                    && !outOfSpareItems.Contains(ii.ItemCode))
            .Select(ii => new ItemConsumptionVm
            {
                IssueID = ii.IssueID,
                //IssueDate = ii.Issue.IssueDate,
                PlateNo = ii.Issue.PlateNo,
                ItemCode = ii.InventoryItem.Name,
                ItemCategoryCode = ii.ItemCategory.Name,
                IssuedQuantity = ii.IssuedQuantity,
                UnitPrice = ii.UnitPrice,
                Total = ii.Total,
            });
            return Json(fuelItems.ToDataSourceResult(request), JsonRequestBehavior.AllowGet);
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

            var records = db.DryLoadingTimeSheets
                            .Where(x => ids.Contains(x.DryLoadingTimeSheetId) && x.TimeSheetSendForAproval == true)
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
                model.TimeSheetId = item.DryLoadingTimeSheetId;
                model.TimeSheetType = TimeSheetType.DryLoading;
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

            var records = db.DryLoadingTimeSheets
                            .Where(x => ids.Contains(x.DryLoadingTimeSheetId) && x.TimeSheetApproved == true)
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

            var records = db.DryLoadingTimeSheets
                            .Where(x => ids.Contains(x.DryLoadingTimeSheetId))
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