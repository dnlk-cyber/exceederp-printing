using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FuelManagement
{
        public interface IFuelService
    {
        bool Insert(Fuel entity);
        bool InsertBatch(List<Fuel> entityList);
        bool Update(Fuel entity);

        bool Delete(Fuel entity);
        Fuel GetById(Guid id);
        List<Fuel> GetAll();
    }
}
