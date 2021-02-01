using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;

namespace ExceedERP.Web.Areas.Construction.Controllers.Estimation
{
    public class ProjectEstimationBoqController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectEstimationBoq_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectEstimationBoq> projectestimationboq = db.ProjectEstimationBoq;
            DataSourceResult result = projectestimationboq.ToDataSourceResult(request, projectEstimationBoq => new {
                ProjectEstimationBoqid = projectEstimationBoq.ProjectEstimationBoqid,
                TypeOfBoq = projectEstimationBoq.TypeOfBoq,
                BillNumber = projectEstimationBoq.BillNumber,
                Description = projectEstimationBoq.Description,
                TotalCost = projectEstimationBoq.TotalCost,
                OrgBranchName = projectEstimationBoq.OrgBranchName,
                DateCreated = projectEstimationBoq.DateCreated,
                LastModified = projectEstimationBoq.LastModified,
                CreatedBy = projectEstimationBoq.CreatedBy,
                ModifiedBy = projectEstimationBoq.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationBoq_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationBoq projectEstimationBoq)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationBoq
                {
                    TypeOfBoq = projectEstimationBoq.TypeOfBoq,
                    BillNumber = projectEstimationBoq.BillNumber,
                    Description = projectEstimationBoq.Description,
                    TotalCost = projectEstimationBoq.TotalCost,
                    OrgBranchName = projectEstimationBoq.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectEstimationBoq.LastModified,
                    CreatedBy =User .Identity .Name ,
                    ModifiedBy = projectEstimationBoq.ModifiedBy
                };

                db.ProjectEstimationBoq.Add(entity);
                db.SaveChanges();
                projectEstimationBoq.ProjectEstimationBoqid = entity.ProjectEstimationBoqid;
            }

            return Json(new[] { projectEstimationBoq }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationBoq_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationBoq projectEstimationBoq)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationBoq
                {
                    ProjectEstimationBoqid = projectEstimationBoq.ProjectEstimationBoqid,
                    TypeOfBoq = projectEstimationBoq.TypeOfBoq,
                    BillNumber = projectEstimationBoq.BillNumber,
                    Description = projectEstimationBoq.Description,
                    TotalCost = projectEstimationBoq.TotalCost,
                    OrgBranchName = projectEstimationBoq.OrgBranchName,
                    DateCreated = projectEstimationBoq.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectEstimationBoq.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.ProjectEstimationBoq.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationBoq }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationBoq_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationBoq projectEstimationBoq)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationBoq
                {
                    ProjectEstimationBoqid = projectEstimationBoq.ProjectEstimationBoqid,
                    TypeOfBoq = projectEstimationBoq.TypeOfBoq,
                    BillNumber = projectEstimationBoq.BillNumber,
                    Description = projectEstimationBoq.Description,
                    TotalCost = projectEstimationBoq.TotalCost,
                    OrgBranchName = projectEstimationBoq.OrgBranchName,
                    DateCreated = projectEstimationBoq.DateCreated,
                    LastModified = projectEstimationBoq.LastModified,
                    CreatedBy = projectEstimationBoq.CreatedBy,
                    ModifiedBy = projectEstimationBoq.ModifiedBy
                };

                db.ProjectEstimationBoq.Attach(entity);
                db.ProjectEstimationBoq.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationBoq }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
