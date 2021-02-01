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
    public class BranchService : IBranchService
    {
 
         private readonly UnitOfWork _uow;
         public BranchService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(Branch entity)
        {
            _uow.BranchRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<Branch> entityList)
        {
            _uow.BranchRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(Branch entity)
        {
            _uow.BranchRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(Branch entity)
        {
            _uow.BranchRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public Branch GetById(Guid id)
        {
         Branch Branch =   _uow.BranchRepository.FindById(id);
         return Branch;
        }

        public List<Branch> GetAll()
        {
         List<Branch> Branch=   _uow.BranchRepository.GetAll();
         return Branch;
          
        }
    }
}