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
    public class LightVehicleTimeSheetSummeryHeaderODSSummary
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public MachineryTimeSheetSummeryHeaderVM LightVehicleTimeSheetSummeryHeader(DateTime from, DateTime to, string project)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new MachineryTimeSheetSummeryHeaderVM();
            if (project != null)
            {
                int.TryParse(project, out int FId);

                var lightVehicle = db.LightVehicleTimeSheets.FirstOrDefault(x => x.BranchId == FId && DbFunctions.TruncateTime(x.Date) >= from.Date && DbFunctions.TruncateTime(x.Date) <= to.Date);
                if (lightVehicle != null)
                {
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == lightVehicle.BranchId);
                    var fleet = db.Fleets.Find(lightVehicle.FleetsID);
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
                        if (fleet.IsExternal)
                        {
                            if (procurementSupplier != null)
                            {
                                header.OwnerName = procurementSupplier?.Name;
                            }
                            else
                            {
                                header.OwnerName = "";
                            }
                        }
                        else
                        {
                            header.OwnerName = db.Companies.FirstOrDefault()?.TradeName;
                        }
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
        public List<MachineryTimeSheetSummeryVM> LightVehicleTimeSheetSummery(DateTime from, DateTime to,string IntExt, string project, string businessUnitId, string categoryId, string subCategoryId)
        {
            var list = new List<MachineryTimeSheetSummeryVM>();

            if (string.IsNullOrWhiteSpace(project))
            {
                return list;
            }

            int FId = int.Parse(project);

            // Get light vehicle timesheets for the given project, sorted by Date
            var fromDate = from.Date;
            var toDate = to.Date;
            var lightVehicleTimeSheet = db.LightVehicleTimeSheets.AsNoTracking().ToList();
            var lightVehicleTimeSheets = lightVehicleTimeSheet
                .Where(x => x.BranchId == FId &&
                            x.CostSheetApproved &&
                            x.Date >= fromDate &&
                            x.Date <= toDate)
                .OrderByDescending(x => x.Date)
                .ToList();

            // Apply additional filtering if required
            if (!string.IsNullOrWhiteSpace(businessUnitId))
            {
                lightVehicleTimeSheets = lightVehicleTimeSheets
                    .Where(x => x.BusinessUnitId == int.Parse(businessUnitId))
                    .ToList();
            }
            if (!string.IsNullOrWhiteSpace(IntExt))
            {
                List<LightVehicleTimeSheet> LightList = new List<LightVehicleTimeSheet>();
                if (IntExt == "Rent")
                {


                    var fleetFiltered = db.Fleets.Where(x => x.IsExternal).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var  LightFiltered = lightVehicleTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        LightList.AddRange(LightFiltered);
                    }
                    lightVehicleTimeSheets = LightList;
                }
                else
                {
                    var fleetFiltered = db.Fleets.Where(x => !x.IsExternal).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var LightFiltered = lightVehicleTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        LightList.AddRange(LightFiltered);
                    }
                    lightVehicleTimeSheets = LightList;
                }
            }
            if (!string.IsNullOrWhiteSpace(categoryId) && Guid.TryParse(categoryId, out Guid categoryGuid))
            {
                var fleetIds = db.Fleets.Where(x => x.EquipmentIdentityID == categoryGuid).Select(f => f.FleetsID).ToList();
                lightVehicleTimeSheets = lightVehicleTimeSheets.Where(x => fleetIds.Contains(x.FleetsID)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(subCategoryId) && Guid.TryParse(subCategoryId, out Guid subCategoryGuid))
            {
                var fleetIds = db.Fleets.Where(x => x.EquipmentTypeID == subCategoryGuid).Select(f => f.FleetsID).ToList();
                lightVehicleTimeSheets = lightVehicleTimeSheets.Where(x => fleetIds.Contains(x.FleetsID)).ToList();
            }

            // Group data by FleetsID
            var groupedData = lightVehicleTimeSheets
                .GroupBy(lv => lv.FleetsID)
                .Select(group =>
                {
                    var firstEntry = group.First(); // First record for static fields

                    var fleet = db.Fleets.FirstOrDefault(f => f.FleetsID == firstEntry.FleetsID);
                    var category = db.EquipmentIdentity.FirstOrDefault(eq => eq.EquipmentIdentityID == firstEntry.EquipmentCategoryId);
                    var equipment = db.EquipmentName.FirstOrDefault(eq => eq.EquipmentNameID == firstEntry.EquipmentNameId);

                    var timeSheet = new MachineryTimeSheetSummeryVM();

                    timeSheet.Date = firstEntry.Date;
                        timeSheet.WorkItem = null; // Combine unique work items
                        timeSheet.Category = category?.EquipmentCategoryName ?? "Unknown";
                        timeSheet.EquipmentName = equipment?.Name ?? "Unknown";
                        timeSheet.PlateNumber = fleet?.PlateNumber ?? "N/A";
                    decimal netHour = 0;
                    decimal operationNetWH = 0;
                        foreach(var g in group)
                    {
                        netHour = Math.Round((decimal)((g.MorningTimeEnd - g.MorningTimeStart).TotalHours + (g.AfternoonTimeEnd - g.AfternoonTimeStart).TotalHours + (g.NightTimeEnd - g.NightTimeStart).TotalHours), 2);
                        if (netHour > 4)
                            operationNetWH = 1;
                        if (netHour <= 4)
                            operationNetWH = 0.5m;
                        if (netHour <= 0)
                            operationNetWH = 0;
                        timeSheet.OperationNetworkinHour+= operationNetWH;
                        timeSheet.OperationRatePerHour = g.RentalRate;
                        timeSheet.OpertionRevenue+= operationNetWH * timeSheet.OperationRatePerHour;
                        timeSheet.IdleRevenue+= g.IdleHours * g.IdealRentalRate;
                        timeSheet.TotalRevenue+= (g.IdleHours * g.IdealRentalRate) + (operationNetWH * timeSheet.OperationRatePerHour);

                    }
                    timeSheet.OperationRatePerHour = group.Average(lv => lv.RentalRate); 
                    timeSheet.IdleHour = group.Sum(lv => lv.IdleHours);
                        timeSheet.IdleRatePerHour = group.Average(lv => lv.IdealRentalRate);
                        timeSheet.DownHour = group.Sum(lv => lv.DownHours);
                        timeSheet.FuelInLiter = group.Sum(lv => lv.FuelConsumption);
                    timeSheet.FuelRate = firstEntry.FuelRate; // Assuming rate is fixed
                        timeSheet.FuelCost = group.Sum(lv => lv.FuelConsumption * lv.FuelRate);
                        timeSheet.LubricantCost = group.Sum(lv => lv.LubricantCost);
                        timeSheet.MaintenanceCost = group.Sum(lv => lv.MaintenanceCost);
                        timeSheet.TireCost = group.Sum(lv => lv.TireCost);
                        timeSheet.Perdium = group.Sum(lv => lv.Perdium);
                       timeSheet.Salary = group.Sum(lv => lv.Salary);

                        // Total calculations
                       // timeSheet.TotalRevenue = 0; // Will be calculated below
                        timeSheet.TotalCost = 0;
                       timeSheet.NetIncome = 0;
                    

                    // Final calculations
                   // timeSheet.TotalRevenue = timeSheet.IdleRevenue + timeSheet.OpertionRevenue;
                    timeSheet.TotalCost = timeSheet.LubricantCost + timeSheet.MaintenanceCost + timeSheet.TireCost + timeSheet.Perdium + timeSheet.Salary + timeSheet.FuelCost;
                    timeSheet.NetIncome = timeSheet.TotalRevenue - timeSheet.TotalCost;

                    return timeSheet;
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Assign sequential SN
            int snCounter = 1;


            return groupedData;
        }
        //public List<MachineryTimeSheetSummeryVM> LightVehicleTimeSheetSummery(DateTime from, DateTime to, string project, string businessUnitId, string categoryId, string subCategoryId)
        //    {

        //        var list = new List<MachineryTimeSheetSummeryVM>();
        //        if (project != null)
        //        {


        //            int FId = int.Parse(project);

        //            var lightVehicleTimeSheets = db.LightVehicleTimeSheets.OrderByDescending(x => x.Date).Where(x => x.BranchId == FId && x.CostSheetApproved).ToList();


        //            lightVehicleTimeSheets = lightVehicleTimeSheets.Where(x => x.Date.Date >= from.Date && x.Date.Date <= to.Date).ToList();


        //            if (!string.IsNullOrWhiteSpace(businessUnitId))
        //            {
        //                lightVehicleTimeSheets = lightVehicleTimeSheets.Where(x => x.BusinessUnitId == int.Parse(businessUnitId)).ToList();
        //            }
        //            if (!string.IsNullOrWhiteSpace(categoryId))
        //            {
        //                Guid.TryParse(categoryId, out Guid id);
        //                List<LightVehicleTimeSheet> lightVehicleList = new List<LightVehicleTimeSheet>();
        //                var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
        //                foreach (var item in fleetFiltered)
        //                {
        //                    var lightVehicleTimesheetList = lightVehicleTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
        //                    lightVehicleList.AddRange(lightVehicleTimesheetList);
        //                }
        //                lightVehicleTimeSheets = lightVehicleList;

        //            }
        //            if (!string.IsNullOrWhiteSpace(subCategoryId))
        //            {
        //                Guid.TryParse(subCategoryId, out Guid id);
        //                List<LightVehicleTimeSheet> lightVehicleList = new List<LightVehicleTimeSheet>();
        //                var fleets = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
        //                foreach (var item in fleets)
        //                {
        //                    var lightVehicleTimesheetList = lightVehicleTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
        //                    lightVehicleList.AddRange(lightVehicleTimesheetList);
        //                }
        //                lightVehicleTimeSheets = lightVehicleList;
        //            }
        //            foreach (var lightVehicleTimeSheet in lightVehicleTimeSheets)
        //            {
        //                var netHour = Math.Round((decimal)((lightVehicleTimeSheet.MorningTimeEnd - lightVehicleTimeSheet.MorningTimeStart).TotalHours + (lightVehicleTimeSheet.AfternoonTimeEnd - lightVehicleTimeSheet.AfternoonTimeStart).TotalHours + (lightVehicleTimeSheet.NightTimeEnd - lightVehicleTimeSheet.NightTimeStart).TotalHours), 2);
        //                var timeSheet = new MachineryTimeSheetSummeryVM();
        //                timeSheet.Date = lightVehicleTimeSheet.Date;
        //                timeSheet.WorkItem = lightVehicleTimeSheet.WorkType;
        //                if (netHour > 4)
        //                    timeSheet.OperationNetworkinHour = 1;
        //                if (netHour <= 4)
        //                    timeSheet.OperationNetworkinHour = 0.5m;
        //                if (netHour <= 0)
        //                    timeSheet.OperationNetworkinHour = 0;
        //                timeSheet.OperationRatePerHour = lightVehicleTimeSheet.RentalRate;
      
        //                timeSheet.Category = db.EquipmentIdentity.FirstOrDefault(eq => eq.EquipmentIdentityID == lightVehicleTimeSheet.EquipmentCategoryId).EquipmentCategoryName;
        //                timeSheet.EquipmentName = db.EquipmentName.FirstOrDefault(eq => eq.EquipmentNameID == lightVehicleTimeSheet.EquipmentNameId).Name;
        //                timeSheet.PlateNumber = db.Fleets.FirstOrDefault(eq => eq.FleetsID == lightVehicleTimeSheet.FleetsID).PlateNumber;
        //                // Calculated
        //                timeSheet.OpertionRevenue = timeSheet.OperationNetworkinHour * timeSheet.OperationRatePerHour;
        //                timeSheet.IdleHour = lightVehicleTimeSheet.IdleHours;
        //                timeSheet.IdleRatePerHour = lightVehicleTimeSheet.IdealRentalRate;
        //                // Calculated
        //                timeSheet.IdleRevenue = timeSheet.IdleHour * timeSheet.IdleRatePerHour;
        //                timeSheet.DownHour = lightVehicleTimeSheet.DownHours;
        //                // Calculated
        //                timeSheet.TotalRevenue = timeSheet.IdleRevenue + timeSheet.OpertionRevenue;
        //                timeSheet.FuelInLiter = lightVehicleTimeSheet.FuelConsumption;
        //                timeSheet.FuelRate = lightVehicleTimeSheet.FuelRate;
        //                // Calculated
        //                timeSheet.FuelCost = timeSheet.FuelInLiter * timeSheet.FuelRate;
        //                timeSheet.LubricantCost = lightVehicleTimeSheet.LubricantCost;
        //                timeSheet.MaintenanceCost = lightVehicleTimeSheet.MaintenanceCost;
        //                timeSheet.TireCost = lightVehicleTimeSheet.TireCost;
        //                timeSheet.Perdium = lightVehicleTimeSheet.Perdium;
        //                timeSheet.Salary = lightVehicleTimeSheet.Salary;
        //                // Calculated
        //                timeSheet.TotalCost = timeSheet.LubricantCost + timeSheet.MaintenanceCost + timeSheet.TireCost + timeSheet.Perdium + timeSheet.Salary;
        //                timeSheet.NetIncome = timeSheet.TotalRevenue - timeSheet.TotalCost;
        //                list.Add(timeSheet);
        //            }
        //            return list.OrderBy(x => x.Date.Date).ToList();
        //        }
        //        return list;
        //    }

        //}

        //[DataObject]
        //public class FleetFilterODS
        //{
        //    ExceedDbContext db = new ExceedDbContext();
        //    [DataObjectMethod(DataObjectMethodType.Select)]
        //    public List<FleetVM> GetFleet()
        //    {

        //        //var list = new List<FleetVM>();
        //        //var Machineries = db.MachineryWorkingTimeSheets.ToList();
        //        //foreach(var Machinery in Machineries)
        //        //{
        //        //    var fleets = db.Fleets.Where(x => x.FleetsID == Machinery.FleetsID);
        //        //    foreach(var fleet in fleets) {
        //        //        var item = new FleetVM();
        //        //        item.FleetsID = fleet.FleetsID.ToString();
        //        //        item.PlateNumber = fleet.PlateNumber;
        //        //        list.Add(item);
        //        //    }

        //        //}

        //        var list = from machineryWorkingTimeSheet in db.MachineryWorkingTimeSheets
        //                   join fleet in db.Fleets on machineryWorkingTimeSheet.FleetsID equals fleet.FleetsID
        //                   select new FleetVM
        //                   {
        //                       FleetsID = fleet.FleetsID.ToString(),
        //                       PlateNumber = fleet.PlateNumber
        //                   };
        //        return list.ToList();
        //    }
        //}

        //[DataObject]
        //public class BranchODS
        //{
        //    ExceedDbContext db = new ExceedDbContext();
        //    [DataObjectMethod(DataObjectMethodType.Select)]
        //    public List<Branch> Branch()
        //    {
        //        return db.Branch.ToList();
        //    }
        //}
        //public class FleetVM
        //{
        //    public string FleetsID { get; set; }
        //    public string PlateNumber { get; set; }

        //}
    }
}
