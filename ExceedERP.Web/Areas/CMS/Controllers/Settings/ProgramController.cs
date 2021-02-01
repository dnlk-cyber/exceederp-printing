﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.CMS;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.CMS.Controllers
{
    public class ProgramController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CMSPrograms_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CMSProgram> cmsprograms = db.CMSPrograms;
            DataSourceResult result = cmsprograms.ToDataSourceResult(request, cMSProgram => new {
                CMSProgramId = cMSProgram.CMSProgramId,
                Name = cMSProgram.Name,
                Code = cMSProgram.Code,
                DateCreated = cMSProgram.DateCreated,
                LastModified = cMSProgram.LastModified,
                CreatedBy = cMSProgram.CreatedBy,
                ModifiedBy = cMSProgram.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CMSPrograms_Create([DataSourceRequest]DataSourceRequest request, CMSProgram cMSProgram)
        {
            if (ModelState.IsValid)
            {
                var entity = new CMSProgram
                {
                    Name = cMSProgram.Name,
                    Code = cMSProgram.Code,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.CMSPrograms.Add(entity);
                db.SaveChanges();
                cMSProgram.CMSProgramId = entity.CMSProgramId;
            }

            return Json(new[] { cMSProgram }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CMSPrograms_Update([DataSourceRequest]DataSourceRequest request, CMSProgram cMSProgram)
        {
            if (ModelState.IsValid)
            {
                var entity = new CMSProgram
                {
                    CMSProgramId = cMSProgram.CMSProgramId,
                    Name = cMSProgram.Name,
                    Code = cMSProgram.Code,

                    DateCreated = cMSProgram.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = cMSProgram.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.CMSPrograms.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { cMSProgram }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CMSPrograms_Destroy([DataSourceRequest]DataSourceRequest request, CMSProgram cMSProgram)
        {
            if (ModelState.IsValid)
            {
                var entity = new CMSProgram
                {
                    CMSProgramId = cMSProgram.CMSProgramId,
                    Name = cMSProgram.Name,
                    Code = cMSProgram.Code,

                    DateCreated = cMSProgram.DateCreated,
                    LastModified = cMSProgram.LastModified,
                    CreatedBy = cMSProgram.CreatedBy,
                    ModifiedBy = cMSProgram.ModifiedBy
                };

                db.CMSPrograms.Attach(entity);
                db.CMSPrograms.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cMSProgram }.ToDataSourceResult(request, ModelState));
        }

        [HttpPost]
        public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }
    
        [HttpPost]
        public ActionResult Pdf_Export_Save(string contentType, string base64, string fileName)
        {
            var fileContents = Convert.FromBase64String(base64);

            return File(fileContents, contentType, fileName);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
