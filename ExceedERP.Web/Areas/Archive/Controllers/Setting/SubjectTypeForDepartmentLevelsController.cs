using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement.Setting;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Archive.Controllers.Setting
{
    public class SubjectTypeForDepartmentLevelsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult SubjectTypeForDepartmentLevels_Read([DataSourceRequest]DataSourceRequest request)
        {
            string username = User.Identity.Name;
            int depId = UserInformationHelper.GetEmployeeDepartmentId(username);
            var subjecttypefordepartmentlevels = db.SubjectTypeForDepartmentLevels.Where(x => x.Owner_OrgStructureId == depId).ToList();

            DataSourceResult result = subjecttypefordepartmentlevels.ToDataSourceResult(request, subjectTypeForDepartmentLevel => new
            {
                SubjectTypeForDepartmentLevelId = subjectTypeForDepartmentLevel.SubjectTypeForDepartmentLevelId,
                Owner_OrgStructureId = subjectTypeForDepartmentLevel.Owner_OrgStructureId,
                Name = subjectTypeForDepartmentLevel.Name,
                Remark = subjectTypeForDepartmentLevel.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubjectTypeForDepartmentLevels_Create([DataSourceRequest]DataSourceRequest request, SubjectTypeForDepartmentLevel subjectTypeForDepartmentLevel)
        {
            string username = User.Identity.Name;
            subjectTypeForDepartmentLevel.Owner_OrgStructureId = UserInformationHelper.GetEmployeeDepartmentId(username);
            if (ModelState.IsValid)
            {
                var entity = new SubjectTypeForDepartmentLevel
                {
                    Owner_OrgStructureId = subjectTypeForDepartmentLevel.Owner_OrgStructureId,
                    Name = subjectTypeForDepartmentLevel.Name,
                    Remark = subjectTypeForDepartmentLevel.Remark
                };

                db.SubjectTypeForDepartmentLevels.Add(entity);
                db.SaveChanges();
                subjectTypeForDepartmentLevel.SubjectTypeForDepartmentLevelId = entity.SubjectTypeForDepartmentLevelId;
            }

            return Json(new[] { subjectTypeForDepartmentLevel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubjectTypeForDepartmentLevels_Update([DataSourceRequest]DataSourceRequest request, SubjectTypeForDepartmentLevel subjectTypeForDepartmentLevel)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubjectTypeForDepartmentLevel
                {
                    SubjectTypeForDepartmentLevelId = subjectTypeForDepartmentLevel.SubjectTypeForDepartmentLevelId,
                    Owner_OrgStructureId = subjectTypeForDepartmentLevel.Owner_OrgStructureId,
                    Name = subjectTypeForDepartmentLevel.Name,
                    Remark = subjectTypeForDepartmentLevel.Remark
                };

                db.SubjectTypeForDepartmentLevels.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { subjectTypeForDepartmentLevel }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SubjectTypeForDepartmentLevels_Destroy([DataSourceRequest]DataSourceRequest request, SubjectTypeForDepartmentLevel subjectTypeForDepartmentLevel)
        {
            if (ModelState.IsValid)
            {
                var entity = new SubjectTypeForDepartmentLevel
                {
                    SubjectTypeForDepartmentLevelId = subjectTypeForDepartmentLevel.SubjectTypeForDepartmentLevelId,
                    Owner_OrgStructureId = subjectTypeForDepartmentLevel.Owner_OrgStructureId,
                    Name = subjectTypeForDepartmentLevel.Name,
                    Remark = subjectTypeForDepartmentLevel.Remark
                };

                db.SubjectTypeForDepartmentLevels.Attach(entity);
                db.SubjectTypeForDepartmentLevels.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { subjectTypeForDepartmentLevel }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
