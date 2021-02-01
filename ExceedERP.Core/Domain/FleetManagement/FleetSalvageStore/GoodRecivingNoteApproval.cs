using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore
{
    public class GoodRecivingNoteApproval : TrackUserOperation
    {
        public int GoodRecivingNoteApprovalID { get; set; }
        public Guid SalvageStoreGoodReceivingId { get; set; }
        public virtual SalvageStoreGoodReceiving SalvageStoreGoodReceiving { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
    }

}
