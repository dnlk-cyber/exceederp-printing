using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.EquipmentInspection
{
    public class AnnualInventoryInspectionApproval : TrackUserOperation
    {
        public int AnnualInventoryInspectionApprovalID { get; set; }
        public Guid AnnualEquipmentInventoryInspectionId { get; set; }
        public virtual AnnualEquipmentInventoryInspection AnnualEquipmentInventoryInspection { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }


}
