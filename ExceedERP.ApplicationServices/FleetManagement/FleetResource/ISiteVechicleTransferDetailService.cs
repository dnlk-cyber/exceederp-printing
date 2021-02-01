using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetResource;

namespace ExceedERP.ApplicationServices.FleetManagement.FleetResource
{
    public interface ISiteVechicleTransferDetailService
    {

        bool Insert(SiteVechicleTransferDetail entity);
        bool InsertBatch(List<SiteVechicleTransferDetail> entityList);
        bool Update(SiteVechicleTransferDetail entity);

        bool Delete(SiteVechicleTransferDetail entity);
        SiteVechicleTransferDetail GetById(int id);
        List<SiteVechicleTransferDetail> GetAll();

    }
}
