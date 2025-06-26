
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
        [Display(Name = "Equipment Category")]
        public Guid EquipmentCategoryId { get; set; }
        [Display(Name = "Equipment Type")]
        public Guid EquipmentTypeId { get; set; }
        [Display(Name = "Equipment Name")]
        public Guid EquipmentNameId { get; set; }
        [Display(Name ="Plate No")]
        public Guid FleetId { get; set; }
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

        
       
    }
}