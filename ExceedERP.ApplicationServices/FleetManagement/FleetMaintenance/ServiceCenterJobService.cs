using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
 public  class ServiceCenterJobService : IServiceCenterJobService
    {
        
       private readonly UnitOfWork _uow;
        public ServiceCenterJobService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(ServiceCenterJob entity)
        {
            _uow.ServiceCenterJobRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<ServiceCenterJob> entityList)
        {
            _uow.ServiceCenterJobRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(ServiceCenterJob entity)
        {
            _uow.ServiceCenterJobRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(ServiceCenterJob entity)
        {
            _uow.ServiceCenterJobRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public ServiceCenterJob GetById(Guid id)
        {
            ServiceCenterJob JobOrder =   _uow.ServiceCenterJobRepository.FindById(id);
         return JobOrder;
        }

        public List<ServiceCenterJob> GetAll()
        {
         List<ServiceCenterJob> JobOrder=   _uow.ServiceCenterJobRepository.GetAll();
         return JobOrder;
          
        }
    }
}
