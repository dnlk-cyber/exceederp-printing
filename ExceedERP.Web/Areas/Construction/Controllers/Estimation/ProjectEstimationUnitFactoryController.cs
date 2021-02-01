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
    public class ProjectEstimationUnitFactoryController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectEstimationUnitFactory_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectEstimationUnitFactory> projectestimationunitfactory = db.ProjectEstimationUnitFactory;
            DataSourceResult result = projectestimationunitfactory.ToDataSourceResult(request, projectEstimationUnitFactory => new {
                ProjectEstimationUnitFactoryId = projectEstimationUnitFactory.ProjectEstimationUnitFactoryId,
                ItemNo = projectEstimationUnitFactory.ItemNo,
                FactoryName = projectEstimationUnitFactory.FactoryName,
                Description = projectEstimationUnitFactory.Description,
                Quantity = projectEstimationUnitFactory.Quantity,
                UnitRate = projectEstimationUnitFactory.UnitRate,
                TotalCost = projectEstimationUnitFactory.TotalCost,
                OrgBranchName = projectEstimationUnitFactory.OrgBranchName,
                DateCreated = projectEstimationUnitFactory.DateCreated,
                LastModified = projectEstimationUnitFactory.LastModified,
                CreatedBy = projectEstimationUnitFactory.CreatedBy,
                ModifiedBy = projectEstimationUnitFactory.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationUnitFactory_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationUnitFactory projectEstimationUnitFactory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationUnitFactory
                {
                    ItemNo = projectEstimationUnitFactory.ItemNo,
                    FactoryName = projectEstimationUnitFactory.FactoryName,
                    Description = projectEstimationUnitFactory.Description,
                    Quantity = projectEstimationUnitFactory.Quantity,
                    UnitRate = projectEstimationUnitFactory.UnitRate,
                    TotalCost = projectEstimationUnitFactory.TotalCost,
                    OrgBranchName = projectEstimationUnitFactory.OrgBranchName,
                    DateCreated = DateTime .Now ,
                    LastModified = projectEstimationUnitFactory.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = projectEstimationUnitFactory.ModifiedBy 
                };

                db.ProjectEstimationUnitFactory.Add(entity);
                db.SaveChanges();
                projectEstimationUnitFactory.ProjectEstimationUnitFactoryId = entity.ProjectEstimationUnitFactoryId;
            }

            return Json(new[] { projectEstimationUnitFactory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationUnitFactory_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationUnitFactory projectEstimationUnitFactory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationUnitFactory
                {
                    ProjectEstimationUnitFactoryId = projectEstimationUnitFactory.ProjectEstimationUnitFactoryId,
                    ItemNo = projectEstimationUnitFactory.ItemNo,
                    FactoryName = projectEstimationUnitFactory.FactoryName,
                    Description = projectEstimationUnitFactory.Description,
                    Quantity = projectEstimationUnitFactory.Quantity,
                    UnitRate = projectEstimationUnitFactory.UnitRate,
                    TotalCost = projectEstimationUnitFactory.TotalCost,
                    OrgBranchName = projectEstimationUnitFactory.OrgBranchName,
                    DateCreated = projectEstimationUnitFactory.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectEstimationUnitFactory.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.ProjectEstimationUnitFactory.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationUnitFactory }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationUnitFactory_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationUnitFactory projectEstimationUnitFactory)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationUnitFactory
                {
                    ProjectEstimationUnitFactoryId = projectEstimationUnitFactory.ProjectEstimationUnitFactoryId,
                    ItemNo = projectEstimationUnitFactory.ItemNo,
                    FactoryName = projectEstimationUnitFactory.FactoryName,
                    Description = projectEstimationUnitFactory.Description,
                    Quantity = projectEstimationUnitFactory.Quantity,
                    UnitRate = projectEstimationUnitFactory.UnitRate,
                    TotalCost = projectEstimationUnitFactory.TotalCost,
                    OrgBranchName = projectEstimationUnitFactory.OrgBranchName,
                    DateCreated = projectEstimationUnitFactory.DateCreated,
                    LastModified = projectEstimationUnitFactory.LastModified,
                    CreatedBy = projectEstimationUnitFactory.CreatedBy,
                    ModifiedBy = projectEstimationUnitFactory.ModifiedBy 
                };

                db.ProjectEstimationUnitFactory.Attach(entity);
                db.ProjectEstimationUnitFactory.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationUnitFactory }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
