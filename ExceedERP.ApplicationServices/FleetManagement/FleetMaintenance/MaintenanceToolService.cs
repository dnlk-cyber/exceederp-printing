using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
   public class MaintenanceToolService : IMaintenanceToolService
    {

          
       private readonly UnitOfWork _uow;
        public MaintenanceToolService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(MaintenanceTool entity)
        {
            _uow.MaintenanceToolRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<MaintenanceTool> entityList)
        {
            _uow.MaintenanceToolRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(MaintenanceTool entity)
        {
            _uow.MaintenanceToolRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(MaintenanceTool entity)
        {
            _uow.MaintenanceToolRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public MaintenanceTool GetById(Guid id)
        {
         MaintenanceTool MaintenanceTool =   _uow.MaintenanceToolRepository.FindById(id);
         return MaintenanceTool;
        }

        public List<MaintenanceTool> GetAll()
        {
         List<MaintenanceTool> MaintenanceTool=   _uow.MaintenanceToolRepository.GetAll();
         return MaintenanceTool;
          
        }
    }
}
