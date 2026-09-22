

using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.FleetManagement.FleetRegistration
{
    public partial class Accident : Operations
    {
        public Guid AccidentID { get; set; }
        [DisplayName("  Equipment ")]
        public Guid FleetsID { get; set; }
        [DisplayName("Date Of Accident")]
        [Required]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]

        public DateTime DateOfOccurrence { get; set; }
        [DisplayName("Cause Of Accident")]
        [DataType(DataType.MultilineText)]
        [Required]
        public string CauseOfAccident { get; set; }
        [DisplayName(" Level Of Damage")]
        [Required]
        public string LevelOfDamage { get; set; }
        public enum LevelOfDamageenum { Minor, Major, TotalDamage }
        [DisplayName(" Accountable body")]
        [Required]
        public string Accountablebody { get; set; }
        [DisplayName("Accident Location")]
        [Required]
        public string AccidentLocation { get; set; }
        [DisplayName("License No ")]

        public string LicenseNo { get; set; }
        public string Policereport { get; set; }
        public string Selectreport { get; set; }
        [DataType(DataType.MultilineText)]
        public string Description { get; set; }
        [DisplayName(" Is Horn Heared ")]
        public bool IsHornHeared { get; set; }
        [DisplayName("Person Was At Accident")]
        public bool PersonWasAtAccident { get; set; }
        [DisplayName("Vehicle Speed")]
        public decimal VehicleSpeed { get; set; }
        [DisplayName("Vehicle Distance From RoadEdge")]
        public decimal VehicleDistanceFromRoadEdge { get; set; }
        [DisplayName("Cost Covered by Insurance")]
        // [Required]
        [DataType(DataType.Currency)]
        public Nullable<int> CostcoveredbyInsurance { get; set; }
        [DisplayName("Cost Covered by Company")]
        //  [Required]
        [DataType(DataType.Currency)]
        public Nullable<int> CostcoveredbyCompany { get; set; }
        [DisplayName("Cost Covered by Operator")]
        //[Required]
        [DataType(DataType.Currency)]
        public Nullable<int> CostcoveredbyOperator { get; set; }
        [DisplayName("Covered by Other Bodies")]
        //   [Required]
        [DataType(DataType.Currency)]
        public Nullable<int> CostcoveredbyOtherBodies { get; set; }

        [NotMapped]
        public string AssetCode { get; set; }
        [NotMapped]
        public string PlateNo { get; set; }
        [NotMapped]
        public string SerialNo { get; set; }
        [NotMapped]
        public string ChasisNo { get; set; }


        [ForeignKey("FleetsID")]
        public virtual Fleets Fleet { get; set; }
        [NotMapped]
        public string Equipment { get; set; }
        [Display(Name ="Insured Name")]
        public string Insured { get; set; }
        [Display(Name ="Full Name")]
        public string FullName { get; set; }
        public string Address { get; set; }
        public string Occupation { get; set; }
        public string PhoneNo { get; set; }
        [DisplayName("Policy No")]
        public string PolicyNo { get; set; }
        //[DisplayName("Renewal Date")]
        [DataType(DataType.Date)]
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        public DateTime? RenewalDate { get; set; }
        [DisplayName("Have Morethan One Insurance?")]
        public bool DoYouHaveMoreThanOneInsurance { get; set; }
        [DisplayName("Have Other Insurance Can Earn?")]
        public bool DoYouHaveOtherInsuranceCanEarnForTheAccident { get; set; }
        [DisplayName("Do the Police Recored?")]
        public bool DoThePoliceRecoredIt { get; set; }
        [DisplayName("Witness In Your Car")]
        public string WitnessInYourCar { get; set; }
        [DisplayName("Witness Our Of Your Car")]
        public string WitnessOutOfYourCar { get; set; }
        public string DescriptionForWhyNotTakeWitness { get; set; }

        [DisplayName("Covered by Other Bodies")]
        public string OpionionResponsible { get; set; }
        [DisplayName("Policy Id")]
        public string PolicyIdNo { get; set; }
        [DisplayName("Wittness Description")]
        public string WittnessDescription { get; set; }
        [DisplayName("Asset Damage")]
        public string AssetDamage { get; set; }
        [DisplayName("ThirdParty Damage")]
        public string ThirdPartyDamage { get; set; }
        
        [DisplayName("Injured Persons")]
        public string InjuredPersons { get; set; }
        [DisplayName("Current Value")]
        public string CurrentValue { get; set; }
        public string RenewalDateAmh { get; set; }
        [NotMapped]
        public string Model { get; set; }
        [NotMapped]
        public decimal PurchaseCost { get; set; }
        [NotMapped]
        public string EngineNo { get; set; }
        [NotMapped]
        public string ManufacturedYear { get; set; }
        [Display(Name ="Driver Name")]
        public Guid? OperatorId { get; set; }
        [NotMapped]
        public int EmployeeId { get; set; } 
       
        [Display(Name ="Time of Accident")]
        public DateTime TimeOfAccident { get; set; }

        [Display(Name ="Other Pary FullName")]
        public string OtherPartyFullName { get; set; }
        [Display(Name ="Other Party Address")]
        public string OtherPartyAddress { get; set; }
        [DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}")]
        [Display(Name ="Police Report Date"), DataType(DataType.DateTime)]
        public DateTime PoliceReportDate { get; set; }
        [Display(Name ="Police Report Address")]
        public string PoliceReportAddress { get; set; }


    }

}