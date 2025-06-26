using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetTireManagement;
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
using ExceedERP.Helpers.Fleet;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
   public class TireDataSource
    {

        ExceedDbContext db = new ExceedDbContext();



        public List<SelectListItem> GetTireType()
        {

            var result = EnumExtension.EnumToSelectList(typeof(ExceedERP.Core.Domain.FleetManagement.FleetTireManagement.TireIssue.TireTypeenum));

            return result;

        }
       
        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetTireOrBatteryType()
        {

            var result = EnumExtension.EnumToSelectList(typeof(ExceedERP.Core.Domain.FleetManagement.FleetTireManagement.FleetBatteryOrTyreType));

            return result;

        }
      
        public List<SelectListItem> GetTireTube()
        {

            var result = EnumExtension.EnumToSelectList(typeof(ExceedERP.Core.Domain.FleetManagement.FleetTireManagement.TireIssue.TireTubeenum));

            return result;

        }

       [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TireViewModel> GetTireList(string Location, string Identity, string category, string EquipmentName, string Manufacturer, string tyretype, string tyretube,
           DateTime startingDate, DateTime endingDate, string tyreOrBattery)
       {
           List<TireViewModel> TireList = new List<TireViewModel>();
           TireViewModel FleetViewModel;
           List<Fleets> Fleets;
           IFleetService _FleetService = new FleetService();
      



           #region SharedResource
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
           #endregion



           Fleets = _FleetService.GetAll().FilterFleet(Identity, category, EquipmentName, string.Empty, string.Empty, string.Empty, string.Empty, Location, string.Empty);

           
           if (Fleets.Any())
           {
               List<Fleets> List = Fleets;
               foreach (Fleets sov in List)
               {
                   string OperatorName = string.Empty;
                   var operators = db.Operator.Where(x => x.FleetsID == sov.FleetsID).FirstOrDefault();
                   if (operators != null)
                   {
                       OperatorName = operators.FullName;
                   }

                 
                   

                    if (tyreOrBattery == FleetBatteryOrTyreType.Tyre.ToString())
                   {
                       List<EquipmentTyre> Issues = new List<EquipmentTyre>();
                        Issues = db.EquipmentTyres.Where(x => x.fleetId == sov.FleetsID && x.DateCreated >= startingDate && x.DateCreated <= endingDate).ToList();

                        if (!string.IsNullOrEmpty(tyretype))
                        {

                            Issues = Issues.Where(x => x.Type == tyretype).ToList();
                        }


                        if (!string.IsNullOrEmpty(tyretube))
                        {
                            Issues = Issues.Where(x => x.Tube == tyretube).ToList();
                        }
                        if (Issues.Any())
                        {
                            foreach (var Issue in Issues)
                            {
                                FleetViewModel = new TireViewModel();
                                FleetViewModel.PlateNumber = sov.UnitNum;
                                FleetViewModel.Brand = Issue.Make;
                                FleetViewModel.SerialNo = Issue.SerialNo;
                                FleetViewModel.TireType = Issue.Type;
                                FleetViewModel.OperatorName = Issues.FirstOrDefault().CreatedBy;
                                FleetViewModel.UnitPrice = decimal.Parse(Issues.Sum(x => x.UnitPrice).ToString());
                                FleetViewModel.Quantity = Issues.Count;
                                FleetViewModel.Location = sov.Location;
                                FleetViewModel.TotalPrice = decimal.Parse((Issues.Sum(x => x.UnitPrice) * Issues.Count).ToString());

                                FleetViewModel.AmharicName = AmharicName;
                                FleetViewModel.Name = Name;
                                FleetViewModel.LogoFileName = LogoFileName;
                                FleetViewModel.Address = Address;
                                TireList.Add(FleetViewModel);
                            }
                        }



                    }
                   if (tyreOrBattery == FleetBatteryOrTyreType.Battery.ToString())
                   {
                       List<EquipmentBattery> Issues = new List<EquipmentBattery>();

                        Issues = db.EquipmentBattery.Where(x => x.fleetId == sov.FleetsID && x.DateCreated >= startingDate && x.DateCreated <= endingDate).ToList();
                        if (!string.IsNullOrEmpty(tyretype))
                        {

                            Issues = Issues.Where(x => x.Type == tyretype).ToList();
                        }


                        //if (!string.IsNullOrEmpty(tyretube))
                        //{
                        //    Issues = Issues.Where(x => x.Tube == tyretube).ToList();
                        //}
                        if (Issues.Any())
                        {
                            foreach (var Issue in Issues)
                            {
                                FleetViewModel = new TireViewModel();
                                FleetViewModel.PlateNumber = sov.UnitNum;
                                FleetViewModel.Brand = Issue.Make;
                                FleetViewModel.SerialNo = Issue.SerialNo;
                                FleetViewModel.TireType = Issue.Type;
                                FleetViewModel.OperatorName = Issues.FirstOrDefault().CreatedBy;
                                FleetViewModel.UnitPrice = decimal.Parse(Issues.Sum(x => x.UnitPrice).ToString());
                                FleetViewModel.Quantity = Issues.Count;
                                FleetViewModel.Location = sov.Location;
                                FleetViewModel.TotalPrice = decimal.Parse((Issues.Sum(x => x.UnitPrice) * Issues.Count).ToString());

                                FleetViewModel.AmharicName = AmharicName;
                                FleetViewModel.Name = Name;
                                FleetViewModel.LogoFileName = LogoFileName;
                                FleetViewModel.Address = Address;
                                TireList.Add(FleetViewModel);
                            }
                        }
                    }
                    /*List<EquipmentTyre>*/

                

               }
           }

           return TireList;
       }

    }
}
