using ExceedERP.Core.Domain.CRM;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using ExceedERP.DataAccess.Context;
using System.Data.Entity;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.CRM.Controllers
{
    [AuthorizeRoles(CRMRoles.CRMAdmin)]
    public class PriorityTypeController : Controller
    {

        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PriorityTypes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<PriorityType> PriorityTypes = db.PriorityTypes;
            DataSourceResult result = PriorityTypes.ToDataSourceResult(request);

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriorityTypes_Create([DataSourceRequest]DataSourceRequest request, PriorityType PriorityType)
        {
            var Group = db.PriorityTypes.Where(x => x.Name == PriorityType.Name).ToList();
            if (Group.Any())
            {
                ModelState.Clear();
                ModelState.AddModelError("SameName", " Name Already Registered ! ");
                return Json(new[] { PriorityType }.ToDataSourceResult(request, ModelState));
            }
            if (ModelState.IsValid)
            {
                var entity = new PriorityType
                {
                    Name = PriorityType.Name,
                    Color = PriorityType.Color,
                    Description = PriorityType.Description,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    Remark = PriorityType.Remark
                };

                db.PriorityTypes.Add(entity);
                db.SaveChanges();
                PriorityType.PriorityTypeId = entity.PriorityTypeId;
            }

            return Json(new[] { PriorityType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriorityTypes_Update([DataSourceRequest]DataSourceRequest request, PriorityType PriorityType)
        {
           
            if (ModelState.IsValid)
            {
                var entity = new PriorityType
                {
                    PriorityTypeId = PriorityType.PriorityTypeId,
                    Name = PriorityType.Name,
                    Color = PriorityType.Color,
                    Description = PriorityType.Description,
                    Remark = PriorityType.Remark,
                };

                db.PriorityTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { PriorityType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PriorityTypes_Destroy([DataSourceRequest]DataSourceRequest request, PriorityType PriorityType)
        {
            if (ModelState.IsValid)
            {
                var entity = new PriorityType
                {
                    PriorityTypeId = PriorityType.PriorityTypeId,
                    Remark = PriorityType.Remark
                };

                db.PriorityTypes.Attach(entity);
                db.PriorityTypes.Remove(entity);
                db.SaveChanges();
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Priority Type number " + PriorityType.PriorityTypeId);

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
