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
   public class ExternalMaintenanceCenterODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]       

        public List<ExternalMaintenanceCenterVM> GetMaintenanceCenters()
        {
            List<ExternalMaintenanceCenterVM> list = new List<ExternalMaintenanceCenterVM>();
            var getList = db.MaintenanceCenter.Where(x=>x.IsExternal==true).ToList();
           foreach(var mc in getList)
            {
                var maintenaceCenter = new ExternalMaintenanceCenterVM();
                maintenaceCenter.MaintenanceCenterId = mc.MaintenanceCenterID.ToString();
                maintenaceCenter.MaintenanceCenter = mc.Name;
                list.Add(maintenaceCenter);

            }
            return list;
        }
        
    }

    public class ExternalMaintenanceCenterVM
    {
        public string MaintenanceCenterId { get; set; }
        public string MaintenanceCenter { get; set; }
    }
}
