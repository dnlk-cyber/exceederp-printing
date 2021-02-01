﻿using System;
using System.Collections.Generic;
using System.Data.Entity;
﻿using System.IO;
﻿using System.Linq;
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
﻿using ExceedERP.Helpers.Common;
﻿using ExceedERP.Web.Controllers;
﻿using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
  
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin)]
    public class EmployeeLetterSenderController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static string DateAmharic { get; set; }
        public static DateTime Date { get; set; }
        public static LettersStatus LettersStatus { get; set; }
        public static int RecivedLettersId { get; set; }
        public ActionResult Index()
        {
            var context = new ApplicationDbContext();

            var allUsers = context.Users.ToList();
            ViewData["Users"] = allUsers;
            return View();
        }
        public void SetDate(string name, string value)
        {
            if (name == "DateAmharic")
            {
                DateAmharic = value;
                var splitDate = DateAmharic.Split('/');
                Date = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            
        }
        public ActionResult EmployeeLetterSender_Read([DataSourceRequest]DataSourceRequest request, string bookId)
        {
            IQueryable<EmployeeLetterSender> EmployeeLetterSenders = db.EmployeeLetterSender.Where( x=> x.Receiver == User.Identity .Name ).OrderBy (x=> x.Date );
            DataSourceResult result = EmployeeLetterSenders.ToDataSourceResult(request, employeeLetterSender => new {
                LetterSenderId = employeeLetterSender.LetterSenderId,
                Date = employeeLetterSender.Date,
              
                Receiver = employeeLetterSender.Receiver,
                Description = employeeLetterSender.Description,
                Sender = employeeLetterSender.Sender,
                BookNo = employeeLetterSender.BookNo,
                OwnerName = GetOwener(employeeLetterSender.BookNo),
                Status = employeeLetterSender.Status,
                DateAmharic = employeeLetterSender.DateAmharic,
                FileName = employeeLetterSender.FileName,
            });

            return Json(result);
        }
        private string GetOwener(string bookno)
        {
            var book = db.EmployeeIncommingLetter.Where(x => x.BookNo == bookno).FirstOrDefault();
            if (book!=null)
            {
                return book.OwnerName;
            }
            return "";

        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeLetterSender_Create([DataSourceRequest]DataSourceRequest request, EmployeeLetterSender EmployeeLetterSender, string bookTwo)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeLetterSender
                {
                    Date = DateTime .Now.Date ,
                  
                    Receiver = EmployeeLetterSender.Receiver,
                    Description = EmployeeLetterSender.Description,
                    Sender = User.Identity.Name,                  
                    BookNo = bookTwo,
                    OwnerName = GetOwener(bookTwo),
                    Status = LettersStatus.NEW,
                    DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    FileName = EmployeeLetterSender.FileName,
                    FileType = EmployeeLetterSender.FileType,
                    File = EmployeeLetterSender.File,
                };

                db.EmployeeLetterSender.Add(entity);
                db.SaveChanges();
                EmployeeLetterSender.LetterSenderId = entity.LetterSenderId;

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                
                    UserHelper.OperationLog(Request.UserHostAddress, "Sent Letter", "Success", User.Identity.Name, User.Identity.Name + " add send new letter to " + entity.OwnerName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

                #endregion
            }

            return Json(new[] { EmployeeLetterSender }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeLetterSender_Update([DataSourceRequest]DataSourceRequest request, EmployeeLetterSender EmployeeLetterSender)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeLetterSender
                {
                    LetterSenderId = EmployeeLetterSender.LetterSenderId,
                    Date = EmployeeLetterSender.Date,
                    DateAmharic = EmployeeLetterSender.DateAmharic,
                    Receiver = EmployeeLetterSender.Receiver,
                    Description = EmployeeLetterSender.Description,
                    Sender = User.Identity.Name,
                    BookNo = EmployeeLetterSender.BookNo ,
                    OwnerName = EmployeeLetterSender.OwnerName,
                    Status = EmployeeLetterSender.Status,
                    FileName = EmployeeLetterSender.FileName,
                };

                db.EmployeeLetterSender.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log

                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
               
                    UserHelper.OperationLog(Request.UserHostAddress, "Sent Letter", "Success", User.Identity.Name, User.Identity.Name + " updated sent letter of " + entity.OwnerName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);
                
                #endregion
            }

            return Json(new[] { EmployeeLetterSender }.ToDataSourceResult(request, ModelState));
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        public void SetTransfer(int id, int reciver)
        {
            db = new ExceedDbContext();
            LettersStatus = LettersStatus.TRANSFERED;
            RecivedLettersId = id;
            var recivedLetters = db.EmployeeLetterSender.Where(x => x.LetterSenderId == id).FirstOrDefault();
            var entity = new EmployeeLetterSender
            {
                LetterSenderId = id,
                OwnerName = recivedLetters.OwnerName,
                Receiver = recivedLetters.Receiver,
                Sender = recivedLetters.Sender,
                Description = recivedLetters.Description,
                FileName = recivedLetters.FileName,
                Status = LettersStatus.ANSWERED,
                Date =DateTime.Now ,
                DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day,DateTime.Now.Month ,DateTime.Now .Year ),
                BookNo = recivedLetters.BookNo,
            };
            db = new ExceedDbContext();
            db.EmployeeLetterSender.Add(entity);
            //  db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            //RedirectToAction("Index");
        }
        public void SetAnswer(int id, string fileName, string path, string fileType)
        {
            db = new ExceedDbContext();
            LettersStatus = LettersStatus.ANSWERED;
            RecivedLettersId = id;
            byte[] image = System.IO.File.ReadAllBytes(path);
            var recivedLetters = db.EmployeeLetterSender.Where(x => x.LetterSenderId == id).FirstOrDefault();
            var entity = new EmployeeLetterSender
            {
                LetterSenderId = id,
                OwnerName = recivedLetters.OwnerName,
                Receiver = recivedLetters.Receiver,
                Sender = recivedLetters.Sender,
                Description = recivedLetters.Description,
                FileName = fileName,
                Status = LettersStatus.ANSWERED,
                Date = DateTime.Now.Date,
                DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                BookNo = recivedLetters.BookNo,
                File = image,
                FileType = fileType
            };
            db = new ExceedDbContext();

            db.EmployeeLetterSender.Add(entity);
            //db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            //  RedirectToAction("Index");
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
                        db.EmployeeLetterSender
                            .FirstOrDefault(x => x.LetterSenderId.ToString() == filename);
                if (attachment != null)
                {
                    WebClient req = new WebClient();
                    HttpResponse response = System.Web.HttpContext.Current.Response;
                    response.Clear();
                    response.ClearContent();
                    response.ClearHeaders();
                    response.Buffer = true;
                    response.AddHeader("Content-Disposition",
                        "attachment;filename=\"" + attachment.FileName + "\"");
                    byte[] datas =attachment.File;
                    response.BinaryWrite(datas);
                    response.End();
                    HRMInputs.SelectedFile = 0;
                    return File(attachment.FileName, attachment.FileType);
                }
            }
            catch (Exception)
            {

                // throw;
            }
            HRMInputs.SelectedFile = 0;
            return null;
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeLetterSender_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeLetterSender EmployeeLetterSender)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeLetterSender
                {
                    LetterSenderId = EmployeeLetterSender.LetterSenderId,
                    Date = EmployeeLetterSender.Date,
                    DateAmharic = EmployeeLetterSender.DateAmharic,
                    Receiver = EmployeeLetterSender.Receiver,
                    Description = EmployeeLetterSender.Description,
                    Sender = EmployeeLetterSender.Sender,
                    BookNo = EmployeeLetterSender.BookNo ,
                    OwnerName = EmployeeLetterSender.OwnerName,
                    Status = EmployeeLetterSender.Status,
                    FileName = EmployeeLetterSender.FileName,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted letter sender information of name " + EmployeeLetterSender.Description);
                #endregion
                db.EmployeeLetterSender.Attach(entity);
                db.EmployeeLetterSender.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { EmployeeLetterSender }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult UpdateStatus(string BookNo)
        {
           
            var incommingLetterRegistration = db.EmployeeIncommingLetter.Where(x => x.BookNo == BookNo).FirstOrDefault();
            if (incommingLetterRegistration != null)
            {

              

                if (incommingLetterRegistration.EnableDisbale)
                {
                    incommingLetterRegistration.EnableDisbale = false;
                }
                else
                {
                    incommingLetterRegistration.EnableDisbale = true;
                }
                incommingLetterRegistration.LastModified = DateTime.Now;
                incommingLetterRegistration.OrgBranchName =
                    UserInformationHelper.GetUserDefaultBranch(User.Identity.Name);
                db = new ExceedDbContext();
                db.EmployeeIncommingLetter.Attach(incommingLetterRegistration);
                db.Entry(incommingLetterRegistration).State = EntityState.Modified;
                db.SaveChanges();


            }

            return null;
        }
        public int CheckStatus(string id)
        {
            int result = 0;
            var book = db.EmployeeIncommingLetter.Where(x => x.BookNo == id).FirstOrDefault();

            if (book != null)
            {
                var newstat =
                    db.EmployeeLetterSender.FirstOrDefault(x => x.BookNo == id && x.Status == LettersStatus.ANSWERED || x.Status == LettersStatus.TRANSFERED);

                if (newstat != null)
                {
                    result = 1;
                }
                else
                {
                    result = 0;
                }
            }
            return result;
        }
        public ActionResult UploadDocument(HttpPostedFileBase file, string url, string txtValue, string txtDecsion, string id)
        {
            try
            {
                HRMInputs.LetterId = id;
                HRMInputs.Owner = "Employee";
                if (file.ContentLength > 0)
                {

                    var fileName = Path.GetFileName(file.FileName);
                    var path =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId.ToString() + "-" +
                                                    AttachmentType.DOCUMENT), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId.ToString() + "-" +
                                                    AttachmentType.DOCUMENT));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    if (!System.IO.File.Exists(path))
                    {


                        file.SaveAs(path);
                    }
                    SetAnswer(Convert.ToInt32(HRMInputs.LetterId), file.FileName, path, file.ContentType);

                    return RedirectToAction("Index");
                }
            }
            catch
            {
                ViewBag.Message = "Upload failed";
              
            }
            return RedirectToAction("Index");
        }
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    db = new ExceedDbContext();
                    //int letterid = Convert.ToInt32(HRMInputs.LetterId);
                    //var recivedLetters =
                    //    db.RecivedLetters.Where(
                    //        x => x.RecivedLetterId == letterid).FirstOrDefault();
                    //if (recivedLetters != null)
                    //{
                    //    recivedLetters.FileName = file.FileName;
                    //    var entityy = new RecivedLetters
                    //    {
                    //        RecivedLetterId = recivedLetters.RecivedLetterId,
                    //        InstnPerson = recivedLetters.InstnPerson,
                    //        Receiver = recivedLetters.Receiver,
                    //        Sender = recivedLetters.Sender,
                    //        Description = recivedLetters.Description,
                    //        FileName = recivedLetters.FileName,
                    //        LetterStatus = recivedLetters.LetterStatus,
                    //        RecivedDate = recivedLetters.RecivedDate,
                    //        BookNo = recivedLetters.BookNo
                    //    };
                    //    db = new ExceedDbContext();
                    //    db.RecivedLetters.Attach(entityy);
                    //    db.RecivedLetters.Remove(entityy);
                    //    db.SaveChanges();
                    //    db = new ExceedDbContext();
                    //    entityy.LetterStatus = LettersStatus.NEW;
                    //    db.RecivedLetters.Add(entityy);

                    //    db.SaveChanges();
                    //    recivedLetters.RecivedLetterId = entityy.RecivedLetterId;
                    //    HRMInputs.LetterId = recivedLetters.RecivedLetterId.ToString();
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId + "-" + AttachmentType.LETTER), fileName);
                    var mainPath =
                        Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId + "-" + AttachmentType.LETTER));
                    if (!Directory.Exists(mainPath))
                    {
                        Directory.CreateDirectory(mainPath);
                    }
                    file.SaveAs(path);

                    var entity = new Attachment
                    {

                        Reference = HRMInputs.LetterId.ToString(),
                        Description = file.FileName,
                        FileType = file.ContentType,
                        ReferenceType = AttachmentType.LETTER,
                        Index = 0,
                        URL = path,
                        Remark = HRMInputs.Owner

                    };

                    //entity.Reference = recivedLetters.RecivedLetterId.ToString();
                    //entity.Remark = recivedLetters.RecivedLetterId.ToString();
                    db.Attachments.Add(entity);

                    db.SaveChanges();

                    //}
                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Index");
            }
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
