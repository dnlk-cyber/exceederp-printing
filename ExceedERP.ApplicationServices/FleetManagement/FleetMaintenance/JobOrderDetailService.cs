using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public class JobOrderDetailService : IJobOrderDetailService
    {

           
       private readonly UnitOfWork _uow;
        public JobOrderDetailService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(JobOrderDetail entity)
        {
            _uow.JobOrderDetailRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<JobOrderDetail> entityList)
        {
            _uow.JobOrderDetailRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(JobOrderDetail entity)
        {
            _uow.JobOrderDetailRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(JobOrderDetail entity)
        {
            _uow.JobOrderDetailRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public JobOrderDetail GetById(Guid id)
        {
         JobOrderDetail JobOrderDetail =   _uow.JobOrderDetailRepository.FindById(id);
         return JobOrderDetail;
        }

        public List<JobOrderDetail> GetAll()
        {
         List<JobOrderDetail> JobOrderDetail=   _uow.JobOrderDetailRepository.GetAll();
         return JobOrderDetail;
          
        }
    }
}
