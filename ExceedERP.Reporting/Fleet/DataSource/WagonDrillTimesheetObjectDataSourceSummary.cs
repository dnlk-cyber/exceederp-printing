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
    public class WagonDrillTimesheetObjectDataSourceSummary
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public WagonDrillTimesheetHeaderViewModel WagonDrillTimesheetHeader(string project, DateTime from, DateTime to)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new WagonDrillTimesheetHeaderViewModel();
            if (project != null)
            {
                int branchId = int.Parse(project);

                var wagonDrillTimesheet = db.WagonDrillTimesheets.FirstOrDefault(x => x.BranchId == branchId && DbFunctions.TruncateTime(x.Date) >= from.Date && DbFunctions.TruncateTime(x.Date) <= to.Date);
                if (wagonDrillTimesheet != null)
                {
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == wagonDrillTimesheet.BranchId);
                    var fleet = db.Fleets.Find(wagonDrillTimesheet.FleetsID);
                    var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
                    var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
                    var rentFromExternals = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == fleet.RentId);
                    var equipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID);
                    var operatorName = db.Operator.Find(wagonDrillTimesheet.OperatorId);
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
        public List<WagonDrillTimesheetViewModel> WagonDrillTimesheet(string project, DateTime? from, DateTime? to,string IntExt, string businessUnitId, string categoryId, string subCategoryId)
        {
            var list = new List<WagonDrillTimesheetViewModel>();

            if (string.IsNullOrWhiteSpace(project)) return list;

            int FId = int.Parse(project);

            // Fetch and filter initial data
            var wagonDrillTimesheets = db.WagonDrillTimesheets
                .Where(x => x.CostSheetApproved && x.BranchId == FId)
                .ToList();

            if (from.HasValue)
                wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.Date.Date >= from.Value.Date).ToList();

            if (to.HasValue)
                wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.Date.Date <= to.Value.Date).ToList();

            if (!string.IsNullOrWhiteSpace(businessUnitId))
                wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.BusinessUnitId == int.Parse(businessUnitId)).ToList();

            if (!string.IsNullOrWhiteSpace(IntExt))
            {
                List<WagonDrillTimesheet> WagonList = new List<WagonDrillTimesheet>();
                if (IntExt == "Rent")
                {


                    var fleetFiltered = db.Fleets.Where(x => x.IsExternal).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var WagonFiltered = wagonDrillTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        WagonList.AddRange(WagonFiltered);
                    }
                    wagonDrillTimesheets = WagonList;
                }
                else
                {
                    var fleetFiltered = db.Fleets.Where(x => !x.IsExternal).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var WagonFiltered = wagonDrillTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        WagonList.AddRange(WagonFiltered);
                    }
                    wagonDrillTimesheets = WagonList;
                }
            }
            if (!string.IsNullOrWhiteSpace(categoryId))
            {
                Guid.TryParse(categoryId, out Guid id);
                var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).Select(x => x.FleetsID).ToHashSet();
                wagonDrillTimesheets = wagonDrillTimesheets.Where(x => fleetFiltered.Contains(x.FleetsID)).ToList();
            }

            if (!string.IsNullOrWhiteSpace(subCategoryId))
            {
                Guid.TryParse(subCategoryId, out Guid id);
                var fleetFiltered = db.Fleets.Where(x => x.EquipmentTypeID == id).Select(x => x.FleetsID).ToHashSet();
                wagonDrillTimesheets = wagonDrillTimesheets.Where(x => fleetFiltered.Contains(x.FleetsID)).ToList();
            }

            // Group by FleetID and aggregate data
            var groupedData = wagonDrillTimesheets
                .GroupBy(w => w.FleetsID)
                .Select(group =>
                {
                    var firstEntry = group.First();
                    var fleet = db.Fleets.FirstOrDefault(f => f.FleetsID == firstEntry.FleetsID);
                    var category = db.EquipmentIdentity.FirstOrDefault(eq => eq.EquipmentIdentityID == firstEntry.EquipmentCategoryId);
                    var equipment = db.EquipmentName.FirstOrDefault(eq => eq.EquipmentNameID == firstEntry.EquipmentNameId);

                    var item = new WagonDrillTimesheetViewModel
                    {
                        Date = firstEntry.Date, // Keep the first entry’s date
                PlateNumber = fleet?.PlateNumber ?? "N/A",
                        Category = category?.EquipmentCategoryName ?? "Unknown",
                        EquipmentName = equipment?.Name ?? "Unknown",
                        WorkItem = string.Join(", ", group.Select(x => x.WorkItem).Distinct()), // Combine Work Items

                // Aggregated Values
                OperationNetWorkingHour = group.Sum(x => x.OperationNetWorkingHour),
                        OperationDrilledHoleDepth = group.Sum(x => x.OperationDrilledHoleDepth),
                        NumberOfHolesDrilled = group.Sum(x => x.NumberOfHolesDrilled),
                        OperationRateForMeter = group.Average(x => x.RateInMeter), // Taking average for rates
                OperationAmount = Math.Round(group.Sum(x => x.OperationDrilledHoleDepth * x.NumberOfHolesDrilled * x.RateInMeter), 2),

                        IdleHour = group.Sum(x => x.IdleHour),
                        IdleRateHour = group.Average(x => x.IdleRateHour),
                        IdleAmount = group.Sum(x => x.IdleHour * x.IdleRateHour),
                        DownTime = group.Sum(x => x.DownTime),

                        FuelInLiter = group.Sum(x => x.FuelInLiter),
                        FuelRate = firstEntry.FuelRate, // Assuming it’s the same across records
                FuelCost = group.Sum(x => x.FuelInLiter * x.FuelRate),

                        LubricantCost = group.Sum(x => x.LubricantCost),
                        Perdium = group.Sum(x => x.Perdium),
                        Salary = group.Sum(x => x.Salary),
                        MaintenanceCost = group.Sum(x => x.MaintenanceCost),
                        TireCost = group.Sum(x => x.TireCost),

                        VAT = firstEntry.VAT, // Assuming same VAT across records
                TotalAmountBeforeVAT = group.Sum(x => x.OperationAmount + x.IdleAmount),
                        TotalAmount = group.Sum(x => x.OperationAmount + x.IdleAmount) * (1 + firstEntry.VAT),

                        Revenue = group.Sum(x => x.OperationAmount + x.IdleAmount),
                        TotalCost = group.Sum(x => x.Perdium + x.Salary + x.LubricantCost + x.FuelCost + x.MaintenanceCost + x.TireCost),
                        NetAmount = group.Sum(x => x.OperationAmount + x.IdleAmount) - group.Sum(x => x.Perdium + x.Salary + x.LubricantCost + x.FuelCost + x.MaintenanceCost + x.TireCost)
                    };

                    return item;
                })
                .OrderBy(x => x.Date)
                .ToList();

            // Assign sequential SN
            int snCounter = 1;
           

            return groupedData;
        }
        // public List<WagonDrillTimesheetViewModel> WagonDrillTimesheet(string project, DateTime? from, DateTime? to, string businessUnitId, string categoryId, string subCategoryId)
        //{
        //    var list = new List<WagonDrillTimesheetViewModel>();
        //    if (project != null)
        //    {


        //        var wagonDrillTimesheets = db.WagonDrillTimesheets.Where(x => x.CostSheetApproved).ToList();

        //        if (project != null)
        //        {
        //            int FId = int.Parse(project);
        //            wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.BranchId == FId).ToList();
        //        }
        //        if (from.HasValue)
        //        {
        //            wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.Date.Date >= from.Value.Date).ToList();
        //        }
        //        if (to.HasValue)
        //        {
        //            wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.Date.Date <= to.Value.Date).ToList();
        //        }
        //        if (!string.IsNullOrWhiteSpace(businessUnitId))
        //        {
        //            wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.BusinessUnitId == int.Parse(businessUnitId)).ToList();
        //        }
        //        if (!string.IsNullOrWhiteSpace(categoryId))
        //        {
        //            Guid.TryParse(categoryId, out Guid id);
        //            List<WagonDrillTimesheet> wagonDrillList = new List<WagonDrillTimesheet>();
        //            var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
        //            foreach (var item in fleetFiltered)
        //            {
        //                var wagonDrillFiltered = wagonDrillTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
        //                wagonDrillList.AddRange(wagonDrillFiltered);
        //            }
        //            wagonDrillTimesheets = wagonDrillList;
        //        }
        //        if (!string.IsNullOrWhiteSpace(subCategoryId))
        //        {
        //            Guid.TryParse(subCategoryId, out Guid id);
        //            List<WagonDrillTimesheet> wagonDrillList = new List<WagonDrillTimesheet>();
        //            var fleetFiltered = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
        //            foreach (var item in fleetFiltered)
        //            {
        //                var wagonDrillFiltered = wagonDrillTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
        //                wagonDrillList.AddRange(wagonDrillFiltered);
        //            }
        //            wagonDrillTimesheets = wagonDrillList;
        //        }
        //        foreach (var wagonDrillTimesheet in wagonDrillTimesheets)
        //        {
        //            var item = new WagonDrillTimesheetViewModel();
        //            var fleet = db.Fleets.Find(wagonDrillTimesheet.FleetsID);
        //            var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
        //            var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
        //            item.Date = wagonDrillTimesheet.Date;
        //            item.PlateNumber = db.Fleets.FirstOrDefault(f => f.FleetsID == wagonDrillTimesheet.FleetsID).PlateNumber;
        //            item.Category = db.EquipmentIdentity.FirstOrDefault(eq => eq.EquipmentIdentityID == wagonDrillTimesheet.EquipmentCategoryId).EquipmentCategoryName;
        //            item.EquipmentName = db.EquipmentName.FirstOrDefault(eq => eq.EquipmentNameID == wagonDrillTimesheet.EquipmentNameId).Name;

        //            item.WorkItem = wagonDrillTimesheet.WorkItem;
        //            item.OperationNetWorkingHour = wagonDrillTimesheet.OperationNetWorkingHour;
        //            item.OperationDrilledHoleDepth = wagonDrillTimesheet.OperationDrilledHoleDepth;
        //            item.NumberOfHolesDrilled = wagonDrillTimesheet.NumberOfHolesDrilled;
        //            item.OperationRateForMeter = wagonDrillTimesheet.RateInMeter;
        //            item.OperationAmount = Math.Round(item.OperationDrilledHoleDepth * item.NumberOfHolesDrilled * item.OperationRateForMeter, 2);
        //            item.IdleHour = wagonDrillTimesheet.IdleHour;
        //            item.IdleRateHour = wagonDrillTimesheet.IdleRateHour;
        //            item.IdleAmount = item.IdleHour * item.IdleRateHour;
        //            item.DownTime = wagonDrillTimesheet.DownTime;
        //            item.TotalAmountBeforeVAT = item.IdleAmount * item.OperationAmount;
        //            item.VAT = wagonDrillTimesheet.VAT;
        //            item.TotalAmount = item.TotalAmountBeforeVAT + item.TotalAmountBeforeVAT * item.VAT;
        //            item.FuelInLiter = wagonDrillTimesheet.FuelInLiter;
        //            item.FuelRate = wagonDrillTimesheet.FuelRate;
        //            item.FuelCost = item.FuelInLiter * item.FuelRate;
        //            item.LubricantCost = wagonDrillTimesheet.LubricantCost;
        //            item.Perdium = wagonDrillTimesheet.Perdium;
        //            item.Salary = wagonDrillTimesheet.Salary;
        //            item.MaintenanceCost = wagonDrillTimesheet.MaintenanceCost;
        //            item.TireCost = wagonDrillTimesheet.TireCost;

        //            item.Revenue = wagonDrillTimesheet.OperationAmount + wagonDrillTimesheet.IdleAmount;
        //            item.TotalCost = item.Perdium + item.Salary + item.LubricantCost + item.FuelCost + item.MaintenanceCost + item.TireCost;

        //            item.NetAmount = item.Revenue - item.TotalCost;

        //            list.Add(item);
        //        }
        //        return list.OrderBy(x => x.Date.Date).ToList();
        //    }
        //    return list;
        //}
    }
}
