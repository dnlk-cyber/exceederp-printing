using ExceedERP.Core.Domain.Production;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Production.Controllers.ProjectInformation
{
    public class ProjectController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            var settings = db.ProductionSettings;
            ViewData["ActivityTypes"] = settings.ToList();

            return View();
        }

        public ActionResult Project_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ProductionProject> projects = db.ProductionProjects;
            DataSourceResult result = projects.ToDataSourceResult(request, project => new
            {
                ProjectId = project.ProductionProjectId,
                Name = project.Name,
                Location = project.Location,
            });

            return Json(result);
        }

        //}
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Project_Create([DataSourceRequest]DataSourceRequest request, ProductionProject project)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionProject
                {
                    Name = project.Name,
                    Location = project.Location,
                };

                db.ProductionProjects.Add(entity);
                db.SaveChanges();
                project.ProductionProjectId = entity.ProductionProjectId;
            }

            return Json(new[] { project }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Project_Update([DataSourceRequest]DataSourceRequest request, ProductionProject project)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionProject
                {
                    ProductionProjectId = project.ProductionProjectId,
                    Name = project.Name,
                    Location = project.Location,
                };

                db.ProductionProjects.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

            return Json(new[] { project }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Project_Destroy([DataSourceRequest]DataSourceRequest request, ProductionProject project)
        {
            if (ModelState.IsValid)
            {
                var entity = new ProductionProject
                {
                    ProductionProjectId = project.ProductionProjectId,
                    Name = project.Name,
                    Location = project.Location,
                };

                db.ProductionProjects.Attach(entity);
                db.ProductionProjects.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { project }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}