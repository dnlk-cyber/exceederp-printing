using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
        public interface IEquipmentTypeService
    {
        bool Insert(EquipmentType entity);
        bool InsertBatch(List<EquipmentType> entityList);
        bool Update(EquipmentType entity);
        bool AddOrUpdate(EquipmentType entity);
        bool Delete(EquipmentType entity);
        EquipmentType GetById(Guid id);
        List<EquipmentType> GetAll();
    }
}
