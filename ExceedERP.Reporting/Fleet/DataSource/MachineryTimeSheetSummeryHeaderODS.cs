using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.Core.Domain.Procurement.Supplier;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
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
                List<MachineryWorkingTimeSheet> PreparedBy = db.MachineryWorkingTimeSheets.Where(x => x.FleetsID == FId && DbFunctions.TruncateTime(x.Date) >= from.Date && DbFunctions.TruncateTime(x.Date) <= to.Date).ToList();
                MachineryWorkingTimeSheet LastMachineryTimeSheet = PreparedBy.LastOrDefault();
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
                    if (LastMachineryTimeSheet.CreatedBy != null)
                    {
                        header.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(LastMachineryTimeSheet.CreatedBy));
                    }
                    if (LastMachineryTimeSheet.CostSheetSendForApprovalBy != null)
                    {
                        header.CheckedBY = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(LastMachineryTimeSheet.CostSheetSendForApprovalBy));
                    }
                    if (LastMachineryTimeSheet.CostSheetApprovedBy != null)
                    {
                        header.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(LastMachineryTimeSheet.CostSheetApprovedBy));
                    }
                    header.PreparedDate = LastMachineryTimeSheet.DateCreated?.ToString("yyyy-MM-dd")??"";
                    header.CheckedDate = LastMachineryTimeSheet.CostSheetSendForApprovalTime?.ToString("yyyy-MM-dd")??"";
                    header.ApprovedDate = LastMachineryTimeSheet.CostSheetApprovedDate?.ToString("yyyy-MM-dd")??"";
                    if (LastMachineryTimeSheet.TimeSheetApprovedBy != null)
                    {
                        header.TimesheetApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(LastMachineryTimeSheet.TimeSheetApprovedBy));
                    }
                    header.TimesheetApprovedDate = LastMachineryTimeSheet.TimeSheetApprovedDate?.ToString("yyyy-MM-dd")??"";
                    header.TimesheetSendForApprovalBy = LastMachineryTimeSheet.TimeSheetSendForApprovalTime?.ToString("yyyy-MM-dd")??"";
                    if (LastMachineryTimeSheet.TimeSheetSendForApprovalBy != null)
                    {
                        header.TimesheetSendForApprovalBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(LastMachineryTimeSheet.TimeSheetSendForApprovalBy));
                    }
                }
            }

            return header;
        }

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MachineryTimeSheetSummeryVM> MachineryTimeSheetSummery(string fleetId, DateTime from, DateTime to, string IntExt, string project, string businessUnitId, string categoryId, string subCategoryId)
        {
            var list = new List<MachineryTimeSheetSummeryVM>();

            if (!Guid.TryParse(fleetId, out Guid FId)) return list;

            var query = db.MachineryWorkingTimeSheets
                .Where(x => x.CostSheetApproved &&
                            x.FleetsID == FId &&
                            x.Date >= from.Date &&
                            x.Date <= to.Date);

            if (int.TryParse(project, out int branchId))
                query = query.Where(x => x.BranchId == branchId);

            if (int.TryParse(businessUnitId, out int buId))
                query = query.Where(x => x.BusinessUnitId == buId);

            // External/Internal filtering
            if (!string.IsNullOrWhiteSpace(IntExt))
            {
                bool isExternal = IntExt == "Rent";
                var fleetIds = db.Fleets.Where(f => f.IsExternal == isExternal).Select(f => f.FleetsID).ToList();
                query = query.Where(x => fleetIds.Contains(x.FleetsID));
            }

            // Category filter
            if (Guid.TryParse(categoryId, out Guid catId))
            {
                var fleetIds = db.Fleets.Where(f => f.EquipmentIdentityID == catId).Select(f => f.FleetsID).ToList();
                query = query.Where(x => fleetIds.Contains(x.FleetsID));
            }

            // Subcategory filter
            if (Guid.TryParse(subCategoryId, out Guid subCatId))
            {
                var fleetIds = db.Fleets.Where(f => f.EquipmentTypeID == subCatId).Select(f => f.FleetsID).ToList();
                query = query.Where(x => fleetIds.Contains(x.FleetsID));
            }

            var filteredSheets = query.ToList();

            foreach (var sheet in filteredSheets)
            {
                var opHours = (sheet.MorningTimeEnd - sheet.MorningTimeStart).TotalHours +
                              (sheet.AfternoonTimeEnd - sheet.AfternoonTimeStart).TotalHours +
                              (sheet.NightTimeEnd - sheet.NightTimeStart).TotalHours;

                var vm = new MachineryTimeSheetSummeryVM
                {
                    Date = sheet.Date.Date,
                    WorkItem = sheet.WorkType,
                    OperationNetworkinHour = Math.Round((decimal)opHours, 2),
                    OperationRatePerHour = sheet.RentalRate,
                    OpertionRevenue = Math.Round((decimal)opHours, 2) * sheet.RentalRate,
                    IdleHour = sheet.IdleHours,
                    IdleRatePerHour = sheet.IdealRentalRate,
                    IdleRevenue = sheet.IdleHours * sheet.IdealRentalRate,
                    DownHour = sheet.DownHours,
                    FuelInLiter = sheet.FuelConsumption,
                    FuelRate = sheet.FuelRate,
                    FuelCost = sheet.FuelConsumption * sheet.FuelRate,
                    LubricantCost = sheet.LubricantCost,
                    MaintenanceCost = sheet.MaintenanceCost,
                    TireCost = sheet.TireCost,
                    Perdium = sheet.Perdium,
                    Salary = sheet.Salary,
                };

                vm.TotalRevenue = vm.OpertionRevenue + vm.IdleRevenue;
                vm.TotalCost = vm.FuelCost + vm.LubricantCost + vm.MaintenanceCost + vm.TireCost + vm.Perdium + vm.Salary;
                vm.NetIncome = vm.TotalRevenue - vm.TotalCost;

                list.Add(vm);
            }

            return list.OrderBy(x => x.Date).ToList();
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
