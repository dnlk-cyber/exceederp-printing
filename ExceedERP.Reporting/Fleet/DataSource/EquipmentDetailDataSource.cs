using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Helpers.Common;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class EquipmentDetailDataSource
    {


        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public async Task<List<EquipmentSummaryViewModel>> GetSheetInfo(DateTime StartingDate, DateTime EndingDate, string EquipmentType, string Project, string EquipmentCategory,   string status, string PlateNo, string Model, string ChasisNo, string serialNo)
        {
            List<EquipmentSummaryViewModel> EquipmentSummaryList = new List<EquipmentSummaryViewModel>();
            List<Branch> Branchs = db.Branch.ToList();
            List<EquipmentType> EquipmentCategoryes = db.EquipmentType.ToList();
            List<EquipmentIdentity> EquipmentTypes = db.EquipmentIdentity.ToList();
            List<FuelItemSetting> FuelItemSettings = db.FuelItemSettings.ToList();
            GLPeriod Periods = new GLPeriod();
            EquipmentIdentity Type = new EquipmentIdentity();
            List<Fleets> Equipments = db.Fleets.ToList();
            FleetReportHelper FleetReportHelper = new FleetReportHelper();



            Equipments = Equipments.FilterFleet(EquipmentCategory, EquipmentType,"", PlateNo, ChasisNo, serialNo, string.Empty, Project, Model);

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





                Periods =
                    db.GLPeriods.Where(x =>
                            x.DateFrom >= StartingDate && x.DateTo <= EndingDate
                            ).FirstOrDefault();

              if (!string.IsNullOrEmpty(EquipmentType))
            {
                Type =
                    db.EquipmentIdentity.Where(x =>
                            x.EquipmentIdentityID.ToString() == EquipmentType).FirstOrDefault();
                if (Type != null)
                {
                    Equipments = Equipments.Where(x => x.EquipmentIdentityID == Type.EquipmentIdentityID).ToList();
                }
            }

     
            if (Periods != null)
            {


                foreach (var Equipment in Equipments)
                {
                    List<TimeSheet> SheetsTemp = new List<TimeSheet>();
                    List<TimeSheet> RentalSheetTemp = new List<TimeSheet>();
                    List<Fuel> FuelConsumptionTemp = new List<Fuel>();


                    List<TimeSheet> TimeSheets = db.TimeSheet.Where(x => x.FleetsID == Equipment.FleetsID && x.DateCreated >  StartingDate && x.DateCreated < EndingDate).ToList();

                    if (!string.IsNullOrEmpty(EquipmentCategory))
                    {
                        TimeSheets = TimeSheets.Where(x => x.FleetCategoryID.ToString() == EquipmentCategory).ToList();

                    }


                    var DistinictProject = TimeSheets.DistinctByField(x => x.CustomerName).ToList();

                    if (!string.IsNullOrEmpty(Project))
                    {
                        TimeSheets = TimeSheets.Where(x => x.CustomerName == Project).ToList();
                    }
 
                    List<Guid> Fleets = TimeSheets.DistinctByField(x => x.FleetsID).Select(x => x.FleetsID).ToList();


                    decimal Workingcost = 0;
                    decimal Workingtime = 0;
                    decimal IdelaCost = 0;
                    decimal DownTime = 0;

                    decimal RentalWorkingcost = 0;
                    decimal RentalWorkingtime = 0;
                    decimal RentalIdelaCost = 0;
                    decimal RentalDownTime = 0;
                    string TypeOfEquipment = string.Empty;




                    foreach (var project in DistinictProject)
                    {

                        var branch = db.Branch.Where(x => x.Name == project.CustomerName).FirstOrDefault();
                        if (branch != null)
                        {

                            SheetsTemp = new List<TimeSheet>();
                            RentalSheetTemp = new List<TimeSheet>();
                            FuelConsumptionTemp = new List<Fuel>();



                            foreach (var Fleet in Fleets)
                            {

                                var FleetObj =
                                    db.Fleets.Where(
                                        x =>
                                            x.FleetsID == Fleet  )
                                        .FirstOrDefault();

                                if (FleetObj != null)
                                {
                                    List<TimeSheet> FleetSheet =
                                        db.TimeSheet.Where(
                                            x =>
                                                x.CustomerName == branch.Name && x.FleetsID == Fleet &&
                                                x.DateCreated >= StartingDate &&
                                                x.DateCreated <= EndingDate)
                                            .ToList();

                                    foreach (var FuelItemSetting in FuelItemSettings)
                                    {

 

                                     
                                            var IssueFuels =
                                              FleetReportHelper.GetInventoryIssueItem(
                                                    x =>
                                                    x.Issue.IsFuel == true && x.Issue.BranchId == branch.BranchId &&
                                                    x.Issue.PlateNo == FleetObj.PlateNumber && x.Issue.GLPeriodId == Periods.GLPeriodId  &&
                                                        x.ItemCategoryCode ==   FuelItemSetting.ItemCategoryCode);
                                                    
                                            FuelConsumptionTemp.Add(new Fuel
                                            {
                                                FuelFilledAmount =
                                                    IssueFuels.Sum(x => x.IssuedQuantity),
                                                IsVoid = FleetObj.IsExternal,
                                                FuelType = FuelItemSetting.ItemCategoryCode,
                                                TotalFuelcost = IssueFuels.Sum(x => x.Total)
                                            });
 
                                    }
                                    SheetsTemp.AddRange(FleetSheet);
                                }
                            }

                            foreach (var Work in SheetsTemp.Where(x => x.IsExternal == false).ToList())
                            {

                                TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) -
                                                           DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                                TimeSpan AfterNoonDuration =
                                    DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) -
                                    DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());
                                DownTime = DownTime + Work.DownTime;
                                Workingtime = Workingtime + (MorningDuration.Hours + AfterNoonDuration.Hours);
                                Workingcost = Workingcost + ((MorningDuration.Hours + AfterNoonDuration.Hours) * decimal.Parse(Work.RentalRate.ToString()));
                                IdelaCost = IdelaCost + (Work.IdleTime * decimal.Parse(Work.RentalRate.ToString()));

                            }

                            foreach (var Work in SheetsTemp.Where(x => x.IsExternal == true).ToList())
                            {

                                TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) -
                                                           DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                                TimeSpan AfterNoonDuration =
                                    DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) -
                                    DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());

                                RentalDownTime = RentalDownTime + Work.DownTime;
                                RentalWorkingtime = RentalWorkingtime + (MorningDuration.Hours + AfterNoonDuration.Hours);
                                RentalWorkingcost = RentalWorkingcost + ((MorningDuration.Hours + AfterNoonDuration.Hours) * decimal.Parse(Work.RentalRate.ToString()));
                                RentalIdelaCost = RentalIdelaCost + (Work.IdleTime * decimal.Parse(Work.RentalRate.ToString()));

                            }
                            TypeOfEquipment = Type.EquipmentCategoryName;

                            RentalSheetTemp = SheetsTemp.Where(x => x.IsExternal == true).ToList();
                            var FleetType =
                                    EquipmentCategoryes.Where(x => x.EquipmentTypeID == Equipment.EquipmentTypeID)
                                        .FirstOrDefault();


                            var EquipmentSummaryViewModel = new EquipmentSummaryViewModel();


                            EquipmentSummaryViewModel.Location = branch.Name;
                            EquipmentSummaryViewModel.EquipmentType = TypeOfEquipment;

                            EquipmentSummaryViewModel.EquipmentType = FleetType.EquipmentTypeName;
                            EquipmentSummaryViewModel.PlateNumber = Equipment.PlateNumber;
                            EquipmentSummaryViewModel.Location = project.CustomerName;
                            EquipmentSummaryViewModel.Make = Equipment.Manufacturer;
                            EquipmentSummaryViewModel.Model = Equipment.Model;
                            EquipmentSummaryViewModel.RentalRate = Equipment.RentalRate.ToString();

                            EquipmentSummaryViewModel.IdealCost = IdelaCost;
                            EquipmentSummaryViewModel.IdealHour = SheetsTemp.Sum(x => x.IdleTime);
                            EquipmentSummaryViewModel.WorkingCost = Workingcost;
                            EquipmentSummaryViewModel.WorkingHour = Workingtime;
                            EquipmentSummaryViewModel.IdealCost = IdelaCost;
                            EquipmentSummaryViewModel.DownTime = DownTime;
                            EquipmentSummaryViewModel.RentalDownTime = RentalDownTime;
                            EquipmentSummaryViewModel.RentalIdealCost = RentalIdelaCost;
                            EquipmentSummaryViewModel.RentalIdealHour = RentalSheetTemp.Sum(x => x.IdleTime);
                            EquipmentSummaryViewModel.RentalWorkingCost = RentalWorkingcost;
                            EquipmentSummaryViewModel.RentalWorkingHour = RentalWorkingtime;
                            EquipmentSummaryViewModel.OwnedNo = SheetsTemp.Where(x => x.IsExternal == false).ToList().Count;
                            EquipmentSummaryViewModel.RentalNo = RentalSheetTemp.Count;
                            EquipmentSummaryViewModel.FuelConsumptionCost = FuelConsumptionTemp.Sum(x => x.TotalFuelcost);
                            EquipmentSummaryViewModel.FuelConsumptionQty = FuelConsumptionTemp.Sum(x => x.FuelFilledAmount);
                             EquipmentSummaryViewModel.Total = IdelaCost + Workingcost;

                             EquipmentSummaryViewModel.AmharicName = AmharicName;
                             EquipmentSummaryViewModel.Name = Name;
                             EquipmentSummaryViewModel.LogoFileName = LogoFileName;
                             EquipmentSummaryViewModel.Address = Address;

                                EquipmentSummaryList.Add(EquipmentSummaryViewModel);
                        }
                    }
                }
            

            }





            EquipmentSummaryList = EquipmentSummaryList.Where(x => x.Total > 0 || x.FuelConsumptionQty > 0).ToList();

            if (!string.IsNullOrEmpty(Project))
            {
                EquipmentSummaryList = EquipmentSummaryList.Where(x => x.Location == Project).ToList();

            }

            if (!EquipmentSummaryList.Any())
            {
                var EquipmentSummaryViewModel = new EquipmentSummaryViewModel();
                EquipmentSummaryViewModel.Location = string.Empty;
                EquipmentSummaryViewModel.AmharicName = AmharicName;
                EquipmentSummaryViewModel.Name = Name;
                EquipmentSummaryViewModel.LogoFileName = LogoFileName;
                EquipmentSummaryViewModel.Address = Address;

                EquipmentSummaryList.Add(EquipmentSummaryViewModel);
            }

            return EquipmentSummaryList;

        }

    }
}
