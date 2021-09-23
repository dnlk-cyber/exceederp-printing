using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
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

        [DisplayName(" Work Unit ")]
        public string RequestingStructureName { get; set; }
        public string Referance { get; set; }
            [DisplayName(" # Distribution  ")]
        public int? RequestNumber { get; set; }
        public Nullable<int> DistrubtionNumber { get; set; }
        [NotMapped]
        public decimal Duration { get; set; }

        public Nullable<int> AssignedQuantity { get; set; }
        [DisplayName("Equipment Status")]
        public string EquipmentStatus { get; set; }
     
        public virtual RentRequest RentRequest { get; set; }

    }
}