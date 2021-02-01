
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
        public interface IManufacturerService
    {
        bool Insert(Manufacturer entity);
        bool InsertBatch(List<Manufacturer> entityList);
        bool AddOrUpdate(Manufacturer entity);

        bool Delete(Manufacturer entity);
        Manufacturer GetById(Guid id);
        List<Manufacturer> GetAll();
    }
}
