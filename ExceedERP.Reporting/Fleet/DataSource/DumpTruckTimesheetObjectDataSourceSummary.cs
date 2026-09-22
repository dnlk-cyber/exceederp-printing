using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModels;
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
    public class DumpTruckTimesheetObjectDataSourceSummary
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public DumpTruckTimesheetHeaderViewModel DumpTruckTimesheetHeader(DateTime from, DateTime to, string project)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new DumpTruckTimesheetHeaderViewModel();

            if (project != null)            {
                
                int.TryParse(project, out int branchId);
                //dumpTruckTimeSheets = dumpTruckTimeSheets.Where(x => x.BranchId == branchId).ToList();
                var dumpTruckTimeSheet = db.DumpTruckTimeSheets.FirstOrDefault(x => x.BranchId == branchId && DbFunctions.TruncateTime(x.Date) >= from.Date && DbFunctions.TruncateTime(x.Date) <= to.Date);

                
                if (dumpTruckTimeSheet != null)
                {
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == dumpTruckTimeSheet.BranchId);
                    var fleet = db.Fleets.Find(dumpTruckTimeSheet.FleetsID);
                    var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
                    var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
                    var rentFromExternals = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == fleet.RentId);
                    var equipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID);
                    var operatorName = db.Operator.Find(dumpTruckTimeSheet.OperatorId);
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
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DumpTruckTimesheetViewModel> DumpTruckTimesheet(DateTime from, DateTime to, string businessUnitId, string categoryId, string subCategoryId, string IntExt,string project)
        {
            var list = new List<DumpTruckTimesheetViewModel>();

            if (project == null) return list;

            int.TryParse(project, out int branchId);
            var fromDate = from.Date;
            var toDate = to.Date;
            var dumpTruckTimeSheets = db.DumpTruckTimeSheets.Where(x => x.BranchId == branchId && x.CostSheetApproved).AsNoTracking().ToList();
            dumpTruckTimeSheets = dumpTruckTimeSheets.Where(x => x.Date.Date >= from.Date && x.Date.Date <= to.Date).ToList();
            if (!string.IsNullOrWhiteSpace(businessUnitId))
            {
                int.TryParse(businessUnitId, out int bUId);
                dumpTruckTimeSheets = dumpTruckTimeSheets.Where(x => x.BusinessUnitId == bUId).ToList();
            }
            if (!string.IsNullOrWhiteSpace(IntExt))
            {
                List<DumpTruckTimeSheet> dumpTruckList = new List<DumpTruckTimeSheet>();
                if (IntExt == "Rent")
                {


                    var fleetFiltered = db.Fleets.Where(x => x.IsExternal).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var dumptruckFiltered = dumpTruckTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        dumpTruckList.AddRange(dumptruckFiltered);
                    }
                    dumpTruckTimeSheets = dumpTruckList;
                }
                else
                {
                    var fleetFiltered = db.Fleets.Where(x => !x.IsExternal).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var dumptruckFiltered = dumpTruckTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        dumpTruckList.AddRange(dumptruckFiltered);
                    }
                    dumpTruckTimeSheets = dumpTruckList;
                }
               }

                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    Guid.TryParse(categoryId, out Guid id);
                    List<DumpTruckTimeSheet> dumpTruckList = new List<DumpTruckTimeSheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var dumptruckFiltered = dumpTruckTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        dumpTruckList.AddRange(dumptruckFiltered);
                    }
                    dumpTruckTimeSheets = dumpTruckList;
                }
                if (!string.IsNullOrWhiteSpace(subCategoryId))
                {
                    Guid.TryParse(subCategoryId, out Guid id);
                    List<DumpTruckTimeSheet> dumpTruckList = new List<DumpTruckTimeSheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var dryLoadingFiltered = dumpTruckTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        dumpTruckList.AddRange(dryLoadingFiltered);
                    }
                    dumpTruckTimeSheets = dumpTruckList;
                }
         
                var groupedData = dumpTruckTimeSheets
                .GroupBy(d => d.FleetsID)
                .Select(group =>
                {
                    var firstEntry = group.First(); // Take first record for static fields
                    var fleet = db.Fleets.Find(firstEntry.FleetsID);
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == firstEntry.BranchId);
                    var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == fleet.EquipmentTypeID);
                    decimal Totalcapacity = 0;
                    decimal TotalNumberOfTrips = 0;
                    decimal TotalActualDistance = 0;
                    decimal TotalTotalHaulage = 0;
                    decimal TotalOperationRateperHour = 0;
                    decimal TotalOperationRevenue = 0;
                    decimal TotalIdleRevenue = 0;
                    decimal TotalTotalRevenue = 0;
                    decimal TotalFuelCost = 0;
                    decimal TotalTotalCost = 0;
                    decimal TotalNetIncome = 0;
                    decimal TotalFuelInLiter = 0;
                    decimal TotalFuelRate = 0;
                    decimal TotalLubricantCost = 0;
                    decimal TotalMaintenaceCost = 0;
                    decimal TotalTireCost = 0;
                    decimal TotalSalary = 0;
                    decimal TotalPerdium = 0;
                    decimal TotalOperationNetWorkingHour = 0;
                    decimal TotalIdleHour = 0;
                    decimal TotalDownHour = 0;
                    foreach(var g in group)
                    {
                        var dumptruckD = db.DumpTruckTimeSheetDetails.Where(x => x.DumpTruckTimeSheetId == g.DumpTruckTimeSheetId).ToList();
                      
                            TotalFuelInLiter += g.FuelConsumption;
                            TotalFuelRate = g.FuelRate;
                            //TotalLubricantCost += g.LubricantCost;
                            TotalMaintenaceCost += g.MaintenanceCost;
                            TotalTireCost += g.TireCost;
                            TotalSalary += g.Salary;
                            TotalPerdium += g.Perdium;
                            TotalIdleHour += g.IdleHour;
                            TotalDownHour += g.DownHour;
                            TotalTotalRevenue +=
                            TotalOperationNetWorkingHour += g.NetworkingHour;
                            TotalIdleRevenue += g.IdealRentalRate * g.IdleHour;
                            TotalFuelCost += g.FuelRate * g.FuelConsumption;
                            TotalTotalCost += g.Perdium + g.Salary + g.TireCost + g.MaintenanceCost /*+ g.LubricantCost*/ + (g.FuelRate * g.FuelConsumption);
                        
                      foreach(var dumpD in dumptruckD)
                        {
                            TotalNumberOfTrips += dumpD.NumberOfTrip;
                            Totalcapacity += dumpD.Capacity;
                            TotalActualDistance += dumpD.ActualDistance;
                            TotalTotalHaulage += dumpD.TotalHaulage;
                            TotalOperationRateperHour = dumpD.RentalRate;
                            TotalOperationRevenue+= dumpD.Capacity * dumpD.ActualDistance * dumpD.NumberOfTrip * dumpD.RentalRate;
                        }
                        
                    }
                    // Create a new timesheet item
                    var item = new DumpTruckTimesheetViewModel
                    {
                        //Category = equipment?.CategoryName ?? "Unknown",
                        Category = db.EquipmentIdentity.SingleOrDefault(x=>x.EquipmentIdentityID==fleet.EquipmentIdentityID).EquipmentCategoryName,
                        EquipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID)?.Name,
                        PlateNo = fleet?.PlateNumber ?? "N/A",
                        Branch = branch?.Name ?? "N/A",
                        SN = 1, // Combine multiple IDs
                        Date = firstEntry.Date, // Taking the earliest date
                        WorkItem = "Work Item", // Combine unique work types

                        // Aggregate numeric values
                        OperationNetWorkingHour = TotalOperationNetWorkingHour,
                        NumberOfTrips = TotalNumberOfTrips,
                        ActualDistance = TotalActualDistance,
                        capacity = Totalcapacity,
                        TotalHaulage = TotalTotalHaulage,

                        OperationRateperHour = TotalOperationRateperHour, // Average operation rate per hour
                        OperationRevenue = TotalOperationRevenue,
                        IdleRateperHour = firstEntry.IdealRentalRate,
                        IdleHour = TotalIdleHour,
                        IdleRevenue = TotalIdleRevenue,
                       
                        DownHour = TotalDownHour,

                        FuelInLiter = TotalFuelInLiter,
                        FuelRate = firstEntry.FuelRate, // Assuming rate is fixed
                        FuelCost =TotalFuelCost,
                        LubricantCost = TotalLubricantCost,
                        MaintenaceCost = TotalMaintenaceCost,
                        TireCost = TotalTireCost,
                        Salary =TotalSalary,
                        Perdium = TotalPerdium,
                    };

                    item.TotalCost =TotalTotalCost;
                    item.TotalRevenue = TotalIdleRevenue + TotalOperationRevenue;
                    item.NetIncome = TotalIdleRevenue + TotalOperationRevenue - TotalTotalCost;

                    return item;
                })
                .OrderBy(x => x.Date)
                .ToList();

                return groupedData;
            }

        //public List<DumpTruckTimesheetViewModel> DumpTruckTimesheet(DateTime from, DateTime to, string businessUnitId, string categoryId, string subCategoryId, string project)
        //{

        //    var list = new List<DumpTruckTimesheetViewModel>();
        //    if (project != null)
        //    {
        //        int.TryParse(project, out int branchId);
        //        //dumpTruckTimeSheets = dumpTruckTimeSheets.Where(x => x.BranchId == branchId).ToList();
        //        var dumpTruckTimeSheets = db.DumpTruckTimeSheets.Where(x => x.BranchId == branchId && x.CostSheetApproved).ToList();

        //        dumpTruckTimeSheets = dumpTruckTimeSheets.Where(x => x.Date.Date >= from.Date && x.Date.Date <= to.Date).ToList();

        //        if (!string.IsNullOrWhiteSpace(businessUnitId))
        //        {
        //            int.TryParse(businessUnitId, out int bUId);
        //            dumpTruckTimeSheets = dumpTruckTimeSheets.Where(x => x.BusinessUnitId == bUId).ToList();
        //        }
        //        if (!string.IsNullOrWhiteSpace(categoryId))
        //        {
        //            Guid.TryParse(categoryId, out Guid id);
        //            List<DumpTruckTimeSheet> dumpTruckList = new List<DumpTruckTimeSheet>();
        //            var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
        //            foreach (var item in fleetFiltered)
        //            {
        //                var dumptruckFiltered = dumpTruckTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
        //                dumpTruckList.AddRange(dumptruckFiltered);
        //            }
        //            dumpTruckTimeSheets = dumpTruckList;
        //        }
        //        if (!string.IsNullOrWhiteSpace(subCategoryId))
        //        {
        //            Guid.TryParse(subCategoryId, out Guid id);
        //            List<DumpTruckTimeSheet> dumpTruckList = new List<DumpTruckTimeSheet>();
        //            var fleetFiltered = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
        //            foreach (var item in fleetFiltered)
        //            {
        //                var dryLoadingFiltered = dumpTruckTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
        //                dumpTruckList.AddRange(dryLoadingFiltered);
        //            }
        //            dumpTruckTimeSheets = dumpTruckList;
        //        }               
        //        foreach (var dumpTruckTimeSheet in dumpTruckTimeSheets)
        //        {

        //            bool checker = true;
        //            var dumptruckDetails = db.DumpTruckTimeSheetDetails.Where(x => x.DumpTruckTimeSheetId == dumpTruckTimeSheet.DumpTruckTimeSheetId).ToList();
        //            var branch = db.Branch.FirstOrDefault(x => x.BranchId == dumpTruckTimeSheet.BranchId);
        //            foreach (var detail in dumptruckDetails)
        //            {

        //                var item = new DumpTruckTimesheetViewModel();
        //                var fleet = db.Fleets.Find(dumpTruckTimeSheet.FleetsID);
        //                var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
        //                var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
        //                item.Category = equipment.CategoryName;
        //                item.EquipmentName = db.EquipmentName.FirstOrDefault(x=>x.EquipmentTypeId==equipment.EquipmentTypeID).Name;
        //                item.PlateNo = fleet.PlateNumber;
        //                item.Branch = branch.Name;
        //                item.SN = dumpTruckTimeSheet.DumpTruckTimeSheetId;
        //                item.Date = dumpTruckTimeSheet.Date;
        //                item.WorkItem = detail.TypeOfWork;
        //                item.OperationNetWorkingHour = dumpTruckTimeSheet.NetworkingHour;
        //                item.NumberOfTrips = detail.NumberOfTrip;
        //                item.ActualDistance = detail.ActualDistance;
        //                item.capacity = detail.Capacity;
        //                item.TotalHaulage = detail.TotalHaulage; 






        //                item.OperationRateperHour = detail.RentalRate;
        //                item.OperationRevenue = item.capacity * item.ActualDistance * item.NumberOfTrips * item.OperationRateperHour;

        //                item.IdleRateperHour = dumpTruckTimeSheet.IdealRentalRate;
        //                item.IdleRevenue = item.IdleRateperHour * item.IdleHour;
        //                item.TotalRevenue = item.IdleRevenue + item.OperationRevenue;
        //                item.DownHour = dumpTruckTimeSheet.DownHour;


        //                if (checker)
        //                {
        //                    item.IdleHour = dumpTruckTimeSheet.IdleHour;
        //                    item.FuelInLiter = dumpTruckTimeSheet.FuelConsumption;
        //                    item.FuelRate = dumpTruckTimeSheet.FuelRate;
        //                    item.FuelCost = item.FuelRate * item.FuelInLiter;
        //                    item.LubricantCost = dumpTruckTimeSheet.LubricantCost;
        //                    item.MaintenaceCost = dumpTruckTimeSheet.MaintenanceCost;
        //                    item.TireCost = dumpTruckTimeSheet.TireCost;
        //                    item.Salary = dumpTruckTimeSheet.Salary;
        //                    item.Perdium = dumpTruckTimeSheet.Perdium;
        //                    item.TotalCost = item.Perdium + item.Salary + item.TireCost + item.MaintenaceCost + item.LubricantCost + item.FuelCost;
        //                    //item.TotalCost = item.NumberOfTrips * item.ActualDistance * item.capacity * item.OperationRateperHour; 
        //                    item.NetIncome = item.TotalRevenue - item.TotalCost;
        //                }
        //                else
        //                {
        //                    item.FuelInLiter = 0;
        //                    item.FuelRate = 0;
        //                    item.FuelCost = 0;
        //                    item.LubricantCost = 0;
        //                    item.MaintenaceCost = 0;
        //                    item.TireCost = 0;
        //                    item.Salary = 0;
        //                    item.Perdium = 0;
        //                    item.TotalCost = 0;
        //                    //item.TotalCost = item.NumberOfTrips * item.ActualDistance * item.capacity * item.OperationRateperHour; 
        //                    item.NetIncome = item.TotalRevenue - item.TotalCost;
        //                }



        //                list.Add(item);
        //                item = null;
        //                checker = false;
        //            }

        //        }
        //        return list.OrderBy(x => x.Date.Date).ToList();
        //    }
        //    return list;
        //}
    }
}
