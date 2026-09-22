using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetPlanAndEvaluation
{
    public class FleetAnnualPlanDetailController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/FleetAnnualPlanDetail
        public ActionResult Index()
        {
            
            return View();
        }

        public ActionResult FleetAnnualPlanDetail_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var annualPlans = db.FleetAnnualPlanDetails.Where(x => x.FleetAnnualPlanId == id);

            return Json(annualPlans.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetAnnualPlanDaily_Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var annualPlans = db.FleetAnnualPlanDaily.Where(x => x.FleetAnnualPlanDetailId == id);

            return Json(annualPlans.ToDataSourceResult(request, ModelState));
        }
        

        public ActionResult FleetAnnualPlanDetail_Create([DataSourceRequest] DataSourceRequest request, FleetAnnualPlanDetail model, int id)
        {
            if(model.EfficiencyPercentile > 1 || model.EfficiencyPercentile < 0 || model.InefficiencyPercentile > 1 || model.InefficiencyPercentile < 0)
            {
                ModelState.AddModelError("", "Percentile values should be between 0 - 1");
            }
            if (ModelState.IsValid)
            {
                model.DateFrom = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == model.GlPeriodId).DateFrom;
                model.DateTo = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == model.GlPeriodId).DateTo;
                model.FleetAnnualPlanId = id;
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Today;
                CalculateValues(model);
                db.FleetAnnualPlanDetails.Add(model);
                db.SaveChanges();
                var plan = model;
                var glPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == plan.GlPeriodId);
                int daysInMonth = (glPeriod.DateTo - glPeriod.DateFrom).Days + 1;
                for (int d = 1; d <= daysInMonth; d++)
                {
                    var dailyPlan = new FleetAnnualPlanDaily();
                    dailyPlan.CreatedBy = User.Identity.Name;
                    dailyPlan.DateCreated = DateTime.Now;

                    // copy values
                    decimal basePerDay = decimal.Floor(plan.NumberOfEquipmentInMonth / daysInMonth);
                    int remainder = plan.NumberOfEquipmentInMonth % daysInMonth;
                    int extra = d <= remainder ? 1 : 0;
                    dailyPlan.NumberOfEquipmentInDay = (int)(basePerDay + extra);
                    dailyPlan.WorkingHourPerDay = plan.WorkingHourPerDay;
                    dailyPlan.InefficiencyPercentile = plan.InefficiencyPercentile;
                    dailyPlan.EfficiencyPercentile = plan.EfficiencyPercentile;
                    dailyPlan.ChargingRate = plan.ChargingRate;
                    basePerDay = decimal.Floor(plan.ExpectedMonthlyIncome / daysInMonth);
                    remainder = (int)plan.ExpectedMonthlyIncome % daysInMonth;
                    extra = d <= remainder ? 1 : 0;
                    dailyPlan.ExpectedDailyIncome = basePerDay + extra;
                    basePerDay = decimal.Floor(plan.TyreCost / daysInMonth);
                    remainder = (int)plan.TyreCost % daysInMonth;
                    extra = d <= remainder ? 1 : 0;
                    dailyPlan.TyreCost = basePerDay + extra;
                    dailyPlan.LiterPerHour = plan.LiterPerHour;
                    basePerDay = decimal.Floor(plan.TotalLiter / daysInMonth);
                    remainder = (int)plan.TotalLiter % daysInMonth;
                    extra = d <= remainder ? 1 : 0;
                    dailyPlan.TotalLiter = basePerDay + extra;
                    dailyPlan.FuelCostPerLiter = plan.FuelCostPerLiter;
                    basePerDay = decimal.Floor(plan.TotalFuelCost / daysInMonth);
                    remainder = (int)plan.TotalFuelCost % daysInMonth;
                    extra = d <= remainder ? 1 : 0;
                    dailyPlan.TotalFuelCost = basePerDay + extra;
                    basePerDay = decimal.Floor(plan.MaintenanceCost / daysInMonth);
                    remainder = (int)plan.MaintenanceCost % daysInMonth;
                    extra = d <= remainder ? 1 : 0;
                    dailyPlan.MaintenanceCost = basePerDay + extra;
                    basePerDay = decimal.Floor(plan.Perduim / daysInMonth);
                    remainder = (int)plan.Perduim % daysInMonth;
                    extra = d <= remainder ? 1 : 0;
                    dailyPlan.Perduim = basePerDay + extra;
                    basePerDay = decimal.Floor(plan.Salary / daysInMonth);
                    remainder = (int)plan.Salary % daysInMonth;
                    extra = d <= remainder ? 1 : 0;
                    dailyPlan.Salary = basePerDay + extra;
                    basePerDay = decimal.Floor(plan.TotalExpectedExpense / daysInMonth);
                    remainder = (int)plan.TotalExpectedExpense % daysInMonth;
                    extra = d <= remainder ? 1 : 0;
                    dailyPlan.TotalExpectedExpense = basePerDay + extra;
                    basePerDay = decimal.Floor(plan.ExpectedTotalProfit / daysInMonth);
                    remainder = (int)plan.ExpectedTotalProfit % daysInMonth;
                    extra = d <= remainder ? 1 : 0;
                    dailyPlan.ExpectedTotalProfit = basePerDay + extra;

                    dailyPlan.FleetAnnualPlanId = plan.FleetAnnualPlanId;
                    dailyPlan.FleetAnnualPlanDetailId = plan.FleetAnnualPlanDetailId;
                    dailyPlan.WorkingDay = d;
                    dailyPlan.GlPeriodId = plan.GlPeriodId;
                    dailyPlan.OperationDate = glPeriod.DateFrom.AddDays(d - 1);
                    db.FleetAnnualPlanDaily.Add(dailyPlan);
                    db.SaveChanges();
                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        
        public ActionResult FleetAnnualPlanDetail_Update([DataSourceRequest] DataSourceRequest request, FleetAnnualPlanDetail model)
        {
            if (model.EfficiencyPercentile > 1 || model.EfficiencyPercentile < 0 || model.InefficiencyPercentile > 1 || model.InefficiencyPercentile < 0)
            {
                ModelState.AddModelError("", "Percentile values should be between 0 - 1");
            }
            if (ModelState.IsValid)
            {
                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Today;
                CalculateValues(model);
                
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                var plan = model;
                var glPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == plan.GlPeriodId);
                int daysInMonth = (glPeriod.DateTo - glPeriod.DateFrom).Days + 1;
                for (int d = 1; d <= daysInMonth; d++)
                {
                    var checkAvailability = db.FleetAnnualPlanDaily.FirstOrDefault(x => x.FleetAnnualPlanDetailId == plan.FleetAnnualPlanDetailId && x.WorkingDay == d);
                    if (checkAvailability == null)
                    {
                        var dailyPlan = new FleetAnnualPlanDaily();
                        dailyPlan.CreatedBy = User.Identity.Name;
                        dailyPlan.DateCreated = DateTime.Now;

                        // copy values
                        decimal basePerDay = decimal.Floor(plan.NumberOfEquipmentInMonth / daysInMonth);
                        int remainder = plan.NumberOfEquipmentInMonth % daysInMonth;
                        int extra = d <= remainder ? 1 : 0;
                        dailyPlan.NumberOfEquipmentInDay = (int)(basePerDay + extra);
                        dailyPlan.WorkingHourPerDay = plan.WorkingHourPerDay;
                        dailyPlan.InefficiencyPercentile = plan.InefficiencyPercentile;
                        dailyPlan.EfficiencyPercentile = plan.EfficiencyPercentile;
                        dailyPlan.ChargingRate = plan.ChargingRate;
                        basePerDay = decimal.Floor(plan.ExpectedMonthlyIncome / daysInMonth);
                        remainder = (int)plan.ExpectedMonthlyIncome % daysInMonth;
                        extra = d <= remainder ? 1 : 0;
                        dailyPlan.ExpectedDailyIncome = basePerDay + extra;
                        basePerDay = decimal.Floor(plan.TyreCost / daysInMonth);
                        remainder = (int)plan.TyreCost % daysInMonth;
                        extra = d <= remainder ? 1 : 0;
                        dailyPlan.TyreCost = basePerDay + extra;
                        dailyPlan.LiterPerHour = plan.LiterPerHour;
                        basePerDay = decimal.Floor(plan.TotalLiter / daysInMonth);
                        remainder = (int)plan.TotalLiter % daysInMonth;
                        extra = d <= remainder ? 1 : 0;
                        dailyPlan.TotalLiter = basePerDay + extra;
                        dailyPlan.FuelCostPerLiter = plan.FuelCostPerLiter;
                        basePerDay = decimal.Floor(plan.TotalFuelCost / daysInMonth);
                        remainder = (int)plan.TotalFuelCost % daysInMonth;
                        extra = d <= remainder ? 1 : 0;
                        dailyPlan.TotalFuelCost = basePerDay + extra;
                        basePerDay = decimal.Floor(plan.MaintenanceCost / daysInMonth);
                        remainder = (int)plan.MaintenanceCost % daysInMonth;
                        extra = d <= remainder ? 1 : 0;
                        dailyPlan.MaintenanceCost = basePerDay + extra;
                        basePerDay = decimal.Floor(plan.Perduim / daysInMonth);
                        remainder = (int)plan.Perduim % daysInMonth;
                        extra = d <= remainder ? 1 : 0;
                        dailyPlan.Perduim = basePerDay + extra;
                        basePerDay = decimal.Floor(plan.Salary / daysInMonth);
                        remainder = (int)plan.Salary % daysInMonth;
                        extra = d <= remainder ? 1 : 0;
                        dailyPlan.Salary = basePerDay + extra;
                        basePerDay = decimal.Floor(plan.TotalExpectedExpense / daysInMonth);
                        remainder = (int)plan.TotalExpectedExpense % daysInMonth;
                        extra = d <= remainder ? 1 : 0;
                        dailyPlan.TotalExpectedExpense = basePerDay + extra;
                        basePerDay = decimal.Floor(plan.ExpectedTotalProfit / daysInMonth);
                        remainder = (int)plan.ExpectedTotalProfit % daysInMonth;
                        extra = d <= remainder ? 1 : 0;
                        dailyPlan.ExpectedTotalProfit = basePerDay + extra;

                        dailyPlan.FleetAnnualPlanId = plan.FleetAnnualPlanId;
                        dailyPlan.FleetAnnualPlanDetailId = plan.FleetAnnualPlanDetailId;
                        dailyPlan.WorkingDay = d;
                        dailyPlan.GlPeriodId = plan.GlPeriodId;
                        dailyPlan.OperationDate = glPeriod.DateFrom.AddDays(d - 1);
                        db.FleetAnnualPlanDaily.Add(dailyPlan);
                        db.SaveChanges();
                    }
                    
                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }public ActionResult FleetAnnualPlanDaily_Update([DataSourceRequest] DataSourceRequest request, FleetAnnualPlanDaily model)
        {
            if (model.EfficiencyPercentile > 1 || model.EfficiencyPercentile < 0 || model.InefficiencyPercentile > 1 || model.InefficiencyPercentile < 0)
            {
                ModelState.AddModelError("", "Percentile values should be between 0 - 1");
            }
            if (ModelState.IsValid)
            {
                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Today;
                CalculateDailyValues(model);
                
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        
        public ActionResult FleetAnnualPlanDetail_Delete([DataSourceRequest] DataSourceRequest request, FleetAnnualPlanDetail model)
        {
            if (ModelState.IsValid)
            {
                var checkAvailability = db.FleetAnnualPlanDaily.Where(x => x.FleetAnnualPlanDetailId == model.FleetAnnualPlanDetailId);
                db.FleetAnnualPlanDaily.RemoveRange(checkAvailability);
                db.SaveChanges();
                var annualPlan = db.FleetAnnualPlanDetails.Find(model.FleetAnnualPlanDetailId);
                db.FleetAnnualPlanDetails.Remove(annualPlan);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetAnnualPlanDaily_Delete([DataSourceRequest] DataSourceRequest request, FleetAnnualPlanDaily model)
        {
            if (ModelState.IsValid)
            {
                var checkAvailability = db.FleetAnnualPlanDaily.FirstOrDefault(x => x.FleetAnnualPlanDailyId == model.FleetAnnualPlanDailyId);
                db.FleetAnnualPlanDaily.Remove(checkAvailability);
                db.SaveChanges();              
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }


        private void CalculateValues(FleetAnnualPlanDetail model)
        {
            model.WorkingHourPerMonth = model.WorkingHourPerDay * model.WorkingDaysInMonth;
            model.HourForInefficiencyPerMonth = model.WorkingHourPerMonth * model.InefficiencyPercentile;
            model.ExpectedEffectiveWorkingHrPerMonth = model.WorkingHourPerMonth * model.EfficiencyPercentile;
            model.ExpectedMonthlyIncome = model.ExpectedEffectiveWorkingHrPerMonth * model.ChargingRate;
            model.TotalLiter = model.ExpectedEffectiveWorkingHrPerMonth * model.LiterPerHour;
            model.TotalFuelCost = model.FuelCostPerLiter * model.FuelCostPerLiter;
            model.TotalExpectedExpense = model.TyreCost + model.TotalFuelCost + model.MaintenanceCost + model.Perduim + model.Salary;
            model.ExpectedTotalProfit = model.ExpectedMonthlyIncome - model.TotalExpectedExpense;
        } private void CalculateDailyValues(FleetAnnualPlanDaily model)
        {
            
            model.HourForInefficiencyPerDay = model.WorkingHourPerDay * model.InefficiencyPercentile;
            model.ExpectedEffectiveWorkingHrPerDay = model.WorkingHourPerDay * model.EfficiencyPercentile;
            model.ExpectedDailyIncome = model.ExpectedEffectiveWorkingHrPerDay * model.ChargingRate;
            model.TotalLiter = model.ExpectedEffectiveWorkingHrPerDay * model.LiterPerHour;
            model.TotalFuelCost = model.FuelCostPerLiter * model.FuelCostPerLiter;
            model.TotalExpectedExpense = model.TyreCost + model.TotalFuelCost + model.MaintenanceCost + model.Perduim + model.Salary;
            model.ExpectedTotalProfit = model.ExpectedDailyIncome - model.TotalExpectedExpense;
        }
    }
}