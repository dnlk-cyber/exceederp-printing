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
    public class CourseController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult CMSCourses_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<CMSCourse> cmscourses = db.CMSCourses;
            DataSourceResult result = cmscourses.ToDataSourceResult(request, cMSCourse => new {
                CMSCourseId = cMSCourse.CMSCourseId,
                Name = cMSCourse.Name,
                Code = cMSCourse.Code,
                CreditHours = cMSCourse.CreditHours,

                DateCreated = cMSCourse.DateCreated,
                LastModified = cMSCourse.LastModified,
                CreatedBy = cMSCourse.CreatedBy,
                ModifiedBy = cMSCourse.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CMSCourses_Create([DataSourceRequest]DataSourceRequest request, CMSCourse cMSCourse)
        {
            if (ModelState.IsValid)
            {
                var entity = new CMSCourse
                {
                    Name = cMSCourse.Name,
                    Code = cMSCourse.Code,
                    CreditHours = cMSCourse.CreditHours,

                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                };

                db.CMSCourses.Add(entity);
                db.SaveChanges();
                cMSCourse.CMSCourseId = entity.CMSCourseId;
            }

            return Json(new[] { cMSCourse }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CMSCourses_Update([DataSourceRequest]DataSourceRequest request, CMSCourse cMSCourse)
        {
            if (ModelState.IsValid)
            {
                var entity = new CMSCourse
                {
                    CMSCourseId = cMSCourse.CMSCourseId,
                    Name = cMSCourse.Name,
                    Code = cMSCourse.Code,
                    CreditHours = cMSCourse.CreditHours,

                    DateCreated = cMSCourse.DateCreated,
                    LastModified = DateTime.Now,
                    CreatedBy = cMSCourse.CreatedBy,
                    ModifiedBy = User.Identity.Name
                };

                db.CMSCourses.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { cMSCourse }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CMSCourses_Destroy([DataSourceRequest]DataSourceRequest request, CMSCourse cMSCourse)
        {
            if (ModelState.IsValid)
            {
                var entity = new CMSCourse
                {
                    CMSCourseId = cMSCourse.CMSCourseId,
                    Name = cMSCourse.Name,
                    Code = cMSCourse.Code,
                    CreditHours = cMSCourse.CreditHours,

                    DateCreated = cMSCourse.DateCreated,
                    LastModified = cMSCourse.LastModified,
                    CreatedBy = cMSCourse.CreatedBy,
                    ModifiedBy = cMSCourse.ModifiedBy
                };

                db.CMSCourses.Attach(entity);
                db.CMSCourses.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { cMSCourse }.ToDataSourceResult(request, ModelState));
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
