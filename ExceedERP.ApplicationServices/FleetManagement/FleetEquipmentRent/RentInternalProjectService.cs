using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent
{
   public class RentInternalProjectService :IRentInternalProjectService
    {


        private readonly UnitOfWork _uow;
        public RentInternalProjectService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(RentInternalProject entity)
        {
            _uow.RentInternalProjectRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<RentInternalProject> entityList)
        {
            _uow.RentInternalProjectRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(RentInternalProject entity)
        {
            _uow.RentInternalProjectRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(RentInternalProject entity)
        {
            _uow.RentInternalProjectRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public RentInternalProject GetById(Guid id)
        {
         RentInternalProject RentInternalProject =   _uow.RentInternalProjectRepository.FindById(id);
         return RentInternalProject;
        }

        public List<RentInternalProject> GetAll()
        {
         List<RentInternalProject> RentInternalProject=   _uow.RentInternalProjectRepository.GetAll();
         return RentInternalProject;
          
        }
    }
}
