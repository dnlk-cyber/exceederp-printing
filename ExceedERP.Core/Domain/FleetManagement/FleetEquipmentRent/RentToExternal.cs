
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
 using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{


  
    public partial class RentToExternal : Operations
    {
          //[ScaffoldColumn(false)]
         public Guid RentToExternalID { get; set; }
          [DisplayName("  Equipment Category ")]
          public Guid? EquipmentIdentityID { get; set; }
          [DisplayName("  Equipment Type ")]
          public Guid? EquipmentTypeID { get; set; }
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
         public Nullable<int> Quantity  {get; set;}
         [Required]
         [DisplayName("Fuel Covered By")]
        
         public string FuelCoveredBy  {get; set;}

              [DisplayName("Equipment Name")]
        
         public string EquipmentName  {get; set;}
         
        //[Required]
        [DisplayName("Branch/Project")]
         public string ProjectLocation { get; set; }
        [Required]
        [DisplayName("Unit Of Work ")]
        public string UnitOfWorkMeasurement { get; set; }
         public string Description { get; set; }
        [Required]
        public string Condition { get; set; }
        [DisplayName("Company Name")]
        public string NameBankBranch { get; set; }
        
        [DisplayName(" Bank Full Address")]
        public string BankFullAddress { get; set; }
     
        [DisplayName(" Bank Slip No")]
        public string BankSlipNo { get; set; }
        [DataType(DataType.Currency)]

        [DisplayName(" Advance Payment")]
        public Nullable<int> AdvancePayment { get; set; }

        [DataType(DataType.Date)]
        public DateTime From { get; set; }
        [DataType(DataType.Date)]
        public DateTime To { get; set; }


    }
}