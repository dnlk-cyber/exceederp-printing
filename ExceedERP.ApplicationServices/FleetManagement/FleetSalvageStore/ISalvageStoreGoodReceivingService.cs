using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetSalvageStore
{
        public interface ISalvageStoreGoodReceivingService
    {
        bool Insert(SalvageStoreGoodReceiving entity);
        bool InsertBatch(List<SalvageStoreGoodReceiving> entityList);
        bool Update(SalvageStoreGoodReceiving entity);

        bool Delete(SalvageStoreGoodReceiving entity);
        SalvageStoreGoodReceiving GetById(Guid id);
        List<SalvageStoreGoodReceiving> GetAll();
    }
}
