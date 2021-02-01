using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ProjectEstimationIndirectLabourController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectEstimationIndirectLabour_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectEstimationIndirectLabour> projectestimationindirectlabour = db.ProjectEstimationIndirectLabour;
            DataSourceResult result = projectestimationindirectlabour.ToDataSourceResult(request, projectEstimationIndirectLabour => new {
                ProjectEstimationIndirectLabourId = projectEstimationIndirectLabour.ProjectEstimationIndirectLabourId,
                ItemNo = projectEstimationIndirectLabour.ItemNo,
                LabourName = projectEstimationIndirectLabour.LabourName,
                Quantity = projectEstimationIndirectLabour.Quantity,
                UnitFactory = projectEstimationIndirectLabour.UnitFactory,
                DailyPay = projectEstimationIndirectLabour.DailyPay,
                TotalCost = projectEstimationIndirectLabour.TotalCost,
                OrgBranchName = projectEstimationIndirectLabour.OrgBranchName,
                DateCreated = projectEstimationIndirectLabour.DateCreated,
                LastModified = projectEstimationIndirectLabour.LastModified,
                CreatedBy = projectEstimationIndirectLabour.CreatedBy,
                ModifiedBy = projectEstimationIndirectLabour.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationIndirectLabour_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationIndirectLabour projectEstimationIndirectLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationIndirectLabour
                {
                    ItemNo = projectEstimationIndirectLabour.ItemNo,
                    LabourName = projectEstimationIndirectLabour.LabourName,
                    Quantity = projectEstimationIndirectLabour.Quantity,
                    UnitFactory = projectEstimationIndirectLabour.UnitFactory,
                    DailyPay = projectEstimationIndirectLabour.DailyPay,
                    TotalCost = projectEstimationIndirectLabour.TotalCost,
                    OrgBranchName = projectEstimationIndirectLabour.OrgBranchName,
                    DateCreated = DateTime .Now ,
                    LastModified = projectEstimationIndirectLabour.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = projectEstimationIndirectLabour.ModifiedBy 
                };

                db.ProjectEstimationIndirectLabour.Add(entity);
                db.SaveChanges();
                projectEstimationIndirectLabour.ProjectEstimationIndirectLabourId = entity.ProjectEstimationIndirectLabourId;
            }

            return Json(new[] { projectEstimationIndirectLabour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationIndirectLabour_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationIndirectLabour projectEstimationIndirectLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationIndirectLabour
                {
                    ProjectEstimationIndirectLabourId = projectEstimationIndirectLabour.ProjectEstimationIndirectLabourId,
                    ItemNo = projectEstimationIndirectLabour.ItemNo,
                    LabourName = projectEstimationIndirectLabour.LabourName,
                    Quantity = projectEstimationIndirectLabour.Quantity,
                    UnitFactory = projectEstimationIndirectLabour.UnitFactory,
                    DailyPay = projectEstimationIndirectLabour.DailyPay,
                    TotalCost = projectEstimationIndirectLabour.TotalCost,
                    OrgBranchName = projectEstimationIndirectLabour.OrgBranchName,
                    DateCreated = projectEstimationIndirectLabour.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectEstimationIndirectLabour.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.ProjectEstimationIndirectLabour.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationIndirectLabour }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationIndirectLabour_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationIndirectLabour projectEstimationIndirectLabour)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationIndirectLabour
                {
                    ProjectEstimationIndirectLabourId = projectEstimationIndirectLabour.ProjectEstimationIndirectLabourId,
                    ItemNo = projectEstimationIndirectLabour.ItemNo,
                    LabourName = projectEstimationIndirectLabour.LabourName,
                    Quantity = projectEstimationIndirectLabour.Quantity,
                    UnitFactory = projectEstimationIndirectLabour.UnitFactory,
                    DailyPay = projectEstimationIndirectLabour.DailyPay,
                    TotalCost = projectEstimationIndirectLabour.TotalCost,
                    OrgBranchName = projectEstimationIndirectLabour.OrgBranchName,
                    DateCreated = projectEstimationIndirectLabour.DateCreated,
                    LastModified = projectEstimationIndirectLabour.LastModified,
                    CreatedBy = projectEstimationIndirectLabour.CreatedBy,
                    ModifiedBy = projectEstimationIndirectLabour.ModifiedBy 
                };

                db.ProjectEstimationIndirectLabour.Attach(entity);
                db.ProjectEstimationIndirectLabour.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationIndirectLabour }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
