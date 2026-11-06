using ExceedERP.Core.Domain.Common;
using System;
using System.ComponentModel.DataAnnotations;

namespace ExceedERP.Core.Domain.FleetManagement.FuelManagement
{
    /// <summary>
    /// Fuel request raised by Fleet and forwarded to Finance for approval and Telebirr payment.
    /// </summary>
    public class FuelRequest : NewOperations
    {
        public int FuelRequestId { get; set; }

        public int RequestorId { get; set; }
        public string RequestorName { get; set; }
        [Required]
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }

       

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        [Display(Name = "Division")]
        public int? DivisionId { get; set; }

        [Display(Name = "Division Name")]
        public string DivisionName { get; set; }
        public string RecieptNo { get; set; }

        //[Display(Name = "Opening KM")]
        //public decimal OpeningKM { get; set; }

        //[Display(Name = "Closing KM")]
        //public decimal ClosingKM { get; set; }

        //[Display(Name = "Distance KM")]
        //public decimal DistanceKM { get; set; }

        //[Display(Name = "Fuel Consumed (L)")]
        //public decimal FuelConsumedL { get; set; }

        //[Display(Name = "Fuel Filled (L)")]
        //public decimal FuelFilledL { get; set; }

        //[Display(Name = "Consumption KM/L")]
        //public decimal ConsumptionKMPerL { get; set; }

        //[Display(Name = "Operational Days")]
        //public int OperationalDays { get; set; }

        //[Display(Name = "Unit Price")]
        //public decimal UnitPrice { get; set; }


        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Fuel Owner / Station")]
        public string FuelOwnerName { get; set; }

        [Display(Name = "Telebirr Phone No")]
        [RegularExpression(@"^(\+?251|0)?9\d{8}$", ErrorMessage = "Enter a valid Ethiopian mobile number (e.g. 09xxxxxxxx).")]
        public string PhoneNumber { get; set; }

        // Finance stage
        [Display(Name = "Finance Approved")]
        public bool IsFinanceApproved { get; set; }
        public string FinanceApprovedBy { get; set; }
        public DateTime? FinanceApprovedTime { get; set; }
        [Display(Name = "Rejection Reason")]
        public string FinanceRejectionReason { get; set; }
    }
    public class FuelRequestDetail:TrackUserSettingOperation
    {
        public int FuelRequestDetailId { get; set; }
        public int FuelRequestId { get; set; }
        [Required]
        [Display(Name = "Plate / Serial")]
        public Guid FleetId { get; set; }

        [Display(Name = "Plate No.")]
        public string PlateNo { get; set; }

        [Display(Name = "Vehicle Type")]
        public Guid? VehicleTypeId { get; set; }

        [Display(Name = "Vehicle Type")]
        public string VehicleType { get; set; }

        [Display(Name = "Side No")]
        public string SideNo { get; set; }

        [Display(Name = "Driver")]
        public Guid? OperatorId { get; set; }

        [Display(Name = "Driver")]
        public string Driver { get; set; }

        [Required]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }

        [Display(Name = "Department Name")]
        public string DepartmentName { get; set; }

        [Display(Name = "Division")]
        public int? DivisionId { get; set; }
        public string RecieptNo { get; set; }

        [Display(Name = "Division Name")]
        public string DivisionName { get; set; }

        [Display(Name = "Opening KM")]
        public decimal OpeningKM { get; set; }

        [Display(Name = "Closing KM")]
        public decimal ClosingKM { get; set; }

        [Display(Name = "Distance KM")]
        public decimal DistanceKM { get; set; }

        [Display(Name = "Fuel Consumed (L)")]
        public decimal FuelConsumedL { get; set; }

        [Display(Name = "Fuel Filled (L)")]
        public decimal FuelFilledL { get; set; }
        [Required]
        [Display(Name = "Request Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime RequestDate { get; set; }

        [Display(Name = "Consumption KM/L")]
        public decimal ConsumptionKMPerL { get; set; }

        [Display(Name = "Operational Days")]
        public int OperationalDays { get; set; }

        [Display(Name = "Unit Price")]
        public decimal UnitPrice { get; set; }

        [Display(Name = "Total Amount")]
        public decimal TotalAmount { get; set; }

        [Display(Name = "Fuel Owner / Station")]
        public string FuelOwnerName { get; set; }

        [Display(Name = "Telebirr Phone No")]
        [RegularExpression(@"^(\+?251|0)?9\d{8}$", ErrorMessage = "Enter a valid Ethiopian mobile number (e.g. 09xxxxxxxx).")]
        public string FuelOwnerPhoneNumber { get; set; }

        // Finance stage
        [Display(Name = "Finance Approved")]
        public bool IsFinanceApproved { get; set; }
        public string FinanceApprovedBy { get; set; }
        public DateTime? FinanceApprovedTime { get; set; }
        [Display(Name = "Rejection Reason")]
        public string FinanceRejectionReason { get; set; }
    }
    public class FuelRequestDocument
    {
        public int FuelRequestDocumentId { get; set; }
        public int FuelRequestId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }
    public static class FuelRequestStatus
    {
        public const string Pending = "Pending";
        public const string SentToFinance = "Sent To Finance";
        public const string FinanceApproved = "Finance Approved";
        public const string Rejected = "Rejected";
    }
}
