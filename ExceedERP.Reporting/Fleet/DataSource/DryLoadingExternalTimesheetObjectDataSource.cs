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
    public class DryLoadingExternalTimesheetObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public DryLoadingExternalTimesheetHeaderVM DryLoadingExternalTimesheetHeader(string fleetId, string startPlace, string ArrivalPlace, DateTime from, DateTime to)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new DryLoadingExternalTimesheetHeaderVM();

            if (fleetId != null)
            {
                Guid FId = Guid.Parse(fleetId);
                
                var dryLoadingTimesheet = db.DryLoadingTimeSheets.FirstOrDefault(x => x.FleetsID == FId && DbFunctions.TruncateTime(x.TripStartDate) >= from.Date && DbFunctions.TruncateTime(x.TripStartDate) <= to.Date);

                if (dryLoadingTimesheet != null)
                {
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == dryLoadingTimesheet.BranchId);
                    var fleet = db.Fleets.Find(FId);
                    var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
                    var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
                    var rentFromExternals = db.RentFromExternal.FirstOrDefault(x => x.RentFromExternalID == fleet.RentId);
                    var equipmentName = db.EquipmentName.FirstOrDefault(x => x.EquipmentNameID == fleet.EquipmentNameID);
                    var operatorName = db.Operator.Find(dryLoadingTimesheet.OperatorId);
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
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DryLoadingExternalTimesheetViewModel> DryLoadingExternalTimesheet(string fleetId, string startPlace, string ArrivalPlace, DateTime from, DateTime to, string categoryId, string subCategoryId, string project)
        {
            var list = new List<DryLoadingExternalTimesheetViewModel>();
            if (fleetId != null)
            {


                Guid FId = Guid.Parse(fleetId);

                var dryLoadingExternalTimesheets = db.DryLoadingTimeSheets.Where(x => x.FleetsID == FId).ToList();

                dryLoadingExternalTimesheets = dryLoadingExternalTimesheets.Where(x => x.TripStartDate.Date >= from.Date && x.TripStartDate <= to.Date).ToList();

                
                if (startPlace != null)
                {
                    dryLoadingExternalTimesheets = dryLoadingExternalTimesheets.Where(x => x.TripStartPlace == startPlace).ToList();
                }
                if (ArrivalPlace != null)
                {
                    int.TryParse(ArrivalPlace, out int aId);
                    dryLoadingExternalTimesheets = dryLoadingExternalTimesheets.Where(x => x.DryLoadingTimeSheetId == aId).ToList();
                }
                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    Guid.TryParse(categoryId, out Guid id);
                    List<DryLoadingTimeSheet> dryLoadingList = new List<DryLoadingTimeSheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var dryLoadingFiltered = dryLoadingExternalTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        dryLoadingList.AddRange(dryLoadingFiltered);
                    }
                    dryLoadingExternalTimesheets = dryLoadingList;
                }
                if (!string.IsNullOrWhiteSpace(subCategoryId))
                {
                    Guid.TryParse(subCategoryId, out Guid id);
                    List<DryLoadingTimeSheet> dryLoadingList = new List<DryLoadingTimeSheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var dryLoadingFiltered = dryLoadingExternalTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        dryLoadingList.AddRange(dryLoadingFiltered);
                    }
                    dryLoadingExternalTimesheets = dryLoadingList;
                }
                if (!string.IsNullOrWhiteSpace(project))
                {
                    int.TryParse(project, out int branchId);
                    dryLoadingExternalTimesheets = dryLoadingExternalTimesheets.Where(x => x.BranchId == branchId).ToList();
                }
                foreach (var dryLoadingExternalTimesheet in dryLoadingExternalTimesheets)
                {
                    var item = new DryLoadingExternalTimesheetViewModel();
                    var fleet = db.Fleets.Find(dryLoadingExternalTimesheet.FleetsID);
                    var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
                    var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
                    //if (equipment != null && fleet != null)
                    //{
                    //    item.EquipmentName = equipment.EquipmentTypeName;
                    //}
                    item.Sn = dryLoadingExternalTimesheet.DryLoadingTimeSheetId;
                    item.WorkType = dryLoadingExternalTimesheet.WorkItem;
                    item.DeparturePlace = dryLoadingExternalTimesheet.TripStartPlace;
                    item.ArrivalPlace = dryLoadingExternalTimesheet.TriplArrivalPlace;
                    item.StartingDate = dryLoadingExternalTimesheet.TripStartDate;
                    item.EndDate = dryLoadingExternalTimesheet.TripArrivalDate;
                    item.Category = dryLoadingExternalTimesheet.Category;
                    item.NoOfTripe = dryLoadingExternalTimesheet.NoOfTripe;
                    item.AsphaltTraveled = dryLoadingExternalTimesheet.Asphalt;
                    item.GravelTraveled = dryLoadingExternalTimesheet.Gravel;
                    item.AsphaltRate = dryLoadingExternalTimesheet.AsphaltRate;
                    item.GravelRate = dryLoadingExternalTimesheet.GravelRate;
                    item.LoadedType = dryLoadingExternalTimesheet.LoadedType;
                    item.TotalAmountBeforeVAT = item.NoOfTripe * (item.AsphaltTraveled * item.AsphaltRate + item.GravelTraveled * item.GravelRate);
                    item.VAT = 0.15M;
                    item.TotalAmount = item.TotalAmountBeforeVAT * item.VAT + item.TotalAmountBeforeVAT;
                    item.FuelInLiter = dryLoadingExternalTimesheet.FuelInLiter;
                    item.FuelRate = dryLoadingExternalTimesheet.FuelRate;
                    item.FuelCost = item.FuelInLiter * item.FuelRate;
                    item.LubricantCost = dryLoadingExternalTimesheet.LubricantCost;
                    item.MaintenaceCost = dryLoadingExternalTimesheet.MaintenanceCost;
                    item.ServiceCharge = dryLoadingExternalTimesheet.Salary; //??
                                                                             //item.TotalCost = 10;
                    item.TotalCost = item.ServiceCharge + item.MaintenaceCost + item.LubricantCost + item.FuelCost;
                    item.NetIncome = item.TotalAmount - item.TotalCost;
                    list.Add(item);
                }
                return list;
            }
            return list;
        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetVM> GetFleetDryLoading()
        {


            var list = from dryLoading in db.DryLoadingTimeSheets
                       join fleet in db.Fleets on dryLoading.FleetsID equals fleet.FleetsID
                       select new FleetVM
                       {
                           FleetsID = fleet.FleetsID.ToString(),
                           PlateNumber = fleet.PlateNumber
                       };
            return list.ToList();
        }
    }
}
