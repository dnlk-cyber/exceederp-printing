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
    public class WagonDrillTimeSheetOnlyOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<WagonDrillTimeSheetDTO> WagonDrillTimeSheet(string fleetId, DateTime from, DateTime to, string businessUnitId, string categoryId, string subCategoryId)
        {
            List<WagonDrillTimeSheetDTO> list = new List<WagonDrillTimeSheetDTO>();
            if (fleetId != null)
            {

                Guid FId = Guid.Parse(fleetId);
                var wagonDrillTimesheets = db.WagonDrillTimesheets.Where(x => x.FleetsID == FId).ToList();


                wagonDrillTimesheets = wagonDrillTimesheets.Where(x => x.Date.Date >= from.Date && x.Date.Date <= to.Date).ToList();


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
                foreach (var item in wagonDrillTimesheets)
                {
                    var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID);
                    WagonDrillTimeSheetDTO wagonDrillTimeSheetDto = new WagonDrillTimeSheetDTO
                    {

                        SerialNo = fleet?.SerialNo,
                        Date = item.Date,
                        DownTime = item.DownTime,
                        DrilledDepthOfHole = item.OperationDrilledHoleDepth,
                        FuelInLiter = item.FuelInLiter,
                        IdleAmount = item.IdleAmount,
                        IdleHour = item.IdleHour,
                        IdleRateHour = item.IdleRateHour,
                        NetWorkingHour = item.OperationNetWorkingHour,
                        NoOfHolesDrilled = item.NumberOfHolesDrilled,
                        OperationAmount = item.OperationAmount,
                        OperationRateForMeter = item.OperationRateForMeter,
                        ServiceCharge = item.ServiceCharge,
                        WorkItem = item.WorkItem

                    };
                    list.Add(wagonDrillTimeSheetDto);
                }
                return list.OrderBy(x => x.Date.Date).ToList();
            }
            return list;
        }

    }
    public class WagonDrillTimeSheetDTO
    {
        public string SerialNo { get; set; }
        public decimal NetWorkingHour { get; set; }
        public decimal OperationRateForMeter { get; set; }
        public decimal NoOfHolesDrilled { get; set; }
        public decimal FuelInLiter { get; set; }
        public decimal ServiceCharge { get; set; }
        public decimal IdleRateHour { get; set; }
        public DateTime Date { get; set; }
        public decimal DrilledDepthOfHole { get; set; }
        public decimal IdleAmount { get; set; }
        public string WorkItem { get; set; }
        public decimal IdleHour { get; set; }
        public decimal DownTime { get; set; }
        public decimal OperationAmount { get; set; }

    }
}
