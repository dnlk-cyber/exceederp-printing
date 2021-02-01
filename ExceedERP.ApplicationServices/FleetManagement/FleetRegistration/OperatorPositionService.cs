using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
         public class OperatorPositionService : IOperatorPositionService
    {
                    
        
       private readonly UnitOfWork _uow;
        public OperatorPositionService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(OperatorPosition entity)
        {
            _uow.OperatorPositionRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<OperatorPosition> entityList)
        {
            _uow.OperatorPositionRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(OperatorPosition entity)
        {
            _uow.OperatorPositionRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(OperatorPosition entity)
        {
            _uow.OperatorPositionRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public OperatorPosition GetById(Guid id)
        {
         OperatorPosition OperatorPosition =   _uow.OperatorPositionRepository.FindById(id);
         return OperatorPosition;
        }

        public List<OperatorPosition> GetAll()
        {
         List<OperatorPosition> OperatorPosition=   _uow.OperatorPositionRepository.GetAll();
         return OperatorPosition;
          
        }


        public bool AddOrUpdate(OperatorPosition entity)
        {
            _uow.OperatorPositionRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }
    }
}

    

