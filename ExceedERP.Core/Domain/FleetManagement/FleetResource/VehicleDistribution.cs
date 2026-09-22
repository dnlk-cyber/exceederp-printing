
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{

    public partial class VehicleDistribution : Operations
    {

        [Key]
        [DisplayName(" Distribution Number ")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }
        public Guid VehicleDistributionID { get; set; }

        [Required]
        [DisplayName(" Request")]
        public Guid RequestID { get; set; }
        public enum CompanyRequestType { Company }
        public enum RequestType { Rent, Company }
        public RequestType Type { get; set; }
        [DisplayName("Plate Number")]
        public string PlateNumber { get; set; }
        [DisplayName("Manufacturing Year")]
        public string ManufacturingYear { get;set;}

        [DisplayName(" Start Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> StartingDate { get; set; }
        [DisplayName(" End Date ")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public Nullable<DateTime> ToDate { get; set; }
        [Required]
        [DisplayName("  Branch ")]
        public string FromBranchName { get; set; }
        [Required]
        [DisplayName("  Work Unit ")]
        public string FromStructureName { get; set; }
        
        [DisplayName(" Branch ")]
        public string RequestingBranchName { get; set; }
        [DisplayName(" Employee Name")]
        public int EmployeeId { get; set; }
        public string ReasonForTravel { get; set; }       
        [DisplayName("Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [DisplayName("Assignment Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime AssignmentDate { get; set; }
        public bool IsAssigned { get; set; }
        [DisplayName(" Work Unit ")]
        public string RequestingStructureName { get; set; }
        public string Reference { get; set; }
            [DisplayName(" # Distribution  ")]
        public int? RequestNumber { get; set; }
        public Nullable<int> DistrubtionNumber { get; set; }
        [NotMapped]
        public decimal Duration { get; set; }
        public string NumberHourKM { get; set; }

        public Nullable<int> AssignedQuantity { get; set; }
        [DisplayName("Equipment Status")]
        public string EquipmentStatus { get; set; }
        [DisplayName("Equipment Type")]
        public string EquipmentType { get; set; }
        [DisplayName("Equipment Name")]
        public string EquipmentName { get; set; }
        public string Destination { get; set; }
        public decimal NumberofPersonston { get; set; }
        public string ParentId { get; set; }
        public bool IsExternal { get; set; }
        public string Remarks { get; set; }
        public virtual RentRequest RentRequest { get; set; }

    }
   
}