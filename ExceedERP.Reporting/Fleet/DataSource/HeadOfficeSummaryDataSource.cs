using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common;
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
    public class HeadOfficeSummaryDataSource
    {


        ExceedDbContext db = new ExceedDbContext();

        public const string INCOME = "INCOME";
        public const string EXPENSE = "EXPENSE";

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<HeadOfficeSummaryViewModel> GetHeadOfficeInfo(DateTime StartingDate, DateTime EndingDate, string Location, string Type, string category, string EquipmentName, string SerialNo, string status, string PlateNo, string Model, string ChasisNo)
        {
            List<HeadOfficeSummaryViewModel> HeadOfficeList = new List<HeadOfficeSummaryViewModel>();
             List<FuelItemSetting> FuelItemSettings = db.FuelItemSettings.ToList();
              List<ItemCategory>  Category =   db.ItemCategorys.ToList();
              FleetReportHelper FleetReportHelper = new FleetReportHelper();

            List<IssueItem> IssueFuels = new List<IssueItem>();
            List<Fleets> Fleets = db.Fleets.ToList();

            var Assets = db.AssetRegistrations.ToList();
            var Branches = db.Branch.ToList();

            if (!string.IsNullOrEmpty(category))
            {
                if (!string.IsNullOrEmpty(Type))
                {
                    Fleets =
                        db.Fleets.Where(
                            so =>
                                so.EquipmentIdentityID.ToString() == category &&
                                so.EquipmentTypeID.ToString() == Type)
                            .ToList();
                }
                else
                {

                    Fleets =
                                          db.Fleets.Where(
                                              so =>

                                                  so.EquipmentIdentityID.ToString() == Type)
                                              .ToList();
                }

            }
            else
            {

                Fleets = db.Fleets.ToList();

            }


                if (!string.IsNullOrEmpty(EquipmentName))
                {


                    Fleets = Fleets.Where(x => x.EquipmentName == EquipmentName).ToList();


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
                if (!string.IsNullOrEmpty(status))
                {
                    Fleets = Fleets.Where(x => x.Status == status).ToList();
                }
          
                decimal EXPENSE = 0;
                decimal INCOME = 0;


            decimal Workingcost = 0;
            decimal IdealCost = 0;





            foreach (var fleet in Fleets)
            {

                List<TimeSheet> InternalTimeSheet = db.TimeSheet.Where(x => x.DateCreated >= StartingDate && x.DateCreated <= EndingDate && x.IsExternal == false && x.FleetsID == fleet.FleetsID).ToList();
                
                foreach (var Work in InternalTimeSheet)
                {
                    TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) - DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                    TimeSpan AfterNoonDuration = DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());


                    Workingcost = Workingcost + ((MorningDuration.Hours + AfterNoonDuration.Hours) * decimal.Parse(Work.RentalRate.ToString()));
                    IdealCost = IdealCost + decimal.Parse((Work.IdleTime * Work.IdealRentalRate).ToString());
                }

                IssueFuels = FleetReportHelper.GetInventoryIssueItem(x => x.Issue.DateCreated >= StartingDate && x.Issue.DateCreated <= EndingDate && x.Issue.IsFuel == true && x.Issue.PlateNo == fleet.PlateNumber);

 }


            var EquipmentUtilizationViewModelExternal = new HeadOfficeSummaryViewModel();


            EquipmentUtilizationViewModelExternal.Detail = "  Rent Income";
            EquipmentUtilizationViewModelExternal.Income = Workingcost;

          
            HeadOfficeList.Add(EquipmentUtilizationViewModelExternal);

            INCOME = INCOME + Workingcost;

              EquipmentUtilizationViewModelExternal = new HeadOfficeSummaryViewModel();

            EquipmentUtilizationViewModelExternal.Detail = "  Ideal Hr Income";
            EquipmentUtilizationViewModelExternal.Income = IdealCost;

          
            HeadOfficeList.Add(EquipmentUtilizationViewModelExternal);

            INCOME = INCOME + IdealCost;

            foreach (var FuelItemSetting in FuelItemSettings)
            {
                var FuelType = Category.Where(x => x.ItemCategoryCode == FuelItemSetting.ItemCategoryCode).FirstOrDefault();

                if (FuelType != null)
                {
                 decimal   Issues = IssueFuels.Where(x => x.ItemCode == FuelItemSetting.InventoryItem.ItemCode).ToList().Sum(x=>x.Total);
                    EquipmentUtilizationViewModelExternal = new HeadOfficeSummaryViewModel();


                    EquipmentUtilizationViewModelExternal.Detail = FuelItemSetting.InventoryItem.Name + " Consumption";
                    EquipmentUtilizationViewModelExternal.Income = Issues;

                 
                    HeadOfficeList.Add(EquipmentUtilizationViewModelExternal);

                    EXPENSE = EXPENSE + Issues;
                }
            }

             EquipmentUtilizationViewModelExternal = new HeadOfficeSummaryViewModel();

            EquipmentUtilizationViewModelExternal.Detail = " Total Expense";
            EquipmentUtilizationViewModelExternal.Income = EXPENSE;

          
            HeadOfficeList.Add(EquipmentUtilizationViewModelExternal);

            EquipmentUtilizationViewModelExternal = new HeadOfficeSummaryViewModel();

            EquipmentUtilizationViewModelExternal.Detail = " Total  Income";
            EquipmentUtilizationViewModelExternal.Income = INCOME;

                 HeadOfficeList.Add(EquipmentUtilizationViewModelExternal);

            EquipmentUtilizationViewModelExternal = new HeadOfficeSummaryViewModel();

            EquipmentUtilizationViewModelExternal.Detail = " Net Income ";
            EquipmentUtilizationViewModelExternal.Income = INCOME - EXPENSE;

          
            HeadOfficeList.Add(EquipmentUtilizationViewModelExternal);
    


            return HeadOfficeList;

        }
   

    }
}
