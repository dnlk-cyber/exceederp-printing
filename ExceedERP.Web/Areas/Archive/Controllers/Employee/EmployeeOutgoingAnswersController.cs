﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.DataAccess.Context;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class EmployeeOutgoingAnswersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmployeeOutgoingAnswers_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var employeeoutgoinganswers = db.EmployeeOutgoingAnswers.Where(x => x.EmployeeOutgoingId == id).ToList();

            DataSourceResult result = employeeoutgoinganswers.ToDataSourceResult(request, employeeOutgoingAnswer => new {
                EmployeeOutgoingAnswerId = employeeOutgoingAnswer.EmployeeOutgoingAnswerId,
                EmployeeOutgoingId = employeeOutgoingAnswer.EmployeeOutgoingId,
                Answer = employeeOutgoingAnswer.Answer,
                FileName = employeeOutgoingAnswer.FileName,
                FilePath = employeeOutgoingAnswer.FilePath,
                ShortPath = employeeOutgoingAnswer.ShortPath,
                FileExtension = employeeOutgoingAnswer.FileExtension,
                IsAnswered = employeeOutgoingAnswer.IsAnswered,
                Date = employeeOutgoingAnswer.Date,
                Time = employeeOutgoingAnswer.Time,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeOutgoingAnswers_Create([DataSourceRequest]DataSourceRequest request, EmployeeOutgoingAnswer employeeOutgoingAnswer, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeOutgoingAnswer
                {
                    EmployeeOutgoingId = id,
                    Answer = employeeOutgoingAnswer.Answer,
                    FileName = employeeOutgoingAnswer.FileName,
                    FilePath = employeeOutgoingAnswer.FilePath,
                    IsAnswered = employeeOutgoingAnswer.IsAnswered,
                    Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    Time = DateTime.Now.ToShortTimeString(),
                };

                db.EmployeeOutgoingAnswers.Add(entity);
                db.SaveChanges();
                employeeOutgoingAnswer.EmployeeOutgoingAnswerId = entity.EmployeeOutgoingAnswerId;
            }

            return Json(new[] { employeeOutgoingAnswer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeOutgoingAnswers_Update([DataSourceRequest]DataSourceRequest request, EmployeeOutgoingAnswer employeeOutgoingAnswer)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeOutgoingAnswer
                {
                    EmployeeOutgoingAnswerId = employeeOutgoingAnswer.EmployeeOutgoingAnswerId,
                    EmployeeOutgoingId = employeeOutgoingAnswer.EmployeeOutgoingId,
                    Answer = employeeOutgoingAnswer.Answer,
                    FileName = employeeOutgoingAnswer.FileName,
                    FilePath = employeeOutgoingAnswer.FilePath,
                    IsAnswered = employeeOutgoingAnswer.IsAnswered,
                    Date = employeeOutgoingAnswer.Date,
                    Time = employeeOutgoingAnswer.Time,
                };

                db.EmployeeOutgoingAnswers.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { employeeOutgoingAnswer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeOutgoingAnswers_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeOutgoingAnswer employeeOutgoingAnswer)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeOutgoingAnswer
                {
                    EmployeeOutgoingAnswerId = employeeOutgoingAnswer.EmployeeOutgoingAnswerId,
                    EmployeeOutgoingId = employeeOutgoingAnswer.EmployeeOutgoingId,
                    Answer = employeeOutgoingAnswer.Answer,
                    FileName = employeeOutgoingAnswer.FileName,
                    FilePath = employeeOutgoingAnswer.FilePath,
                    IsAnswered = employeeOutgoingAnswer.IsAnswered,
                    Date = employeeOutgoingAnswer.Date,
                    Time = employeeOutgoingAnswer.Time,
                };

                db.EmployeeOutgoingAnswers.Attach(entity);
                db.EmployeeOutgoingAnswers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeOutgoingAnswer }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
