using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System.ComponentModel;
 using ExceedERP.Core.Domain.Common;


namespace ExceedERP.Core.Domain.FleetManagement.EquipmentInspection
{


    public partial class AnnualEquipmentInventoryInspection : Operations
    {
       [ScaffoldColumn(false)]
        public Guid AnnualEquipmentInventoryInspectionID { get; set; }
        [DisplayName("Fleet")]
       public Guid FleetsID { get; set; }
        [Required]
        [DisplayName("Inspected By")]
       public string InspectedBy  { get; set; }
        [Required]
        [DisplayName("Finding Outcome")]
        [DataType(DataType.MultilineText)]
        public string FindingsOutcome { get; set; }
        [Required]
        [DisplayName("Date of Inspection")]
        [DataType(DataType.Date)]
       [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
       public DateTime DateofInspection { get; set; }
        [NotMapped]
        public string EquipmentTypeId { get; set; }
           [ForeignKey("FleetsID")]
       public virtual Fleets Fleets { get; set; }
    }
}