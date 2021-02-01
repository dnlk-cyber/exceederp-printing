using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.EquipmentPlanandEvaluation
{
    public  interface IEquipmentPlanandEvaluationsService
    {

        bool Insert(EquipmentPlanandEvaluations entity);
        bool InsertBatch(List<EquipmentPlanandEvaluations> entityList);
        bool Update(EquipmentPlanandEvaluations entity);

        bool Delete(EquipmentPlanandEvaluations entity);
        EquipmentPlanandEvaluations GetById(Guid id);
        List<EquipmentPlanandEvaluations> GetAll();
    }
}
