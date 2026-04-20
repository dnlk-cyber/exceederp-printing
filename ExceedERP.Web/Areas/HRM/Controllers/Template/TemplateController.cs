using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.HRM.Templates;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Controllers;
using System.Net;
using PagedList;

namespace ExceedERP.Web.Areas.HRM.Controllers.Template
{
    public class TemplateController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        // GET: HRM/Templates
        public ActionResult Index(int? page)
        {
            int pageSize = 2;
            int pageNumber = (page ?? 1);

            var templates = db.Templates.OrderBy(t => t.TemplateId).ToPagedList(pageNumber, pageSize);
            return View(templates);
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var template = db.Templates.Find(id);
            if (template == null) return HttpNotFound();
            return View(template);
        }

        public ActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TemplateId,TempOne,TemplateType,Document")] Core.Domain.HRM.Templates.Template template)
        {
            if (ModelState.IsValid)
            {
                db.Templates.Add(template);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(template);
        }

        public ActionResult Edit(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var template = db.Templates.Find(id);
            if (template == null) return HttpNotFound();
            return View(template);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TemplateId,TempOne,TemplateType,Document")] Core.Domain.HRM.Templates.Template template)
        {
            if (ModelState.IsValid)
            {
                db.Entry(template).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(template);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var template = db.Templates.Find(id);
            if (template == null) return HttpNotFound();
            return View(template);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var template = db.Templates.Find(id);
            db.Templates.Remove(template);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
    }
}

//namespace ExceedERP.Web.Areas.HRM.Controllers.Template
//{

//    //public class TemplateController : BaseController
//    //{
//    //    ExceedDbContext db = new ExceedDbContext();
//    //    GET: HRM/Template
//    //    public ActionResult Index()
//    //    {
//    //        return View();
//    //    }
//    //    public ActionResult Template_Read([DataSourceRequest]DataSourceRequest request)
//    //    {
//    //        var templates = db.Templates.ToList();
//    //        DataSourceResult result = templates.ToDataSourceResult(request, template => new
//    //        {
//    //            TemplateId = template.TemplateId,
//    //            TempOne = template.TempOne,
//    //            TemplateType = template.TemplateType
//    //        });

//    //        return Json(result);
//    //    }

//    //    [AcceptVerbs(HttpVerbs.Post)]
//    //    public ActionResult Template_Create([DataSourceRequest]DataSourceRequest request, ExceedERP.Core.Domain.HRM.Templates.Template template)
//    //    {

//    //        if (ModelState.IsValid)
//    //        {
//    //            var entity = new ExceedERP.Core.Domain.HRM.Templates.Template
//    //            {
//    //                TempOne = template.TempOne,
//    //                TemplateType = template.TemplateType
//    //            };

//    //            db.Templates.Add(entity);
//    //            db.SaveChanges();
//    //            template.TemplateId = entity.TemplateId;

//    //            #region operation log

//    //            UserHelper.OperationLog(Request.UserHostAddress, "Template", "Success", User.Identity.Name, User.Identity.Name + " add new Template called " + entity.TempOne, Modules.HRM);

//    //            #endregion

//    //        }

//    //        return Json(new[] { template }.ToDataSourceResult(request, ModelState));
//    //    }

//    //    [AcceptVerbs(HttpVerbs.Post)]
//    //    public ActionResult Template_Update([DataSourceRequest]DataSourceRequest request, ExceedERP.Core.Domain.HRM.Templates.Template template)
//    //    {
//    //        if (ModelState.IsValid)
//    //        {


//    //            var entity = new ExceedERP.Core.Domain.HRM.Templates.Template
//    //            {
//    //                TemplateId = template.TemplateId,
//    //                TempOne = template.TempOne,
//    //                TemplateType = template.TemplateType
//    //            };


//    //            db.Templates.Attach(entity);
//    //            db.Entry(entity).State = EntityState.Modified;
//    //            db.SaveChanges();

//    //            #region operation log

//    //            UserHelper.OperationLog(Request.UserHostAddress, "Template", "Success", User.Identity.Name, User.Identity.Name + " update the Template " + entity.TempOne, Modules.HRM);

//    //            #endregion
//    //        }

//    //        return Json(new[] { template }.ToDataSourceResult(request, ModelState));
//    //    }

//    //    [AcceptVerbs(HttpVerbs.Post)]
//    //    public ActionResult Template_Destroy([DataSourceRequest]DataSourceRequest request, ExceedERP.Core.Domain.HRM.Templates.Template template)
//    //    {
//    //        if (ModelState.IsValid)
//    //        {

//    //            var entity = new ExceedERP.Core.Domain.HRM.Templates.Template
//    //            {
//    //                TemplateId = template.TemplateId,
//    //                TempOne = template.TempOne,
//    //                TemplateType = template.TemplateType
//    //            };

//    //            #region delete log
//    //            UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment" + template.TemplateType);
//    //            #endregion

//    //            db.Templates.Attach(entity);
//    //            db.Templates.Remove(entity);
//    //            db.SaveChanges();
//    //        }

//    //        return Json(new[] { template }.ToDataSourceResult(request, ModelState));
//    //    }

//    //    protected override void Dispose(bool disposing)
//    //    {
//    //        db.Dispose();
//    //        base.Dispose(disposing);
//    //    }
//    //}
//}