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
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    public class ApplicationFormHRController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            PopulateViewData();
            return View();
        }
        private void PopulateViewData()
        {
            ViewData["EmployeeName"] = db.Person_Employee.ToList();
            ViewData["ApplicationVacancyCodes"] = db.VaccancyPlans.Select(x => new { x.VacancyId, x.VaccancyCode }).ToList();
            ViewData["workunits"] = db.MainCompanyStructures.ToList();
            var vacancyJobTitles = db.VaccancyPlans.Select(x => x.JobTitle);
            var jobtitles = db.Lookups.Where(x => x.LookupType == LookUpTypes.JOB_TITLE || x.LookupType == LookUpTypes.OLD_JOB_TITLE
                || vacancyJobTitles.Contains(x.LookupId)).ToList();
            ViewData["JobTitle"] = jobtitles;
            
        }

        public ActionResult ApplicantionFormes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApplicantionForm> applicantionformes = db.ApplicantionFormes;
            DataSourceResult result = applicantionformes.ToDataSourceResult(request, applicantionForm => new {
                ApplicantionFormId = applicantionForm.ApplicantionFormId,
                EmployeeCode = applicantionForm.EmployeeCode,
                EmployeeId = applicantionForm.EmployeeId,
                VacancyId = applicantionForm.VacancyId,
                ApplicationType = applicantionForm.ApplicationType,
                Remark = applicantionForm.Remark,
                TotalResult = applicantionForm.TotalResult,
                LookupId = applicantionForm.LookupId,
                VacancyType = applicantionForm.VacancyType,
                MediaVacancyCode = applicantionForm.MediaVacancyCode,
                IsForOther = applicantionForm.IsForOther,
                OrgStructureId = applicantionForm.OrgStructureId,
                From = applicantionForm.From,
                To = applicantionForm.To,
                OrgBranchName = applicantionForm.OrgBranchName,
                DateCreated = applicantionForm.DateCreated,
                LastModified = applicantionForm.LastModified,
                CreatedBy = applicantionForm.CreatedBy,
                ModifiedBy = applicantionForm.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantionFormes_Create([DataSourceRequest]DataSourceRequest request, ApplicantionForm applicantionForm)
        {
            ApplicationVacancy.Apply(db, applicantionForm, ModelState);
            var employee = db.Person_Employee.SingleOrDefault(x => x.EmployeeId == applicantionForm.EmployeeId && x.IsActive);
            if (employee == null)
                ModelState.AddModelError(nameof(applicantionForm.EmployeeId), "Select an active employee.");
            else
                applicantionForm.EmployeeCode = employee.EmployeeCode;

            var appForm = db.ApplicantionFormes.FirstOrDefault(x => x.EmployeeId == applicantionForm.EmployeeId && x.VacancyId == applicantionForm.VacancyId);

            if (appForm != null)
            {
                ModelState.AddModelError("", "This employee already has an application for the vacancy.");
            }
            var desciplines = db.DisciplinaryCases.Where(x => x.EmployeeId == applicantionForm.EmployeeId).ToList();
            if (desciplines.Any())
            {
                foreach (var disc in desciplines)
                {
                    var disciplineMeasures = db.EmpDisciplineMeasure.Where(x => x.DisciplinaryCaseId == disc.DisciplinaryCaseId).ToList();
                    foreach (var dm in disciplineMeasures)
                    {
                        var dif = dm.DecisionDate.AddMonths(dm.Month);
                        if (dif > DateTime.Today)
                        {
                            ModelState.AddModelError("", "you can not apply, you have Discipline case");
                        }
                    }
                }
            }
            if (ModelState.IsValid)
            {
                var entity = new ApplicantionForm
                {
                    EmployeeCode = applicantionForm.EmployeeCode,
                    VacancyId = applicantionForm.VacancyId,
                    EmployeeId = applicantionForm.EmployeeId,
                    ApplicationType = applicantionForm.ApplicationType,
                    Remark = applicantionForm.Remark,
                    TotalResult = applicantionForm.TotalResult,
                    LookupId = applicantionForm.LookupId,
                    VacancyType = applicantionForm.VacancyType,
                    MediaVacancyCode = applicantionForm.MediaVacancyCode,
                    IsForOther = applicantionForm.IsForOther,
                    OrgStructureId = applicantionForm.OrgStructureId,
                    From = DateTime.Today,
                    To = DateTime.Today,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name
                };

                db.ApplicantionFormes.Add(entity);
                db.SaveChanges();
                applicantionForm.ApplicantionFormId = entity.ApplicantionFormId;
            }

            return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantionFormes_Update([DataSourceRequest]DataSourceRequest request, ApplicantionForm applicantionForm)
        {
            var entity = db.ApplicantionFormes.Find(applicantionForm.ApplicantionFormId);
            if (entity == null)
            {
                ModelState.AddModelError("", "This application could not be found.");
                return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));
            }

            ApplicationVacancy.Apply(db, applicantionForm, ModelState, entity.VacancyId);
            var employee = db.Person_Employee.SingleOrDefault(x =>
                x.EmployeeId == applicantionForm.EmployeeId && (x.IsActive || x.EmployeeId == entity.EmployeeId));
            if (employee == null)
                ModelState.AddModelError(nameof(applicantionForm.EmployeeId), "Select an employee.");

            if (db.ApplicantionFormes.Any(x => x.EmployeeId == applicantionForm.EmployeeId && x.VacancyId == applicantionForm.VacancyId
                && x.ApplicantionFormId != entity.ApplicantionFormId))
            {
                ModelState.AddModelError("", "This employee already has an application for the vacancy.");
            }

            if (ModelState.IsValid)
            {
                applicantionForm.EmployeeCode = employee.EmployeeCode;
                entity.EmployeeCode = applicantionForm.EmployeeCode;
                entity.EmployeeId = applicantionForm.EmployeeId;
                entity.VacancyId = applicantionForm.VacancyId;
                entity.OrgStructureId = applicantionForm.OrgStructureId;
                entity.LookupId = applicantionForm.LookupId;
                entity.VacancyType = applicantionForm.VacancyType;
                entity.Remark = applicantionForm.Remark;
                entity.ModifiedBy = User.Identity.Name;
                entity.LastModified = DateTime.Now;
                db.SaveChanges();
            }

            return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult ApplicantionFormes_Destroy([DataSourceRequest]DataSourceRequest request, ApplicantionForm applicantionForm)
        {
            if (ModelState.IsValid)
            {
                var entity = new ApplicantionForm
                {
                    ApplicantionFormId = applicantionForm.ApplicantionFormId,
                    EmployeeCode = applicantionForm.EmployeeCode,
                    VacancyId = applicantionForm.VacancyId,
                    EmployeeId = applicantionForm.EmployeeId,
                    ApplicationType = applicantionForm.ApplicationType,
                    Remark = applicantionForm.Remark,
                    TotalResult = applicantionForm.TotalResult,
                    LookupId = applicantionForm.LookupId,
                    VacancyType = applicantionForm.VacancyType,
                    MediaVacancyCode = applicantionForm.MediaVacancyCode,
                    IsForOther = applicantionForm.IsForOther,
                    OrgStructureId = applicantionForm.OrgStructureId,
                    From = applicantionForm.From,
                    To = applicantionForm.To,
                    OrgBranchName = applicantionForm.OrgBranchName,
                    DateCreated = applicantionForm.DateCreated,
                    LastModified = applicantionForm.LastModified,
                    CreatedBy = applicantionForm.CreatedBy,
                    ModifiedBy = applicantionForm.ModifiedBy
                };

                db.ApplicantionFormes.Attach(entity);
                db.ApplicantionFormes.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { applicantionForm }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
