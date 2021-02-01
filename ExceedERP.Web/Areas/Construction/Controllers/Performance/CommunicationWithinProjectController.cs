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
    public class CommunicationWithinProjectController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CommunicationWithinProject_Read([DataSourceRequest]DataSourceRequest request, int pIdReadComn)
        {
            IQueryable<MonthlyProjectCommunicationWithinProject> communications = db.MonthlyProjectCommunicationWithinProject.Where(x => x.MonthlyProjectCommunicationId == pIdReadComn);
            DataSourceResult result = communications.ToDataSourceResult(request, comn => new
            {
                MonthlyProjectCommunicationWithinProjectId = comn.MonthlyProjectCommunicationWithinProjectId,
                MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                Description = comn.Description,
                MaxAllowedPoints = comn.MaxAllowedPoints,
                Points = comn.Points,

                OrgBranchName = comn.OrgBranchName,
                DateCreated = comn.DateCreated,
                LastModified = comn.LastModified,
                CreatedBy = comn.CreatedBy,
                ModifiedBy = comn.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommunicationWithinProject_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunicationWithinProject comn, int pIdCreateComn)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCommunicationWithinProject
                {
                    MonthlyProjectCommunicationWithinProjectId = comn.MonthlyProjectCommunicationWithinProjectId,
                    MonthlyProjectCommunicationId = pIdCreateComn,
                    Description = comn.Description,
                    MaxAllowedPoints = comn.MaxAllowedPoints,
                    Points = comn.Points > comn.MaxAllowedPoints && 
                            comn.MaxAllowedPoints != null ? 
                            (decimal)comn.MaxAllowedPoints : comn.Points,


                    OrgBranchName = comn.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = comn.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = comn.ModifiedBy
                };

                db.MonthlyProjectCommunicationWithinProject.Add(entity);
                db.SaveChanges();
                comn.MonthlyProjectCommunicationWithinProjectId = entity.MonthlyProjectCommunicationWithinProjectId;

                PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);
            }
            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommunicationWithinProject_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunicationWithinProject comn)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCommunicationWithinProject
                {
                    MonthlyProjectCommunicationWithinProjectId = comn.MonthlyProjectCommunicationWithinProjectId,
                    MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                    Description = comn.Description,
                    MaxAllowedPoints = comn.MaxAllowedPoints,
                    Points = comn.Points > comn.MaxAllowedPoints &&
                            comn.MaxAllowedPoints != null ?
                            (decimal)comn.MaxAllowedPoints : comn.Points,


                    OrgBranchName = comn.OrgBranchName,
                    DateCreated = comn.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = comn.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.MonthlyProjectCommunicationWithinProject.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);
            }

            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CommunicationWithinProject_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectCommunicationWithinProject comn)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectCommunicationWithinProject
                {
                    MonthlyProjectCommunicationWithinProjectId = comn.MonthlyProjectCommunicationWithinProjectId,
                    MonthlyProjectCommunicationId = comn.MonthlyProjectCommunicationId,
                    Description = comn.Description,
                    MaxAllowedPoints = comn.MaxAllowedPoints,
                    Points = comn.Points,

                    OrgBranchName = comn.OrgBranchName,
                    DateCreated = comn.DateCreated,
                    LastModified = comn.LastModified,
                    CreatedBy = comn.CreatedBy,
                    ModifiedBy = comn.ModifiedBy
                };

                db.MonthlyProjectCommunicationWithinProject.Attach(entity);
                db.MonthlyProjectCommunicationWithinProject.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyProjectComn(entity.MonthlyProjectCommunicationId);
            }

            return Json(new[] { comn }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}