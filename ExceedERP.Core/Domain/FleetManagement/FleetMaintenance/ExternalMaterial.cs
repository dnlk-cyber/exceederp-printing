using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
   public  class ExternalMaterial : Operations
    {
       public Guid ExternalMaterialID { get; set; }
       public Guid JobId { get; set; }
       [Required]
       public string MaterialName { get; set; }
       [DisplayName(" Spare Type")]
        public SpareType Type { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Quantity { get; set; }
        public decimal TotalAmount { get; set; }
          [DisplayName(" Approval Status")]
       public ApprovalStatuses ApprovalStatuses { get; set; }
    }
}
