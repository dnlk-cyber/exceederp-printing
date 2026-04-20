using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Maintenance.DataSource
{
    [DataObject]
    public class StandardTimeVsActualTime
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<StandardTimeVsActualTimeVM> GetStandardTimeVsActualTime(string MCId, string ShopId,string MechanicId,DateTime From,DateTime To)
        {
            List<StandardTimeVsActualTimeVM> list = new List<StandardTimeVsActualTimeVM>();
            decimal TActual = 0;
            decimal TStandard = 0;
            decimal TPerformance = 0;
            if (!(Guid.TryParse(MCId, out Guid MaintenanceCId)))
            {
                return list;
            }
            if(MechanicId==null&&ShopId==null)
            {
                return list;
            }
           var FetchServiceCenterJob = db.ServiceCenterJob.Where(x => x.MaintenanceCenterId == MaintenanceCId&& x.IsSubmittedToServiceCenter == true && x.IsExternalMaintenanceCenter == false).ToList();
            if (FetchServiceCenterJob.Any())
            {
             FetchServiceCenterJob = FetchServiceCenterJob.Where(x=> x.SecondOnlineApprovedTime.HasValue && x.SecondOnlineApprovedTime.Value.Date >= From.Date && x.SecondOnlineApprovedTime.Value.Date <= To.Date).ToList();
            }
            if (ShopId!=null)
            {
                FetchServiceCenterJob = FetchServiceCenterJob.Where(x => x.ServiceCenterId.ToString()== ShopId).ToList();
                foreach (var ServiceJob in FetchServiceCenterJob)
                {
                    var WorkSheets = db.ServiceCrewWorkLogs.Where(x => x.JobId == ServiceJob.ServiceCenterJobId).ToList();
                        if(MechanicId!=null)
                    {
                        WorkSheets = WorkSheets.Where(x => x.MecanicId.ToString() == MechanicId).ToList();
                    }
                    decimal maxHour = WorkSheets.Any()? WorkSheets.Max(x => x.TotalLabourHours): 0;
                    foreach (var FetchWorkSheet in WorkSheets)
                    {
                        var fetchService = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == ServiceJob.ServiceCenterJobId);
                        var StandardTimeVsActualTime = new StandardTimeVsActualTimeVM();
                        StandardTimeVsActualTime.ActualTime = FetchWorkSheet.TotalLabourHours;
                        StandardTimeVsActualTime.StandardTime = db.RepairType.FirstOrDefault(x => x.RepairTypeID == fetchService.RepairTypeID).StandaredTime;
                        StandardTimeVsActualTime.Performance = 0;
                        if (maxHour != 0)
                        {
                            StandardTimeVsActualTime.Performance = Math.Round((StandardTimeVsActualTime.StandardTime / maxHour) * 100, 2);
                        }
                        StandardTimeVsActualTime.DateIn = ServiceJob.OnlineAssignedTime?.ToString("yyyy-MM-dd");
                        var dateOut = db.JobOrder.FirstOrDefault(x => x.JobOrderID == ServiceJob.JobOrderID);
                        if(dateOut!=null)
                        {
                            StandardTimeVsActualTime.DateOut = ServiceJob.SecondOnlineApprovedTime?.ToString("yyyy-MM-dd");
                        }
                        StandardTimeVsActualTime.JobService = "";
                        var FetchServiceCenterDetail = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == ServiceJob.ServiceCenterJobId);
                        if(FetchServiceCenterDetail!=null)
                        {
                            var FetchServiceName = db.RepairType.FirstOrDefault(x => x.RepairTypeID == FetchServiceCenterDetail.RepairTypeID);
                            if(FetchServiceName!=null)
                            {
                                StandardTimeVsActualTime.JobService = FetchServiceName.Name;
                            }
                        }
                        
                        StandardTimeVsActualTime.EquipmentName = ServiceJob.EquipmentName;
                        StandardTimeVsActualTime.PlateNumber = ServiceJob.LicensePlate;
                        var FetchServiceCenter = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == ServiceJob.ServiceCenterId);
                        if(FetchServiceCenter!=null)
                        {
                            StandardTimeVsActualTime.WorkShop = FetchServiceCenter.Name;
                        }
                        //StandardTimeVsActualTime.WorkShop = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == ServiceJob.ServiceCenterId).Name;
                        var FetchMechanicCrews = db.MechanicCrew.FirstOrDefault(x => x.MechanicCrewID == FetchWorkSheet.MecanicId);
                        if(FetchMechanicCrews!=null)
                        {
                            var FetchEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == FetchMechanicCrews.EmployeeId);
                            if(FetchEmployee!=null)
                            {
                                StandardTimeVsActualTime.AssignedTechnician = FetchEmployee.FullNameaEnglish;
                            }
                           
                        }
                        list.Add(StandardTimeVsActualTime);
                    }
                }

            }
            else
            {

                foreach (var ServiceJob in FetchServiceCenterJob)
                {
                    var WorkSheets = db.ServiceCrewWorkLogs.Where(x => x.JobId == ServiceJob.ServiceCenterJobId&&x.MecanicId.ToString()== MechanicId).ToList();
                    foreach(var FetchWorkSheet in WorkSheets)
                    {
                        var StandardTimeVsActualTime = new StandardTimeVsActualTimeVM();
                        StandardTimeVsActualTime.ActualTime = FetchWorkSheet.TotalLabourHours;
                        var fetchService = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == ServiceJob.ServiceCenterJobId);
                        StandardTimeVsActualTime.StandardTime = db.RepairType.FirstOrDefault(x => x.RepairTypeID == fetchService.RepairTypeID).StandaredTime;
                        if (FetchWorkSheet.TotalLabourHours != 0)
                        {
                            StandardTimeVsActualTime.Performance = Math.Round((StandardTimeVsActualTime.StandardTime / FetchWorkSheet.TotalLabourHours) * 100, 2);
                        }
                        StandardTimeVsActualTime.DateIn =ServiceJob.OnlineAssignedTime?.ToString("yyyy-MM-dd");
                        var dateOut = db.JobOrder.FirstOrDefault(x => x.JobOrderID == ServiceJob.JobOrderID);
                        if (dateOut != null)
                        {
                            StandardTimeVsActualTime.DateOut = ServiceJob.SecondOnlineApprovedTime?.ToString("yyyy-MM-dd");
                        }
                        StandardTimeVsActualTime.EquipmentName = ServiceJob.EquipmentName;
                        var FetchFollowUp = db.FollowUp.FirstOrDefault(x => x.ServiceCrewWorkLogId == FetchWorkSheet.ServiceCrewWorkLogId);
                        if(FetchFollowUp!=null)
                        {
                            StandardTimeVsActualTime.ReasonForDelay = FetchFollowUp.Status;
                        }
                       
                        StandardTimeVsActualTime.JobService = "";
                        var FetchServiceCenterDetail = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == ServiceJob.ServiceCenterJobId);
                        if (FetchServiceCenterDetail != null)
                        {
                            var FetchServiceName = db.RepairType.FirstOrDefault(x => x.RepairTypeID == FetchServiceCenterDetail.RepairTypeID);
                            if (FetchServiceName != null)
                            {
                                StandardTimeVsActualTime.JobService = FetchServiceName.Name;
                            }
                        }
                        StandardTimeVsActualTime.PlateNumber = ServiceJob.LicensePlate;
                        var FetchServiceCenter = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == ServiceJob.ServiceCenterId);
                        if (FetchServiceCenter != null)
                        {
                            StandardTimeVsActualTime.WorkShop = FetchServiceCenter.Name;
                        }
                        StandardTimeVsActualTime.AssignedTechnician = "";
                        var FetchCrewInfo = db.MechanicCrew.FirstOrDefault(x => x.MechanicCrewID == FetchWorkSheet.MecanicId);
                        if(FetchCrewInfo!=null)
                        {
                            var FetchFromEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == FetchCrewInfo.EmployeeId);
                            if(FetchFromEmployee!=null)
                            {
                                StandardTimeVsActualTime.AssignedTechnician = FetchFromEmployee.FullNameaEnglish;
                            }
                        }
                        
                        list.Add(StandardTimeVsActualTime);
                    }
                }
                 
            }
           
                return list;
            }
           

        }
    

    public class StandardTimeVsActualTimeVM
    {
        
       public string EquipmentName { get; set; }
       public string PlateNumber { get; set; }
       public string JobService { get; set; }
       public string WorkShop { get; set; }
       public string AssignedTechnician { get; set; }
       public string DateIn { get; set; }
       public string DateOut { get; set; }       
       public string ReasonForDelay { get; set; }       
       public decimal StandardTime { get; set; }
       public decimal ActualTime { get; set; }
       public decimal Performance { get; set; }
       public decimal TotalActual { get; set; }
       public decimal TotalStandard { get; set; }
        public decimal TotalPerformance { get; set; }


    }
}
