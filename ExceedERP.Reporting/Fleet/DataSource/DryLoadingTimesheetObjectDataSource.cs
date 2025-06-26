using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
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
                List<DryLoadingTimeSheet> PreparedBy = db.DryLoadingTimeSheets.Where(x => x.FleetsID == FId && DbFunctions.TruncateTime(x.TripStartDate) >= from.Date && DbFunctions.TruncateTime(x.TripStartDate) <= to.Date).ToList();
                DryLoadingTimeSheet LastDryLoadingtimeSheet = PreparedBy.LastOrDefault();
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
                    header.PreparedBy = LastDryLoadingtimeSheet.CreatedBy;
                    if (LastDryLoadingtimeSheet.CreatedBy != null)
                    {
                        header.PreparedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(LastDryLoadingtimeSheet.CreatedBy));
                    }
                    if (LastDryLoadingtimeSheet.CostSheetSendForApprovalBy != null)
                    {
                        header.CheckedBY = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(LastDryLoadingtimeSheet.CostSheetSendForApprovalBy));
                    }
                    header.CheckedDate = LastDryLoadingtimeSheet.CostSheetSendForApprovalTime?.ToString("yyyy-MM-dd")??"";
                    if (LastDryLoadingtimeSheet.CostSheetApprovedBy != null)
                    {
                        header.ApprovedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(LastDryLoadingtimeSheet.CostSheetApprovedBy));
                    }
                    header.PreparedDate = LastDryLoadingtimeSheet.DateCreated?.ToString("yyyy-MM-dd")??"";
                    header.ApprovedDate = LastDryLoadingtimeSheet.CostSheetApprovedDate?.ToString("yyyy-MM-dd")??"";
                    if (LastDryLoadingtimeSheet.ModifiedBy != null)
                    {
                        header.LastModifiedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(LastDryLoadingtimeSheet.ModifiedBy));
                    }
                    header.ModifiedDate = LastDryLoadingtimeSheet.LastModified?.ToString("yyyy-MM-dd")??"";
                    if (LastDryLoadingtimeSheet.TimeSheetApprovedBy != null)
                    {
                        header.TimesheetApporvedBy = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(LastDryLoadingtimeSheet.TimeSheetApprovedBy));
                    }
                    header.TimesheetApporvedDate = LastDryLoadingtimeSheet.TimeSheetApprovedDate?.ToString("yyyy-MM-dd")??"";
                    if (LastDryLoadingtimeSheet.TimeSheetSendForApprovalBy != null)
                    {
                        header.TimesheetSendForApprovalBY = EmployeeHelper.GetEmployeeEnglishName(UserHelper.GetUserEmployeeId(LastDryLoadingtimeSheet.TimeSheetSendForApprovalBy));
                    }
                    header.TimesheetSendForApprovalDate = LastDryLoadingtimeSheet.CostSheetSendForApprovalTime?.ToString("yyyy-MM-dd")??"";

    }
            }

            return header;
        }
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DryLoadingTimesheetViewModel> DryLoadingTimesheet(
    string fleetId, string startPlace, string IntExt, string ArrivalPlace,
    DateTime from, DateTime to, string businessUnitId,
    string categoryId, string subCategoryId, string projectId)
{
            var fromDate = from.Date;
            var toDate = to.Date.AddDays(1); // Make upper bound exclusive
            var list = new List<DryLoadingTimesheetViewModel>();

    if (string.IsNullOrWhiteSpace(fleetId) || !Guid.TryParse(fleetId, out Guid FId))
        return list;

    var query = db.DryLoadingTimeSheets
                  .AsNoTracking()
                  .Where(x => x.FleetsID == FId && x.CostSheetApproved);

   // query = query.Where(x => x.TripStartDate.Date >= from.Date && x.TripStartDate.Date <= to.Date);
    query = query.Where(x => x.TripStartDate >= fromDate && x.TripStartDate < toDate);
            if (!string.IsNullOrWhiteSpace(startPlace))
        query = query.Where(x => x.TripStartPlace == startPlace);

    if (int.TryParse(ArrivalPlace, out int arrivalId))
        query = query.Where(x => x.DryLoadingTimeSheetId == arrivalId);

    if (!string.IsNullOrWhiteSpace(businessUnitId) && int.TryParse(businessUnitId, out int buId))
        query = query.Where(x => x.BusinessUnitId == buId);

    if (!string.IsNullOrWhiteSpace(IntExt))
    {
        var external = IntExt == "Rent";
        var externalFleetIds = db.Fleets
                                 .AsNoTracking()
                                 .Where(f => f.IsExternal == external)
                                 .Select(f => f.FleetsID);
        query = query.Where(x => externalFleetIds.Contains(x.FleetsID));
    }

    if (!string.IsNullOrWhiteSpace(categoryId) && Guid.TryParse(categoryId, out Guid catId))
    {
        var catFleetIds = db.Fleets
                            .AsNoTracking()
                            .Where(f => f.EquipmentIdentityID == catId)
                            .Select(f => f.FleetsID);
        query = query.Where(x => catFleetIds.Contains(x.FleetsID));
    }

    if (!string.IsNullOrWhiteSpace(subCategoryId) && Guid.TryParse(subCategoryId, out Guid subCatId))
    {
        var subCatFleetIds = db.Fleets
                               .AsNoTracking()
                               .Where(f => f.EquipmentIdentityID == subCatId)
                               .Select(f => f.FleetsID);
        query = query.Where(x => subCatFleetIds.Contains(x.FleetsID));
    }

    // Project / Branch Filter
    if (!string.IsNullOrWhiteSpace(projectId) && int.TryParse(projectId, out int projectBranchId))
        query = query.Where(x => x.BranchId == projectBranchId);

    // Execute query
    var dryLoadingTimesheets = query.ToList();
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
                    item.PreparedBy = dryLoadingTimesheet.CreatedBy;
                    item.Name = dryLoadingTimesheet.CreatedBy;
                    item.PreparedDate = dryLoadingTimesheet.DateCreated;
                    item.ApprovedBy = dryLoadingTimesheet.ModifiedBy;
                    item.ApprovedDate = dryLoadingTimesheet.LastModified;
                    item.ApproverName = dryLoadingTimesheet.ModifiedBy;
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
                    item.Perdium = dryLoadingTimesheet.Perdium;
                    item.Salary = dryLoadingTimesheet.Salary;
                    //Calculated
                    item.TotalCost = item.Salary + item.Perdium + item.LoadingUnloadingCost + item.TireCost + item.MaintenaceCost + item.LubricantCost + item.FuelCost;
                    item.NetIncome = item.TotalRevenue - item.TotalCost;
                    list.Add(item);
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
