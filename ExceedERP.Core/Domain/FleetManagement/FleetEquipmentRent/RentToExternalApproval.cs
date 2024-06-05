using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    public class RentToExternalApproval : TrackUserOperation
    {
        public int RentToExternalApprovalID { get; set; }
        public Guid RentToExternalId { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }


        [NotMapped]
        public int? AssignedQuantity { get; set; }

    }


}