using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetSalvageStore
{
    public class CannibalizationService : ICannibalizationService
    {
 
         private readonly UnitOfWork _uow;
         public CannibalizationService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(Cannibalization entity)
        {
            _uow.CannibalizationRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<Cannibalization> entityList)
        {
            _uow.CannibalizationRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(Cannibalization entity)
        {
            _uow.CannibalizationRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(Cannibalization entity)
        {
            _uow.CannibalizationRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public Cannibalization GetById(Guid id)
        {
         Cannibalization Cannibalization =   _uow.CannibalizationRepository.FindById(id);
         return Cannibalization;
        }

        public List<Cannibalization> GetAll()
        {
         List<Cannibalization> Cannibalization=   _uow.CannibalizationRepository.GetAll();
         return Cannibalization;
          
        }
    }
}