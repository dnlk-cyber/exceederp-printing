using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.EquipmentPlanandEvaluation
{
        public  class EquipmentPlanandEvaluationsService : IEquipmentPlanandEvaluationsService
    {

      private readonly UnitOfWork _uow;
        public EquipmentPlanandEvaluationsService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(EquipmentPlanandEvaluations entity)
        {
            _uow.EquipmentPlanandEvaluationsRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<EquipmentPlanandEvaluations> entityList)
        {
            _uow.EquipmentPlanandEvaluationsRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(EquipmentPlanandEvaluations entity)
        {
            _uow.EquipmentPlanandEvaluationsRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(EquipmentPlanandEvaluations entity)
        {
            _uow.EquipmentPlanandEvaluationsRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public EquipmentPlanandEvaluations GetById(Guid id)
        {
         EquipmentPlanandEvaluations EquipmentPlanandEvaluations =   _uow.EquipmentPlanandEvaluationsRepository.FindById(id);
         return EquipmentPlanandEvaluations;
        }

        public List<EquipmentPlanandEvaluations> GetAll()
        {
         List<EquipmentPlanandEvaluations> EquipmentPlanandEvaluations=   _uow.EquipmentPlanandEvaluationsRepository.GetAll();
         return EquipmentPlanandEvaluations;
          
        }
    }
}
