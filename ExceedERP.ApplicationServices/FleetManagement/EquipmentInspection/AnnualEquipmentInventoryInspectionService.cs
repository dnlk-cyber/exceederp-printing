
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.EquipmentInspection
{
  public  class AnnualEquipmentInventoryInspectionService :IAnnualEquipmentInventoryInspectionService
    {

           private readonly UnitOfWork _uow;
        public AnnualEquipmentInventoryInspectionService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(AnnualEquipmentInventoryInspection entity)
        {
            _uow.AnnualEquipmentInventoryInspectionRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<AnnualEquipmentInventoryInspection> entityList)
        {
            _uow.AnnualEquipmentInventoryInspectionRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(AnnualEquipmentInventoryInspection entity)
        {
            _uow.AnnualEquipmentInventoryInspectionRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(AnnualEquipmentInventoryInspection entity)
        {
            _uow.AnnualEquipmentInventoryInspectionRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public AnnualEquipmentInventoryInspection GetById(Guid id)
        {
         AnnualEquipmentInventoryInspection AnnualEquipmentInventoryInspection =   _uow.AnnualEquipmentInventoryInspectionRepository.FindById(id);
         return AnnualEquipmentInventoryInspection;
        }

        public List<AnnualEquipmentInventoryInspection> GetAll()
        {
         List<AnnualEquipmentInventoryInspection> AnnualEquipmentInventoryInspection=   _uow.AnnualEquipmentInventoryInspectionRepository.GetAll();
         return AnnualEquipmentInventoryInspection;
          
        }
    }
}