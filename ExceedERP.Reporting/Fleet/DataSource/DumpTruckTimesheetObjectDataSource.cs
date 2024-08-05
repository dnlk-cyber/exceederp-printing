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
    public class DumpTruckTimesheetObjectDataSource
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public DumpTruckTimesheetHeaderViewModel DumpTruckTimesheetHeader(string fleetId, DateTime from, DateTime to, string project)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new DumpTruckTimesheetHeaderViewModel();

            if (fleetId != null)
            {
                Guid FId = Guid.Parse(fleetId);
                var dumpTruckTimeSheet = db.DumpTruckTimeSheets.FirstOrDefault(x => x.FleetsID == FId && DbFunctions.TruncateTime(x.Date) >= from.Date && DbFunctions.TruncateTime(x.Date) <= to.Date);

                
                if (dumpTruckTimeSheet != null)
                {
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == dumpTruckTimeSheet.BranchId);
                    var fleet = db.Fleets.Find(FId);
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
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DumpTruckTimesheetViewModel> DumpTruckTimesheet(string fleetId, DateTime from, DateTime to, string businessUnitId, string categoryId, string subCategoryId, string project)
        {

            var list = new List<DumpTruckTimesheetViewModel>();
            if (fleetId != null)
            {

                Guid FId = Guid.Parse(fleetId);
                var dumpTruckTimeSheets = db.DumpTruckTimeSheets.Where(x => x.FleetsID == FId && x.CostSheetApproved).ToList();

                dumpTruckTimeSheets = dumpTruckTimeSheets.Where(x => x.Date.Date >= from.Date && x.Date.Date <= to.Date).ToList();
               
                if (!string.IsNullOrWhiteSpace(businessUnitId))
                {
                    int.TryParse(businessUnitId, out int bUId);
                    dumpTruckTimeSheets = dumpTruckTimeSheets.Where(x => x.BusinessUnitId == bUId).ToList();
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
                if (!string.IsNullOrWhiteSpace(project))
                {
                    int.TryParse(project, out int branchId);
                    dumpTruckTimeSheets = dumpTruckTimeSheets.Where(x => x.BranchId == branchId).ToList();
                }
                foreach (var dumpTruckTimeSheet in dumpTruckTimeSheets)
                {

                    bool checker = true;
                    var dumptruckDetails = db.DumpTruckTimeSheetDetails.Where(x => x.DumpTruckTimeSheetId == dumpTruckTimeSheet.DumpTruckTimeSheetId).ToList();
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == dumpTruckTimeSheet.BranchId);
                    foreach (var detail in dumptruckDetails)
                    {

                        var item = new DumpTruckTimesheetViewModel();
                        var fleet = db.Fleets.Find(dumpTruckTimeSheet.FleetsID);
                        var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
                        var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
                        item.PlateNo = fleet.PlateNumber;
                        item.Branch = branch.Name;
                        item.SN = dumpTruckTimeSheet.DumpTruckTimeSheetId;
                        item.Date = dumpTruckTimeSheet.Date;
                        item.WorkItem = detail.TypeOfWork;
                        item.OperationNetWorkingHour = dumpTruckTimeSheet.NetworkingHour;
                        item.NumberOfTrips = detail.NumberOfTrip;
                        item.ActualDistance = detail.ActualDistance;
                        item.capacity = detail.Capacity;
                        item.TotalHaulage = detail.TotalHaulage;

                        item.OperationRateperHour = detail.RentalRate;
                        item.OperationRevenue = item.capacity * item.ActualDistance * item.NumberOfTrips * item.OperationRateperHour;

                        item.IdleRateperHour = dumpTruckTimeSheet.IdealRentalRate;
                        item.IdleRevenue = item.IdleRateperHour * item.IdleHour;
                        item.TotalRevenue = item.IdleRevenue + item.OperationRevenue;
                        item.DownHour = dumpTruckTimeSheet.DownHour;


                        if (checker)
                        {
                            item.IdleHour = dumpTruckTimeSheet.IdleHour;
                            item.FuelInLiter = dumpTruckTimeSheet.FuelConsumption;
                            item.FuelRate = dumpTruckTimeSheet.FuelRate;
                            item.FuelCost = item.FuelRate * item.FuelInLiter;
                            item.LubricantCost = dumpTruckTimeSheet.LubricantCost;
                            item.MaintenaceCost = dumpTruckTimeSheet.MaintenanceCost;
                            item.TireCost = dumpTruckTimeSheet.TireCost;
                            item.Salary = dumpTruckTimeSheet.Salary;
                            item.Perdium = dumpTruckTimeSheet.Perdium;
                            item.TotalCost = item.Perdium + item.Salary + item.TireCost + item.MaintenaceCost + item.LubricantCost + item.FuelCost;
                            //item.TotalCost = item.NumberOfTrips * item.ActualDistance * item.capacity * item.OperationRateperHour; 
                            item.NetIncome = item.TotalRevenue - item.TotalCost;
                        }
                        else
                        {
                            item.FuelInLiter = 0;
                            item.FuelRate = 0;
                            item.FuelCost = 0;
                            item.LubricantCost = 0;
                            item.MaintenaceCost = 0;
                            item.TireCost = 0;
                            item.Salary = 0;
                            item.Perdium = 0;
                            item.TotalCost = 0;
                            //item.TotalCost = item.NumberOfTrips * item.ActualDistance * item.capacity * item.OperationRateperHour; 
                            item.NetIncome = item.TotalRevenue - item.TotalCost;
                        }



                        list.Add(item);
                        item = null;
                        checker = false;
                    }

                }
                return list.OrderBy(x => x.Date.Date).ToList();
            }
            return list;
        }
    }
}
