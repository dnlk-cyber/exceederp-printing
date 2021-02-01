
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetRegistration
{
    public class ManufacturerService : IManufacturerService
    {
   
       private readonly UnitOfWork _uow;
        public ManufacturerService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(Manufacturer entity)
        {
            _uow.ManufacturerRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<Manufacturer> entityList)
        {
            _uow.ManufacturerRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(Manufacturer entity)
        {
            _uow.ManufacturerRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(Manufacturer entity)
        {
            _uow.ManufacturerRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public Manufacturer GetById(Guid id)
        {
         Manufacturer Manufacturer =   _uow.ManufacturerRepository.FindById(id);
         return Manufacturer;
        }

        public List<Manufacturer> GetAll()
        {
         List<Manufacturer> Manufacturer=   _uow.ManufacturerRepository.GetAll();
         return Manufacturer;
          
        }


        public bool AddOrUpdate(Manufacturer entity)
        {
            _uow.ManufacturerRepository.AddOrUpdate(entity);
            return _uow.SaveChanges();
        }
    }
}
