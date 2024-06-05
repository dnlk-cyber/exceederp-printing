
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetMaintenance
{
    public class SparePart : TrackUserOperation
    {

        [ScaffoldColumn(false)]
        public Guid SparePartID { get; set; }
        [DisplayName("Name")]
       
        public string SparePartName { get; set; }
        [DisplayName("Part Number")]
        [Required]
        public string PartNo { get; set; }
        public Nullable<int> Quantity { get; set; }
        [DisplayName("Unit Price")]
       
        public decimal UnitPrice { get; set; }
        [DisplayName("Receipt Number ")]
        [Required]
        public Nullable<int> ReceiptNo { get; set; }


    }
}