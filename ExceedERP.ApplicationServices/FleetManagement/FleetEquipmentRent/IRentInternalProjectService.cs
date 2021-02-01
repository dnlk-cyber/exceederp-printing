using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent
{
 public interface IRentInternalProjectService
    {

        bool Insert(RentInternalProject entity);
        bool InsertBatch(List<RentInternalProject> entityList);
        bool Update(RentInternalProject entity);

        bool Delete(RentInternalProject entity);
        RentInternalProject GetById(Guid id);
        List<RentInternalProject> GetAll();
    }
}
