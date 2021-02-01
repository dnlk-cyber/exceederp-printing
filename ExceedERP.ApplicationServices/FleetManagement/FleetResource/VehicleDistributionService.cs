using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
    public class VehicleDistributionService : IVehicleDistributionService
    {

        

         private readonly UnitOfWork _uow;
         public VehicleDistributionService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(VehicleDistribution entity)
        {
            _uow.VehicleDistributionRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<VehicleDistribution> entityList)
        {
            _uow.VehicleDistributionRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(VehicleDistribution entity)
        {
            _uow.VehicleDistributionRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(VehicleDistribution entity)
        {
            _uow.VehicleDistributionRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public VehicleDistribution GetById(Guid id)
        {
         VehicleDistribution VehicleDistribution =   _uow.VehicleDistributionRepository.FindById(id);
         return VehicleDistribution;
        }

        public List<VehicleDistribution> GetAll()
        {
         List<VehicleDistribution> VehicleDistribution=   _uow.VehicleDistributionRepository.GetAll();
         return VehicleDistribution;
          
        }
    }
}