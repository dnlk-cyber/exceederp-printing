using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
         public interface IOperatorPositionService
    {
        bool Insert(OperatorPosition entity);
        bool InsertBatch(List<OperatorPosition> entityList);
        bool AddOrUpdate(OperatorPosition entity);

        bool Delete(OperatorPosition entity);
        OperatorPosition GetById(Guid id);
        List<OperatorPosition> GetAll();

    }
}
