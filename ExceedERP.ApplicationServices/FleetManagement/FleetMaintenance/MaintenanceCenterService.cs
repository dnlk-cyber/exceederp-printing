using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public class MaintenanceCenterService : IMaintenanceCenterService
    {

             
       private readonly UnitOfWork _uow;
        public MaintenanceCenterService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(MaintenanceCenter entity)
        {
            _uow.MaintenanceCenterRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<MaintenanceCenter> entityList)
        {
            _uow.MaintenanceCenterRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(MaintenanceCenter entity)
        {
            _uow.MaintenanceCenterRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(MaintenanceCenter entity)
        {
            _uow.MaintenanceCenterRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public MaintenanceCenter GetById(Guid id)
        {
         MaintenanceCenter MaintenanceCenter =   _uow.MaintenanceCenterRepository.FindById(id);
         return MaintenanceCenter;
        }

        public List<MaintenanceCenter> GetAll()
        {
         List<MaintenanceCenter> MaintenanceCenter=   _uow.MaintenanceCenterRepository.GetAll();
         return MaintenanceCenter;
          
        }
    }
}
