using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public interface IOperatorService
    {
        bool Insert(Operator entity);
        bool InsertBatch(List<Operator> entityList);
        bool Update(Operator entity);
        bool Edit(Operator entity);
        bool Delete(Operator entity);
        Operator GetById(Guid id);
        List<Operator> GetAll();
        bool EditNoTracking(Operator entity);
    }
}
