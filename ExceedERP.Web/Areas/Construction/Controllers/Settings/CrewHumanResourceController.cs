using ExceedERP.Core.Domain.Construction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    public class CrewHumanResourceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/CrewHumanResource
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CrewHumanResource_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CrewHumanResource> manPower = db.CrewHumanResources;
            DataSourceResult result = manPower.ToDataSourceResult(request, resource => new {
                CrewHumanResourceId = resource.CrewHumanResourceId,
                ProjectCrewId = resource.ProjectCrewId,
                Name = resource.Name,
                Quantity = resource.Quantity,
                JobTitle = resource.JobTitle
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CrewHumanResource_Create([DataSourceRequest]DataSourceRequest request, CrewHumanResource resource, int crewId )
        {
            if (ModelState.IsValid)
            {
                var entity = new CrewHumanResource
                {
                    CrewHumanResourceId = resource.CrewHumanResourceId,
                    ProjectCrewId = crewId,
                    Name = resource.Name,
                    Quantity = resource.Quantity,
                    JobTitle = resource.JobTitle
                };

                db.CrewHumanResources.Add(entity);
                db.SaveChanges();
                resource.CrewHumanResourceId = entity.CrewHumanResourceId;
            }

            return Json(new[] { resource }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CrewHumanResource_Update([DataSourceRequest]DataSourceRequest request, CrewHumanResource resource)
        {
            if (ModelState.IsValid)
            {
                var entity = new CrewHumanResource
                {
                    CrewHumanResourceId = resource.CrewHumanResourceId,
                    ProjectCrewId = resource.ProjectCrewId,
                    Name = resource.Name,
                    Quantity = resource.Quantity,
                    JobTitle = resource.JobTitle
                };

                db.CrewHumanResources.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { resource }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CrewHumanResource_Destroy([DataSourceRequest]DataSourceRequest request, CrewHumanResource resource)
        {
            if (ModelState.IsValid)
            {
                var entity = new CrewHumanResource
                {
                    CrewHumanResourceId = resource.CrewHumanResourceId,
                    ProjectCrewId = resource.ProjectCrewId,
                    Name = resource.Name,
                    Quantity = resource.Quantity,
                    JobTitle = resource.JobTitle
                };

                db.CrewHumanResources.Attach(entity);
                db.CrewHumanResources.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { resource }.ToDataSourceResult(request, ModelState));
        }
    }
}