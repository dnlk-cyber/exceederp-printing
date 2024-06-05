using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.FleetManagement.FleetRegistration;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
 

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    public class EmployeeFleetMapController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/AlertLevel
        public ActionResult Index()
        {
            ViewData["JobTitleCodes"] = db.Lookups.ToList();
            return View();
        }

        public ActionResult EmployeeFleetMap_Read([DataSourceRequest] DataSourceRequest request)
        {
            var HRFleetMapping = db.HRFleetMapping.ToList();

            return Json(HRFleetMapping.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeFleetMap_Create([DataSourceRequest] DataSourceRequest request, HRFleetMapping HRFleetMapping)
        {

            var Maps = db.HRFleetMapping.Where(x => x.JobTitleCode == HRFleetMapping.JobTitleCode && x.PlacmentType == HRFleetMapping.PlacmentType ).ToList();
            if (Maps.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("", "Mapping Already Exist!");
                return Json(new[] { HRFleetMapping }.ToDataSourceResult(request, ModelState));

            }
            ModelState.Remove("HRFleetMappingId");


            HRFleetMapping.HRFleetMappingId = Guid.NewGuid();

            HRFleetMapping.DateCreated = DateTime.Now;
            HRFleetMapping.CreatedBy = User.Identity.Name;

            db.HRFleetMapping.Add(HRFleetMapping);
            db.SaveChanges();

            return Json(new[] { HRFleetMapping }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeFleetMap_Update([DataSourceRequest] DataSourceRequest request, HRFleetMapping HRFleetMapping)
        {
            var Maps = db.HRFleetMapping.Where(x => x.JobTitleCode == HRFleetMapping.JobTitleCode && x.PlacmentType == HRFleetMapping.PlacmentType && x.HRFleetMappingId != HRFleetMapping.HRFleetMappingId).ToList();
          
            if (Maps.Any())
            {
                ModelState.AddModelError("", "Mapping Already Exist!");
                return Json(new[] { HRFleetMapping }.ToDataSourceResult(request, ModelState));

            }
            if (HRFleetMapping != null)
            {
                HRFleetMapping.ModifiedBy = User.Identity.Name;
                HRFleetMapping.LastModified = DateTime.Now;

                db.HRFleetMapping.Attach(HRFleetMapping);
                db.Entry(HRFleetMapping).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { HRFleetMapping }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeFleetMap_Destroy([DataSourceRequest] DataSourceRequest request, HRFleetMapping HRFleetMapping)
        {
            var HRFleetMappings = db.HRFleetMapping.Where(x => x.HRFleetMappingId == HRFleetMapping.HRFleetMappingId).FirstOrDefault();
            if (HRFleetMappings != null)
            {

                db.HRFleetMapping.Attach(HRFleetMappings);
                db.HRFleetMapping.Remove(HRFleetMappings);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted HRFleetMapping number " + HRFleetMapping.HRFleetMappingId);

            }

            return Json(new[] { HRFleetMapping }.ToDataSourceResult(request));
        }


        public ActionResult JobTitleDDBind()
        {

            var ItemCategory = db.Lookups.Where(x=>x.LookupType == LookUpTypes.JOB_TITLE).Select(s => new
            {
                Text = s.Description,
                Value = s.LookupId
            });

            return Json(ItemCategory, JsonRequestBehavior.AllowGet);
        }
    }
}
