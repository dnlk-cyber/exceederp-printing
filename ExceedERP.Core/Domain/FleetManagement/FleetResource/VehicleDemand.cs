
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
    public partial class VehicleDemand : OperationWithSecondValidation
    {
        [Key]
        [DisplayName(" Request Number ")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }
        public Guid VehicleDemandId { get; set; }
        [DisplayName("Request Number")]
        public int? RequestNumber { get; set; }
      
        [DisplayName(" Request Quantity ")]
      
        public Nullable<int> RequestQuantity { get; set; }
       [Required]
        [DisplayName(" Project ")]
        public string FromBranchName { get; set; }
           
        [Required]
        [DisplayName("  Work Unit ")]
        public string FromStructureName { get; set; }

        [DisplayName(" Branch ")]
        public string RequestingBranchName { get; set; }

        [DisplayName(" Work Unit ")]
        public string RequestingStructureName { get; set; }

        [DisplayName("Start Date")]
     
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartingDate { get; set; }
        [DisplayName("End Date")]
       
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ToDate { get; set; }
        public string EquipmnetStatus { get; set; }
  
        public enum EquipmentStatusenum { New, Good, NeverMind }


    }
}