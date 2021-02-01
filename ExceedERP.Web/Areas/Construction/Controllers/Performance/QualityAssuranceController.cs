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
    public class QualityAssuranceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult QualityAssurance_Read([DataSourceRequest]DataSourceRequest request, int pIdRead)
        {
            IQueryable<QualityAssurance> qualityAssurance = db.QualityAssurance.Where(x => x.MonthlyProjectQualityId == pIdRead);
            DataSourceResult result = qualityAssurance.ToDataSourceResult(request, quality => new
            {
                QualityAssuranceId = quality.QualityAssuranceId,
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
        public ActionResult QualityAssurance_Create([DataSourceRequest]DataSourceRequest request, QualityAssurance quality, int pIdCreate)
        {
            if (ModelState.IsValid)
            {
                //if (CheckExistance(Block.Name, iddTwo))
                //{
                //    ModelState.AddModelError("", "Duplicate names are not allowed!!");
                //}
                //else
                //{
                var entity = new QualityAssurance
                {
                    MonthlyProjectQualityId = pIdCreate,
                    Description = quality.Description,
                    MaxPoints = quality.MaxPoints,
                    Points = quality.Points,

                    OrgBranchName = quality.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = quality.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = quality.ModifiedBy
                };

                db.QualityAssurance.Add(entity);
                db.SaveChanges();
                quality.QualityAssuranceId = entity.QualityAssuranceId;

                PerformanceEvnHelper.UpdateMonthlyProjectQualityScore(entity.MonthlyProjectQualityId);

            }

            return Json(new[] { quality }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityAssurance_Update([DataSourceRequest]DataSourceRequest request, QualityAssurance quality)
        {
            if (ModelState.IsValid)
            {
                var entity = new QualityAssurance
                {
                    QualityAssuranceId = quality.QualityAssuranceId,
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

                db.QualityAssurance.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectQualityScore(entity.MonthlyProjectQualityId);

            }

            return Json(new[] { quality }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult QualityAssurance_Destroy([DataSourceRequest]DataSourceRequest request, QualityAssurance quality)
        {
            if (ModelState.IsValid)
            {
                var entity = new QualityAssurance
                {
                    QualityAssuranceId = quality.QualityAssuranceId,
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

                db.QualityAssurance.Attach(entity);
                db.QualityAssurance.Remove(entity);
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