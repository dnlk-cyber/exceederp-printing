using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public class AdministrativeJobApprovalService : IAdministrativeApprovalService
    {
  
             
       private readonly UnitOfWork _uow;
        public AdministrativeJobApprovalService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(AdministrativeJobApproval entity)
        {
            _uow.AdministrativeJobApprovalRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<AdministrativeJobApproval> entityList)
        {
            _uow.AdministrativeJobApprovalRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(AdministrativeJobApproval entity)
        {
            _uow.AdministrativeJobApprovalRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(AdministrativeJobApproval entity)
        {
            _uow.AdministrativeJobApprovalRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public AdministrativeJobApproval GetById(Guid id)
        {
         AdministrativeJobApproval AdministrativeJobApproval =   _uow.AdministrativeJobApprovalRepository.FindById(id);
         return AdministrativeJobApproval;
        }

        public List<AdministrativeJobApproval> GetAll()
        {
         List<AdministrativeJobApproval> AdministrativeJobApproval=   _uow.AdministrativeJobApprovalRepository.GetAll();
         return AdministrativeJobApproval;
          
        }
    }
}
