using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent
{
    public interface IRentOtherSuppliersService
    {
        bool Insert(RentToExternal entity);
        bool InsertBatch(List<RentToExternal> entityList);
        bool Update(RentToExternal entity);

        bool Delete(RentToExternal entity);
        RentToExternal GetById(Guid id);
        List<RentToExternal> GetAll();
    }
}
