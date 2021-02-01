using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;

namespace ExceedERP.Web.Areas.Construction.Controllers.Project
{
    public class ProjectConstraintsAndRiskController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ProjectConstraint_Read([DataSourceRequest]DataSourceRequest request, int idOne)
        {
            IQueryable<ProjectConstraintsAndRisk> projectconstraint = db.ProjectConstraint.Where( x=> x.ProjectInformationId==idOne );
            DataSourceResult result = projectconstraint.ToDataSourceResult(request, projectConstraintsAndRisk => new {
                ProjectConstraintsAndRiskId = projectConstraintsAndRisk.ProjectConstraintsAndRiskId,
                Assumptions = projectConstraintsAndRisk.Assumptions,
                Constraints = projectConstraintsAndRisk.Constraints,
                RiskDescription = projectConstraintsAndRisk.RiskDescription,
                RiskStatus = projectConstraintsAndRisk.RiskStatus,
                RiskImpact = projectConstraintsAndRisk.RiskImpact,
                RiskProbablity = projectConstraintsAndRisk.RiskProbablity,
                ProjectInformationId = projectConstraintsAndRisk.ProjectInformationId ,
                OrgBranchName = projectConstraintsAndRisk.OrgBranchName,
                DateCreated = projectConstraintsAndRisk.DateCreated,
                LastModified = projectConstraintsAndRisk.LastModified,
                CreatedBy = projectConstraintsAndRisk.CreatedBy,
                ModifiedBy = projectConstraintsAndRisk.ModifiedBy 
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectConstraint_Create([DataSourceRequest]DataSourceRequest request, ProjectConstraintsAndRisk projectConstraintsAndRisk, int idTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectConstraintsAndRisk
                {
                    Assumptions = projectConstraintsAndRisk.Assumptions,
                    Constraints = projectConstraintsAndRisk.Constraints,
                    RiskDescription = projectConstraintsAndRisk.RiskDescription,
                    RiskStatus = projectConstraintsAndRisk.RiskStatus,
                    RiskImpact = projectConstraintsAndRisk.RiskImpact,
                    RiskProbablity = projectConstraintsAndRisk.RiskProbablity,
                    ProjectInformationId = idTwo,
                    OrgBranchName = projectConstraintsAndRisk.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectConstraintsAndRisk.LastModified,
                    CreatedBy = User.Identity .Name ,
                    ModifiedBy = projectConstraintsAndRisk.ModifiedBy 
                };

                db.ProjectConstraint.Add(entity);
                db.SaveChanges();
                projectConstraintsAndRisk.ProjectConstraintsAndRiskId = entity.ProjectConstraintsAndRiskId;
            }

            return Json(new[] { projectConstraintsAndRisk }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectConstraint_Update([DataSourceRequest]DataSourceRequest request, ProjectConstraintsAndRisk projectConstraintsAndRisk)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectConstraintsAndRisk
                {
                    ProjectConstraintsAndRiskId = projectConstraintsAndRisk.ProjectConstraintsAndRiskId,
                    Assumptions = projectConstraintsAndRisk.Assumptions,
                    Constraints = projectConstraintsAndRisk.Constraints,
                    RiskDescription = projectConstraintsAndRisk.RiskDescription,
                    RiskStatus = projectConstraintsAndRisk.RiskStatus,
                    RiskImpact = projectConstraintsAndRisk.RiskImpact,
                    RiskProbablity = projectConstraintsAndRisk.RiskProbablity,
                    ProjectInformationId = projectConstraintsAndRisk.ProjectInformationId ,
                    OrgBranchName = projectConstraintsAndRisk.OrgBranchName,
                    DateCreated = projectConstraintsAndRisk.DateCreated,
                    LastModified = DateTime .Now ,
                    CreatedBy = projectConstraintsAndRisk.CreatedBy,
                    ModifiedBy = User.Identity .Name  
                };

                db.ProjectConstraint.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectConstraintsAndRisk }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectConstraint_Destroy([DataSourceRequest]DataSourceRequest request, ProjectConstraintsAndRisk projectConstraintsAndRisk)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectConstraintsAndRisk
                {
                    ProjectConstraintsAndRiskId = projectConstraintsAndRisk.ProjectConstraintsAndRiskId,
                    Assumptions = projectConstraintsAndRisk.Assumptions,
                    Constraints = projectConstraintsAndRisk.Constraints,
                    RiskDescription = projectConstraintsAndRisk.RiskDescription,
                    RiskStatus = projectConstraintsAndRisk.RiskStatus,
                    RiskImpact = projectConstraintsAndRisk.RiskImpact,
                    RiskProbablity = projectConstraintsAndRisk.RiskProbablity,
                    ProjectInformationId = projectConstraintsAndRisk.ProjectInformationId,
                    OrgBranchName = projectConstraintsAndRisk.OrgBranchName,
                    DateCreated = projectConstraintsAndRisk.DateCreated,
                    LastModified = projectConstraintsAndRisk.LastModified,
                    CreatedBy = projectConstraintsAndRisk.CreatedBy,
                    ModifiedBy = projectConstraintsAndRisk.ModifiedBy 
                };

                db.ProjectConstraint.Attach(entity);
                db.ProjectConstraint.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectConstraintsAndRisk }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
