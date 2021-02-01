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
    public class MonthlyProjectReportController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MonthlyProjectReport_Read([DataSourceRequest]DataSourceRequest request, int pIdReadReport)
        {
            IQueryable<MonthlyProjectReport> reports = db.MonthlyProjectReport.Where(x => x.PeriodMappingId == pIdReadReport);
            DataSourceResult result = reports.ToDataSourceResult(request, report => new
            {
                MonthlyProjectReportId = report.MonthlyProjectReportId,
                PeriodMappingId = report.PeriodMappingId,
                ReportingTime = report.ReportingTime,
                Days = report.Days,
                Points = report.Points,
                Completeness = report.Completeness,
                Narrative = report.Narrative,
                MaxAllowablePoints = report.MaxAllowablePoints,
                TotalPoints = report.TotalPoints,

                Progress = report.Progress,
                QualityControl = report.QualityControl,
                HealthAndSafety = report.HealthAndSafety,
                EarnedValueAnalysis = report.EarnedValueAnalysis,
                MaterialUtilization = report.MaterialUtilization,
                LabourUtilixation = report.LabourUtilixation,
                EquipmentUtulization = report.EquipmentUtulization,
                FuelUtilization = report.FuelUtilization,
                ProperIdentificationOfProblems = report.ProperIdentificationOfProblems,

                OrgBranchName = report.OrgBranchName,
                DateCreated = report.DateCreated,
                LastModified = report.LastModified,
                CreatedBy = report.CreatedBy,
                ModifiedBy = report.ModifiedBy
            });

            return Json(result);
        }
        private bool MonthlyProjectReportExists(int periodId)
        {
            var valueIsAvailable = false;
            var existingValue = db.MonthlyProjectReport.FirstOrDefault(p => p.PeriodMappingId == periodId);
            if (existingValue != null)
            {
                valueIsAvailable = true;
            }
            return valueIsAvailable;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProjectReport_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectReport report, int pIdCreateReport)
        {
            if (ModelState.IsValid)
            {
                var errorExists = false;
                var componentWait = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.REPORT, pIdCreateReport);
                if (componentWait == 0)
                {
                    ModelState.AddModelError("", "Component weight is not set for REPORT");
                    errorExists = true;
                }
                if (MonthlyProjectReportExists(pIdCreateReport))
                {
                    ModelState.AddModelError("", "Report for this Period exists");
                    errorExists = true;
                }
                if(!errorExists)
                {
                    var entity = new MonthlyProjectReport
                    {
                        MonthlyProjectReportId = report.MonthlyProjectReportId,
                        PeriodMappingId = pIdCreateReport,
                        ReportingTime = report.ReportingTime,
                        Days = report.Days,
                        Points = this.CalcuReportingTimeScore(report),
                        Completeness = report.Progress + report.QualityControl + report.HealthAndSafety +
                                    report.EarnedValueAnalysis + report.MaterialUtilization +
                                    report.LabourUtilixation + report.EquipmentUtulization + report.FuelUtilization,
                        Narrative = report.ProperIdentificationOfProblems,
                        MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.REPORT, pIdCreateReport),
                        TotalPoints = report.TotalPoints,

                        Progress = report.Progress,
                        QualityControl = report.QualityControl,
                        HealthAndSafety = report.HealthAndSafety,
                        EarnedValueAnalysis = report.EarnedValueAnalysis,
                        MaterialUtilization = report.MaterialUtilization,
                        LabourUtilixation = report.LabourUtilixation,
                        EquipmentUtulization = report.EquipmentUtulization,
                        FuelUtilization = report.FuelUtilization,
                        ProperIdentificationOfProblems = report.ProperIdentificationOfProblems,

                        OrgBranchName = report.OrgBranchName,
                        DateCreated = DateTime.Now,
                        LastModified = report.LastModified,
                        CreatedBy = User.Identity.Name,
                        ModifiedBy = report.ModifiedBy
                    };
                    entity.TotalPoints = entity.Points + entity.Completeness + entity.Narrative;

                    entity.TotalPoints = entity.MaxAllowablePoints < entity.TotalPoints ? entity.MaxAllowablePoints : entity.TotalPoints;
                    db.MonthlyProjectReport.Add(entity);
                    db.SaveChanges();
                    report.MonthlyProjectReportId = entity.MonthlyProjectReportId;

                    //PerformanceEvnHelper.UpdateMonthlyReport(entity.MonthlyProjectReportId);
                }
                
            }
            return Json(new[] { report }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProjectReport_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectReport report)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectReport
                {
                    MonthlyProjectReportId = report.MonthlyProjectReportId,
                    PeriodMappingId = report.PeriodMappingId,
                    ReportingTime = report.ReportingTime,
                    Days = report.Days,
                    Points = this.CalcuReportingTimeScore(report),
                    Completeness = report.Progress + report.QualityControl + report.HealthAndSafety +
                                    report.EarnedValueAnalysis + report.MaterialUtilization +
                                    report.LabourUtilixation + report.EquipmentUtulization + report.FuelUtilization,
                    Narrative = report.ProperIdentificationOfProblems,
                    MaxAllowablePoints = ConsSettingHelper.GetComponentWeight(ProjectPerformanceEvaluationComponents.PLANNING, report.PeriodMappingId),
                    TotalPoints = report.TotalPoints,

                    Progress = report.Progress,
                    QualityControl = report.QualityControl,
                    HealthAndSafety = report.HealthAndSafety,
                    EarnedValueAnalysis = report.EarnedValueAnalysis,
                    MaterialUtilization = report.MaterialUtilization,
                    LabourUtilixation = report.LabourUtilixation,
                    EquipmentUtulization = report.EquipmentUtulization,
                    FuelUtilization = report.FuelUtilization,
                    ProperIdentificationOfProblems = report.ProperIdentificationOfProblems,

                    OrgBranchName = report.OrgBranchName,
                    DateCreated = report.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = report.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                entity.TotalPoints = entity.Points + entity.Completeness + entity.Narrative;
                entity.TotalPoints = entity.MaxAllowablePoints < entity.TotalPoints ? entity.MaxAllowablePoints : entity.TotalPoints;

                db.MonthlyProjectReport.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                //PerformanceEvnHelper.UpdateMonthlyReport(entity.MonthlyProjectReportId);

            }

            return Json(new[] { report }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MonthlyProjectReport_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectReport report)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectReport
                {
                    MonthlyProjectReportId = report.MonthlyProjectReportId,
                    PeriodMappingId = report.PeriodMappingId,
                    ReportingTime = report.ReportingTime,
                    Days = report.Days,
                    Points = report.Points,
                    Completeness = report.Completeness,
                    Narrative = report.Narrative,
                    MaxAllowablePoints = report.MaxAllowablePoints,
                    TotalPoints = report.TotalPoints,

                    Progress = report.Progress,
                    QualityControl = report.QualityControl,
                    HealthAndSafety = report.HealthAndSafety,
                    EarnedValueAnalysis = report.EarnedValueAnalysis,
                    MaterialUtilization = report.MaterialUtilization,
                    LabourUtilixation = report.LabourUtilixation,
                    EquipmentUtulization = report.EquipmentUtulization,
                    FuelUtilization = report.FuelUtilization,
                    ProperIdentificationOfProblems = report.ProperIdentificationOfProblems,

                    OrgBranchName = report.OrgBranchName,
                    DateCreated = report.DateCreated,
                    LastModified = report.LastModified,
                    CreatedBy = report.CreatedBy,
                    ModifiedBy = report.ModifiedBy
                };

                db.MonthlyProjectReport.Attach(entity);
                db.MonthlyProjectReport.Remove(entity);
                db.SaveChanges();

                //PerformanceEvnHelper.UpdateMonthlyReport(entity.MonthlyProjectReportId);

            }

            return Json(new[] { report }.ToDataSourceResult(request, ModelState));
        }

        private decimal CalcuReportingTimeScore(MonthlyProjectReport model)
        {
            var pt = new decimal();
            var reportTime = model.ReportingTime;
            //if (model.IsRevisionTime)
            //{
                if (reportTime == MonthlyDueDate.On_Due_Date)
                {
                    pt = 0;
                }
                else if (reportTime == MonthlyDueDate.Before_Due_Date)
                {
                    pt = (decimal)(model.Days);
                }
                else if (reportTime == MonthlyDueDate.After_Due_Date)
                {
                    pt = (decimal)(0 - model.Days);
                }
            //}
            return pt;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}