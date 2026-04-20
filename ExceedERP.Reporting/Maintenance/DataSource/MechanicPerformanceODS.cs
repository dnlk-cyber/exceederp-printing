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
   public class MechanicPerformanceODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<MechanicDto> GetAssignedMechanics(DateTime From,DateTime To,string selector,string MCId,string CrewId,string ServiceCenterId)
        {
            List<MechanicDto> list = new List<MechanicDto>();
            if (selector == "Technician")
            {
                var FetchServiceCrewWorkLogs = db.ServiceCrewWorkLogs.ToList();
                if(CrewId!=null)
                {
                    FetchServiceCrewWorkLogs = FetchServiceCrewWorkLogs.Where(x => x.MecanicId.ToString() == CrewId).ToList();
                }
                if (FetchServiceCrewWorkLogs.Any())
                {
                    var groupedCrewLogs = FetchServiceCrewWorkLogs.GroupBy(x => x.MecanicId);
                    foreach (var group in groupedCrewLogs)
                    {
                        int LogCount = 0;
                        decimal averageStandardTime = 0;
                        MechanicDto MechanicList = new MechanicDto();
                        var FirstCrewLog = group.First();
                        var FetchParentService = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId == FirstCrewLog.JobId);
                        var count = 0;
                        List<JobOrderDTO> JOlists = new List<JobOrderDTO>();
                        foreach (var crewLog in group)
                        {
                            var FetchService = db.ServiceCenterJob.FirstOrDefault(x => x.ServiceCenterJobId == crewLog.JobId && x.MaintenanceCenterId.ToString() == MCId);

                            if (FetchService != null)
                            {

                                var fetchJobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == FetchService.JobOrderID);

                                if (fetchJobOrder != null && fetchJobOrder.SecondOnlineApprovedTime.HasValue && fetchJobOrder.SecondOnlineApprovedTime.Value.Date >= From.Date && fetchJobOrder.SecondOnlineApprovedTime.Value.Date <= To.Date)
                                {
                                    decimal averageStandarTimePerLog = 0;
                                    if (crewLog.TotalLabourHours>0)
                                    {
                                        var fetchServiceCenterDetail = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == FetchService.ServiceCenterJobId);
                                        var fetchStandardTime = db.RepairType.FirstOrDefault(x => x.RepairTypeID == fetchServiceCenterDetail.RepairTypeID).StandaredTime;
                                            averageStandarTimePerLog = Math.Round((fetchStandardTime / crewLog.TotalLabourHours), 2);
                                    }
                                    LogCount += 1;
                                    averageStandardTime += averageStandarTimePerLog;
                                    var JOlist = new JobOrderDTO();
                                    JOlist.JobOrderId = fetchJobOrder.JobOrderID.ToString();

                                    var OIDList = JOlists.Select(x => x.JobOrderId).ToList();
                                    if (!OIDList.Contains(fetchJobOrder.JobOrderID.ToString()))
                                    {
                                        MechanicList.JobCompletedQuantity += 1;
                                    }
                                    var FetchServiceDetail = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == FetchService.ServiceCenterJobId);
                                    if (FetchServiceDetail != null)
                                    {
                                        //var FetchWeight = db.RepairType.FirstOrDefault(x => x.RepairTypeID == FetchServiceDetail.RepairTypeID);
                                        var FetchWeight = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == FetchService.ServiceCenterId);
                                        if (FetchWeight != null)
                                        {
                                            MechanicList.Weight += FetchWeight.Weight;
                                        }
                                    }
                                    JOlists.Add(JOlist);
                                }
                            }
                        }

                        count = 0;
                        if (MechanicList.JobCompletedQuantity > 0)
                        {
                            var FetchCrew = db.MechanicCrew.SingleOrDefault(x => x.MechanicCrewID == FirstCrewLog.MecanicId);
                            if (FetchCrew != null)
                            {
                                var FetchEmployee = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == FetchCrew.EmployeeId);
                                if (FetchEmployee != null)
                                {
                                    MechanicList.FullName = FetchEmployee.FullNameaEnglish;
                                }
                            }
                            if(LogCount>0)
                            {
                                MechanicList.StandardToActualTimeRatio = Math.Round((averageStandardTime / LogCount), 2);
                            }
                            MechanicList.DateFrom = From.ToString("yyyy-MM-dd");
                            MechanicList.DateTo = To.ToString("yyyy-MM-dd");
                            MechanicList.CompletionEfeciency = MechanicList.Weight;
                            if(MechanicList.JobCompletedQuantity>0)
                            {
                                MechanicList.Weight = Math.Round((MechanicList.Weight / MechanicList.JobCompletedQuantity), 2);
                            }
                            list.Add(MechanicList);
                        }

                    }
                }
            }
            else
            {
                var FetchServiceCenterJobs = db.ServiceCenterJob.Where(x => x.MaintenanceCenterId.ToString() == MCId).ToList();
                if(ServiceCenterId!=null)
                {
                    FetchServiceCenterJobs = FetchServiceCenterJobs.Where(x => x.ServiceCenterId.ToString() == ServiceCenterId.ToString()).ToList();
                }
                if(FetchServiceCenterJobs.Any())
                {
                    var GroupedData = FetchServiceCenterJobs.GroupBy(x => x.ServiceCenterId);
                    foreach(var group in GroupedData)
                    {
                        int LogCount = 0;
                        decimal averageStandardTime = 0;
                        var FirstServiceCenter = group.First();
                        MechanicDto MechanicList = new MechanicDto();
                        foreach (var ServiceLog in group)
                        {
                                var fetchJobOrder = db.JobOrder.FirstOrDefault(x => x.JobOrderID == ServiceLog.JobOrderID);

                            if (fetchJobOrder != null && fetchJobOrder.SecondOnlineApprovedTime.HasValue && fetchJobOrder.SecondOnlineApprovedTime.Value.Date >= From.Date && fetchJobOrder.SecondOnlineApprovedTime.Value.Date <= To.Date)
                            {
                                var FetchCrewLogs = db.ServiceCrewWorkLogs.Where(x => x.JobId == ServiceLog.ServiceCenterJobId).ToList();
                                var groupedCrew = FetchCrewLogs.GroupBy(x => x.MecanicId);
                                decimal TotalAveragCrewLogTime = 0;
                                decimal crewCount = 0;
                                foreach (var groupedcrew in groupedCrew)
                                {
                                    decimal xyz = 0;
                                    xyz = groupedcrew.Sum(x => x.TotalLabourHours);
                                    decimal averageStandarTimePerLog = 0;
                                    var fetchServiceCenterDetail = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == ServiceLog.ServiceCenterJobId);
                                    var fetchStandardTime = db.RepairType.FirstOrDefault(x => x.RepairTypeID == fetchServiceCenterDetail.RepairTypeID).StandaredTime;
                                    if (xyz > 0)
                                    {
                                            averageStandarTimePerLog += Math.Round((fetchStandardTime / xyz), 2);

                                        }
                                    TotalAveragCrewLogTime += averageStandarTimePerLog;
                                    crewCount += 1;
                                }
                                LogCount += 1;
                                if (crewCount > 0)
                                {
                                    averageStandardTime += Math.Round(TotalAveragCrewLogTime / crewCount, 2);
                                }
                                MechanicList.JobCompletedQuantity += 1;
                                var FetchServiceDetail = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == ServiceLog.ServiceCenterJobId);
                                if (FetchServiceDetail != null)
                                {
                                   // var FetchWeight = db.RepairType.FirstOrDefault(x => x.RepairTypeID == FetchServiceDetail.RepairTypeID);
                                    var FetchWeight = db.ServiceCenters.FirstOrDefault(x => x.ServiceCenterId == ServiceLog.ServiceCenterId);
                                    if (FetchWeight != null)
                                    {
                                        MechanicList.Weight += FetchWeight.Weight;
                                    }
                                }
                            }


                                }
                        if (MechanicList.JobCompletedQuantity > 0)
                        {
                            var FetchCrew = db.ServiceCenters.SingleOrDefault(x => x.ServiceCenterId == FirstServiceCenter.ServiceCenterId);
                            if (FetchCrew != null)
                            {
                               
                                    MechanicList.FullName = FetchCrew.Name;
                                
                            }
                            if(LogCount>0)
                            {
                                MechanicList.StandardToActualTimeRatio = Math.Round((averageStandardTime / LogCount), 2);
                            }
                           
                            MechanicList.DateFrom = From.ToString("yyyy-MM-dd");
                            MechanicList.DateTo = To.ToString("yyyy-MM-dd");
                            MechanicList.CompletionEfeciency = MechanicList.Weight;
                            if(MechanicList.JobCompletedQuantity>0)
                            {
                                MechanicList.Weight = Math.Round((MechanicList.Weight / MechanicList.JobCompletedQuantity), 2);
                            }
                            list.Add(MechanicList);
                        }
                    }
                }
            }
            if(selector == "Technician")
            {
                list = list.OrderByDescending(m => m.StandardToActualTimeRatio).ToList();
                int rank = 1;
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > 0 && list[i].StandardToActualTimeRatio == list[i - 1].StandardToActualTimeRatio)
                    {
                        list[i].Rank = list[i - 1].Rank; // Same rank for equal CompletionEfeciency
                    }
                    else
                    {
                        list[i].Rank = rank;
                    }
                    rank++;
                }

            }
            else
            {
                list = list.OrderByDescending(m => m.CompletionEfeciency).ToList();
                int rank = 1;
                for (int i = 0; i < list.Count; i++)
                {
                    if (i > 0 && list[i].CompletionEfeciency == list[i - 1].CompletionEfeciency)
                    {
                        list[i].Rank = list[i - 1].Rank; // Same rank for equal CompletionEfeciency
                    }
                    else
                    {
                        list[i].Rank = rank;
                    }
                    rank++;
                }

            }
            // Step 3: Assign Rank

            return list;
        }


    }
    public class JobOrderDTO
    {
        public string JobOrderId { get; set; }
    }

    public class MechanicDto
    {
        public string FullName { get; set; }
        public int AssignedCount { get; set; }
        public int JobCompletedQuantity { get; set; }
        public decimal Weight { get; set; }
        public string DateFrom { get; set; }
        public string DateTo { get; set; }
        public decimal CompletionEfeciency {get;set;}
        public decimal StandardToActualTimeRatio { get; set; }

        public int Rank { get; set; }
        
    }
}
