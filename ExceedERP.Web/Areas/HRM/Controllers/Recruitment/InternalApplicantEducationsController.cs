﻿using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{
    public class InternalApplicantEducationsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult InternalApplicantEducations_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<InternalApplicantEducation> internalapplicanteducations = db.InternalApplicantEducations.Where(x=>x.InternalApplicantsId == id);

            DataSourceResult result = internalapplicanteducations.ToDataSourceResult(request, internalApplicantEducation => new {
                InternalApplicantEducationId = internalApplicantEducation.InternalApplicantEducationId,
                ApplicationFormId = internalApplicantEducation.ApplicationFormId,
                InternalApplicantsId = internalApplicantEducation.InternalApplicantsId,
                EducationLevelSettingId = internalApplicantEducation.EducationLevelSettingId,
                EducationFieldSettingId = internalApplicantEducation.EducationFieldSettingId,
                EducationInstitution = internalApplicantEducation.EducationInstitution,
                Remark = internalApplicantEducation.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantEducations_Create([DataSourceRequest]DataSourceRequest request, InternalApplicantEducation internalApplicantEducation,int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalApplicantEducation
                {
                    ApplicationFormId = internalApplicantEducation.ApplicationFormId,
                    InternalApplicantsId = id,
                    EducationLevelSettingId = internalApplicantEducation.EducationLevelSettingId,
                    EducationFieldSettingId = internalApplicantEducation.EducationFieldSettingId,
                    EducationInstitution = internalApplicantEducation.EducationInstitution,
                    Remark = internalApplicantEducation.Remark
                };

                db.InternalApplicantEducations.Add(entity);
                db.SaveChanges();
                internalApplicantEducation.InternalApplicantEducationId = entity.InternalApplicantEducationId;
            }

            return Json(new[] { internalApplicantEducation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantEducations_Update([DataSourceRequest]DataSourceRequest request, InternalApplicantEducation internalApplicantEducation)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalApplicantEducation
                {
                    InternalApplicantEducationId = internalApplicantEducation.InternalApplicantEducationId,
                    ApplicationFormId = internalApplicantEducation.ApplicationFormId,
                    InternalApplicantsId = internalApplicantEducation.InternalApplicantsId,
                    EducationLevelSettingId = internalApplicantEducation.EducationLevelSettingId,
                    EducationFieldSettingId = internalApplicantEducation.EducationFieldSettingId,
                    EducationInstitution = internalApplicantEducation.EducationInstitution,
                    Remark = internalApplicantEducation.Remark
                };

                db.InternalApplicantEducations.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { internalApplicantEducation }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantEducations_Destroy([DataSourceRequest]DataSourceRequest request, InternalApplicantEducation internalApplicantEducation)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalApplicantEducation
                {
                    InternalApplicantEducationId = internalApplicantEducation.InternalApplicantEducationId,
                    ApplicationFormId = internalApplicantEducation.ApplicationFormId,
                    InternalApplicantsId = internalApplicantEducation.InternalApplicantsId,
                    EducationLevelSettingId = internalApplicantEducation.EducationLevelSettingId,
                    EducationFieldSettingId = internalApplicantEducation.EducationFieldSettingId,
                    EducationInstitution = internalApplicantEducation.EducationInstitution,
                    Remark = internalApplicantEducation.Remark
                };

                db.InternalApplicantEducations.Attach(entity);
                db.InternalApplicantEducations.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { internalApplicantEducation }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
