
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetTireManagement
{
    public partial class TireReturn : Operations
    {
        
        public Guid TireReturnID { get; set; }
        public Guid TireId { get; set; }
        public Guid FleetId { get; set; }
         [DisplayName(" Equipment")]
        public string PlateNumber { get; set; }
        public FleetBatteryOrTyreType ReturnType { get; set; }
        public string StoreCode { get; set; }
         [DisplayName(" Operator Name")]
         public string OperatorName { get; set; }
        [DisplayName("Tyre Size")]
        [Required]
        public string TireSize { get; set; }
        [DisplayName("Tyre Type")]
        [Required]
        public string TireType { get; set; }
        public enum TireTypeenum { Wire, Fiber }
        [DisplayName("Installed Date")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> InstalledDate { get; set; }
        [Required]
        public string Brand { get; set; }
        [Required]
        [DisplayName("Serial Number ")]
        public string SerialNo { get; set; }
        public enum TireTubeenum { With, WithOut }
        [Required]
        [DisplayName("Tyre Tube  ")]
        public string TireTube { get; set; }
        [Required]
        [DisplayName("KM Reading  ")]
        public decimal KMReading { get; set; }
        [Required]
        [DisplayName("Unit Price ")]
        public decimal UnitPrice { get; set; } 

          [DisplayName(" Description Reason ")]
          [DataType(DataType.MultilineText)]
          [Required] 
        public string DescriptionReason { get; set; }
        public int ReturnId { get; set; }
        public int ReturnItemId { get; set; } 

    }
}