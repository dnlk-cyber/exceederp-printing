using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.Fleet.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.Reporting.SharedDataSources;
using System.Drawing;
using ExceedERP.Core.Domain.FleetManagement.FuelManagement;
using ExceedERP.Helpers.Fleet;

namespace ExceedERP.Reporting.Fleet.DataSource
{
    [DataObject]
    public class PlanEvaluationPerEquipmentDataSource
    {



        ExceedDbContext db = new ExceedDbContext();


        [DataObjectMethod(DataObjectMethodType.Select)]
        public List<ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject> GetPlateNo()
        {

            var result = db.Fleets.Select(e => new ExceedERP.Helpers.Fleet.FleetBindingHelper.HelperObject
            {

                Name = e.PlateNumber+" - "+e.UnitNum,
                Value = e.UnitNum

            }
            ).ToList();

            return result;

        }
   [DataObjectMethod(DataObjectMethodType.Select)]
        public List<PlanEvaluationViewModel> GetPlanEvaluation(string Project, string Category, string Type, string EquipmentName, string PlateNo, string Model, string ChasisNo, string Serial)
           
        {
           List<PlanEvaluationViewModel> FleetList = new List<PlanEvaluationViewModel>();
      
          
            var fleets = db.Fleets.ToList();
            var fiscalYears = db.GLFiscalYears.ToList();

            FleetReportHelper fleetReportHelper = new Helpers.Fleet.FleetReportHelper();
        

            Fleets fleet = fleets.FilterSingleFleet( Category, Type, EquipmentName, PlateNo, ChasisNo, Serial, PlateNo, Project, Model);

           
            if (fleet!= null)
            {
              
                foreach (var fiscalYear in fiscalYears)
                {
                    foreach (var GLPeriod in fiscalYear.GLPeriod)
                    {
                        decimal actualDowntime = 0;
                        decimal actualFuelConsumption = 0;
                        int actualIdletime = 0;
                        int actualWorkingtime = 0;

                       var fuel = fleetReportHelper.GetInventoryIssueItem(x => x.Issue.PlateNo == fleet.PlateNumber && x.Issue.IsFuel && x.DateCreated >= GLPeriod.DateFrom && x.DateCreated <= GLPeriod.DateTo).ToList();
                       List<TimeSheet> timeSheet = db.TimeSheet.Where(x => x.FleetsID== fleet.FleetsID   &&    x.DateCreated >= GLPeriod.DateFrom && x.DateCreated <= GLPeriod.DateTo).ToList();

                        if (fuel.Any())
                        {
                            actualFuelConsumption = fuel.Sum(x => x.IssuedQuantity);

                        }
                        if (timeSheet.Any())
                        {
                            foreach (var ts in timeSheet)
                            {
                                actualWorkingtime = actualWorkingtime + (DateTime.Parse(ts.WorkingHoursMorningto.ToString()).Hour - DateTime.Parse(ts.WorkingHoursMorningFrom.ToString()).Hour) + (DateTime.Parse(ts.WorkingHoursAfternoonTo.ToString()).Hour - DateTime.Parse(ts.WorkingHoursAfternoonFrom.ToString()).Hour);// + (ts.WorkingHoursAfternoonTo.Value - ts.WorkingHoursAfternoonFrom.Value);
                            }
                            actualIdletime = timeSheet.Sum(x => x.IdleTime);
                            actualDowntime = timeSheet.Sum(x => x.DownTime);
                            actualWorkingtime = timeSheet.Sum(x => x.DownTime);

                        }

                      EquipmentPlanandEvaluations p = db.EquipmentPlanandEvaluation.Where(x => x.EquipmentCategoryId == fleet.FleetsID ).FirstOrDefault();
                        if (p !=null)
                        {
                          
                                var PlanEvaluationViewModel = new PlanEvaluationViewModel();

                                PlanEvaluationViewModel.Equipment = fleet.PlateNumber + " - " + fleet.EquipmentName;
                                PlanEvaluationViewModel.PlanDowntime = decimal.Parse(p.PlanDowntime.ToString());
                                PlanEvaluationViewModel.PlanEquipmentsPittyCashExpenditure = decimal.Parse(p.PlanEquipmentsPittyCashExpenditure.ToString());
                                PlanEvaluationViewModel.PlanFuelConsumption = decimal.Parse(p.PlanFuelConsumption.ToString());
                                PlanEvaluationViewModel.Year = fiscalYear.Name;
                                PlanEvaluationViewModel.Month = GLPeriod.Name;
                                PlanEvaluationViewModel.PlanIdletime = decimal.Parse(p.PlanIdletime.ToString());
                                PlanEvaluationViewModel.Description = fleet.PlateNumber + " - " + fleet.EquipmentName;
                                PlanEvaluationViewModel.PlanIncome = decimal.Parse(p.PlanIncome.ToString());
                                PlanEvaluationViewModel.PlanWorkingtime = decimal.Parse(p.PlanWorkingtime.ToString());
                                PlanEvaluationViewModel.PlanTyreUtilization = decimal.Parse(p.PlanTyreUtilization.ToString());
                                PlanEvaluationViewModel.ActualFuelConsumption = actualFuelConsumption;
                                PlanEvaluationViewModel.ActualPlanDowntime = actualDowntime;
                                PlanEvaluationViewModel.ActualPlanIdletime = actualIdletime;
                                PlanEvaluationViewModel.ActualPlanWorkingtime = actualWorkingtime;
                                PlanEvaluationViewModel.UnitOfMeasurement = p.UnitOfMeasurement;

 

                                FleetList.Add(PlanEvaluationViewModel);
                            }
                        }
                    
                }
            }


            return FleetList;     
        }
       
    }
}
