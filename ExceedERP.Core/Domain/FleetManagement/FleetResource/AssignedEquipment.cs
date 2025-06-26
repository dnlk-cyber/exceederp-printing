using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetResource
{
    public enum FleetReturnType
    {
        R,
        A
    }
    public partial class AssignedEquipment : Operations
    {
        public Guid AssignedEquipmentID { get; set; }
        public Guid RequestedEquipmentId { get; set; }
        [DisplayName("Equipment Name")]
        public Guid EquipmentNameID { get; set; }
        public Guid RequestNumber { get; set; }
        [DisplayName("Operator Name")]
        public Guid? OperatorId { get; set; }
        [Required]
        [DisplayName("Plate No")]
        public Guid FleetId { get; set; }
        public string ArrivalPlace { get; set; }
        public string EquipmentName { get; set; }
        [Required]
        public int Quantity { get; set; }
        public bool IsArrived { get; set; }

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
        [DisplayName("Equipment Status")]
        public string EquipmentStatus { get; set; }
        public string RequestDesc { get; set; }
        public decimal AsphaltKm { get; set; }
        public decimal GravelKm { get; set; }
        [DisplayName("Number (Hour / Km)")]
        public string NumberHourPerKm { get; set; }
        [DisplayName("Destination")]
        public string Destination { get; set; }
        [DisplayName("Number of Persons/Ton")]
        public decimal NumberofPersonOrTon { get; set; }

        public enum EquipmentStatusenum { New, Good, NeverMind }
        public bool IsAssigned { get; set; }

        public decimal StartOdometr { get; set; }
        public decimal EndOdometer { get; set; }
        public decimal NetOdometer { get; set; }
        public bool Returned { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReturnDate { get; set; }
        public int VehicleDemandId { get; set; } // Parent => RequestEquipment 
    }
    public partial class AssignedEquipmentDTO : Operations
    {
        public Guid AssignedEquipmentID { get; set; }
        public Guid RequestedEquipmentId { get; set; }
        [DisplayName("Equipment Name")]
        public Guid EquipmentNameID { get; set; }
        public Guid RequestNumber { get; set; }
        public string SentDate { get; set; }
        public string ArrivalDate { get; set; }
        public string ArrivalPlace { get; set; }
        [DisplayName("Operator Name")]
        public Guid? OperatorId { get; set; }
        [Required]
        [DisplayName("Plate No")]
        public Guid FleetId { get; set; }
        public string EquipmentName { get; set; }
        [Required]
        public int Quantity { get; set; }
        public bool IsArrived { get; set; }

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
        [DisplayName("Equipment Status")]
        public string EquipmentStatus { get; set; }
        public string RequestDesc { get; set; }
        public decimal AsphaltKm { get; set; }
        public decimal GravelKm { get; set; }
        [DisplayName("Number (Hour / Km)")]
        public string NumberHourPerKm { get; set; }
        [DisplayName("Destination")]
        public string Destination { get; set; }
        [DisplayName("Number of Persons/Ton")]
        public decimal NumberofPersonOrTon { get; set; }

        public enum EquipmentStatusenum { New, Good, NeverMind }
        public bool IsAssigned { get; set; }

        public decimal StartOdometr { get; set; }
        public decimal EndOdometer { get; set; }
        public decimal NetOdometer { get; set; }
        public bool Returned { get; set; }
        [DataType(DataType.DateTime)]
        [DisplayFormat(DataFormatString = "{0:dd-MM-yyyy}", ApplyFormatInEditMode = true)]
        public DateTime ReturnDate { get; set; }
        public int VehicleDemandId { get; set; } // Parent => RequestEquipment 
    }

    public class AssignedEquipmentAdditionalInfo
    {
        [Key]
        public int AssignedEquipmentAdditionalInfoId { get; set; }
        public Guid? AssignedEquipmentId { get; set; }
        [Display(Name = "Plate No")]
        public string PlateNo { get; set; }
        [Display(Name = "Km Traveled Per Liter")]
        public float KmTravelledPerLitre { get; set; }
        [Display(Name = "Km Traveled Total")]
        public float KmTravelledTotal { get; set; }
        [Display(Name = "Km Intersite")]
        public float KmInterSite { get; set; }
        [Display(Name = "Km Grand Total")]
        public float KmGrandTotal { get; set; }

        [Display(Name = "Req Fuel In Liter")]
        public float ReqFuelInLiter { get; set; }

        [Display(Name = "Total In Litre")]
        public float TotalInLitre { get; set; }
        [Display(Name = "From Deppot")]
        public float FromDeppot { get; set; }
        [Display(Name = "In Cash")]
        public decimal InCash { get; set; }
        [Display(Name = "From Project")]
        public float FromProject { get; set; }
        [Display(Name = "Oil Req For Engine")]
        public float OilReqForEngine { get; set; }
        [Display(Name = "Oil Req For Other")]
        public float OilReqForOther { get; set; }
        [Display(Name = "Grease Req")]
        public float GreaseReq { get; set; }
        [Display(Name = "Total")]
        public decimal TotalBirr { get; set; }
        [Display(Name = "Total Perdium")]
        public decimal TotalPerdium { get; set; }
        [Display(Name ="Fuel Rate")]
        public decimal FuelRate { get; set; }
        [Display(Name ="In Cash Liter")]
        public decimal InCashLiter { get; set; }
    }
}
