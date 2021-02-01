using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.EquipmentInspection;
using ExceedERP.Core.Domain.FleetManagement.FleetMaintenance;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Fleet.DataSource
{
      [DataObject]
   public class InspectionDataSource
    {

          ExceedDbContext db = new ExceedDbContext();



          
        [DataObjectMethod(DataObjectMethodType.Select)]
          public List<EquipmentInspectionViewModel> GetInspectionList(string Identity, string category, DateTime StartingDate, DateTime EndingDate)
        {
            List<EquipmentInspectionViewModel> FleetList = new List<EquipmentInspectionViewModel>();
            EquipmentInspectionViewModel InspectionModel;
            List<Fleets> List;
          


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
                    List =
                        db.Fleets.Where(
                            so =>
                                so.EquipmentIdentityID.ToString() == Identity &&
                                so.EquipmentTypeID.ToString() == category)
                            .ToList();
                }
                else
                {

                    List =
                                          db.Fleets.Where(
                                              so =>

                                                  so.EquipmentIdentityID.ToString() == Identity)
                                              .ToList();
                }

            }
            else
            {

                List = db.Fleets.ToList();

            }
            if (List.Any())
            {
              
            
               foreach (var f in List)
                   {
                   string EquipmentName = string.Empty;
                       List<AnnualEquipmentInventoryInspection> Inspections = db.AnnualEquipmentInventoryInspection.Where(x => x.FleetsID == f.FleetsID && x.DateCreated > StartingDate && x.DateCreated < EndingDate ).ToList();

             
                       foreach (var Inspection in Inspections)
                           {

                               Fleets fleet = db.Fleets.Where(x => x.FleetsID == Inspection.FleetsID).FirstOrDefault();

                               if (fleet != null)
                                   {
                                       EquipmentName = fleet.PlateNumber + " - " + fleet.EquipmentName;
                                   }

                               List<InspectionLog> LineInspections = db.InspectionLogs.Where(x => x.InspectionId == Inspection.AnnualEquipmentInventoryInspectionID.ToString()).OrderByDescending(x=>x.DateCreated).ToList();

                               foreach (var LineInspection in LineInspections)
                               {

                                   InspectionModel = new EquipmentInspectionViewModel();

                                   InspectionModel.EquipmentName = EquipmentName;
                                   InspectionModel.Date = LineInspection.DateCreated;
                                   InspectionModel.Code = LineInspection.CategoryName +" - "+ LineInspection.LineName;
                                   InspectionModel.DateofInspection = Inspection.DateofInspection;
                                
                                   InspectionModel.FindingsOutcome = Inspection.FindingsOutcome;
                                   InspectionModel.InspectedBy =Inspection.InspectedBy;
                                   InspectionModel.IsActive = LineInspection.IsActive ? "Available" : "UnAvailable";
                                   


                                   InspectionModel.AmharicName = AmharicName;
                                   InspectionModel.Name = Name;
                                   InspectionModel.LogoFileName = LogoFileName;
                                   InspectionModel.Address = Address;

                                   FleetList.Add(InspectionModel);
                               }
                           }



                }



            }

            return FleetList;
        }

    }
        }
  