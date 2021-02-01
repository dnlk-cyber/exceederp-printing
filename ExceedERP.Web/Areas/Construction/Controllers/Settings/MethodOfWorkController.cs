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
    public class MethodOfWorkController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Construction/MethodOfWork
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult MethodOfWork_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<MethodOfWork> methods = db.MethodOfWork;
            DataSourceResult result = methods.ToDataSourceResult(request, method => new {
                MethodOfWorkId = method.MethodOfWorkId,
                Name = method.Name,
                Abbreviation = method.Abbreviation
                
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MethodOfWork_Create([DataSourceRequest]DataSourceRequest request, MethodOfWork method)
        {
            if (ModelState.IsValid)
            {
                if (this.ValueExists(method))
                {
                    ModelState.AddModelError("ValueExists", "Value Exists");
                }
                var entity = new MethodOfWork
                {
                    //MethodOfWorkId = method.MethodOfWorkId,
                    Name = method.Name,
                    Abbreviation = method.Abbreviation
                };

                db.MethodOfWork.Add(entity);
                db.SaveChanges();
                method.MethodOfWorkId = entity.MethodOfWorkId;
            }

            return Json(new[] { method }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MethodOfWork_Update([DataSourceRequest]DataSourceRequest request, MethodOfWork method)
        {
            if (ModelState.IsValid)
            {
                var entity = new MethodOfWork
                {
                    MethodOfWorkId = method.MethodOfWorkId,
                    Name = method.Name,
                    Abbreviation = method.Abbreviation
                };

                db.MethodOfWork.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { method }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult MethodOfWork_Destroy([DataSourceRequest]DataSourceRequest request, MethodOfWork method)
        {
            if (ModelState.IsValid)
            {
                var entity = new MethodOfWork
                {
                    MethodOfWorkId = method.MethodOfWorkId,
                    Name = method.Name,
                    Abbreviation = method.Abbreviation
                };

                db.MethodOfWork.Attach(entity);
                db.MethodOfWork.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { method }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        private bool ValueExists(MethodOfWork model)
        {
            bool valueExists = false;
            var list = db.MethodOfWork.ToList();
            foreach (var item in list)
            {
                if (item.Name == model.Name || item.Abbreviation == model.Abbreviation )
                {
                    valueExists = true;
                }
            }
            return valueExists;
        }
    }
}