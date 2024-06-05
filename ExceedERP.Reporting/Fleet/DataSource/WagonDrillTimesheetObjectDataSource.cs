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
        public List<WagonDrillTimesheetViewModel> WagonDrillTimesheet(string fleetId, DateTime? from, DateTime? to, string businessUnitId, string categoryId, string subCategoryId)
        {
            var list = new List<WagonDrillTimesheetViewModel>();
            if (fleetId != null)
            {


                var wagonDrillTimesheets = db.WagonDrillTimesheets.ToList();

                if (fleetId != null)
                {
                    Guid FId = Guid.Parse(fleetId);
                    wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.FleetsID == FId).ToList();
                }
                if (from.HasValue)
                {
                    wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.Date.Date >= from.Value.Date).ToList();
                }
                if (to.HasValue)
                {
                    wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.Date.Date <= to.Value.Date).ToList();
                }
                if (!string.IsNullOrWhiteSpace(businessUnitId))
                {
                    wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.BusinessUnitId == int.Parse(businessUnitId)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    Guid.TryParse(categoryId, out Guid id);
                    List<WagonDrillTimesheet> wagonDrillList = new List<WagonDrillTimesheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var wagonDrillFiltered = wagonDrillTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        wagonDrillList.AddRange(wagonDrillFiltered);
                    }
                    wagonDrillTimesheets = wagonDrillList;
                }
                if (!string.IsNullOrWhiteSpace(subCategoryId))
                {
                    Guid.TryParse(subCategoryId, out Guid id);
                    List<WagonDrillTimesheet> wagonDrillList = new List<WagonDrillTimesheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var wagonDrillFiltered = wagonDrillTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        wagonDrillList.AddRange(wagonDrillFiltered);
                    }
                    wagonDrillTimesheets = wagonDrillList;
                }
                foreach (var wagonDrillTimesheet in wagonDrillTimesheets)
                {
                    var item = new WagonDrillTimesheetViewModel();
                    var fleet = db.Fleets.Find(wagonDrillTimesheet.FleetsID);
                    var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
                    var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
                    item.Date = wagonDrillTimesheet.Date;

                    item.WorkItem = wagonDrillTimesheet.WorkItem;
                    item.OperationNetWorkingHour = wagonDrillTimesheet.OperationNetWorkingHour;
                    item.OperationDrilledHoleDepth = wagonDrillTimesheet.OperationDrilledHoleDepth;
                    item.NumberOfHolesDrilled = wagonDrillTimesheet.NumberOfHolesDrilled;
                    item.OperationRateForMeter = wagonDrillTimesheet.OperationRateForMeter;
                    item.OperationAmount = Math.Round(item.OperationDrilledHoleDepth * item.NumberOfHolesDrilled * item.OperationRateForMeter, 2);
                    item.IdleHour = wagonDrillTimesheet.IdleHour;
                    item.IdleRateHour = wagonDrillTimesheet.IdleRateHour;
                    item.IdleAmount = item.IdleHour * item.IdleRateHour;
                    item.DownTime = wagonDrillTimesheet.DownTime;
                    item.TotalAmountBeforeVAT = item.IdleAmount * item.OperationAmount;
                    item.VAT = wagonDrillTimesheet.VAT;
                    item.TotalAmount = item.TotalAmountBeforeVAT + item.TotalAmountBeforeVAT * item.VAT;
                    item.FuelInLiter = wagonDrillTimesheet.FuelInLiter;
                    item.FuelRate = wagonDrillTimesheet.FuelRate;
                    item.FuelCost = item.FuelInLiter * item.FuelRate;
                    item.LubricantCost = wagonDrillTimesheet.LubricantCost;
                    item.Perdium = wagonDrillTimesheet.Perdium;
                    item.Salary = wagonDrillTimesheet.Salary;
                    item.MaintenanceCost = wagonDrillTimesheet.MaintenanceCost;
                    item.TireCost = wagonDrillTimesheet.TireCost;
                    item.Revenue = wagonDrillTimesheet.NumberOfHolesDrilled * wagonDrillTimesheet.OperationDrilledHoleDepth * wagonDrillTimesheet.RateInMeter;
                    item.TotalCost = item.Perdium + item.Salary + item.LubricantCost + item.FuelCost + item.MaintenanceCost + item.TireCost;

                    item.NetAmount = item.Revenue - item.TotalCost;

                    list.Add(item);
                }
                return list.OrderBy(x => x.Date.Date).ToList();
            }
            return list;
        }
    }
}
