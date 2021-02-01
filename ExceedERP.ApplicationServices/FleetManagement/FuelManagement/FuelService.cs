using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FuelManagement
{
    public class FuelService :IFuelService
    {


         private readonly UnitOfWork _uow;
         public FuelService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(Fuel entity)
        {
            _uow.FuelRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<Fuel> entityList)
        {
            _uow.FuelRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(Fuel entity)
        {
            _uow.FuelRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(Fuel entity)
        {
            _uow.FuelRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public Fuel GetById(Guid id)
        {
         Fuel Fuel =   _uow.FuelRepository.FindById(id);
         return Fuel;
        }

        public List<Fuel> GetAll()
        {
         List<Fuel> Fuel=   _uow.FuelRepository.GetAll();
         return Fuel;
          
        }
    }
}