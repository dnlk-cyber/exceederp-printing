using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
   public  class EquipmentTypeService : IEquipmentTypeService
    {

        private readonly UnitOfWork _uow;
        public EquipmentTypeService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(EquipmentType entity)
        {
            _uow.EquipmentTypeRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<EquipmentType> entityList)
        {
            _uow.EquipmentTypeRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(EquipmentType entity)
        {
            _uow.EquipmentTypeRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(EquipmentType entity)
        {
            _uow.EquipmentTypeRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public EquipmentType GetById(Guid id)
        {
         EquipmentType EquipmentType =   _uow.EquipmentTypeRepository.FindById(id);
         return EquipmentType;
        }

        public List<EquipmentType> GetAll()
        {
         List<EquipmentType> EquipmentType=   _uow.EquipmentTypeRepository.GetAll();
         return EquipmentType;
          
        }


        public bool AddOrUpdate(EquipmentType entity)
        {
            _uow.EquipmentTypeRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }
    }
}

