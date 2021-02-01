using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Repository;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
    public class SiteVechicleTransferDetailService : ISiteVechicleTransferDetailService
    {
  


       

         private readonly UnitOfWork _uow;
         public SiteVechicleTransferDetailService()
        {
            _uow = new UnitOfWork();
        }
        public bool Insert(SiteVechicleTransferDetail entity)
        {
            _uow.SiteVehicleDetailTransferRepository.Add(entity);
            return _uow.SaveChanges();
        }

        public bool InsertBatch(List<SiteVechicleTransferDetail> entityList)
        {
            _uow.SiteVehicleDetailTransferRepository.AddRange(entityList);
            return _uow.SaveChanges();
        }

        public bool Update(SiteVechicleTransferDetail entity)
        {
            _uow.SiteVehicleDetailTransferRepository.Edit(entity);
            return _uow.SaveChanges();
        }

        public bool Delete(SiteVechicleTransferDetail entity)
        {
            _uow.SiteVehicleDetailTransferRepository.Delete(entity);
            return _uow.SaveChanges();
        }

        public SiteVechicleTransferDetail GetById(int id)
        {
         SiteVechicleTransferDetail SiteVechicleTransferDetail =   _uow.SiteVehicleDetailTransferRepository.FindById(id);
         return SiteVechicleTransferDetail;
        }

        public List<SiteVechicleTransferDetail> GetAll()
        {
         List<SiteVechicleTransferDetail> SiteVechicleTransferDetail=   _uow.SiteVehicleDetailTransferRepository.GetAll();
         return SiteVechicleTransferDetail;
          
        }

      

    }
}