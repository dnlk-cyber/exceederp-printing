using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Inventory;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class ServiceSpare : Operations
    {

       
            public Guid ServiceSpareID { get; set; }
        [DisplayName("Service")]
        [Required]
        public Guid ServiceID { get; set; }
       [DisplayName("Item Name")]
        public string InventoryItemId { get; set; }
        [DisplayName("Name")]
        public string ServiceName { get; set; }

        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> Date { get; set; }
         public decimal Quantity { get; set; }
           [DisplayName("Used Quantity")]
         public decimal UsedQuantity { get; set; }
           [DisplayName("Unit Price")]
           public decimal UnitPrice { get; set; }
         public SpareType Type { get; set; }
         [DisplayName("Status")]
        public ApprovalStatuses ApprovalStatus { get; set; } 

     
        public virtual InventoryItem InventoryItems { get; set; }
    
    }
}
