using ExceedERP.Core.Domain.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
   public interface IJobOrderDetailService
    {

        bool Insert(JobOrderDetail entity);
        bool InsertBatch(List<JobOrderDetail> entityList);
        bool Update(JobOrderDetail entity);

        bool Delete(JobOrderDetail entity);
        JobOrderDetail GetById(Guid id);
        List<JobOrderDetail> GetAll();
    }
}
