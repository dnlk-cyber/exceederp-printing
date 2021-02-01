using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
        public interface IVehicleReturnService
    {
        bool Insert(VehicleReturn entity);
        bool InsertBatch(List<VehicleReturn> entityList);
        bool Update(VehicleReturn entity);

        bool Delete(VehicleReturn entity);
        VehicleReturn GetById(Guid id);
        List<VehicleReturn> GetAll();
    }
}
