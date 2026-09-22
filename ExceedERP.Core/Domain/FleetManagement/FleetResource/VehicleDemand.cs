
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
    public partial class VehicleDemand : DepartmentNewOperationWithSecondValidation
    {
        [Key]
        [DisplayName(" Request Number ")]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }
        public Guid VehicleDemandId { get; set; }
        [DisplayName("Request Number")]
        public int? RequestNumber { get; set; }
        [NotMapped]
        [DisplayName("Operator Name")]
        public int? OperatorId { get; set; }
        public string ServiceCenterJobId { get; set; }
        [NotMapped]
        public string EmployeeName { get; set; }
        [NotMapped]
        [DisplayName("Plate Number")]
        public string PlateNumber { get; set; }
        [NotMapped]
        [DisplayName("Phone Number")]
        public string phoneNumber { get; set; }
        [DisplayName("Employee Name")]
        public int EmployeeId { get; set; }      
        [DisplayName(" Request Quantity ")]      
        public Nullable<int> RequestQuantity { get; set; }
        [Required]
        [DisplayName(" Project ")]
        public string FromBranchName { get; set; }           
        [Required]
        [DisplayName("  Work Unit ")]
        public string FromStructureName { get; set; }

        //Added
        public int BranchId { get; set; }
        public int BusinessUnitId { get; set; }
        //

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
        [DisplayName("Reason For Travel"), Required]
        public string ReasonForTravel { get; set; }
        [DisplayName("Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }
        [DisplayName("Is Assigned")]
        public bool IsAssigned { get; set; }
        public enum EquipmentStatusenum { New, Good, NeverMind }

        public int OrgStructureId { get; set; }
        public string ForwardedTo { get; set; }
        public string ForwardedFrom { get; set; }

    }
}