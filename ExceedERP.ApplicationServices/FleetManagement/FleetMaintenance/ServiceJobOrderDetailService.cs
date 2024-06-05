using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public class ServiceJobOrderDetailService : IServiceJobOrderDetailService
    {

           
       private readonly UnitOfWork _uow;
        public ServiceJobOrderDetailService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(JobOrderServiceCenterDetail entity)
        {
            _uow.JobOrderServiceCenterDetailRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<JobOrderServiceCenterDetail> entityList)
        {
            _uow.JobOrderServiceCenterDetailRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(JobOrderServiceCenterDetail entity)
        {
            _uow.JobOrderServiceCenterDetailRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(JobOrderServiceCenterDetail entity)
        {
            _uow.JobOrderServiceCenterDetailRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public JobOrderServiceCenterDetail GetById(Guid id)
        {
            JobOrderServiceCenterDetail JobOrderDetail =   _uow.JobOrderServiceCenterDetailRepository.FindById(id);
         return JobOrderDetail;
        }

        public List<JobOrderServiceCenterDetail> GetAll()
        {
         List<JobOrderServiceCenterDetail> JobOrderDetail=   _uow.JobOrderServiceCenterDetailRepository.GetAll();
         return JobOrderDetail;
          
        }
    }
}
