using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public class ServiceService :IServiceService
    {
    
       private readonly UnitOfWork _uow;
        public ServiceService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(Service entity)
        {
            _uow.ServiceRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<Service> entityList)
        {
            _uow.ServiceRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(Service entity)
        {
            _uow.ServiceRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(Service entity)
        {
            _uow.ServiceRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public Service GetById(Guid id)
        {
         Service Service =   _uow.ServiceRepository.FindById(id);
         return Service;
        }

        public List<Service> GetAll()
        {
         List<Service> Service=   _uow.ServiceRepository.GetAll();
         return Service;
          
        }
    }
}
