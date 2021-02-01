using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public class FleetService : IFleetService
    {
          private readonly UnitOfWork _uow;
          public FleetService()
        {
            _uow = new UnitOfWork();
        }
          public bool Insert(Core.Domain.FleetManagement.FleetRegistration.Fleets entity)
        {
            _uow.FleetsRepository.Add(entity);
            return _uow.SaveChanges();
        }

          public bool InsertBatch(List<Core.Domain.FleetManagement.FleetRegistration.Fleets> entityList)
        {
            _uow.FleetsRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

          public bool Update(Core.Domain.FleetManagement.FleetRegistration.Fleets entity)
        {
            _uow.FleetsRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(Core.Domain.FleetManagement.FleetRegistration.Fleets entity)
        {
            _uow.FleetsRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public Core.Domain.FleetManagement.FleetRegistration.Fleets GetById(Guid id)
        {
            Core.Domain.FleetManagement.FleetRegistration.Fleets fleets = _uow.FleetsRepository.FindById(id);
            return fleets;
        }

        public List<Core.Domain.FleetManagement.FleetRegistration.Fleets> GetAll()
        {
            List<Core.Domain.FleetManagement.FleetRegistration.Fleets> fleets = _uow.FleetsRepository.GetAll();
            return fleets;
           
        }


        public bool EditNoTracking(Core.Domain.FleetManagement.FleetRegistration.Fleets entity)
        {
            _uow.FleetsRepository.EditNoTracking(entity);
            return _uow.SaveChanges();
        }

        public Task<bool> UpdateAsync(Fleets entity)
        {
            _uow.FleetsRepository.AddOrUpdate(entity);
            return _uow.SaveChangeAsync();
        }
    }
}
