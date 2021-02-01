using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public interface IFleetService
    {
        bool Insert(Fleets entity);
        bool InsertBatch(List<Fleets> entityList);
        bool Update(Fleets entity);
        Task<bool> UpdateAsync(Fleets entity);
        bool EditNoTracking(Fleets entity);

        bool Delete(Fleets entity);
        Fleets GetById(Guid id);
        List<Fleets> GetAll();
    }
}
