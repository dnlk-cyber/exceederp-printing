using ExceedERP.ApplicationServices.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.Fleet.Printing.ViewModel;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;

namespace ExceedERP.Reporting.Fleet.Printing.DataSource
{
    [DataObject]
    public class EquipmentDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();





        [DataObjectMethod(DataObjectMethodType.Select)]
        public FleetViewModel GetFleetList(string Id)
        {
            IFleetService _FleetService = new FleetService();
            List<FleetViewModel> FleetList = new List<FleetViewModel>();
            FleetViewModel FleetViewModel;
            List<Branch> branches = db.Branch.ToList();
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

            Fleets Fleet = _FleetService.GetAll().Where(x => x.FleetsID.ToString() == Id).FirstOrDefault();
           

            if (Fleet != null )
            {
                var PhotoUploded = File.Exists("~/Content/Images/Fleet/Equipment/" + Fleet.FleetsID + ".jpg");
                FleetViewModel = new FleetViewModel
                {
                    Color = Fleet.Color,
                    CurentOdoMeter = Fleet.CurentOdoMeter,
                    PurchasingCost = Fleet.PurchasingCost,
                    AnnualDepreciationCost = Fleet.AnnualDepreciationCost,
                    Capacity = Fleet.Capacity,
                    EngineNumber = Fleet.EngineNumber,
                    UnitNum = Fleet.UnitNum,
                    EquipmentName = Fleet.EquipmentName,
                    Manufacturer = Fleet.Manufacturer,
                    SerialNo = Fleet.SerialNo,
                    ChasisNumber = Fleet.ChasisNumber
                };
                 FleetViewModel.ManufacturerYear = Fleet.ManufacturerYear;
                FleetViewModel.Heading = Fleet.UnitNum + " - " + Fleet.EquipmentName;
                FleetViewModel.StateOfOdoMeter = Fleet.StateOfOdoMeter;
                FleetViewModel.TypeOfFuel = Fleet.TypeOfFuel;
                if (PhotoUploded)
                {
                    FleetViewModel.FleetPhoto = "Content/Images/Fleet/Equipment/" + Fleet.FleetsID + ".jpg";
                }
                else
                {
                //    FleetViewModel.FleetPhoto = "~/Content/Images/Fleet/Asset/default.png";
                }
                FleetViewModel.PlateNumber = Fleet.PlateNumber;
                if (Fleet.EquipmentType != null)
                {
                    FleetViewModel.Type = Fleet.EquipmentType.EquipmentTypeName;
                }

                if (Fleet.EquipmentIdentity != null)
                {
                    FleetViewModel.Category = Fleet.EquipmentIdentity.EquipmentCategoryName;
                }            
                FleetViewModel.Location = Fleet.Location;         
                FleetViewModel.Model = Fleet.Model;
                FleetViewModel.FuelStandard = ((Fleet.MaxFuelStandard + Fleet.MinFuelStandard) / 2).ToString();                    
                FleetViewModel.PurchasedDate = Fleet.PurchasedDate;
                FleetViewModel.PurchasingCost = Fleet.PurchasingCost;
                FleetViewModel.TyreSize = Fleet.TyreSize;
                FleetViewModel.Status = Fleet.Status;
                FleetViewModel.CreatedBy = Fleet.CreatedBy;
                FleetViewModel.CreatedDate = Fleet.DateCreated.Value.Date.ToString("yyyy-MM-dd");
                FleetViewModel.CheckedDate = Fleet.SendForApprovalTime.Value.Date.ToString("yyyy-MM-dd");
                FleetViewModel.ApprovedDate = Fleet.OnlineApprovedTime.Value.Date.ToString("yyyy-MM-dd");
                FleetViewModel.ApprovedBySignature = SignatureHelper.GetUserSignature(Fleet.OnlineApprovedBy);
                FleetViewModel.CreatedBySignature = SignatureHelper.GetUserSignature(Fleet.CreatedBy);
                FleetViewModel.CheckedBySignature = SignatureHelper.GetUserSignature(Fleet.SendForApprovalBy);
                FleetViewModel.SendForApprovalBy = Fleet.SendForApprovalBy;
                FleetViewModel.ApprovedBy = Fleet.OnlineApprovedBy;
                FleetViewModel.AmharicName = AmharicName;
                FleetViewModel.Name = Name;
                FleetViewModel.Address = Address;
                FleetViewModel.LogoFileName = LogoFileName;

                return FleetViewModel;

            }

            return new FleetViewModel();
        }



        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FleetViewModel> GetAccessoriesList(string Id)
        {
            IAccessoriesService _AccessoriesService = new AccessoriesService();
            List<FleetViewModel> FleetList = new List<FleetViewModel>();
            FleetViewModel FleetViewModel;



            var Accesories = _AccessoriesService.GetAll().Where(x => x.FleetsID == Guid.Parse(Id)).ToList();

            foreach (var Acc in Accesories)
            {
                try
                {
                    FleetViewModel = new FleetViewModel
                    {
                        EquipmentName = Acc.AccessoryName,
                        Heading = Acc.Quantity.ToString(),
                        Capacity = Acc.UnitPrice.ToString(),
                        SerialNo = Acc.SerialNo
                    };
                    FleetList.Add(FleetViewModel);
                }
                catch (Exception)
                { }
            }

            return FleetList;
        }




        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FuelIssueViewModel> GetFuelIssuePrint(string PlateNo,
         DateTime startingDate, DateTime endingDate)
        {
            List<FuelIssueViewModel> fuels = new List<FuelIssueViewModel>();


            if (!string.IsNullOrWhiteSpace(PlateNo))
            {
                fuels = db.IssueItems.AsNoTracking().Where(x => x.Issue.PlateNo.ToUpper().Contains(PlateNo) && x.DateCreated >= startingDate && x.DateCreated <= endingDate && x.Issue.IsFuel).Select(s => new FuelIssueViewModel()
                {
                    InventoryCategoryName = s.InventoryItem.ItemCategory.Name,
                    InventoryItemName = s.InventoryItem.Name,
                    ApprovedQuantity = s.ApprovedQuantity,
                    PlateNo = s.Issue.PlateNo,
                    ReadingKM = s.Issue.ReadingKM,
                    Reference = s.Issue.Reference,
                    TotalCost = s.Total,
                    StoreCode = s.Issue.StoreCode,
                    UnitPrice = s.UnitPrice,
                    IssueID = s.IssueID.ToString(),
                    Refrance = s.Issue.Reference,
                    IssuedDate = s.Issue.DateCreated,
                    

                }).ToList();

            }
            else
            {
                fuels = db.IssueItems.AsNoTracking().Where(x => x.DateCreated >= startingDate && x.DateCreated <= endingDate && x.Issue.IsFuel).Select( s=> new FuelIssueViewModel() {
                    InventoryCategoryName = s.InventoryItem.ItemCategory.Name,
                    InventoryItemName =s.InventoryItem.Name,
                    ApprovedQuantity = s.ApprovedQuantity,
                    PlateNo = s.Issue.PlateNo,
                    ReadingKM = s.Issue.ReadingKM,
                    Reference = s.Issue.Reference,
                    TotalCost = s.Total,
                    Refrance = s.Issue.Reference,
                    StoreCode = s.Issue.StoreCode,
                    UnitPrice = s.UnitPrice

                }).ToList();

            }

            return fuels;
        }
    }
}
