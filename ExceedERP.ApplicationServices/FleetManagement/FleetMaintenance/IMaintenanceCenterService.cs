using ExceedERP.Core.Domain.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
        public interface IMaintenanceCenterService
    {
        bool Insert(MaintenanceCenter entity);
        bool InsertBatch(List<MaintenanceCenter> entityList);
        bool Update(MaintenanceCenter entity);

        bool Delete(MaintenanceCenter entity);
        MaintenanceCenter GetById(Guid id);
        List<MaintenanceCenter> GetAll();
    }
}
