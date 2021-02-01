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
using ExceedERP.Reporting.SharedDataSources;
using ExceedERP.Helpers.Fleet;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class EquipmentTypeSummaryDataSource
    {


        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentSummaryViewModel> GetSheetInfo(DateTime StartingDate, DateTime EndingDate, string EquipmentType, string EquipmentCategory)
        {
            List<EquipmentSummaryViewModel> EquipmentSummaryList = new List<EquipmentSummaryViewModel>();
            List<Branch> Branchs = db.Branch.ToList();
            List<FuelItemSetting> FuelItemSettings = db.FuelItemSettings.ToList();
            GLPeriod Periods = new GLPeriod();
            EquipmentIdentity Type = new EquipmentIdentity();
            FleetReportHelper fleetReportHelper = new FleetReportHelper();
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

             
            Type =
                db.EquipmentIdentity.Where(x =>
                        x.EquipmentIdentityID.ToString() == EquipmentType).FirstOrDefault();

            List<Fleets> Equipments = db.Fleets.Where(x => x.EquipmentIdentityID == Type.EquipmentIdentityID).ToList();
            Equipments = Equipments.FilterFleet(EquipmentCategory, EquipmentType,"", string.Empty, string.Empty, string.Empty, string.Empty, string.Empty,string.Empty);

            if (Periods != null)
            {
                foreach (var Branch in Branchs)
                {


                    List<TimeSheet> TimeSheets = db.TimeSheet.Where(x => x.CustomerName == Branch.Name && x.IsOnlineApproved == true && x.FleetTypeID == Type.EquipmentIdentityID && x.DateCreated > Periods.DateFrom && x.DateCreated < Periods.DateTo).ToList();

                    if (!string.IsNullOrEmpty(EquipmentCategory))
                    {
                        TimeSheets = TimeSheets.Where(x => x.FleetCategoryID.ToString() == EquipmentCategory).ToList();

                    }



                    decimal Workingcost = 0;
                    decimal Workingtime = 0;
                    decimal IdelaCost = 0;
                    decimal DownTime = 0;

                    string TypeOfEquipment = string.Empty;



                    List<Fuel> FuelConsumptionTemp = new List<Fuel>();


                    foreach (var Equipment in Equipments)
                    {

                        foreach (var FuelItemSetting in FuelItemSettings)
                        {

                            List<IssueItem> FuelConsumptions =
                                                         fleetReportHelper.GetInventoryIssueItem(
                                                                   x =>
                                                                       x.Issue.IsFuel == true && x.Issue.BranchId == Branch.BranchId &&
                                                                       x.Issue.PlateNo == Equipment.PlateNumber && x.Issue.GLPeriodId == Periods.GLPeriodId &&
                                                                           x.ItemCategoryCode ==
                                                                           FuelItemSetting.ItemCategoryCode
                                                                     );


                           

                      
                                FuelConsumptionTemp.Add(new Fuel
                                {
                                    FuelFilledAmount =
                                        FuelConsumptions.Sum(x => x.IssuedQuantity),
                                    FuelType = FuelItemSetting.ItemCode,
                                    TotalFuelcost = FuelConsumptions.Sum(x => x.Total)
                                });

                            }
                        }



                    


                    foreach (var Work in TimeSheets)
                    {

                        TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) -
                                                   DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                        TimeSpan AfterNoonDuration =
                            DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) -
                            DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());

                        DownTime = DownTime + Work.DownTime + Work.BreakDownTime + Work.IdleTime;
                        Workingtime =  (MorningDuration.Hours + AfterNoonDuration.Hours);
                        Workingcost =Workingcost +( Workingtime * decimal.Parse(Work.RentalRate.ToString()));
                        IdelaCost = IdelaCost + (DownTime* decimal.Parse(Work.IdealRentalRate.ToString()));

                    }



                    var TotalIdealHr = TimeSheets.Sum(x => x.IdleTime);
                    var TotalConsumption = FuelConsumptionTemp.Sum(x => x.FuelFilledAmount);
                    var TotalConsumptionCost = FuelConsumptionTemp.Sum(x => x.FuelFilledAmount);

                    var EquipmentSummaryViewModel = new EquipmentSummaryViewModel();


                    EquipmentSummaryViewModel.Location = Branch.Name;
                    EquipmentSummaryViewModel.EquipmentType = TypeOfEquipment;
                    EquipmentSummaryViewModel.IdealCost = IdelaCost;
                    EquipmentSummaryViewModel.IdealHour = TotalIdealHr;
                    EquipmentSummaryViewModel.WorkingCost = Workingcost;
                    EquipmentSummaryViewModel.WorkingHour = Workingtime;
                    EquipmentSummaryViewModel.DownTime = DownTime;
                    EquipmentSummaryViewModel.FuelConsumptionCost = TotalConsumption;
                    EquipmentSummaryViewModel.FuelConsumptionQty = TotalConsumptionCost;
                    EquipmentSummaryViewModel.Total = TotalConsumption + Workingcost;
                    EquipmentSummaryViewModel.AmharicName = AmharicName;
                    EquipmentSummaryViewModel.Name = Name;
                    EquipmentSummaryViewModel.LogoFileName = LogoFileName;
                    EquipmentSummaryViewModel.Address = Address;


                    EquipmentSummaryList.Add(EquipmentSummaryViewModel);



                }

                EquipmentSummaryList = EquipmentSummaryList.Where(x => x.Total > 0).ToList();


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
