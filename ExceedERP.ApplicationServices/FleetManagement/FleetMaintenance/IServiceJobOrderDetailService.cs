using ExceedERP.Core.Domain.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
   public interface IServiceJobOrderDetailService
    {

        bool Insert(JobOrderServiceCenterDetail entity);
        bool InsertBatch(List<JobOrderServiceCenterDetail> entityList);
        bool Update(JobOrderServiceCenterDetail entity);

        bool Delete(JobOrderServiceCenterDetail entity);
        JobOrderServiceCenterDetail GetById(Guid id);
        List<JobOrderServiceCenterDetail> GetAll();
    }
}
