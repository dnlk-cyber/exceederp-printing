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
    public class ProjectEstimationCoefficientsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectEstimationCoefficients_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProjectEstimationCoefficients> projectestimationcoefficients = db.ProjectEstimationCoefficients;
            DataSourceResult result = projectestimationcoefficients.ToDataSourceResult(request, projectEstimationCoefficients => new {
                ProjectEstimationCoefficientsId = projectEstimationCoefficients.ProjectEstimationCoefficientsId,
                ItemNo = projectEstimationCoefficients.ItemNo,
                Description = projectEstimationCoefficients.Description,
                ItemName = projectEstimationCoefficients.ItemName,
                Ratio = projectEstimationCoefficients.Ratio,
                Kg = projectEstimationCoefficients.Kg,
                Shrinkage = projectEstimationCoefficients.Shrinkage,
                Wastage = projectEstimationCoefficients.Wastage,
                Time = projectEstimationCoefficients.Time,
                Quantity = projectEstimationCoefficients.Quantity,
                OrgBranchName = projectEstimationCoefficients.OrgBranchName,
                DateCreated = projectEstimationCoefficients.DateCreated,
                LastModified = projectEstimationCoefficients.LastModified,
                CreatedBy = projectEstimationCoefficients.CreatedBy,
                ModifiedBy = projectEstimationCoefficients.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationCoefficients_Create([DataSourceRequest]DataSourceRequest request, ProjectEstimationCoefficients projectEstimationCoefficients)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationCoefficients
                {
                    ItemNo = projectEstimationCoefficients.ItemNo,
                    Description = projectEstimationCoefficients.Description,
                    ItemName = projectEstimationCoefficients.ItemName,
                    Ratio = projectEstimationCoefficients.Ratio,
                    Kg = projectEstimationCoefficients.Kg,
                    Shrinkage = projectEstimationCoefficients.Shrinkage,
                    Wastage = projectEstimationCoefficients.Wastage,
                    Time = projectEstimationCoefficients.Time,
                    Quantity = projectEstimationCoefficients.Quantity,
                    OrgBranchName = projectEstimationCoefficients.OrgBranchName,
                    DateCreated = DateTime .Now ,
                    LastModified = projectEstimationCoefficients.LastModified,
                    CreatedBy = User .Identity .Name ,
                    ModifiedBy = projectEstimationCoefficients.ModifiedBy
                };

                db.ProjectEstimationCoefficients.Add(entity);
                db.SaveChanges();
                projectEstimationCoefficients.ProjectEstimationCoefficientsId = entity.ProjectEstimationCoefficientsId;
            }

            return Json(new[] { projectEstimationCoefficients }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationCoefficients_Update([DataSourceRequest]DataSourceRequest request, ProjectEstimationCoefficients projectEstimationCoefficients)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationCoefficients
                {
                    ProjectEstimationCoefficientsId = projectEstimationCoefficients.ProjectEstimationCoefficientsId,
                    ItemNo = projectEstimationCoefficients.ItemNo,
                    Description = projectEstimationCoefficients.Description,
                    ItemName = projectEstimationCoefficients.ItemName,
                    Ratio = projectEstimationCoefficients.Ratio,
                    Kg = projectEstimationCoefficients.Kg,
                    Shrinkage = projectEstimationCoefficients.Shrinkage,
                    Wastage = projectEstimationCoefficients.Wastage,
                    Time = projectEstimationCoefficients.Time,
                    Quantity = projectEstimationCoefficients.Quantity,
                    OrgBranchName = projectEstimationCoefficients.OrgBranchName,
                    DateCreated = projectEstimationCoefficients.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectEstimationCoefficients.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.ProjectEstimationCoefficients.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationCoefficients }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectEstimationCoefficients_Destroy([DataSourceRequest]DataSourceRequest request, ProjectEstimationCoefficients projectEstimationCoefficients)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectEstimationCoefficients
                {
                    ProjectEstimationCoefficientsId = projectEstimationCoefficients.ProjectEstimationCoefficientsId,
                    ItemNo = projectEstimationCoefficients.ItemNo,
                    Description = projectEstimationCoefficients.Description,
                    ItemName = projectEstimationCoefficients.ItemName,
                    Ratio = projectEstimationCoefficients.Ratio,
                    Kg = projectEstimationCoefficients.Kg,
                    Shrinkage = projectEstimationCoefficients.Shrinkage,
                    Wastage = projectEstimationCoefficients.Wastage,
                    Time = projectEstimationCoefficients.Time,
                    Quantity = projectEstimationCoefficients.Quantity,
                    OrgBranchName = projectEstimationCoefficients.OrgBranchName,
                    DateCreated = projectEstimationCoefficients.DateCreated,
                    LastModified = projectEstimationCoefficients.LastModified,
                    CreatedBy = projectEstimationCoefficients.CreatedBy,
                    ModifiedBy = projectEstimationCoefficients.ModifiedBy
                };

                db.ProjectEstimationCoefficients.Attach(entity);
                db.ProjectEstimationCoefficients.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectEstimationCoefficients }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
