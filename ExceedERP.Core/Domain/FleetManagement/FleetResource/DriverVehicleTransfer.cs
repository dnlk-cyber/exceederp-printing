
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public partial class DriverVehicleTransfer : Operations
    {
        public DriverVehicleTransfer()
        {

        }
        public Guid DriverVehicleTransferID { get; set; }

        [DisplayName("From Equipment ")]
        public Guid FromEquipment { get; set; }
        [DisplayName("To Equipment ")]
        public Guid ToEquipment { get; set; }

       [DisplayName("From Operator ")]
        public string FromOperatorId { get; set; }
        [DisplayName("To Operator ")]
        public string ToOperatorId { get; set; }
        [DisplayName("Starting Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartingDate { get; set; }
        [DisplayName("End Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ToDate { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [NotMapped]
        public string EquipmentTypeId { get; set; }
       
    }
}