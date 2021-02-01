using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Core.Domain.FleetManagement
{
    public enum ReadingType
    {
        KM, HR
    }
    public enum TransferMode
    {
        ByItSelf, Loaded
    }
    public enum OwningandOperatingCostConstantType
    {
        Under_Carrage_Life, Resdual_Scrap, Numerator, Denomator, Interest_Rate, Insurance_Rate, Property_Rate, Fuel_UnitPrice, SparePart_Rate, Lubricant_Rate, Tyre_Life, Repair_Factor, Maintenace_Repair_Depreciation, SpecialWearItemRate, Operator_Hours, Operator_Days
    }


    public enum EquipmentCondition
    {
        Old, New, Maintainable, Not_Maintainable
    }
    public enum FuelAmount
    {
        Full, ThreeFourth, Half, Quarter
    }
    public enum HydrolicAmount
    {
        Full, ThreeFourth, Half, Quarter, None
    }
    public enum UOM
    {
        Pcs, Lit
    }

    public enum CheckListLogType
    {
        Registration, SiteTransfer, DriverTransfer, MaintenaceInspection,
    }
    public enum Weeks
    {
        First,
        Second,
        Third,
        Fourth
    }
    public enum SpareType
    {
        Spare,
        Oil_Lubricant,
        Tyre,
        Battery,
        Other
    }

    public enum WeekDays
    {
        Monday,
        Tuesday,
        Wednesday,
        Thursday,
        Friday
    }
    public enum FleetPlacmentType
    {
        OPERATOR,
        DRIVER,
        MECHANIC

    }
    public enum RentType
    {
        Hourly,
        KM,
        MeterCubePerKm,
        Repetition,
        Hectar
    }
    public enum CheckListType
    {
        Engine_Parts,
        Rear_Axle,
        Steering_System,
        Cooling_System,
        Cab_And_Body,
        Fuel_System,
        MOTOR,
        Front_Axle,
        Gauges,
        Chassis_and_Suspension,
        Electrical_And_Lightning_Sytem,
        Attachements,
        Brakes

    }
}
