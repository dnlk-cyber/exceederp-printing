﻿using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
﻿using ExceedERP.Core.Domain.Common;
﻿using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
﻿using ExceedERP.Core.Domain.HRM.ArchiveManagement;
﻿using ExceedERP.DataAccess.Context;
﻿using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Helpers.Common;
﻿using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
   
    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class RecivedLetterController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public  static  LettersStatus LettersStatus { get; set; }
        public static int RecivedLettersId { get; set; }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RecivedLetters_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RecivedLetters> recivedletters = db.RecivedLetters;
            DataSourceResult result = recivedletters.ToDataSourceResult(request, recivedLetters => new {
                RecivedLetterId = recivedLetters.RecivedLetterId,
                InstnPerson = recivedLetters.InstnPerson,
                Receiver = recivedLetters.Receiver,
                Sender = recivedLetters.Sender,
                Description = recivedLetters.Description,
                FileName = recivedLetters.FileName,
                LetterStatus = recivedLetters.LetterStatus,
                RecivedDate = recivedLetters.RecivedDate,
                BookNo = recivedLetters.BookNo,
                Status = SetStatus(recivedLetters.LetterStatus.ToString())
            });

            return Json(result);
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
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecivedLetters_Create([DataSourceRequest]DataSourceRequest request, RecivedLetters recivedLetters)
        {
            if (ModelState.IsValid)
            {
                var entity = new RecivedLetters
                {
                    InstnPerson = recivedLetters.InstnPerson,
                    Receiver = recivedLetters.Receiver,
                    Sender = recivedLetters.Sender,
                    Description = recivedLetters.Description,
                    FileName = recivedLetters.FileName,
                    LetterStatus = recivedLetters.LetterStatus,
                    RecivedDate = recivedLetters.RecivedDate,
                       BookNo = recivedLetters.BookNo,
                };

                db.RecivedLetters.Add(entity);
                db.SaveChanges();
                recivedLetters.RecivedLetterId = entity.RecivedLetterId;
            }

            return Json(new[] { recivedLetters }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecivedLetters_Update([DataSourceRequest]DataSourceRequest request, RecivedLetters recivedLetters)
        {
            if (ModelState.IsValid)
            {
                var entity = new RecivedLetters
                {
                    RecivedLetterId = recivedLetters.RecivedLetterId,
                    InstnPerson = recivedLetters.InstnPerson,
                    Receiver = recivedLetters.Receiver,
                    Sender = recivedLetters.Sender,
                    Description = recivedLetters.Description,
                    FileName = recivedLetters.FileName,
                    LetterStatus = recivedLetters.LetterStatus,
                    RecivedDate = recivedLetters.RecivedDate,
                    BookNo = recivedLetters.BookNo,
                };

                db.RecivedLetters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { recivedLetters }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecivedLetters_Destroy([DataSourceRequest]DataSourceRequest request, RecivedLetters recivedLetters)
        {
            if (ModelState.IsValid)
            {
                var entity = new RecivedLetters
                {
                    RecivedLetterId = recivedLetters.RecivedLetterId,
                    InstnPerson = recivedLetters.InstnPerson,
                    Receiver = recivedLetters.Receiver,
                    Sender = recivedLetters.Sender,
                    Description = recivedLetters.Description,
                    FileName = recivedLetters.FileName,
                    LetterStatus = recivedLetters.LetterStatus,
                    RecivedDate = recivedLetters.RecivedDate,
                    BookNo = recivedLetters.BookNo,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted recived letter information of file no " + recivedLetters.Receiver);
                #endregion
                db.RecivedLetters.Attach(entity);
                db.RecivedLetters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { recivedLetters }.ToDataSourceResult(request, ModelState));
        }

        public void SetAnswer(int id)

        {
            db = new ExceedDbContext();
            LettersStatus = LettersStatus.ANSWERED;
            RecivedLettersId = id;
          
            var recivedLetters = db.RecivedLetters.Where(x => x.RecivedLetterId == id).FirstOrDefault();
            var entity = new RecivedLetters
            {
                RecivedLetterId = id,
                InstnPerson = recivedLetters.InstnPerson,
                Receiver = recivedLetters.Receiver,
                Sender = recivedLetters.Sender,
                Description = recivedLetters.Description,
                FileName = recivedLetters.FileName,
                LetterStatus = LettersStatus.ANSWERED,
                RecivedDate = recivedLetters.RecivedDate,
                BookNo = recivedLetters.BookNo,
            };
            db = new ExceedDbContext();

            db.RecivedLetters.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            RedirectToAction("Index");
        }
        public string CheckStatus(int id)
        {
            string result = "";
            var stat = db.RecivedLetters .Where(x => x.RecivedLetterId  == id).FirstOrDefault();
            if (stat != null)
            {
                if (stat.LetterStatus  == LettersStatus.NEW)
                {
                    result = "c";
                }
                else
                {
                    result = "p";
                }
            }
            return result;
        }
        public void SetTransfer(int id)
        {
            db = new ExceedDbContext();
            LettersStatus = LettersStatus.TRANSFERED ;
            RecivedLettersId = id;
            var recivedLetters = db.RecivedLetters.Where(x => x.RecivedLetterId == id).FirstOrDefault();
            var entity = new RecivedLetters
            {
                RecivedLetterId =id,
                InstnPerson = recivedLetters.InstnPerson,
                Receiver = recivedLetters.Receiver,
                Sender = recivedLetters.Sender,
                Description = recivedLetters.Description,
                FileName = recivedLetters.FileName,
                LetterStatus = LettersStatus.TRANSFERED,
                RecivedDate = recivedLetters.RecivedDate,
                BookNo = recivedLetters.BookNo,
            };
            db = new ExceedDbContext();
            db.RecivedLetters.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
            RedirectToAction("Index");
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;
          
        }
        public ActionResult DownloadFile(string filename)
        {
            if (filename== "")
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
