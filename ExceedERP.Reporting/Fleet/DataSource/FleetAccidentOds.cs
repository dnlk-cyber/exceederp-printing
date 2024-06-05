using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class FleetAccidentOds
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetAccidentViewModel> GetFleetAccident()
        {
            List<FleetAccidentViewModel> list = new List<FleetAccidentViewModel>();

            var fleetAccidentList = db.Accident.ToList();

            foreach (var fleetAccident in fleetAccidentList)
            {
                var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetAccident.FleetsID);

                FleetAccidentViewModel fleetAccidentViewModel = new FleetAccidentViewModel
                {
                    AccidentLocation = fleetAccident.AccidentLocation,
                    Accountablebody = fleetAccident.Accountablebody,
                    AssetDamage = fleetAccident.AssetDamage,
                    CauseOfAccident = fleetAccident.CauseOfAccident,
                    CostcoveredbyOtherBodies = fleetAccident.CostcoveredbyOtherBodies,
                    Description = fleetAccident.Description,
                    InjuredPersons = fleetAccident.InjuredPersons,
                    IsHornHeared = fleetAccident.IsHornHeared ? "Yes" : "No",
                    LevelOfDamage = fleetAccident.LevelOfDamage,
                    PersonWasAtAccident = fleetAccident.PersonWasAtAccident ? "Yes" : "No",
                    PlateNumber = fleet?.PlateNumber ?? "Not Assigned",
                    PolicyIdNo = fleetAccident.PolicyIdNo,
                    ThirdPartyDamage = fleetAccident.ThirdPartyDamage,
                    VehicleDistanceFromRoadEdge = fleetAccident.VehicleDistanceFromRoadEdge,
                    VehicleSpeed = fleetAccident.VehicleSpeed,
                    WitnessInYourCar = fleetAccident.WitnessInYourCar,
                    WittnessDescription = fleetAccident.WittnessDescription
                    
                };
                list.Add(fleetAccidentViewModel);
            }
            return list;
        }
    }
    public class FleetAccidentViewModel
    {
        public string PlateNumber { get; set; }
        public string AccidentLocation { get; set; }
        public string PersonWasAtAccident { get; set; }
        public string Description { get; set; }
        public int? CostcoveredbyOtherBodies { get; set; }
        public string WitnessInYourCar { get; set; }
        public string LevelOfDamage { get; set; }
        public string CauseOfAccident { get; set; }
        public string IsHornHeared { get; set; }
        public decimal VehicleDistanceFromRoadEdge { get; set; }
        public string PolicyIdNo { get; set; }
        public string AssetDamage { get; set; }
        public string Accountablebody { get; set; }
        public string ThirdPartyDamage { get; set; }
        public decimal VehicleSpeed { get; set; }
        public string InjuredPersons { get; set; }
        public string WittnessDescription { get; set; }
    }


}
