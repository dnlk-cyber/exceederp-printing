using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetPlanAndEvaluation
{
    [AuthorizeRoles(FleetManagementRoles.FleetAnnualPlanner, FleetManagementRoles.FleetAnnualPlanApprover)]
    public class FleetAnnualPlanController : Controller
    {
        private readonly CacheData CacheData = new CacheData();
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/FleetAnnualPlan
        public ActionResult Index()
        {

           
            ViewData["Category"] = CacheData.GetEquipmentIdentityCaches().Select(
      i => new
      {
          Text = i.Name,
          Value = i.Id
      }).AsQueryable();
            ViewData["Type"] = CacheData.GetEquipmentTypeCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["Name"] = CacheData.GetEquipmentNameCaches().Select(
        i => new
        {
            Text = i.Name,
            Value = i.Id
        }).AsQueryable();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + " " + s.SerialNo
            }).AsQueryable(); ;

            ViewData["GlFiscalYear"] = db.GLFiscalYears.Select(s => new
            {
                Value = s.GlFiscalYearId,
                Text = s.Name
            }).AsQueryable(); ;
            ViewData["GlPeriod"] = db.GLPeriods.Select(s => new
            {
                Value = s.GLPeriodId,
                Text = s.Name
            }).AsQueryable(); ;
           
            return View();
        }
        private void GenerateDate()
        {
            var details = db.FleetAnnualPlanDetails.ToList();
            foreach (var item in details)
            {
                item.DateFrom = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == item.GlPeriodId).DateFrom;
                item.DateTo = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == item.GlPeriodId).DateTo;
                db.Entry(item).State = EntityState.Modified;
            }
            db.SaveChanges();
        }
        public ActionResult FleetAnnualPlan_Read([DataSourceRequest] DataSourceRequest request)
        {
            var annualPlans = db.FleetAnnualPlans;
            
            return Json(annualPlans.ToDataSourceResult(request, ModelState));
        }

        public ActionResult FleetAnnualPlan_Create([DataSourceRequest] DataSourceRequest request, FleetAnnualPlan model)
        {
            if (ModelState.IsValid)
            {

                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Today;
                db.FleetAnnualPlans.Add(model);
                db.SaveChanges();
                GenerateAnnualPlanDetail(model.FleetAnnualPlanId);
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetAnnualPlan_Update([DataSourceRequest] DataSourceRequest request, FleetAnnualPlan model)
        {
            if (ModelState.IsValid)
            {

                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Today;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetAnnualPlan_Delete([DataSourceRequest] DataSourceRequest request, FleetAnnualPlan model)
        {
            if (ModelState.IsValid)
            {

                var annualPlan = db.FleetAnnualPlans.Find(model.FleetAnnualPlanId);
                var fleetAnnualPlanDetails = db.FleetAnnualPlanDetails.Where(x => x.FleetAnnualPlanId == annualPlan.FleetAnnualPlanId).ToList();
                if (fleetAnnualPlanDetails.Any())
                {
                    db.FleetAnnualPlanDetails.RemoveRange(fleetAnnualPlanDetails);
                    db.SaveChanges();
                }
                db.FleetAnnualPlans.Remove(annualPlan);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void GenerateAnnualPlanDetail(int fleetAnnualPlanId)
        {
            var annualPlan = db.FleetAnnualPlans.Find(fleetAnnualPlanId);
            var glPeriod = db.GLPeriods.Where(x => x.GlFiscalYearId == annualPlan.GlFiscalYearId).ToList();
            foreach (var item in glPeriod)
            {
                var entity = new FleetAnnualPlanDetail
                {
                    GlPeriodId = item.GLPeriodId,
                    FleetAnnualPlanId = annualPlan.FleetAnnualPlanId,
                    DateFrom = item.DateFrom,
                    DateTo = item.DateTo
                };
                db.FleetAnnualPlanDetails.Add(entity);
            }
            db.SaveChanges();
        }

        public JsonResult GenerateNextAnnualPlan()
        {
            var lastPlan = db.FleetAnnualPlans
                             .OrderByDescending(p => p.FleetAnnualPlanId)
                             .FirstOrDefault();

            if (lastPlan == null)
            {
                return Json(new { error = "No annual plan found" }, JsonRequestBehavior.AllowGet);
            }

            int currentFiscalYearId = lastPlan.GlFiscalYearId;
           
            var fetchAllLastPlans = db.FleetAnnualPlans
                                      .Where(x => x.GlFiscalYearId == currentFiscalYearId)
                                      .ToList();
            var planIds = fetchAllLastPlans
                  .Select(x => x.FleetAnnualPlanId)
                  .ToList();

            // Get plan IDs where details count is less than 12
            var missedPlans = db.FleetAnnualPlanDetails
                             .Where(x => planIds.Contains(x.FleetAnnualPlanId))
                             .GroupBy(x => x.FleetAnnualPlanId)
                             .Where(g => g.Count() < 12)
                             .Select(g => g.Key)   // <-- just get the ID
                             .ToList();
            if (missedPlans.Count > 0)
            {
                foreach (var missedPlan in missedPlans)
                {
                    var fetchCurPlan = db.FleetAnnualPlans.FirstOrDefault(x => x.FleetAnnualPlanId == missedPlan);
                    var fetchPrevPlan = db.FleetAnnualPlans
                           .Where(x => x.FleetId == fetchCurPlan.FleetId &&
                                       x.GlFiscalYearId < fetchCurPlan.GlFiscalYearId).OrderByDescending(p => p.GlFiscalYearId).FirstOrDefault();
                    var curDetails = db.FleetAnnualPlanDetails
                            .Where(d => d.FleetAnnualPlanId == fetchCurPlan.FleetAnnualPlanId).ToList();

                    var prevDetails = db.FleetAnnualPlanDetails.Where(d => d.FleetAnnualPlanId == fetchPrevPlan.FleetAnnualPlanId)
                                            .ToList();
                    foreach (var oldPlan in prevDetails)
                    {
                        var period = db.GLPeriods.FirstOrDefault(x => x.GlFiscalYearId == fetchCurPlan.GlFiscalYearId && x.DateFrom.Month == oldPlan.DateFrom.Month);
                        if (period != null)
                        {
                            var fetchDetailFromCur = db.FleetAnnualPlanDetails.FirstOrDefault(x => x.FleetAnnualPlanId == missedPlan && x.GlPeriodId == period.GLPeriodId);
                            if (fetchDetailFromCur == null)
                            {
                                var planModel = new FleetAnnualPlanDetail
                                {
                                    CreatedBy = User.Identity.Name,
                                    DateCreated = DateTime.Now,

                                    // copy values
                                    NumberOfEquipmentInMonth = oldPlan.NumberOfEquipmentInMonth,
                                    WorkingHourPerDay = oldPlan.WorkingHourPerDay,
                                    WorkingDaysInMonth = oldPlan.WorkingDaysInMonth,
                                    WorkingHourPerMonth = oldPlan.WorkingHourPerMonth,
                                    InefficiencyPercentile = oldPlan.InefficiencyPercentile,
                                    EfficiencyPercentile = oldPlan.EfficiencyPercentile,
                                    HourForInefficiencyPerMonth = oldPlan.HourForInefficiencyPerMonth,
                                    ExpectedEffectiveWorkingHrPerMonth = oldPlan.ExpectedEffectiveWorkingHrPerMonth,
                                    ChargingRate = oldPlan.ChargingRate,
                                    ExpectedMonthlyIncome = oldPlan.ExpectedMonthlyIncome,
                                    TyreCost = oldPlan.TyreCost,
                                    LiterPerHour = oldPlan.LiterPerHour,
                                    TotalLiter = oldPlan.TotalLiter,
                                    FuelCostPerLiter = oldPlan.FuelCostPerLiter,
                                    TotalFuelCost = oldPlan.TotalFuelCost,
                                    MaintenanceCost = oldPlan.MaintenanceCost,
                                    Perduim = oldPlan.Perduim,
                                    Salary = oldPlan.Salary,
                                    TotalExpectedExpense = oldPlan.TotalExpectedExpense,
                                    ExpectedTotalProfit = oldPlan.ExpectedTotalProfit,

                                    // FK assignment
                                    FleetAnnualPlanId = fetchCurPlan.FleetAnnualPlanId,
                                    GlPeriodId = period.GLPeriodId,
                                    DateFrom = period.DateFrom,
                                    DateTo = period.DateTo
                                };
                                db.FleetAnnualPlanDetails.Add(planModel);
                                db.SaveChanges();
                                var plan = planModel;
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
                                    dailyPlan.FleetAnnualPlanId = plan.FleetAnnualPlanId;
                                    dailyPlan.FleetAnnualPlanDetailId = plan.FleetAnnualPlanDetailId;
                                    dailyPlan.GlPeriodId = plan.GlPeriodId;
                                        dailyPlan.OperationDate = glPeriod.DateFrom.AddDays(d - 1);
                                        db.FleetAnnualPlanDaily.Add(dailyPlan);
                                        db.SaveChanges();
                                    }

                            }
                            else
                            {

                                fetchDetailFromCur.ModifiedBy = User.Identity.Name;
                                fetchDetailFromCur.LastModified = DateTime.Now;
                                    // copy values
                                fetchDetailFromCur.NumberOfEquipmentInMonth = oldPlan.NumberOfEquipmentInMonth;
                                fetchDetailFromCur.WorkingHourPerDay = oldPlan.WorkingHourPerDay;
                                fetchDetailFromCur.WorkingDaysInMonth = oldPlan.WorkingDaysInMonth;
                                fetchDetailFromCur.WorkingHourPerMonth = oldPlan.WorkingHourPerMonth;
                                fetchDetailFromCur.InefficiencyPercentile = oldPlan.InefficiencyPercentile;
                                fetchDetailFromCur.EfficiencyPercentile = oldPlan.EfficiencyPercentile;
                                fetchDetailFromCur.HourForInefficiencyPerMonth = oldPlan.HourForInefficiencyPerMonth;
                                fetchDetailFromCur.ExpectedEffectiveWorkingHrPerMonth = oldPlan.ExpectedEffectiveWorkingHrPerMonth;
                                fetchDetailFromCur.ChargingRate = oldPlan.ChargingRate;
                                fetchDetailFromCur.ExpectedMonthlyIncome = oldPlan.ExpectedMonthlyIncome;
                                fetchDetailFromCur.TyreCost = oldPlan.TyreCost;
                                fetchDetailFromCur.LiterPerHour = oldPlan.LiterPerHour;
                                fetchDetailFromCur.TotalLiter = oldPlan.TotalLiter;
                                fetchDetailFromCur.FuelCostPerLiter = oldPlan.FuelCostPerLiter;
                                fetchDetailFromCur.TotalFuelCost = oldPlan.TotalFuelCost;
                                fetchDetailFromCur.MaintenanceCost = oldPlan.MaintenanceCost;
                                fetchDetailFromCur.Perduim = oldPlan.Perduim;
                                fetchDetailFromCur.Salary = oldPlan.Salary;
                                fetchDetailFromCur.TotalExpectedExpense = oldPlan.TotalExpectedExpense;
                                fetchDetailFromCur.ExpectedTotalProfit = oldPlan.ExpectedTotalProfit;

                                // FK assignment
                                fetchDetailFromCur.FleetAnnualPlanId = fetchCurPlan.FleetAnnualPlanId;
                                fetchDetailFromCur.GlPeriodId = period.GLPeriodId;
                                fetchDetailFromCur.DateFrom = period.DateFrom;
                                fetchDetailFromCur.DateTo = period.DateTo;
                                    db.Entry(fetchDetailFromCur).State = EntityState.Modified;
                                    db.SaveChanges();

                            }
                        }
                    }

                }
                return Json(new
                {
                    success = true,
                    message = $"Annual plan Updated successfully "
                }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var nextFiscalYear = db.GLFiscalYears
                                  .Where(fy => fy.GlFiscalYearId > currentFiscalYearId)
                                  .OrderBy(fy => fy.GlFiscalYearId)
                                  .FirstOrDefault();

                if (nextFiscalYear == null)
                {
                    return Json(new
                    {
                        success = true,
                        message = $"No Next fiscal found  "
                    }, JsonRequestBehavior.AllowGet);
                }
                foreach (var fiscalPlan in fetchAllLastPlans)
                {
                    var fetchPlanDetailForPreviousYear = db.FleetAnnualPlanDetails.Where(x => x.FleetAnnualPlanId == fiscalPlan.FleetAnnualPlanId).ToList();
                    if (fetchAllLastPlans.Count < 12)
                    {

                    }
                    var model = new FleetAnnualPlan
                    {
                        CreatedBy = User.Identity.Name,
                        DateCreated = DateTime.Today,
                        GlFiscalYearId = nextFiscalYear.GlFiscalYearId,
                        EquipmentCategoryId = fiscalPlan.EquipmentCategoryId,
                        EquipmentNameId = fiscalPlan.EquipmentNameId,
                        EquipmentTypeId = fiscalPlan.EquipmentTypeId,
                        EquipmentGrade = fiscalPlan.EquipmentGrade,
                        FleetId = fiscalPlan.FleetId
                    };

                    db.FleetAnnualPlans.Add(model);
                    db.SaveChanges();
                    var OldPlanDetails = db.FleetAnnualPlanDetails.Where(x => x.FleetAnnualPlanId == fiscalPlan.FleetAnnualPlanId).ToList();
                    var glPeriods = db.GLPeriods
        .Where(x => x.GlFiscalYearId == model.GlFiscalYearId)
        .OrderBy(x => x.DateFrom) // make sure order is correct
        .ToList();

                    int count = 0;
                    List<FleetAnnualPlanDetail> monthlyPlans = new List<FleetAnnualPlanDetail>();
                    foreach (var oldPlan in OldPlanDetails)
                    {
                        if (count >= glPeriods.Count) break; // safety check

                        var period = glPeriods[count];

                        var planModel = new FleetAnnualPlanDetail
                        {
                            CreatedBy = User.Identity.Name,
                            DateCreated = DateTime.Now,

                            // copy values
                            NumberOfEquipmentInMonth = oldPlan.NumberOfEquipmentInMonth,
                            WorkingHourPerDay = oldPlan.WorkingHourPerDay,
                            WorkingDaysInMonth = oldPlan.WorkingDaysInMonth,
                            WorkingHourPerMonth = oldPlan.WorkingHourPerMonth,
                            InefficiencyPercentile = oldPlan.InefficiencyPercentile,
                            EfficiencyPercentile = oldPlan.EfficiencyPercentile,
                            HourForInefficiencyPerMonth = oldPlan.HourForInefficiencyPerMonth,
                            ExpectedEffectiveWorkingHrPerMonth = oldPlan.ExpectedEffectiveWorkingHrPerMonth,
                            ChargingRate = oldPlan.ChargingRate,
                            ExpectedMonthlyIncome = oldPlan.ExpectedMonthlyIncome,
                            TyreCost = oldPlan.TyreCost,
                            LiterPerHour = oldPlan.LiterPerHour,
                            TotalLiter = oldPlan.TotalLiter,
                            FuelCostPerLiter = oldPlan.FuelCostPerLiter,
                            TotalFuelCost = oldPlan.TotalFuelCost,
                            MaintenanceCost = oldPlan.MaintenanceCost,
                            Perduim = oldPlan.Perduim,
                            Salary = oldPlan.Salary,
                            TotalExpectedExpense = oldPlan.TotalExpectedExpense,
                            ExpectedTotalProfit = oldPlan.ExpectedTotalProfit,

                            // FK assignment
                            FleetAnnualPlanId = model.FleetAnnualPlanId,
                            GlPeriodId = period.GLPeriodId,
                            DateFrom = period.DateFrom,
                            DateTo = period.DateTo
                        };
                        db.FleetAnnualPlanDetails.Add(planModel);
                        db.SaveChanges();
                        count++;
                        monthlyPlans.Add(planModel);
                    }
                    foreach (var plan in monthlyPlans)
                    {
                        var glPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == plan.GlPeriodId);
                        int daysInMonth = (glPeriod.DateTo - glPeriod.DateFrom).Days + 1;
                        

                            


                            for (int d=1;d<=daysInMonth;d++)
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
                            dailyPlan.GlPeriodId = plan.GlPeriodId;
                            dailyPlan.FleetAnnualPlanId = plan.FleetAnnualPlanId;
                            dailyPlan.FleetAnnualPlanDetailId = plan.FleetAnnualPlanDetailId;
                            dailyPlan.OperationDate = glPeriod.DateFrom.AddDays(d-1);
                            db.FleetAnnualPlanDaily.Add(dailyPlan);
                            db.SaveChanges();
                        }
                       
                    }
                    }
                    //GenerateAnnualPlanDetail(model.FleetAnnualPlanId);
                
                var fetchFiscalYearName = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == nextFiscalYear.GlFiscalYearId);
                return Json(new
                {
                    success = true,
                    message = $"Annual plan generated successfully for fiscal year {fetchFiscalYearName.Name}"
                }, JsonRequestBehavior.AllowGet);
            }

            //return Json(new
            //{
            //    success = true,
            //    message = $"Annual plan Not generated successfully"
            //}, JsonRequestBehavior.AllowGet);
        }
        
    }
}