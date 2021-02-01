using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
   public class MaintainaceDataSource
    {
       ExceedDbContext db = new ExceedDbContext();
          [DataObjectMethod(DataObjectMethodType.Select)]
       public List<MaintenanceViewModel> GetFleetList(string Identity, string category, string Manufacturer, string EquipmentName,
        DateTime startingDate, DateTime endingDate)
       {
           List<MaintenanceViewModel> TireList = new List<MaintenanceViewModel>();
           MaintenanceViewModel FleetViewModel;
           List<Fleets> Fleets;
         



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
                       db.Fleets.Where(so => so.EquipmentTypeID.ToString() == category)
                           .ToList();
               }
           }
           else
           {
               if (!string.IsNullOrEmpty(Manufacturer))
               {
                   Fleets = db.Fleets.Where(so => so.Manufacturer == Manufacturer).ToList();
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
               List<Fleets> List =
                     Fleets.Where(x => x.DateCreated >= startingDate && x.DateCreated <= endingDate).ToList();
               foreach (Fleets sov in List)
               {

                   List<JobOrder> Orders = db.JobOrder.Where(x => x.FleetsID == sov.FleetsID).ToList();
                   if (Orders.Any())
                   {
                       foreach(var order in Orders)
                       {
                       FleetViewModel = new MaintenanceViewModel();
                       FleetViewModel.PlateNumber = sov.PlateNumber;
                       FleetViewModel.MaterialCost = order.SpareCost;
                       FleetViewModel.LabourCost = order.LabourCost;
                   

                       FleetViewModel.Name = Name;
                       FleetViewModel.LogoFileName = LogoFileName;
                       FleetViewModel.Address = Address;

                       TireList.Add(FleetViewModel);
                   }
                   }

               }
           }

           return TireList;
       }

    }
}
