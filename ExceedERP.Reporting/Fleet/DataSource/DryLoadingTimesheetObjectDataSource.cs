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
    public class DryLoadingTimesheetObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public DryLoadingTimesheetHeaderVM DryLoadingTimesheetHeader(string fleetId, string startPlace, string ArrivalPlace, DateTime from, DateTime to)
        {
            ProcurementSupplier procurementSupplier = new ProcurementSupplier();
            var header = new DryLoadingTimesheetHeaderVM();

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
        public List<DryLoadingTimesheetViewModel> DryLoadingTimesheet(string fleetId, string startPlace, string ArrivalPlace, DateTime from, DateTime to, string businessUnitId, string categoryId, string subCategoryId, string projectId)
        {

            var list = new List<DryLoadingTimesheetViewModel>();
            if (fleetId != null)
            {
                Guid FId = Guid.Parse(fleetId);

                var dryLoadingTimesheets = db.DryLoadingTimeSheets.Where(x => x.FleetsID == FId && x.CostSheetApproved).ToList();

                dryLoadingTimesheets = dryLoadingTimesheets.Where(x => x.TripStartDate.Date >= from.Date && x.TripStartDate <= to.Date).ToList();

                if (startPlace != null)
                {
                    dryLoadingTimesheets = dryLoadingTimesheets.Where(x => x.TripStartPlace == startPlace).ToList();
                }
                if (ArrivalPlace != null)
                {
                    int.TryParse(ArrivalPlace, out int aId);
                    dryLoadingTimesheets = dryLoadingTimesheets.Where(x => x.DryLoadingTimeSheetId == aId).ToList();
                }
                if (!string.IsNullOrWhiteSpace(businessUnitId))
                {
                    dryLoadingTimesheets = dryLoadingTimesheets.Where(x => x.BusinessUnitId == int.Parse(businessUnitId)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    Guid.TryParse(categoryId, out Guid id);
                    List<DryLoadingTimeSheet> dryLoadingList = new List<DryLoadingTimeSheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var dryLoadingFiltered = dryLoadingTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        dryLoadingList.AddRange(dryLoadingFiltered);
                    }
                    dryLoadingTimesheets = dryLoadingList;
                }
                if (!string.IsNullOrWhiteSpace(subCategoryId))
                {
                    Guid.TryParse(subCategoryId, out Guid id);
                    List<DryLoadingTimeSheet> dryLoadingList = new List<DryLoadingTimeSheet>();
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var dryLoadingFiltered = dryLoadingTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        dryLoadingList.AddRange(dryLoadingFiltered);
                    }
                    dryLoadingTimesheets = dryLoadingList;
                }
                if (!string.IsNullOrWhiteSpace(projectId))
                {
                    int.TryParse(projectId, out int branchId);
                    dryLoadingTimesheets = dryLoadingTimesheets.Where(x => x.BranchId == branchId).ToList();
                }
                foreach (var dryLoadingTimesheet in dryLoadingTimesheets)
                {
                    var item = new DryLoadingTimesheetViewModel();
                    var fleet = db.Fleets.Find(dryLoadingTimesheet.FleetsID);
                    var equipmentId = fleet != null ? fleet.EquipmentTypeID.ToString() : "0";
                    var equipment = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID.ToString() == equipmentId);
                    //if (equipment != null && fleet != null)
                    //{
                    //    item.EquipmentName = equipment.EquipmentTypeName;
                    //}
                    item.Sn = dryLoadingTimesheet.DryLoadingTimeSheetId;
                    item.WorkType = dryLoadingTimesheet.WorkItem;
                    item.DeparturePlace = dryLoadingTimesheet.TripStartPlace;
                    item.ArrivalPlace = dryLoadingTimesheet.TriplArrivalPlace;
                    item.StartingDate = dryLoadingTimesheet.TripStartDate;
                    item.EndDate = dryLoadingTimesheet.TripArrivalDate;
                    item.Category = dryLoadingTimesheet.Category;
                    item.NoOfTripe = dryLoadingTimesheet.NoOfTripe;
                    item.AsphaltTraveled = dryLoadingTimesheet.Asphalt;
                    item.GravelTraveled = dryLoadingTimesheet.Gravel;
                    item.AsphaltRate = dryLoadingTimesheet.AsphaltRate;
                    item.GravelRate = dryLoadingTimesheet.GravelRate;
                    item.LoadedType = dryLoadingTimesheet.LoadedType;
                    //Calculated
                    item.TotalRevenue = item.NoOfTripe * (item.AsphaltTraveled * item.AsphaltRate + item.GravelRate * item.GravelTraveled);
                    item.FuelInLiter = dryLoadingTimesheet.FuelInLiter;
                    item.FuelRate = dryLoadingTimesheet.FuelRate;
                    // Calculated
                    item.FuelCost = item.FuelInLiter * item.FuelRate;
                    item.LubricantCost = dryLoadingTimesheet.LubricantCost;
                    item.MaintenaceCost = dryLoadingTimesheet.MaintenanceCost;
                    item.TireCost = dryLoadingTimesheet.TireCost;
                    item.LoadingUnloadingCost = dryLoadingTimesheet.LoadingUnloadingCost;
                    item.Perdium = dryLoadingTimesheet.PerDiem;
                    item.Salary = dryLoadingTimesheet.Salary;
                    //Calculated
                    item.TotalCost = item.Salary + item.Perdium + item.LoadingUnloadingCost + item.TireCost + item.MaintenaceCost + item.LubricantCost + item.FuelCost;
                    item.NetIncome = item.TotalRevenue - item.TotalCost;
                    list.Add(item);
                }
                return list;
            }
            return list;
        }
    }
    [DataObject]
    public class PlaceODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DryLoadVM> GetPlace()
        {
            var Placelists = new List<DryLoadVM>();
            var places = db.DryLoadingTimeSheets.ToList();
            foreach (var place in places)
            {
                var itemList = new DryLoadVM();
                itemList.Id = place.DryLoadingTimeSheetId;
                itemList.TripStartPlace = place.TripStartPlace;
                itemList.TriplArrivalPlace = place.TriplArrivalPlace;
                Placelists.Add(itemList);
            }
            return Placelists;
        }
    }
    public class DryLoadVM
    {
        public int Id { get; set; }
        public string TripStartPlace { get; set; }
        public string TriplArrivalPlace { get; set; }
    }

}
