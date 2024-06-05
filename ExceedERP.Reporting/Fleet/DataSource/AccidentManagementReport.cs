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
    public class AccidentManagementReport
    {
        ExceedDbContext db = new ExceedDbContext();
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<OperatorDTO> GetOpersator()
        {
            List<OperatorDTO> operatorDtos = new List<OperatorDTO>();
            var operators = db.Operator.ToList();
            var fleetAccidents = db.Accident.ToList();

            var opByAccident = operators.GroupJoin(fleetAccidents,
                op => op.OperatorID,
                fa => fa.OperatorId,
                (driver, fleetAccident) => new
                {
                    Driver = driver,
                    FleetAccidents = fleetAccident
                }).ToList();

            foreach (var item in opByAccident)
            {
                if (item.FleetAccidents.Any())
                {
                    var operatorDto = new OperatorDTO
                    {
                        OperatorName = item.Driver.FullName,
                        NoOfAccidents = item.FleetAccidents.Count()
                    };

                    foreach (var fleetAccident in item.FleetAccidents)
                    {
                        var fleet = db.Fleets.FirstOrDefault(x => x.FleetsID == fleetAccident.FleetsID);
                        var accidentDto = new AccidentsDTO
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
                        operatorDto.Accidents.Add(accidentDto);
                    }
                    operatorDtos.Add(operatorDto);
                }
            }

            return operatorDtos.ToList();
        }

    }

    public class OperatorDTO
    {
        public string OperatorName { get; set; }
        public int NoOfAccidents { get; set; }
        public List<AccidentsDTO> Accidents { get; set; } = new List<AccidentsDTO>();

    }

    public class AccidentsDTO
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
