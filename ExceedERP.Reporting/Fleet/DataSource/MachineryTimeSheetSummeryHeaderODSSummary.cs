using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class MachineryTimeSheetSummeryHeaderODSSummary
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public MachineryTimeSheetSummeryHeaderVM MachineryTimeSheetSummeryHeader(DateTime from, DateTime to, string project)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new MachineryTimeSheetSummeryHeaderVM();

            if (project != null)
            {
                int FId = int.Parse(project);

                MachineryWorkingTimeSheet machineryTimeSheet = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.BranchId == FId && DbFunctions.TruncateTime(x.Date) >= from.Date && DbFunctions.TruncateTime(x.Date) <= to.Date);

                if (machineryTimeSheet != null)
                {
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == machineryTimeSheet.BranchId);
                    var fleet = db.Fleets.Find(FId);
                    var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
                    var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
                    var rentFromExternals = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == fleet.RentId);
                    var equipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID);
                    var operatorName = db.Operator.Find(machineryTimeSheet.OperatorId);
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
                        header.PlateNumber = fleet.SerialNo;
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
        public List<MachineryTimeSheetSummeryVM> MachineryTimeSheetSummery(DateTime from, DateTime to,string IntExt, string project, string businessUnitId, string categoryId, string subCategoryId)
        {
            var list = new List<MachineryTimeSheetSummeryVM>();

            if (project == null) return list;

            int.TryParse(project, out int FId);
           
            var fromDate = from.Date;
            var toDate = to.Date;
            var machineryTimeSheets = db.MachineryWorkingTimeSheets
                .Where(x => x.CostSheetApproved && x.BranchId == FId && x.Date >= fromDate && x.Date <= toDate)
                .OrderByDescending(x => x.Date)
                .ToList();

            // Filtering by Business Unit
            if (!string.IsNullOrWhiteSpace(businessUnitId))
            {
                machineryTimeSheets = machineryTimeSheets.Where(x => x.BusinessUnitId == int.Parse(businessUnitId)).ToList();
            }
            if (!string.IsNullOrWhiteSpace(IntExt))
            {
                List<MachineryWorkingTimeSheet> MachineList = new List<MachineryWorkingTimeSheet>();
                if (IntExt == "Rent")
                {


                    var fleetFiltered = db.Fleets.Where(x => x.IsExternal).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var MachineFiltered = machineryTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        MachineList.AddRange(MachineFiltered);
                    }
                    machineryTimeSheets = MachineList;
                }
                else
                {
                    var fleetFiltered = db.Fleets.Where(x => !x.IsExternal).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var MachineFiltered = machineryTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        MachineList.AddRange(MachineFiltered);
                    }
                    machineryTimeSheets = MachineList;
                }
            }
            // Filtering by Category
            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                Guid.TryParse(categoryId, out Guid id);
                var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).Select(x => x.FleetsID).ToHashSet();
                machineryTimeSheets = machineryTimeSheets.Where(x => fleetFiltered.Contains(x.FleetsID)).ToList();
            }

            // Filtering by Subcategory
            if (!string.IsNullOrWhiteSpace(subCategoryId))
            {
                Guid.TryParse(subCategoryId, out Guid id);
                var fleetFiltered = db.Fleets.Where(x => x.EquipmentTypeID == id).Select(x => x.FleetsID).ToHashSet();
                machineryTimeSheets = machineryTimeSheets.Where(x => fleetFiltered.Contains(x.FleetsID)).ToList();
            }

            // Grouping by FleetID
            var groupedData = machineryTimeSheets
                .GroupBy(m => m.FleetsID)
                .Select(group =>
                {
                    var firstEntry = group.First();
                    var fleet = db.Fleets.FirstOrDefault(f => f.FleetsID == firstEntry.FleetsID);
                    var category = db.EquipmentIdentity.FirstOrDefault(eq => eq.EquipmentIdentityID == firstEntry.EquipmentCategoryId);
                    var equipment = db.EquipmentName.FirstOrDefault(eq => eq.EquipmentNameID == firstEntry.EquipmentNameId);

                    var timeSheet = new MachineryTimeSheetSummeryVM();
                    timeSheet.Date = firstEntry.Date; // Keep the first date
                         timeSheet.WorkItem = string.Join(", ", group.Select(x => x.WorkType).Distinct()); // Combine work items
                         timeSheet.PlateNumber = fleet?.PlateNumber ?? "N/A";
                         timeSheet.Category = category?.EquipmentCategoryName ?? "Unknown";
                         timeSheet.EquipmentName = equipment?.Name ?? "Unknown";
                         timeSheet.OperationNetworkinHour = 0;
                    timeSheet.OperationRatePerHour = group.Average(m => m.RentalRate); // Average rental rate per hour
                        foreach(var g in group)
                    {
                        decimal netHour = 0;
                        netHour= Math.Round((decimal)((g.MorningTimeEnd - g.MorningTimeStart).TotalHours + (g.AfternoonTimeEnd - g.AfternoonTimeStart).TotalHours + (g.NightTimeEnd - g.NightTimeStart).TotalHours), 2);
                        //if (netHour > 4)
                        //{
                        //    netHour = 1;
                        //}

                        //else if (netHour > 0)
                        //{
                        //    netHour=0.5m;
                        //}
                        timeSheet.OperationNetworkinHour+=netHour;
                        timeSheet.OpertionRevenue += netHour * g.RentalRate;
                        timeSheet.IdleRevenue+= g.IdleHours * g.IdealRentalRate;

                    }
                    timeSheet.IdleHour = group.Sum(m => m.IdleHours);
                         timeSheet.IdleRatePerHour = group.Average(m => m.IdealRentalRate);
                         timeSheet.DownHour = group.Sum(m => m.DownHours);
                         timeSheet.FuelInLiter = group.Sum(m => m.FuelConsumption);
                         timeSheet.FuelRate = firstEntry.FuelRate; // Assuming it's the same for all
                         timeSheet.FuelCost = group.Sum(m => m.FuelConsumption * m.FuelRate);
                         timeSheet.LubricantCost = group.Sum(m => m.LubricantCost);
                         timeSheet.MaintenanceCost = group.Sum(m => m.MaintenanceCost);
                         timeSheet.TireCost = group.Sum(m => m.TireCost);
                         timeSheet.Perdium = group.Sum(m => m.Perdium);
                         timeSheet.Salary = group.Sum(m => m.Salary);

                        // Total calculations
                         timeSheet.TotalRevenue = 0; // Will be calculated below
                         timeSheet.TotalCost = 0;
                         timeSheet.NetIncome = 0;
                   
                    // Final Calculations
                    timeSheet.TotalRevenue = timeSheet.IdleRevenue + timeSheet.OpertionRevenue;
                    timeSheet.TotalCost = timeSheet.FuelCost + timeSheet.LubricantCost + timeSheet.MaintenanceCost + timeSheet.TireCost + timeSheet.Perdium + timeSheet.Salary;
                    timeSheet.NetIncome = timeSheet.TotalRevenue - timeSheet.TotalCost;
                    return timeSheet;
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Assign sequential SN

            return groupedData;
        }
        //   public List<MachineryTimeSheetSummeryVM> MachineryTimeSheetSummery(DateTime from, DateTime to, string project, string businessUnitId, string categoryId, string subCategoryId)
        //    {

        //        var list = new List<MachineryTimeSheetSummeryVM>();
        //        if (project != null)
        //        {


        //            var machineryTimeSheets = db.MachineryWorkingTimeSheets.OrderByDescending(x => x.Date).Where(x => x.CostSheetApproved).ToList();
        //            var categories = db.EquipmentIdentity.ToList();
        //            var equipmentTypes = db.EquipmentType.ToList();

        //            int.TryParse(project, out int FId);
        //            machineryTimeSheets = machineryTimeSheets.Where(x => x.BranchId == FId && x.Date.Date >= from.Date && x.Date.Date <= to.Date).ToList();
        //            #region Rental Rate
        //            //var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == FId);
        //            var fleetList = db.Fleets.ToList();
        //            var fleetRentalRates = db.FleetRentalRates.ToList();
        //            var timeSheetBranchIds = new HashSet<int>(machineryTimeSheets.Select(s => s.BranchId));
        //            var timeSheetDates = new HashSet<int>(machineryTimeSheets.Select(s => s.Date.Year));
        //            fleetRentalRates = fleetRentalRates.Where(x => timeSheetBranchIds.Contains(x.BranchId)).ToList();
        //            fleetRentalRates = fleetRentalRates.Where(x => timeSheetDates.Contains(x.Year.Year)).ToList();

        //            foreach (var item in fleetRentalRates)
        //            {
        //                var equipmentRates = db.EquipmentRates.Where(x => x.FleetRentalRateId == item.FleetRentalRateId).ToList();

        //            }
        //            #endregion


        //            if (!string.IsNullOrWhiteSpace(businessUnitId))
        //            {
        //                machineryTimeSheets = machineryTimeSheets.Where(x => x.BusinessUnitId == int.Parse(businessUnitId)).ToList();
        //            }
        //            if (!string.IsNullOrWhiteSpace(categoryId))
        //            {
        //                Guid.TryParse(categoryId, out Guid id);
        //                List<MachineryWorkingTimeSheet> machineryList = new List<MachineryWorkingTimeSheet>();
        //                var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
        //                foreach (var item in fleetFiltered)
        //                {
        //                    var machineryTimesheetList = machineryTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
        //                    machineryList.AddRange(machineryTimesheetList);
        //                }
        //                machineryTimeSheets = machineryList;

        //            }
        //            if (!string.IsNullOrWhiteSpace(subCategoryId))
        //            {
        //                Guid.TryParse(subCategoryId, out Guid id);
        //                List<MachineryWorkingTimeSheet> machineryList = new List<MachineryWorkingTimeSheet>();
        //                var fleets = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
        //                foreach (var item in fleets)
        //                {
        //                    var machineryTimesheetList = machineryTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
        //                    machineryList.AddRange(machineryTimesheetList);
        //                }
        //                machineryTimeSheets = machineryList;
        //            }
        //            foreach (var machineryTimeSheet in machineryTimeSheets)
        //            {
        //                var timeSheet = new MachineryTimeSheetSummeryVM();
        //                timeSheet.Date = machineryTimeSheet.Date.Date;
        //                timeSheet.WorkItem = machineryTimeSheet.WorkType;
        //                timeSheet.PlateNumber = db.Fleets.FirstOrDefault(f => f.FleetsID == machineryTimeSheet.FleetsID).PlateNumber;
        //                timeSheet.Category = db.EquipmentIdentity.FirstOrDefault(eq => eq.EquipmentIdentityID == machineryTimeSheet.EquipmentCategoryId).EquipmentCategoryName;
        //                timeSheet.EquipmentName = db.EquipmentName.FirstOrDefault(eq => eq.EquipmentNameID == machineryTimeSheet.EquipmentNameId).Name;
        //                timeSheet.OperationNetworkinHour = Math.Round((decimal)((machineryTimeSheet.MorningTimeEnd - machineryTimeSheet.MorningTimeStart).TotalHours + (machineryTimeSheet.AfternoonTimeEnd - machineryTimeSheet.AfternoonTimeStart).TotalHours + (machineryTimeSheet.NightTimeEnd - machineryTimeSheet.NightTimeStart).TotalHours), 2);
        //                timeSheet.OperationRatePerHour = machineryTimeSheet.RentalRate;
        //                // Calculated
        //                timeSheet.OpertionRevenue = timeSheet.OperationNetworkinHour * timeSheet.OperationRatePerHour;
        //                timeSheet.IdleHour = machineryTimeSheet.IdleHours;
        //                timeSheet.IdleRatePerHour = machineryTimeSheet.IdealRentalRate;
        //                // Calculated
        //                timeSheet.IdleRevenue = timeSheet.IdleHour * timeSheet.IdleRatePerHour;
        //                timeSheet.DownHour = machineryTimeSheet.DownHours;
        //                // Calculated
        //                timeSheet.TotalRevenue = timeSheet.IdleRevenue + timeSheet.OpertionRevenue;
        //                timeSheet.FuelInLiter = machineryTimeSheet.FuelConsumption;
        //                timeSheet.FuelRate = machineryTimeSheet.FuelRate;
        //                // Calculated
        //                timeSheet.FuelCost = timeSheet.FuelInLiter * timeSheet.FuelRate;
        //                timeSheet.LubricantCost = machineryTimeSheet.LubricantCost;
        //                timeSheet.MaintenanceCost = machineryTimeSheet.MaintenanceCost;
        //                timeSheet.TireCost = machineryTimeSheet.TireCost;
        //                timeSheet.Perdium = machineryTimeSheet.Perdium;
        //                timeSheet.Salary = machineryTimeSheet.Salary;
        //                // Calculated
        //                timeSheet.TotalCost = timeSheet.FuelCost + timeSheet.LubricantCost + timeSheet.MaintenanceCost + timeSheet.TireCost + timeSheet.Perdium + timeSheet.Salary;
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