using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Maintenance;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class WorkUnitSectionController : Controller
    {



        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/WorkUnitSection
        public ActionResult Index()
        {
      
            return View();
        }

        public ActionResult WorkUnitSection_Read([DataSourceRequest] DataSourceRequest request)
        {
            var WorkUnitSection = db.WorkUnitSection.ToList();

            return Json(WorkUnitSection.ToDataSourceResult(request));
        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkUnitSection_Create([DataSourceRequest] DataSourceRequest request, WorkUnitSection model)
        {
            ModelState.Remove("WorkUnitSectionId");

            var WorkUnitSections = db.WorkUnitSection.Where(x => x.Name == model.Name ).ToList();

            if (WorkUnitSections.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("Exist", " Section Already Exists !");
                return Json(new[] { model }.ToDataSourceResult(request, ModelState));
            }
            var Level = new WorkUnitSection
            {
                WorkUnitSectionId = Guid.NewGuid(),
                Name = model.Name,
                Description = model.Description,
                DateCreated = DateTime.Now,
                CreatedBy = User.Identity.Name

            };
            db.WorkUnitSection.Add(Level);
            db.SaveChanges();
            model.WorkUnitSectionId = Level.WorkUnitSectionId;

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkUnitSection_Update([DataSourceRequest] DataSourceRequest request, WorkUnitSection WorkUnitSection)
        {
            var WorkUnitSections = db.WorkUnitSection.Where(x => x.Name == WorkUnitSection.Name && x.WorkUnitSectionId != WorkUnitSection.WorkUnitSectionId).ToList();

            if (WorkUnitSections.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("Exist", " Section Already Exists !");
                return Json(new[] { WorkUnitSection }.ToDataSourceResult(request, ModelState));
            }
            if (WorkUnitSection != null)
            {
                WorkUnitSection.ModifiedBy = User.Identity.Name;
                WorkUnitSection.LastModified = DateTime.Now;

                db.WorkUnitSection.Attach(WorkUnitSection);
                db.Entry(WorkUnitSection).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { WorkUnitSection }.ToDataSourceResult(request));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult WorkUnitSection_Destroy([DataSourceRequest] DataSourceRequest request, WorkUnitSection WorkUnitSection)
        {
            var WorkUnitSections = db.WorkUnitSection.Where(x => x.WorkUnitSectionId == WorkUnitSection.WorkUnitSectionId).FirstOrDefault();
            if (WorkUnitSections != null)
            {

                db.WorkUnitSection.Attach(WorkUnitSections);
                db.WorkUnitSection.Remove(WorkUnitSections);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted WorkUnitSection number " + WorkUnitSection.WorkUnitSectionId);

            }

            return Json(new[] { WorkUnitSection }.ToDataSourceResult(request));
        }

        public ActionResult SectionDDBind()
        {

            var Sections = db.WorkUnitSection.ToList();

            return Json(Sections, JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
