using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;


namespace ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore
{
    public partial class SalvageStore : Operations
    {
        public Guid SalvageStoreID { get; set; }
        [DisplayName("  Equipment ")]
        [Required]
        public Guid FleetsID { get; set; }
        public Nullable<Guid> JobId { get; set; }
        [DisplayName(" Spare Part")]
        public Guid SalvageStoreGoodReceivingId { get; set; }
          [DisplayName(" Part Name ")]
        public string SparePartName { get; set; }
          public decimal Price { get; set; }
        public decimal Quantity { get; set; }
         [DisplayName(" Approved Quantity ")]
        public decimal ApprovedQuantity { get; set; }
        [DisplayName(" Voucher Number")]
        [Required]
        public Nullable<int> VoucherNo { get; set; }
        public Nullable<DateTime> Date { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
     
       
        [NotMapped]
        public string PlateNumber { get; set; }


        public virtual Fleets Fleet { get; set; } 

    }
}