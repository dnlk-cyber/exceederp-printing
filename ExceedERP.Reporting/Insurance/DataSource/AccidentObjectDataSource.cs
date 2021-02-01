using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Insurance.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Insurance.DataSource
{


    [DataObject]
    public class AccidentObjectDataSource
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<AccidentViewModel> GetAccident()
        {
            List<AccidentViewModel> accidentList = new List<AccidentViewModel>();

            var accidents = db.Accident.ToList();
            foreach(var accident in accidents)
            {
                AccidentViewModel accid = new AccidentViewModel();
                accid.Insured = accident.Insured;
                accid.FullName = accident.FullName;
                accid.Address = accident.Address;
                accid.PhoneNo = accident.PhoneNo;
                accid.Occupation = accident.Occupation;
                accid.PolicyNo = accident.PolicyNo;
                accid.RenewalDate = accident.RenewalDate.ToString();
                //accid.InsuredVehicleType = accident.;
                //accid.VehecleCC = accident.CC;
                //accid.Manufacture = accident.m;
                accid.SeledaNo = accident.PlateNo;
                //accid.PurposeOfVehicle = accident.purpose;
                //accid.DriverFullName = accident.;
                accid.DriverAddress = accident.Insured;
                accid.DriverAge = accident.Insured;
                accid.DriverPhoneNo = accident.Insured;
                accid.DriverOccupation = accident.Insured;
                accid.DriverLicenseNo = accident.Insured;
                accid.DriverGrade = accident.Insured;
                accid.LicenseExpiryDate = accident.Insured;
                accid.AccidentDate = accident.Insured;
                accid.AccidentTime = accident.Insured;
                accid.AccidentPlace = accident.Insured;
                accid.SpeedOfVehicle = accident.Insured;
                accid.DistanceFromEdge = accident.Insured;
                accid.DescriptionOfAccident = accident.Description;
                //accid.NameandAddressOfVehicleInvolved = accident.Inv;
                accid.OpionionResponsible = accident.OpionionResponsible;
                accid.DoYouHaveMoreThanOneInsurance = accident.DoYouHaveMoreThanOneInsurance;
                accid.DoYouHaveOtherInsuranceCanEarnForTheAccident = accident.DoYouHaveOtherInsuranceCanEarnForTheAccident;
                accid.DoThePoliceRecoredIt = accident.DoThePoliceRecoredIt;
                accid.WitnessInYourCar = accident.WitnessInYourCar;
                accid.WitnessOutOfYourCar = accident.WitnessOutOfYourCar;
                accid.DescriptionForWhyNotTakeWitness = accident.DescriptionForWhyNotTakeWitness;
                accid.AssetDamage = accident.AssetDamage;
                accid.ThirdPartyDamage = accident.ThirdPartyDamage;
                accid.WittnessDescription = accident.WittnessDescription;



                accidentList.Add(accid);
            }
            return accidentList;
        }
    }
}