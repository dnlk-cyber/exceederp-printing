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
    public class ProjectPnanningCompletenessController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        //public ActionResult Index()
        //{
        //    return View();
        //}

        public ActionResult PnanningCompleteness_Read([DataSourceRequest]DataSourceRequest request, int planIdReadCompleteness)
        {
            IQueryable<ProjectPnanningCompleteness> compl = db.ProjectPnanningCompleteness.Where(x => x.MonthlyProjectPlanningId == planIdReadCompleteness);
            DataSourceResult result = compl.ToDataSourceResult(request, completness => new
            {
                ProjectPnanningCompletenessId = completness.ProjectPnanningCompletenessId,
                MonthlyProjectPlanningId = completness.MonthlyProjectPlanningId,
                ScheduleType = completness.ScheduleType,
                MaxAllowablePoints = completness.MaxAllowablePoints,
                Points = completness.Points,

                OrgBranchName = completness.OrgBranchName,
                DateCreated = completness.DateCreated,
                LastModified = completness.LastModified,
                CreatedBy = completness.CreatedBy,
                ModifiedBy = completness.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PnanningCompleteness_Create([DataSourceRequest]DataSourceRequest request, ProjectPnanningCompleteness completness, int planIdCreateCompleteness)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPnanningCompleteness
                {
                    ProjectPnanningCompletenessId = completness.ProjectPnanningCompletenessId,
                    MonthlyProjectPlanningId = planIdCreateCompleteness,
                    ScheduleType = completness.ScheduleType,
                    MaxAllowablePoints = completness.MaxAllowablePoints,
                    Points = completness.MaxAllowablePoints != null &&
                             completness.Points > completness.MaxAllowablePoints ?
                             (decimal)completness.MaxAllowablePoints : completness.Points,

                    OrgBranchName = completness.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = completness.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = completness.ModifiedBy
                };

                db.ProjectPnanningCompleteness.Add(entity);
                db.SaveChanges();
                completness.ProjectPnanningCompletenessId = entity.ProjectPnanningCompletenessId;

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);

            }
            return Json(new[] { completness }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PnanningCompleteness_Update([DataSourceRequest]DataSourceRequest request, ProjectPnanningCompleteness completness)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPnanningCompleteness
                {
                    ProjectPnanningCompletenessId = completness.ProjectPnanningCompletenessId,
                    MonthlyProjectPlanningId = completness.MonthlyProjectPlanningId,
                    ScheduleType = completness.ScheduleType,
                    MaxAllowablePoints = completness.MaxAllowablePoints,
                    Points = completness.MaxAllowablePoints != null &&
                             completness.Points > completness.MaxAllowablePoints ?
                             (decimal)completness.MaxAllowablePoints : completness.Points,

                    OrgBranchName = completness.OrgBranchName,
                    DateCreated = completness.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = completness.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.ProjectPnanningCompleteness.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);

            }

            return Json(new[] { completness }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PnanningCompleteness_Destroy([DataSourceRequest]DataSourceRequest request, ProjectPnanningCompleteness completness)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectPnanningCompleteness
                {
                    ProjectPnanningCompletenessId = completness.ProjectPnanningCompletenessId,
                    MonthlyProjectPlanningId = completness.MonthlyProjectPlanningId,
                    ScheduleType = completness.ScheduleType,
                    MaxAllowablePoints = completness.MaxAllowablePoints,
                    Points = completness.Points,

                    OrgBranchName = completness.OrgBranchName,
                    DateCreated = completness.DateCreated,
                    LastModified = completness.LastModified,
                    CreatedBy = completness.CreatedBy,
                    ModifiedBy = completness.ModifiedBy
                };

                db.ProjectPnanningCompleteness.Attach(entity);
                db.ProjectPnanningCompleteness.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectPlanningScore(entity.MonthlyProjectPlanningId);

            }

            return Json(new[] { completness }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}