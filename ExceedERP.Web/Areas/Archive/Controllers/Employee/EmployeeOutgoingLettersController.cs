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
using System.IO;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class EmployeeOutgoingLettersController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Employees"] = db.Person_Employee.ToList();
            ViewData["Departments"] = db.MainCompanyStructures.AsQueryable();

            return View();
        }

        public ActionResult EmployeeOutgoingLetters_Read([DataSourceRequest]DataSourceRequest request)
        {
            var employeeoutgoingletters = db.EmployeeOutgoingLetters.ToList();
            DataSourceResult result = employeeoutgoingletters.ToDataSourceResult(request, employeeOutgoingLetter => new {
                EmployeeOutgoingLetterId = employeeOutgoingLetter.EmployeeOutgoingLetterId,
                LetterReferenceNo = employeeOutgoingLetter.LetterReferenceNo,
                SenderOrgId = employeeOutgoingLetter.SenderOrgId,
                ReceiverEmpId = employeeOutgoingLetter.ReceiverEmpId,
                Subject = employeeOutgoingLetter.Subject,
                RoomNo = employeeOutgoingLetter.RoomNo,
                Shelve = employeeOutgoingLetter.Shelve,
                Row = employeeOutgoingLetter.Row,
                FolderNo = employeeOutgoingLetter.FolderNo,
                Volume = employeeOutgoingLetter.Volume,
                FileNo = employeeOutgoingLetter.FileNo,
                OutgoingLetterDate = employeeOutgoingLetter.OutgoingLetterDate,
                OutgoingLetterDateAm = employeeOutgoingLetter.OutgoingLetterDateAm,
                Time = employeeOutgoingLetter.Time,
                IsTransfered = employeeOutgoingLetter.IsTransfered,
                Remark = employeeOutgoingLetter.Remark
            });

            return Json(result);
        }
        public ActionResult EmployeeOutgoingAnswers_Archive_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var employeeoutgoinganswers = db.EmployeeOutgoingAnswers.Where(x => x.EmployeeOutgoingId == id && x.IsAnswered).ToList();

            DataSourceResult result = employeeoutgoinganswers.ToDataSourceResult(request, employeeOutgoingAnswer => new {
                EmployeeOutgoingAnswerId = employeeOutgoingAnswer.EmployeeOutgoingAnswerId,
                EmployeeOutgoingId = employeeOutgoingAnswer.EmployeeOutgoingId,
                Answer = employeeOutgoingAnswer.Answer,
                FileName = employeeOutgoingAnswer.FileName,
                FilePath = employeeOutgoingAnswer.FilePath,
                ShortPath = employeeOutgoingAnswer.ShortPath,
                FileExtension = employeeOutgoingAnswer.FileExtension,
                IsAnswered = employeeOutgoingAnswer.IsAnswered
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeOutgoingLetters_Create([DataSourceRequest]DataSourceRequest request, EmployeeOutgoingLetter employeeOutgoingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeOutgoingLetter
                {
                    LetterReferenceNo = employeeOutgoingLetter.LetterReferenceNo,
                    SenderOrgId = employeeOutgoingLetter.SenderOrgId,
                    ReceiverEmpId = employeeOutgoingLetter.ReceiverEmpId,
                    Subject = employeeOutgoingLetter.Subject,
                    RoomNo = employeeOutgoingLetter.RoomNo,
                    Shelve = employeeOutgoingLetter.Shelve,
                    Row = employeeOutgoingLetter.Row,
                    FolderNo = employeeOutgoingLetter.FolderNo,
                    Volume = employeeOutgoingLetter.Volume,
                    FileNo = employeeOutgoingLetter.FileNo,
                    OutgoingLetterDate = employeeOutgoingLetter.OutgoingLetterDate,
                    OutgoingLetterDateAm = employeeOutgoingLetter.OutgoingLetterDateAm,
                    Time = employeeOutgoingLetter.Time,
                    IsTransfered = employeeOutgoingLetter.IsTransfered,
                    Remark = employeeOutgoingLetter.Remark
                };

                db.EmployeeOutgoingLetters.Add(entity);
                db.SaveChanges();
                employeeOutgoingLetter.EmployeeOutgoingLetterId = entity.EmployeeOutgoingLetterId;
            }

            return Json(new[] { employeeOutgoingLetter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeOutgoingLetters_Update([DataSourceRequest]DataSourceRequest request, EmployeeOutgoingLetter employeeOutgoingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeOutgoingLetter
                {
                    EmployeeOutgoingLetterId = employeeOutgoingLetter.EmployeeOutgoingLetterId,
                    LetterReferenceNo = employeeOutgoingLetter.LetterReferenceNo,
                    SenderOrgId = employeeOutgoingLetter.SenderOrgId,
                    ReceiverEmpId = employeeOutgoingLetter.ReceiverEmpId,
                    Subject = employeeOutgoingLetter.Subject,
                    RoomNo = employeeOutgoingLetter.RoomNo,
                    Shelve = employeeOutgoingLetter.Shelve,
                    Row = employeeOutgoingLetter.Row,
                    FolderNo = employeeOutgoingLetter.FolderNo,
                    Volume = employeeOutgoingLetter.Volume,
                    FileNo = employeeOutgoingLetter.FileNo,
                    OutgoingLetterDate = employeeOutgoingLetter.OutgoingLetterDate,
                    OutgoingLetterDateAm = employeeOutgoingLetter.OutgoingLetterDateAm,
                    Time = employeeOutgoingLetter.Time,
                    IsTransfered = employeeOutgoingLetter.IsTransfered,
                    Remark = employeeOutgoingLetter.Remark
                };

                db.EmployeeOutgoingLetters.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { employeeOutgoingLetter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeOutgoingLetters_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeOutgoingLetter employeeOutgoingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeOutgoingLetter
                {
                    EmployeeOutgoingLetterId = employeeOutgoingLetter.EmployeeOutgoingLetterId,
                    LetterReferenceNo = employeeOutgoingLetter.LetterReferenceNo,
                    SenderOrgId = employeeOutgoingLetter.SenderOrgId,
                    ReceiverEmpId = employeeOutgoingLetter.ReceiverEmpId,
                    Subject = employeeOutgoingLetter.Subject,
                    RoomNo = employeeOutgoingLetter.RoomNo,
                    Shelve = employeeOutgoingLetter.Shelve,
                    Row = employeeOutgoingLetter.Row,
                    FolderNo = employeeOutgoingLetter.FolderNo,
                    Volume = employeeOutgoingLetter.Volume,
                    FileNo = employeeOutgoingLetter.FileNo,
                    OutgoingLetterDate = employeeOutgoingLetter.OutgoingLetterDate,
                    OutgoingLetterDateAm = employeeOutgoingLetter.OutgoingLetterDateAm,
                    Time = employeeOutgoingLetter.Time,
                    IsTransfered = employeeOutgoingLetter.IsTransfered,
                    Remark = employeeOutgoingLetter.Remark
                };

                db.EmployeeOutgoingLetters.Attach(entity);
                db.EmployeeOutgoingLetters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeOutgoingLetter }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DownloadArchiveAttachment(int ArchiveAttachementId)
        {
            var record = db.ArchiveAttachements.Find(ArchiveAttachementId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        public JsonResult SendToEmployee(int empOutGoingId)
        {
            var outgoing = db.EmployeeOutgoingLetters.Find(empOutGoingId);
            if(outgoing != null)
            {
                outgoing.IsTransfered = true;

                db.Entry(outgoing).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                return Json(new { Message = "Successfully Sent" });
            }

            return Json(new { Message = "Can't send please try again later" });
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
