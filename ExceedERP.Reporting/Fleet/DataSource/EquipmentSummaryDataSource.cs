using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Core.Domain.Inventory;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Helpers.Fleet;
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Helpers.Common;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class EquipmentSummaryDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentSummaryViewModel> GetSheetInfo(DateTime StartingDate, DateTime EndingDate, string Project, string status, string PlateNo, string Model, string ChasisNo, string serialNo)
        {
            List<EquipmentSummaryViewModel> EquipmentSummaryList = new List<EquipmentSummaryViewModel>();
            List<Branch> Branchs = db.Branch.ToList();
            List<EquipmentIdentity> EquipmentTypes = db.EquipmentIdentity.ToList();
            List<FuelItemSetting> FuelItemSettings = db.FuelItemSettings.ToList();
            GLPeriod Periods = new GLPeriod();
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



            if (Periods != null)
            {
                foreach (var Branch in Branchs)
                {


                    List<TimeSheet> TimeSheets = db.TimeSheet.Where(x => x.CustomerName == Branch.Name && x.IsOnlineApproved == true && x.DateCreated >= StartingDate && x.DateCreated <= EndingDate).ToList();

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

                    List<TimeSheet> SheetsTemp = new List<TimeSheet>();
                    List<TimeSheet> RentalSheetTemp = new List<TimeSheet>();
                    List<Fuel> FuelConsumptionTemp = new List<Fuel>();



                    foreach (var Type in EquipmentTypes)
                    {


                        SheetsTemp = new List<TimeSheet>();
                        RentalSheetTemp = new List<TimeSheet>();
                        FuelConsumptionTemp = new List<Fuel>();



                        foreach (var Fleet in Fleets)
                        {

                            var FleetObj =
                                db.Fleets.Where(
                                    x =>
                                        x.FleetsID == Fleet &&
                                        x.EquipmentIdentityID == Type.EquipmentIdentityID)
                                    .FirstOrDefault();

                            if (FleetObj != null)
                            {
                                List<TimeSheet> FleetSheet =
                                    db.TimeSheet.Where(
                                        x =>
                                            x.CustomerName == Branch.Name && x.FleetsID == Fleet &&
                                            x.DateCreated >= StartingDate &&
                                            x.DateCreated <= EndingDate)
                                        .ToList();

                                foreach (var FuelItemSetting in FuelItemSettings)
                                {



                                    List<IssueItem> FuelConsumptions =
                                   FleetReportHelper.GetInventoryIssueItem(
                                             x =>
                                                 x.Issue.IsFuel == true && x.Issue.BranchId == Branch.BranchId &&
                                                 x.Issue.PlateNo == FleetObj.PlateNumber && x.Issue.GLPeriodId == Periods.GLPeriodId &&
                                                     x.ItemCategoryCode ==
                                                     FuelItemSetting.ItemCategoryCode
                                               );





                                    FuelConsumptionTemp.Add(new Fuel
                                    {
                                        FuelFilledAmount =
                                                FuelConsumptions.Sum(x => x.IssuedQuantity),
                                        IsVoid = FleetObj.IsExternal,
                                        FuelType = FuelItemSetting.ItemCategoryCode,
                                        TotalFuelcost = FuelConsumptions.Sum(x => x.Total)
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
                            IdelaCost = IdelaCost + ((Work.IdleTime + Work.BreakDownTime + Work.DownTime) * decimal.Parse(Work.IdealRentalRate.ToString()));

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
                            RentalIdelaCost = RentalIdelaCost + ((Work.IdleTime + Work.BreakDownTime + Work.DownTime) * decimal.Parse(Work.IdealRentalRate.ToString()));

                        }
                        TypeOfEquipment = Type.EquipmentCategoryName;

                        RentalSheetTemp = SheetsTemp.Where(x => x.IsExternal == true).ToList();


                        var EquipmentSummaryViewModel = new EquipmentSummaryViewModel();


                        EquipmentSummaryViewModel.Location = Branch.Name;
                        EquipmentSummaryViewModel.EquipmentType = TypeOfEquipment;

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
                        EquipmentSummaryViewModel.RentalFuelConsumptionCost = FuelConsumptionTemp.Where(x => x.IsVoid == false).Sum(x => x.TotalFuelcost);
                        EquipmentSummaryViewModel.FuelConsumptionQty = FuelConsumptionTemp.Where(x => x.IsVoid == false).Sum(x => x.FuelFilledAmount);
                        EquipmentSummaryViewModel.RentalFuelConsumptionCost = FuelConsumptionTemp.Where(x => x.IsVoid == true).Sum(x => x.TotalFuelcost);
                        EquipmentSummaryViewModel.RentalFuelConsumptionQty = FuelConsumptionTemp.Where(x => x.IsVoid == true).Sum(x => x.FuelFilledAmount);

                        EquipmentSummaryViewModel.AmharicName = AmharicName;
                        EquipmentSummaryViewModel.Name = Name;
                        EquipmentSummaryViewModel.LogoFileName = LogoFileName;
                        EquipmentSummaryViewModel.Address = Address;


                        EquipmentSummaryList.Add(EquipmentSummaryViewModel);
                    }
                }

            }





            EquipmentSummaryList = EquipmentSummaryList.Where(x => x.OwnedNo > 0 || x.RentalNo > 0).ToList();

            if (!string.IsNullOrEmpty(Project))
            {
                EquipmentSummaryList = EquipmentSummaryList.Where(x => x.Location == Project).ToList();

            }

            if (!EquipmentSummaryList.Any())
            {
                var EquipmentSummary = new EquipmentSummaryViewModel();
                EquipmentSummary.AmharicName = AmharicName;
                EquipmentSummary.Name = Name;
                EquipmentSummary.LogoFileName = LogoFileName;
                EquipmentSummary.Address = Address;
                EquipmentSummary.EquipmentType = string.Empty;
                EquipmentSummaryList.Add(EquipmentSummary);
            }

            return EquipmentSummaryList;

        }

    }
}
