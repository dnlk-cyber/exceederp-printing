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
using ExceedERP.Core.Domain.HRM.TrainingEducation;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.HRM.Controllers.TrainingAndEducation
{
    public class EmployeesInAssessmentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmployeesInAssessments_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<EmployeesInAssessment> employeesinassessments = db.EmployeesInAssessments.Where(x=>x.AssesmentId == id);
            DataSourceResult result = employeesinassessments.ToDataSourceResult(request, employeesInAssessment => new {
                EmployeesInAssessmentId = employeesInAssessment.EmployeesInAssessmentId,
                EmployeeId = employeesInAssessment.EmployeeId,
                AssesmentId = employeesInAssessment.AssesmentId,
                JobtitleId = employeesInAssessment.JobtitleId
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeesInAssessments_Create([DataSourceRequest]DataSourceRequest request, EmployeesInAssessment employeesInAssessment, int id)
        {
            var check = db.EmployeesInAssessments.FirstOrDefault(x=> x.AssesmentId == id && x.EmployeeId == employeesInAssessment.EmployeeId);

            if(check != null)
            {
                ModelState.AddModelError("","This employee is registered before");
            }
            if (ModelState.IsValid)
            {
                var placement = db.Placements.FirstOrDefault(x=>x.EmployeeId == employeesInAssessment.EmployeeId && x.isCurrent);
                if(placement != null)
                {
                    employeesInAssessment.JobtitleId = placement.LookupId;
                }
                var entity = new EmployeesInAssessment
                {
                    AssesmentId = id,
                    EmployeeId = employeesInAssessment.EmployeeId,
                    JobtitleId = employeesInAssessment.JobtitleId
                };

                db.EmployeesInAssessments.Add(entity);
                db.SaveChanges();
                employeesInAssessment.EmployeesInAssessmentId = entity.EmployeesInAssessmentId;
            }

            return Json(new[] { employeesInAssessment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeesInAssessments_Update([DataSourceRequest]DataSourceRequest request, EmployeesInAssessment employeesInAssessment)
        {
            var check = db.EmployeesInAssessments.FirstOrDefault(x => x.AssesmentId == employeesInAssessment.AssesmentId && x.EmployeeId == employeesInAssessment.EmployeeId && x.EmployeesInAssessmentId != employeesInAssessment.EmployeesInAssessmentId);

            if (check != null)
            {
                ModelState.AddModelError("", "This employee is registered before");
            }
            if (ModelState.IsValid)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == employeesInAssessment.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    employeesInAssessment.JobtitleId = placement.LookupId;
                }
                var entity = new EmployeesInAssessment
                {
                    AssesmentId = employeesInAssessment.AssesmentId,
                    EmployeesInAssessmentId = employeesInAssessment.EmployeesInAssessmentId,
                    EmployeeId = employeesInAssessment.EmployeeId,
                    JobtitleId = employeesInAssessment.JobtitleId
                };

                db.EmployeesInAssessments.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { employeesInAssessment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeesInAssessments_Destroy([DataSourceRequest]DataSourceRequest request, EmployeesInAssessment employeesInAssessment)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeesInAssessment
                {
                    EmployeesInAssessmentId = employeesInAssessment.EmployeesInAssessmentId,
                    AssesmentId = employeesInAssessment.AssesmentId,
                    EmployeeId = employeesInAssessment.EmployeeId,
                    JobtitleId = employeesInAssessment.JobtitleId
                };

                db.EmployeesInAssessments.Attach(entity);
                db.EmployeesInAssessments.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeesInAssessment }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
