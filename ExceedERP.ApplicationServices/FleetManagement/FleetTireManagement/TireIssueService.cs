using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;

using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetTireManagement
{
    public class TireIssueService :ITireIssueService
    {


        
        
       private readonly UnitOfWork _uow;
        public TireIssueService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(TireIssue entity)
        {
            _uow.TireIssueRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<TireIssue> entityList)
        {
            _uow.TireIssueRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(TireIssue entity)
        {
            _uow.TireIssueRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(TireIssue entity)
        {
            _uow.TireIssueRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public TireIssue GetById(Guid id)
        {
         TireIssue TireIssue =   _uow.TireIssueRepository.FindById(id);
         return TireIssue;
        }

        public List<TireIssue> GetAll()
        {
         List<TireIssue> TireIssue=   _uow.TireIssueRepository.GetAll();
         return TireIssue;
          
        }
    }
}
