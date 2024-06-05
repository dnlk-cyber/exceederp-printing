
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    // feedback obj that reference this obj
    public partial class RentInternalProject : Operations
    {
        [ScaffoldColumn(false)]
        public Guid RentInternalProjectID { get; set; }

        public enum EquipmentTypeenum { DoubleCap, Dump, Track, Excavator }
        public enum EquipmentOwnerenum { Company, Rental }
        [DisplayName("  Equipment Category ")]
        [Required]
        public string EquipmentIdentity  { get; set; }
        [DisplayName("Equipment Type ")]
        [Required]
        public string EquipmentType  { get; set; }

        [Required]
        [DisplayName("Equipment Owner")]
        public string EquipmentOwner { get; set; }
        [DisplayName("Assigned Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> AssignedDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Start Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartingDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("End Date")]
        [Required]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ToDate { get; set; }
        public string AssignedBy { get; set; }
        public string ApprovedBy { get; set; }
        public string Status { get; set; }
        [Required]
        [DisplayName("Request Quantity")]
        public Nullable<int> RequestQuantity { get; set; }
        public Nullable<int> AssignedQuantity { get; set; }


    }
}