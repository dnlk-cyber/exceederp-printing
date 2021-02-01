using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
   public class RentToExternalDataSource
    {

        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<RentToExternalViewModel> GetRentToExternal(string Identity, string category, string EquipmentName, string FuelCovered,
            DateTime startingDate, DateTime endingDate)
        {
            List<RentToExternalViewModel> ExternalRentList = new List<RentToExternalViewModel>();
            RentToExternalViewModel RentToExternalViewModel;
            List<RentOtherSuppliers> RentOtherSuppliers = new List<RentOtherSuppliers>();




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
                    RentOtherSuppliers =
                        db.RentOtherSuppliers.Where(
                            so =>
                                so.EquipmentIdentityID.ToString() == Identity &&
                                so.EquipmentIdentityID.ToString() == category)
                            .ToList();
                }
                else
                {

                    RentOtherSuppliers =
                                          db.RentOtherSuppliers.Where(so =>
                                                    so.EquipmentIdentityID.ToString() == Identity)
                                              .ToList();
                }

            }

            if (RentOtherSuppliers.Any())
            {



                if (!string.IsNullOrEmpty(FuelCovered))
                {
                    RentOtherSuppliers = RentOtherSuppliers.Where(so => so.FuelCoveredBy == FuelCovered).ToList();
                }


                if (RentOtherSuppliers.Any())
                {
                    foreach (var a in RentOtherSuppliers)
                    {
                        RentToExternalViewModel = new RentToExternalViewModel();

                        RentToExternalViewModel.FuelCoveredBy = a.FuelCoveredBy;
                        RentToExternalViewModel.AdvancePayment = a.AdvancePayment;
                        RentToExternalViewModel.BankFullAddress = a.BankFullAddress;
                        RentToExternalViewModel.BankSlipNo = a.BankSlipNo;
                        RentToExternalViewModel.Description = a.Description;
                    
                        RentToExternalViewModel.NameBankBranch = a.NameBankBranch;
                        RentToExternalViewModel.ProjectLocation = a.ProjectLocation;
                        RentToExternalViewModel.AdvancePayment = a.AdvancePayment;
                        RentToExternalViewModel.Quantity = a.Quantity;
                        RentToExternalViewModel.RentalRateIncludingVAT = a.RentalRateIncludingVAT;
                        RentToExternalViewModel.Status = a.Status;
                       





                        RentToExternalViewModel.AmharicName = AmharicName;
                        RentToExternalViewModel.Name = Name;
                        RentToExternalViewModel.LogoFileName = LogoFileName;
                        RentToExternalViewModel.Address = Address;

                        ExternalRentList.Add(RentToExternalViewModel);
                    }
                }

            }


            return ExternalRentList;
        }

    }
}