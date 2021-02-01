using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetSalvageStore
{
    public class SalvageStoreGoodReceivingService : ISalvageStoreGoodReceivingService
    {
                  private readonly UnitOfWork _uow;
         public SalvageStoreGoodReceivingService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(SalvageStoreGoodReceiving entity)
        {
            _uow.SalvageStoreGoodReceivingRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<SalvageStoreGoodReceiving> entityList)
        {
            _uow.SalvageStoreGoodReceivingRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(SalvageStoreGoodReceiving entity)
        {
            _uow.SalvageStoreGoodReceivingRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(SalvageStoreGoodReceiving entity)
        {
            _uow.SalvageStoreGoodReceivingRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public SalvageStoreGoodReceiving GetById(Guid id)
        {
         SalvageStoreGoodReceiving SalvageStoreGoodReceiving =   _uow.SalvageStoreGoodReceivingRepository.FindById(id);
         return SalvageStoreGoodReceiving;
        }

        public List<SalvageStoreGoodReceiving> GetAll()
        {
         List<SalvageStoreGoodReceiving> SalvageStoreGoodReceiving=   _uow.SalvageStoreGoodReceivingRepository.GetAll();
         return SalvageStoreGoodReceiving;
          
        }
    }
}
 