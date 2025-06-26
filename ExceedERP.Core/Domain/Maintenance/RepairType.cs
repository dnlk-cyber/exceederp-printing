using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Core.Domain.Maintenance
{
    public  class RepairType : TrackUserOperation
    {

        public Guid RepairTypeID { get; set; }
        [Required]
       
        [DisplayName("Category")]
        public string CategoryCode { get; set; }
        [DisplayName("Service Group")]
        public ServiceGroup ServiceGroup { get; set; }
       // public int ServiceGroupSettingId { get; set; }
        [Required]
        [DisplayName("Vehicle Category")]
        public string SubCategoryCode { get; set; }

        [Required]
        [DisplayName("Code ")]
        public string Code { get; set; }
        [Required]
        [DisplayName("Repair Type ")]
        public string Name { get; set; }
        public decimal Weight { get; set; }
        [Required]
        [DisplayName("Standared Time")]
        public decimal StandaredTime { get; set; }
        [DisplayName("Automotive Standared Hour")]
        public decimal AutomotiveStandaredHour { get; set; }
        [Required]
        [DisplayName("Payable Time")]
        public decimal PayableTime { get; set; }
        [Required]
        [DisplayName("Difference")]
        public decimal Difference { get; set; }
        [Required]
        [DisplayName("Labour Rate @Company")]
        public decimal LabourRateAtCompany { get; set; }

        [Required]
        [DisplayName("Labour Rate  @Customer")]
        public decimal LabourRateAtCustomer { get; set; }
        [Required]
        [DisplayName("Total Amount @Company")]
        public decimal TotalAmountCompany { get; set; }

        [Required]
        [DisplayName("Total Amount @Customer")]
        public decimal TotalAmountCustomer { get; set; }
        [DataType(DataType.MultilineText)]
        public string Remark { get; set; }
        [DisplayName("Why")]
        public string Reason { get; set; }
        [DisplayName("Where ( in which process )")]
        public string Place { get; set; }
        [DisplayName("When")]
        public string Time { get; set; }
        [DisplayName("How it is done")]
        public string Process { get; set; }
    }
    public enum ServiceGroup
    {
        [Description("Drive System")]
        [Display(Name = "Drive System")]
        Drive_System,
        [Description("Suspension system")]
        [Display(Name = "Suspension system")]
        Suspension_system,
        [Description("Clutch System")]
        [Display(Name = "Clutch System")]
        Clutch_System,
        [Description("Brake System")]
        [Display(Name = "Brake System")]
        Brake_System,
        [Description("Steering System")]
        [Display(Name = "Steering System")]
        Steering_System,
        [Description("Electrical System")]
        [Display(Name = "Electrical System")]
        Electrical_System,
        [Description("Fuel System")]
        [Display(Name ="Fuel System")]
        Fuel_System,
        [Description("Engine Mechanical System")]
        [Display(Name = "Engine Mechanical System")]
        Engine_Mechanical_System,
        [Description("Cabin, Seats & door lock Work")]
        [Display(Name = "Cabin, Seats & door lock Work")]
        Cabin_Seats_and_door_lock_Work,
        [Description("Exhaust System")]
        [Display(Name = "Exhaust System")]
        Exhaust_System,
        [Description("Cooling System")]
        [Display(Name = "Cooling System")]
        Cooling_System,
        [Description("Air conditioner & heating System")]
        [Display(Name = "Air conditioner & heating System")]
        Air_conditioner_and_heating_System,
        [Description("Basic Maintenance")]
        [Display(Name = "Basic Maintenance")]
        Basic_Maintenance,
        [Display(Name ="Hydrolic System"),Description("Hydrolic System")]
        Hydrolic_System,
        [Display(Name = "Pneumatic System"), Description("Pneumatic System")]
        Pneumatic_System,
        [Display(Name = "Transmission System"), Description("Transmission System")]
        Transmission_System,
        [Display(Name = "Wheels, Tires And Hubs"), Description("Wheels, Tires And Hubs")]
        Wheels_Tires_And_Hubs,
        [Display(Name = "Body and Frames"), Description("Body and frames")]
        Body_And_Frames,
        [Display(Name = "Track Drive Undercarriage System"), Description("Track Drive Undercarriage System")]
        TrackDrive_Undercarriage_System,
        [Display(Name = "Lubrication System"), Description("Lubrication System")]
        Lubrication_System,
        [Display(Name = "Fabrication and Modification"), Description("Fabrication and Modification")]
        Fabrication_And_Modification,
        [Display(Name = "10000 service"), Description("10000 service")]
        Ten_Thousand_Service,
        [Display(Name = "5000 service"), Description("5000 service")]
        Five_Thousand_Service,
        [Display(Name = "Mechanical Maintenance"), Description("Mechanical Maintenance")]
        Mechanical_Maintenance,
        [Display(Name = "Electrical Maintenance"), Description("Electrical Maintenance")]
        Electrical_Maintenance,
        [Display(Name = "10000 service and other"), Description("10000 service and other")]
        Ten_Thousand_Service_and_Other,
        [Display(Name = "5000 service and other"), Description("5000 service and other")]
        Five_Thousand_Service_and_Other,
        Feeder_Maintenance,
        Crusher_Blade_Maintenance,
        Conveyor_Belt_Maintenance,
        Screens_Maintenance,
        Dust_Collectors_Maintenance,
        Control_System_Maintenance,
        Cone_Maintenance,
        Jaw_Maintenance,
        Matle_Maintenance,
        Impacter_Liner_Maintenance,
        Sieve_Maintenance,
        Jaw_V_Belt_Maintenance,
        Roller_Maintenance,
        Jaw_Plate_Maintenance,
        Concrete_Mixer_Maintenance,
        Batching_Component,
        Cement_Silo,
        Screw_Conveyor,
        Control_Room,
        Control_Panels_And_Electrical_System,
        Dust_Collection_System,
        Silos_And_Storage,
        Water_Removing_Process,
        Bearing_Maintenance,
        Mixing_Drum_System,
        Mixing_Blade_Mixing_Arm_and_Mixing_shaft,
        
        Vibration_and_Noice_Maintenance,
        PLC_Failure_Maintenance,
        Seals_and_Gasket_Maintenance,
        Vibrating_Screen,
        Control_System,
        Bitumen_Tank,
        Drying_Drum,
        Mixing_Unit,
        Colds_Feeds_Bin,
        Conveyors,
        Burner,
        Dust_Collector,
        Aggregate_Elevetor,
        Cold_Aggregate_Feeder,
        Filler_Silo,
        Hot_Elevetor,
        Weghing_Bins,
        Asphalt_Storage,
        Drum_Mixer,
        Charging_Conevoyor,
        Aggregate_drying_and_heating,
        Emmission_control_component,
        Engine_overhaul,
        Alterternator_Maintenance,
        Voltage_regulator,
        Cooling_System_and_exhaust_system,
        Control_panel,
        Frame_Maintenance,
        Ignition_System,
        Starting_System,
        Impeller_System,
        Rotor_Maintenance,
        Seal_Maintenance,
        Compressor,
        Filter_System,
        Cleaner_System,

        Other,
    }
}
