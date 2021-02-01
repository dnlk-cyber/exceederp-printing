using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.HRM;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
   public interface IStructureService
    {
        bool Insert(Structure entity);
        bool InsertBatch(List<Structure> entityList);
        bool Update(Structure entity);

        bool Delete(Structure entity);
        Structure GetById(Guid id);
        List<Structure> GetAll();
    }
}
