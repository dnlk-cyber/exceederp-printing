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
   public class ServiceCenterSelectorODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ServiceCenterSelectorVM> GetServiceCenter(string id)
        {
            List<ServiceCenterSelectorVM> list = new List<ServiceCenterSelectorVM>();
            if(!Guid.TryParse(id,out Guid MCId))
            {
                return list;
            }
            var FetchServiceCenter = db.ServiceCenters.Where(x => x.MaintenanceCenterId == MCId).ToList();
            foreach(var serviceCenter in FetchServiceCenter)
            {
                var Center = new ServiceCenterSelectorVM();
                Center.ServiceCenterId = serviceCenter.ServiceCenterId.ToString();
                Center.ServiceCenterName = serviceCenter.Name;
                list.Add(Center);
            }
            return list;
        }
    }

    public class ServiceCenterSelectorVM
    {
        public string ServiceCenterId { get; set; }
        public string ServiceCenterName { get; set; }
    }
}
