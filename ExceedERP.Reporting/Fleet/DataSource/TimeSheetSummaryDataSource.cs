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
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using ExceedERP.Reporting.SharedDataSources;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class TimeSheetSummaryDataSource
    {

        ExceedDbContext db = new ExceedDbContext();

        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<TimeSheetViewModel> GetSheetInfo(DateTime StartingDate, DateTime EndingDate, string Project, string Category, string Type, string EquipmentName, string PlateNo, string Model, string ChasisNo, string Serial)
        {
            List<TimeSheetViewModel> TimeSheetList = new List<TimeSheetViewModel>();
            List<Branch> Branchs = db.Branch.ToList();
    
                foreach (var Branch in Branchs)
                {

                    decimal Workingcost = 0;
                    decimal Workingtime = 0;
                    decimal IdealCosttime = 0;
                    string TypeOfWork = string.Empty;
                    List<TimeSheet> c = db.TimeSheet.Where(x => x.CustomerName == Branch.Name && x.IsOnlineApproved == true && x.DateCreated >= StartingDate && x.DateCreated <= EndingDate).ToList();



                    if (!string.IsNullOrEmpty(Project))
                    {
                        c = c.Where(x => x.CustomerName == Project).ToList();
                    }


                    foreach (var Work in c)
                    {
                        TimeSpan MorningDuration = DateTime.Parse(Work.WorkingHoursMorningto.ToString()) - DateTime.Parse(Work.WorkingHoursMorningFrom.ToString());
                        TimeSpan AfterNoonDuration = DateTime.Parse(Work.WorkingHoursAfternoonTo.ToString()) - DateTime.Parse(Work.WorkingHoursAfternoonFrom.ToString());

                        Workingtime = Workingtime + (MorningDuration.Hours + AfterNoonDuration.Hours);
                        Workingcost = Workingtime * decimal.Parse(Work.RentalRate.ToString());
                        TypeOfWork = TypeOfWork + " - " + Work.TypeofWork;
                        IdealCosttime = IdealCosttime + ((Work.IdleTime + Work.BreakDownTime + Work.DownTime)*decimal.Parse(Work.IdealRentalRate.ToString()));


                    }


                    var TimeSheetsViewModel = new TimeSheetViewModel();


                    TimeSheetsViewModel.CustomerName = Branch.Name??"";

                    TimeSheetsViewModel.TotalIdealCost = IdealCosttime;
                    TimeSheetsViewModel.TotalIdleTime = c.Sum(x => x.IdleTime) + c.Sum(x => x.BreakDownTime) + c.Sum(x => x.DownTime);
                    TimeSheetsViewModel.TotalWorkingCost = Workingcost;
                    TimeSheetsViewModel.TotalWorkingTime = Workingtime;
                    TimeSheetsViewModel.TypeOfWork = TypeOfWork; 



                    TimeSheetList.Add(TimeSheetsViewModel);


                }
             return TimeSheetList.Where(x=>x.TotalWorkingTime > 0).ToList();

        }
    }
}

