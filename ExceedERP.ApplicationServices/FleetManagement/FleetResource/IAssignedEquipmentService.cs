using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
            public interface IAssignedEquipmentService
    {

                bool Insert(AssignedEquipment entity);
        bool InsertBatch(List<AssignedEquipment> entityList);
        bool Update(AssignedEquipment entity);

        bool Delete(AssignedEquipment entity);
        AssignedEquipment GetById(Guid id);
        List<AssignedEquipment> GetAll();
                
    }
}
