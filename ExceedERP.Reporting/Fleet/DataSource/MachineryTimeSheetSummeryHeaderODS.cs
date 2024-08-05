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
    public class MachineryTimeSheetSummeryHeaderODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public MachineryTimeSheetSummeryHeaderVM MachineryTimeSheetSummeryHeader(string fleetId, DateTime from, DateTime to, string project)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new MachineryTimeSheetSummeryHeaderVM();

            if (fleetId != null)
            {
                Guid FId = Guid.Parse(fleetId);

                MachineryWorkingTimeSheet machineryTimeSheet = db.MachineryWorkingTimeSheets.FirstOrDefault(x => x.FleetsID == FId && DbFunctions.TruncateTime(x.Date) >= from.Date && DbFunctions.TruncateTime(x.Date) <= to.Date);
                
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
        public List<MachineryTimeSheetSummeryVM> MachineryTimeSheetSummery(string fleetId, DateTime from, DateTime to, string project, string businessUnitId, string categoryId, string subCategoryId)
        {

            var list = new List<MachineryTimeSheetSummeryVM>();
            if (fleetId != null)
            {

                
                var machineryTimeSheets = db.MachineryWorkingTimeSheets.OrderByDescending(x => x.Date).Where(x => x.CostSheetApproved).ToList();
                var categories = db.EquipmentIdentity.ToList();
                var equipmentTypes = db.EquipmentType.ToList();

                Guid.TryParse(fleetId, out Guid FId);
                machineryTimeSheets = machineryTimeSheets.Where(x => x.FleetsID == FId && x.Date.Date >= from.Date && x.Date.Date <= to.Date).ToList();
                #region Rental Rate
                //var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == FId);
                var fleetList = db.Fleets.ToList();
                var fleetRentalRates = db.FleetRentalRates.ToList();
                var timeSheetBranchIds = new HashSet<int>(machineryTimeSheets.Select(s => s.BranchId));
                var timeSheetDates = new HashSet<int>(machineryTimeSheets.Select(s => s.Date.Year));
                fleetRentalRates = fleetRentalRates.Where(x => timeSheetBranchIds.Contains(x.BranchId)).ToList();
                fleetRentalRates = fleetRentalRates.Where(x => timeSheetDates.Contains(x.Year.Year)).ToList();

                foreach (var item in fleetRentalRates)
                {
                    var equipmentRates = db.EquipmentRates.Where(x => x.FleetRentalRateId == item.FleetRentalRateId).ToList();
                  
                }
                #endregion


                if (project != null)
                {
                    int.TryParse(project, out int branchId);
                    machineryTimeSheets = machineryTimeSheets.Where(x => x.BranchId == branchId).ToList();

                }
                if (!string.IsNullOrWhiteSpace(businessUnitId))
                {
                    machineryTimeSheets = machineryTimeSheets.Where(x => x.BusinessUnitId == int.Parse(businessUnitId)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    Guid.TryParse(categoryId, out Guid id);
                    List<MachineryWorkingTimeSheet> machineryList = new List<MachineryWorkingTimeSheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var machineryTimesheetList = machineryTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        machineryList.AddRange(machineryTimesheetList);
                    }
                    machineryTimeSheets = machineryList;

                }
                if (!string.IsNullOrWhiteSpace(subCategoryId))
                {
                    Guid.TryParse(subCategoryId, out Guid id);
                    List<MachineryWorkingTimeSheet> machineryList = new List<MachineryWorkingTimeSheet>();
                    var fleets = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
                    foreach (var item in fleets)
                    {
                        var machineryTimesheetList = machineryTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        machineryList.AddRange(machineryTimesheetList);
                    }
                    machineryTimeSheets = machineryList;
                }
                foreach (var machineryTimeSheet in machineryTimeSheets)
                {
                    var timeSheet = new MachineryTimeSheetSummeryVM();
                    timeSheet.Date = machineryTimeSheet.Date.Date;
                    timeSheet.WorkItem = machineryTimeSheet.WorkType;
                    timeSheet.OperationNetworkinHour = Math.Round((decimal)((machineryTimeSheet.MorningTimeEnd - machineryTimeSheet.MorningTimeStart).TotalHours + (machineryTimeSheet.AfternoonTimeEnd - machineryTimeSheet.AfternoonTimeStart).TotalHours + (machineryTimeSheet.NightTimeEnd - machineryTimeSheet.NightTimeStart).TotalHours), 2);
                    timeSheet.OperationRatePerHour = machineryTimeSheet.RentalRate;
                    // Calculated
                    timeSheet.OpertionRevenue = timeSheet.OperationNetworkinHour * timeSheet.OperationRatePerHour;
                    timeSheet.IdleHour = machineryTimeSheet.IdleHours;
                    timeSheet.IdleRatePerHour = machineryTimeSheet.IdealRentalRate;
                    // Calculated
                    timeSheet.IdleRevenue = timeSheet.IdleHour * timeSheet.IdleRatePerHour * timeSheet.OperationRatePerHour;
                    timeSheet.DownHour = machineryTimeSheet.DownHours;
                    // Calculated
                    timeSheet.TotalRevenue = timeSheet.IdleRevenue + timeSheet.OpertionRevenue;
                    timeSheet.FuelInLiter = machineryTimeSheet.FuelConsumption;
                    timeSheet.FuelRate = machineryTimeSheet.FuelRate;
                    // Calculated
                    timeSheet.FuelCost = timeSheet.FuelInLiter * timeSheet.FuelRate;
                    timeSheet.LubricantCost = machineryTimeSheet.LubricantCost;
                    timeSheet.MaintenanceCost = machineryTimeSheet.MaintenanceCost;
                    timeSheet.TireCost = machineryTimeSheet.TireCost;
                    timeSheet.Perdium = machineryTimeSheet.Perdium;
                    timeSheet.Salary = machineryTimeSheet.Salary;
                    // Calculated
                    timeSheet.TotalCost = timeSheet.FuelCost + timeSheet.LubricantCost + timeSheet.MaintenanceCost + timeSheet.TireCost + timeSheet.Perdium + timeSheet.Salary;
                    timeSheet.NetIncome = timeSheet.TotalRevenue - timeSheet.TotalCost;
                    list.Add(timeSheet);
                }
                return list.OrderBy(x => x.Date.Date).ToList();
            }
            return list;
        }

    }

    [DataObject]
    public class FleetFilterODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetVM> GetFleet()
        {

            //var list = new List<FleetVM>();
            //var Machineries = db.MachineryWorkingTimeSheets.ToList();
            //foreach(var Machinery in Machineries)
            //{
            //    var fleets = db.Fleets.Where(x => x.FleetsID == Machinery.FleetsID);
            //    foreach(var fleet in fleets) {
            //        var item = new FleetVM();
            //        item.FleetsID = fleet.FleetsID.ToString();
            //        item.PlateNumber = fleet.PlateNumber;
            //        list.Add(item);
            //    }

            //}

            var list = from machineryWorkingTimeSheet in db.MachineryWorkingTimeSheets
                       join fleet in db.Fleets on machineryWorkingTimeSheet.FleetsID equals fleet.FleetsID
                       select new FleetVM
                       {
                           FleetsID = fleet.FleetsID.ToString(),
                           PlateNumber = fleet.PlateNumber
                       };
            return list.ToList();
        }
    }

    [DataObject]
    public class BranchODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<Branch> Branch()
        {
            return db.Branch.ToList();
        }
    }
    public class FleetVM
    {
        public string FleetsID { get; set; }
        public string PlateNumber { get; set; }

    }
}
