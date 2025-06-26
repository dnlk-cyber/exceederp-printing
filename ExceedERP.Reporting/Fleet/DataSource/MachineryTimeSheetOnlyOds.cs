using ExceedERP.Core.Domain.FleetManagement.FleetTimeSheet;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class MachineryTimeSheetOnlyOds
    {
        ExceedDbContext db = new ExceedDbContext(); 




        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MachineryTimeSheetDTO> MachineryTimeSheetOnly(string fleetId, DateTime from, DateTime to, string project, string businessUnitId, string categoryId, string subCategoryId)
        {
            List<MachineryTimeSheetDTO> list = new List<MachineryTimeSheetDTO>();
            if (fleetId != null)
            {
                var machineryTimeSheetOnly = db.MachineryWorkingTimeSheets.OrderByDescending(x => x.Date).Where(x => x.TimeSheetApproved).ToList();
                Guid.TryParse(fleetId, out Guid FId);
                machineryTimeSheetOnly = machineryTimeSheetOnly.Where(x => x.FleetsID == FId && x.Date.Date >= from.Date && x.Date.Date <= to.Date).ToList();


                if (project != null)
                {
                    int.TryParse(project, out int branchId);
                    machineryTimeSheetOnly = machineryTimeSheetOnly.Where(x => x.BranchId == branchId).ToList();

                }
                if (!string.IsNullOrWhiteSpace(businessUnitId))
                {
                    machineryTimeSheetOnly = machineryTimeSheetOnly.Where(x => x.BusinessUnitId == int.Parse(businessUnitId)).ToList();
                }
                if (!string.IsNullOrWhiteSpace(categoryId))
                {
                    List<MachineryWorkingTimeSheet> machineryList = new List<MachineryWorkingTimeSheet>();
                    Guid.TryParse(categoryId, out Guid id);
                    var fleetFiltered = db.Fleets.Where(x => x.EquipmentIdentityID == id).ToList();
                    foreach (var item in fleetFiltered)
                    {

                        var machineryFiltered = machineryTimeSheetOnly.Where(x => x.FleetsID == item.FleetsID).ToList();
                        machineryList.AddRange(machineryFiltered);

                    }
                    machineryTimeSheetOnly = machineryList;

                }
                if (!string.IsNullOrWhiteSpace(subCategoryId))
                {
                    List<MachineryWorkingTimeSheet> machineryList = new List<MachineryWorkingTimeSheet>();
                    Guid.TryParse(subCategoryId, out Guid id);
                    var fleets = db.Fleets.Where(x => x.EquipmentTypeID == id).ToList();
                    foreach (var item in fleets)
                    {
                        var machineryFiltered = machineryTimeSheetOnly.Where(x => x.FleetsID == item.FleetsID).ToList();
                        machineryList.AddRange(machineryFiltered);
                    }
                    machineryTimeSheetOnly = machineryList;
                }
                foreach (var item in machineryTimeSheetOnly)
                {
                    var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == item.FleetsID);
                    var morningTime = (item.MorningTimeEnd - item.MorningTimeStart).TotalMinutes;
                    var afternoonTime = (item.AfternoonTimeEnd - item.AfternoonTimeStart).TotalMinutes;
                    var nightTime = (item.NightTimeEnd - item.NightTimeStart).TotalMinutes;
                    var totalMinutes = morningTime + afternoonTime + nightTime;
                    double totalHour = totalMinutes / 60;
                    totalHour = Math.Round(totalHour, 2);

                    MachineryTimeSheetDTO machineryTimeSheetDTO = new MachineryTimeSheetDTO
                    {
                        SerialNumber = fleet?.SerialNo,
                        AfternoonTimeEnd = item.AfternoonTimeEnd,
                        AfternoonTimeStart = item.AfternoonTimeStart,
                        Date = item.Date.Date,
                        DownHours = item.DownHours,
                        DownReason = item.DownReason,
                        FuelConsumption = item.FuelConsumption,
                        GreaseConsumption = item.GreaseConsumption,//////
                        IdleHours = item.IdleHours,
                        IdleReason = item.IdleReason,
                        MorningTimeEnd = item.MorningTimeEnd,
                        MorningTimeStart = item.MorningTimeStart,
                        NightTimeEnd = item.NightTimeEnd,
                        NightTimeStart = item.NightTimeStart,
                        TotalWorkingHour = totalHour,
                        Remark = item.Remark,
                        WorkType = item.WorkType
                    };

                    list.Add(machineryTimeSheetDTO);

                }

                return list.OrderBy(x => x.Date.Date).ToList();
            }
            return list;
        }
    }
    public class MachineryTimeSheetDTO
    {
        public string SerialNumber { get; set; }
        public DateTime Date { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime MorningTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime MorningTimeEnd { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime AfternoonTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime AfternoonTimeEnd { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime NightTimeStart { get; set; }
        [DataType(DataType.Time)]
        [DisplayFormat(DataFormatString = "{0:HH:mm}", ApplyFormatInEditMode = true)]
        public DateTime NightTimeEnd { get; set; }
        public decimal HoleDepth { get; set; }
        public decimal GreaseConsumption { get; set; }
        public decimal FuelConsumption { get; set; }
        public decimal DownHours { get; set; }
        public string IdleReason { get; set; }
        public string WorkType { get; set; }
        
        public decimal IdleHours { get; set; }
        public string DownReason { get; set; }
        public string Remark { get; set; }
        public double TotalWorkingHour { get; set; }

    }
}
