using ExceedERP.Core.Domain.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public interface IPlannerService
    {
        bool Insert(Planner entity);
        bool InsertBatch(List<Planner> entityList);
        bool Update(Planner entity);

        bool Delete(Planner entity);
        Planner GetById(Guid id);
        List<Planner> GetAll();
    }
}
