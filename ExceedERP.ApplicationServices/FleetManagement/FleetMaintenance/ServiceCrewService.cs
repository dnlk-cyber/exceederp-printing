using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public class ServiceCrewService : IServiceCrewService
    {


         private readonly UnitOfWork _uow;
        public ServiceCrewService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(ServiceCrew entity)
        {
            _uow.ServiceCrewRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<ServiceCrew> entityList)
        {
            _uow.ServiceCrewRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(ServiceCrew entity)
        {
            _uow.ServiceCrewRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(ServiceCrew entity)
        {
            _uow.ServiceCrewRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public ServiceCrew GetById(Guid id)
        {
         ServiceCrew ServiceCrew =   _uow.ServiceCrewRepository.FindById(id);
         return ServiceCrew;
        }

        public List<ServiceCrew> GetAll()
        {
         List<ServiceCrew> ServiceCrew=   _uow.ServiceCrewRepository.GetAll();
         return ServiceCrew;
          
        }
    }
}
