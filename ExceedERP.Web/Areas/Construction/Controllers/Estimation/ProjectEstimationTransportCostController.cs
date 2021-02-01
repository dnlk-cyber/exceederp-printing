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
    public class ProjectEstimationTransportCostController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectEstimationTransportCost_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectEstimationTransportCost> projectestimationtransportcost = db.ProjectEstimationTransportCost;
            DataSourceResult result = projectestimationtransportcost.ToDataSourceResult(request, projectEstimationTransportCost => new {
                ProjectEstimationTransportCostId = projectEstimationTransportCost.ProjectEstimationTransportCostId,
                ItemNo = projectEstimationTransportCost.ItemNo,
                Description = projectEstimationTransportCost.Description,
                UnitFactory = projectEstimationTransportCost.UnitFactory,
                Amount = projectEstimationTransportCost.Amount,
                OrgBranchName = projectEstimationTransportCost.OrgBranchName,
                DateCreated = projectEstimationTransportCost.DateCreated,
                LastModified = projectEstimationTransportCost.LastModified,
                CreatedBy = projectEstimationTransportCost.CreatedBy,
                ModifiedBy = projectEstimationTransportCost.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationTransportCost_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationTransportCost projectEstimationTransportCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationTransportCost
                {
                    ItemNo = projectEstimationTransportCost.ItemNo,
                    Description = projectEstimationTransportCost.Description,
                    UnitFactory = projectEstimationTransportCost.UnitFactory,
                    Amount = projectEstimationTransportCost.Amount,
                    OrgBranchName = projectEstimationTransportCost.OrgBranchName,
                    DateCreated = DateTime .Now ,
                    LastModified = projectEstimationTransportCost.LastModified,
                    CreatedBy = User .Identity .Name ,
                    ModifiedBy = projectEstimationTransportCost.ModifiedBy 
                };

                db.ProjectEstimationTransportCost.Add(entity);
                db.SaveChanges();
                projectEstimationTransportCost.ProjectEstimationTransportCostId = entity.ProjectEstimationTransportCostId;
            }

            return Json(new[] { projectEstimationTransportCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationTransportCost_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationTransportCost projectEstimationTransportCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationTransportCost
                {
                    ProjectEstimationTransportCostId = projectEstimationTransportCost.ProjectEstimationTransportCostId,
                    ItemNo = projectEstimationTransportCost.ItemNo,
                    Description = projectEstimationTransportCost.Description,
                    UnitFactory = projectEstimationTransportCost.UnitFactory,
                    Amount = projectEstimationTransportCost.Amount,
                    OrgBranchName = projectEstimationTransportCost.OrgBranchName,
                    DateCreated = projectEstimationTransportCost.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectEstimationTransportCost.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.ProjectEstimationTransportCost.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationTransportCost }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationTransportCost_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationTransportCost projectEstimationTransportCost)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationTransportCost
                {
                    ProjectEstimationTransportCostId = projectEstimationTransportCost.ProjectEstimationTransportCostId,
                    ItemNo = projectEstimationTransportCost.ItemNo,
                    Description = projectEstimationTransportCost.Description,
                    UnitFactory = projectEstimationTransportCost.UnitFactory,
                    Amount = projectEstimationTransportCost.Amount,
                    OrgBranchName = projectEstimationTransportCost.OrgBranchName,
                    DateCreated = projectEstimationTransportCost.DateCreated,
                    LastModified = projectEstimationTransportCost.LastModified,
                    CreatedBy = projectEstimationTransportCost.CreatedBy,
                    ModifiedBy = projectEstimationTransportCost.ModifiedBy 
                };

                db.ProjectEstimationTransportCost.Attach(entity);
                db.ProjectEstimationTransportCost.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationTransportCost }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
