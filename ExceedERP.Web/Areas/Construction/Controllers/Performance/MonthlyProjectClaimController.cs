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
    public class MonthlyProjectClaimController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult MonthlyProjectClaim_Read([DataSourceRequest]DataSourceRequest request, int pIdReadClaim)
        {
            IQueryable<MonthlyProjectClaim> claims = db.MonthlyProjectClaim.Where(x => x.PeriodMappingId == pIdReadClaim);
            DataSourceResult result = claims.ToDataSourceResult(request, claim => new
            {
                MonthlyProjectClaimId = claim.MonthlyProjectClaimId,
                PeriodMappingId = claim.PeriodMappingId,
                MaxAllowablePoints = claim.MaxAllowablePoints,
                Points = claim.Points,
                //IsClaimPreparedAndIssuedInThisMonth = claim.IsClaimPreparedAndIssuedInThisMonth,
                ContempraryRecordKeeping = claim.ContempraryRecordKeeping,
                FullFillingRequirementForClaims = claim.FullFillingRequirementForClaims,
                AnalysisTechnique = claim.AnalysisTechnique,
                ApprovalStatusOfClaims = claim.ApprovalStatusOfClaims,

                DailylyRecords = claim.DailylyRecords,
                IsClaimPreparedAndIssuedInTheMonth = claim.IsClaimPreparedAndIssuedInTheMonth,
                SubstantiveRequirementRefClauses = claim.SubstantiveRequirementRefClauses,
                ProceduralRequirementDocumentation = claim.ProceduralRequirementDocumentation,
                ProofRequirementDocumentation = claim.ProofRequirementDocumentation,
                WasThereissuetoClaim = claim.WasThereissuetoClaim,
                AnalysisType = claim.AnalysisType,
                IsApprovalMadInTheMonth = claim.IsApprovalMadInTheMonth,
                ClaimApprovalStatus = claim.ClaimApprovalStatus,

                OrgBranchName = claim.OrgBranchName,
                DateCreated = claim.DateCreated,
                LastModified = claim.LastModified,
                CreatedBy = claim.CreatedBy,
                ModifiedBy = claim.ModifiedBy
            });

            return Json(result);
        }
        private bool MonthlyProjectClaimExists(int periodId)
        {
            var valueIsAvailable = false;
            var existingValue = db.MonthlyProjectClaim.FirstOrDefault(p => p.PeriodMappingId == periodId);
            if (existingValue != null)
            {
                valueIsAvailable = true;
            }
            return valueIsAvailable;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProjectClaim_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectClaim claim, int pIdCreateClaim)
        {
            if (ModelState.IsValid)
            {
                var errorExists = false;
                var componentWait = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.CLAIM, pIdCreateClaim);
                if (componentWait == 0)
                {
                    ModelState.AddModelError("", "Component weight is not set for CLAIM");
                    errorExists = true;
                }
                if (MonthlyProjectClaimExists(pIdCreateClaim)) {
                    ModelState.AddModelError("", "Claim for this Period exists");
                    errorExists = true;
                }
                if(!errorExists)
                {
                    var entity = new MonthlyProjectClaim
                    {
                        MonthlyProjectClaimId = claim.MonthlyProjectClaimId,
                        PeriodMappingId = pIdCreateClaim,
                        MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.CLAIM, pIdCreateClaim),
                        Points = claim.Points,
                        //IsClaimPreparedAndIssuedInThisMonth = claim.IsClaimPreparedAndIssuedInThisMonth,
                        ContempraryRecordKeeping = claim.ContempraryRecordKeeping,
                        FullFillingRequirementForClaims = claim.FullFillingRequirementForClaims,
                        AnalysisTechnique = claim.AnalysisTechnique,
                        ApprovalStatusOfClaims = claim.ApprovalStatusOfClaims,

                        DailylyRecords = claim.DailylyRecords,
                        IsClaimPreparedAndIssuedInTheMonth = claim.IsClaimPreparedAndIssuedInTheMonth,
                        SubstantiveRequirementRefClauses = claim.SubstantiveRequirementRefClauses,
                        ProceduralRequirementDocumentation = claim.ProceduralRequirementDocumentation,
                        ProofRequirementDocumentation = claim.ProofRequirementDocumentation,
                        WasThereissuetoClaim = claim.WasThereissuetoClaim,
                        AnalysisType = claim.AnalysisType,
                        IsApprovalMadInTheMonth = claim.IsApprovalMadInTheMonth,
                        ClaimApprovalStatus = claim.ClaimApprovalStatus,

                        OrgBranchName = claim.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = claim.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = claim.ModifiedBy
                    };
                    entity.ContempraryRecordKeeping = entity.DailylyRecords;
                    entity.FullFillingRequirementForClaims = entity.IsClaimPreparedAndIssuedInTheMonth
                        ? entity.SubstantiveRequirementRefClauses + entity.ProceduralRequirementDocumentation +
                          entity.ProofRequirementDocumentation : 0;
                    entity.AnalysisTechnique = PerformanceEvnHelper.GetScoreOfAnalysisTechniqueAppliedForClaims(entity);
                    entity.ApprovalStatusOfClaims = PerformanceEvnHelper.GetScoereOfForClaimsApprovalStatus(entity);
                    entity.Points = entity.ContempraryRecordKeeping + entity.AnalysisTechnique +
                                    entity.ApprovalStatusOfClaims + entity.FullFillingRequirementForClaims;
                    entity.Points = (decimal)entity.MaxAllowablePoints < entity.Points ? (decimal)entity.MaxAllowablePoints : entity.Points;
                    db.MonthlyProjectClaim.Add(entity);
                    db.SaveChanges();
                    claim.MonthlyProjectClaimId = entity.MonthlyProjectClaimId;
                }
                

            }
            return Json(new[] { claim }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProjectClaim_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectClaim claim)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectClaim
                {
                    MonthlyProjectClaimId = claim.MonthlyProjectClaimId,
                    PeriodMappingId = claim.PeriodMappingId,
                    MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.CLAIM, claim.PeriodMappingId),
                    Points = claim.Points,
                    //IsClaimPreparedAndIssuedInThisMonth = claim.IsClaimPreparedAndIssuedInThisMonth,
                    ContempraryRecordKeeping = claim.ContempraryRecordKeeping,
                    FullFillingRequirementForClaims = claim.FullFillingRequirementForClaims,
                    AnalysisTechnique = claim.AnalysisTechnique,
                    ApprovalStatusOfClaims = claim.ApprovalStatusOfClaims,

                    DailylyRecords = claim.DailylyRecords,
                    IsClaimPreparedAndIssuedInTheMonth = claim.IsClaimPreparedAndIssuedInTheMonth,
                    SubstantiveRequirementRefClauses = claim.SubstantiveRequirementRefClauses,
                    ProceduralRequirementDocumentation = claim.ProceduralRequirementDocumentation,
                    ProofRequirementDocumentation = claim.ProofRequirementDocumentation,
                    WasThereissuetoClaim = claim.WasThereissuetoClaim,
                    AnalysisType = claim.AnalysisType,
                    IsApprovalMadInTheMonth = claim.IsApprovalMadInTheMonth,
                    ClaimApprovalStatus = claim.ClaimApprovalStatus,

                    OrgBranchName = claim.OrgBranchName,
                    DateCreated = claim.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = claim.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.ContempraryRecordKeeping = entity.DailylyRecords;
                entity.FullFillingRequirementForClaims = entity.IsClaimPreparedAndIssuedInTheMonth
                    ? entity.SubstantiveRequirementRefClauses + entity.ProceduralRequirementDocumentation +
                      entity.ProofRequirementDocumentation : 0;
                entity.AnalysisTechnique = PerformanceEvnHelper.GetScoreOfAnalysisTechniqueAppliedForClaims(entity);
                entity.ApprovalStatusOfClaims = PerformanceEvnHelper.GetScoereOfForClaimsApprovalStatus(entity);
                entity.Points = entity.ContempraryRecordKeeping + entity.AnalysisTechnique +
                                entity.ApprovalStatusOfClaims + entity.FullFillingRequirementForClaims;
                entity.Points = (decimal)entity.MaxAllowablePoints < entity.Points ? (decimal)entity.MaxAllowablePoints : entity.Points;

                db.MonthlyProjectClaim.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { claim }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProjectClaim_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectClaim claim)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectClaim
                {
                    MonthlyProjectClaimId = claim.MonthlyProjectClaimId,
                    PeriodMappingId = claim.PeriodMappingId,
                    MaxAllowablePoints = claim.MaxAllowablePoints,
                    Points = claim.Points,
                    //IsClaimPreparedAndIssuedInThisMonth = claim.IsClaimPreparedAndIssuedInThisMonth,
                    ContempraryRecordKeeping = claim.ContempraryRecordKeeping,
                    FullFillingRequirementForClaims = claim.FullFillingRequirementForClaims,
                    AnalysisTechnique = claim.AnalysisTechnique,
                    ApprovalStatusOfClaims = claim.ApprovalStatusOfClaims,

                    DailylyRecords = claim.DailylyRecords,
                    IsClaimPreparedAndIssuedInTheMonth = claim.IsClaimPreparedAndIssuedInTheMonth,
                    SubstantiveRequirementRefClauses = claim.SubstantiveRequirementRefClauses,
                    ProceduralRequirementDocumentation = claim.ProceduralRequirementDocumentation,
                    ProofRequirementDocumentation = claim.ProofRequirementDocumentation,
                    WasThereissuetoClaim = claim.WasThereissuetoClaim,
                    AnalysisType = claim.AnalysisType,
                    IsApprovalMadInTheMonth = claim.IsApprovalMadInTheMonth,
                    ClaimApprovalStatus = claim.ClaimApprovalStatus,

                    OrgBranchName = claim.OrgBranchName,
                    DateCreated = claim.DateCreated,
                    LastModified = claim.LastModified,
                    CreatedBy = claim.CreatedBy,
                    ModifiedBy = claim.ModifiedBy
                };

                db.MonthlyProjectClaim.Attach(entity);
                db.MonthlyProjectClaim.Remove(entity);
                db.SaveChanges();

            }

            return Json(new[] { claim }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}