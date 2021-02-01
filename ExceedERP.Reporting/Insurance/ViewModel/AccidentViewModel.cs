using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Insurance.ViewModel
{
   public class AccidentViewModel
    {
        public string  Insured { get; set; }
      
        public string FullName { get; set; }
       
        public string Address { get; set; }
      
        public string Occupation { get; set; }
        public string PhoneNo { get; set; }
      
        public string PolicyNo { get; set; }
      
        public string RenewalDate { get; set; }
       
        public string InsuredVehicleType { get; set; }
        public string VehecleCC { get; set; }
        public string SeledaNo { get; set; }
  
        public string PurposeOfVehicle { get; set; }
       
        public string VehicleCarryingCapacity { get; set; }
     
        public string DriverFullName { get; set; }

        public string DriverPhoneNo { get; set; }
        
        public string DriverAddress { get; set; }
    
        public string DriverOccupation { get; set; }
      
        public string DriverAge { get; set; }
   
        public string DriverLicenseNo { get; set; }
     
        public string DriverGrade { get; set; }

        public string LicenseExpiryDate { get; set; }
       
        public string AccidentDate { get; set; }
        
        public string AccidentTime { get; set; }
     
        public string AccidentPlace { get; set; }

        public string SpeedOfVehicle { get; set; }
        public string DistanceFromEdge { get; set; }
        public string DescriptionOfAccident { get; set; }
        public string NameandAddressOfVehicleInvolved { get; set; }
        public string YorOpinionResponsibleForTheAccident { get; set; }
        public bool DoYouHaveMoreThanOneInsurance { get; set; }
    
        public bool DoYouHaveOtherInsuranceCanEarnForTheAccident { get; set; }
     
        public bool DoThePoliceRecoredIt { get; set; }
        public string WitnessInYourCar { get; set; }
        public string WitnessOutOfYourCar { get; set; }
        public string DescriptionForWhyNotTakeWitness { get; set; }
        
        public string OpionionResponsible { get; set; }
      
        public string PoliceIdNo { get; set; }
     
        public string WittnessDescription { get; set; }
      
        public string AssetDamage { get; set; }
     
        public string ThirdPartyDamage { get; set; }
      
        public string InjuredPersons { get; set; }
    }
}
