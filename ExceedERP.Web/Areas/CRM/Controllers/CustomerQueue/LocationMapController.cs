using System;
//////using System.Data
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.CRM;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMQueueSetting)]
    public class LocationMapController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Branches"] = db.Branch.Select(b => new { b.BranchId, b.Name, b.Abbreviation }).ToList();
            return View();
        }

        public ActionResult LocationMap_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<LocationMap> locationmaps = db.LocationMap;
           
            DataSourceResult result = locationmaps.ToDataSourceResult(request, locationMap => new {
                LocationMapId = locationMap.LocationMapId,
                BranchId = locationMap.BranchId,
                LocationName = locationMap.LocationName,
                Description = locationMap.Description,
                OrgBranchName = locationMap.OrgBranchName,
                DateCreated = locationMap.DateCreated,
                LastModified = locationMap.LastModified,
                CreatedBy = locationMap.CreatedBy,
                ModifiedBy = locationMap.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LocationMap_Create([DataSourceRequest]DataSourceRequest request, LocationMap locationMap)
        {
            if (ModelState.IsValid)
            {
                var entity = new LocationMap
                {
                    BranchId = locationMap.BranchId,
                    LocationName = locationMap.LocationName,
                    Description = locationMap.Description,
                    OrgBranchName = locationMap.OrgBranchName,
                    DateCreated = locationMap.DateCreated,
                    LastModified = locationMap.LastModified,
                    CreatedBy = locationMap.CreatedBy,
                    ModifiedBy = locationMap.ModifiedBy
                };

                db.LocationMap.Add(entity);
                db.SaveChanges();
                locationMap.LocationMapId = entity.LocationMapId;
            }

            return Json(new[] { locationMap }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LocationMap_Update([DataSourceRequest]DataSourceRequest request, LocationMap locationMap)
        {
            if (ModelState.IsValid)
            {
                var entity = new LocationMap
                {
                    LocationMapId = locationMap.LocationMapId,
                    BranchId = locationMap.BranchId,
                    LocationName = locationMap.LocationName,
                    Description = locationMap.Description,
                    OrgBranchName = locationMap.OrgBranchName,
                    DateCreated = locationMap.DateCreated,
                    LastModified = locationMap.LastModified,
                    CreatedBy = locationMap.CreatedBy,
                    ModifiedBy = locationMap.ModifiedBy
                };

                db.LocationMap.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { locationMap }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LocationMap_Destroy([DataSourceRequest]DataSourceRequest request, LocationMap locationMap)
        {
            if (ModelState.IsValid)
            {
                var entity = new LocationMap
                {
                    LocationMapId = locationMap.LocationMapId,
                    BranchId = locationMap.BranchId,
                    LocationName = locationMap.LocationName,
                    Description = locationMap.Description,
                    OrgBranchName = locationMap.OrgBranchName,
                    DateCreated = locationMap.DateCreated,
                    LastModified = locationMap.LastModified,
                    CreatedBy = locationMap.CreatedBy,
                    ModifiedBy = locationMap.ModifiedBy
                };

                db.LocationMap.Attach(entity);
                db.LocationMap.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { locationMap }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
        private string GetUserFullName(string userName)
        {
            var identityDb = new ApplicationDbContext();
            string name = userName;
            var currentUser = identityDb.Users.FirstOrDefault(u => u.UserName == userName);
            if (currentUser != null)
            {
                name = $"{currentUser.FirstName} {currentUser.MiddleName} {currentUser.LastName}";
            }
            return name;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
