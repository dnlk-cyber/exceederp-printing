using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
            public interface IRepairTypeService
    {
                bool Insert(RepairType entity);
        bool InsertBatch(List<RepairType> entityList);
        bool AddOrUpdate(RepairType entity);

        bool Delete(RepairType entity);
        RepairType GetById(Guid id);
        List<RepairType> GetAll();
    }
}
