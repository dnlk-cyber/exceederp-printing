using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.BatteryManagement
{
    public class BatteryReturn : Operations
    {


       public Guid BatteryReturnId { get; set; }
       public Guid BatteryId { get; set; }
        [DisplayName(" Equipment")]
        public string PlateNumber { get; set; }
        [DisplayName(" Operator Name")]
        public string OperatorName { get; set; }
        [DisplayName(" Make Country ")]
        public string MakeCountry { get; set; }
      
        public string StoreCode { get; set; }
      
        [DisplayName("Battery Type")]
        [Required]
        public string BatteryType { get; set; } 
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
        public string Condition { get; set; }
        [Required]
        [DisplayName("KM Reading  ")]
        public Nullable<int> KMReading { get; set; }
        [Required]
        [DisplayName("Unit Price ")]
        public decimal UnitPrice { get; set; }
  

        [DisplayName(" Description Reason ")]
        [DataType(DataType.MultilineText)]
        [Required]
        public string DescriptionReason { get; set; }
    }
}
