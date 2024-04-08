﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
﻿using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.HRM.Controllers.Profile;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Areas.HRM.Controllers;

namespace ExceedERP.Web.Areas.Archive.Controllers
{

    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class PersonalFolderExplorerController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            var employees = db.Person_Employee.ToList();

            ViewData["EmployeeList"] = employees.Select(q => new
            {
                q.EmployeeId,
                FullNameaEnglish = q.EmployeeCode + "-" + q.FullNameaEnglish
            });

            ViewData["Branch"] = db.Branch.ToList();
            return View();
        }
        public ActionResult GetFolders()
        {
            var folders = db.PersonalArchiveNo.ToList();
            return Json(folders.Select(p => new
            {
                Folder = p.Folder
            }), JsonRequestBehavior.AllowGet);
        }        
        public ActionResult PersonalArchiveNo_Read([DataSourceRequest]DataSourceRequest request)
        {
            HRMInputs input = new HRMInputs();
            IQueryable<PersonalArchiveNo> personalarchiveno = db.PersonalArchiveNo;
            DataSourceResult result = personalarchiveno.ToDataSourceResult(request, personalArchiveNo => new {
                PersonalArchiveNoId = personalArchiveNo.PersonalArchiveNoId,
                Folder = personalArchiveNo.Folder,
                EmployeeId = personalArchiveNo.EmployeeId,
                EmployeeCode =input.GetEmployeeCode(personalArchiveNo.EmployeeId),
                ZoneId = personalArchiveNo.ZoneId,
                Shelve = personalArchiveNo.Shelve,
                Row = personalArchiveNo.Row,
                FileNo = personalArchiveNo.FileNo,
                Volume = personalArchiveNo.Volume,
                BranchId = personalArchiveNo.BranchId,
                Remark = personalArchiveNo.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PersonalArchiveNo_Create([DataSourceRequest]DataSourceRequest request, PersonalArchiveNo personalArchiveNo)
        {
            if (ModelState.IsValid)
            {
                var entity = new PersonalArchiveNo
                {
                    Folder = personalArchiveNo.Folder,
                    EmployeeId = personalArchiveNo.EmployeeId,
                    ZoneId = personalArchiveNo.ZoneId,
                    Shelve = personalArchiveNo.Shelve,
                    Row = personalArchiveNo.Row,
                    FileNo = personalArchiveNo.FileNo,
                    Volume = personalArchiveNo.Volume,
                    BranchId = personalArchiveNo.BranchId,
                    Remark = personalArchiveNo.Remark
                };

                db.PersonalArchiveNo.Add(entity);
                db.SaveChanges();
                personalArchiveNo.PersonalArchiveNoId = entity.PersonalArchiveNoId;
            }

            return Json(new[] { personalArchiveNo }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PersonalArchiveNo_Update([DataSourceRequest]DataSourceRequest request, PersonalArchiveNo personalArchiveNo)
        {
            if (ModelState.IsValid)
            {
                var entity = new PersonalArchiveNo
                {
                    PersonalArchiveNoId = personalArchiveNo.PersonalArchiveNoId,
                    Folder = personalArchiveNo.Folder,
                    EmployeeId = personalArchiveNo.EmployeeId,
                    ZoneId = personalArchiveNo.ZoneId,
                    Shelve = personalArchiveNo.Shelve,
                    Row = personalArchiveNo.Row,
                    FileNo = personalArchiveNo.FileNo,
                    Volume = personalArchiveNo.Volume,
                    BranchId = personalArchiveNo.BranchId,
                    Remark = personalArchiveNo.Remark
                };

                db.PersonalArchiveNo.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { personalArchiveNo }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult PersonalArchiveNo_Destroy([DataSourceRequest]DataSourceRequest request, PersonalArchiveNo personalArchiveNo)
        {
            if (ModelState.IsValid)
            {
                var entity = new PersonalArchiveNo
                {
                    PersonalArchiveNoId = personalArchiveNo.PersonalArchiveNoId,
                    Folder = personalArchiveNo.Folder,
                    EmployeeId = personalArchiveNo.EmployeeId,
                    ZoneId = personalArchiveNo.ZoneId,
                    Shelve = personalArchiveNo.Shelve,
                    Row = personalArchiveNo.Row,
                    FileNo = personalArchiveNo.FileNo,
                    Volume = personalArchiveNo.Volume,
                    BranchId = personalArchiveNo.BranchId,
                    Remark = personalArchiveNo.Remark
                };

                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Employee Folder Of" + personalArchiveNo.EmployeeCode);
                #endregion

                db.PersonalArchiveNo.Attach(entity);
                db.PersonalArchiveNo.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { personalArchiveNo }.ToDataSourceResult(request, ModelState));
        }
        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "Excel-Export", "Success", User.Identity.Name, "Exported Personal ArchiveNo");
            #endregion
            return File(fileContents, contentType, fileName);
        }
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);
            #region export log
            UserHelper.ExportLog(Request.UserHostAddress, "PDF-Export", "Success", User.Identity.Name, "Exported Personal ArchiveNo");
            #endregion
            return File(fileContents, contentType, fileName);
        }
      
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
