using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
 public  class JobOrderService :IJobOrderService
    {
        
       private readonly UnitOfWork _uow;
        public JobOrderService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(JobOrder entity)
        {
            _uow.JobOrderRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<JobOrder> entityList)
        {
            _uow.JobOrderRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(JobOrder entity)
        {
            _uow.JobOrderRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(JobOrder entity)
        {
            _uow.JobOrderRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public JobOrder GetById(Guid id)
        {
         JobOrder JobOrder =   _uow.JobOrderRepository.FindById(id);
         return JobOrder;
        }

        public List<JobOrder> GetAll()
        {
         List<JobOrder> JobOrder=   _uow.JobOrderRepository.GetAll();
         return JobOrder;
          
        }
    }
}
