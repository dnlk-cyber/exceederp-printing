using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public class EquipmentIdentityService:IEquipmentIdentityService
    {
       private readonly UnitOfWork _uow;
        public EquipmentIdentityService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(EquipmentIdentity entity)
        {
            _uow.EquipmentIdentityRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<EquipmentIdentity> entityList)
        {
            _uow.EquipmentIdentityRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool AddOrUpdate(EquipmentIdentity entity)
        {
            _uow.EquipmentIdentityRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(EquipmentIdentity entity)
        {
            _uow.EquipmentIdentityRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public EquipmentIdentity GetById(Guid id)
        {
         EquipmentIdentity EquipmentIdentity =   _uow.EquipmentIdentityRepository.FindById(id);
         return EquipmentIdentity;
        }

        public List<EquipmentIdentity> GetAll()
        {
         List<EquipmentIdentity> EquipmentIdentity=   _uow.EquipmentIdentityRepository.GetAll();
         return EquipmentIdentity;
          
        }
    }
}

