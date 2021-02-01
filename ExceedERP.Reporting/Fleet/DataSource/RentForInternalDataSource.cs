using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FleetSalvageStore;
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
    public class RentForInternalDataSource
    {
        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<InternalRentViewModel> GetInternalRent(string Identity, string category, string ManufacturerYear, string Manufacturer,
           DateTime startingDate, DateTime endingDate)
        {
            List<InternalRentViewModel> InternalRentList = new List<InternalRentViewModel>();
             List<RentInternalProject> InternalRent =new List<RentInternalProject>();
         



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
                    InternalRent =
                        db.RentInternalProject.Where(
                            so =>
                                so.EquipmentIdentity.ToString() == Identity &&
                                so.EquipmentType.ToString() == category &&  so.DateCreated > startingDate && so.DateCreated < endingDate)
                            .ToList();
                }
                else
                {
                    InternalRent =
                        db.RentInternalProject.Where(so => so.EquipmentIdentity.ToString() == Identity &&  so.DateCreated > startingDate && so.DateCreated < endingDate)
                            .ToList();
                }
            }

            if (InternalRent.Any())
            {




                List<RentInternalProject> RentInternalProject = db.RentInternalProject.Where(x => x.DateCreated > startingDate && x.DateCreated < endingDate).ToList();


                foreach (var c in RentInternalProject)
                {

                    var InternalRentViewModel = new InternalRentViewModel();


                    InternalRentViewModel.ApprovedBy = c.ApprovedBy;
                    InternalRentViewModel.AssignedDate = c.AssignedDate;
                    InternalRentViewModel.AssignedQuantity = c.AssignedQuantity;
                    InternalRentViewModel.EquipmentOwner = c.EquipmentOwner;
                    //      InternalRentViewModel.EquipmentType = c.;
                    InternalRentViewModel.RequestQuantity = c.RequestQuantity;
                    InternalRentViewModel.StartingDate = c.StartingDate;
                    InternalRentViewModel.ToDate = c.ToDate;
                    InternalRentViewModel.Status = c.Status;
                    InternalRentViewModel.AmharicName = AmharicName;
                    InternalRentViewModel.Name = Name;
                    InternalRentViewModel.LogoFileName = LogoFileName;
                    InternalRentViewModel.Address = Address;



                    InternalRentViewModel.AmharicName = AmharicName;
                    InternalRentViewModel.Name = Name;
                    InternalRentViewModel.LogoFileName = LogoFileName;
                    InternalRentViewModel.Address = Address;

                    InternalRentList.Add(InternalRentViewModel);

                }
            }

            return InternalRentList;

        }
    }
}
