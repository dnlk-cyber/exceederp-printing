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
    public class MonthlyProgressController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/MonthlyProgress
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MonthlyProgress_Read([DataSourceRequest]DataSourceRequest request, int periodIdRead)
        {
            IQueryable<MonthlyProjectProgress> progresses = db.MonthlyProjectProgress.Where(x => x.PeriodMappingId == periodIdRead);
            DataSourceResult result = progresses.ToDataSourceResult(request, progress => new
            {
                MonthlyProjectProgressId = progress.MonthlyProjectProgressId,
                PeriodMappingId = progress.PeriodMappingId,
                //ProjectInformationId = progress.ProjectInformationId,
                PlannedAmount = progress.PlannedAmount,
                PlannedAmountTillPreviousMonth = progress.PlannedAmountTillPreviousMonth,
                PlannedToDate = progress.PlannedToDate,
                ExecutedAmount = progress.ExecutedAmount,
                ExecutedAmountTillPreviousMonth = progress.ExecutedAmountTillPreviousMonth,
                ExecutedAmountToDate = progress.ExecutedAmountToDate,
                ExecutedAmountToDateIncludingPriceAdjustment = progress.ExecutedAmountToDateIncludingPriceAdjustment,
                PriceAdjustmentThisMonth = progress.PriceAdjustmentThisMonth,
                PriceAdjustmentTillPrevMonth = progress.PriceAdjustmentTillPrevMonth,
                PriceAdjustmentToDate = progress.PriceAdjustmentToDate,
                ScheduleBrokenDownToWeeklyBase = progress.ScheduleBrokenDownToWeeklyBase,
                AreFormenAwareOfIt = progress.AreFormenAwareOfIt,
                ActionPlanIsPreparedWithAmountAndProblemsAreAddressed = progress.ActionPlanIsPreparedWithAmountAndProblemsAreAddressed,
                WellDefinedTeamStructureExists = progress.WellDefinedTeamStructureExists,
                NumberOfEvaluationMeething = progress.NumberOfEvaluationMeething,
                CorrectionFactor = progress.CorrectionFactor,

                OrgBranchName = progress.OrgBranchName,
                DateCreated = progress.DateCreated,
                LastModified = progress.LastModified,
                CreatedBy = progress.CreatedBy,
                ModifiedBy = progress.ModifiedBy

            });

            return Json(result);
        }
        private bool MonthlyProgressExists(int periodId)
        {
            var progressIsAvailable = false;
            var existingProgress = db.MonthlyProjectProgress.Where(p => p.PeriodMappingId == periodId).FirstOrDefault();
            if (existingProgress != null)
            {
                progressIsAvailable = true;
            }
            return progressIsAvailable;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProgress_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectProgress progress, int periodIdCreate)
        {
            if (ModelState.IsValid)
            {
                if (MonthlyProgressExists(periodIdCreate))
                {
                    ModelState.AddModelError("", "Project Progress for this Period is available already!!!");
                }
                else
                {
                    var entity = new MonthlyProjectProgress
                    {
                        PeriodMappingId = periodIdCreate,
                        PlannedAmount = progress.PlannedAmount,
                        PlannedAmountTillPreviousMonth = progress.PlannedAmountTillPreviousMonth,
                        PlannedToDate = progress.PlannedToDate,
                        ExecutedAmount = progress.ExecutedAmount,
                        ExecutedAmountTillPreviousMonth = progress.ExecutedAmountTillPreviousMonth,
                        ExecutedAmountToDate = progress.ExecutedAmountToDate,
                        ExecutedAmountToDateIncludingPriceAdjustment = progress.ExecutedAmountToDateIncludingPriceAdjustment,
                        PriceAdjustmentThisMonth = progress.PriceAdjustmentThisMonth,
                        PriceAdjustmentTillPrevMonth = progress.PriceAdjustmentTillPrevMonth,
                        PriceAdjustmentToDate = progress.PriceAdjustmentToDate,
                        ScheduleBrokenDownToWeeklyBase = progress.ScheduleBrokenDownToWeeklyBase,
                        AreFormenAwareOfIt = progress.AreFormenAwareOfIt,
                        ActionPlanIsPreparedWithAmountAndProblemsAreAddressed = progress.ActionPlanIsPreparedWithAmountAndProblemsAreAddressed,
                        WellDefinedTeamStructureExists = progress.WellDefinedTeamStructureExists,
                        NumberOfEvaluationMeething = progress.NumberOfEvaluationMeething,
                        CorrectionFactor = PerformanceEvnHelper.GetMonthlyProgressCorrectionFactor(progress),

                        OrgBranchName = progress.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = progress.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = progress.ModifiedBy
                    };
                    // check if the sum of the planned for this month and planned till previous month
                    // doesn't exceed the total project amount
                    var totalProjectAmnt = ContractMgmtHelper.GetProjectTotalAmount(entity.PeriodMappingId);
                    var sumOfToDateAndCurrentMonthTotal = entity.PlannedAmount + entity.PlannedAmountTillPreviousMonth;
                    entity.PlannedToDate = totalProjectAmnt < sumOfToDateAndCurrentMonthTotal ?
                                            totalProjectAmnt : sumOfToDateAndCurrentMonthTotal;

                    entity.ExecutedAmountToDate = entity.ExecutedAmount + entity.ExecutedAmountTillPreviousMonth;
                    entity.PriceAdjustmentToDate = entity.PriceAdjustmentThisMonth + entity.PriceAdjustmentTillPrevMonth;
                    entity.ExecutedAmountToDateIncludingPriceAdjustment = entity.ExecutedAmountToDate + entity.PriceAdjustmentToDate;

                    db.MonthlyProjectProgress.Add(entity);
                    db.SaveChanges();
                    progress.MonthlyProjectProgressId = entity.MonthlyProjectProgressId;

                }
            }

            return Json(new[] { progress }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProgress_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectProgress progress)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectProgress
                {
                    MonthlyProjectProgressId = progress.MonthlyProjectProgressId,
                    PeriodMappingId = progress.PeriodMappingId,
                    //ProjectInformationId = progress.ProjectInformationId,
                    PlannedAmount = progress.PlannedAmount,
                    PlannedAmountTillPreviousMonth = progress.PlannedAmountTillPreviousMonth,
                    PlannedToDate = progress.PlannedToDate,
                    ExecutedAmount = progress.ExecutedAmount,
                    ExecutedAmountTillPreviousMonth = progress.ExecutedAmountTillPreviousMonth,
                    ExecutedAmountToDate = progress.ExecutedAmountToDate,
                    ExecutedAmountToDateIncludingPriceAdjustment = progress.ExecutedAmountToDateIncludingPriceAdjustment,
                    PriceAdjustmentThisMonth = progress.PriceAdjustmentThisMonth,
                    PriceAdjustmentTillPrevMonth = progress.PriceAdjustmentTillPrevMonth,
                    PriceAdjustmentToDate = progress.PriceAdjustmentToDate,
                    ScheduleBrokenDownToWeeklyBase = progress.ScheduleBrokenDownToWeeklyBase,
                    AreFormenAwareOfIt = progress.AreFormenAwareOfIt,
                    ActionPlanIsPreparedWithAmountAndProblemsAreAddressed = progress.ActionPlanIsPreparedWithAmountAndProblemsAreAddressed,
                    WellDefinedTeamStructureExists = progress.WellDefinedTeamStructureExists,
                    NumberOfEvaluationMeething = progress.NumberOfEvaluationMeething,
                    CorrectionFactor = PerformanceEvnHelper.GetMonthlyProgressCorrectionFactor(progress),

                    OrgBranchName = progress.OrgBranchName,
                    DateCreated = progress.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = progress.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.PlannedToDate = entity.PlannedAmount + entity.PlannedAmountTillPreviousMonth;
                entity.ExecutedAmountToDate = entity.ExecutedAmount + entity.ExecutedAmountTillPreviousMonth;
                entity.PriceAdjustmentToDate = entity.PriceAdjustmentThisMonth + entity.PriceAdjustmentTillPrevMonth;
                entity.ExecutedAmountToDateIncludingPriceAdjustment = entity.ExecutedAmountToDate + entity.PriceAdjustmentToDate;

                db.MonthlyProjectProgress.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { progress }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProgress_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectProgress progress)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectProgress
                {
                    MonthlyProjectProgressId = progress.MonthlyProjectProgressId,
                    PeriodMappingId = progress.PeriodMappingId,
                    //ProjectInformationId = progress.ProjectInformationId,
                    PlannedAmount = progress.PlannedAmount,
                    PlannedAmountTillPreviousMonth = progress.PlannedAmountTillPreviousMonth,
                    PlannedToDate = progress.PlannedToDate,
                    ExecutedAmount = progress.ExecutedAmount,
                    ExecutedAmountTillPreviousMonth = progress.ExecutedAmountTillPreviousMonth,
                    ExecutedAmountToDate = progress.ExecutedAmountToDate,
                    ExecutedAmountToDateIncludingPriceAdjustment = progress.ExecutedAmountToDateIncludingPriceAdjustment,
                    PriceAdjustmentThisMonth = progress.PriceAdjustmentThisMonth,
                    PriceAdjustmentTillPrevMonth = progress.PriceAdjustmentTillPrevMonth,
                    PriceAdjustmentToDate = progress.PriceAdjustmentToDate,
                    ScheduleBrokenDownToWeeklyBase = progress.ScheduleBrokenDownToWeeklyBase,
                    AreFormenAwareOfIt = progress.AreFormenAwareOfIt,
                    ActionPlanIsPreparedWithAmountAndProblemsAreAddressed = progress.ActionPlanIsPreparedWithAmountAndProblemsAreAddressed,
                    WellDefinedTeamStructureExists = progress.WellDefinedTeamStructureExists,
                    NumberOfEvaluationMeething = progress.NumberOfEvaluationMeething,
                    CorrectionFactor = progress.CorrectionFactor,
                    OrgBranchName = progress.OrgBranchName,
                    DateCreated = progress.DateCreated,
                    LastModified = progress.LastModified,
                    CreatedBy = progress.CreatedBy,
                    ModifiedBy = progress.ModifiedBy
                };

                db.MonthlyProjectProgress.Attach(entity);
                db.MonthlyProjectProgress.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { progress }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}