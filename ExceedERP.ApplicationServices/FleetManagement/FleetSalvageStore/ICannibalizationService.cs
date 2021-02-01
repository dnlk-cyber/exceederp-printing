using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetSalvageStore
{
    public  interface ICannibalizationService
    {
        bool Insert(Cannibalization entity);
        bool InsertBatch(List<Cannibalization> entityList);
        bool Update(Cannibalization entity);

        bool Delete(Cannibalization entity);
        Cannibalization GetById(Guid id);
        List<Cannibalization> GetAll();
    }
}
