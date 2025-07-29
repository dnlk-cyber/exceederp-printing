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
    public class WagonDrillTimesheetObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public WagonDrillTimesheetHeaderViewModel WagonDrillTimesheetHeader(string fleetId, string startPlace, string ArrivalPlace, DateTime from, DateTime to)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new WagonDrillTimesheetHeaderViewModel();
            if (fleetId != null)
            {
                Guid FId = Guid.Parse(fleetId);

                var wagonDrillTimesheet = db.WagonDrillTimesheets.FirstOrDefault(x => x.FleetsID == FId && DbFunctions.TruncateTime(x.Date) >= from.Date && DbFunctions.TruncateTime(x.Date) <= to.Date);
                if (wagonDrillTimesheet != null)
                {
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == wagonDrillTimesheet.BranchId);
                    var fleet = db.Fleets.Find(FId);
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
        public List<WagonDrillTimesheetViewModel> WagonDrillTimesheet(string fleetId, DateTime? from, DateTime? to, string IntExt, string businessUnitId, string categoryId, string subCategoryId)
        {
            var result = new List<WagonDrillTimesheetViewModel>();

            if (!Guid.TryParse(fleetId, out Guid FId)) return result;

            var query = db.WagonDrillTimesheets.Where(x => x.CostSheetApproved && x.FleetsID == FId);
           // var query = db.WagonDrillTimesheets.AsQueryable();
            var fromDate = from?.Date ?? DateTime.MinValue;
            var toDate = to?.Date ?? DateTime.MaxValue;
            var toDateExclusive = toDate.AddDays(1); // Avoid using AddDays() in query

            query = query.Where(x => x.Date >= fromDate && x.Date < toDateExclusive);
            //if (from.HasValue)
            //    query = query.Where(x => DbFunctions.TruncateTime(x.Date) >= from.Value.Date);

            //if (to.HasValue)
            //    query = query.Where(x => DbFunctions.TruncateTime(x.Date) <= to.Value.Date);

            if (int.TryParse(businessUnitId, out int buId))
                query = query.Where(x => x.BusinessUnitId == buId);

            // Filter by internal or external fleet
            if (!string.IsNullOrWhiteSpace(IntExt))
            {
                bool isExternal = IntExt == "Rent";
                var fleetIds = db.Fleets.Where(f => f.IsExternal == isExternal).Select(f => f.FleetsID).ToList();
                query = query.Where(x => fleetIds.Contains(x.FleetsID));
            }

            // Filter by category
            if (Guid.TryParse(categoryId, out Guid catId))
            {
                var fleetIds = db.Fleets.Where(f => f.EquipmentIdentityID == catId).Select(f => f.FleetsID).ToList();
                query = query.Where(x => fleetIds.Contains(x.FleetsID));
            }

            // Filter by subcategory
            if (Guid.TryParse(subCategoryId, out Guid subCatId))
            {
                var fleetIds = db.Fleets.Where(f => f.EquipmentTypeID == subCatId).Select(f => f.FleetsID).ToList();
                query = query.Where(x => fleetIds.Contains(x.FleetsID));
            }

            var fleetMap = db.Fleets.ToDictionary(f => f.FleetsID, f => f.EquipmentTypeID);
            var equipmentMap = db.EquipmentType.ToDictionary(e => e.EquipmentTypeID, e => e);

            var timesheets = query.ToList();

            foreach (var ts in timesheets)
            {
                var vm = new WagonDrillTimesheetViewModel
                {
                    Date = ts.Date,
                    WorkItem = ts.WorkItem,
                    OperationNetWorkingHour = ts.OperationNetWorkingHour,
                    OperationDrilledHoleDepth = ts.OperationDrilledHoleDepth,
                    NumberOfHolesDrilled = ts.NumberOfHolesDrilled,
                    OperationRateForMeter = ts.RateInMeter,
                    OperationAmount = Math.Round(ts.OperationDrilledHoleDepth * ts.NumberOfHolesDrilled * ts.RateInMeter, 2),
                    IdleHour = ts.IdleHour,
                    IdleRateHour = ts.IdleRateHour,
                    IdleAmount = ts.IdleHour * ts.IdleRateHour,
                    DownTime = ts.DownTime,
                    VAT = ts.VAT,
                    FuelInLiter = ts.FuelInLiter,
                    FuelRate = ts.FuelRate,
                    FuelCost = ts.FuelInLiter * ts.FuelRate,
                    LubricantCost = ts.LubricantCost,
                    Perdium = ts.Perdium,
                    Salary = ts.Salary,
                    MaintenanceCost = ts.MaintenanceCost,
                    TireCost = ts.TireCost,
                };

                // Computed Fields
                vm.TotalAmountBeforeVAT = vm.OperationAmount + vm.IdleAmount;
                vm.TotalAmount = vm.TotalAmountBeforeVAT + (vm.TotalAmountBeforeVAT * vm.VAT);
                vm.Revenue = vm.OperationAmount + vm.IdleAmount;
                vm.TotalCost = vm.Perdium + vm.Salary + vm.LubricantCost + vm.FuelCost + vm.MaintenanceCost + vm.TireCost;
                vm.NetAmount = vm.Revenue - vm.TotalCost;

                result.Add(vm);
            }

            return result.OrderBy(x => x.Date).ToList();
        }
    }
}
