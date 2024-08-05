using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Helpers.Fleet
{

    public static class FleetGenericHelper
    {

      

        public static string GetApprovalStatus<T>(this T Obj) where T : Operations
        {

            if (Obj.IsSendForApproval == false && Obj.IsOnlineApproved == false)
            {

                return "Editing";
            }

            else if (Obj.IsSendForApproval == true && Obj.IsOnlineApproved == false)
            {

                return "Waiting For Approval";
            }

            else if (Obj.IsOnlineApproved == true)
            {

                return "Approved";
            }

            return string.Empty;
        }
        
        public static string DefaultValue(this string Value)
        {
            if (string.IsNullOrWhiteSpace(Value))
            {
                return "Not Assigned";
            }
            return Value;
        }

        public static string IfNullDefaultValue(this string Value)
        {
            if (string.IsNullOrWhiteSpace(Value) || Value == "Not Assigned")
            {
                return string.Empty;
            }
            return Value;
        }
        public static decimal DivideByZero(decimal Value)
        {
            if (Value == decimal.Zero)
            {
                return decimal.One;
            }
            return Value;
        }
        public static decimal TranslateBnOneAndHundered(this decimal Value)
        {
            if (Value < decimal.Zero)
            {
                return decimal.Zero;
            }
            if (Value > 100)
            {
                return 100;
            }
            return Value;
        }


        public static bool FilterSingleFleet(Guid FleetId, string EquipmentCategory, string EquipmentType, string PlateNumber, string ChasisNumber, string SerialNumber, string AssetNumber, string Location, string Model)
        {
            IFleetService _FleetService = new FleetService();
            var Fleets = _FleetService.GetAll().Where(x => x.FleetsID == FleetId).ToList();

            if (!string.IsNullOrWhiteSpace(PlateNumber))
            {
                Fleets = Fleets.Where(x => x.PlateNumber == PlateNumber).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Model))
            {
                Fleets = Fleets.Where(x => x.Model == Model).ToList();
            }

            if (!string.IsNullOrWhiteSpace(ChasisNumber))
            {
                Fleets = Fleets.Where(x => x.ChasisNumber == ChasisNumber).ToList();
            }

            if (!string.IsNullOrWhiteSpace(EquipmentCategory))
            {
                Fleets = Fleets.Where(x => x.EquipmentIdentityID.ToString() == EquipmentCategory).ToList();
            }

            if (!string.IsNullOrWhiteSpace(EquipmentType))
            {
                Fleets = Fleets.Where(x => x.EquipmentTypeID.ToString() == EquipmentType).ToList();
            }
            if (!string.IsNullOrWhiteSpace(SerialNumber))
            {
                Fleets = Fleets.Where(x => x.SerialNo == SerialNumber).ToList();
            }
            if (!string.IsNullOrWhiteSpace(AssetNumber))
            {
                Fleets = Fleets.Where(x => x.UnitNum == AssetNumber).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Location))
            {
                Fleets = Fleets.Where(x => x.Location == Location).ToList();
            }

            if (Fleets.Any())
            {
                return true;

            }
            else
            {
                return false;
            }

        }

        public static List<Fleets> FilterFleet(this List<Fleets> Fleets, string EquipmentCategory, string EquipmentType, string EquipmentName,string PlateNumber, string ChasisNumber, string SerialNumber, string AssetNumber, string Location, string Model)
        {


            if (!string.IsNullOrWhiteSpace(PlateNumber))
            {
                Fleets = Fleets.Where(x => x.PlateNumber == PlateNumber).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Model))
            {
                Fleets = Fleets.Where(x => x.Model == Model).ToList();
            }

            if (!string.IsNullOrWhiteSpace(ChasisNumber))
            {
                Fleets = Fleets.Where(x => x.ChasisNumber == ChasisNumber).ToList();
            }

            if (!string.IsNullOrWhiteSpace(EquipmentCategory))
            {
                Fleets = Fleets.Where(x => x.EquipmentIdentityID.ToString() == EquipmentCategory).ToList();
            }

            if (!string.IsNullOrWhiteSpace(EquipmentType))
            {
                Fleets = Fleets.Where(x => x.EquipmentTypeID.ToString() == EquipmentType).ToList();
            }
            if (!string.IsNullOrWhiteSpace(EquipmentName))
            {
                Fleets = Fleets.Where(x => x.EquipmentName.ToString() == EquipmentName).ToList();
            }
            if (!string.IsNullOrWhiteSpace(SerialNumber))
            {
                Fleets = Fleets.Where(x => x.SerialNo == SerialNumber).ToList();
            }
            if (!string.IsNullOrWhiteSpace(AssetNumber))
            {
                Fleets = Fleets.Where(x => x.UnitNum == AssetNumber).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Location))
            {
                Fleets = Fleets.Where(x => x.Location == Location).ToList();
            }

            return Fleets;
        }


        public static Fleets FilterSingleFleet(this List<Fleets> Fleets, string EquipmentCategory, string EquipmentType, string EquipmentName, string PlateNumber, string ChasisNumber, string SerialNumber, string AssetNumber, string Location, string Model)
        {


            if (!string.IsNullOrWhiteSpace(PlateNumber))
            {
                Fleets = Fleets.Where(x => x.PlateNumber == PlateNumber).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Model))
            {
                Fleets = Fleets.Where(x => x.Model == Model).ToList();
            }

            if (!string.IsNullOrWhiteSpace(ChasisNumber))
            {
                Fleets = Fleets.Where(x => x.ChasisNumber == ChasisNumber).ToList();
            }

            if (!string.IsNullOrWhiteSpace(EquipmentCategory))
            {
                Fleets = Fleets.Where(x => x.EquipmentIdentityID.ToString() == EquipmentCategory).ToList();
            }

            if (!string.IsNullOrWhiteSpace(EquipmentType))
            {
                Fleets = Fleets.Where(x => x.EquipmentTypeID.ToString() == EquipmentType).ToList();
            }
            if (!string.IsNullOrWhiteSpace(EquipmentName))
            {
                Guid.TryParse(EquipmentName, out Guid id);
                Fleets = Fleets.Where(x => x.EquipmentNameID == id).ToList();
            }
            if (!string.IsNullOrWhiteSpace(SerialNumber))
            {
                Fleets = Fleets.Where(x => x.SerialNo == SerialNumber).ToList();
            }
            if (!string.IsNullOrWhiteSpace(AssetNumber))
            {
                Fleets = Fleets.Where(x => x.UnitNum == AssetNumber).ToList();
            }
            if (!string.IsNullOrWhiteSpace(Location))
            {
                Fleets = Fleets.Where(x => x.Location == Location).ToList();
            }

            return Fleets.FirstOrDefault();
        }
    }

}
