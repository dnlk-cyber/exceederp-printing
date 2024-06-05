using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTireManagement
{
    public class TyreTransferApproval : TrackUserOperation
    {
        public int TyreTransferApprovalId { get; set; }
        public Guid TyreId { get; set; }

        public string Status { get; set; }
        public string Remark { get; set; }

        public virtual TyreTransfer TyreTransfers { get; set; }
    }


}
