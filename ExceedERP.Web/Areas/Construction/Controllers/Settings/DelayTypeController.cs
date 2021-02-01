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
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Construction.Controllers.Settings
{
    [AuthorizeRoles(ConstructionConstants.ConstructionSettingAdmin)]
    public class DelayTypeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/DelayType
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult DelayType_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DelayType> delayTypes = db.DelayTypes;
            DataSourceResult result = delayTypes.ToDataSourceResult(request, delayType => new
            {
                DelayTypeId = delayType.DelayTypeId,
                Type = delayType.Type,
                Description = delayType.Description,
                Abbriviation = delayType.Abbriviation,
            });

            return Json(result);
        }

        private bool CheckExistance(DelayType model)
        {
            var delayType = db.DelayTypes.Where(dt => dt.Type == model.Type || dt.Abbriviation == model.Abbriviation).FirstOrDefault();
            if (delayType != null)
            {
                return true;
            }
            return false;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DelayType_Create([DataSourceRequest]DataSourceRequest request, DelayType delayType)
        {
            if (ModelState.IsValid)
            {
                if (CheckExistance(delayType))
                {
                    ModelState.AddModelError("", "similar value is already registered!!");
                }
                else
                {
                    var entity = new DelayType
                    {
                        //DelayTypeId = delayType.DelayTypeId,
                        Description = delayType.Description,
                        Type = delayType.Type,
                        Abbriviation = delayType.Abbriviation,
                    };

                    db.DelayTypes.Add(entity);
                    db.SaveChanges();
                    delayType.DelayTypeId = entity.DelayTypeId;

                }
            }

            return Json(new[] { delayType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DelayType_Update([DataSourceRequest]DataSourceRequest request, DelayType delayType)
        {
            if (ModelState.IsValid)
            {
                var entity = new DelayType
                {
                    DelayTypeId = delayType.DelayTypeId,
                    Description = delayType.Description,
                    Type = delayType.Type,
                    Abbriviation = delayType.Abbriviation,
                };

                db.DelayTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { delayType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DelayType_Destroy([DataSourceRequest]DataSourceRequest request, DelayType delayType)
        {
            if (ModelState.IsValid)
            {
                var entity = new DelayType
                {
                    DelayTypeId = delayType.DelayTypeId,
                    Description = delayType.Description,
                    Type = delayType.Type,
                    Abbriviation = delayType.Abbriviation,
                };

                db.DelayTypes.Attach(entity);
                db.DelayTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { delayType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}