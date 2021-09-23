using System;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExceedERP.Core.Domain.Inventory;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{

    public class LogisticPlanDetail : Operations
    {
        public int LogisticPlanDetailId { get; set; }
        public int LogisticPlanId { get; set; }
        public Weeks Weeks { get; set; }
           [DisplayName("Service")]
        public Guid ServiceId { get; set; }
            [DisplayName("Product")]
        public string ItemCode { get; set; }
       
        public decimal Quantity { get; set; }
        public ApprovalStatuses ApprovalStatus { get; set; }
     
        [NotMapped]
        public decimal WHA { get; set; }
        [NotMapped]
        public decimal OPQ { get; set; }
        public virtual Fleets Fleets { get; set; }
        public virtual Service Service { get; set; }
        [ForeignKey("ItemCode")]
        public virtual InventoryItem InventoryItem { get; set; }
    }


}
