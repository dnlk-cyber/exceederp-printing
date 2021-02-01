using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetTireManagement
{
     public class TireReturnService : ITireReturnService
    {

         
        
       private readonly UnitOfWork _uow;
        public TireReturnService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(TireReturn entity)
        {
            _uow.TireReturnRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<TireReturn> entityList)
        {
            _uow.TireReturnRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(TireReturn entity)
        {
            _uow.TireReturnRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(TireReturn entity)
        {
            _uow.TireReturnRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public TireReturn GetById(Guid id)
        {
         TireReturn TireReturn =   _uow.TireReturnRepository.FindById(id);
         return TireReturn;
        }

        public List<TireReturn> GetAll()
        {
         List<TireReturn> TireReturn=   _uow.TireReturnRepository.GetAll();
         return TireReturn;
          
        }
    }
}
