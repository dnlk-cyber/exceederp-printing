
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    public class ExternalRentedEquipment : Operations
    {
 
        public Guid ExternalRentedEquipmentId { get; set; }
        [DisplayName("  Equipment Category ")]
        public Guid? EquipmentIdentityID { get; set; }
         [DisplayName("  Equipment Type ")]
        public Guid? EquipmentTypeID { get; set; }
        public Guid? EquipmentNameID { get; set; }
        public Guid FleetId { get; set; }
          public Guid RentId { get; set; }
          public string CurrentOdoMeter { get; set; }  
        public string AssetCode { get; set; } 
        public string EquipmentDetail { get; set; }
        public string EquipmentName { get; set; }
        public string OperatorName { get; set; }
        public decimal RentedRate { get; set; }
        public string RentStatus { get; set; }


        [ForeignKey("FleetId")]
        public virtual Fleets Fleets { get; set; }
    }
}