using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
  public  class VehicleReturnService : IVehicleReturnService
    {

        

         private readonly UnitOfWork _uow;
         public VehicleReturnService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(VehicleReturn entity)
        {
            _uow.VehicleReturnRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<VehicleReturn> entityList)
        {
            _uow.VehicleReturnRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(VehicleReturn entity)
        {
            _uow.VehicleReturnRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(VehicleReturn entity)
        {
            _uow.VehicleReturnRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public VehicleReturn GetById(Guid id)
        {
         VehicleReturn VehicleReturn =   _uow.VehicleReturnRepository.FindById(id);
         return VehicleReturn;
        }

        public List<VehicleReturn> GetAll()
        {
         List<VehicleReturn> VehicleReturn=   _uow.VehicleReturnRepository.GetAll();
         return VehicleReturn;
          
        }
    }
}