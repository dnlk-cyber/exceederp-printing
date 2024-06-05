using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public class ServiceToolService : IServiceToolService
    {
   


        
         private readonly UnitOfWork _uow;
        public ServiceToolService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(ServiceTool entity)
        {
            _uow.ServiceToolRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<ServiceTool> entityList)
        {
            _uow.ServiceToolRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(ServiceTool entity)
        {
            _uow.ServiceToolRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(ServiceTool entity)
        {
            _uow.ServiceToolRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public ServiceTool GetById(Guid id)
        {
         ServiceTool ServiceTool =   _uow.ServiceToolRepository.FindById(id);
         return ServiceTool;
        }

        public List<ServiceTool> GetAll()
        {
         List<ServiceTool> ServiceTool=   _uow.ServiceToolRepository.GetAll();
         return ServiceTool;
          
        }
    }
}
