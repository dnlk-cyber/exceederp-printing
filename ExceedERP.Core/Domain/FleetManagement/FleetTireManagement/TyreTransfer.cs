using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTireManagement
{

    public partial class TyreTransfer : Operations
    {
         public Guid TyreTransferId { get; set; }
        [DisplayName(" Tyre")]
        public string TyreSerialNo { get; set; }

        [DisplayName("From Equipment")]
        [Required]
        public string FromEquipment { get; set; }
        [DisplayName("To Equipment")]
        [Required]
        public string ToEquipment { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public virtual EquipmentTyre EquipmentTyres { get; set; }

    }
}