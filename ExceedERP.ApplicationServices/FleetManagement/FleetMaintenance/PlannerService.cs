using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public class PlannerService : IPlannerService
    {
 
    
       
       private readonly UnitOfWork _uow;
        public PlannerService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(Planner entity)
        {
            _uow.PlannerRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<Planner> entityList)
        {
            _uow.PlannerRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(Planner entity)
        {
            _uow.PlannerRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(Planner entity)
        {
            _uow.PlannerRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public Planner GetById(Guid id)
        {
         Planner Planner =   _uow.PlannerRepository.FindById(id);
         return Planner;
        }

        public List<Planner> GetAll()
        {
         List<Planner> Planner=   _uow.PlannerRepository.GetAll();
         return Planner;
          
        }
    }
}

