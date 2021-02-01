using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public class FleetHistoryService : IFleetHistoryService
    { 
        private readonly UnitOfWork _uow;
        public FleetHistoryService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(FleetHistory entity)
        {
            _uow.FleetHistoryRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<FleetHistory> entityList)
        {
            _uow.FleetHistoryRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(FleetHistory entity)
        {
            _uow.FleetHistoryRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(FleetHistory entity)
        {
            _uow.FleetHistoryRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public FleetHistory GetById(Guid id)
        {
         FleetHistory FleetHistory =   _uow.FleetHistoryRepository.FindById(id);
         return FleetHistory;
        }

        public List<FleetHistory> GetAll()
        {
         List<FleetHistory> FleetHistory=   _uow.FleetHistoryRepository.GetAll();
         return FleetHistory;
          
        }
    }
}

