using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetTireManagement
{
 public interface ITireReturnService
    {

        bool Insert(TireReturn entity);
        bool InsertBatch(List<TireReturn> entityList);
        bool Update(TireReturn entity);

        bool Delete(TireReturn entity);
        TireReturn GetById(Guid id);
        List<TireReturn> GetAll();
    }
}
