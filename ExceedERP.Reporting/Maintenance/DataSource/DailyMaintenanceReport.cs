using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class DailyMaintenanceReport
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DailyMaintenanceReportVM> GetDailyReport(DateTime? from, DateTime? to, string mcenter)
        {
            List<DailyMaintenanceReportVM> list = new List<DailyMaintenanceReportVM>();
            var request = db.JobRequest.ToList();
            var serviceCenter = db.ServiceCenterJob.ToList();
            var jobOrder = db.JobOrder.ToList();

            
            if (from.HasValue)
            {
                jobOrder = jobOrder.Where(x => x.DateIssued.Value.Date >= from.Value.Date).ToList();
            }
            if (to.HasValue)
            {
                jobOrder = jobOrder.Where(x => x.DateIssued.Value.Date <= to.Value.Date).ToList();
            }
            if(Guid.TryParse(mcenter, out Guid id))
            {
                jobOrder = jobOrder.Where(x => x.MaintenanceCenterId == id).ToList();
            }
           
            foreach (var item in jobOrder)
            {
                var jobRequest = db.JobRequest.FirstOrDefault(x => x.Num == item.Referance);
                if(jobRequest != null)
                {
                    var fleet = db.Fleets.FirstOrDefault(x => x.PlateNumber == item.LicensePlate || x.SerialNo == item.LicensePlate);
                    if (fleet != null)
                    {
                        var equipmentCategory = db.EquipmentIdentity.FirstOrDefault(x => x.EquipmentIdentityID == fleet.EquipmentIdentityID);
                        var equipmentType = db.EquipmentType.FirstOrDefault(x => x.EquipmentTypeID == fleet.EquipmentTypeID);

                        var jobCause = db.JobCauses.Where(x => x.OrderId == jobRequest.JobRequestId).ToList();
                        string xyz =null;
                          foreach(var cause in jobCause)
                        {
                            xyz = xyz + " " + cause.Description;
                        }

                        var dailyReport = new DailyMaintenanceReportVM
                        {
                            MVType = equipmentType?.EquipmentTypeName,
                            Category = equipmentCategory?.EquipmentCategoryName,
                            DateIn = item.DateIssued,
                            DateOut = item.DateCompleted,
                            Location = item.MaintenanceCenter,
                            OperatorName = item.OperatorName,
                            PhoneNumber = "",
                            Num = item.Num,
                            PlateNo = item.LicensePlate,
                            Problems = xyz,
                            Remark = item.Remark
                        };
                        list.Add(dailyReport);
                    }
                }
                
                
            }
            return list;
        }
    }


    public class DailyMaintenanceReportVM
    {
        public string MVType { get; set; }
        public string Category { get; set; }
        public string PlateNo { get; set; }
        public DateTime? DateIn { get; set; }
        public DateTime? DateOut { get; set; }
        public string Problems { get; set; }
        public string Location { get; set; }
        public string OperatorName { get; set; }
        public string PhoneNumber { get; set; }
        public string Remark { get; set; }
        public int Num { get; set; }
    }
}
