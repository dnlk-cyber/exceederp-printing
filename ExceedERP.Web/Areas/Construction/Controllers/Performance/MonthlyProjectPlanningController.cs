using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Helpers.Construction;

namespace ExceedERP.Web.Areas.Construction.Controllers.Performance
{
    public class MonthlyProjectPlanningController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult MonthlyPlanning_Read([DataSourceRequest]DataSourceRequest request, int periodIdReadPlanning)
        {
            IQueryable<MonthlyProjectPlanning> plannings = db.MonthlyProjectPlanning.Where(p => p.PeriodMappingId == periodIdReadPlanning);
            DataSourceResult result = plannings.ToDataSourceResult(request, plan => new
            {
                MonthlyProjectPlanningId = plan.MonthlyProjectPlanningId,
                PeriodMappingId = plan.PeriodMappingId,
                RevScheduleSubAcToManual = plan.RevScheduleSubAcToManual,
                RevScheduleSubAcToContract = plan.RevScheduleSubAcToContract,
                Completness = plan.Completness,
                QualityAndAcceptablity = plan.QualityAndAcceptablity,
                MaxAllowablePoints = plan.MaxAllowablePoints,
                TotalPoints = plan.TotalPoints,

                IsRevisionTimeAccManual = plan.IsRevisionTimeAccManual,
                RevisedSdlSubmitedAccordinglyAccToManual = plan.RevisedSdlSubmitedAccordinglyAccToManual,
                SubmssinTimeAcctoManual = plan.SubmssinTimeAcctoManual,
                DaysAfterOrBeforeDueDateAccToManual = plan.DaysAfterOrBeforeDueDateAccToManual,
                DaysAfterOrBeforeDueDateAccToContract = plan.DaysAfterOrBeforeDueDateAccToContract,
                IsRevisionTimeAccContract = plan.IsRevisionTimeAccContract,
                RevisedSdlSubmitedAccordinglyAccToContract = plan.RevisedSdlSubmitedAccordinglyAccToContract,
                SubmssinTimeAcctoContract = plan.SubmssinTimeAcctoContract,
                WorkSchedule = plan.WorkSchedule,
                CashFlowSchedule = plan.CashFlowSchedule,
                ResourceSchedule = plan.ResourceSchedule,
                BudgetRequirementSchedule = plan.BudgetRequirementSchedule,
                IsApprovalMadeInTheMonth = plan.IsApprovalMadeInTheMonth,
                ApprovalType = plan.ApprovalType,

                OrgBranchName = plan.OrgBranchName,
                DateCreated = plan.DateCreated,
                LastModified = plan.LastModified,
                CreatedBy = plan.CreatedBy,
                ModifiedBy = plan.ModifiedBy
            });

            return Json(result);
        }

        private bool MonthlyProjectPlanningExists(int periodId)
        {
            var valueIsAvailable = false;
            var existingValue = db.MonthlyProjectPlanning.FirstOrDefault(p => p.PeriodMappingId == periodId);
            if (existingValue != null)
            {
                valueIsAvailable = true;
            }
            return valueIsAvailable;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyPlanning_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectPlanning plan, int periodIdCreatePlanning)
        {
            if (ModelState.IsValid)
            {
                var errorExists = false;
                var componentWait = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PLANNING, periodIdCreatePlanning);
                if (componentWait == 0)
                {
                    ModelState.AddModelError("", "Component weight is not set for PLANNING");
                    errorExists = true;
                }
                if (MonthlyProjectPlanningExists(periodIdCreatePlanning))
                {
                    ModelState.AddModelError("", "Planning for this Period exists!!");
                    errorExists = true;
                }
                if(!errorExists)
                {
                    var entity = new MonthlyProjectPlanning
                    {
                        MonthlyProjectPlanningId = plan.MonthlyProjectPlanningId,
                        PeriodMappingId = periodIdCreatePlanning,
                        RevScheduleSubAcToManual = plan.RevScheduleSubAcToManual,
                        RevScheduleSubAcToContract = plan.RevScheduleSubAcToContract,
                        Completness = plan.Completness,
                        QualityAndAcceptablity = plan.QualityAndAcceptablity,
                        MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PLANNING, periodIdCreatePlanning),
                        TotalPoints = plan.TotalPoints,

                        IsRevisionTimeAccManual = plan.IsRevisionTimeAccManual,
                        RevisedSdlSubmitedAccordinglyAccToManual = plan.RevisedSdlSubmitedAccordinglyAccToManual,
                        SubmssinTimeAcctoManual = plan.SubmssinTimeAcctoManual,
                        DaysAfterOrBeforeDueDateAccToManual = plan.DaysAfterOrBeforeDueDateAccToManual,
                        DaysAfterOrBeforeDueDateAccToContract = plan.DaysAfterOrBeforeDueDateAccToContract,
                        IsRevisionTimeAccContract = plan.IsRevisionTimeAccContract,
                        RevisedSdlSubmitedAccordinglyAccToContract = plan.RevisedSdlSubmitedAccordinglyAccToContract,
                        SubmssinTimeAcctoContract = plan.SubmssinTimeAcctoContract,
                        WorkSchedule = plan.WorkSchedule,
                        CashFlowSchedule = plan.CashFlowSchedule,
                        ResourceSchedule = plan.ResourceSchedule,
                        BudgetRequirementSchedule = plan.BudgetRequirementSchedule,
                        IsApprovalMadeInTheMonth = plan.IsApprovalMadeInTheMonth,
                        ApprovalType = plan.ApprovalType,

                        OrgBranchName = plan.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = plan.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = plan.ModifiedBy

                    };
                    entity.RevScheduleSubAcToManual = PerformanceEvnHelper.GetScoreOfScheduleRevAccToManual(entity);
                    entity.RevScheduleSubAcToContract = PerformanceEvnHelper.GetScoreOfScheduleRevAccToContract(entity);
                    entity.Completness = entity.WorkSchedule + entity.ResourceSchedule + entity.CashFlowSchedule +
                                         entity.BudgetRequirementSchedule;
                    entity.QualityAndAcceptablity = PerformanceEvnHelper.GetQualityAndAcceptablityScore(entity);
                    entity.TotalPoints = entity.RevScheduleSubAcToManual + entity.RevScheduleSubAcToContract +
                                         entity.Completness + entity.QualityAndAcceptablity;

                    entity.TotalPoints = entity.TotalPoints > entity.MaxAllowablePoints ? entity.MaxAllowablePoints : entity.TotalPoints;

                    db.MonthlyProjectPlanning.Add(entity);
                    db.SaveChanges();
                    plan.MonthlyProjectPlanningId = entity.MonthlyProjectPlanningId;

                }
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyPlanning_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectPlanning plan)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectPlanning
                {
                    MonthlyProjectPlanningId = plan.MonthlyProjectPlanningId,
                    PeriodMappingId = plan.PeriodMappingId,
                    RevScheduleSubAcToManual = plan.RevScheduleSubAcToManual,
                    RevScheduleSubAcToContract = plan.RevScheduleSubAcToContract,
                    Completness = plan.Completness,
                    QualityAndAcceptablity = plan.QualityAndAcceptablity,
                    MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PLANNING, plan.PeriodMappingId),
                    TotalPoints = plan.TotalPoints,

                    IsRevisionTimeAccManual = plan.IsRevisionTimeAccManual,
                    RevisedSdlSubmitedAccordinglyAccToManual = plan.RevisedSdlSubmitedAccordinglyAccToManual,
                    SubmssinTimeAcctoManual = plan.SubmssinTimeAcctoManual,
                    DaysAfterOrBeforeDueDateAccToManual = plan.DaysAfterOrBeforeDueDateAccToManual,
                    DaysAfterOrBeforeDueDateAccToContract = plan.DaysAfterOrBeforeDueDateAccToContract,
                    IsRevisionTimeAccContract = plan.IsRevisionTimeAccContract,
                    RevisedSdlSubmitedAccordinglyAccToContract = plan.RevisedSdlSubmitedAccordinglyAccToContract,
                    SubmssinTimeAcctoContract = plan.SubmssinTimeAcctoContract,
                    WorkSchedule = plan.WorkSchedule,
                    CashFlowSchedule = plan.CashFlowSchedule,
                    ResourceSchedule = plan.ResourceSchedule,
                    BudgetRequirementSchedule = plan.BudgetRequirementSchedule,
                    IsApprovalMadeInTheMonth = plan.IsApprovalMadeInTheMonth,
                    ApprovalType = plan.ApprovalType,

                    OrgBranchName = plan.OrgBranchName,
                    DateCreated = plan.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = plan.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.RevScheduleSubAcToManual = PerformanceEvnHelper.GetScoreOfScheduleRevAccToManual(entity);
                entity.RevScheduleSubAcToContract = PerformanceEvnHelper.GetScoreOfScheduleRevAccToContract(entity);
                entity.Completness = entity.WorkSchedule + entity.ResourceSchedule + entity.CashFlowSchedule +
                                     entity.BudgetRequirementSchedule;
                entity.QualityAndAcceptablity = PerformanceEvnHelper.GetQualityAndAcceptablityScore(entity);
                entity.TotalPoints = entity.RevScheduleSubAcToManual + entity.RevScheduleSubAcToContract +
                                     entity.Completness + entity.QualityAndAcceptablity;
                entity.TotalPoints = entity.TotalPoints > entity.MaxAllowablePoints ? entity.MaxAllowablePoints : entity.TotalPoints;

                db.MonthlyProjectPlanning.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyPlanning_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectPlanning plan)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectPlanning
                {
                    MonthlyProjectPlanningId = plan.MonthlyProjectPlanningId,
                    PeriodMappingId = plan.PeriodMappingId,
                    RevScheduleSubAcToManual = plan.RevScheduleSubAcToManual,
                    RevScheduleSubAcToContract = plan.RevScheduleSubAcToContract,
                    Completness = plan.Completness,
                    QualityAndAcceptablity = plan.QualityAndAcceptablity,
                    MaxAllowablePoints = plan.MaxAllowablePoints,
                    TotalPoints = plan.TotalPoints,

                    IsRevisionTimeAccManual = plan.IsRevisionTimeAccManual,
                    RevisedSdlSubmitedAccordinglyAccToManual = plan.RevisedSdlSubmitedAccordinglyAccToManual,
                    SubmssinTimeAcctoManual = plan.SubmssinTimeAcctoManual,
                    DaysAfterOrBeforeDueDateAccToManual = plan.DaysAfterOrBeforeDueDateAccToManual,
                    DaysAfterOrBeforeDueDateAccToContract = plan.DaysAfterOrBeforeDueDateAccToContract,
                    IsRevisionTimeAccContract = plan.IsRevisionTimeAccContract,
                    RevisedSdlSubmitedAccordinglyAccToContract = plan.RevisedSdlSubmitedAccordinglyAccToContract,
                    SubmssinTimeAcctoContract = plan.SubmssinTimeAcctoContract,
                    WorkSchedule = plan.WorkSchedule,
                    CashFlowSchedule = plan.CashFlowSchedule,
                    ResourceSchedule = plan.ResourceSchedule,
                    BudgetRequirementSchedule = plan.BudgetRequirementSchedule,
                    IsApprovalMadeInTheMonth = plan.IsApprovalMadeInTheMonth,
                    ApprovalType = plan.ApprovalType,

                    OrgBranchName = plan.OrgBranchName,
                    DateCreated = plan.DateCreated,
                    LastModified = plan.LastModified,
                    CreatedBy = plan.CreatedBy,
                    ModifiedBy = plan.ModifiedBy
                };

                db.MonthlyProjectPlanning.Attach(entity);
                db.MonthlyProjectPlanning.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { plan }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}