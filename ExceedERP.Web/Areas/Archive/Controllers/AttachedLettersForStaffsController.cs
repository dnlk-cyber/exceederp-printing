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

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class AttachedLettersForStaffsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AttachedLettersForStaffs_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
           var attachedlettersforstaffs = db.AttachedLettersForStaffs.Where(x => x.LetterForMultipleStaffId == id).ToList();

            DataSourceResult result = attachedlettersforstaffs.ToDataSourceResult(request, attachedLettersForStaff => new {
                AttachedLettersForStaffId = attachedLettersForStaff.AttachedLettersForStaffId,
                LetterForMultipleStaffId = attachedLettersForStaff.LetterForMultipleStaffId,
                FilePath = attachedLettersForStaff.FilePath,
                FileName = attachedLettersForStaff.FileName,
                ShortPath = attachedLettersForStaff.ShortPath,
                FileExtension = attachedLettersForStaff.FileExtension,
                Description = attachedLettersForStaff.Description,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttachedLettersForStaffs_Create([DataSourceRequest]DataSourceRequest request, AttachedLettersForStaff attachedLettersForStaff, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new AttachedLettersForStaff
                {
                    LetterForMultipleStaffId = id,
                    FilePath = attachedLettersForStaff.FilePath,
                    FileName = attachedLettersForStaff.FileName,
                    Description = attachedLettersForStaff.Description
                };

                db.AttachedLettersForStaffs.Add(entity);
                db.SaveChanges();
                attachedLettersForStaff.AttachedLettersForStaffId = entity.AttachedLettersForStaffId;
            }

            return Json(new[] { attachedLettersForStaff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttachedLettersForStaffs_Update([DataSourceRequest]DataSourceRequest request, AttachedLettersForStaff attachedLettersForStaff)
        {
            if (ModelState.IsValid)
            {
                var entity = new AttachedLettersForStaff
                {
                    AttachedLettersForStaffId = attachedLettersForStaff.AttachedLettersForStaffId,
                    LetterForMultipleStaffId = attachedLettersForStaff.LetterForMultipleStaffId,
                    FilePath = attachedLettersForStaff.FilePath,
                    FileName = attachedLettersForStaff.FileName,
                    Description = attachedLettersForStaff.Description
                };

                db.AttachedLettersForStaffs.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attachedLettersForStaff }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult AttachedLettersForStaffs_Destroy([DataSourceRequest]DataSourceRequest request, AttachedLettersForStaff attachedLettersForStaff)
        {
            if (ModelState.IsValid)
            {
                var entity = new AttachedLettersForStaff
                {
                    AttachedLettersForStaffId = attachedLettersForStaff.AttachedLettersForStaffId,
                    LetterForMultipleStaffId = attachedLettersForStaff.LetterForMultipleStaffId,
                    FilePath = attachedLettersForStaff.FilePath,
                    FileName = attachedLettersForStaff.FileName,
                    Description = attachedLettersForStaff.Description
                };

                db.AttachedLettersForStaffs.Attach(entity);
                db.AttachedLettersForStaffs.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachedLettersForStaff }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
