
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    public partial class RentRequest : OperationWithSecondValidation
    {
        [Key]
        [DisplayName(" Request Number ")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }
          [ScaffoldColumn(false)]
        public Guid RentRequestID { get; set; }
        [DisplayName("Request Number")]
        public int? RequestNumber { get; set; }
      
       
      
  
        [DisplayName("Request Quantity")]
        public Nullable<int> RequestQuantity { get; set; }
        [Required]
        [DisplayName("  Project ")]
        public string FromBranchName { get; set; }
        [Required]
        [DisplayName("  Work Unit ")]
        public string FromStructureName { get; set; }

        [DisplayName("Requesting Branch ")]
        public string RequestingBranchName { get; set; }
     
        [DisplayName("Requesting Structure ")]
        public string RequestingStructureName { get; set; }
    
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("Start Date")]
        public Nullable<DateTime> StartingDate { get; set; }
   
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("End Date")]
        public Nullable<DateTime> ToDate { get; set; }
                                                              
    
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        public string EquipmnetStatus { get; set; }
        public enum EquipmentStatusenum { New, Good, NeverMind }
    }
}