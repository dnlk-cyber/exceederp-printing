﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Archive.Controllers.DepartmentToDepartment
{
    public class DepartmentFoldersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Departments"] = db.MainCompanyStructures.ToList();
            ViewData["DepSubjectType"] = db.SubjectTypeForDepartmentLevels.ToList();
            return View();
        }

        public ActionResult DepartmentFolders_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<DepartmentFolder> departmentfolders = db.DepartmentFolders;
            DataSourceResult result = departmentfolders.ToDataSourceResult(request, departmentFolder => new {
                DepartmentFolderId = departmentFolder.DepartmentFolderId,
                OrgStructureId = departmentFolder.OrgStructureId,
                Department_SubjectTypeId = departmentFolder.Department_SubjectTypeId,
                Owner_OrgStructureId = departmentFolder.Owner_OrgStructureId,
                Room = departmentFolder.Room,
                Shelve = departmentFolder.Shelve,
                Row = departmentFolder.Row,
                Folder = departmentFolder.Folder,
                Volume = departmentFolder.Volume,
                FileNo = departmentFolder.FileNo
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentFolders_Create([DataSourceRequest]DataSourceRequest request, DepartmentFolder departmentFolder)
        {
            string username = User.Identity.Name;
            departmentFolder.Owner_OrgStructureId = UserInformationHelper.GetEmployeeDepartmentId(username);
            if (ModelState.IsValid)
            {
                var entity = new DepartmentFolder
                {
                    OrgStructureId = departmentFolder.OrgStructureId,
                    Owner_OrgStructureId = departmentFolder.Owner_OrgStructureId,
                    Department_SubjectTypeId = departmentFolder.Department_SubjectTypeId,
                    Room = departmentFolder.Room,
                    Shelve = departmentFolder.Shelve,
                    Row = departmentFolder.Row,
                    Folder = departmentFolder.Folder,
                    Volume = departmentFolder.Volume,
                    FileNo = departmentFolder.FileNo
                };

                db.DepartmentFolders.Add(entity);
                db.SaveChanges();
                departmentFolder.DepartmentFolderId = entity.DepartmentFolderId;
            }

            return Json(new[] { departmentFolder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentFolders_Update([DataSourceRequest]DataSourceRequest request, DepartmentFolder departmentFolder)
        {
            if (ModelState.IsValid)
            {
                var entity = new DepartmentFolder
                {
                    DepartmentFolderId = departmentFolder.DepartmentFolderId,
                    OrgStructureId = departmentFolder.OrgStructureId,
                    Owner_OrgStructureId = departmentFolder.Owner_OrgStructureId,
                    Department_SubjectTypeId = departmentFolder.Department_SubjectTypeId,
                    Room = departmentFolder.Room,
                    Shelve = departmentFolder.Shelve,
                    Row = departmentFolder.Row,
                    Folder = departmentFolder.Folder,
                    Volume = departmentFolder.Volume,
                    FileNo = departmentFolder.FileNo
                };

                db.DepartmentFolders.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { departmentFolder }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult DepartmentFolders_Destroy([DataSourceRequest]DataSourceRequest request, DepartmentFolder departmentFolder)
        {
            if (ModelState.IsValid)
            {
                var entity = new DepartmentFolder
                {
                    DepartmentFolderId = departmentFolder.DepartmentFolderId,
                    OrgStructureId = departmentFolder.OrgStructureId,
                    Owner_OrgStructureId = departmentFolder.Owner_OrgStructureId,
                    Department_SubjectTypeId = departmentFolder.Department_SubjectTypeId,
                    Room = departmentFolder.Room,
                    Shelve = departmentFolder.Shelve,
                    Row = departmentFolder.Row,
                    Folder = departmentFolder.Folder,
                    Volume = departmentFolder.Volume,
                    FileNo = departmentFolder.FileNo
                };

                db.DepartmentFolders.Attach(entity);
                db.DepartmentFolders.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { departmentFolder }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
