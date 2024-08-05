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

    public class DumpTruckExternalTimesheetObjectDataSource
    {

        [DataObjectMethod(DataObjectMethodType.Select)]
        public DumpTruckExternalTimesheetHeaderViewModel DumpTruckExternalTimesheetHeader(string fleetId, DateTime from, DateTime to)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new DumpTruckExternalTimesheetHeaderViewModel();

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
        public List<DumpTruckExternalTimesheetViewModel> DumpTruckExternalTimesheet(string fleetId, DateTime from, DateTime to, string businessUnitId, string categoryId, string subCategoryId, string project)
        {
            var list = new List<DumpTruckExternalTimesheetViewModel>();
            if (fleetId != null)
            {
                Guid FId = Guid.Parse(fleetId);
                var dumpTruckTimeSheets = db.DumpTruckTimeSheets.Where(x => x.FleetsID == FId && x.CostSheetApproved).ToList();
                dumpTruckTimeSheets = dumpTruckTimeSheets.Where(x => x.Date.Date >= from.Date && x.Date.Date <= to.Date).ToList();
               
                if (!string.IsNullOrWhiteSpace(businessUnitId))
                {
                    dumpTruckTimeSheets = dumpTruckTimeSheets.Where(x => x.BusinessUnitId == int.Parse(businessUnitId)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    Guid.TryParse(categoryId, out Guid id);
                    List<DumpTruckTimeSheet> dumpTruckList = new List<DumpTruckTimeSheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var dryLoadingFiltered = dumpTruckTimeSheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        dumpTruckList.AddRange(dryLoadingFiltered);
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
                    var item = new DumpTruckExternalTimesheetViewModel();
                    var dumpTruckDetails = db.DumpTruckTimeSheetDetails.Where(x => x.DumpTruckTimeSheetId == dumpTruckTimeSheet.DumpTruckTimeSheetId).ToList();
                    foreach (var detail in dumpTruckDetails)
                    {
                        var fleet = db.Fleets.Find(dumpTruckTimeSheet.FleetsID);
                        var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
                        var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
                        item.SN = dumpTruckTimeSheet.DumpTruckTimeSheetId;
                        item.Date = dumpTruckTimeSheet.Date;
                        item.WorkItem = detail.TypeOfWork;
                        item.OperationNetWorkingHour = 1; //??
                        item.NumberOfTrips = detail.NumberOfTrip;
                        item.ActualDistance = detail.ActualDistance;
                        item.Volume = detail.Capacity;
                        item.RatePerKm = detail.RentalRate;
                        item.TotalHaulage = detail.TotalHaulage;
                        item.OperationAmount = item.Volume * item.ActualDistance * item.NumberOfTrips * item.RatePerKm;
                        item.IdleHour = dumpTruckTimeSheet.IdleHour;
                        item.IdleRateperHour = dumpTruckTimeSheet.IdealRentalRate;
                        item.IdleAmount = item.IdleRateperHour * item.IdleHour;
                        item.TotalAmountBeforVAT = item.IdleAmount + item.OperationAmount;
                        item.VAT = 0.15M;
                        item.TotalAmount = item.TotalAmountBeforVAT * item.VAT + item.TotalAmountBeforVAT;
                        item.DownHour = dumpTruckTimeSheet.DownHour;
                        item.FuelInLiter = dumpTruckTimeSheet.FuelConsumption;
                        item.FuelRate = dumpTruckTimeSheet.FuelRate;
                        item.FuelCost = item.FuelRate * item.FuelInLiter;
                        item.ServiceCharge = dumpTruckTimeSheet.Salary;//??
                        item.LubricantCost = dumpTruckTimeSheet.LubricantCost;
                        item.MaintenaceCost = dumpTruckTimeSheet.MaintenanceCost;
                        item.TotalCost = item.MaintenaceCost + item.LubricantCost + item.FuelCost + item.ServiceCharge;
                        item.NetIncome = item.TotalAmount - item.TotalCost;

                        list.Add(item);
                    }

                }
                return list.OrderBy(x => x.Date.Date).ToList();
            }
            return list;
        }
    }
}
