using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    public class RentOtherSupplierApproval : TrackUserOperation
    {
        public int RentOtherSupplierApprovalID { get; set; }
        public Guid RentOtherSupplierId { get; set; }
        public virtual RentOtherSuppliers RentOtherSupplier { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }


        [NotMapped]
        public int? AssignedQuantity { get; set; }

    }


}