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
using ExceedERP.Core.Domain.ArchiveManagement.Employee;
using ExceedERP.DataAccess.Context;
using System.IO;

namespace ExceedERP.Web.Areas.Archive.Controllers.Employee
{
    public class Attachment_Answer_LetterSentForEmployeesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Attachment_Answer_LetterSentForEmployees_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var attachments = db.Attachment_Answer_LetterSentForEmployees.Where(x => x.EmployeeIncomingAnswerId == id).ToList();

            DataSourceResult result = attachments.ToDataSourceResult(request, attachment_Answer_LetterSentForEmployee => new {
                Attachment_Answer_LetterSentForEmployeeId = attachment_Answer_LetterSentForEmployee.Attachment_Answer_LetterSentForEmployeeId,
                EmployeeIncomingAnswerId = attachment_Answer_LetterSentForEmployee.EmployeeIncomingAnswerId,
                FileName = attachment_Answer_LetterSentForEmployee.FileName,
                FilePath = attachment_Answer_LetterSentForEmployee.FilePath,
                ShortPath = attachment_Answer_LetterSentForEmployee.ShortPath,
                FileExtension = attachment_Answer_LetterSentForEmployee.FileExtension
            });

            return Json(result);
        }
        public ActionResult AttachmentLetterForEmployeeAnswer_MultipleFile(int masterId, HttpPostedFileBase[] AnswerFile)
        {
            var answer = db.EmployeeOutgoingAnswers.Find(masterId);
            if (answer != null && !answer.IsAnswered)
            {
                for (int i = 0; i < AnswerFile.Count(); i++)
                {
                    HttpPostedFileBase oneFile = AnswerFile[i];
                    if (oneFile != null && oneFile.ContentLength > 0)
                    {
                        string path = HttpContext.Server.MapPath("~/Content/Files/Archive/");
                        string shortPath = "/Content/Files/Archive/";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        string newFileName = Path.GetFileNameWithoutExtension(oneFile.FileName);
                        var fileExtension = System.IO.Path.GetExtension(oneFile.FileName);
                        string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                        newFileName = newFileName + name + fileExtension;
                        string filePath = path + newFileName;

                        oneFile.SaveAs(filePath);

                        var entity = new Attachment_Answer_LetterSentForEmployee
                        {
                            EmployeeIncomingAnswerId = masterId,
                            FilePath = filePath,
                            FileName = newFileName,
                            ShortPath = shortPath,
                            FileExtension = fileExtension
                        };

                        db.Attachment_Answer_LetterSentForEmployees.Add(entity);
                        db.SaveChanges();
                    }
                }
            }

            return RedirectToAction("Index", "LettersSentForEmployee");
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attachment_Answer_LetterSentForEmployees_Destroy([DataSourceRequest]DataSourceRequest request, Attachment_Answer_LetterSentForEmployee attachment_Answer_LetterSentForEmployee)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment_Answer_LetterSentForEmployee
                {
                    Attachment_Answer_LetterSentForEmployeeId = attachment_Answer_LetterSentForEmployee.Attachment_Answer_LetterSentForEmployeeId,
                    EmployeeIncomingAnswerId = attachment_Answer_LetterSentForEmployee.EmployeeIncomingAnswerId,
                    FileName = attachment_Answer_LetterSentForEmployee.FileName,
                    FilePath = attachment_Answer_LetterSentForEmployee.FilePath,
                    ShortPath = attachment_Answer_LetterSentForEmployee.ShortPath,
                    FileExtension = attachment_Answer_LetterSentForEmployee.FileExtension
                };

                db.Attachment_Answer_LetterSentForEmployees.Attach(entity);
                db.Attachment_Answer_LetterSentForEmployees.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { attachment_Answer_LetterSentForEmployee }.ToDataSourceResult(request, ModelState));
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
