using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
   public interface IJobOrderService
    {

        bool Insert(JobOrder entity);
        bool InsertBatch(List<JobOrder> entityList);
        bool Update(JobOrder entity);

        bool Delete(JobOrder entity);
        JobOrder GetById(Guid id);
        List<JobOrder> GetAll();
    }
}
