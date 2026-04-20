using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class FuelConsumptionDataSource
    {
        private ExceedDbContext db = new ExceedDbContext();

        public const string OWNED = "OWNED";
        public const string RENTAL = "RENTAL";

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<FuelConsumptionSummary> GetConsumptionInfo(string category, string type, string equipmentname, string PlateNo, string Model, string ChasisNo, string Serial, string Location, DateTime StartingDate, DateTime EndingDate)
        {
            List<FuelConsumptionSummary> FuelConsumptionList = new List<FuelConsumptionSummary>();
            List<Branch> Branchs = db.Branch.ToList();
            List<FuelItemSetting> FuelItemSettings = db.FuelItemSettings.ToList();
            List<Fleets> Fleets = new List<Fleets>();
            //var Assets = db.AssetRegistrations.ToList();

            FleetReportHelper FleetReportHelper = new FleetReportHelper();
            #region for ShardedDataSource
            string AmharicName = "";
            string Name = "";
            string Address = "";
            Image LogoFileName = null;
            var organization = db.Companies.FirstOrDefault(x => x.Category == "0");

            if (organization != null)
            {
                Name = organization.TradeName;
                AmharicName = organization.AmharicName;
                var address = db.Addresses.Where(a =>a.Reference == organization.MainCompanyId.ToString() && a.ReferenceType.ToLower() == "company").ToList();
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
                var logo = db.Attachments.FirstOrDefault(a => a.Reference == organization.MainCompanyId.ToString() &&
                            a.ReferenceType == ExceedERP.Core.Domain.Common.AttachmentType.LOGO);
                if (logo != null)
                {
                    LogoFileName = OrganizationObjectDataSource.ByteArrayToImage(logo.BinaryFile);
                }
            }
            #endregion

            if (!string.IsNullOrEmpty(equipmentname) || !string.IsNullOrEmpty(Model) || !string.IsNullOrEmpty(category) || !string.IsNullOrEmpty(Serial) || !string.IsNullOrEmpty(type) || !string.IsNullOrEmpty(PlateNo) || !string.IsNullOrEmpty(ChasisNo) || !string.IsNullOrEmpty(Location))
            {
                Fleets = db.Fleets.ToList();
            }
            if (!string.IsNullOrEmpty(category))
            {
                if (!string.IsNullOrEmpty(type))
                {
                    Fleets = Fleets.Where(so =>so.EquipmentIdentityID.ToString() == category && so.EquipmentTypeID.ToString() == type).ToList();
                }
                else
                {

                    Fleets = Fleets.Where( so =>so.EquipmentIdentityID.ToString() == category).ToList();
                }

            }


            if (!string.IsNullOrEmpty(equipmentname))
            {


                Fleets = Fleets.Where(x => x.EquipmentName == equipmentname).ToList();


            }
            if (!string.IsNullOrEmpty(PlateNo))
            {
                Fleets = Fleets.Where(x => x.PlateNumber == PlateNo).ToList();
            }

            if (!string.IsNullOrEmpty(Location))
            {
                Fleets = Fleets.Where(x => x.Location == Location).ToList();
            }
            if (!string.IsNullOrEmpty(ChasisNo))
            {
                Fleets = Fleets.Where(x => x.ChasisNumber == ChasisNo).ToList();
            }
            if (!string.IsNullOrEmpty(PlateNo))
            {
                Fleets = Fleets.Where(x => x.PlateNumber == PlateNo).ToList();
            }
            if (!string.IsNullOrEmpty(Model))
            {
                Fleets = Fleets.Where(x => x.Model == Model).ToList();
            }




            if (!string.IsNullOrWhiteSpace(Location))
            {
                Branchs = Branchs.Where(x => x.BranchId.ToString() == Location).ToList();
            }
            if (!Fleets.Any())
            {
                Fleets.Add(new Fleets { PlateNumber = string.Empty });
            }
            List<Fuel> FuelConsumptionTemp = new List<Fuel>();
            List<IssueItem> OwnedFuelConsumptions = new List<IssueItem>();
            List<IssueItem> RentalFuelConsumptions = new List<IssueItem>();
            foreach (var Branch in Branchs)
            {



                foreach (var f in Fleets)
                {


                    if (!string.IsNullOrWhiteSpace(f.PlateNumber))
                    {
                        OwnedFuelConsumptions = FleetReportHelper.GetInventoryIssueItem(x => x.Issue.IsFuel == true && x.Issue.PlateNo == f.PlateNumber && x.Issue.BranchId == Branch.BranchId && x.DateCreated >= StartingDate && x.DateCreated <= EndingDate && x.Issue.Owner == IssueFuelVehicleOwner.Internal);

                        RentalFuelConsumptions = FleetReportHelper.GetInventoryIssueItem(x => x.Issue.IsFuel == true && x.Issue.PlateNo == f.PlateNumber && x.Issue.BranchId == Branch.BranchId && x.DateCreated >= StartingDate && x.DateCreated <= EndingDate && x.Issue.Owner == IssueFuelVehicleOwner.External);
                    }

                    if (string.IsNullOrWhiteSpace(f.PlateNumber))
                    {
                        OwnedFuelConsumptions = FleetReportHelper.GetInventoryIssueItem(x => x.Issue.IsFuel == true && x.Issue.BranchId == Branch.BranchId && x.DateCreated >= StartingDate && x.DateCreated <= EndingDate && x.Issue.Owner == IssueFuelVehicleOwner.Internal);

                        RentalFuelConsumptions = FleetReportHelper.GetInventoryIssueItem(x => x.Issue.IsFuel == true && x.Issue.BranchId == Branch.BranchId && x.DateCreated >= StartingDate && x.DateCreated <= EndingDate && x.Issue.Owner == IssueFuelVehicleOwner.External);
                    }
                    foreach (var FuelConsumption in RentalFuelConsumptions)
                    {
 
                            FuelConsumptionTemp.Add(new Fuel { FuelFilledAmount = FuelConsumption.IssuedQuantity, FuelType = FuelConsumption.InventoryItem.ItemCategory.Name, Description = FuelConsumption.ItemCode + " - " + FuelConsumption.InventoryItem.Name, TotalFuelcost = FuelConsumption.IssuedQuantity, FuelSourceDetail = RENTAL, CreatedBy = Branch.Name });
                      }
                    foreach (var FuelConsumption in OwnedFuelConsumptions)
                    {

                              FuelConsumptionTemp.Add(new Fuel { FuelFilledAmount = FuelConsumption.IssuedQuantity, FuelType = FuelConsumption.InventoryItem.ItemCategory.Name, Description = FuelConsumption.ItemCode + " - " + FuelConsumption.InventoryItem.Name, TotalFuelcost = FuelConsumption.Total, FuelSourceDetail = OWNED, CreatedBy = Branch.Name });
                      }
                }
            }
            if (FuelConsumptionTemp.Any())
            {
                foreach (var project in FuelConsumptionTemp.DistinctByField(x => x.CreatedBy))
                {
                    foreach (var FuelConsumption in FuelConsumptionTemp.Where(x=>x.CreatedBy == project.CreatedBy).DistinctByField(x=>x.Description))
                    {
                        var FuelConsupmtions = FuelConsumptionTemp.Where(x => x.CreatedBy == project.CreatedBy && x.Description == FuelConsumption.Description).DistinctByField(x => x.Description);

                        var FuelConsumptionViewModel = new FuelConsumptionSummary
                        {
                            ProjectName = FuelConsumption.CreatedBy??"NA",
                            FuelType = FuelConsumption.FuelType,
                            FuelItem = FuelConsumption.Description,
                            Amount = FuelConsupmtions.Where(x => x.FuelSourceDetail == OWNED ).Sum(x => x.FuelFilledAmount),                            
                            //TotalKm = FuelConsupmtions.Where(x => x.FuelSourceDetail == OWNED ).Sum(x => x.),                            
                            TotalCost = FuelConsupmtions.Where(x => x.FuelSourceDetail == OWNED ).Sum(x => x.TotalFuelcost),

                            RentalAmount = FuelConsupmtions.Where(x => x.FuelSourceDetail == RENTAL ).Sum(x => x.FuelFilledAmount),
                            RentalTotalCost = FuelConsupmtions.Where(x => x.FuelSourceDetail == RENTAL ).Sum(x => x.TotalFuelcost)
                        };
                        FuelConsumptionViewModel.TotalCost = FuelConsupmtions.Where(x => x.FuelSourceDetail == RENTAL ).Sum(x => x.TotalFuelcost) + FuelConsumptionTemp.Where(x => x.FuelSourceDetail == OWNED ).Sum(x => x.TotalFuelcost);

                        FuelConsumptionList.Add(FuelConsumptionViewModel);
                    }
                }
            }






            if (!FuelConsumptionList.Any())
            {
                var FuelConsumptionViewModel = new FuelConsumptionSummary
                {
                    ProjectName = string.Empty,
                    AmharicName = AmharicName,
                    Name = Name,
                    LogoFileName = LogoFileName,
                    Address = Address,
                    Total = 0
                };
                FuelConsumptionList.Add(FuelConsumptionViewModel);
                return FuelConsumptionList.ToList();
            }
            else
            {

                return FuelConsumptionList.ToList();
            }
        }

    }
}
