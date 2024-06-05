using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
   public class JobStoreRequisitionService : IJobStoreRequisitionService
    {
        
       
       private readonly UnitOfWork _uow;
        public JobStoreRequisitionService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(JobStoreRequisition entity)
        {
            _uow.JobStoreRequisitionRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<JobStoreRequisition> entityList)
        {
            _uow.JobStoreRequisitionRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(JobStoreRequisition entity)
        {
            _uow.JobStoreRequisitionRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(JobStoreRequisition entity)
        {
            _uow.JobStoreRequisitionRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public JobStoreRequisition GetById(Guid id)
        {
         JobStoreRequisition JobStoreRequisition =   _uow.JobStoreRequisitionRepository.FindById(id);
         return JobStoreRequisition;
        }

        public List<JobStoreRequisition> GetAll()
        {
         List<JobStoreRequisition> JobStoreRequisition=   _uow.JobStoreRequisitionRepository.GetAll();
         return JobStoreRequisition;
          
        }
    }
}

