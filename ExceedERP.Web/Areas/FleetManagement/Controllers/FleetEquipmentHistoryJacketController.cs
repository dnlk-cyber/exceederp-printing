using ExceedERP.Core.Domain.CRM;
using ExceedERP.Core.Domain.Finance.AP.Travel;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Core.Domain.FleetManagement.FleetViewModel;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
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
            ViewData["Category"] = db.EquipmentIdentity.Select(s => new
            {
                
                Value = s.EquipmentIdentityID,
                Text = s.EquipmentCategoryName
            }).ToList();
            ViewData["Type"] = db.EquipmentType.Select(s => new
            {
                Value = s.EquipmentTypeID,
                Text = s.EquipmentTypeName
            }).ToList();

            ViewData["Name"] = db.EquipmentName.Select(s => new
            {
                Value = s.EquipmentNameID,
                Text = s.Name
            }).ToList();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Text = s.PlateNumber,
                Value = s.FleetsID
            }).ToList();
            return View();
        }

        public ActionResult Equipment_Read([DataSourceRequest] DataSourceRequest request)
        {
            var equip = db.Fleets.AsQueryable();
            var equipmentHistoryJacket = equip.Select(s => new FleetEquipmentHistoryJacketViewModel
            {
                FleetId =s.FleetsID,
                EquipmentCategoryName = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == s.EquipmentIdentityID).EquipmentCategoryName,
                EquipmentTypeName = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == s.EquipmentTypeID).EquipmentTypeName,
                EquipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameID).Name,
                PlateNo = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetsID).PlateNumber,
                PurchasingCost = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetsID).PurchasingCost.ToString(),
                SerialNo = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetsID).SerialNo
            });
            return Json(equipmentHistoryJacket.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Read_BoloRegistrationHistory([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            var boloreg = db.FleetBoloRegistrations.Where(x => x.FleetId == id).ToList();

            var fleetBoloJacket = boloreg.Select(s => new BoloRegistrationHistoryVM
            {
                FleetBoloRegistrationId = s.FleetBoloRegistrationId,
                BoloExpireDate = s.BoloExpireDate,
                BoloRegistrationDate = s.BoloRegistrationDate,
                EquipmentCategoryId = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == s.EquipmentCategoryId).EquipmentCategoryName,
                EquipmentTypeId = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == s.EquipmentTypeId).EquipmentTypeName,
                EquipmentNameId = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == s.EquipmentNameId).Name,
                FleetId = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetId).PlateNumber,
                SerialNo = db.Fleets.FirstOrDefault(x => x.FleetsID == s.FleetId).SerialNo,
                Remark = s.Remark
            });
            return Json(fleetBoloJacket.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Read_DryLoadingTimeSheetHistory([DataSourceRequest] DataSourceRequest request, Guid id)
        {

            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var dryloadingHistory = db.DryLoadingTimeSheets.Where(x=>mybraches.Contains(x.BranchId)&&x.FleetsID==id).ToList();

            var dryLoadingJacket = dryloadingHistory.Select(dryLoadingTimeSheet => new DryLoadingTimesheetHistoryVM
            {
                DryLoadingTimeSheetId = dryLoadingTimeSheet.DryLoadingTimeSheetId,
                TripStartDate = dryLoadingTimeSheet.TripStartDate,
                TripArrivalDate = dryLoadingTimeSheet.TripArrivalDate,
                EquipmentCategoryId = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == dryLoadingTimeSheet.EquipmentCategoryId).EquipmentCategoryName,
                EquipmentTypeId = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == dryLoadingTimeSheet.EquipmentTypeId).EquipmentTypeName,
                EquipmentNameId = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == dryLoadingTimeSheet.EquipmentNameId).Name,
                FleetsID = db.Fleets.FirstOrDefault(x => x.FleetsID == dryLoadingTimeSheet.FleetsID).PlateNumber,
                WorkItem = dryLoadingTimeSheet.WorkItem,
                WorkingHour = dryLoadingTimeSheet.WorkingHour,
                TripStartPlace = dryLoadingTimeSheet.TripStartPlace,
                TriplArrivalPlace = dryLoadingTimeSheet.TriplArrivalPlace,               
                Asphalt = dryLoadingTimeSheet.Asphalt,
                Gravel = dryLoadingTimeSheet.Gravel,
                TotalKmTravelled = dryLoadingTimeSheet.TotalKmTravelled,
                FuelInLiter = dryLoadingTimeSheet.FuelInLiter,
                PerDiem = dryLoadingTimeSheet.PerDiem,
                IdleHour = dryLoadingTimeSheet.IdleHour,
                ReasonForIdle = dryLoadingTimeSheet.ReasonForIdle,              
                DownReason = dryLoadingTimeSheet.DownReason,
                OilConsumption = dryLoadingTimeSheet.OilConsumption,
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
               // ProcurementSupplier = dryLoadingTimeSheet.ProcurementSupplier,
                Salary = dryLoadingTimeSheet.Salary,
                TireCost = dryLoadingTimeSheet.TireCost,
                BranchId = db.Branch.FirstOrDefault(x => x.BranchId == dryLoadingTimeSheet.BranchId).Name,
                BusinessUnitId = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == dryLoadingTimeSheet.BusinessUnitId).Name,
                OperatorId = db.Operator.FirstOrDefault(x => x.OperatorID == dryLoadingTimeSheet.OperatorId).FullName,
               // TimeSheetStatus = dryLoadingTimeSheet.TimeSheetStatus,
                Downhour = dryLoadingTimeSheet.Downhour,
                

            });
            return Json(dryLoadingJacket.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Read_DumpTruckTimeSheetHistory([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            List<int> mybraches = UserInformationHelper.GetUserBranchsAsInt(User.Identity.Name);
            var dumpTruckTimeSheets = db.DumpTruckTimeSheets.Where(x => mybraches.Contains(x.BranchId)&&x.FleetsID==id);

            var dumptruckJacket = dumpTruckTimeSheets.Select(dumpTruckTimeSheet => new DumpTruckTimeSheetViewModel
            {
                DumpTruckTimeSheetId = dumpTruckTimeSheet.DumpTruckTimeSheetId,
                FleetsID = db.Fleets.FirstOrDefault(x => x.FleetsID == dumpTruckTimeSheet.FleetsID).PlateNumber,
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
                RentalRate = dumpTruckTimeSheet.RentalRate,
                IdealRentalRate = dumpTruckTimeSheet.IdealRentalRate,
                LubricantCost = dumpTruckTimeSheet.LubricantCost,
                FuelRate = dumpTruckTimeSheet.FuelRate,
                MaintenanceCost = dumpTruckTimeSheet.MaintenanceCost,
                NetworkingHour = dumpTruckTimeSheet.NetworkingHour,
                Perdium = dumpTruckTimeSheet.Perdium,
                Salary = dumpTruckTimeSheet.Salary,
                TireCost = dumpTruckTimeSheet.TireCost,
                BranchId = db.Branch.FirstOrDefault(x => x.BranchId == dumpTruckTimeSheet.BranchId).Name,
                BusinessUnitId = db.BusinessUnits.FirstOrDefault(x => x.BusinessUnitId == dumpTruckTimeSheet.BusinessUnitId).Name,
                OperatorId = db.Operator.FirstOrDefault(x => x.OperatorID == dumpTruckTimeSheet.OperatorId).FullName,               
                EquipmentCategoryId = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == dumpTruckTimeSheet.EquipmentCategoryId).EquipmentCategoryName,
                EquipmentTypeId = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == dumpTruckTimeSheet.EquipmentTypeId).EquipmentTypeName,
                EquipmentNameId = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == dumpTruckTimeSheet.EquipmentNameId).Name,

            });

            return Json(dumptruckJacket.ToDataSourceResult(request, ModelState));
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

        public ActionResult Distribution_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            List<EquipmentDistributionViewModel> list = new List<EquipmentDistributionViewModel>();
            var assignedEquip = db.AssignedEquipment.Where(x => x.FleetId == id).ToList();
            foreach (var item in assignedEquip)
            {
                var reqEquipment = db.RequestedEquipments.FirstOrDefault(x => x.VehicleDemandId == item.VehicleDemandId);
                var additionalInfo = db.AssignedEquipmentAdditionalInfos.FirstOrDefault(x => x.AssignedEquipmentId == item.AssignedEquipmentID);
                if(reqEquipment != null)
                {

                    list.Add(new EquipmentDistributionViewModel
                    {
                        AssignedEqiupmentId = item.AssignedEquipmentID,
                        Destination = reqEquipment?.Destination,
                        StartDate = reqEquipment?.StartDate,
                        EndDate = reqEquipment?.EndDate,
                        Quantity = reqEquipment?.Quantity ?? 0,
                        OperatorName = db.Operator.FirstOrDefault(x => x.OperatorID == item.OperatorId)?.FullName,
                        StartOdometer = item.StartOdometr,
                        EndOdometer = item.EndOdometer,
                        NetOdometer = item.NetOdometer,
                        TotalCost = additionalInfo?.TotalBirr ?? 0
                    });
                }

            }
            return Json(list.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Fuel_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            List<FuelConsumptionViewModel> list = new List<FuelConsumptionViewModel>();
            var assignedEquipments = db.AssignedEquipment.AsNoTracking().Where(x => x.FleetId == id).ToList();
            var groupedAssignedEquipments = assignedEquipments.GroupBy(x => x.FleetId).ToList();
            var fleetIds = groupedAssignedEquipments.Select(g => g.Key).ToList();
            var fleets = db.Fleets.Where(f => fleetIds.Contains(f.FleetsID)).ToList();
            var equipmentTypeIds = fleets.Select(f => f.EquipmentTypeID).Distinct().ToList();
            var equipmentTypes = db.EquipmentType.Where(et => equipmentTypeIds.Contains(et.EquipmentTypeID)).ToList();
            var equipmentNameIds = fleets.Select(f => f.EquipmentNameID).Distinct().ToList();
            var equipmentNames = db.EquipmentName.Where(en => equipmentNameIds.Contains(en.EquipmentNameID)).ToList();
            
            foreach (var group in groupedAssignedEquipments)
            {
                var fleet = fleets.FirstOrDefault(f => f.FleetsID == group.Key);
                if (fleet != null)
                {
                   
                    var totalFuelFromStore = GetFuelFromIssueItem(fleet.PlateNumber);
                    var totalFuelFromTravelOrder = GetTotalFuelFromTravelOrder(group);

                    list.Add(new FuelConsumptionViewModel
                    {
                        FleetId = fleet.FleetsID,
                        TotalFuelFromStore = totalFuelFromStore,
                        TotalFuelFromTravelOrder = totalFuelFromTravelOrder,
                        TotalFuelConsumption = totalFuelFromStore + totalFuelFromTravelOrder
                    });
                    
                }
            }
            return Json(list.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Maintenace_Read([DataSourceRequest] DataSourceRequest request, Guid id)
        {
            List<MaintenanceViewModel> list = new List<MaintenanceViewModel>();
            var jobOrder = db.JobOrder.Where(x => x.FleetsID == id).ToList();
            var jobNums = jobOrder.Select(s => s.Num);
            foreach (var item in jobNums)
            {
                list.Add(new MaintenanceViewModel
                {
                    MaitenanceServiceCost = CalculateServiceCost(item),
                    AdditionalCost = CalculateAdditionalCost(item),
                    SparePartsCost = CalculateSparePartCost(item),
                    TotalCost = CalculateTotalCost(item),
                    JobNumber = GetJobNum(item),
                    MaintenanceCenter = GetServiceCenter(item),
                    ServiceType = GetServiceType(item)
                });
            }
            return Json(list.ToDataSourceResult(request, ModelState));
        }


        private string GetJobNum(int jobNum)
        {
            return db.JobOrder.FirstOrDefault(x => x.Num == jobNum)?.Num.ToString() ?? "";
        }
        private string GetServiceCenter(int jobNum)
        {
            return db.JobOrder.FirstOrDefault(x => x.Num == jobNum)?.MaintenanceCenter ?? "";
        }
        private string GetServiceType(int jobNum)
        {
            return db.JobOrder.FirstOrDefault(x => x.Num == jobNum)?.Type ?? "";
        }
        private decimal CalculateServiceCost(int jobNum)
        {

            var salesQuote = db.SalesQuoteVouchers
              .Where(x => x.Source == SalesQuoteSource.Maintenance)
              .AsEnumerable() // Switch to in-memory filtering to use TryParse
              .FirstOrDefault(x => int.TryParse(x.JobOrderNumber, out var jobOrderNumber) && jobOrderNumber == jobNum);
            if (salesQuote == null)
                return 0;
            var serviceLineItems = db.ServiceLineItems.Where(t => t.VoucherId == salesQuote.SalesQuoteVoucherID && t.VoucherType == CRMVoucherType.SalesQuote).ToList();

            return serviceLineItems.Sum(x => (decimal?)x.UnitAmount) ?? 0;

        }

        private decimal CalculateSparePartCost(int jobNum)
        {
            var salesQuote = db.SalesQuoteVouchers
              .Where(x => x.Source == SalesQuoteSource.Maintenance)
              .AsEnumerable() // Switch to in-memory filtering to use TryParse
              .FirstOrDefault(x => int.TryParse(x.JobOrderNumber, out var jobOrderNumber) && jobOrderNumber == jobNum);
            if (salesQuote == null)
                return 0;
            var lineItems = db.LineItems.Where(t => t.Voucher == salesQuote.SalesQuoteVoucherID.ToString() && t.VoucherDefinitionID == "0").ToList();

            return lineItems.Sum(x => (decimal?)x.UnitAmount) ?? 0;

        }

        private decimal CalculateAdditionalCost(int jobNum)
        {
            var workShop = db.ServiceCenterJob.FirstOrDefault(x => x.Num == jobNum);
            if (workShop == null)
                return 0;
            return db.AdditionalWorkShopDetails.Where(x => x.ServiceCenterJobId == workShop.ServiceCenterJobId).Sum(x => (decimal?)x.OtherCosts) ?? 0;
        }
        private decimal CalculateTotalCost(int jobNum)
        {
            return CalculateServiceCost(jobNum) + CalculateSparePartCost(jobNum) + CalculateAdditionalCost(jobNum);
        }


        private decimal GetFuelFromIssueItem(string plateNo)
        {
            var query = db.IssueItems.AsNoTracking().Where(ii => ii.Issue.IsFuel);
            if (query == null)
                return 0;
            
            if (!string.IsNullOrWhiteSpace(plateNo))
            {
                query = query.Where(ii => ii.Issue.PlateNo == plateNo);
            }

            return query.Sum(ii => (decimal?)ii.Total) ?? 0;
        }
        private decimal GetTotalFuelFromTravelOrder(IEnumerable<AssignedEquipment> assignedEquipments)
        {
            var totalFuelFromTravelOrder = 0m;
            var vehicleDemandIds = assignedEquipments.Select(ae => ae.VehicleDemandId).ToList();
            var requestedEquipments = db.RequestedEquipments.Where(re => vehicleDemandIds.Contains(re.VehicleDemandId)).ToList();

            var requestNumbers = requestedEquipments.Select(re => re.RequestNumber).ToList();
            var vehicleDistributions = db.VehicleDistribution.Where(vd => requestNumbers.Contains(vd.RequestID)).ToList();

            foreach (var vd in vehicleDistributions)
            {
                totalFuelFromTravelOrder += GetFuelSumFromTravelOrder(vd.VehicleDistributionID);
            }

            return totalFuelFromTravelOrder;
        }
        private decimal GetFuelSumFromTravelOrder(Guid vehicleDistributionId)
        {
            var query = db.ApTravelOrders.AsNoTracking().Where(ato => ato.VehicleDistributionID == vehicleDistributionId);
            if (query == null)
                return 0;                    
            var travelOrders = query.ToList();
            if (!travelOrders.Any())
            {
                return 0m;
            }

            var travelOrderIds = travelOrders.Select(ato => ato.ApTravelOrderId).ToList();

            var advancePayments = db.ApTravelOrderAdvancePayments
                                   .Where(ap => travelOrderIds.Contains(ap.ApTravelOrderId) && ap.Type == TravelPaymentType.Fuel)
                                   .Sum(ap => (decimal?)ap.Total) ?? 0;

            var expenseClaims = db.ApTravelExpenseClaims
                                  .Where(ec => travelOrderIds.Contains((int)ec.ApTravelOrderId))
                                  .ToList();

            var claimIds = expenseClaims.Select(ec => ec.ApTravelExpenseClaimId).ToList();

            var claimExpenses = db.ApTravelExpenseExpenses
                                   .Where(ce => claimIds.Contains(ce.ApTravelExpenseClaimId) && ce.Type == TravelPaymentType.Fuel)
                                   .Sum(ce => (decimal?)ce.Total) ?? 0;

            return advancePayments + Math.Max(0, claimExpenses - advancePayments);
        }
    }
}