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
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Archive.Controllers.Setting
{
    public class DepartmentOrSubjectTypeSelectionForFolderCreationController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult DepartmentOrSubjectTypeSelectionForFolderCreations_Read([DataSourceRequest]DataSourceRequest request)
        {
            string username = User.Identity.Name;
            int depId = UserInformationHelper.GetEmployeeDepartmentId(username);
            var selection = db.DepartmentOrSubjectTypeSelectionForFolderCreations.Where(x => x.Owner_OrgStructureId == depId).ToList();

            DataSourceResult result = selection.ToDataSourceResult(request, sele => new
            {
                DepartmentOrSubjectTypeSelectionForFolderCreationId = sele.DepartmentOrSubjectTypeSelectionForFolderCreationId,
                Owner_OrgStructureId = sele.Owner_OrgStructureId,
                DepartmentOrSubjectType = sele.DepartmentOrSubjectType,
                Remark = sele.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentOrSubjectTypeSelectionForFolderCreations_Create([DataSourceRequest]DataSourceRequest request, DepartmentOrSubjectTypeSelectionForFolderCreation department)
        {
            string username = User.Identity.Name;
            department.Owner_OrgStructureId = UserInformationHelper.GetEmployeeDepartmentId(username);
            var one = db.DepartmentOrSubjectTypeSelectionForFolderCreations.FirstOrDefault(x => x.Owner_OrgStructureId == department.Owner_OrgStructureId);
            if (one != null)
            {
                ModelState.AddModelError("", "you can't add this setting two times");
            }
            if (ModelState.IsValid)
            {
                var entity = new DepartmentOrSubjectTypeSelectionForFolderCreation
                {
                    Owner_OrgStructureId = department.Owner_OrgStructureId,
                    DepartmentOrSubjectType = department.DepartmentOrSubjectType,
                    Remark = department.Remark
                };

                db.DepartmentOrSubjectTypeSelectionForFolderCreations.Add(entity);
                db.SaveChanges();
                department.DepartmentOrSubjectTypeSelectionForFolderCreationId = entity.DepartmentOrSubjectTypeSelectionForFolderCreationId;
            }

            return Json(new[] { department }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentOrSubjectTypeSelectionForFolderCreations_Update([DataSourceRequest]DataSourceRequest request, DepartmentOrSubjectTypeSelectionForFolderCreation department)
        {
            var folder = db.DepartmentFolders.FirstOrDefault(x => x.Owner_OrgStructureId == department.Owner_OrgStructureId);
            if (folder != null)
            {
                ModelState.AddModelError("", "you can't edit this setting it is already applied to the folder");
            }
            if (ModelState.IsValid)
            {
                var entity = new DepartmentOrSubjectTypeSelectionForFolderCreation
                {
                    DepartmentOrSubjectTypeSelectionForFolderCreationId = department.DepartmentOrSubjectTypeSelectionForFolderCreationId,
                    Owner_OrgStructureId = department.Owner_OrgStructureId,
                    DepartmentOrSubjectType = department.DepartmentOrSubjectType,
                    Remark = department.Remark
                };

                db.DepartmentOrSubjectTypeSelectionForFolderCreations.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { department }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentOrSubjectTypeSelectionForFolderCreations_Destroy([DataSourceRequest]DataSourceRequest request, DepartmentOrSubjectTypeSelectionForFolderCreation department)
        {
            var folder = db.DepartmentFolders.FirstOrDefault(x => x.Owner_OrgStructureId == department.Owner_OrgStructureId);
            if (folder != null)
            {
                ModelState.AddModelError("", "you can't delete this setting it is already applied to the folder");
            }
            if (ModelState.IsValid)
            {
                var entity = new DepartmentOrSubjectTypeSelectionForFolderCreation
                {
                    DepartmentOrSubjectTypeSelectionForFolderCreationId = department.DepartmentOrSubjectTypeSelectionForFolderCreationId,
                    Owner_OrgStructureId = department.Owner_OrgStructureId,
                    DepartmentOrSubjectType = department.DepartmentOrSubjectType,
                    Remark = department.Remark
                };

                db.DepartmentOrSubjectTypeSelectionForFolderCreations.Attach(entity);
                db.DepartmentOrSubjectTypeSelectionForFolderCreations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { department }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
