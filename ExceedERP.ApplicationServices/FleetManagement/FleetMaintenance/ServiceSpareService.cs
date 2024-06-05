using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public class ServiceSpareService : IServiceSpareService
    {
  

        
         private readonly UnitOfWork _uow;
        public ServiceSpareService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(ServiceSpare entity)
        {
            _uow.ServiceSpareRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<ServiceSpare> entityList)
        {
            _uow.ServiceSpareRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(ServiceSpare entity)
        {
            _uow.ServiceSpareRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(ServiceSpare entity)
        {
            _uow.ServiceSpareRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public ServiceSpare GetById(Guid id)
        {
         ServiceSpare ServiceSpare =   _uow.ServiceSpareRepository.FindById(id);
         return ServiceSpare;
        }

        public List<ServiceSpare> GetAll()
        {
         List<ServiceSpare> ServiceSpare=   _uow.ServiceSpareRepository.GetAll();
         return ServiceSpare;
          
        }
    }
}
