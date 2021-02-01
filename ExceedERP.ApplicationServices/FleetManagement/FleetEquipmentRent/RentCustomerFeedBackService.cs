using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent
{
   public class RentCustomerFeedBackService :IRentCustomerFeedBackService
    {
        

       private readonly UnitOfWork _uow;
        public RentCustomerFeedBackService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(RentCustomerFeedBack entity)
        {
            _uow.RentCustomerFeedBackRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<RentCustomerFeedBack> entityList)
        {
            _uow.RentCustomerFeedBackRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(RentCustomerFeedBack entity)
        {
            _uow.RentCustomerFeedBackRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(RentCustomerFeedBack entity)
        {
            _uow.RentCustomerFeedBackRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public RentCustomerFeedBack GetById(Guid id)
        {
         RentCustomerFeedBack RentCustomerFeedBack =   _uow.RentCustomerFeedBackRepository.FindById(id);
         return RentCustomerFeedBack;
        }

        public List<RentCustomerFeedBack> GetAll()
        {
         List<RentCustomerFeedBack> RentCustomerFeedBack=   _uow.RentCustomerFeedBackRepository.GetAll();
         return RentCustomerFeedBack;
          
        }
    }
}
