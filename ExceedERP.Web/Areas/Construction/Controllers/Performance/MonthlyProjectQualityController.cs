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
    public class MonthlyProjectQualityController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MonthlyQuality_Read([DataSourceRequest]DataSourceRequest request, int pIdReadQuality)
        {
            IQueryable<MonthlyProjectQuality> qualities = db.MonthlyProjectQuality.Where(x => x.PeriodMappingId == pIdReadQuality);
            DataSourceResult result = qualities.ToDataSourceResult(request, quality => new
            {
                MonthlyProjectQualityId = quality.MonthlyProjectQualityId,
                PeriodMappingId = quality.PeriodMappingId,
                QualityPlanning = quality.QualityPlanning,
                QualityAssurance = quality.QualityAssurance,
                QualityControl = quality.QualityControl,
                QualityComplaint = quality.QualityComplaint,
                MaxAllowablePoints = quality.MaxAllowablePoints,
                TotalPoints = quality.TotalPoints,

                DoPersonnelKnowTheQualityRequirements = quality.DoPersonnelKnowTheQualityRequirements,
                DoPersonnelKnowHowToMeetQualityRequirements = quality.DoPersonnelKnowHowToMeetQualityRequirements,
                MaterialTestsPerformed = quality.MaterialTestsPerformed,
                EvaluationOfMethod = quality.EvaluationOfMethod,
                OverAllEvaluationOfLabour = quality.OverAllEvaluationOfLabour,
                NumberOfTestsPerformedOnExecutedWorks = quality.NumberOfTestsPerformedOnExecutedWorks,
                ExtentOfInspectionPerformedOnExecutedWorks = quality.ExtentOfInspectionPerformedOnExecutedWorks,
                FromClient = quality.FromClient,
                FromEngineers = quality.FromEngineers,
                FromHeadOffice = quality.FromHeadOffice,
                FromQualityEngineer = quality.FromQualityEngineer,


                OrgBranchName = quality.OrgBranchName,
                DateCreated = quality.DateCreated,
                LastModified = quality.LastModified,
                CreatedBy = quality.CreatedBy,
                ModifiedBy = quality.ModifiedBy
            });

            return Json(result);
        }
        private bool MonthlyProjectQualityExists(int periodId)
        {
            var qualityIsAvailable = false;
            var existingQuality = db.MonthlyProjectQuality.Where(p => p.PeriodMappingId == periodId).FirstOrDefault();
            if (existingQuality != null)
            {
                qualityIsAvailable = true;
            }
            return qualityIsAvailable;
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyQuality_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectQuality quality, int pIdCreateQuality)
        {
            if (ModelState.IsValid)
            {
                var errorExists = false;
                var componentWait = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.QUALITY, pIdCreateQuality);
                if (componentWait == 0)
                {
                    ModelState.AddModelError("", "Component weight is not set for QUALITY");
                    errorExists = true;
                }

                if (MonthlyProjectQualityExists(pIdCreateQuality))
                {
                    ModelState.AddModelError("", "Quality for this period exists");
                    errorExists = true;
                }
                if(!errorExists)
                {
                    var entity = new MonthlyProjectQuality
                    {
                        MonthlyProjectQualityId = quality.MonthlyProjectQualityId,
                        PeriodMappingId = pIdCreateQuality,
                        QualityPlanning = quality.QualityPlanning,
                        QualityAssurance = quality.QualityAssurance,
                        QualityControl = quality.QualityControl,
                        QualityComplaint = quality.QualityComplaint,
                        MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.QUALITY, pIdCreateQuality),
                        TotalPoints = quality.TotalPoints,

                        DoPersonnelKnowTheQualityRequirements = quality.DoPersonnelKnowTheQualityRequirements,
                        DoPersonnelKnowHowToMeetQualityRequirements = quality.DoPersonnelKnowHowToMeetQualityRequirements,
                        MaterialTestsPerformed = quality.MaterialTestsPerformed,
                        EvaluationOfMethod = quality.EvaluationOfMethod,
                        OverAllEvaluationOfLabour = quality.OverAllEvaluationOfLabour,
                        NumberOfTestsPerformedOnExecutedWorks = quality.NumberOfTestsPerformedOnExecutedWorks,
                        ExtentOfInspectionPerformedOnExecutedWorks = quality.ExtentOfInspectionPerformedOnExecutedWorks,
                        FromClient = quality.FromClient,
                        FromEngineers = quality.FromEngineers,
                        FromHeadOffice = quality.FromHeadOffice,
                        FromQualityEngineer = quality.FromQualityEngineer,

                        OrgBranchName = quality.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = quality.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = quality.ModifiedBy
                    };
                    entity.QualityPlanning = entity.DoPersonnelKnowTheQualityRequirements +
                        entity.DoPersonnelKnowHowToMeetQualityRequirements;

                    entity.QualityAssurance = entity.MaterialTestsPerformed +
                        entity.OverAllEvaluationOfLabour +
                        entity.EvaluationOfMethod;

                    entity.QualityControl = entity.NumberOfTestsPerformedOnExecutedWorks +
                        entity.ExtentOfInspectionPerformedOnExecutedWorks;

                    entity.QualityComplaint = -1 * (entity.FromClient + entity.FromEngineers + entity.FromHeadOffice + entity.FromQualityEngineer);
                    entity.TotalPoints = entity.QualityPlanning + entity.QualityAssurance + entity.QualityControl + entity.QualityComplaint;

                    entity.TotalPoints = entity.MaxAllowablePoints < entity.TotalPoints ? entity.MaxAllowablePoints : entity.TotalPoints;

                    db.MonthlyProjectQuality.Add(entity);
                    db.SaveChanges();
                    quality.MonthlyProjectQualityId = entity.MonthlyProjectQualityId;
                }

            }
            return Json(new[] { quality }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyQuality_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectQuality quality)
        {
            if (ModelState.IsValid)
            {
                
                var entity = new MonthlyProjectQuality
                {
                    MonthlyProjectQualityId = quality.MonthlyProjectQualityId,
                    PeriodMappingId = quality.PeriodMappingId,
                    QualityPlanning = quality.QualityPlanning,
                    QualityAssurance = quality.QualityAssurance,
                    QualityControl = quality.QualityControl,
                    QualityComplaint = quality.QualityComplaint,
                    MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.QUALITY, quality.PeriodMappingId),
                    TotalPoints = quality.TotalPoints,

                    DoPersonnelKnowTheQualityRequirements = quality.DoPersonnelKnowTheQualityRequirements,
                    DoPersonnelKnowHowToMeetQualityRequirements = quality.DoPersonnelKnowHowToMeetQualityRequirements,
                    MaterialTestsPerformed = quality.MaterialTestsPerformed,
                    EvaluationOfMethod = quality.EvaluationOfMethod,
                    OverAllEvaluationOfLabour = quality.OverAllEvaluationOfLabour,
                    NumberOfTestsPerformedOnExecutedWorks = quality.NumberOfTestsPerformedOnExecutedWorks,
                    ExtentOfInspectionPerformedOnExecutedWorks = quality.ExtentOfInspectionPerformedOnExecutedWorks,
                    FromClient = quality.FromClient,
                    FromEngineers = quality.FromEngineers,
                    FromHeadOffice = quality.FromHeadOffice,
                    FromQualityEngineer = quality.FromQualityEngineer,

                    OrgBranchName = quality.OrgBranchName,
                    DateCreated = quality.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = quality.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.QualityPlanning = entity.DoPersonnelKnowTheQualityRequirements +
                    entity.DoPersonnelKnowHowToMeetQualityRequirements;

                entity.QualityAssurance = entity.MaterialTestsPerformed +
                    entity.OverAllEvaluationOfLabour +
                    entity.EvaluationOfMethod;

                entity.QualityControl = entity.NumberOfTestsPerformedOnExecutedWorks +
                    entity.ExtentOfInspectionPerformedOnExecutedWorks;

                entity.QualityComplaint = -1 * (entity.FromClient + entity.FromEngineers + entity.FromHeadOffice + entity.FromQualityEngineer);

                entity.TotalPoints = entity.QualityPlanning + entity.QualityAssurance + entity.QualityControl + entity.QualityComplaint;

                entity.TotalPoints = entity.MaxAllowablePoints < entity.TotalPoints ? entity.MaxAllowablePoints : entity.TotalPoints;

                db.MonthlyProjectQuality.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { quality }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyQuality_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectQuality quality)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectQuality
                {
                    MonthlyProjectQualityId = quality.MonthlyProjectQualityId,
                    PeriodMappingId = quality.PeriodMappingId,
                    QualityPlanning = quality.QualityPlanning,
                    QualityAssurance = quality.QualityAssurance,
                    QualityControl = quality.QualityControl,
                    QualityComplaint = quality.QualityComplaint,
                    MaxAllowablePoints = quality.MaxAllowablePoints,
                    TotalPoints = quality.TotalPoints,

                    DoPersonnelKnowTheQualityRequirements = quality.DoPersonnelKnowTheQualityRequirements,
                    DoPersonnelKnowHowToMeetQualityRequirements = quality.DoPersonnelKnowHowToMeetQualityRequirements,
                    MaterialTestsPerformed = quality.MaterialTestsPerformed,
                    EvaluationOfMethod = quality.EvaluationOfMethod,
                    OverAllEvaluationOfLabour = quality.OverAllEvaluationOfLabour,
                    NumberOfTestsPerformedOnExecutedWorks = quality.NumberOfTestsPerformedOnExecutedWorks,
                    ExtentOfInspectionPerformedOnExecutedWorks = quality.ExtentOfInspectionPerformedOnExecutedWorks,
                    FromClient = quality.FromClient,
                    FromEngineers = quality.FromEngineers,
                    FromHeadOffice = quality.FromHeadOffice,
                    FromQualityEngineer = quality.FromQualityEngineer,

                    OrgBranchName = quality.OrgBranchName,
                    DateCreated = quality.DateCreated,
                    LastModified = quality.LastModified,
                    CreatedBy = quality.CreatedBy,
                    ModifiedBy = quality.ModifiedBy
                };

                db.MonthlyProjectQuality.Attach(entity);
                db.MonthlyProjectQuality.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { quality }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}