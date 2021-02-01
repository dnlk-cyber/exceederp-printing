using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent
{
  public  class RentOtherSuppliersService :IRentOtherSuppliersService
    {

        private readonly UnitOfWork _uow;
        public RentOtherSuppliersService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(RentOtherSuppliers entity)
        {
            _uow.RentOtherSuppliersRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<RentOtherSuppliers> entityList)
        {
            _uow.RentOtherSuppliersRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(RentOtherSuppliers entity)
        {
            _uow.RentOtherSuppliersRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(RentOtherSuppliers entity)
        {
            _uow.RentOtherSuppliersRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public RentOtherSuppliers GetById(Guid id)
        {
         RentOtherSuppliers RentOtherSuppliers =   _uow.RentOtherSuppliersRepository.FindById(id);
         return RentOtherSuppliers;
        }

        public List<RentOtherSuppliers> GetAll()
        {
         List<RentOtherSuppliers> RentOtherSuppliers=   _uow.RentOtherSuppliersRepository.GetAll();
         return RentOtherSuppliers;
          
        }
    }
}
