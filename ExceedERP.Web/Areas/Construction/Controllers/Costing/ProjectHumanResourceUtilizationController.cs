using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Costing
{
    public class ProjectHumanResourceUtilizationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        
        public ActionResult Index()
        {

            DataProviderController inputs = new DataProviderController();
            ViewData["EmployeeList"] = inputs. GetEmployees(User.Identity.Name ).Where(x => x.IsActive);
            return View();
        }

        public ActionResult ProjectHumanResourceUtilization_Read([DataSourceRequest]DataSourceRequest request, int idd)
        {
            IQueryable<ProjectHumanResourceUtilization> projecthumanresourceutilization = db.ProjectHumanResourceUtilization.Where( x=> x.ProjectInformationId==idd);
            DataSourceResult result = projecthumanresourceutilization.ToDataSourceResult(request, projectHumanResourceUtilization => new {
                ProjectHumanResourceUtilizationId = projectHumanResourceUtilization.ProjectHumanResourceUtilizationId,
                CrewCode = projectHumanResourceUtilization.CrewCode,
                CrewName = projectHumanResourceUtilization.CrewName,
                EmployeeId = projectHumanResourceUtilization.EmployeeId,
                CombinationOfCrew = projectHumanResourceUtilization.CombinationOfCrew,
                TypeOfWork = projectHumanResourceUtilization.TypeOfWork,
                CrewOutput = projectHumanResourceUtilization.CrewOutput,
                CrewDirectCost = projectHumanResourceUtilization.CrewDirectCost,
                CrewIndirectCost = projectHumanResourceUtilization.CrewIndirectCost,
                ProjectInformationId = projectHumanResourceUtilization.ProjectInformationId,
                OrgBranchName = projectHumanResourceUtilization.OrgBranchName,
                DateCreated = projectHumanResourceUtilization.DateCreated,
                LastModified = projectHumanResourceUtilization.LastModified,
                CreatedBy = projectHumanResourceUtilization.CreatedBy,
                ModifiedBy = projectHumanResourceUtilization.ModifiedBy

            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectHumanResourceUtilization_Create([DataSourceRequest]DataSourceRequest request, ProjectHumanResourceUtilization projectHumanResourceUtilization, int idTwo)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectHumanResourceUtilization
                {
                    CrewCode = projectHumanResourceUtilization.CrewCode,
                    CrewName = projectHumanResourceUtilization.CrewName,
                    EmployeeId = projectHumanResourceUtilization.EmployeeId,
                    CombinationOfCrew = projectHumanResourceUtilization.CombinationOfCrew,
                    TypeOfWork = projectHumanResourceUtilization.TypeOfWork,
                    CrewOutput = projectHumanResourceUtilization.CrewOutput,
                    CrewDirectCost = projectHumanResourceUtilization.CrewDirectCost,
                    CrewIndirectCost = projectHumanResourceUtilization.CrewIndirectCost,
                    ProjectInformationId = idTwo ,
                    OrgBranchName = projectHumanResourceUtilization.OrgBranchName,
                    DateCreated = DateTime.Now,
                    LastModified = projectHumanResourceUtilization.LastModified,
                    CreatedBy = User .Identity .Name ,
                    ModifiedBy = projectHumanResourceUtilization.ModifiedBy
                };

                db.ProjectHumanResourceUtilization.Add(entity);
                db.SaveChanges();
                projectHumanResourceUtilization.ProjectHumanResourceUtilizationId = entity.ProjectHumanResourceUtilizationId;
            }

            return Json(new[] { projectHumanResourceUtilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectHumanResourceUtilization_Update([DataSourceRequest]DataSourceRequest request, ProjectHumanResourceUtilization projectHumanResourceUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectHumanResourceUtilization
                {
                    ProjectHumanResourceUtilizationId = projectHumanResourceUtilization.ProjectHumanResourceUtilizationId,
                    CrewCode = projectHumanResourceUtilization.CrewCode,
                    CrewName = projectHumanResourceUtilization.CrewName,
                    EmployeeId = projectHumanResourceUtilization.EmployeeId,
                    CombinationOfCrew = projectHumanResourceUtilization.CombinationOfCrew,
                    TypeOfWork = projectHumanResourceUtilization.TypeOfWork,
                    CrewOutput = projectHumanResourceUtilization.CrewOutput,
                    CrewDirectCost = projectHumanResourceUtilization.CrewDirectCost,
                    CrewIndirectCost = projectHumanResourceUtilization.CrewIndirectCost,
                    ProjectInformationId = projectHumanResourceUtilization.ProjectInformationId,
                    OrgBranchName = projectHumanResourceUtilization.OrgBranchName,
                    DateCreated = projectHumanResourceUtilization.DateCreated,
                    LastModified =DateTime .Now ,
                    CreatedBy = projectHumanResourceUtilization.CreatedBy,
                    ModifiedBy = User.Identity .Name 
                };

                db.ProjectHumanResourceUtilization.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { projectHumanResourceUtilization }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ProjectHumanResourceUtilization_Destroy([DataSourceRequest]DataSourceRequest request, ProjectHumanResourceUtilization projectHumanResourceUtilization)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProjectHumanResourceUtilization
                {
                    ProjectHumanResourceUtilizationId = projectHumanResourceUtilization.ProjectHumanResourceUtilizationId,
                    CrewCode = projectHumanResourceUtilization.CrewCode,
                    CrewName = projectHumanResourceUtilization.CrewName,
                    EmployeeId = projectHumanResourceUtilization.EmployeeId,
                    CombinationOfCrew = projectHumanResourceUtilization.CombinationOfCrew,
                    TypeOfWork = projectHumanResourceUtilization.TypeOfWork,
                    CrewOutput = projectHumanResourceUtilization.CrewOutput,
                    CrewDirectCost = projectHumanResourceUtilization.CrewDirectCost,
                    CrewIndirectCost = projectHumanResourceUtilization.CrewIndirectCost,
                    ProjectInformationId = projectHumanResourceUtilization.ProjectInformationId,
                    OrgBranchName = projectHumanResourceUtilization.OrgBranchName,
                    DateCreated = projectHumanResourceUtilization.DateCreated,
                    LastModified = projectHumanResourceUtilization.LastModified,
                    CreatedBy = projectHumanResourceUtilization.CreatedBy,
                    ModifiedBy = projectHumanResourceUtilization.ModifiedBy
                };

                db.ProjectHumanResourceUtilization.Attach(entity);
                db.ProjectHumanResourceUtilization.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { projectHumanResourceUtilization }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
