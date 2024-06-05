using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public partial class SiteVechicleTransferDetail : Operations
    {

        public int SiteVechicleTransferDetailId { get; set; }

        public Guid SiteVechicleTransferId { get; set; }

        
        public string Type { get; set; }
        public string SerialNo { get; set; }
        public decimal Quantity { get; set; }

        public string Description { get; set; }

        public string DamagedParts { get; set; }
         
        public string MissingParts { get; set; }


    }
}
