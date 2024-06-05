using ExceedERP.Core.Domain.Maintenance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
     public interface IAdministrativeApprovalService
    {

        bool Insert(AdministrativeJobApproval entity);
        bool InsertBatch(List<AdministrativeJobApproval> entityList);
        bool Update(AdministrativeJobApproval entity);

        bool Delete(AdministrativeJobApproval entity);
        AdministrativeJobApproval GetById(Guid id);
        List<AdministrativeJobApproval> GetAll();
    }
}
