using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Construction;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Performance
{
    public class MonthlyProjectPerformanceModifierController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult Modifier_Read([DataSourceRequest]DataSourceRequest request, int pIdReadModifier)
        {
            IQueryable<MonthlyProjectPerformanceModifier> modifiers = db.MonthlyProjectPerformanceModifier.Where(x => x.PeriodMappingId == pIdReadModifier);
            DataSourceResult result = modifiers.ToDataSourceResult(request, modifier => new
            {
                MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                PeriodMappingId = modifier.PeriodMappingId,
                HealthAndSafety = modifier.HealthAndSafety,
                BudgetRequestAndUtilization = modifier.BudgetRequestAndUtilization,
                MaterialUtilization = modifier.MaterialUtilization,
                ConductQualityAssurance = modifier.ConductQualityAssurance,
                SubContractAdministration = modifier.SubContractAdministration,
                MaxAllowablePoints = modifier.MaxAllowablePoints,
                TotalPoints = modifier.TotalPoints,

                HeavyInjuriesCount = modifier.HeavyInjuriesCount,
                LightInjuriesCount = modifier.LightInjuriesCount,
                ForcastedBudget = modifier.ForcastedBudget,
                RequestedBudget = modifier.RequestedBudget,
                RequestDeviation = modifier.RequestDeviation,
                PointFromReqDeviation = modifier.PointFromReqDeviation,
                UtilizedBudget = modifier.UtilizedBudget,
                UtilizationDeviation = modifier.UtilizationDeviation,
                PointFromUtlDeviation = modifier.PointFromUtlDeviation,
                //Cement = modifier.Cement,
                //ReadyMixConcrete = modifier.ReadyMixConcrete,
                //ReinforcementBar = modifier.ReinforcementBar,
                //FormWork = modifier.FormWork,
                //Sand = modifier.Sand,
                //Ceramic = modifier.Ceramic,
                PRPImplementationLevel = modifier.PRPImplementationLevel,
                ReworkHappened = modifier.ReworkHappened,
                ConsultantComplaint = modifier.ConsultantComplaint,
                DefectOccured = modifier.DefectOccured,

                SiteAccess = modifier.SiteAccess,
                LackOfGoodGovernance = modifier.LackOfGoodGovernance,
                InactionForSubContractor = modifier.InactionForSubContractor,
                DelayedPayment = modifier.DelayedPayment,


                OrgBranchName = modifier.OrgBranchName,
                DateCreated = modifier.DateCreated,
                LastModified = modifier.LastModified,
                CreatedBy = modifier.CreatedBy,
                ModifiedBy = modifier.ModifiedBy
            });

            return Json(result);
        }
        private bool MonthlyProjectModifierExists(int periodId)
        {
            var valueIsAvailable = false;
            var existingValue = db.MonthlyProjectPerformanceModifier.FirstOrDefault(p => p.PeriodMappingId == periodId);
            if (existingValue != null)
            {
                valueIsAvailable = true;
            }
            return valueIsAvailable;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectPerformanceModifier modifier, int pIdCreateModifier, AvailableProjectTypes projectType)
        {
            if (ModelState.IsValid)
            {
                if (MonthlyProjectModifierExists(pIdCreateModifier))
                {
                    ModelState.AddModelError("", "Modifiers for this period exist");
                }
                else
                {
                    var entity = new MonthlyProjectPerformanceModifier
                    {
                        //MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                        PeriodMappingId = pIdCreateModifier,
                        HealthAndSafety = projectType != AvailableProjectTypes.Road ? (-5 * modifier.HeavyInjuriesCount) + (-1 * modifier.LightInjuriesCount) : (-2 * modifier.HeavyInjuriesCount) + (-1 * modifier.LightInjuriesCount),
                        BudgetRequestAndUtilization = modifier.BudgetRequestAndUtilization,
                        MaterialUtilization = modifier.MaterialUtilization,
                        ConductQualityAssurance = modifier.ReworkHappened + modifier.ConsultantComplaint + modifier.DefectOccured,
                        SubContractAdministration = modifier.SubContractAdministration,
                        MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIERS),
                        TotalPoints = modifier.TotalPoints,

                        HeavyInjuriesCount = modifier.HeavyInjuriesCount,
                        LightInjuriesCount = modifier.LightInjuriesCount,
                        ForcastedBudget = modifier.ForcastedBudget,
                        RequestedBudget = modifier.RequestedBudget,
                        RequestDeviation = this.CalculateRequestDeviation(modifier),
                        PointFromReqDeviation = modifier.PointFromReqDeviation,
                        UtilizedBudget = modifier.UtilizedBudget,
                        UtilizationDeviation = this.CalculateUtilizationDeviation(modifier),
                        PointFromUtlDeviation = modifier.PointFromUtlDeviation,
                        //Cement = modifier.Cement,
                        //ReadyMixConcrete = modifier.ReadyMixConcrete,
                        //ReinforcementBar = modifier.ReinforcementBar,
                        //FormWork = modifier.FormWork,
                        //Sand = modifier.Sand,
                        //Ceramic = modifier.Ceramic,
                        PRPImplementationLevel = modifier.PRPImplementationLevel,
                        ReworkHappened = modifier.ReworkHappened,
                        ConsultantComplaint = modifier.ConsultantComplaint,
                        DefectOccured = modifier.DefectOccured,

                        SiteAccess = modifier.SiteAccess < 0 ? modifier.SiteAccess : 0,
                        LackOfGoodGovernance = modifier.LackOfGoodGovernance < 0 ? modifier.LackOfGoodGovernance : 0,
                        InactionForSubContractor = modifier.InactionForSubContractor < 0 ? modifier.InactionForSubContractor : 0,
                        DelayedPayment = modifier.DelayedPayment < 0 ? modifier.DelayedPayment : 0,

                        OrgBranchName = modifier.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = modifier.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = modifier.ModifiedBy
                    };
                    // check if the vaules are within the specified rages
                    var healthAndSafetyWeight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_HEALTH_AND_SAFETY);
                    var budgetUtilWeight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_BUDGET_UTILIZATION);
                    //var materialUtilWeight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_MATERIAL_UTILIZATION);
                    var subContractAdminWeight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_SUBCONTRACT_ADMINISTRATION);



                    if (projectType != AvailableProjectTypes.Road)
                    {
                        entity.PointFromReqDeviation = entity.RequestDeviation > 15 ? -2 : 0;
                        entity.PointFromUtlDeviation = entity.UtilizationDeviation > 15 ? -3 : 0;

                       
                    }
                    else
                    {
                        entity.PointFromReqDeviation = entity.RequestDeviation > 15 ? (decimal)-1.5 : 0;
                        entity.PointFromUtlDeviation = entity.UtilizationDeviation > 15 ? (decimal)-1.5 : 0;

                        // if there is no accident at all, +3 marks in road projects performance evaluation.
                        if (entity.HeavyInjuriesCount == 0 && entity.LightInjuriesCount == 0)
                        {
                            entity.HealthAndSafety = 3;
                        }
                    }
                    entity.BudgetRequestAndUtilization = entity.PointFromReqDeviation + entity.PointFromUtlDeviation;
                    //entity.MaterialUtilization = PerformanceEvnHelper.CalculateMaterialUtilizationScore(entity);
                    entity.SubContractAdministration = entity.SiteAccess +
                                                        entity.InactionForSubContractor +
                                                        entity.LackOfGoodGovernance +
                                                        entity.DelayedPayment;

                    if (entity.HealthAndSafety < (-1 * healthAndSafetyWeight))
                    {
                        entity.HealthAndSafety = -1 * healthAndSafetyWeight;
                    }
                    if (entity.SubContractAdministration < (-1 * subContractAdminWeight))
                    {
                        entity.SubContractAdministration = -1 * subContractAdminWeight;
                    }
                    if (entity.BudgetRequestAndUtilization < (-1 * budgetUtilWeight))
                    {
                        entity.BudgetRequestAndUtilization = -1 * budgetUtilWeight;
                    }
                    //if(entity.MaterialUtilization < (-1 * materialUtilWeight))
                    //{
                    //    entity.MaterialUtilization = -1 * materialUtilWeight;
                    //}

                    if (projectType == AvailableProjectTypes.Road)
                    {
                        entity.MaterialUtilization = this.CalculatePRPScore(modifier, projectType);
                    }

                    entity.TotalPoints = entity.HealthAndSafety + entity.BudgetRequestAndUtilization +
                                        entity.MaterialUtilization + entity.ConductQualityAssurance + entity.SubContractAdministration;


                    if (entity.MaxAllowablePoints > 0)
                    {
                        entity.MaxAllowablePoints = -1 * entity.MaxAllowablePoints;
                    }
                    if (entity.TotalPoints < entity.MaxAllowablePoints)
                    { entity.TotalPoints = entity.MaxAllowablePoints; }

                    db.MonthlyProjectPerformanceModifier.Add(entity);
                    db.SaveChanges();
                    modifier.MonthlyProjectPerformanceModifierId = entity.MonthlyProjectPerformanceModifierId;

                }

            }
            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectPerformanceModifier modifier, AvailableProjectTypes projectType)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectPerformanceModifier
                {
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    PeriodMappingId = modifier.PeriodMappingId,
                    HealthAndSafety = projectType != AvailableProjectTypes.Road ? (-5 * modifier.HeavyInjuriesCount) + (-1 * modifier.LightInjuriesCount) : (-2 * modifier.HeavyInjuriesCount) + (-1 * modifier.LightInjuriesCount),
                    BudgetRequestAndUtilization = modifier.BudgetRequestAndUtilization,
                    MaterialUtilization = modifier.MaterialUtilization,
                    ConductQualityAssurance = modifier.ReworkHappened + modifier.ConsultantComplaint + modifier.DefectOccured,
                    SubContractAdministration = modifier.SubContractAdministration,
                    MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIERS),
                    TotalPoints = modifier.TotalPoints,

                    HeavyInjuriesCount = modifier.HeavyInjuriesCount,
                    LightInjuriesCount = modifier.LightInjuriesCount,
                    ForcastedBudget = modifier.ForcastedBudget,
                    RequestedBudget = modifier.RequestedBudget,
                    RequestDeviation = this.CalculateRequestDeviation(modifier),
                    PointFromReqDeviation = modifier.PointFromReqDeviation,
                    UtilizedBudget = modifier.UtilizedBudget,
                    UtilizationDeviation = this.CalculateUtilizationDeviation(modifier),
                    PointFromUtlDeviation = modifier.PointFromUtlDeviation,
                    //Cement = modifier.Cement,
                    //ReadyMixConcrete = modifier.ReadyMixConcrete,
                    //ReinforcementBar = modifier.ReinforcementBar,
                    //FormWork = modifier.FormWork,
                    //Sand = modifier.Sand,
                    //Ceramic = modifier.Ceramic,
                    PRPImplementationLevel = modifier.PRPImplementationLevel,
                    ReworkHappened = modifier.ReworkHappened,
                    ConsultantComplaint = modifier.ConsultantComplaint,
                    DefectOccured = modifier.DefectOccured,

                    SiteAccess = modifier.SiteAccess < 0 ? modifier.SiteAccess : 0,
                    LackOfGoodGovernance = modifier.LackOfGoodGovernance < 0 ? modifier.LackOfGoodGovernance : 0,
                    InactionForSubContractor = modifier.InactionForSubContractor < 0 ? modifier.InactionForSubContractor : 0,
                    DelayedPayment = modifier.DelayedPayment < 0 ? modifier.DelayedPayment : 0,

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                // check for range
                var healthAndSafetyWeight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_HEALTH_AND_SAFETY);
                var budgetUtilWeight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_BUDGET_UTILIZATION);
                //var materialUtilWeight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_MATERIAL_UTILIZATION);
                var subContractAdminWeight = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.MODIFIER_SUBCONTRACT_ADMINISTRATION);

                if (projectType != AvailableProjectTypes.Road)
                {
                    entity.PointFromReqDeviation = entity.RequestDeviation > 15 ? -2 : 0;
                    entity.PointFromUtlDeviation = entity.UtilizationDeviation > 15 ? -3 : 0;
                }
                else
                {
                    entity.PointFromReqDeviation = entity.RequestDeviation > 15 ? (decimal)-1.5 : 0;
                    entity.PointFromUtlDeviation = entity.UtilizationDeviation > 15 ? (decimal)-1.5 : 0;

                    // if there is no accident at all, +3 marks in road projects performance evaluation.
                    if (entity.HeavyInjuriesCount == 0 && entity.LightInjuriesCount == 0)
                    {
                        entity.HealthAndSafety = 3;
                    }
                }
                entity.BudgetRequestAndUtilization = entity.PointFromReqDeviation + entity.PointFromUtlDeviation;
                //entity.MaterialUtilization = PerformanceEvnHelper.CalculateMaterialUtilizationScore(entity);
                entity.SubContractAdministration = entity.SiteAccess +
                                                    entity.InactionForSubContractor +
                                                    entity.LackOfGoodGovernance +
                                                    entity.DelayedPayment;

                #region check if values are within setting rages
                if (entity.HealthAndSafety < (-1 * healthAndSafetyWeight))
                {
                    entity.HealthAndSafety = -1 * healthAndSafetyWeight;
                }
                if (entity.SubContractAdministration < (-1 * subContractAdminWeight))
                {
                    entity.SubContractAdministration = -1 * subContractAdminWeight;
                }
                if (entity.BudgetRequestAndUtilization < (-1 * budgetUtilWeight))
                {
                    entity.BudgetRequestAndUtilization = -1 * budgetUtilWeight;
                }
                //if (entity.MaterialUtilization < (-1 * materialUtilWeight))
                //{
                //    entity.MaterialUtilization = -1 * materialUtilWeight;
                //}

                #endregion

                if (projectType == AvailableProjectTypes.Road)
                {
                    entity.MaterialUtilization = this.CalculatePRPScore(modifier, projectType);
                }
                entity.TotalPoints = entity.HealthAndSafety + entity.BudgetRequestAndUtilization +
                                        entity.MaterialUtilization + entity.ConductQualityAssurance + entity.SubContractAdministration;
                if (entity.MaxAllowablePoints > 0)
                {
                    entity.MaxAllowablePoints = -1 * entity.MaxAllowablePoints;
                }
                // check if the total point is not beyond the limit set in the setting
                if (entity.MaxAllowablePoints > entity.TotalPoints)
                {
                    // reset total point to be the max allowable point.
                    entity.TotalPoints = entity.MaxAllowablePoints;
                }
                db.MonthlyProjectPerformanceModifier.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Modifier_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectPerformanceModifier modifier)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectPerformanceModifier
                {
                    MonthlyProjectPerformanceModifierId = modifier.MonthlyProjectPerformanceModifierId,
                    PeriodMappingId = modifier.PeriodMappingId,
                    HealthAndSafety = modifier.HealthAndSafety,
                    BudgetRequestAndUtilization = modifier.BudgetRequestAndUtilization,
                    MaterialUtilization = modifier.MaterialUtilization,
                    ConductQualityAssurance = modifier.ConductQualityAssurance,
                    SubContractAdministration = modifier.SubContractAdministration,
                    MaxAllowablePoints = modifier.MaxAllowablePoints,
                    TotalPoints = modifier.TotalPoints,

                    HeavyInjuriesCount = modifier.HeavyInjuriesCount,
                    LightInjuriesCount = modifier.LightInjuriesCount,
                    ForcastedBudget = modifier.ForcastedBudget,
                    RequestedBudget = modifier.RequestedBudget,
                    RequestDeviation = modifier.RequestDeviation,
                    PointFromReqDeviation = modifier.PointFromReqDeviation,
                    UtilizedBudget = modifier.UtilizedBudget,
                    UtilizationDeviation = modifier.UtilizationDeviation,
                    PointFromUtlDeviation = modifier.PointFromUtlDeviation,
                    //Cement = modifier.Cement,
                    //ReadyMixConcrete = modifier.ReadyMixConcrete,
                    //ReinforcementBar = modifier.ReinforcementBar,
                    //FormWork = modifier.FormWork,
                    //Sand = modifier.Sand,
                    //Ceramic = modifier.Ceramic,
                    PRPImplementationLevel = modifier.PRPImplementationLevel,
                    ReworkHappened = modifier.ReworkHappened,
                    ConsultantComplaint = modifier.ConsultantComplaint,
                    DefectOccured = modifier.DefectOccured,

                    SiteAccess = modifier.SiteAccess,
                    LackOfGoodGovernance = modifier.LackOfGoodGovernance,
                    InactionForSubContractor = modifier.InactionForSubContractor,
                    DelayedPayment = modifier.DelayedPayment,

                    OrgBranchName = modifier.OrgBranchName,
                    DateCreated = modifier.DateCreated,
                    LastModified = modifier.LastModified,
                    CreatedBy = modifier.CreatedBy,
                    ModifiedBy = modifier.ModifiedBy
                };

                db.MonthlyProjectPerformanceModifier.Attach(entity);
                db.MonthlyProjectPerformanceModifier.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { modifier }.ToDataSourceResult(request, ModelState));
        }
        private decimal CalculateRequestDeviation(MonthlyProjectPerformanceModifier model)
        {
            var dv = new decimal();
            if (model.ForcastedBudget > 0 && model.RequestedBudget > 0)
            {
                dv = (Math.Abs((model.ForcastedBudget - model.RequestedBudget) / model.ForcastedBudget)) * 100;
            }
            return dv;
        }
        private decimal CalculateUtilizationDeviation(MonthlyProjectPerformanceModifier model)
        {
            var dv = new decimal();
            if (model.RequestedBudget > 0 && model.UtilizedBudget > 0)
            {
                dv = (Math.Abs((model.RequestedBudget - model.UtilizedBudget) / model.RequestedBudget)) * 100;
            }
            return dv;
        }
        private decimal CalculatePRPScore(MonthlyProjectPerformanceModifier modifier, AvailableProjectTypes projectType)
        {
            var score = new decimal();
            if (projectType == AvailableProjectTypes.Road)
            {
                var level = modifier.PRPImplementationLevel;
                if (level == PerformanceModifierPRPImplementationLevel.NotImplemented)
                {
                    score = -7;
                }
                else if (level == PerformanceModifierPRPImplementationLevel.ImplementedNotOrganizedNoResult)
                {
                    score = -5;
                }
                else if (level == PerformanceModifierPRPImplementationLevel.ImplementedAndOrganizedNoResult)
                {
                    score = -3;
                }
                else if (level == PerformanceModifierPRPImplementationLevel.ImplementedAndOrganizedWithResultButPoorReporting)
                {
                    score = -2;
                }
            }
            return score;
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}