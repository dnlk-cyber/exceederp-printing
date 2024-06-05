using ExceedERP.Core.Domain.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
   public interface IServiceCenterJobService
    {

        bool Insert(ServiceCenterJob entity);
        bool InsertBatch(List<ServiceCenterJob> entityList);
        bool Update(ServiceCenterJob entity);

        bool Delete(ServiceCenterJob entity);
        ServiceCenterJob GetById(Guid id);
        List<ServiceCenterJob> GetAll();
    }
}
