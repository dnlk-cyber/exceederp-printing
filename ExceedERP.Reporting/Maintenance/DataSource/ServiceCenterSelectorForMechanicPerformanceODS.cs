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
   public class ServiceCenterSelectorForMechanicPerformanceODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ServiceCenterSelectorVM> GetServiceCenter()
        {
            List<ServiceCenterSelectorVM> list = new List<ServiceCenterSelectorVM>();
           
            var FetchServiceCenter = db.ServiceCenters.ToList();
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

   
}
