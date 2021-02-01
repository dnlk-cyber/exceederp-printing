using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public  class JobStoreRequisition : TrackUserOperation
    {
        public Guid JobStoreRequisitionId { get; set; }
        public Guid JobId { get; set; }
        public Guid JobDetailId { get; set; }
        public int RequisitionId { get; set; }
        public string Type { get; set; }
        public bool IsSalvage { get; set; }
    }
}
