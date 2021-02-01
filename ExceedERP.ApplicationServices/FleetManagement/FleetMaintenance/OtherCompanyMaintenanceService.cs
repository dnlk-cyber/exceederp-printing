using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
   public class OtherCompanyMaintenanceService :IOtherCompanyMaintenanceService
    {
    
       private readonly UnitOfWork _uow;
        public OtherCompanyMaintenanceService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(OtherCompanyMaintenance entity)
        {
            _uow.OtherCompanyMaintenanceRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<OtherCompanyMaintenance> entityList)
        {
            _uow.OtherCompanyMaintenanceRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(OtherCompanyMaintenance entity)
        {
            _uow.OtherCompanyMaintenanceRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(OtherCompanyMaintenance entity)
        {
            _uow.OtherCompanyMaintenanceRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public OtherCompanyMaintenance GetById(Guid id)
        {
         OtherCompanyMaintenance OtherCompanyMaintenance =   _uow.OtherCompanyMaintenanceRepository.FindById(id);
         return OtherCompanyMaintenance;
        }

        public List<OtherCompanyMaintenance> GetAll()
        {
         List<OtherCompanyMaintenance> OtherCompanyMaintenance=   _uow.OtherCompanyMaintenanceRepository.GetAll();
         return OtherCompanyMaintenance;
          
        }
    }
}
