using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers.Settings
{
    public class MechanicTypeController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        // GET: Maintenance/MechanicType
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult MechanicType_Read([DataSourceRequest] DataSourceRequest request)
        {
            var mechanicTypes = db.MechanicTypeClasses;
            var result = mechanicTypes.ToDataSourceResult(request, type => new MechanicTypeClass
            {
                MechanicTypeTitle = type.MechanicTypeTitle,
                Description = type.Description,
                MechanicTypeClassId = type.MechanicTypeClassId
            });
            return Json(result);
        }
        public ActionResult MechanicType_Create([DataSourceRequest] DataSourceRequest request, MechanicTypeClass model)
        {
            if(ModelState.IsValid)
            {
                var entity = new MechanicTypeClass
                {
                    MechanicTypeTitle = model.MechanicTypeTitle,
                    Description = model.Description
                };
                db.MechanicTypeClasses.Add(entity);
                db.SaveChanges();
                model.MechanicTypeClassId = entity.MechanicTypeClassId; 
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult MechanicType_Update([DataSourceRequest] DataSourceRequest request, MechanicTypeClass model)
        {
            if (ModelState.IsValid)
            {

                db.MechanicTypeClasses.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
                
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult MechanicType_Delete([DataSourceRequest] DataSourceRequest request, MechanicTypeClass model)
        {
            if (ModelState.IsValid)
            {
                var mechanicType = db.MechanicTypeClasses.Find(model.MechanicTypeClassId);
                db.MechanicTypeClasses.Remove(mechanicType);
                db.SaveChanges();
                
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}