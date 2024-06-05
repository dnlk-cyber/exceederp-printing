using System;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using ExceedERP.Core.Domain.FleetManagement;

namespace ExceedERP.Core.Domain.Maintenance
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
