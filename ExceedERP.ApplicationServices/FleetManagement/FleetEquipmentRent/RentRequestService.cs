using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetEquipmentRent
{
   public class RentRequestService :IRentRequestService
    {
 

        
       private readonly UnitOfWork _uow;
        public RentRequestService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(RentRequest entity)
        {
            _uow.RentRequestRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<RentRequest> entityList)
        {
            _uow.RentRequestRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(RentRequest entity)
        {
            _uow.RentRequestRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(RentRequest entity)
        {
            _uow.RentRequestRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public RentRequest GetById(Guid id)
        {
         RentRequest RentRequest =   _uow.RentRequestRepository.FindById(id);
         return RentRequest;
        }

        public List<RentRequest> GetAll()
        {
         List<RentRequest> RentRequest=   _uow.RentRequestRepository.GetAll();
         return RentRequest;
          
        }
    }
}
