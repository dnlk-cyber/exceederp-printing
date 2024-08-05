using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class DryLoadingTimeSheetOnlyOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<DryLoadingTimeSheetDTO> DryLoadingTimeSheet(string fleetId, string startPlace, string ArrivalPlace, DateTime from, DateTime to, string businessUnitId, string categoryId, string subCategoryId, string project)
        {
            List<DryLoadingTimeSheetDTO> list = new List<DryLoadingTimeSheetDTO>();
            if (fleetId != null)
            {

                Guid FId = Guid.Parse(fleetId);
                var dryLoadingTimesheets = db.DryLoadingTimeSheets.Where(x => x.FleetsID == FId && x.TimeSheetApproved).ToList();

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
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {
                        var dryLoadingFiltered = dryLoadingTimesheets.Where(x => x.FleetsID == item.FleetsID).ToList();
                        dryLoadingList.AddRange(dryLoadingFiltered);
                    }
                    dryLoadingTimesheets = dryLoadingList;
                }
                if (!string.IsNullOrWhiteSpace(project))
                {
                    int.TryParse(project, out int branchId);
                    dryLoadingTimesheets = dryLoadingTimesheets.Where(x => x.BranchId == branchId).ToList();

                }
                foreach (var item in dryLoadingTimesheets)
                {
                    var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID);
                    DryLoadingTimeSheetDTO dryLoadingTimeSheet = new DryLoadingTimeSheetDTO
                    {
                        PlateNo = fleet?.PlateNumber,
                        CategoryType = item.Category,
                        FuelInLiter = item.FuelInLiter,
                        IdleHour = item.IdleHour,
                        LoadedType = item.LoadedType,
                        //Quantity = item.,
                        ReasonForIdle = item.ReasonForIdle,
                        //RoadType = item.,
                        TotalKmTravelled = item.TotalKmTravelled,
                        TripArrivalDate = item.TripArrivalDate,
                        TripArrivalPlace = item.TriplArrivalPlace,
                        TripStartDate = item.TripStartDate,
                        TripStartPlace = item.TripStartPlace,
                        WorkingHour = item.WorkingHour,
                        WorkItem = item.WorkItem,
                        AsphaltKm = item.Asphalt,
                        GravelKm = item.Gravel

                    };
                    list.Add(dryLoadingTimeSheet);
                }

                return list;
            }
            return list;
        }

    }
    public class DryLoadingTimeSheetDTO
    {
        public string PlateNo { get; set; }
        public DateTime TripStartDate { get; set; }
        public DateTime TripArrivalDate { get; set; }
        public string TripStartPlace { get; set; }
        public decimal TotalKmTravelled { get; set; }
        public string TripArrivalPlace { get; set; }
        public string WorkItem { get; set; }
        public decimal FuelInLiter { get; set; }
        public string ReasonForIdle { get; set; }
        public decimal WorkingHour { get; set; }
        public decimal IdleHour { get; set; }
        public decimal RoadType { get; set; }
        public FleetMesuringCategory CategoryType { get; set; }
        public decimal Quantity { get; set; }
        public string LoadedType { get; set; }
        public decimal AsphaltKm { get; set; }
        public decimal GravelKm { get; set; }


    }
}
