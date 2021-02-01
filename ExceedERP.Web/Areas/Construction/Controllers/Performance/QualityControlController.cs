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
    public class QualityControlController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QualityControl_Read([DataSourceRequest]DataSourceRequest request, int prdIdRead)
        {
            IQueryable<QualityControl> qualityControl = db.QualityControl.Where(x => x.MonthlyProjectQualityId == prdIdRead);
            DataSourceResult result = qualityControl.ToDataSourceResult(request, quality => new
            {
                QualityControlId = quality.QualityControlId,
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
        public ActionResult QualityControl_Create([DataSourceRequest]DataSourceRequest request, QualityControl quality, int prdIdCreate)
        {
            if (ModelState.IsValid)
            {
                //if (CheckExistance(Block.Name, iddTwo))
                //{
                //    ModelState.AddModelError("", "Duplicate names are not allowed!!");
                //}
                //else
                //{
                var entity = new QualityControl
                {
                    MonthlyProjectQualityId = prdIdCreate,
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

                db.QualityControl.Add(entity);
                db.SaveChanges();
                quality.QualityControlId = entity.QualityControlId;

                PerformanceEvnHelper.UpdateMonthlyProjectQualityScore(entity.MonthlyProjectQualityId);
            }

            return Json(new[] { quality }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityControl_Update([DataSourceRequest]DataSourceRequest request, QualityControl quality)
        {
            if (ModelState.IsValid)
            {
                var entity = new QualityControl
                {
                    QualityControlId = quality.QualityControlId,
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

                db.QualityControl.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectQualityScore(entity.MonthlyProjectQualityId);

            }

            return Json(new[] { quality }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityControl_Destroy([DataSourceRequest]DataSourceRequest request, QualityControl quality)
        {
            if (ModelState.IsValid)
            {
                var entity = new QualityControl
                {
                    QualityControlId = quality.QualityControlId,
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

                db.QualityControl.Attach(entity);
                db.QualityControl.Remove(entity);
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