using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore
{
    public partial class SalvageStoreGoodReceiving : Operations
    {
        [Key]
         [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }
        public Guid SalvageStoreGoodReceivingID { get; set; }
        [DisplayName("  Equipment ")]
        [Required]
        public Guid FleetsID { get; set; }
        [DisplayName(" Part Name ")]
        public string SparePartName { get; set; }
        [DisplayName(" Voucher Number")]
        [Required]
        public Nullable<int> VoucherNo { get; set; }
        public Nullable<DateTime> Date { get; set; }
        public decimal Quantity { get; set; }
           [DisplayName(" Unit Price ")]
        public decimal UnitPrice { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

       
        [NotMapped]
        public string PlateNumber { get; set; }
        [NotMapped]
        [DisplayName(" Total Price ")]
        public decimal TotalPrice { get; set; }

        public virtual Fleets Fleet { get; set; }
       

    }

}
