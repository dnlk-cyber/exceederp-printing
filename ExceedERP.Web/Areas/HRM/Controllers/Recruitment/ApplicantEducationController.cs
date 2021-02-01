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
    public class ApplicantEducationController : BaseController
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
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRRecruitmentUser, HrmConstants.HRAdmin)]
        public ActionResult ApplicantExperience_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ApplicantEducation> applicantexperience = db.ApplicantEducation.Where(x => x.ApplicantRegistrationId == id);
            DataSourceResult result = applicantexperience.ToDataSourceResult(request, applicantExperience => new
            {
                ApplicantEducationId = applicantExperience.ApplicantEducationId,
                EduLevel = applicantExperience.EduLevel,
                EducField = applicantExperience.EducField,

                ApplicantRegistrationId = applicantExperience.ApplicantRegistrationId,
                // From = applicantExperience.From,
                To = applicantExperience.To,
                Remark = applicantExperience.Remark,
                //  FromAmharic = applicantExperience.FromAmharic,
                ToAmahric = applicantExperience.ToAmahric,
                EducationInstitution = applicantExperience.EducationInstitution
            });

            return Json(result);
        }
     
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExperience_Create([DataSourceRequest]DataSourceRequest request, ApplicantEducation applicantExperience, int id)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (ToAmharic == null)
                { ModelState.AddModelError("", "Date to value is mandatory!!"); }
                else if (To.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "End date should be less or equal to the current date!!");
                }
                else
                {


                    var entity = new ApplicantEducation
                    {
                        EduLevel = applicantExperience.EduLevel,
                        EducField = applicantExperience.EducField,
                        // From = From,
                        To = To,

                        ApplicantRegistrationId = id,
                        Remark = applicantExperience.Remark,
                        // FromAmharic = FromAmharic,
                        ToAmahric = ToAmharic,
                        EducationInstitution = applicantExperience.EducationInstitution
                    };

                    db.ApplicantEducation.Add(entity);
                    db.SaveChanges();
                    applicantExperience.ApplicantEducationId = entity.ApplicantEducationId;

                    #region operation log

                    var applicant = db.ApplicantRegistration.FirstOrDefault(x=>x.ApplicantRegistrationId == entity.ApplicantRegistrationId);
                    if (applicant != null)
                    {
                            UserHelper.OperationLog(Request.UserHostAddress, "Applicant Education ", "Success", User.Identity.Name, User.Identity.Name + " add new applicant education for " + applicant.ApplicantName + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                        
                    }

                    #endregion

                }
            }

            return Json(new[] { applicantExperience }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExperience_Update([DataSourceRequest]DataSourceRequest request, ApplicantEducation applicantExperience)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                //if (FromAmharic == null)
                //{
                //    FromAmharic = applicantExperience.FromAmharic;
                //    var splitDate = FromAmharic.Split('/');
                //    From = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                //}
                if (ToAmharic == null)
                {
                    ToAmharic = applicantExperience.ToAmahric;
                    var splitDate = ToAmharic.Split('/');
                    To = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (To.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "End date should be less or equal to the current date!!");
                }
                else
                {
                    var entity = new ApplicantEducation
                    {
                        ApplicantEducationId = applicantExperience.ApplicantEducationId,
                        EduLevel = applicantExperience.EduLevel,
                        EducField = applicantExperience.EducField,


                        //   From = From,
                        To = To,
                        ApplicantRegistrationId = applicantExperience.ApplicantRegistrationId,
                        Remark = applicantExperience.Remark,
                        //  FromAmharic = FromAmharic,
                        ToAmahric = ToAmharic,
                        EducationInstitution = applicantExperience.EducationInstitution
                    };

                    db.ApplicantEducation.Attach(entity);
                    db.Entry(entity).State = EntityState.Modified;
                    db.SaveChanges();

                    #region operation log

                    var applicant = db.ApplicantRegistration.FirstOrDefault(x => x.ApplicantRegistrationId == entity.ApplicantRegistrationId);
                    if (applicant != null)
                    {
                            UserHelper.OperationLog(Request.UserHostAddress, "Applicant Education ", "Success", User.Identity.Name, User.Identity.Name + " updated applicant education info of " + applicant.ApplicantName + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                        
                    }

                    #endregion

                }
            }

            return Json(new[] { applicantExperience }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantExperience_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantEducation applicantExperience)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new ApplicantEducation
                {
                    ApplicantEducationId = applicantExperience.ApplicantEducationId,
                    EduLevel = applicantExperience.EduLevel,
                    EducField = applicantExperience.EducField,
                    //  From = applicantExperience.From,
                    To = applicantExperience.To,
                    ApplicantRegistrationId = applicantExperience.ApplicantRegistrationId,
                    Remark = applicantExperience.Remark,
                    // FromAmharic = applicantExperience.FromAmharic,
                    ToAmahric = applicantExperience.ToAmahric,
                    EducationInstitution = applicantExperience.EducationInstitution
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted applicant educational information of applicant" + applicantExperience.ApplicantRegistrationId);
                #endregion
                db.ApplicantEducation.Attach(entity);
                db.ApplicantEducation.Remove(entity);
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
