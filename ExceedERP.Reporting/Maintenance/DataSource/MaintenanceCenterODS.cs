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
   public class MaintenanceCenterODS
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]       

        public List<MaintenanceCenterVM> GetMaintenanceCenters()
        {
            List<MaintenanceCenterVM> list = new List<MaintenanceCenterVM>();
            var getList = db.MaintenanceCenter.ToList();
           foreach(var mc in getList)
            {
                var maintenaceCenter = new MaintenanceCenterVM();
                maintenaceCenter.MaintenanceCenterId = mc.MaintenanceCenterID.ToString();
                maintenaceCenter.MaintenanceCenter = mc.Name;
                list.Add(maintenaceCenter);

            }
            return list;
        }
        
    }

    public class MaintenanceCenterVM
    {
        public string MaintenanceCenterId { get; set; }
        public string MaintenanceCenter { get; set; }
    }
}
