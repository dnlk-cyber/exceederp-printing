using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{



    public partial class RentOtherSuppliers : Operations
    {
          [ScaffoldColumn(false)]
         public Guid RentOtherSuppliersID { get; set; }
          [DisplayName("  Equipment Category ")]
          [Required]
          public Guid EquipmentIdentityID { get; set; }
          [DisplayName("  Equipment Type ")]
          [Required]
          public Guid EquipmentTypeID { get; set; }
         [Required]
         [DisplayName("Working Capacity")]
         public string WorkingCapacity { get; set; }
         [Required]
         [DisplayName("Including VAT ")]
         public bool RentalRateIncludingVAT { get; set; }
         [DisplayName("With Driver/Operator ")]
         public bool WithDriver { get; set; }
          [DisplayName("Good")]
         public bool GoodCondition { get; set; }
          [DisplayName("Normal")]
         public bool NormalCondition { get; set; }
        [DisplayName("Bad")]
         public bool BadCondition { get; set; }
         [DisplayName("Location/Address")]
         public string Address { get; set; }
         public string Status  {get; set;}
         public Nullable<int> Quantity  {get; set;}
         [Required]
         [DisplayName("Fuel Covered By")]
        
         public string FuelCoveredBy  {get; set;}

              [Required]
              [DisplayName("Equipment Name")]
        
         public string EquipmentName  {get; set;}
         
        [Required]
        [DisplayName("Branch/Project")]
         public string ProjectLocation { get; set; }
        [Required]
        [DisplayName("Unit Of Work ")]
        public string UnitOfWorkMeasurement { get; set; }
        [Required]
         public string Description { get; set; }
        [Required]
        public string Condition { get; set; }
        [DisplayName("Company Name")]
        public string NameBankBranch { get; set; }
        
        [DisplayName(" Bank Full Address")]
        public string BankFullAddress { get; set; }
     
        [DisplayName(" Bank Slip No")]
        public string BankSlipNo { get; set; }
        [Required]
        [DataType(DataType.Currency)]

        [DisplayName(" Advance Payment")]
        public Nullable<int> AdvancePayment { get; set; }

         
     
    }
}