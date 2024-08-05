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
    public class WagonDrillExternalTimesheetDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public WagonDrillExternalTimesheetHeaderViewModel WagonDrillExternalTimesheetHeader(string fleetId, DateTime from, DateTime to)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new WagonDrillExternalTimesheetHeaderViewModel();
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
        public List<WagonDrillExternalTimesheetViewModel> WagonDrillExternalTimesheet(string fleetId, DateTime from, DateTime to, string businessUnitId, string categoryId, string subCategoryId, string project)
        {
            var list = new List<WagonDrillExternalTimesheetViewModel>();
            if (fleetId != null)
            {

                Guid FId = Guid.Parse(fleetId);
                var wagonDrillExternalTimesheets = db.WagonDrillTimesheets.Where(x => x.FleetsID == FId && x.CostSheetApproved).ToList();

                wagonDrillExternalTimesheets = wagonDrillExternalTimesheets.Where(x => x.Date.Date >= from.Date && x.Date.Date <= from.Date).ToList();

                if (!string.IsNullOrWhiteSpace(businessUnitId))
                {
                    wagonDrillExternalTimesheets = wagonDrillExternalTimesheets.Where(x => x.BusinessUnitId == int.Parse(businessUnitId)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    Guid.TryParse(categoryId, out Guid id);
                    List<WagonDrillTimesheet> wagonDrillList = new List<WagonDrillTimesheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var wagonDrillFiltered = wagonDrillExternalTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        wagonDrillList.AddRange(wagonDrillFiltered);
                    }
                    wagonDrillExternalTimesheets = wagonDrillList;
                }
                if (!string.IsNullOrWhiteSpace(subCategoryId))
                {
                    Guid.TryParse(subCategoryId, out Guid id);
                    List<WagonDrillTimesheet> wagonDrillList = new List<WagonDrillTimesheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var wagonDrillFiltered = wagonDrillExternalTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        wagonDrillList.AddRange(wagonDrillFiltered);
                    }
                    wagonDrillExternalTimesheets = wagonDrillList;
                }
                if (!string.IsNullOrWhiteSpace(project))
                {
                    int.TryParse(project, out int branchId);
                    wagonDrillExternalTimesheets = wagonDrillExternalTimesheets.Where(x => x.BranchId == branchId).ToList();
                }
                foreach (var wagonDrillExternalTimesheet in wagonDrillExternalTimesheets)
                {
                    var item = new WagonDrillExternalTimesheetViewModel();
                    var fleet = db.Fleets.Find(wagonDrillExternalTimesheet.FleetsID);
                    var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
                    var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
                    item.Date = wagonDrillExternalTimesheet.Date;
                    item.WorkItem = wagonDrillExternalTimesheet.WorkItem;
                    item.OperationNetWorkingHr = wagonDrillExternalTimesheet.OperationNetWorkingHour;
                    item.OperationDrilledDepthHole = wagonDrillExternalTimesheet.OperationDrilledHoleDepth;
                    item.OperationNumberOfHoles = wagonDrillExternalTimesheet.NumberOfHolesDrilled;
                    item.OperationRatePerMeter = wagonDrillExternalTimesheet.OperationRateForMeter;
                    item.OperationAmount = item.OperationDrilledDepthHole * item.OperationNumberOfHoles * item.OperationRatePerMeter;
                    item.IdleHour = wagonDrillExternalTimesheet.IdleHour;
                    item.IdleRatePerHour = wagonDrillExternalTimesheet.IdleRateHour;
                    item.IdleAmount = item.IdleHour * item.IdleRatePerHour;
                    item.DownTime = wagonDrillExternalTimesheet.DownTime;
                    item.TotalAmountBeforVAT = wagonDrillExternalTimesheet.TotalAmountBeforeVAT;
                    item.VAT = wagonDrillExternalTimesheet.VAT;
                    item.TotalAmount = item.TotalAmountBeforVAT * item.VAT + item.TotalAmountBeforVAT;
                    item.FuelInLiter = wagonDrillExternalTimesheet.FuelInLiter;
                    item.FuelRate = wagonDrillExternalTimesheet.FuelRate;
                    item.FuelCost = item.FuelInLiter * item.FuelRate;
                    item.TireCost = wagonDrillExternalTimesheet.TireCost;
                    item.LubricantCost = wagonDrillExternalTimesheet.LubricantCost;
                    item.MaintenanceCost = wagonDrillExternalTimesheet.MaintenanceCost;
                    item.ServiceCharge = wagonDrillExternalTimesheet.ServiceCharge;
                    item.Revenue = wagonDrillExternalTimesheet.NumberOfHolesDrilled * wagonDrillExternalTimesheet.OperationDrilledHoleDepth * wagonDrillExternalTimesheet.RateInMeter;
                    item.TotalCost = item.ServiceCharge + item.MaintenanceCost + item.LubricantCost + item.FuelCost + item.TireCost;
                    item.NetAmount = item.TotalAmount - item.TotalCost;
                    item.NetAmount = item.Revenue - item.TotalCost;
                    list.Add(item);
                }
                return list.OrderBy(x => x.Date.Date).ToList();
            }
            return list;
        }
    }
}
