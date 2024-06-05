using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetMaintenance
{
    public class OutsourceMaintenanceService : IOutsourceMaintenanceService
    {

                  
       private readonly UnitOfWork _uow;
        public OutsourceMaintenanceService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(OutsourceMaintenance entity)
        {
            _uow.OutsourceMaintenanceRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<OutsourceMaintenance> entityList)
        {
            _uow.OutsourceMaintenanceRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(OutsourceMaintenance entity)
        {
            _uow.OutsourceMaintenanceRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(OutsourceMaintenance entity)
        {
            _uow.OutsourceMaintenanceRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public OutsourceMaintenance GetById(Guid id)
        {
         OutsourceMaintenance OutsourceMaintenance =   _uow.OutsourceMaintenanceRepository.FindById(id);
         return OutsourceMaintenance;
        }

        public List<OutsourceMaintenance> GetAll()
        {
         List<OutsourceMaintenance> OutsourceMaintenance=   _uow.OutsourceMaintenanceRepository.GetAll();
         return OutsourceMaintenance;
          
        }
    }
}
