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
    public class InternalApplicantExperiencesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Lookups"] = db.Lookups.ToList();
            return View();
        }

        public ActionResult InternalApplicantExperiences_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<InternalApplicantExperience> internalapplicantexperiences = db.InternalApplicantExperiences.Where(x=>x.InternalApplicantsId == id);

            DataSourceResult result = internalapplicantexperiences.ToDataSourceResult(request, internalApplicantExperience => new {
                InternalApplicantExperienceId = internalApplicantExperience.InternalApplicantExperienceId,
                ApplicationFormId = internalApplicantExperience.ApplicationFormId,
                InternalApplicantsId = internalApplicantExperience.InternalApplicantsId,
                OrganizationType = internalApplicantExperience.OrganizationType,
                JobTitleId = internalApplicantExperience.JobTitleId,
                Jobtitle = internalApplicantExperience.Jobtitle,
                Year = internalApplicantExperience.Year,
                OrganizationName = internalApplicantExperience.OrganizationName,
                From = internalApplicantExperience.From,
                To = internalApplicantExperience.To,
                Remark = internalApplicantExperience.Remark
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantExperiences_Create([DataSourceRequest]DataSourceRequest request, InternalApplicantExperience internalApplicantExperience, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalApplicantExperience
                {
                    ApplicationFormId = internalApplicantExperience.ApplicationFormId,
                    InternalApplicantsId = id,
                    OrganizationType = internalApplicantExperience.OrganizationType,
                    JobTitleId = internalApplicantExperience.JobTitleId,
                    Jobtitle = internalApplicantExperience.Jobtitle,
                    Year = internalApplicantExperience.Year,
                    OrganizationName = internalApplicantExperience.OrganizationName,
                    From = internalApplicantExperience.From,
                    To = internalApplicantExperience.To,
                    Remark = internalApplicantExperience.Remark
                };

                db.InternalApplicantExperiences.Add(entity);
                db.SaveChanges();
                internalApplicantExperience.InternalApplicantExperienceId = entity.InternalApplicantExperienceId;
            }

            return Json(new[] { internalApplicantExperience }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantExperiences_Update([DataSourceRequest]DataSourceRequest request, InternalApplicantExperience internalApplicantExperience)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalApplicantExperience
                {
                    InternalApplicantExperienceId = internalApplicantExperience.InternalApplicantExperienceId,
                    ApplicationFormId = internalApplicantExperience.ApplicationFormId,
                    InternalApplicantsId = internalApplicantExperience.InternalApplicantsId,
                    OrganizationType = internalApplicantExperience.OrganizationType,
                    JobTitleId = internalApplicantExperience.JobTitleId,
                    Jobtitle = internalApplicantExperience.Jobtitle,
                    Year = internalApplicantExperience.Year,
                    OrganizationName = internalApplicantExperience.OrganizationName,
                    From = internalApplicantExperience.From,
                    To = internalApplicantExperience.To,
                    Remark = internalApplicantExperience.Remark
                };

                db.InternalApplicantExperiences.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { internalApplicantExperience }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InternalApplicantExperiences_Destroy([DataSourceRequest]DataSourceRequest request, InternalApplicantExperience internalApplicantExperience)
        {
            if (ModelState.IsValid)
            {
                var entity = new InternalApplicantExperience
                {
                    InternalApplicantExperienceId = internalApplicantExperience.InternalApplicantExperienceId,
                    ApplicationFormId = internalApplicantExperience.ApplicationFormId,
                    InternalApplicantsId = internalApplicantExperience.InternalApplicantsId,
                    OrganizationType = internalApplicantExperience.OrganizationType,
                    JobTitleId = internalApplicantExperience.JobTitleId,
                    Jobtitle = internalApplicantExperience.Jobtitle,
                    Year = internalApplicantExperience.Year,
                    OrganizationName = internalApplicantExperience.OrganizationName,
                    From = internalApplicantExperience.From,
                    To = internalApplicantExperience.To,
                    Remark = internalApplicantExperience.Remark
                };

                db.InternalApplicantExperiences.Attach(entity);
                db.InternalApplicantExperiences.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { internalApplicantExperience }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
