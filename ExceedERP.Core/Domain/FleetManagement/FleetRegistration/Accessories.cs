
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{


    public partial class Accessories : Operations
    {
        [ScaffoldColumn(false)]
        public Guid AccessoriesID { get; set; }
        public Guid FleetsID { get; set; }
          [DisplayName("Accessory Name")]
          [Required]
        public string AccessoryName {get; set;}
         [DisplayName("Unit of Measurement")]
        public string UnitofMeasurement {get; set;}
         [Required]
        public Nullable<int> Quantity {get; set;}
        [DisplayName("Unit Price")]
         [Required]
         public Nullable<int> UnitPrice { get; set; }
        public string SerialNo { get; set; }
        [DisplayName("Issued Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> IssuedDate {get; set;}
        public enum Statusenum {New, Serviceable, Unserviceable }
        public string Status { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

      [ForeignKey("FleetsID")]
        public virtual Fleets Fleet { get; set; }
     
    }



}