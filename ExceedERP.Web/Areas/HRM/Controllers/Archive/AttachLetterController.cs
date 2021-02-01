using System;
using System.Collections.Generic;

using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.HRM;
using ExceedERP.Core.Domain.HRM.ArchiveManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{

    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class AttachLetterController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static LettersStatus LettersStatus { get; set; }
        public static int RecivedLettersId { get; set; }
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RecivedLetters_Read([DataSourceRequest]DataSourceRequest request)
        {
            IQueryable<RecivedLetters> recivedletters = db.RecivedLetters;
            DataSourceResult result = recivedletters.ToDataSourceResult(request, recivedLetters => new
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
                if (recivedLetters.RecivedDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Received date should be less or equal to the current date!!");
                }
                else
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

                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);

                    UserHelper.OperationLog(Request.UserHostAddress, "Attach Letter", "Success", User.Identity.Name, User.Identity.Name + " attach letter called " + entity.FileName + " at date " + DateTime.Today, Modules.ArchiveAndLetters);

                    #endregion
                }
            }

            return Json(new[] { recivedLetters }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult RecivedLetters_Update([DataSourceRequest]DataSourceRequest request, RecivedLetters recivedLetters)
        {
            if (ModelState.IsValid)
            {
                if (recivedLetters.RecivedDate.Date > DateTime.Now.Date)
                {

                    ModelState.AddModelError("", "Received date should be less or equal to the current date!!");
                }
                else
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


                    #region operation log

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                  
                        UserHelper.OperationLog(Request.UserHostAddress, "Attach Letter", "Success", User.Identity.Name, User.Identity.Name + " updated attached letter called " + entity.FileName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

                    #endregion
                }
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
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted received letter of file name" + recivedLetters.FileName);
                #endregion
                db.RecivedLetters.Attach(entity);
                db.RecivedLetters.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { recivedLetters }.ToDataSourceResult(request, ModelState));
        }

        public void SetAnswer(int id)

        {
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

            db.RecivedLetters.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
        public void SetTransfer(int id)
        {
            LettersStatus = LettersStatus.TRANSFERED;
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
                LetterStatus = LettersStatus.TRANSFERED,
                RecivedDate = recivedLetters.RecivedDate,
                BookNo = recivedLetters.BookNo,
            };

            db.RecivedLetters.Attach(entity);
            db.Entry(entity).State = EntityState.Modified;
            db.SaveChanges();
        }
        public ActionResult Upload(HttpPostedFileBase file)
        {
            try
            {
                if (file.ContentLength > 0)
                {
                    db = new ExceedDbContext();
                    int letterid = Convert.ToInt32(HRMInputs.LetterId);
                    var recivedLetters =
                        db.RecivedLetters.Where(
                            x => x.RecivedLetterId == letterid).FirstOrDefault();
                    if (recivedLetters != null)
                    {
                        recivedLetters.FileName = file.FileName;
                        var entityy = new RecivedLetters
                        {
                            RecivedLetterId = recivedLetters.RecivedLetterId,
                            InstnPerson = recivedLetters.InstnPerson,
                            Receiver = recivedLetters.Receiver,
                            Sender = recivedLetters.Sender,
                            Description = recivedLetters.Description,
                            FileName = recivedLetters.FileName,
                            LetterStatus = recivedLetters.LetterStatus,
                            RecivedDate = recivedLetters.RecivedDate,
                            BookNo = recivedLetters.BookNo
                        };
                        db = new ExceedDbContext();
                        db.RecivedLetters.Attach(entityy);
                        db.RecivedLetters.Remove(entityy);
                        db.SaveChanges();
                        db = new ExceedDbContext();
                        entityy.LetterStatus = LettersStatus.NEW;
                        db.RecivedLetters.Add(entityy);

                        db.SaveChanges();
                        recivedLetters.RecivedLetterId = entityy.RecivedLetterId;
                        HRMInputs.LetterId = recivedLetters.RecivedLetterId.ToString();
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

                        entity.Reference = recivedLetters.RecivedLetterId.ToString();
                        entity.Remark = recivedLetters.RecivedLetterId.ToString();
                        db.Attachments.Add(entity);

                        db.SaveChanges();

                    }
                }
                return RedirectToAction("Index");
            }
            catch
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Index");
            }
        }



        public ActionResult Attachments_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = new Attachment
                {
                    AttachmentId = attachment.AttachmentId,
                    ReferenceType = attachment.ReferenceType,
                    Reference = attachment.Reference,
                    Description = attachment.Description,
                    URL = attachment.URL,
                    BinaryFile = attachment.BinaryFile,
                    FileType = attachment.FileType,
                    Index = attachment.Index,
                    Remark = attachment.Remark
                };

                db.Attachments.Attach(entity);
                db.Attachments.Remove(entity);
                db.SaveChanges();
                RemoveFiles(attachment.Description);
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RemoveFiles(string fileName)
        {
            if (fileName != null)
            {

                var physicalPath = Path.Combine(Server.MapPath("~/Content/Images/" + HRMInputs.LetterId + "-" + AttachmentType.LETTER), fileName);

                if (System.IO.File.Exists(physicalPath))
                {
                    // Deleting the File from the Location
                    System.IO.File.Delete(physicalPath);
                }
            }

            return Content("");
        }
        public void Download(int id)
        {
            HRMInputs.SelectedFile = id;

        }
        public ActionResult DownloadFile(string filename)
        {
            if (HRMInputs.SelectedFile == 0)
            {
                string messages = "Please select the row first!!";
                return new JavaScriptResult() { Script = "alert('" + messages + "');" };
            }

            db = new ExceedDbContext();
            try
            {
                var attachment =
                        db.Attachments.Where(x => x.Reference == filename && x.ReferenceType == AttachmentType.LETTER && x.Remark == HRMInputs.Owner && x.AttachmentId == HRMInputs.SelectedFile)
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
