
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using System.ComponentModel;

namespace ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent
{

    public enum CheckingType
    {
        Working, Stopped
    }
    public enum Type
    {
        New, Normal, Damaged
    }
    public enum Window
    {
        Good_Looking, Bad_Looking
    }
    public enum DoorWindow
    {
        Closable, UnClosable
    }
    public enum EngineStarting
    {
        Fast, Slow
    }
    public enum YesNo
    {
        Yes, No
    }

    public enum BrakeType
    {
        Quick, Slow, Slipping
    }
    public enum TireType
    {
        All_New, Slow, Some_Old, All_Old
    }

    public enum ChainType
    {
        All_New, Slow, Some_Old, All_Old, Not_Specified
    }
    public enum Excavator_Teeth
    {
        New, Old, SomeTime_Working
    }

    public class ExternalRentedEquipmentCheckList : Operations
    {

        public Guid ExternalRentedEquipmentCheckListId { get; set; }
        public Guid ExternalRentedEquipmentId { get; set; }
          [DisplayName(" Equipment Type ")]
        public string EquipmentType { get; set; }
            [DisplayName(" Horse Power ")]
        public string HorsePower { get; set; }
           [DisplayName(" MeterCube")]
        public string MeterCube { get; set; }
               [DisplayName(" SeatNo")]
        public string SeatNo { get; set; }
         [DisplayName(" Branch/Project")]
        public string Location { get; set; }
        public string Manufacturer { get; set; }
        public string Model { get; set; }
          [DisplayName(" Engine Manufacturer")]
        public string EngineManufacturer { get; set; }
         [DisplayName(" Manufacturer Date ")]
        public string ManufacturerDate { get; set; }
          [DisplayName(" Renting Company ")]
        public string RentingCompany { get; set; }
         [DisplayName(" Checked Renting Company ")]
        public string CheckedRentingCompany { get; set; }
        public string LibreNo { get; set; }
           [DisplayName(" Checked Renting WorkUnit ")]
        public string CheckingRequestingWorkUnit { get; set; }
         [DisplayName(" Checking Location ")]
        public string CheckingLocation { get; set; }
          [DisplayName("Excavator Teeth ")]
        public Excavator_Teeth ExcavatorTeeth { get; set; }
              [DisplayName("Chain Type")]
        public ChainType ChainType { get; set; }
          [DisplayName("Tire Type")]
        public TireType TireType { get; set; }
            [DisplayName("Air Conditioner")]
        public YesNo AirConditioner { get; set; }
          [DisplayName("Brake Type")]
        public BrakeType BrakeType { get; set; }
          [DisplayName("Electrical System")]
        public YesNo ElectricalSystem { get; set; }
          [DisplayName("Hydrolic Leak")]
        public YesNo HydrolicLeak { get; set; }
          [DisplayName(" Engine Starting")]
        public EngineStarting EngineStarting { get; set; }
         [DisplayName(" Engine Leak")]
        public YesNo EngineLeak { get; set; }
             [DisplayName(" Engine Smoke")]
        public YesNo EngineSmoke { get; set; }
             [DisplayName(" Checking Type")]
        public CheckingType CheckingType { get; set; }

    }
}