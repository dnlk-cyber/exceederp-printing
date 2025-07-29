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
    public class JobServicesSelector
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]

        public List<JobServicesVM> GetServices(string LicensePlate, DateTime ClosedDate)
        {
            List<JobServicesVM> list = new List<JobServicesVM>();
            var FetchJobOrder = db.JobOrder.FirstOrDefault(x => x.LicensePlate == LicensePlate && DbFunctions.TruncateTime(x.SecondOnlineApprovedTime).Value == ClosedDate.Date&&x.Status==MaintenanceConst.Delivered);
            if(FetchJobOrder!=null)
            {
                var FetchJobServices = db.ServiceCenterJob.Where(x => x.JobOrderID == FetchJobOrder.JobOrderID).ToList();
                foreach(var JobService in FetchJobServices)
                {
                    var FetchService = db.JobOrderServiceCenterDetails.FirstOrDefault(x => x.ServiceCenterJobId == JobService.ServiceCenterJobId);
                    if(FetchService!=null)
                    {
                        var JobServiceInstance = new JobServicesVM();
                        JobServiceInstance.ServiceId = FetchService.JobOrderServiceCenterDetailId.ToString();
                        JobServiceInstance.JobName = FetchService.ServiceName;
                        list.Add(JobServiceInstance);
                    }
                }
            }
            return list;
        }

    }
    public class JobServicesVM
    {
        public string ServiceId { get; set; }
        public string JobName { get; set; }
    }
}
