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
using ExceedERP.Web.Helpers;
using System.IO;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class LettersSentForEmployeeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["Employees"] = db.Person_Employee.ToList();
            ViewData["Departments"] = db.MainCompanyStructures.AsQueryable();

            return View();
        }

        public ActionResult LettersSentForEmployee_Read([DataSourceRequest]DataSourceRequest request)
        {
            var username = User.Identity.Name;
            int empId = UserInformationHelper.GetEmployeeId(username);

            var employeeoutgoingletters = db.EmployeeOutgoingLetters.Where(x => x.ReceiverEmpId == empId && x.IsTransfered).ToList();

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
        public JsonResult UploadAnswerFile(HttpPostedFileBase FileAnswer, int Outgoing_AnswerId)
        {
            var answer = db.EmployeeOutgoingAnswers.Find(Outgoing_AnswerId);
            if (answer != null)
            {
                var filePath = "";
                var newFileName = "";
                if (FileAnswer.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/Files/Archive/");
                    string shortPath = "/Content/Files/Archive/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    newFileName = Path.GetExtension(FileAnswer.FileName);
                    var fileExtension = System.IO.Path.GetExtension(FileAnswer.FileName);
                    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    newFileName = name + newFileName;
                    filePath = path + newFileName;

                    FileAnswer.SaveAs(filePath);

                    answer.FileName = newFileName;
                    answer.FilePath = filePath;
                    answer.ShortPath = shortPath;
                    answer.FileExtension = fileExtension;

                    db.Entry(answer).State = EntityState.Modified;
                    db.SaveChanges();

                    return Json(new
                    {
                        Message = "Successfully uploaded"
                    });
                }
            }

            return Json(new
            {
                Message = "Can't upload please try again"
            });
        }
        public JsonResult ApproveInstitutionIncomingAnswer(int id)
        {
            var outgoing = db.EmployeeOutgoingAnswers.Find(id);

            if (outgoing != null)
            {
                outgoing.IsAnswered = true;

                db.Entry(outgoing).State = EntityState.Modified;
                db.SaveChanges();

                return Json(new
                {
                    Message = "Successfully Approved"
                });
            }

            return Json(new
            {
                Message = "Please try again later"
            });
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
