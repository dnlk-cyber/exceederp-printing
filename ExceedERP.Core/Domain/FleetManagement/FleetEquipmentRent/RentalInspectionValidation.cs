using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    public class RentalInspectionValidation
    {
        public int RentalInspectionValidationId { get; set; }
        public int RentalInspectionId { get; set; }
        public virtual RentalInspection RentalInspection { get; set; }
        public ApprovalStatuses Status { get; set; }
        public string Remark { get; set; }
    }
}
