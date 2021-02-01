using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public interface IInsuranceService
    {

        bool Insert(FleetInsurance entity);
        bool InsertBatch(List<FleetInsurance> entityList);
        bool Update(FleetInsurance entity);

        bool Delete(FleetInsurance entity);
        FleetInsurance GetById(Guid id);
        List<FleetInsurance> GetAll();
    }
}