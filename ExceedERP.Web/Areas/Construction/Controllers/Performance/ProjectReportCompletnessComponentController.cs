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
    public class ProjectReportCompletnessComponentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CompletnessComponent_Read([DataSourceRequest]DataSourceRequest request, int rIdReadComponent)
        {
            IQueryable<MonthlyProjectReportCompletnessComponent> components = db.MonthlyProjectReportCompletnessComponent.Where(x => x.MonthlyProjectReportId == rIdReadComponent);
            DataSourceResult result = components.ToDataSourceResult(request, component => new
            {
                MonthlyProjectReportCompletnessComponentId = component.MonthlyProjectReportCompletnessComponentId,
                MonthlyProjectReportId = component.MonthlyProjectReportId,
                Attribute = component.Attribute,
                MaxAllowablePoints = component.MaxAllowablePoints,
                Points = component.Points,

                OrgBranchName = component.OrgBranchName,
                DateCreated = component.DateCreated,
                LastModified = component.LastModified,
                CreatedBy = component.CreatedBy,
                ModifiedBy = component.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompletnessComponent_Create([DataSourceRequest]DataSourceRequest request, MonthlyProjectReportCompletnessComponent component, int rIdCreateComponent)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectReportCompletnessComponent
                {
                    MonthlyProjectReportCompletnessComponentId = component.MonthlyProjectReportCompletnessComponentId,
                    MonthlyProjectReportId = rIdCreateComponent,
                    Attribute = component.Attribute,
                    MaxAllowablePoints = component.MaxAllowablePoints,
                    Points = component.Points,

                    OrgBranchName = component.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = component.LastModified,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = component.ModifiedBy
                };

                db.MonthlyProjectReportCompletnessComponent.Add(entity);
                db.SaveChanges();
                component.MonthlyProjectReportId = entity.MonthlyProjectReportId;

                PerformanceEvnHelper.UpdateMonthlyReport(entity.MonthlyProjectReportId);
            }
            return Json(new[] { component }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompletnessComponent_Update([DataSourceRequest]DataSourceRequest request, MonthlyProjectReportCompletnessComponent component)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectReportCompletnessComponent
                {
                    MonthlyProjectReportCompletnessComponentId = component.MonthlyProjectReportCompletnessComponentId,
                    MonthlyProjectReportId = component.MonthlyProjectReportId,
                    Attribute = component.Attribute,
                    MaxAllowablePoints = component.MaxAllowablePoints,
                    Points = component.Points,


                    OrgBranchName = component.OrgBranchName,
                    DateCreated = component.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = component.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.MonthlyProjectReportCompletnessComponent.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyReport(entity.MonthlyProjectReportId);

            }

            return Json(new[] { component }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CompletnessComponent_Destroy([DataSourceRequest]DataSourceRequest request, MonthlyProjectReportCompletnessComponent component)
        {
            if (ModelState.IsValid)
            {
                var entity = new MonthlyProjectReportCompletnessComponent
                {
                    MonthlyProjectReportCompletnessComponentId = component.MonthlyProjectReportCompletnessComponentId,
                    MonthlyProjectReportId = component.MonthlyProjectReportId,
                    Attribute = component.Attribute,
                    MaxAllowablePoints = component.MaxAllowablePoints,
                    Points = component.Points,

                    OrgBranchName = component.OrgBranchName,
                    DateCreated = component.DateCreated,
                    LastModified = component.LastModified,
                    CreatedBy = component.CreatedBy,
                    ModifiedBy = component.ModifiedBy
                };

                db.MonthlyProjectReportCompletnessComponent.Attach(entity);
                db.MonthlyProjectReportCompletnessComponent.Remove(entity);
                db.SaveChanges();

                PerformanceEvnHelper.UpdateMonthlyReport(entity.MonthlyProjectReportId);

            }

            return Json(new[] { component }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}