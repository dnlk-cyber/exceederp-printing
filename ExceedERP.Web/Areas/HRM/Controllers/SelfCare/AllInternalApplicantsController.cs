﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM.SelfCare;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    public class AllInternalApplicantsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Department"] = db.MainCompanyStructures.ToList();          
            ViewData["Employee"] = db.Person_Employee.ToList();
            ViewData["Vacancy"] = db.VaccancyPlans.ToList();

            return View();
        }

        public ActionResult ApplicantionFormes_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<ApplicantionForm> applicantionformes = db.ApplicantionFormes;
            DataSourceResult result = applicantionformes.ToDataSourceResult(request, applicantionForm => new {
                ApplicantionFormId = applicantionForm.ApplicantionFormId,
                EmployeeId = applicantionForm.EmployeeId,
                OrgStructureId = applicantionForm.OrgStructureId,
                VacancyType = applicantionForm.VacancyType,
                VacancyId = applicantionForm.VacancyId,
                Remark = applicantionForm.Remark,
                IsForOther = applicantionForm.IsForOther,
                OrgBranchName = applicantionForm.OrgBranchName,
                DateCreated = applicantionForm.DateCreated,
                LastModified = applicantionForm.LastModified,
                CreatedBy = applicantionForm.CreatedBy,
                ModifiedBy = applicantionForm.ModifiedBy
            });

            return Json(result);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
