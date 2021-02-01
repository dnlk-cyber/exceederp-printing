using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
    public class StructureService : IStructureService
    {
  


       

         private readonly UnitOfWork _uow;
         public StructureService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(Structure entity)
        {
            _uow.StructureRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<Structure> entityList)
        {
            _uow.StructureRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(Structure entity)
        {
            _uow.StructureRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(Structure entity)
        {
            _uow.StructureRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public Structure GetById(Guid id)
        {
         Structure Structure =   _uow.StructureRepository.FindById(id);
         return Structure;
        }

        public List<Structure> GetAll()
        {
         List<Structure> Structure=   _uow.StructureRepository.GetAll();
         return Structure;
          
        }

      

    }
}