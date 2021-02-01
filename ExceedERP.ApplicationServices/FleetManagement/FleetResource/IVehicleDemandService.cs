using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
   public interface IVehicleDemandService
    {
        bool Insert(VehicleDemand entity);
        bool InsertBatch(List<VehicleDemand> entityList);
        bool Update(VehicleDemand entity);

        bool Delete(VehicleDemand entity);
        VehicleDemand GetById(Guid id);
        List<VehicleDemand> GetAll();
    }
}
