using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent
{
    public class RentFromExternalService : IRentFromExternalService
    {

        
       private readonly UnitOfWork _uow;
        public RentFromExternalService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(RentFromExternal entity)
        {
            _uow.RentFromExternalRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<RentFromExternal> entityList)
        {
            _uow.RentFromExternalRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(RentFromExternal entity)
        {
            _uow.RentFromExternalRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(RentFromExternal entity)
        {
            _uow.RentFromExternalRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public RentFromExternal GetById(Guid id)
        {
         RentFromExternal RentFromExternal =   _uow.RentFromExternalRepository.FindById(id);
         return RentFromExternal;
        }

        public List<RentFromExternal> GetAll()
        {
         List<RentFromExternal> RentFromExternal=   _uow.RentFromExternalRepository.GetAll();
         return RentFromExternal;
          
        }
    }
}