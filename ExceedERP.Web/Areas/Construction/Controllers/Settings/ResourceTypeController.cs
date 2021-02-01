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
    public class ResourceTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ResourceTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ResourceType> resourceTypes = db.ResourceTypes;
            DataSourceResult result = resourceTypes.ToDataSourceResult(request, resourceType => new {
                ResourceTypeId = resourceType.ResourceTypeId,
                Name = resourceType.Name,
                Description = resourceType.Description,
                //Resources = resourceType.Resources
            });

            return Json(result);
        }

        public JsonResult GetAllResourceTypes()
        {
            var items = db.ResourceTypes;
            //var items = db.ProjectInformation .Where (X=> X.ProjectStatus == ProjectStatus.APPROVED || X.ProjectStatus == ProjectStatus.STARTED);
            return Json(items.Select(p => new { ResourceTypeId = p.ResourceTypeId, Name = p.Name }), JsonRequestBehavior.AllowGet);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceTypes_Create([DataSourceRequest]DataSourceRequest request, ResourceType resourceType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceType
                {
                   ResourceTypeId = resourceType.ResourceTypeId,
                   Name = resourceType.Name,
                   Description = resourceType.Description,
                   //Resources = resourceType.Resources
                };

                db.ResourceTypes.Add(entity);
                db.SaveChanges();
                resourceType.ResourceTypeId = entity.ResourceTypeId;
            }

            return Json(new[] { resourceType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceTypes_Update([DataSourceRequest]DataSourceRequest request, ResourceType resourceType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceType
                {
                    ResourceTypeId = resourceType.ResourceTypeId,
                    Name = resourceType.Name,
                    Description = resourceType.Description,
                    //Resources = resourceType.Resources
                };

                db.ResourceTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { resourceType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ResourceTypes_Destroy([DataSourceRequest]DataSourceRequest request, ResourceType resourceType)
        {
            if (ModelState.IsValid)
            {
                var entity = new ResourceType
                {
                    ResourceTypeId = resourceType.ResourceTypeId,
                    Name = resourceType.Name,
                    Description = resourceType.Description,
                    //Resources = resourceType.Resources
                };

                db.ResourceTypes.Attach(entity);
                db.ResourceTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { resourceType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}