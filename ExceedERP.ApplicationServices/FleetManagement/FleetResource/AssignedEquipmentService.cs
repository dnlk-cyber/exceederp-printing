using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Repository;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
    public class AssignedEquipmentService : IAssignedEquipmentService
    {




        

         private readonly UnitOfWork _uow;
         public AssignedEquipmentService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(AssignedEquipment entity)
        {
            _uow.AssignedEquipmentRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<AssignedEquipment> entityList)
        {
            _uow.AssignedEquipmentRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(AssignedEquipment entity)
        {
            _uow.AssignedEquipmentRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(AssignedEquipment entity)
        {
            _uow.AssignedEquipmentRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public AssignedEquipment GetById(Guid id)
        {
         AssignedEquipment AssignedEquipment =   _uow.AssignedEquipmentRepository.FindById(id);
         return AssignedEquipment;
        }

        public List<AssignedEquipment> GetAll()
        {
         List<AssignedEquipment> AssignedEquipment=   _uow.AssignedEquipmentRepository.GetAll();
         return AssignedEquipment;
          
        }
    }
}