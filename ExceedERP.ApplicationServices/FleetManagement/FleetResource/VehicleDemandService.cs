using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
   public class VehicleDemandService : IVehicleDemandService
    {
       

         private readonly UnitOfWork _uow;
         public VehicleDemandService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(VehicleDemand entity)
        {
            _uow.VehicleDemandRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<VehicleDemand> entityList)
        {
            _uow.VehicleDemandRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(VehicleDemand entity)
        {
            _uow.VehicleDemandRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(VehicleDemand entity)
        {
            _uow.VehicleDemandRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public VehicleDemand GetById(Guid id)
        {
         VehicleDemand VehicleDemand =   _uow.VehicleDemandRepository.FindById(id);
         return VehicleDemand;
        }

        public List<VehicleDemand> GetAll()
        {
         List<VehicleDemand> VehicleDemand=   _uow.VehicleDemandRepository.GetAll();
         return VehicleDemand;
          
        }
    }
}