using ExceedERP.Core.Domain.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public interface IServiceCrewService
    {

           bool Insert(ServiceCrew entity);
           bool InsertBatch(List<ServiceCrew> entityList);
           bool Update(ServiceCrew entity);

           bool Delete(ServiceCrew entity);
           ServiceCrew GetById(Guid id);
           List<ServiceCrew> GetAll();
    }
}
