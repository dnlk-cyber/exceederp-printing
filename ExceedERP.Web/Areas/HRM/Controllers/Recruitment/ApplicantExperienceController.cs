using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.RecruitmentManagment;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    public class ApplicantExperienceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public string GetTotalExperience(DateTime? from, DateTime? to)
        {
            if (from != null && to != null)
            {
                return XAPI.Common.getDaystoWords((to - from).Value.Days);
            }
            return null;
        }
        public ActionResult ApplicantExperience_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ApplicantExperience> applicantexperience = db.ApplicantExperience.Where(x => x.ApplicantDetailId == id);
            DataSourceResult result = applicantexperience.ToDataSourceResult(request, applicantExperience => new
            {
                ApplicantExpId = applicantExperience.ApplicantExpId,
                OrganizationName = applicantExperience.OrganizationName,
                ApplicantDetailId = applicantExperience.ApplicantDetailId,
                ApplicantRegistrationId = applicantExperience.ApplicantRegistrationId,
                OrganizationType = applicantExperience.OrganizationType,
                JobTitleId = applicantExperience.JobTitleId,
                Jobtitle = applicantExperience.Jobtitle,
                From = applicantExperience.From,
                To = applicantExperience.To,
                FromAm = applicantExperience.FromAm,
                ToAm = applicantExperience.ToAm,
                Remark = applicantExperience.Remark,
                TotalExperience = GetTotalExperience(applicantExperience.From, applicantExperience.To)
                //EducationInstitution = applicantExperience.EducationInstitution
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExperience_Create([DataSourceRequest]DataSourceRequest request, ApplicantExperience applicantExperience, int id)
        {
            ModelState.Clear();

            if (applicantExperience.FromAm != null)
            {
                var splitDate = applicantExperience.FromAm.Split('/');
                applicantExperience.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (applicantExperience.ToAm != null)
            {
                var splitDate = applicantExperience.ToAm.Split('/');
                applicantExperience.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            if (ModelState.IsValid)
            {
                var entity = new ApplicantExperience
                {
                    OrganizationName = applicantExperience.OrganizationName,
                    ApplicantDetailId = id,
                    OrganizationType = applicantExperience.OrganizationType,
                    JobTitleId = applicantExperience.JobTitleId,
                    From = applicantExperience.From,
                    To = applicantExperience.To,
                    FromAm = applicantExperience.FromAm,
                    ToAm = applicantExperience.ToAm,
                    Remark = applicantExperience.Remark,

                };

                db.ApplicantExperience.Add(entity);
                db.SaveChanges();
                applicantExperience.ApplicantExpId = entity.ApplicantExpId;
            }

            return Json(new[] { applicantExperience }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExperience_Update([DataSourceRequest]DataSourceRequest request, ApplicantExperience applicantExperience)
        {
            ModelState.Clear();

            if (applicantExperience.FromAm != null)
            {
                var splitDate = applicantExperience.FromAm.Split('/');
                applicantExperience.From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            if (applicantExperience.ToAm != null)
            {
                var splitDate = applicantExperience.ToAm.Split('/');
                applicantExperience.To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

            if (ModelState.IsValid)
            {
                var entity = new ApplicantExperience
                {
                    ApplicantExpId = applicantExperience.ApplicantExpId,
                    ApplicantDetailId = applicantExperience.ApplicantDetailId,
                    OrganizationName = applicantExperience.OrganizationName,
                    ApplicantRegistrationId = applicantExperience.ApplicantRegistrationId,
                    OrganizationType = applicantExperience.OrganizationType,
                    JobTitleId = applicantExperience.JobTitleId,
                    From = applicantExperience.From,
                    To = applicantExperience.To,
                    FromAm = applicantExperience.FromAm,
                    ToAm = applicantExperience.ToAm,
                    Remark = applicantExperience.Remark,
                };

                db.ApplicantExperience.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

            }

            return Json(new[] { applicantExperience }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExperience_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantExperience applicantExperience)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ApplicantExperience
                {
                    ApplicantExpId = applicantExperience.ApplicantExpId,
                    ApplicantDetailId = applicantExperience.ApplicantDetailId,
                    OrganizationName = applicantExperience.OrganizationName,
                    ApplicantRegistrationId = applicantExperience.ApplicantRegistrationId,
                    OrganizationType = applicantExperience.OrganizationType,
                    JobTitleId = applicantExperience.JobTitleId,
                    From = applicantExperience.From,
                    To = applicantExperience.To,
                    FromAm = applicantExperience.FromAm,
                    ToAm = applicantExperience.ToAm,
                    Remark = applicantExperience.Remark,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted applicant experience  information of applicant" + applicantExperience.ApplicantRegistrationId);
                #endregion
                db.ApplicantExperience.Attach(entity);
                db.ApplicantExperience.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { applicantExperience }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
