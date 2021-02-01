
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.InfoDesk.Support;
using ExceedERP.Core.Domain.InfoDesk;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{

    public class PriorityTypeController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PriorityTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PriorityType_HelpDesk> PriorityTypes = db.PriorityType_HelpDesks;
            DataSourceResult result = PriorityTypes.ToDataSourceResult(request);

            return Json(result);
        }
        [AuthorizeRoles(InfoDeskConstants.HelpDeskAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriorityTypes_Create([DataSourceRequest]DataSourceRequest request, PriorityType_HelpDesk PriorityType)
        {
            var Group = db.PriorityType_HelpDesks.Where(x => x.Name == PriorityType.Name).ToList();
            if (Group.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("SameName", " Name Already Registered ! ");
                return Json(new[] { PriorityType }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new PriorityType_HelpDesk
                {
                    Name = PriorityType.Name,
                    Color = PriorityType.Color,
                    Description = PriorityType.Description,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = PriorityType.Remark
                };

                db.PriorityType_HelpDesks.Add(entity);
                db.SaveChanges();
                PriorityType.PriorityType_HelpDeskId = entity.PriorityType_HelpDeskId;
            }

            return Json(new[] { PriorityType }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(InfoDeskConstants.HelpDeskAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriorityTypes_Update([DataSourceRequest]DataSourceRequest request, PriorityType_HelpDesk PriorityType)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new PriorityType_HelpDesk
                {
                    PriorityType_HelpDeskId = PriorityType.PriorityType_HelpDeskId,
                    Name = PriorityType.Name,
                    Color = PriorityType.Color,
                    Description = PriorityType.Description,
                    Remark = PriorityType.Remark,
                };

                db.PriorityType_HelpDesks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { PriorityType }.ToDataSourceResult(request, ModelState));
        }
        [AuthorizeRoles(InfoDeskConstants.HelpDeskAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriorityTypes_Destroy([DataSourceRequest]DataSourceRequest request, PriorityType_HelpDesk PriorityType)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriorityType_HelpDesk
                {
                    PriorityType_HelpDeskId = PriorityType.PriorityType_HelpDeskId,
                    Remark = PriorityType.Remark
                };

                db.PriorityType_HelpDesks.Attach(entity);
                db.PriorityType_HelpDesks.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Priority Type number " + PriorityType.PriorityType_HelpDeskId);

            }

            return Json(new[] { PriorityType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
