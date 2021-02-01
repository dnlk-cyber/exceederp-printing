using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel;

namespace ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore
{
    public class SalvageStoreApproval : TrackUserOperation
    {
        public int SalvageStoreApprovalID { get; set; }
        public Guid SalvageStoreId { get; set; }
          [DisplayName(" Approved Quantity ")]
        public decimal ApprovedQuantity { get; set; }
        public string Status { get; set; }
        public string Remark { get; set; }
        public virtual SalvageStore SalvageStore { get; set; }
    }

}