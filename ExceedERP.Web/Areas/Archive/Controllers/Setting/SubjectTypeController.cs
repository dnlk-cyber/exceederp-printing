using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class SubjectTypeController : Controller
    {
        // GET: HRM/SubjectType
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SubjectType_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<SubjectType> zones = db.SubjectTypes;
            DataSourceResult result = zones.ToDataSourceResult(request, subjectType => new
            {
                SubjectTypeId = subjectType.SubjectTypeId,
                SubjectTypeName = subjectType.SubjectTypeName,
                SubjectCode = subjectType.SubjectCode,
                Remark = subjectType.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubjectType_Create([DataSourceRequest]DataSourceRequest request, SubjectType subjectType)
        {
            if (ModelState.IsValid)
            {

                var entity = new SubjectType
                {
                    SubjectTypeName = subjectType.SubjectTypeName,
                    SubjectCode = subjectType.SubjectCode,
                    Remark = subjectType.Remark
                };

                db.SubjectTypes.Add(entity);
                db.SaveChanges();


                #region operation log                             

                UserHelper.OperationLog(Request.UserHostAddress, "Subject Type", "Success", User.Identity.Name, User.Identity.Name + " add new Subject Type called " + entity.SubjectTypeId, Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { subjectType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubjectType_Update([DataSourceRequest]DataSourceRequest request, SubjectType subjectType)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubjectType
                {
                    SubjectTypeId = subjectType.SubjectTypeId,
                    SubjectTypeName = subjectType.SubjectTypeName,
                    SubjectCode = subjectType.SubjectCode,
                    Remark = subjectType.Remark
                };

                db.SubjectTypes.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


                #region operation log                             

                UserHelper.OperationLog(Request.UserHostAddress, "Subject Type", "Success", User.Identity.Name, User.Identity.Name + " update Subject Type called " + entity.SubjectTypeId, Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { subjectType }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubjectType_Destroy([DataSourceRequest]DataSourceRequest request, SubjectType subjectType)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubjectType
                {
                    SubjectTypeId = subjectType.SubjectTypeId,
                    SubjectTypeName = subjectType.SubjectTypeName,
                    SubjectCode = subjectType.SubjectCode,
                    Remark = subjectType.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted subject type information of " + entity.SubjectTypeId);
                #endregion
                db.SubjectTypes.Attach(entity);
                db.SubjectTypes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { subjectType }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
