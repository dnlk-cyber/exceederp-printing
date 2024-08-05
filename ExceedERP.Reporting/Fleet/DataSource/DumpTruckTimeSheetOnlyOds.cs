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
    public class DumpTruckTimeSheetOnlyOds
    {
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DumpTruckTimeSheetDTO> DumpTruckTimeSheet(string fleetId, DateTime from, DateTime to, string businessUnitId, string categoryId, string subCategoryId)
        {
            ExceedDbContext db = new ExceedDbContext();
            List<DumpTruckTimeSheetDTO> list = new List<DumpTruckTimeSheetDTO>();
            if (fleetId != null)
            {


                Guid FId = Guid.Parse(fleetId);
                var dumpTruckTimeSheets = db.DumpTruckTimeSheets.Where(x => x.FleetsID == FId && x.TimeSheetApproved).ToList();

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
                foreach (var item in dumpTruckTimeSheets)
                {
                    bool checker = true;
                    var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID);
                    var branch = db.Branch.FirstOrDefault(x => x.BranchId == item.BranchId);
                    var dumpTruckDetails = db.DumpTruckTimeSheetDetails.Where(x => x.DumpTruckTimeSheetId == item.DumpTruckTimeSheetId).ToList();
                    foreach (var detail in dumpTruckDetails)
                    {
                        var dtVm = new DumpTruckTimeSheetDTO();

                        dtVm.PlateNo = fleet?.PlateNumber;
                        dtVm.Branch = branch?.Name;
                        dtVm.ActualDistance = detail.ActualDistance;
                        dtVm.Capacity = detail.Capacity;
                        dtVm.Date = item.Date;




                        dtVm.IdleRentalRate = item.IdealRentalRate;
                        dtVm.LoadingLocationFrom = detail.LoadingLocationFrom;
                        dtVm.LoadingLocationTo = detail.LoadingLocationTo;
                        dtVm.NetWorkingHour = item.NetworkingHour;
                        dtVm.NoOfTrip = detail.NumberOfTrip;
                        dtVm.OilConsumption = item.OilConsumption;
                        dtVm.ReasonForDown = item.ReasonForDown;
                        dtVm.ReasonForIdle = item.ReasonForIdle;
                        dtVm.TotalHaulage = detail.TotalHaulage;
                        dtVm.TypeOfWorkExcuted = detail.TypeOfWork;
                        if (checker)
                        {
                            dtVm.DownHour = item.DownHour;
                            dtVm.FuelConsumption = item.FuelConsumption;
                            dtVm.GreaseConsumption = item.GreaseConsumption;
                            dtVm.IdleHour = item.IdleHour;
                        }
                        else
                        {
                            dtVm.DownHour = 0;
                            dtVm.FuelConsumption = 0;
                            dtVm.GreaseConsumption = 0;
                            dtVm.IdleHour = 0;
                        }
                        list.Add(dtVm);
                        dtVm = null;
                        checker = false;
                    }

                }
                return list.OrderBy(x => x.Date.Date).ToList();
            }
            return list;
        }
    }
    public class DumpTruckTimeSheetDTO
    {
        public string PlateNo { get; set; }
        public string Branch { get; set; }
        public string LoadingLocationFrom { get; set; }
        public decimal ActualDistance { get; set; }
        public decimal FuelConsumption { get; set; }
        public decimal IdleHour { get; set; }
        public string ReasonForDown { get; set; }
        public decimal IdleRentalRate { get; set; }
        public string LoadingLocationTo { get; set; }
        public int NoOfTrip { get; set; }
        public decimal OilConsumption { get; set; }
        public string ReasonForIdle { get; set; }
        public string TypeOfWorkExcuted { get; set; }
        public DateTime Date { get; set; }
        public decimal TotalHaulage { get; set; }
        public decimal GreaseConsumption { get; set; }
        public decimal DownHour { get; set; }
        public decimal Capacity { get; set; }
        public decimal NetWorkingHour { get; set; }

    }
}
