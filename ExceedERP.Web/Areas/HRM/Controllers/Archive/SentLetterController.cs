﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Models;
using ExceedERP.Core.Domain.Common;
﻿using ExceedERP.Core.Domain.Common.HRM;
﻿using ExceedERP.Core.Domain.HRM.ArchiveManagement;
﻿using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Helpers.Common;
﻿using ExceedERP.Web.Controllers;
﻿using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
   
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin)]
    public class SentLetterController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static LettersStatus LettersStatus { get; set; }
        public static int RecivedLettersId { get; set; }
        public static int UserDepartment { get; set; }
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();
            UserDepartment = UserInformationHelper.GetEmployeeDepartmentId(User.Identity.Name);
            var allUsers = context.Users.ToList();
            ViewData["Users"] = allUsers;
            GetDepartments();
            return View();
        }
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.Where(x => x.Parent == 0).FirstOrDefault();
            if (departments != null)
            {
                var deptlevelOne = db.MainCompanyStructures.Where(x => x.Parent == departments.OrgStructureId).ToList();
                if (deptlevelOne.Any())
                {
                    foreach (var a in deptlevelOne)
                    {
                        var departmentLevelTwo = db.MainCompanyStructures.Where(x => x.Parent == a.OrgStructureId).ToList();
                        if (departmentLevelTwo.Any())
                        {
                            foreach (var b in departmentLevelTwo)
                            {
                                allDepartment.Add(b);
                            }
                        }
                        allDepartment.Add(a);
                    }
                }
            }
            ViewData["Departments"] = allDepartment;
        }
    
        public ActionResult LetterSender_Read([DataSourceRequest]DataSourceRequest request)
        {

            IQueryable<LetterSender> LetterSender = db.LetterSender.Where(x => x.Receiver == UserDepartment);
            DataSourceResult result = LetterSender.ToDataSourceResult(request, letterSender => new {
                LetterSenderId = letterSender.LetterSenderId,
                Date = letterSender.Date,
                Receiver = letterSender.Receiver,
                Description = letterSender.Description,
                Sender = letterSender.Sender,
                BookNo = letterSender.BookNo ,
                OwnerName = GetOwener(letterSender.BookNo),
                Status = letterSender.Status,
               
                CurrentStatus = CheckStatus(letterSender.BookNo),
                SenderDepartment = letterSender.SenderDepartment,
            });

            return Json(result);
        }
        private string GetOwener(string bookno)
        {
            var book = db.NewIncommingLetter.Where(x => x.BookNo == bookno).FirstOrDefault();
            if (book != null)
            {
                return book.OwnerName;
            }
            return "";

        }
        [AcceptVerbs(HttpVerbs.Post)]
    
        public ActionResult LetterSender_Create([DataSourceRequest]DataSourceRequest request, LetterSender letterSender)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LetterSender
                {
                    Date = DateTime .Now ,
                    Receiver = letterSender.Receiver,
                    Description = letterSender.Description,
                    Sender = User.Identity.Name,
                    BookNo = letterSender.BookNo,
                    SenderDepartment = letterSender.SenderDepartment,
                  
                    OwnerName = letterSender.OwnerName,
                    Status = LettersStatus.NEW,
                };

                db.LetterSender.Add(entity);
                db.SaveChanges();
                letterSender.LetterSenderId = entity.LetterSenderId;
            }

            return Json(new[] { letterSender }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult LetterSender_Update([DataSourceRequest]DataSourceRequest request, LetterSender letterSender)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LetterSender
                {
                    LetterSenderId = letterSender.LetterSenderId,
                    Date = letterSender.Date,
                    Receiver = letterSender.Receiver,
                    Description = letterSender.Description,
                    Sender = User.Identity.Name,
                    BookNo = letterSender.BookNo ,
                    OwnerName = letterSender.OwnerName,
                    Status = letterSender.Status,
                    SenderDepartment = letterSender.SenderDepartment,
                };

                db.LetterSender.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { letterSender }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
       
        public ActionResult LetterSender_Destroy([DataSourceRequest]DataSourceRequest request, LetterSender letterSender)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new LetterSender
                {
                    LetterSenderId = letterSender.LetterSenderId,
                    Date = letterSender.Date,
                    Receiver = letterSender.Receiver,
                    Description = letterSender.Description,
                    Sender = letterSender.Sender,
                    BookNo = letterSender.BookNo ,
                    OwnerName = letterSender.OwnerName,
                    Status = letterSender.Status,
                    SenderDepartment = letterSender.SenderDepartment,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted sent letter information of book no " + letterSender.BookNo);
                #endregion
                db.LetterSender.Attach(entity);
                db.LetterSender.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { letterSender }.ToDataSourceResult(request, ModelState));
        }
        public void SetTransfer(int id)
        {
            db = new ExceedDbContext();
            LettersStatus = LettersStatus.TRANSFERED;
            RecivedLettersId = id;
            var recivedLetters = db.LetterSender.Where(x => x.LetterSenderId == id).FirstOrDefault();
            var entity = new LetterSender
            {
                LetterSenderId = id,
                OwnerName = recivedLetters.OwnerName,
                Receiver = recivedLetters.Receiver,
                Sender = recivedLetters.Sender,
                Description = recivedLetters.Description,
                FileName = recivedLetters.FileName,
                Status = LettersStatus.ANSWERED,
                Date = recivedLetters.Date,
                BookNo = recivedLetters.BookNo,

            };
            db = new ExceedDbContext();
            db.LetterSender.Add(entity);
          //  db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            //RedirectToAction("Index");
        }
        public void SetAnswer(int id)
        {
            db = new ExceedDbContext();
            LettersStatus = LettersStatus.ANSWERED;
            RecivedLettersId = id;

            var recivedLetters = db.LetterSender.Where(x => x.LetterSenderId == id).FirstOrDefault();
            var entity = new LetterSender
            {
                LetterSenderId = id,
                OwnerName = recivedLetters.OwnerName,
                Receiver = recivedLetters.Receiver,
                Sender = recivedLetters.Sender,
                Description = recivedLetters.Description,
                FileName = recivedLetters.FileName,
                Status = LettersStatus.ANSWERED,
                Date = recivedLetters.Date,
                BookNo = recivedLetters.BookNo,
            };
            db = new ExceedDbContext();

            db.LetterSender.Add(entity);
            //db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
          //  RedirectToAction("Index");
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        public ActionResult DownloadFile(string filename)
        {
            if (filename == "")
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }
            HRMInputs.LetterId = filename;
            HRMInputs.Owner = filename;
            db = new ExceedDbContext();
            try
            {
                var attachment =
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.LETTER && x.Remark == HRMInputs.Owner)
                            .FirstOrDefault();
                if (attachment != null)
                {
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.Description + "\"");
                    byte[] datas = req.DownloadData(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId.ToString() + "-" + AttachmentType.LETTER + "/" + attachment.Description));
                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(filename, attachment.FileType);
                }
            }
            catch (Exception)
            {

                // throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }
        public int SetStatus(string statusName)
        {
            int status = 0;
            if (statusName == "ANSWERED" || statusName == "TRANSFERED")
            {
                status = 1;
            }
            return status;
        }
        public int CheckStatus(string id)
        {
            int result = 0;
            var book = db.NewIncommingLetter.FirstOrDefault(x => x.BookNo == id);

            if (book != null)
            {
                var newstat =
                    db.LetterSender.Where(x => x.BookNo == id);

                if (newstat.Any())
                {
                    var answer = newstat.FirstOrDefault(x => x.Status == LettersStatus.ANSWERED);
                    if (answer != null)
                    {
                        result = 1;
                    }
                    var transfer = newstat.FirstOrDefault(x => x.Status == LettersStatus.TRANSFERED);
                    if (transfer != null)
                    {
                        result = 2;
                    }
                }
            }
            return result;
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
