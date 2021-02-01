using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public class RepairTypeService : IRepairTypeService
    {


        
       private readonly UnitOfWork _uow;
        public RepairTypeService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(RepairType entity)
        {
            _uow.RepairTypeRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<RepairType> entityList)
        {
            _uow.RepairTypeRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

      
        public bool Delete(RepairType entity)
        {
            _uow.RepairTypeRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public RepairType GetById(Guid id)
        {
         RepairType RepairType =   _uow.RepairTypeRepository.FindById(id);
         return RepairType;
        }

        public List<RepairType> GetAll()
        {
         List<RepairType> RepairType=   _uow.RepairTypeRepository.GetAll();
         return RepairType;
          
        }


        public bool AddOrUpdate(RepairType entity)
        {
            _uow.RepairTypeRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }
    }
}
