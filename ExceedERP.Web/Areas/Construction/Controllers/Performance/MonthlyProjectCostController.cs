using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Performance
{
    public class MonthlyProjectCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult MonthlyCost_Read([DataSourceRequest]DataSourceRequest request, int periodIdReadCost)
        {
            IQueryable<MonthlyProjectCost> costs = db.MonthlyProjectCost.Where(x => x.PeriodMappingId == periodIdReadCost);
            DataSourceResult result = costs.ToDataSourceResult(request, cost => new
            {
                MonthlyProjectCostId = cost.MonthlyProjectCostId,
                PeriodMappingId = cost.PeriodMappingId,
                PlannedDirectCost = cost.PlannedDirectCost,
                PlannedProjectOHCost = cost.PlannedProjectOHCost,
                PlannedProfitAndOHCost = cost.PlannedProfitAndOHCost,
                ActualDirectCost = cost.ActualDirectCost,
                ActualProjectOHCost = cost.ActualProjectOHCost,
                ActualProfitAndOHCost = cost.ActualProfitAndOHCost,
                PlannedDirectCostToDate = cost.PlannedDirectCostToDate,
                PlannedProjectOHCostToDate = cost.PlannedProjectOHCostToDate,
                PlannedProfitAndOHToDate = cost.PlannedProfitAndOHToDate,
                ActualDirectCostTillPrevMonth = cost.ActualDirectCostTillPrevMonth,
                ActualDirectCostToDate = cost.ActualDirectCostToDate,
                ActualProjectOHCostTillPrevMonth = cost.ActualProjectOHCostTillPrevMonth,
                ActualProjectOHCostToDate = cost.ActualProjectOHCostToDate,
                ActualProfitAndOHTillPrevMonth = cost.ActualProfitAndOHTillPrevMonth,
                ActualProfitAndOHToDate = cost.ActualProfitAndOHToDate,
                CostTracking = cost.CostTracking,

                EquipmentUtilizationEfficiency = cost.EquipmentUtilizationEfficiency,
                FuelUtilizationBasedOnStandard = cost.FuelUtilizationBasedOnStandard,
                CostsFromExpectation = cost.CostsFromExpectation,
                ProductivitiyOfLaborAndEquipmentMeaure = cost.ProductivitiyOfLaborAndEquipmentMeaure,
                ProperAndAccurateEarnedValueAnalysis = cost.ProperAndAccurateEarnedValueAnalysis,
                ReusingOfConstructionMaterials = cost.ReusingOfConstructionMaterials,

                OrgBranchName = cost.OrgBranchName,
                DateCreated = cost.DateCreated,
                LastModified = cost.LastModified,
                CreatedBy = cost.CreatedBy,
                ModifiedBy = cost.ModifiedBy

            });

            return Json(result);
        }
        private bool MonthlyProjectCostExists(int periodId)
        {
            var valueIsAvailable = false;
            var existingValue = db.MonthlyProjectCost.FirstOrDefault(p => p.PeriodMappingId == periodId);
            if (existingValue != null)
            {
                valueIsAvailable = true;
            }
            return valueIsAvailable;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyCost_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectCost cost, int periodIdCreateCost)
        {
            if (ModelState.IsValid)
            {
                if (MonthlyProjectCostExists(periodIdCreateCost))
                {
                    ModelState.AddModelError("", "Cost for this Period exists!!");
                }
                else
                {
                    var entity = new MonthlyProjectCost
                    {
                        //MonthlyProjectCostId = cost.MonthlyProjectCostId,
                        PeriodMappingId = periodIdCreateCost,
                        PlannedDirectCost = PerformanceEvnHelper.GetPlannedProjectDirectCostForMonth(periodIdCreateCost),
                        PlannedProjectOHCost = PerformanceEvnHelper.GetPlannedProjectOverHeadCost(periodIdCreateCost),
                        PlannedProfitAndOHCost = PerformanceEvnHelper.GetPlannedProjectProfitAndOHCost(periodIdCreateCost),
                        ActualDirectCost = cost.ActualDirectCost,
                        ActualProjectOHCost = cost.ActualProjectOHCost,
                        ActualProfitAndOHCost = PerformanceEvnHelper.GetActualProfitAndHeadOfficeOH(cost, periodIdCreateCost),
                        PlannedDirectCostToDate = PerformanceEvnHelper.GetPlannedDirectCostTodate(periodIdCreateCost),
                        PlannedProjectOHCostToDate = PerformanceEvnHelper.GetPlannedOHcostToDate(periodIdCreateCost),
                        PlannedProfitAndOHToDate = PerformanceEvnHelper.GetPlannedProfitAndHeadOfficeOHToDate(periodIdCreateCost),
                        ActualDirectCostTillPrevMonth = cost.ActualDirectCostTillPrevMonth,
                        ActualDirectCostToDate = cost.ActualDirectCost + cost.ActualDirectCostTillPrevMonth,
                        ActualProjectOHCostTillPrevMonth = cost.ActualProjectOHCostTillPrevMonth,
                        ActualProjectOHCostToDate = cost.ActualProjectOHCost + cost.ActualProjectOHCostTillPrevMonth,
                        ActualProfitAndOHTillPrevMonth = cost.ActualProfitAndOHTillPrevMonth,
                        //ActualProfitAndOHToDate = PerformanceEvnHelper.GetActualProfitAndOHCostToDate(cost, cost.PeriodMappingId),
                        CostTracking = cost.CostTracking,

                        EquipmentUtilizationEfficiency = cost.EquipmentUtilizationEfficiency,
                        FuelUtilizationBasedOnStandard = cost.FuelUtilizationBasedOnStandard,
                        CostsFromExpectation = cost.CostsFromExpectation,
                        ProductivitiyOfLaborAndEquipmentMeaure = cost.ProductivitiyOfLaborAndEquipmentMeaure,
                        ProperAndAccurateEarnedValueAnalysis = cost.ProperAndAccurateEarnedValueAnalysis,
                        ReusingOfConstructionMaterials = cost.ReusingOfConstructionMaterials,

                        OrgBranchName = cost.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = cost.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = cost.ModifiedBy
                    };
                    entity.ActualProfitAndOHToDate = PerformanceEvnHelper.GetActualProfitAndOHCostToDate(entity, entity.PeriodMappingId);
                    //entity.PlannedToDate = entity.PlannedAmount + entity.PlannedAmountTillPreviousMonth;
                    //entity.ExecutedAmountToDate = entity.ExecutedAmount + entity.ExecutedAmountTillPreviousMonth;

                    db.MonthlyProjectCost.Add(entity);
                    db.SaveChanges();
                    cost.MonthlyProjectCostId = entity.MonthlyProjectCostId;

                }
            }

            return Json(new[] { cost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyCost_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectCost cost)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCost
                {
                    MonthlyProjectCostId = cost.MonthlyProjectCostId,
                    PeriodMappingId = cost.PeriodMappingId,
                    PlannedDirectCost = PerformanceEvnHelper.GetPlannedProjectDirectCostForMonth(cost.PeriodMappingId),
                    PlannedProjectOHCost = PerformanceEvnHelper.GetPlannedProjectOverHeadCost(cost.PeriodMappingId),
                    PlannedProfitAndOHCost = PerformanceEvnHelper.GetPlannedProjectProfitAndOHCost(cost.PeriodMappingId),
                    ActualDirectCost = cost.ActualDirectCost,
                    ActualProjectOHCost = cost.ActualProjectOHCost,
                    ActualProfitAndOHCost = PerformanceEvnHelper.GetActualProfitAndHeadOfficeOH(cost, cost.PeriodMappingId),
                    PlannedDirectCostToDate = PerformanceEvnHelper.GetPlannedDirectCostTodate(cost.PeriodMappingId),
                    PlannedProjectOHCostToDate = PerformanceEvnHelper.GetPlannedOHcostToDate(cost.PeriodMappingId),
                    PlannedProfitAndOHToDate = PerformanceEvnHelper.GetPlannedProfitAndHeadOfficeOHToDate(cost.PeriodMappingId),
                    ActualDirectCostTillPrevMonth = cost.ActualDirectCostTillPrevMonth,
                    ActualDirectCostToDate = cost.ActualDirectCost + cost.ActualDirectCostTillPrevMonth,
                    ActualProjectOHCostTillPrevMonth = cost.ActualProjectOHCostTillPrevMonth,
                    ActualProjectOHCostToDate = cost.ActualProjectOHCost + cost.ActualProjectOHCostTillPrevMonth,
                    ActualProfitAndOHTillPrevMonth = cost.ActualProfitAndOHTillPrevMonth,
                    //ActualProfitAndOHToDate = PerformanceEvnHelper.GetActualProfitAndOHCostToDate(cost, cost.PeriodMappingId),
                    CostTracking = cost.CostTracking,

                    EquipmentUtilizationEfficiency = cost.EquipmentUtilizationEfficiency,
                    FuelUtilizationBasedOnStandard = cost.FuelUtilizationBasedOnStandard,
                    CostsFromExpectation = cost.CostsFromExpectation,
                    ProductivitiyOfLaborAndEquipmentMeaure = cost.ProductivitiyOfLaborAndEquipmentMeaure,
                    ProperAndAccurateEarnedValueAnalysis = cost.ProperAndAccurateEarnedValueAnalysis,
                    ReusingOfConstructionMaterials = cost.ReusingOfConstructionMaterials,

                    OrgBranchName = cost.OrgBranchName,
                    DateCreated = cost.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = cost.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.ActualProfitAndOHToDate = PerformanceEvnHelper.GetActualProfitAndOHCostToDate(entity, entity.PeriodMappingId);
                //entity.PlannedToDate = entity.PlannedAmount + entity.PlannedAmountTillPreviousMonth;
                //entity.ExecutedAmountToDate = entity.ExecutedAmount + entity.ExecutedAmountTillPreviousMonth;

                db.MonthlyProjectCost.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { cost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyCost_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectCost cost)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCost
                {
                    MonthlyProjectCostId = cost.MonthlyProjectCostId,
                    PeriodMappingId = cost.PeriodMappingId,
                    PlannedDirectCost = cost.PlannedDirectCost,
                    PlannedProjectOHCost = cost.PlannedProjectOHCost,
                    PlannedProfitAndOHCost = cost.PlannedProfitAndOHCost,
                    ActualDirectCost = cost.ActualDirectCost,
                    ActualProjectOHCost = cost.ActualProjectOHCost,
                    ActualProfitAndOHCost = cost.ActualProfitAndOHCost,
                    PlannedDirectCostToDate = cost.PlannedDirectCostToDate,
                    PlannedProjectOHCostToDate = cost.PlannedProjectOHCostToDate,
                    PlannedProfitAndOHToDate = cost.PlannedProfitAndOHToDate,
                    ActualDirectCostTillPrevMonth = cost.ActualDirectCostTillPrevMonth,
                    ActualDirectCostToDate = cost.ActualDirectCostToDate,
                    ActualProjectOHCostTillPrevMonth = cost.ActualProjectOHCostTillPrevMonth,
                    ActualProjectOHCostToDate = cost.ActualProjectOHCostToDate,
                    ActualProfitAndOHTillPrevMonth = cost.ActualProfitAndOHTillPrevMonth,
                    ActualProfitAndOHToDate = cost.ActualProfitAndOHToDate,
                    CostTracking = cost.CostTracking,

                    EquipmentUtilizationEfficiency = cost.EquipmentUtilizationEfficiency,
                    FuelUtilizationBasedOnStandard = cost.FuelUtilizationBasedOnStandard,
                    CostsFromExpectation = cost.CostsFromExpectation,
                    ProductivitiyOfLaborAndEquipmentMeaure = cost.ProductivitiyOfLaborAndEquipmentMeaure,
                    ProperAndAccurateEarnedValueAnalysis = cost.ProperAndAccurateEarnedValueAnalysis,
                    ReusingOfConstructionMaterials = cost.ReusingOfConstructionMaterials,

                    OrgBranchName = cost.OrgBranchName,
                    DateCreated = cost.DateCreated,
                    LastModified = cost.LastModified,
                    CreatedBy = cost.CreatedBy,
                    ModifiedBy = cost.ModifiedBy
                };

                db.MonthlyProjectCost.Attach(entity);
                db.MonthlyProjectCost.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cost }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}