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
        [Required]
        [DisplayName("Working Capacity")]
        public string WorkingCapacity { get; set; }

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

        [DataType(DataType.Currency)]
        [DisplayName("Advance Payment")]
        public Nullable<int> AdvancePayment { get; set; }

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

        public Guid FleetId { get; set; }
        [DisplayName("Rent Type")]
        public RentType RentType { get; set; }

        [DisplayName(" Rental Rate  ")]
        public decimal RentalRate { get; set; }
        [DisplayName("Idle Rental Rate  ")]
        public decimal IdleRentalRate { get; set; }
        //fk
        public Guid RentFromExternalId { get; set; }
        public string Status { get; set; }

    }
}