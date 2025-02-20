
using ExceedERP.Helpers.Common;
using Kendo.Mvc.UI;
using System;
using System.Linq;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using ExceedERP.Core.Domain.InfoDesk.Support;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers.Settings
{

    [Authorize]
    public class StatusTypeController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StatusTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<StatusType_HelpDesk> StatusTypes = db.StatusType_HelpDesks;
            DataSourceResult result = StatusTypes.ToDataSourceResult(request);

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StatusTypes_Create([DataSourceRequest]DataSourceRequest request, StatusType_HelpDesk StatusType)
        {

            var Group = db.StatusType_HelpDesks.Where(x => x.Name == StatusType.Name).ToList();
            if (Group.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("SameName", " Name Already Registered ! ");
                return Json(new[] { StatusType }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new StatusType_HelpDesk
                {
                    Name = StatusType.Name,
                    Color = StatusType.Color,
                    Description = StatusType.Description,
                    Status = StatusType.Status,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = StatusType.Remark
                };

                db.StatusType_HelpDesks.Add(entity);
                db.SaveChanges();
                StatusType.StatusType_HelpDeskId = entity.StatusType_HelpDeskId;
            }

            return Json(new[] { StatusType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StatusTypes_Update([DataSourceRequest]DataSourceRequest request, StatusType_HelpDesk StatusType)
        {
            if (ModelState.IsValid)
            {
                var entity = new StatusType_HelpDesk
                {
                    StatusType_HelpDeskId = StatusType.StatusType_HelpDeskId,
                    Name = StatusType.Name,
                    Color = StatusType.Color,
                    Status = StatusType.Status,
                    Description = StatusType.Description,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    Remark = StatusType.Remark
                };

                db.StatusType_HelpDesks.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { StatusType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StatusTypes_Destroy([DataSourceRequest]DataSourceRequest request, StatusType_HelpDesk StatusType)
        {
            if (ModelState.IsValid)
            {
                var entity = new StatusType_HelpDesk
                {
                    StatusType_HelpDeskId = StatusType.StatusType_HelpDeskId,
                    Remark = StatusType.Remark
                };

                db.StatusType_HelpDesks.Attach(entity);
                db.StatusType_HelpDesks.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Priority Type number " + StatusType.StatusType_HelpDeskId);

            }

            return Json(new[] { StatusType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
