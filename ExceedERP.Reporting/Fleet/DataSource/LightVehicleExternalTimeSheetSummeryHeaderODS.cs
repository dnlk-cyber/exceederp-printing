using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class LightVehicleExternalTimeSheetSummeryHeaderODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public MachineryExternalTimeSheetSummeryHeaderVM LightVehicleExternalHeader(string fleetId, DateTime from, DateTime to, string project)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new MachineryExternalTimeSheetSummeryHeaderVM();
            if (fleetId != null)
            {
                Guid FId = Guid.Parse(fleetId);

                var lightVehicle = db.LightVehicleTimeSheets.FirstOrDefault(x => x.FleetsID == FId && DbFunctions.TruncateTime(x.Date) >= from.Date && DbFunctions.TruncateTime(x.Date) <= to.Date);

                if (lightVehicle != null)
                {
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == lightVehicle.BranchId);
                    var fleet = db.Fleets.Find(FId);
                    var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
                    var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
                    var rentFromExternals = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == fleet.RentId);
                    var equipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID);
                    var operatorName = db.Operator.FirstOrDefault(x => x.OperatorID == lightVehicle.OperatorId);
                    if (rentFromExternals != null)
                    {
                        procurementSupplier = db.ProcurementSuppliers.FirstOrDefault(x => x.ProcurementSupplierID == rentFromExternals.ProcurementSupplierID);
                    }
                    if (equipment != null && fleet != null)
                    {
                        if (equipment != null)
                            header.EquipmentType = equipment.EquipmentTypeName;
                        if (equipmentName != null)
                            header.EquipmentName = equipmentName.Name;
                        header.PlateNumber = fleet.PlateNumber;
                        header.OwnerName = procurementSupplier?.Name == null ? db.Companies.FirstOrDefault()?.TradeName : "";
                        if (operatorName != null)
                            header.OperatorName = operatorName.FullName;
                        if (branch != null)
                            header.Project = branch.Name;
                    }
                    header.DurationFrom = from.Date;


                    header.DurationTo = to.Date;


                    header.Duration = from.ToString("MM/dd/yyyy") + "-" + to.ToString("MM/dd/yyyy");

                    header.Period = DateTime.Now.Month.ToString() + "," + DateTime.Now.Year.ToString();
                }
            }

            return header;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MachineryExternalTimeSheetSummeryVM> LightVehicleExternal(string fleetId, DateTime from, DateTime to, string project, string categoryId, string subCategoryId)
        {

            var list = new List<MachineryExternalTimeSheetSummeryVM>();
            if (fleetId != null)
            {

                Guid FId = Guid.Parse(fleetId);

                var lightVehicleTimeSheets = db.LightVehicleTimeSheets.OrderByDescending(x => x.Date).Where(x => x.FleetsID == FId).ToList();


                lightVehicleTimeSheets = lightVehicleTimeSheets.Where(x => x.Date.Date >= from.Date && x.Date.Date <= to.Date).ToList();


                if (project != null)
                {
                    int.TryParse(project, out int branchId);
                    lightVehicleTimeSheets = lightVehicleTimeSheets.Where(x => x.BranchId == branchId).ToList();
                }
                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    Guid.TryParse(categoryId, out Guid id);
                    List<LightVehicleTimeSheet> lightVehicleList = new List<LightVehicleTimeSheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var lightTimesheetList = lightVehicleTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        lightVehicleList.AddRange(lightTimesheetList);
                    }
                    lightVehicleTimeSheets = lightVehicleList;

                }
                if (!string.IsNullOrWhiteSpace(subCategoryId))
                {
                    Guid.TryParse(subCategoryId, out Guid id);
                    List<LightVehicleTimeSheet> lightVehicleList = new List<LightVehicleTimeSheet>();
                    var fleets = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
                    foreach (var item in fleets)
                    {
                        var lightVehicleTimesheetList = lightVehicleTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        lightVehicleList.AddRange(lightVehicleTimesheetList);
                    }
                    lightVehicleTimeSheets = lightVehicleList;
                }
                foreach (var lightVehicleTimeSheet in lightVehicleTimeSheets)
                {
                    var timeSheet = new MachineryExternalTimeSheetSummeryVM();
                    timeSheet.Date = lightVehicleTimeSheet.Date;
                    timeSheet.WorkItem = lightVehicleTimeSheet.WorkType;
                    //
                    timeSheet.OperationNetworkinHour = 1;
                    timeSheet.OperationRatePerHour = lightVehicleTimeSheet.RentalRate;
                    //
                    timeSheet.OpertionRevenue = timeSheet.OperationNetworkinHour * timeSheet.OperationRatePerHour;

                    timeSheet.IdleHour = lightVehicleTimeSheet.IdleHours;
                    timeSheet.IdleRatePerHour = lightVehicleTimeSheet.IdealRentalRate;
                    //----
                    timeSheet.IdleRevenue = timeSheet.IdleHour * timeSheet.IdleRatePerHour;

                    timeSheet.DownHour = lightVehicleTimeSheet.DownHours;
                    //----
                    timeSheet.TotalAmountBeforeVat = timeSheet.IdleRevenue + timeSheet.OpertionRevenue;
                    //----
                    timeSheet.VAT = 0.15M;
                    //----
                    timeSheet.TotalAmount = timeSheet.TotalAmountBeforeVat * timeSheet.VAT + timeSheet.TotalAmountBeforeVat;

                    timeSheet.FuelInLiter = lightVehicleTimeSheet.FuelConsumption;
                    timeSheet.FuelRate = lightVehicleTimeSheet.FuelRate;
                    //----
                    timeSheet.FuelCost = timeSheet.FuelInLiter * timeSheet.FuelRate;
                    timeSheet.LubricantCost = lightVehicleTimeSheet.LubricantCost;
                    timeSheet.MaintenanceCost = lightVehicleTimeSheet.MaintenanceCost;
                    timeSheet.ServiceCharge = lightVehicleTimeSheet.Salary; // ??

                    //----
                    timeSheet.TotalCost = timeSheet.ServiceCharge + timeSheet.FuelCost + timeSheet.LubricantCost + timeSheet.MaintenanceCost;
                    timeSheet.NetAmount = timeSheet.TotalAmount - timeSheet.TotalCost;
                    list.Add(timeSheet);
                }
                return list.OrderBy(x => x.Date.Date).ToList();
            }
            return list;
        }

    }
}
