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
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Recruitment
{

    [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
    public class ApplicantExperienceController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime From { get; set; }
        public static string FromAmharic { get; set; }
        public static DateTime To { get; set; }
        public static string ToAmharic { get; set; }
        public ActionResult Index()
        {
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "FromAmharic")
            {
                FromAmharic = value;
                var splitDate = FromAmharic.Split('/');
                From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "ToAmahric")
            {
                ToAmharic = value;
                var splitDate = ToAmharic.Split('/');
                To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }

        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult ApplicantExperience_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ApplicantExperience> applicantexperience = db.ApplicantExperience.Where(x => x.ApplicantRegistrationId == id);
            DataSourceResult result = applicantexperience.ToDataSourceResult(request, applicantExperience => new
            {
                ApplicantExpId = applicantExperience.ApplicantExpId,
                EduLevel = applicantExperience.EduLevel,
                EducField = applicantExperience.EducField,
                OrganizationName = applicantExperience.OrganizationName,
                ApplicantRegistrationId = applicantExperience.ApplicantRegistrationId,
                Postion = applicantExperience.Postion,
                Experience = applicantExperience.Experience,
                From = applicantExperience.From,
                To = applicantExperience.To,
                Remark = applicantExperience.Remark,
                FromAmharic = applicantExperience.FromAmharic,
                ToAmahric = applicantExperience.ToAmahric,
                //EducationInstitution = applicantExperience.EducationInstitution
            });

            return Json(result);
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExperience_Create([DataSourceRequest]DataSourceRequest request, ApplicantExperience applicantExperience, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (From.Year > To.Year)
                {
                    ModelState.AddModelError("", "Please check the date range, The year of experience should be non negative!!");
                }
                else if (FromAmharic == null || ToAmharic == null)
                {
                    ModelState.AddModelError("", "Both date values are mandatory!!");
                }
                else if (From.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
                }
                else if (To.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "End date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new ApplicantExperience
                    {
                        EduLevel = applicantExperience.EduLevel,
                        EducField = applicantExperience.EducField,
                        OrganizationName = applicantExperience.OrganizationName,
                        Postion = applicantExperience.Postion,
                        From = From,
                        To = To,
                        Experience = To.Year - From.Year,
                        ApplicantRegistrationId = id,
                        Remark = applicantExperience.Remark,
                        FromAmharic = FromAmharic,
                        ToAmahric = ToAmharic,

                    };

                    db.ApplicantExperience.Add(entity);
                    db.SaveChanges();
                    applicantExperience.ApplicantExpId = entity.ApplicantExpId;

                    #region operation log

                    var applicant = db.ApplicantRegistration.FirstOrDefault(x => x.ApplicantRegistrationId == entity.ApplicantRegistrationId);
                    if (applicant != null)
                    {
                            UserHelper.OperationLog(Request.UserHostAddress, "Applicant Experience ", "Success", User.Identity.Name, User.Identity.Name + " add new applicant Experience Info for " + applicant.ApplicantName + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                        
                    }

                    #endregion

                }
            }

            return Json(new[] { applicantExperience }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExperience_Update([DataSourceRequest]DataSourceRequest request, ApplicantExperience applicantExperience)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (FromAmharic == null)
                {
                    FromAmharic = applicantExperience.FromAmharic;
                    var splitDate = FromAmharic.Split('/');
                    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (ToAmharic == null)
                {
                    ToAmharic = applicantExperience.ToAmahric;
                    var splitDate = ToAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (From.Year > To.Year)
                {
                    ModelState.AddModelError("", "Please check the date range, The year of experience should be non negative!!");
                }
                else if (From.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Start date should be less or equal to the current date!!");
                }
                else if (To.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "End date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new ApplicantExperience
                    {
                        ApplicantExpId = applicantExperience.ApplicantExpId,
                        EduLevel = applicantExperience.EduLevel,
                        EducField = applicantExperience.EducField,
                        OrganizationName = applicantExperience.OrganizationName,
                        Postion = applicantExperience.Postion,
                        Experience = Convert.ToDecimal(To.Year - From.Year),
                        From = From,
                        To = To,
                        ApplicantRegistrationId = applicantExperience.ApplicantRegistrationId,
                        Remark = applicantExperience.Remark,
                        FromAmharic = FromAmharic,
                        ToAmahric = ToAmharic,
                        //EducationInstitution = applicantExperience.EducationInstitution
                    };

                    db.ApplicantExperience.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log

                    var applicant = db.ApplicantRegistration.FirstOrDefault(x => x.ApplicantRegistrationId == entity.ApplicantRegistrationId);
                    if (applicant != null)
                    {
                            UserHelper.OperationLog(Request.UserHostAddress, "Applicant Experience ", "Success", User.Identity.Name, User.Identity.Name + " update applicant Experience Info of " + applicant.ApplicantName + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                        
                    }

                    #endregion

                }
            }

            return Json(new[] { applicantExperience }.ToDataSourceResult(request, ModelState));
        }

        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExperience_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantExperience applicantExperience)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ApplicantExperience
                {
                    ApplicantExpId = applicantExperience.ApplicantExpId,
                    EduLevel = applicantExperience.EduLevel,
                    EducField = applicantExperience.EducField,
                    OrganizationName = applicantExperience.OrganizationName,
                    Postion = applicantExperience.Postion,
                    Experience = applicantExperience.Experience,
                    From = applicantExperience.From,
                    To = applicantExperience.To,
                    ApplicantRegistrationId = applicantExperience.ApplicantRegistrationId,
                    Remark = applicantExperience.Remark,
                    FromAmharic = applicantExperience.FromAmharic,
                    ToAmahric = applicantExperience.ToAmahric,
                    //EducationInstitution = applicantExperience.EducationInstitution
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
