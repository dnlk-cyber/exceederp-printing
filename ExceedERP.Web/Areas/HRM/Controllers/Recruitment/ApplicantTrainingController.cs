﻿using System;
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
    
     [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
    public class ApplicantTrainingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

         [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin, HrmConstants.HRRecruitmentUser)]
        public ActionResult ApplicantTraining_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<ApplicantTraining> applicanttraining = db.ApplicantTraining.Where( x=> x.ApplicantRegistrationId==id);
            DataSourceResult result = applicanttraining.ToDataSourceResult(request, applicantTraining => new {
                ApplicantTrainingId = applicantTraining.ApplicantTrainingId,
                ApplicantRegistrationId = applicantTraining.ApplicantRegistrationId,
                TrainingName = applicantTraining.TrainingName,
                Remark = applicantTraining.Remark
            });

            return Json(result);
        }
     
        [AuthorizeRoles( HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantTraining_Create([DataSourceRequest]DataSourceRequest request, ApplicantTraining applicantTraining, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApplicantTraining
                {
                    TrainingName = applicantTraining.TrainingName,
                    ApplicantRegistrationId=id,
                    Remark = applicantTraining.Remark
                };

                db.ApplicantTraining.Add(entity);
                db.SaveChanges();
                applicantTraining.ApplicantTrainingId = entity.ApplicantTrainingId;

                #region operation log

                var applicant = db.ApplicantRegistration.FirstOrDefault(x => x.ApplicantRegistrationId == entity.ApplicantRegistrationId);
                if (applicant != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Applicant Training ", "Success", User.Identity.Name, User.Identity.Name + " add applicant training Info for " + applicant.ApplicantName + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                    
                }

                #endregion

            }

            return Json(new[] { applicantTraining }.ToDataSourceResult(request, ModelState));
        }
      
        [AuthorizeRoles(HrmConstants.HRRecruitmentUser)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantTraining_Update([DataSourceRequest]DataSourceRequest request, ApplicantTraining applicantTraining)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApplicantTraining
                {
                    ApplicantTrainingId = applicantTraining.ApplicantTrainingId,
                    ApplicantRegistrationId= applicantTraining .ApplicantRegistrationId,
                    TrainingName = applicantTraining.TrainingName,
                    Remark = applicantTraining.Remark
                };

                db.ApplicantTraining.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var applicant = db.ApplicantRegistration.FirstOrDefault(x => x.ApplicantRegistrationId == entity.ApplicantRegistrationId);
                if (applicant != null)
                {
                        UserHelper.OperationLog(Request.UserHostAddress, "Applicant Training ", "Success", User.Identity.Name, User.Identity.Name + " update applicant training Info of " + applicant.ApplicantName + " date " + DateTime.Today.ToShortDateString(),Modules.HRM);
                    
                }

                #endregion

            }

            return Json(new[] { applicantTraining }.ToDataSourceResult(request, ModelState));
        }
     
        [AuthorizeRoles(HrmConstants.HRRecruitmentAdmin, HrmConstants.HRAdmin)]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantTraining_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantTraining applicantTraining)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApplicantTraining
                {
                    ApplicantTrainingId = applicantTraining.ApplicantTrainingId,
                    ApplicantRegistrationId = applicantTraining.ApplicantRegistrationId,
                    TrainingName = applicantTraining.TrainingName,
                    Remark = applicantTraining.Remark
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted applicant training information of applicant" + applicantTraining.ApplicantRegistrationId );
                #endregion
                db.ApplicantTraining.Attach(entity);
                db.ApplicantTraining.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { applicantTraining }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
