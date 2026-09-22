using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{
    public partial class RentFromExternal : Operations
    { 

        [ScaffoldColumn(false)]
        public Guid RentFromExternalID { get; set; }
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Num { get; set; }
        public Guid? ExpKey { get; set; } // self-ref when it expires
        public bool IsExpired { get; set; }
        public bool IsReplaced { get; set; }
        [Display(Name = "Operator")]
        public Guid? OperatorId { get; set; }
        public enum EquipmentTypeenum { DoubleCap, Dump, Track, Excavator }
        public enum FuelCoveredenum { Supplier, Customer }

        [DisplayName("Equipment Category")]
        public Guid EquipmentIdentity { get; set; }
        [DisplayName("Equipment Type")]
        public Guid EquipmentType { get; set; }

        [DisplayName("Supplier Name")]
        public int ProcurementSupplierID { get; set; }

        [DisplayName("Company Name")]
        public string CompanyName { get; set; }

        [DisplayName("Company Address")]
        public string CompanyAddress { get; set; }

        [DisplayName(" Phone Number")]
        public string CompanyPhno { get; set; }

        [DisplayName(" Email ")]
        public string CompanyEmail { get; set; }
        [DisplayName("Including VAT ")]
        public bool RentalRateIncludingVAT { get; set; }

        public string Status { get; set; }
        public int? Quantity { get; set; }
        [Required]
        [DisplayName("Fuel Covered By")]

        public string FuelCoveredBy { get; set; }

        [DisplayName("Project Location")]
        public string ProjectLocation { get; set; }
        [DisplayName("Project Location")]
        public int BranchId { get; set; }
        [Required]
        [DisplayName("Unit Of Work ")]
        public string UnitOfWorkMeasurement { get; set; }

        [DataType(DataType.MultilineText)]
        public string Description { get; set; }

        [DisplayName("Name Bank Branch")]
        public string NameBankBranch { get; set; }

        [DisplayName(" Bank Full Address")]
        public string BankFullAddress { get; set; }

        [DisplayName(" Bank Slip No")]
        public string BankSlipNo { get; set; }

        //[DataType(DataType.Currency)]
        [DisplayName("Advance Payment")]
        public int? AdvancePayment { get; set; }

        [Required]
        [DisplayName("Start Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime? StartDate { get; set; }
        //[Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        [DisplayName("End Date")]
        public DateTime? EndDate { get; set; }

        [NotMapped]
        [DisplayName("Total Cost")]

        public decimal TotalCost { get; set; }
        [NotMapped]
        [DisplayName("Date Difference")]
        public double DateDifference { get; set; }
       
        public double Hour { get; set; }
        [NotMapped]
        public string PlateNo { get; set; }
        [Display(Name = "Hp/Cc")]
        public string HP_CC { get; set; }
        public string Operator { get; set; } 
    }


    public class RentFromExternalEquipment : TrackUserSettingOperation
    {
        [Key]
        public int RentFromExternalEquipmentId { get; set; }
        public Guid EquipmentIdentityID { get; set; }
        [DisplayName("  Equipment Type ")]
        [Required]
        public Guid EquipmentTypeID { get; set; }
        [DisplayName(" Equipment Name ")]
        public Guid EquipmentNameID { get; set; }
        public bool IsContractTerminated { get; set; }

        public Guid FleetId { get; set; }
        [DisplayName("Rent Type")]
        public RentType RentType { get; set; }

        [DisplayName(" Rental Rate  ")]
        public decimal RentalRate { get; set; }
        public decimal PayableIdleHrPercent { get; set; }
        public decimal Vat { get; set; }
        //public decimal Tot { get; set; }
        public bool IsOnlineApproved { get; set; }
        public bool IsDumpTruck { get; set; }
        [DisplayName("Idle Rental Rate  ")]
        public decimal IdleRentalRate { get; set; }
        public Guid RentFromExternalId { get; set; }
        public string Status { get; set; }

        [NotMapped]
        public string EquipmentType { get; set; }

    }
    public class RentalEquipmentAdvancePayment : TrackUserSettingOperation
    {
        public int RentalEquipmentAdvancePaymentId { get; set; }
        public int RentFromExternalEquipmentId { get; set; }
        public decimal AdvancePayed { get; set; }
        public decimal VAT { get; set; }
        public decimal Tot { get; set; }
        public decimal AdvanceWithTax { get; set; }
        public string FleetId { get; set; }
        public decimal CostSheetAmount { get; set; }
        public bool IsFullPayment { get; set; }
    }
    public class RentFromExternalDocument : TrackUserOperation
    {
        public int RentFromExternalDocumentId { get; set; }
        public string RentFromExternalId { get; set; }
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string ShortPath { get; set; }
        public string FileExtension { get; set; }
        public string Description { get; set; }
    }

    public class RentFromExternalCustomerChange:TrackUserOperation
    {
        public int RentFromExternalCustomerChangeId { get; set; }
        public Guid RentFromExternalId { get; set; }
        [DisplayName("Supplier Name")]
        public int ProcurementSupplierID { get; set; }
        public bool IsCurent { get; set; }
    }
    public class RentFromExternalEquipmentRate : TrackUserSettingOperation
    {
        [Key]
        public int RentFromExternalEquipmentRateId { get; set; }
        public int RentFromExternalEquipmentId { get; set; } 
        public decimal RentalRate { get; set; }
        public decimal IdleRentalRate { get; set; }
        [DataType(DataType.Date)]
        public DateTime ManufacturingYearFrom { get; set; }
        [DataType(DataType.Date)]
        public DateTime ManufacturingYearTo { get; set; }
        public decimal DistanceFrom { get; set; }
        public decimal DistanceTo { get; set; }
        public DumpTruckTypeOfWork? DumpTruckTypeOfWork { get; set; }
        public FleetRoadType? RoadType { get; set; }

        [DisplayName("Rent Type")]
        public RentType RentType { get; set; }
        public string Description { get; set; }

    }
    public class HeavyORDumpTruck:TrackUserSettingOperation
    {
        public int HeavyORDumpTruckId { get; set;}
        public string Name { get; set; }
        public string Description { get; set; }

    }
    public class RoadORLoad : TrackUserSettingOperation
    {
        public int RoadOrLoadId { get; set; }
        public int HeavyORDumpTruckId { get; set; }
        public string Name { get; set;}
        public string Description { get; set; }
    }
    public class RateOfRoadORLoad:TrackUserSettingOperation
    {
        public int RateOfRoadORLoadId { get; set; }
        public int RoadOrLoadId { get; set; }
        public string Name { get; set; }
        public decimal Rate { get; set; }
    }

}