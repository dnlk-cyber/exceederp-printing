using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Windows.Forms;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Core.Domain.FleetManagement.FleetEquipmentRent;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class ExternalRentDataSource
    {


        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<SelectListItem> GetFuelCovered()
        {

            var result = EnumExtension.EnumToSelectList(typeof(RentFromExternal.FuelCoveredenum));

            return result;

        }


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExternalRentViewModel> GetExternalRent(DateTime StartingDate, DateTime EndingDate, string EquipmentType, string Project, string EquipmentCategory, string EquipmentName, string status, string PlateNo, string Model, string ChasisNo, string serialNo)
        {
            List<ExternalRentViewModel> ExternalRentList = new List<ExternalRentViewModel>();
            ExternalRentViewModel ExternalRentViewModel;
            List<RentFromExternal> RentFromExternal = db.RentFromExternal.ToList();
            FleetCostHelper FleetCostHelper = new FleetCostHelper();

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


            if (!string.IsNullOrEmpty(EquipmentCategory))
            {
                if (!string.IsNullOrEmpty(EquipmentType))
                {
                    RentFromExternal =
                        db.RentFromExternal.Where(
                            so =>
                                so.EquipmentIdentity.ToString() == EquipmentCategory &&
                                so.EquipmentType.ToString() == EquipmentType && so.DateCreated > StartingDate && so.DateCreated < EndingDate)
                            .ToList();
                }
                else
                {

                    RentFromExternal =
                                          db.RentFromExternal.Where(so =>
                                                    so.EquipmentIdentity.ToString() == EquipmentCategory && so.DateCreated > StartingDate && so.DateCreated < EndingDate)
                                              .ToList();
                }

            }

            if (RentFromExternal.Any())
            {




                if (RentFromExternal.Any())
                {
                    foreach (var a in RentFromExternal)
                    {
                        var Equipments = db.Fleets.Where(x => x.RentId == a.RentFromExternalID).ToList();
                        Equipments = Equipments.FilterFleet(EquipmentCategory, EquipmentType, EquipmentName, PlateNo, ChasisNo, serialNo, string.Empty, Project, Model);

                        foreach (var Equipment in Equipments)
                        {

                            ExternalRentViewModel = new ExternalRentViewModel();

                            ExternalRentViewModel.EquipmentCategory = Equipment.EquipmentIdentity.EquipmentCategoryName;
                            ExternalRentViewModel.EquipmentType = Equipment.EquipmentType.EquipmentTypeName;
                            ExternalRentViewModel.Equipment = Equipment.EquipmentName + " - " + Equipment.PlateNumber + " - " + Equipment.UnitNum;
                            ExternalRentViewModel.TotalCost = FleetCostHelper.GetExternalRentTotalCost(a);
                            //ExternalRentViewModel.EquipmentCost = FleetCostHelper.GetSingleEquipmentExternalRentCost(a,Equipment.RentId);
                            ExternalRentViewModel.RentNo = a.Num;
                            ExternalRentViewModel.AdvancePayment = a.AdvancePayment;
                            ExternalRentViewModel.BankFullAddress = a.BankFullAddress;
                            ExternalRentViewModel.BankSlipNo = a.BankSlipNo;
                            ExternalRentViewModel.Description = a.Description;
                            ExternalRentViewModel.FuelCoveredBy = a.FuelCoveredBy;
                            ExternalRentViewModel.ProjectLocation = a.ProjectLocation;
                            ExternalRentViewModel.Quantity = a.Quantity;
                            ExternalRentViewModel.RentalRateIncludingVAT = a.RentalRateIncludingVAT;
                            ExternalRentViewModel.Status = a.Status;
                            ExternalRentViewModel.UnitOfWorkMeasurement = a.UnitOfWorkMeasurement;
                            //ExternalRentViewModel.WorkingCapacity = a.WorkingCapacity;

                            ExternalRentViewModel.AmharicName = AmharicName;
                            ExternalRentViewModel.Name = Name;
                            ExternalRentViewModel.LogoFileName = LogoFileName;
                            ExternalRentViewModel.Address = Address;

                            ExternalRentList.Add(ExternalRentViewModel);
                        }
                    }
                }

            }


            if (!ExternalRentList.Any())
            {
                  ExternalRentViewModel = new ExternalRentViewModel();
                ExternalRentViewModel.AmharicName = AmharicName;
                ExternalRentViewModel.Name = Name;
                ExternalRentViewModel.LogoFileName = LogoFileName;
                ExternalRentViewModel.Address = Address;

                ExternalRentList.Add(ExternalRentViewModel);
            }


            return ExternalRentList;
        }

    }
}
