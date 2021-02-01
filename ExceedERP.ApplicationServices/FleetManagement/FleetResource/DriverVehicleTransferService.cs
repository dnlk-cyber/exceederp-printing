using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
  public class DriverVehicleTransferService :IDriverVehicleTransferService
    {


      

         private readonly UnitOfWork _uow;
         public DriverVehicleTransferService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(DriverVehicleTransfer entity)
        {
            _uow.DriverVehicleTransferRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<DriverVehicleTransfer> entityList)
        {
            _uow.DriverVehicleTransferRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(DriverVehicleTransfer entity)
        {
            _uow.DriverVehicleTransferRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(DriverVehicleTransfer entity)
        {
            _uow.DriverVehicleTransferRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public DriverVehicleTransfer GetById(Guid id)
        {
         DriverVehicleTransfer DriverVehicleTransfer =   _uow.DriverVehicleTransferRepository.FindById(id);
         return DriverVehicleTransfer;
        }

        public List<DriverVehicleTransfer> GetAll()
        {
         List<DriverVehicleTransfer> DriverVehicleTransfer=   _uow.DriverVehicleTransferRepository.GetAll();
         return DriverVehicleTransfer;
          
        }


      
    }
}