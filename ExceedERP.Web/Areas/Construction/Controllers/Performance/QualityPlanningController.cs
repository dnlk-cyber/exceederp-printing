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
    public class QualityPlanningController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QualityPlanning_Read([DataSourceRequest]DataSourceRequest request, int idRead)
        {
            IQueryable<QualityPlanning> qualityPlanning = db.QualityPlanning.Where(x => x.MonthlyProjectQualityId == idRead);
            DataSourceResult result = qualityPlanning.ToDataSourceResult(request, quality => new
            {
                QualityPlanningId = quality.QualityPlanningId,
                MonthlyProjectQualityId = quality.MonthlyProjectQualityId,
                Description = quality.Description,
                MaxPoints = quality.MaxPoints,
                Points = quality.Points,

                OrgBranchName = quality.OrgBranchName,
                DateCreated = quality.DateCreated,
                LastModified = quality.LastModified,
                CreatedBy = quality.CreatedBy,
                ModifiedBy = quality.ModifiedBy

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityPlanning_Create([DataSourceRequest]DataSourceRequest request, QualityPlanning quality, int idCreate)
        {
            if (ModelState.IsValid)
            {
                //if (CheckExistance(Block.Name, iddTwo))
                //{
                //    ModelState.AddModelError("", "Duplicate names are not allowed!!");
                //}
                //else
                //{
                var entity = new QualityPlanning
                {
                    MonthlyProjectQualityId = idCreate,
                    Description = quality.Description,
                    MaxPoints = quality.MaxPoints,
                    Points = quality.Points,

                    OrgBranchName = quality.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = quality.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = quality.ModifiedBy
                };
                //entity.PlannedToDate = entity.PlannedAmount + entity.PlannedAmountTillPreviousMonth;
                //entity.ExecutedAmountToDate = entity.ExecutedAmount + entity.ExecutedAmountTillPreviousMonth;

                db.QualityPlanning.Add(entity);
                db.SaveChanges();
                quality.QualityPlanningId = entity.QualityPlanningId;

                PerformanceEvnHelper.UpdateMonthlyProjectQualityScore(entity.MonthlyProjectQualityId);
            }

            return Json(new[] { quality }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityPlanning_Update([DataSourceRequest]DataSourceRequest request, QualityPlanning quality)
        {
            if (ModelState.IsValid)
            {
                var entity = new QualityPlanning
                {
                    QualityPlanningId = quality.QualityPlanningId,
                    MonthlyProjectQualityId = quality.MonthlyProjectQualityId,
                    Description = quality.Description,
                    MaxPoints = quality.MaxPoints,
                    Points = quality.Points,

                    OrgBranchName = quality.OrgBranchName,
                    DateCreated = quality.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = quality.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };
                db.QualityPlanning.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectQualityScore(entity.MonthlyProjectQualityId);
            }

            return Json(new[] { quality }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityPlanning_Destroy([DataSourceRequest]DataSourceRequest request, QualityPlanning quality)
        {
            if (ModelState.IsValid)
            {
                var entity = new QualityPlanning
                {
                    QualityPlanningId = quality.QualityPlanningId,
                    MonthlyProjectQualityId = quality.MonthlyProjectQualityId,
                    Description = quality.Description,
                    MaxPoints = quality.MaxPoints,
                    Points = quality.Points,

                    OrgBranchName = quality.OrgBranchName,
                    DateCreated = quality.DateCreated,
                    LastModified = quality.LastModified,
                    CreatedBy = quality.CreatedBy,
                    ModifiedBy = quality.ModifiedBy
                };

                db.QualityPlanning.Attach(entity);
                db.QualityPlanning.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectQualityScore(entity.MonthlyProjectQualityId);

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