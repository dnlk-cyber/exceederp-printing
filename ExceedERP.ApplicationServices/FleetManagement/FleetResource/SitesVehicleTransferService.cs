using ExceedERP.Core.Domain.FleetManagement.FleetResource;
using ExceedERP.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
   public  class SitesVehicleTransferService :ISitesVehicleTransferService
    {

       
      

         private readonly UnitOfWork _uow;
         public SitesVehicleTransferService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(SitesVehicleTransfer entity)
        {
            _uow.SitesVehicleTransferRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<SitesVehicleTransfer> entityList)
        {
            _uow.SitesVehicleTransferRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(SitesVehicleTransfer entity)
        {
            _uow.SitesVehicleTransferRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(SitesVehicleTransfer entity)
        {
            _uow.SitesVehicleTransferRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public SitesVehicleTransfer GetById(Guid id)
        {
         SitesVehicleTransfer SitesVehicleTransfer =   _uow.SitesVehicleTransferRepository.FindById(id);
         return SitesVehicleTransfer;
        }

        public List<SitesVehicleTransfer> GetAll()
        {
         List<SitesVehicleTransfer> SitesVehicleTransfer=   _uow.SitesVehicleTransferRepository.GetAll();
         return SitesVehicleTransfer;
          
        }
    }
}