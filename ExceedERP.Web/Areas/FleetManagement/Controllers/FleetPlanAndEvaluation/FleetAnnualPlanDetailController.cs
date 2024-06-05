using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
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

        public ActionResult FleetAnnualPlanDetail_Create([DataSourceRequest] DataSourceRequest request, FleetAnnualPlanDetail model, int id)
        {
            if(model.EfficiencyPercentile > 1 || model.EfficiencyPercentile < 0 || model.InefficiencyPercentile > 1 || model.InefficiencyPercentile < 0)
            {
                ModelState.AddModelError("", "Percentile values should be between 0 - 1");
            }
            if (ModelState.IsValid)
            {
                model.FleetAnnualPlanId = id;
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Today;
                CalculateValues(model);
                db.FleetAnnualPlanDetails.Add(model);
                db.SaveChanges();
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
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetAnnualPlanDetail_Delete([DataSourceRequest] DataSourceRequest request, FleetAnnualPlanDetail model)
        {
            if (ModelState.IsValid)
            {

                var annualPlan = db.FleetAnnualPlanDetails.Find(model.FleetAnnualPlanId);
                db.FleetAnnualPlanDetails.Remove(annualPlan);
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
        }
    }
}