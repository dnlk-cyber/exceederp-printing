using ExceedERP.Core.Domain.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public interface IServiceService
    {

        bool Insert(Service entity);
        bool InsertBatch(List<Service> entityList);
        bool Update(Service entity);

        bool Delete(Service entity);
        Service GetById(Guid id);
        List<Service> GetAll();
    }
}
