using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetViewModel;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers
{
    public class FleetEquipmentHistoryJacketController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/FleetEquipmentHistoryJacket
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Equipment_Read([DataSourceRequest] DataSourceRequest request)
        {
            var equip = db.Fleets.ToList();
            var equipmentHistoryJacket = equip.Select(s => new FleetEquipmentHistoryJacketViewModel
            {
                FleetId = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetsID).FleetsID,
                EquipmentCategoryName = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == s.EquipmentIdentityID).EquipmentCategoryName,
                EquipmentTypeName = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == s.EquipmentTypeID).EquipmentTypeName,
                EquipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID).Name,
                PlateNo = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetsID).PlateNumber,
                SerialNo = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetsID).SerialNo
            });
            return Json(equipmentHistoryJacket.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Insurance_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            var insurance = db.FleetInsurance.Where(x => x.FleetsID == id).ToList();
            var insuranceVm = insurance.Select(s => new FleetInsuranceViewModel
            {
                FleetInsuranceId = s.FleetInsuranceId,
                ComprehensiveInsurance = s.ComprehensiveInsurance,
                DueDate = s.DueDate,
                InsuranceCompanyName = db.InsuranceCompany.FirstOrDefault(x => x.InsuranceCompanyId == s.InsuranceCompanyId).InsuranceCompanyName,
                InsuranceType = s.InsuranceType,
                PolicyNo = s.PolicyNo,
                PremiumCost = s.PremiumCost,
                ThirdPartyInsurance = s.ThirdPartyInsurance,
                Description = s.Description
            });
            return Json(insuranceVm.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Accident_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            var accidentReg = db.Accident.Where(x => x.FleetsID == id).ToList();
            var accidentVm = accidentReg.Select(s => new FleetAcidentViewModel
            {
                AccidentId = s.AccidentID,
                AccidentLocation = s.AccidentLocation,
                Accountablebody = s.Accountablebody,
                AssetDamage = s.AssetDamage,
                CauseOfAccident = s.CauseOfAccident,
                DateOfOccurrence = s.DateOfOccurrence,
                InjuredPersons = s.InjuredPersons,
                LevelOfDamage = s.LevelOfDamage,
                OperatorName = s.LevelOfDamage,
                PolicyIdNo = s.PolicyIdNo,
                ThirdPartyDamage = s.ThirdPartyDamage,
                TimeOfAccident = s.TimeOfAccident,
                VehicleDistanceFromRoadEdge = s.VehicleDistanceFromRoadEdge,
                VehicleSpeed = s.VehicleSpeed,
            });
            return Json(accidentVm.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Read_TimeSheet([DataSourceRequest]DataSourceRequest request, Guid id)
        {
            var fleet = db.Fleets.Find(id);
            var equipmentType = db.EquipmentType.Find(fleet.EquipmentTypeID);
            var timesheetType = equipmentType.TimeSheetType;

            IEnumerable<TimesheetVm> timesheet = new List<TimesheetVm>();
            if (timesheetType == TimeSheetType.Machinery)
            {
                var machinery = db.MachineryWorkingTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                timesheet = machinery.Select(s => new TimesheetVm
                {
                    TimeSheetType = TimeSheetType.Machinery,
                    FleetTimeSheetId = s.MachineryWorkingTimeSheetId,
                    BranchName = db.Branch.FirstOrDefault(x => x.BranchId == s.BranchId)?.Name ?? "",
                    BusinessUnitName = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == s.BusinessUnitId)?.Name ?? "",
                    Date = s.Date,
                    MorningTimeStart = s.MorningTimeStart,
                    MorningTimeEnd = s.MorningTimeEnd,
                    AfternoonTimeStart = s.AfternoonTimeStart,
                    AfternoonTimeEnd = s.AfternoonTimeEnd,
                    NightTimeStart = s.NightTimeStart,
                    NightTimeEnd = s.NightTimeEnd,
                    GreaseConsumption = s.GreaseConsumption,
                    FuelConsumption = s.FuelConsumption,
                    DownHour = s.DownHours,
                    IdleReason = s.IdleReason,
                    WorkType = s.WorkType,
                    IdleHour = s.IdleHours,
                    DownReason = s.DownReason,
                    OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == s.OperatorId)?.FullName ?? "",

                });
            }
            else if (timesheetType == TimeSheetType.LightVehicle)
            {
                var lightVehicle = db.LightVehicleTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                timesheet = lightVehicle.Select(s => new TimesheetVm
                {
                    TimeSheetType = TimeSheetType.LightVehicle,
                    FleetTimeSheetId = s.LightVehicleTimeSheetId,
                    BranchName = db.Branch.FirstOrDefault(x => x.BranchId == s.BranchId)?.Name ?? "",
                    BusinessUnitName = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == s.BusinessUnitId)?.Name ?? "",
                    Date = s.Date,
                    MorningTimeStart = s.MorningTimeStart,
                    MorningTimeEnd = s.MorningTimeEnd,
                    AfternoonTimeStart = s.AfternoonTimeStart,
                    AfternoonTimeEnd = s.AfternoonTimeEnd,
                    NightTimeStart = s.NightTimeStart,
                    NightTimeEnd = s.NightTimeEnd,
                    GreaseConsumption = s.GreaseConsumption,
                    FuelConsumption = s.FuelConsumption,
                    DownHour = s.DownHours,
                    IdleReason = s.IdleReason,
                    WorkType = s.WorkType,
                    IdleHour = s.IdleHours,
                    DownReason = s.DownReason,
                    OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == s.OperatorId)?.FullName ?? ""

                });
            }
            else if (timesheetType == TimeSheetType.DumpTruck)
            {
                var dumpTruck = db.DumpTruckTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                timesheet = dumpTruck.Select(s => new TimesheetVm
                {
                    TimeSheetType = TimeSheetType.DumpTruck,
                    FleetTimeSheetId = s.DumpTruckTimeSheetId,
                    OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == s.OperatorId)?.FullName ?? "",
                    BranchName = db.Branch.FirstOrDefault(x => x.BranchId == s.BranchId)?.Name ?? "",
                    BusinessUnitName = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == s.BusinessUnitId)?.Name ?? "",
                    Date = s.Date,
                    FuelConsumption = s.FuelConsumption,
                    IdleHour = s.IdleHour,
                    ReasonForIdle = s.ReasonForIdle,
                    DownHour = s.DownHour,
                    ReasonForDown = s.ReasonForDown,
                    OilConsumption = s.OilConsumption,

                    GreaseConsumption = s.GreaseConsumption,
                    NetWorkingHour = s.NetworkingHour,

                });
            }
            else if (timesheetType == TimeSheetType.DryLoading)
            {
                var dryLoad = db.DryLoadingTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                timesheet = dryLoad.Select(s => new TimesheetVm
                {
                    TimeSheetType = TimeSheetType.DryLoading,
                    FleetTimeSheetId = s.DryLoadingTimeSheetId,
                    OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == s.OperatorId)?.FullName ?? "",
                    BranchName = db.Branch.FirstOrDefault(x => x.BranchId == s.BranchId)?.Name ?? "",
                    BusinessUnitName = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == s.BusinessUnitId)?.Name ?? "",
                    TripStartDate = s.TripStartDate,
                    TripStartPlace = s.TripStartPlace,
                    TripArrivalDate = s.TripArrivalDate,
                    TripArrivalPlace = s.TriplArrivalPlace,
                    TotalKmTravelled = s.TotalKmTravelled,
                    WorkItem = s.WorkItem,
                    FuelInLiter = s.FuelInLiter,
                    ReasonForIdle = s.ReasonForIdle,
                    WorkingHour = s.WorkingHour,
                    IdleHour = s.IdleHour,
                    GravelKm = s.Gravel,
                    CategoryType = s.Category,
                    NoOfTrip = s.NoOfTripe,
                    AsphaltKm = s.Asphalt
                });
            }
            else if (timesheetType == TimeSheetType.WagonDrill)
            {
                var wagonDrill = db.WagonDrillTimesheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                timesheet = wagonDrill.Select(s => new TimesheetVm
                {
                    FleetTimeSheetId = s.WagonDrillTimesheetId,
                    TimeSheetType = TimeSheetType.WagonDrill,
                    OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == s.OperatorId)?.FullName ?? "",
                    BranchName = db.Branch.FirstOrDefault(x => x.BranchId == s.BranchId)?.Name ?? "",
                    BusinessUnitName = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == s.BusinessUnitId)?.Name ?? "",
                    NoOfHolesDrilled = s.NumberOfHolesDrilled,
                    FuelInLiter = s.FuelInLiter,
                    ReasonForDown = s.ReasonForDown,
                    ReasonForIdle = s.ReasonForIdle,
                    Date = s.Date,
                    HoleDepth = s.OperationDrilledHoleDepth,
                    WorkItem = s.WorkItem,
                    IdleHour = s.IdleHour,
                    DownTime = s.DownTime

                });
            }
            return Json(timesheet.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Read_CostSheet([DataSourceRequest]DataSourceRequest request, Guid id)
        {
            var fleet = db.Fleets.Find(id);
            var equipmentType = db.EquipmentType.Find(fleet.EquipmentTypeID);
            var timesheetType = equipmentType.TimeSheetType;

            IEnumerable<CostSheetVm> costsheet = new List<CostSheetVm>();
            if (timesheetType == TimeSheetType.Machinery)
            {
                var machinery = db.MachineryWorkingTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                costsheet = machinery.Select(s => new CostSheetVm
                {
                    TimeSheetType = TimeSheetType.Machinery,
                    OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == s.OperatorId)?.FullName ?? "",
                    BranchName = db.Branch.FirstOrDefault(x => x.BranchId == s.BranchId)?.Name ?? "",
                    BusinessUnitName = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == s.BusinessUnitId)?.Name ?? "",
                    Date = s.Date,
                    DownHour = s.DownHours,
                    WorkType = s.WorkType,
                    IdleHour = s.IdleHours,
                    FuelRate = s.FuelRate,
                    MaintenanceCost = s.MaintenanceCost,
                    Perdium = s.Perdium,
                    Salary = s.Salary,
                    GreaseRate = s.GreaseRate,
                    TireCost = s.TireCost,
                    ServiceCharge = s.ServiceRecharge
                });
            }
            else if (timesheetType == TimeSheetType.LightVehicle)
            {
                var lightVehicle = db.LightVehicleTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                costsheet = lightVehicle.Select(s => new CostSheetVm
                {
                    OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == s.OperatorId)?.FullName ?? "",
                    BranchName = db.Branch.FirstOrDefault(x => x.BranchId == s.BranchId)?.Name ?? "",
                    BusinessUnitName = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == s.BusinessUnitId)?.Name ?? "",
                    Date = s.Date,
                    DownHour = s.DownHours,
                    WorkType = s.WorkType,
                    IdleHour = s.IdleHours,
                    FuelRate = s.FuelRate,
                    MaintenanceCost = s.MaintenanceCost,
                    Perdium = s.Perdium,
                    Salary = s.Salary,
                    GreaseRate = s.GreaseRate,
                    TireCost = s.TireCost,
                    ServiceCharge = s.ServiceRecharge

                });
            }
            else if (timesheetType == TimeSheetType.DumpTruck)
            {
                var dumpTruck = db.DumpTruckTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                costsheet = dumpTruck.Select(s => new CostSheetVm
                {
                    TimeSheetType = TimeSheetType.DumpTruck,
                    OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == s.OperatorId)?.FullName ?? "",
                    BranchName = db.Branch.FirstOrDefault(x => x.BranchId == s.BranchId)?.Name ?? "",
                    BusinessUnitName = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == s.BusinessUnitId)?.Name ?? "",
                    Date = s.Date,
                    TireCost = s.TireCost,
                    MaintenanceCost = s.MaintenanceCost,
                    LubricantCost = s.LubricantCost,
                    Perdium = s.Perdium,
                    Salary = s.Salary,
                    FuelRate = s.FuelRate,

                    IdleHour = s.IdleHour,

                    DownHour = s.DownHour,


                });
            }
            else if (timesheetType == TimeSheetType.DryLoading)
            {
                var dryLoad = db.DryLoadingTimeSheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                costsheet = dryLoad.Select(s => new CostSheetVm
                {
                    TimeSheetType = TimeSheetType.DryLoading,
                    OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == s.OperatorId)?.FullName ?? "",
                    BranchName = db.Branch.FirstOrDefault(x => x.BranchId == s.BranchId)?.Name ?? "",
                    BusinessUnitName = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == s.BusinessUnitId)?.Name ?? "",
                    WorkItem = s.WorkItem,
                    FuelInLiter = s.FuelInLiter,
                    AsphaltRate = s.AsphaltRate,
                    GravelRate = s.GravelRate,
                    LubricantCost = s.LubricantCost,
                    FuelRate = s.FuelRate,
                    MaintenanceCost = s.MaintenanceCost,
                    Perdium = s.Perdium,
                    Salary = s.Salary,
                    TireCost = s.TireCost,

                    IdleHour = s.IdleHour,

                });
            }
            else if (timesheetType == TimeSheetType.WagonDrill)
            {
                var wagonDrill = db.WagonDrillTimesheets.Where(x => x.FleetsID == fleet.FleetsID).ToList();
                costsheet = wagonDrill.Select(s => new CostSheetVm
                {
                    TimeSheetType = TimeSheetType.WagonDrill,
                    OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == s.OperatorId)?.FullName ?? "",
                    BranchName = db.Branch.FirstOrDefault(x => x.BranchId == s.BranchId)?.Name ?? "",
                    BusinessUnitName = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == s.BusinessUnitId)?.Name ?? "",
                    FuelInLiter = s.FuelInLiter,
                    Date = s.Date,
                    TireCost = s.TireCost,
                    ServiceCharge = s.ServiceCharge,
                    MaintenanceCost = s.MaintenanceCost,
                    Perdium = s.Perdium,
                    Salary = s.Salary,
                    FuelRate = s.FuelRate,
                    WorkItem = s.WorkItem,
                    IdleHour = s.IdleHour,
                    DownTime = s.DownTime

                });
            }
            return Json(costsheet.ToDataSourceResult(request, ModelState));
        }

        public ActionResult TireAndBattery_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            var tireAndBatteryIssue = db.TireIssue.Where(x => x.FleetId == id).ToList();
            var insuranceVm = tireAndBatteryIssue.Select(s => new TyreAndBatteryViewModel
            {
               Brand = s.Brand,
               IssueType = s.IssueType,
               OperatorName = s.OperatorName,
               PlateNumber = s.PlateNumber,
               SerialNumber = s.SerialNo,
               Status = s.Status,
               TyreAndBatteryId = s.TireIssueID,
               TireSize = s.TireSize,
               UnitPrice = s.UnitPrice
            });
            return Json(insuranceVm.ToDataSourceResult(request, ModelState));
        }
    }
}