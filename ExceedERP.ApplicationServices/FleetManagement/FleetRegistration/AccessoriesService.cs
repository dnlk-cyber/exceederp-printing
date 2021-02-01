
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
   public class AccessoriesService : IAccessoriesService
    {
        private readonly UnitOfWork _uow;
        public AccessoriesService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(Accessories entity)
        {
            _uow.AccessoriesRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<Accessories> entityList)
        {
            _uow.AccessoriesRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(Accessories entity)
        {
            _uow.AccessoriesRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(Accessories entity)
        {
            _uow.AccessoriesRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public Accessories GetById(Guid id)
        {
         Accessories accessories =   _uow.AccessoriesRepository.FindById(id);
         return accessories;
        }

        public List<Accessories> GetAll()
        {
         List<Accessories> accessories=   _uow.AccessoriesRepository.GetAll();
         return accessories;
          
        }
    }
}
