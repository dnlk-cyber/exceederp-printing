using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public class OperatorService : IOperatorService
    {
                
        private readonly UnitOfWork _uow;
        public OperatorService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(Operator entity)
        {
            _uow.OperatorRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<Operator> entityList)
        {
            _uow.OperatorRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(Operator entity)
        {
            _uow.OperatorRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(Operator entity)
        {
            _uow.OperatorRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public Operator GetById(Guid id)
        {
            Operator Operator = _uow.OperatorRepository.FindById(id);
            return Operator;
        }

        public List<Operator> GetAll()
        {
            List<Operator> Operator = _uow.OperatorRepository.GetAll();
            return Operator;

        }


        public bool Edit(Operator entity)
        {
       
            _uow.OperatorRepository.Edit(entity);
            return _uow.SaveChanges();
        
        }
        public bool EditNoTracking(Operator entity)
        {
            _uow.OperatorRepository.EditNoTracking(entity);
            return _uow.SaveChanges();
        }
    }
}