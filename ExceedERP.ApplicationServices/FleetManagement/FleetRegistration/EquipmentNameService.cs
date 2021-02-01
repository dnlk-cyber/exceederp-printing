using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public class EquipmentNameService : IEquipmentNameService
    {

  private readonly UnitOfWork _uow;
        public EquipmentNameService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(EquipmentName entity)
        {
            _uow.EquipmentNameRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<EquipmentName> entityList)
        {
            _uow.EquipmentNameRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool AddOrUpdate(EquipmentName entity)
        {
            _uow.EquipmentNameRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(EquipmentName entity)
        {
            _uow.EquipmentNameRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public EquipmentName GetById(Guid id)
        {
         EquipmentName EquipmentName =   _uow.EquipmentNameRepository.FindById(id);
         return EquipmentName;
        }

        public List<EquipmentName> GetAll()
        {
         List<EquipmentName> EquipmentName=   _uow.EquipmentNameRepository.GetAll();
         return EquipmentName;
          
        }


        public bool Update(EquipmentName entity)
        {
            _uow.EquipmentNameRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }
    }
}
