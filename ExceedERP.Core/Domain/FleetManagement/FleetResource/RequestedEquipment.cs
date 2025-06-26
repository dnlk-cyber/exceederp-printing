using ExceedERP.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public class RequestedEquipment : DepartmentNewOperations
    {
        public Guid RequestedEquipmentId { get; set; }
        public Guid RequestNumber { get; set; }
        public Guid EquipmentIdentityID { get; set; }
        [Display(Name ="Equipment Type")]
        public Guid EquipmentTypeID { get; set; }
        [Display(Name ="Equipment Name")]
        public Guid? EquipmentNameId { get; set; } // Added
        [Required]
        public int Quantity { get; set; }
        public string ManufacturedYear { get; set; }
        [Required]
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? AssignedDate { get; set; }
        public DateTime? RequestedDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public DateTime? ArrivalDate { get; set; }
        public DateTime? SentDate { get; set; }
        public string ArrivalPlace { get; set; }
        public decimal AsphaltKm { get; set; }
        public decimal GravelKm { get; set; }

        [DisplayName("Number (Hour / Km)")]
        public string NumberHourPerKm { get; set; }
        [DisplayName("Destination")]
        public string Destination { get; set; }
        [DisplayName("Number of Persons/Ton")]
        public string NumberofPersonOrTon { get; set; }
        [DisplayName("Equipment Status")]
        public string EquipmentStatus { get; set; }
        public string RequestDesc { get; set; }
        [DisplayName("Number Hour/KM")]
        public string NumberHourKM { get; set; }
        [DisplayName("Number sof Persons/ton")]
        public string NumberofPersonston { get; set; }
        [DisplayName("Remarks")]
        public string Remarks { get; set; }
        public enum EquipmentStatusenum { New, Good, NeverMind }
        public bool IsAssigned { get; set; }

        public int VehicleDemandId { get; set; } // r/nship with VehicleDemand


    }
}
