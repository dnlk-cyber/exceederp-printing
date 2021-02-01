using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
  public interface IMechanicCrewService
    {
        bool Insert(MechanicCrew entity);
        bool InsertBatch(List<MechanicCrew> entityList);
        bool Update(MechanicCrew entity);

        bool Delete(MechanicCrew entity);
        MechanicCrew GetById(Guid id);
        List<MechanicCrew> GetAll();
    }
}
