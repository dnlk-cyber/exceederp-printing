using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetSalvageStore
{
  public interface IFleetSalvageStoreService
    {
        bool Insert(SalvageStore entity);
        bool InsertBatch(List<SalvageStore> entityList);
        bool Update(SalvageStore entity);

        bool Delete(SalvageStore entity);
        SalvageStore GetById(Guid id);
        List<SalvageStore> GetAll();
    }
}
