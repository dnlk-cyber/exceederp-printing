using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public interface IAccidentService
    {
        bool Insert(Accident entity);
        bool InsertBatch(List<Accident> entityList);
        bool Update(Accident entity);

        bool Delete(Accident entity);
        Accident GetById(Guid id);
        List<Accident> GetAll();
    }
}
