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

    public class DailyJobRegistrationReportODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<DailyJobRegistrationVM> getDailyJobRegistration(string MCId, string ServiceCenterId, DateTime From, DateTime To)
        {
            var list = new List<DailyJobRegistrationVM>();

            var FetchJobs = db.ServiceCenterJob.ToList();
            FetchJobs= FetchJobs.Where(x => x.OnlineAssignedTime.HasValue &&
                            x.OnlineAssignedTime.Value.Date >= From.Date &&
                            x.OnlineAssignedTime.Value.Date <= To.Date).ToList();

            if (!string.IsNullOrEmpty(MCId))
            {
                FetchJobs = FetchJobs.Where(x => x.MaintenanceCenterId.ToString() == MCId).ToList();
            }

            if (!string.IsNullOrEmpty(ServiceCenterId))
            {
                FetchJobs = FetchJobs.Where(x => x.ServiceCenterId.ToString() == ServiceCenterId).ToList();
            }
            //var FetchJobs = db.ServiceCenterJob.AsNoTracking()
            //    .Where(x => x.OnlineAssignedTime.HasValue &&
            //                x.OnlineAssignedTime.Value.Date >= From.Date &&
            //                x.OnlineAssignedTime.Value.Date <= To.Date);

            //if (!string.IsNullOrEmpty(MCId))
            //{
            //    FetchJobs = FetchJobs.Where(x => x.MaintenanceCenterId.ToString() == MCId);
            //}

            //if (!string.IsNullOrEmpty(ServiceCenterId))
            //{
            //    FetchJobs = FetchJobs.Where(x => x.ServiceCenterId.ToString() == ServiceCenterId);
            //}

            var jobList = FetchJobs.ToList();

            foreach (var job in FetchJobs)
            {
                var dailyJob = new DailyJobRegistrationVM
                {
                    EquipmentType = job.EquipmentType,
                    PlateNumber = job.LicensePlate,
                    STDTime = 0,
                    ActualTime = 0
                };

                // Collect Repair Types
                var FetchJobDetail = db.JobOrderServiceCenterDetails
                    .Where(x => x.ServiceCenterJobId == job.ServiceCenterJobId)
                    .ToList();

                var repairNames = FetchJobDetail
                    .Select(detail => db.RepairType.FirstOrDefault(r => r.RepairTypeID == detail.RepairTypeID)?.Name)
                    .Where(name => !string.IsNullOrEmpty(name))
                    .ToList();

                dailyJob.FailureToBeMaintained = string.Join(", ", repairNames);

                // Collect Technician Names
                var techNames = new List<string>();
                var FetchCrews = db.ServiceCrew
                    .Where(x => x.ServiceID == job.ServiceCenterJobId)
                    .ToList();

                foreach (var crew in FetchCrews)
                {
                    if (crew.MechanicCrewID!=null)
                    {
                        var mechanic = db.MechanicCrew.FirstOrDefault(x => x.MechanicCrewID == crew.MechanicCrewID);
                        if (mechanic != null)
                        {
                            var employee = db.Person_Employee.FirstOrDefault(e => e.EmployeeId == mechanic.EmployeeId);
                            if (employee != null && !string.IsNullOrWhiteSpace(employee.FullNameaEnglish))
                            {
                                techNames.Add(employee.FullNameaEnglish);
                            }
                        }
                    }
                }

                dailyJob.TechnicianName = string.Join(", ", techNames);

                // Work Logs
                var FetchWorkLog = db.ServiceCrewWorkLogs
                    .Where(x => x.JobId == job.ServiceCenterJobId)
                    .ToList();

                foreach (var workLog in FetchWorkLog)
                {
                    if (workLog.JobOrderDetailId != null)
                    {
                        dailyJob.STDTime += workLog.StandaredHours;
                        dailyJob.ActualTime += workLog.TotalLabourHours;
                    }

                    var idleReason = db.FollowUp.FirstOrDefault(x => x.ServiceCrewWorkLogId == workLog.JobId);
                    if (idleReason != null && string.IsNullOrWhiteSpace(dailyJob.ReasonOfIdle))
                    {
                        dailyJob.ReasonOfIdle = idleReason.Status;
                    }
                }

                // Idle Time & Status
                if (job.OnlineAssignedTime.HasValue)
                {
                    var assignedTime = job.OnlineAssignedTime.Value;
                    if (job.IsSecondOnlineApproved && job.SecondOnlineApprovedTime.HasValue)
                    {
                        var approvedTime = job.SecondOnlineApprovedTime.Value;
                        TimeSpan idleDuration = approvedTime - assignedTime;
                        dailyJob.IdleTime = $"{idleDuration.TotalHours:F2} hours";
                        dailyJob.Status = "Closed";
                    }
                    else
                    {
                        TimeSpan idleDuration = DateTime.Now - assignedTime;
                        dailyJob.IdleTime = $"{idleDuration.TotalHours:F2} hours";
                        dailyJob.Status = "Open";
                    }
                }

                // Workshop Name
                var center = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == job.ServiceCenterId);
                dailyJob.Workshop = center?.Name ?? "Unknown";

                list.Add(dailyJob);
            }

            return list;
        }

    }

    public class DailyJobRegistrationVM
    {
        public string Workshop { get; set; }
        public string TechnicianName { get; set; }
        public string EquipmentType { get; set; }
        public string PlateNumber { get; set; }
        public string FailureToBeMaintained { get; set; }
        public decimal STDTime { get; set; }
        public decimal ActualTime { get; set; }
        public decimal Performance { get; set; }
        public string IdleTime { get; set; }
        public string ReasonOfIdle { get; set; }
        public string Status { get; set; }
    }
}
