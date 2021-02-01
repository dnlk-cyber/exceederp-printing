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
    public class MonthlyProjectReportNarrativeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReportNarrative_Read([DataSourceRequest]DataSourceRequest request, int rIdReadNarrative)
        {
            IQueryable<MonthlyProjectReportNarrative> narratives = db.MonthlyProjectReportNarrative.Where(x => x.MonthlyProjectReportId == rIdReadNarrative);
            DataSourceResult result = narratives.ToDataSourceResult(request, narrative => new
            {
                MonthlyProjectReportNarrativeId = narrative.MonthlyProjectReportNarrativeId,
                MonthlyProjectReportId = narrative.MonthlyProjectReportId,
                Attribute = narrative.Attribute,
                MaxAllowablePoints = narrative.MaxAllowablePoints,
                Points = narrative.Points,

                OrgBranchName = narrative.OrgBranchName,
                DateCreated = narrative.DateCreated,
                LastModified = narrative.LastModified,
                CreatedBy = narrative.CreatedBy,
                ModifiedBy = narrative.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReportNarrative_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectReportNarrative narrative, int rIdCreateNarrative)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectReportNarrative
                {
                    MonthlyProjectReportNarrativeId = narrative.MonthlyProjectReportNarrativeId,
                    MonthlyProjectReportId = rIdCreateNarrative,
                    Attribute = narrative.Attribute,
                    MaxAllowablePoints = narrative.MaxAllowablePoints,
                    Points = narrative.Points,

                    OrgBranchName = narrative.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = narrative.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = narrative.ModifiedBy
                };

                db.MonthlyProjectReportNarrative.Add(entity);
                db.SaveChanges();
                narrative.MonthlyProjectReportId = entity.MonthlyProjectReportId;

                PerformanceEvnHelper.UpdateMonthlyReport(entity.MonthlyProjectReportId);
            }
            return Json(new[] { narrative }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReportNarrative_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectReportNarrative narrative)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectReportNarrative
                {
                    MonthlyProjectReportNarrativeId = narrative.MonthlyProjectReportNarrativeId,
                    MonthlyProjectReportId = narrative.MonthlyProjectReportId,
                    Attribute = narrative.Attribute,
                    MaxAllowablePoints = narrative.MaxAllowablePoints,
                    Points = narrative.Points,


                    OrgBranchName = narrative.OrgBranchName,
                    DateCreated = narrative.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = narrative.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.MonthlyProjectReportNarrative.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyReport(entity.MonthlyProjectReportId);

            }

            return Json(new[] { narrative }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ReportNarrative_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectReportNarrative narrative)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectReportNarrative
                {
                    MonthlyProjectReportNarrativeId = narrative.MonthlyProjectReportNarrativeId,
                    MonthlyProjectReportId = narrative.MonthlyProjectReportId,
                    Attribute = narrative.Attribute,
                    MaxAllowablePoints = narrative.MaxAllowablePoints,
                    Points = narrative.Points,

                    OrgBranchName = narrative.OrgBranchName,
                    DateCreated = narrative.DateCreated,
                    LastModified = narrative.LastModified,
                    CreatedBy = narrative.CreatedBy,
                    ModifiedBy = narrative.ModifiedBy
                };

                db.MonthlyProjectReportNarrative.Attach(entity);
                db.MonthlyProjectReportNarrative.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyReport(entity.MonthlyProjectReportId);

            }

            return Json(new[] { narrative }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}