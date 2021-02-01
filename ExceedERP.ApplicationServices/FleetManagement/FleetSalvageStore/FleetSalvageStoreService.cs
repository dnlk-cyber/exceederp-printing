using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetSalvageStore
{
        public   class FleetSalvageStoreService : IFleetSalvageStoreService
    {


         private readonly UnitOfWork _uow;
         public FleetSalvageStoreService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(SalvageStore entity)
        {
            _uow.SalvageStoreRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<SalvageStore> entityList)
        {
            _uow.SalvageStoreRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(SalvageStore entity)
        {
            _uow.SalvageStoreRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(SalvageStore entity)
        {
            _uow.SalvageStoreRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public SalvageStore GetById(Guid id)
        {
         SalvageStore SalvageStore =   _uow.SalvageStoreRepository.FindById(id);
         return SalvageStore;
        }

        public List<SalvageStore> GetAll()
        {
         List<SalvageStore> SalvageStore=   _uow.SalvageStoreRepository.GetAll();
         return SalvageStore;
          
        }
    }
}