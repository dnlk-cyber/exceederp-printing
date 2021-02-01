using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
   public  class FuelDataSource
    {

        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetFuelType()
        {

            var result = EnumExtension.EnumToSelectList(typeof(ExceedERP.Core.Domain.FleetManagement.FuelManagement.Fuel.FuelTypeenum));

            return result;

        }
               [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetFleetStatus()
        {

            List<Helpers.Fleet.FleetBindingHelper.HelperObject> result = new List<Helpers.Fleet.FleetBindingHelper.HelperObject>();

            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = FleetViewModel.ACTIVE });
            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = FleetViewModel.ONDUTY });
            result.Add(new Helpers.Fleet.FleetBindingHelper.HelperObject { Name = FleetViewModel.OnMaintenance });

            return result;

        }
       [DataObjectMethod(DataObjectMethodType.Select)]
               public List<FuelViewModel> GetFuelIssues(string Identity, string category, string EquipmentName, string FuelType, string Manufacturer,
           DateTime startingDate, DateTime endingDate)
       {
           List<FuelViewModel> FleetList = new List<FuelViewModel>();
           FleetViewModel FleetViewModel;
           List<Fleets> Fleets;
           List<TireIssue> Issues;
          




           string AmharicName = "";
           string Name = "";
           string Address = "";
           Image LogoFileName = null;
           var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

           if (organization != null)
           {
               Name = organization.TradeName;
               AmharicName = organization.AmharicName;
               var address =
                   db.Addresses.Where(
                       a =>
                           a.Reference == organization.MainCompanyId.ToString() &&
                           a.ReferenceType.ToLower() == "company").ToList();
               if (address.Any())
               {
                   foreach (var a in address)
                   {

                       if (Address == null)
                       {
                           Address = a.Category + ":- " + a.Value;
                       }
                       else
                       {
                           Address += ", " + a.Category + "-" + a.Value;
                       }

                   }

               }
               var logo =
                   db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                           a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
               if (logo != null)
               {
                   LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
               }
           }



           if (!string.IsNullOrEmpty(Identity))
           {
               if (!string.IsNullOrEmpty(category))
               {
                   Fleets =
                       db.Fleets.Where(
                           so =>
                               so.EquipmentIdentityID.ToString() == Identity &&
                               so.EquipmentTypeID.ToString() == category)
                           .ToList();
               }
               else
               {
                   Fleets =
                       db.Fleets.Where(so => so.EquipmentIdentityID.ToString() == Identity)
                           .ToList();
               }
           }
           else
           {
               if (!string.IsNullOrEmpty(Manufacturer))
               {
                   Fleets = db.Fleets.Where(so => so.ManufacturerYear == Manufacturer).ToList();
               }
               else
               {
                   Fleets = db.Fleets.ToList();
               }
           }

           if (!string.IsNullOrEmpty(EquipmentName))
           {
               Fleets = Fleets.Where(so => so.EquipmentName == EquipmentName).ToList();
           }
           if (Fleets.Any())
           {
               List<Fleets> List = Fleets;
               foreach (Fleets sov in List)
               {

                   
                   List<Fuel> fuels = db.Fuel.Where(x => x.FleetsID == sov.FleetsID && x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();

                   if (!string.IsNullOrEmpty(FuelType))
                   {
                       fuels = fuels.Where(so => so.FuelType == FuelType).ToList();
                   }
                   
                   if (fuels.Any())
                   {
                     var  FuelViewModel = new FuelViewModel();

                     FuelViewModel.UnitPrice = decimal.Parse( fuels.Sum(x => x.UnitPrice).ToString());
                     FuelViewModel.FinalKmHr = decimal.Parse((fuels.Sum(x => x.FinalKmHr) - fuels.Sum(x => x.InitialKmHr)).ToString());
                     FuelViewModel.PlanKmHr = int.Parse(fuels.Sum(x => x.PlanKmHr).ToString());
                     FuelViewModel.FuelFilledAmount =decimal.Parse( fuels.Sum(x => x.FuelFilledAmount).ToString());
                     FuelViewModel.FuelType = sov.TypeOfFuel;
                     FuelViewModel.Description = sov.PlateNumber +" - "+sov.EquipmentName;
                     FuelViewModel.TotalFuelcost = decimal.Parse((fuels.Sum(x => x.FuelFilledAmount) * fuels.Sum(x => x.UnitPrice)).ToString());

                       FuelViewModel.AmharicName = AmharicName;
                       FuelViewModel.Name = Name;
                       FuelViewModel.LogoFileName = LogoFileName;
                       FuelViewModel.Address = Address;

                       FleetList.Add(FuelViewModel);
                   }
               }
           }

           return FleetList;
       }


       

    }
}
