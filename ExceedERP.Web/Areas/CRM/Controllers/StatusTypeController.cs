using ExceedERP.Core.Domain.CRM;
using ExceedERP.Helpers.Common;  
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class StatusTypeController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StatusTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<StatusType> StatusTypes = db.StatusTypes;
            DataSourceResult result = StatusTypes.ToDataSourceResult(request);

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StatusTypes_Create([DataSourceRequest]DataSourceRequest request, StatusType StatusType)
        {

            var Group = db.StatusTypes.Where(x => x.Name == StatusType.Name).ToList();
            if (Group.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("SameName", " Name Already Registered ! ");
                return Json(new[] { StatusType }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new StatusType
                {
                    Name = StatusType.Name,
                    Color = StatusType.Color,
                    Description = StatusType.Description,
                    Status = StatusType.Status,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = StatusType.Remark
                };

                db.StatusTypes.Add(entity);
                db.SaveChanges();
                StatusType.StatusTypeId = entity.StatusTypeId;
            }

            return Json(new[] { StatusType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StatusTypes_Update([DataSourceRequest]DataSourceRequest request, StatusType StatusType)
        {
            if (ModelState.IsValid)
            {
                var entity = new StatusType
                {
                    StatusTypeId = StatusType.StatusTypeId,
                    Name = StatusType.Name,
                    Color = StatusType.Color,
                    Status = StatusType.Status,
                    Description = StatusType.Description,
                    ModifiedBy = User.Identity.Name,
                    LastModified = DateTime.Now,
                    Remark = StatusType.Remark
                };

                db.StatusTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { StatusType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult StatusTypes_Destroy([DataSourceRequest]DataSourceRequest request, StatusType StatusType)
        {
            if (ModelState.IsValid)
            {
                var entity = new StatusType
                {
                    StatusTypeId = StatusType.StatusTypeId,
                    Remark = StatusType.Remark
                };

                db.StatusTypes.Attach(entity);
                db.StatusTypes.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Priority Type number " + StatusType.StatusTypeId);

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
