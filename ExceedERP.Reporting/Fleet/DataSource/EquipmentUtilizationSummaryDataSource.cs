using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Finance.GL;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class EquipmentUtilizationSummaryDataSource
    {
        ExceedDbContext db = new ExceedDbContext();

        public const string INCOME = "INCOME";
        public const string EXPENSE = "EXPENSE";

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<EquipmentUtilizationSummary> GetUtilazationInfo(DateTime StartingDate, DateTime EndingDate, string Category, string Type, string PlateNo, string EquipmentName, string Model, string ChasisNo, string Serial, string Location)
        {
            List<EquipmentUtilizationSummary> TimeSheetList = new List<EquipmentUtilizationSummary>();
            List<Branch> Branchs = db.Branch.ToList();
            GLPeriod Periods = new GLPeriod();





          
             



            decimal Workingcost = 0;
            decimal IdealCost = 0;

            decimal RentalWorkingcost = 0;
            decimal RentalIdealCost = 0;

            List<TimeSheet> InternalTimeSheet = db.TimeSheet.Where(x => x.DateCreated >= StartingDate&& x.DateCreated <= EndingDate && x.IsExternal == false).ToList();

          //  Equipments = Equipments.FilterFleet(Category, Type, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty, string.Empty);


            foreach (var Work in InternalTimeSheet)
            {
                TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) - DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                TimeSpan AfterNoonDuration = DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());


                Workingcost = Workingcost + ((MorningDuration.Hours + AfterNoonDuration.Hours) * decimal.Parse(Work.RentalRate.ToString()));

                IdealCost = IdealCost + decimal.Parse((Work.IdleTime * Work.RentalRate).ToString());


            }


            var EquipmentUtilizationViewModelInternal = new EquipmentUtilizationSummary();


            EquipmentUtilizationViewModelInternal.Type = INCOME; ;
            EquipmentUtilizationViewModelInternal.Description = "Total Rental Income Cost";
            EquipmentUtilizationViewModelInternal.IdealIncome = IdealCost;
            EquipmentUtilizationViewModelInternal.WorkingExpense = Workingcost;
            EquipmentUtilizationViewModelInternal.TotalIncome = Workingcost + IdealCost;



            TimeSheetList.Add(EquipmentUtilizationViewModelInternal);




            List<TimeSheet> ExternalTimeSheet = db.TimeSheet.Where(x => x.DateCreated > StartingDate&& x.DateCreated < EndingDate && x.IsExternal == true).ToList();




            foreach (var Work in ExternalTimeSheet)
            {
                TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) - DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                TimeSpan AfterNoonDuration = DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());


                RentalWorkingcost = RentalWorkingcost + ((MorningDuration.Hours + AfterNoonDuration.Hours) * decimal.Parse(Work.RentalRate.ToString()));

                RentalIdealCost = RentalIdealCost + decimal.Parse((Work.IdleTime * Work.RentalRate).ToString());


            }


            var EquipmentUtilizationViewModelExternal = new EquipmentUtilizationSummary();


            EquipmentUtilizationViewModelExternal.Type = EXPENSE;
            EquipmentUtilizationViewModelExternal.Description = "Total Rental Expense Cost";
            EquipmentUtilizationViewModelExternal.IdealIncome = RentalIdealCost;
            EquipmentUtilizationViewModelExternal.WorkingExpense = RentalWorkingcost;
            EquipmentUtilizationViewModelExternal.TotalIncome = RentalWorkingcost + RentalIdealCost;


            TimeSheetList.Add(EquipmentUtilizationViewModelExternal);




            return TimeSheetList;

        }
   
    
    
    }
}

