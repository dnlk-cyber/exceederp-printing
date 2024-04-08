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
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using System.IO;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class EmployeeIncomingAnswersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult EmployeeIncomingAnswers_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var employeeincominganswers = db.EmployeeIncomingAnswers.Where(x => x.EmployeeIncomingLetterId == id).ToList();

            DataSourceResult result = employeeincominganswers.ToDataSourceResult(request, employeeIncomingAnswer => new {
                EmployeeIncomingAnswerId = employeeIncomingAnswer.EmployeeIncomingAnswerId,
                EmployeeIncomingLetterId = employeeIncomingAnswer.EmployeeIncomingLetterId,
               // EmployeeLetterSenderId = employeeIncomingAnswer.EmployeeLetterSenderId,
                OrgStructureId = employeeIncomingAnswer.OrgStructureId,
                Answer = employeeIncomingAnswer.Answer,
                IsAnswered = employeeIncomingAnswer.IsAnswered,
                Date = employeeIncomingAnswer.Date,
                Time = employeeIncomingAnswer.Time,
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeIncomingAnswers_Create([DataSourceRequest]DataSourceRequest request, EmployeeIncomingAnswer employeeIncomingAnswer, int id)
        {
            string username = User.Identity.Name;
            var udb = new ApplicationDbContext();
            var user = udb.Users.FirstOrDefault(x => x.UserName == username);
            if (user != null)
            {
                var placement = db.Placements.FirstOrDefault(x => x.EmployeeId == user.EmployeeId && x.isCurrent);
                if (placement != null)
                {
                    employeeIncomingAnswer.OrgStructureId = placement.OrgStructureId;
                }
                else
                {
                    ModelState.AddModelError("", "you don't have active placement");
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid user");
            }


            if (ModelState.IsValid)
            {
                var entity = new EmployeeIncomingAnswer
                {
                    EmployeeIncomingLetterId = id,
                   // EmployeeLetterSenderId = employeeIncomingAnswer.EmployeeLetterSenderId,
                    OrgStructureId = employeeIncomingAnswer.OrgStructureId,
                    Answer = employeeIncomingAnswer.Answer,
                    IsAnswered = employeeIncomingAnswer.IsAnswered,
                    Date = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day,DateTime.Now.Month,DateTime.Now.Year),
                    Time = DateTime.Now.ToShortTimeString(),
                };

                db.EmployeeIncomingAnswers.Add(entity);
                db.SaveChanges();
                employeeIncomingAnswer.EmployeeIncomingAnswerId = entity.EmployeeIncomingAnswerId;
            }

            return Json(new[] { employeeIncomingAnswer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeIncomingAnswers_Update([DataSourceRequest]DataSourceRequest request, EmployeeIncomingAnswer employeeIncomingAnswer)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeIncomingAnswer
                {
                    EmployeeIncomingAnswerId = employeeIncomingAnswer.EmployeeIncomingAnswerId,
                    EmployeeIncomingLetterId = employeeIncomingAnswer.EmployeeIncomingLetterId,
                   // EmployeeLetterSenderId = employeeIncomingAnswer.EmployeeLetterSenderId,
                    OrgStructureId = employeeIncomingAnswer.OrgStructureId,
                    Answer = employeeIncomingAnswer.Answer,
                    IsAnswered = employeeIncomingAnswer.IsAnswered,
                    Date = employeeIncomingAnswer.Date,
                    Time = employeeIncomingAnswer.Time,
                };

                db.EmployeeIncomingAnswers.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { employeeIncomingAnswer }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeIncomingAnswers_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeIncomingAnswer employeeIncomingAnswer)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeIncomingAnswer
                {
                    EmployeeIncomingAnswerId = employeeIncomingAnswer.EmployeeIncomingAnswerId,
                    EmployeeIncomingLetterId = employeeIncomingAnswer.EmployeeIncomingLetterId,
                   // EmployeeLetterSenderId = employeeIncomingAnswer.EmployeeLetterSenderId,
                    OrgStructureId = employeeIncomingAnswer.OrgStructureId,
                    Answer = employeeIncomingAnswer.Answer,
                    IsAnswered = employeeIncomingAnswer.IsAnswered,
                    Date = employeeIncomingAnswer.Date,
                    Time = employeeIncomingAnswer.Time,
                };

                db.EmployeeIncomingAnswers.Attach(entity);
                db.EmployeeIncomingAnswers.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeIncomingAnswer }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadAnswer(int EmployeeIncomingAnswerId)
        {
            var record = db.Attachment_Answer_LetterSentForEmployees.Find(EmployeeIncomingAnswerId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
