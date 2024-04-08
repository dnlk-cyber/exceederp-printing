using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using System.IO;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class LettersFromEmployeeController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewData["InstuType"] = db.InstitutionType.AsQueryable();
            ViewData["Departments"] = db.MainCompanyStructures.ToList();

            return View();
        }

        public ActionResult LettersFromEmployee_Read([DataSourceRequest]DataSourceRequest request)
        {
            string username = User.Identity.Name;
            var orgId = UserInformationHelper.GetEmployeeDepartmentId(username);
            var incomingIds = db.EmployeeLetterSender.Where(x => x.Receiver == orgId && x.IsTransfered).Select(x => x.EmployeeIncomingLetterId).ToList();

            var employeeincommingletter = db.EmployeeIncommingLetter.Where(x => incomingIds.Contains(x.EmployeeIncomingLetterId)).ToList();

            DataSourceResult result = employeeincommingletter.ToDataSourceResult(request, employeeIncomingLetter => new
            {
                EmployeeIncomingLetterId = employeeIncomingLetter.EmployeeIncomingLetterId,
                BookNo = employeeIncomingLetter.BookNo,
                LetterDate = employeeIncomingLetter.LetterDate,
                LetterAmharic = employeeIncomingLetter.LetterAmharic,
                LetterReferenceNo = employeeIncomingLetter.LetterReferenceNo,
                RecivedDate = employeeIncomingLetter.RecivedDate,
                RecivedDateAmharic = employeeIncomingLetter.RecivedDateAmharic,
                Sender = employeeIncomingLetter.Sender,
                Subject = employeeIncomingLetter.Subject,
                Shelve = employeeIncomingLetter.Shelve,
                Row = employeeIncomingLetter.Row,
                FolderNo = employeeIncomingLetter.FolderNo,
                FileNo = employeeIncomingLetter.FileNo,
                //Receiver = employeeIncomingLetter.Receiver,
                VolumeNo = employeeIncomingLetter.VolumeNo,
                Status = employeeIncomingLetter.Status,
                RecivedTime = employeeIncomingLetter.RecivedTime,
                IsTransfered = employeeIncomingLetter.IsTransfered,
                Remark = employeeIncomingLetter.Remark,
                OrgBranchName = employeeIncomingLetter.OrgBranchName,
                DateCreated = employeeIncomingLetter.DateCreated,
                LastModified = employeeIncomingLetter.LastModified,
                CreatedBy = employeeIncomingLetter.CreatedBy,
                ModifiedBy = employeeIncomingLetter.ModifiedBy
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LettersFromEmployee_Create([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter employeeIncomingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeIncomingLetter
                {
                    BookNo = employeeIncomingLetter.BookNo,
                    LetterDate = employeeIncomingLetter.LetterDate,
                    LetterAmharic = employeeIncomingLetter.LetterAmharic,
                    LetterReferenceNo = employeeIncomingLetter.LetterReferenceNo,
                    RecivedDate = employeeIncomingLetter.RecivedDate,
                    RecivedDateAmharic = employeeIncomingLetter.RecivedDateAmharic,
                    Sender = employeeIncomingLetter.Sender,
                    Subject = employeeIncomingLetter.Subject,
                    Shelve = employeeIncomingLetter.Shelve,
                    Row = employeeIncomingLetter.Row,
                    FolderNo = employeeIncomingLetter.FolderNo,
                    FileNo = employeeIncomingLetter.FileNo,
                    //Receiver = employeeIncomingLetter.Receiver,
                    VolumeNo = employeeIncomingLetter.VolumeNo,
                    Status = employeeIncomingLetter.Status,
                    RecivedTime = employeeIncomingLetter.RecivedTime,
                    IsTransfered = employeeIncomingLetter.IsTransfered,
                    Remark = employeeIncomingLetter.Remark,
                    OrgBranchName = employeeIncomingLetter.OrgBranchName,
                    DateCreated = employeeIncomingLetter.DateCreated,
                    LastModified = employeeIncomingLetter.LastModified,
                    CreatedBy = employeeIncomingLetter.CreatedBy,
                    ModifiedBy = employeeIncomingLetter.ModifiedBy
                };

                db.EmployeeIncommingLetter.Add(entity);
                db.SaveChanges();
                employeeIncomingLetter.EmployeeIncomingLetterId = entity.EmployeeIncomingLetterId;
            }

            return Json(new[] { employeeIncomingLetter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LettersFromEmployee_Update([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter employeeIncomingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeIncomingLetter
                {
                    EmployeeIncomingLetterId = employeeIncomingLetter.EmployeeIncomingLetterId,
                    BookNo = employeeIncomingLetter.BookNo,
                    LetterDate = employeeIncomingLetter.LetterDate,
                    LetterAmharic = employeeIncomingLetter.LetterAmharic,
                    LetterReferenceNo = employeeIncomingLetter.LetterReferenceNo,
                    RecivedDate = employeeIncomingLetter.RecivedDate,
                    RecivedDateAmharic = employeeIncomingLetter.RecivedDateAmharic,
                    Sender = employeeIncomingLetter.Sender,
                    Subject = employeeIncomingLetter.Subject,
                    Shelve = employeeIncomingLetter.Shelve,
                    Row = employeeIncomingLetter.Row,
                    FolderNo = employeeIncomingLetter.FolderNo,
                    FileNo = employeeIncomingLetter.FileNo,
                    //Receiver = employeeIncomingLetter.Receiver,
                    VolumeNo = employeeIncomingLetter.VolumeNo,
                    Status = employeeIncomingLetter.Status,
                    RecivedTime = employeeIncomingLetter.RecivedTime,
                    IsTransfered = employeeIncomingLetter.IsTransfered,
                    Remark = employeeIncomingLetter.Remark,
                    OrgBranchName = employeeIncomingLetter.OrgBranchName,
                    DateCreated = employeeIncomingLetter.DateCreated,
                    LastModified = employeeIncomingLetter.LastModified,
                    CreatedBy = employeeIncomingLetter.CreatedBy,
                    ModifiedBy = employeeIncomingLetter.ModifiedBy
                };

                db.EmployeeIncommingLetter.Attach(entity);
                db.Entry(entity).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { employeeIncomingLetter }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LettersFromEmployee_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter employeeIncomingLetter)
        {
            if (ModelState.IsValid)
            {
                var entity = new EmployeeIncomingLetter
                {
                    EmployeeIncomingLetterId = employeeIncomingLetter.EmployeeIncomingLetterId,
                    BookNo = employeeIncomingLetter.BookNo,
                    LetterDate = employeeIncomingLetter.LetterDate,
                    LetterAmharic = employeeIncomingLetter.LetterAmharic,
                    LetterReferenceNo = employeeIncomingLetter.LetterReferenceNo,
                    RecivedDate = employeeIncomingLetter.RecivedDate,
                    RecivedDateAmharic = employeeIncomingLetter.RecivedDateAmharic,
                    Sender = employeeIncomingLetter.Sender,
                    Subject = employeeIncomingLetter.Subject,
                    Shelve = employeeIncomingLetter.Shelve,
                    Row = employeeIncomingLetter.Row,
                    FolderNo = employeeIncomingLetter.FolderNo,
                    FileNo = employeeIncomingLetter.FileNo,
                    //Receiver = employeeIncomingLetter.Receiver,
                    VolumeNo = employeeIncomingLetter.VolumeNo,
                    Status = employeeIncomingLetter.Status,
                    RecivedTime = employeeIncomingLetter.RecivedTime,
                    IsTransfered = employeeIncomingLetter.IsTransfered,
                    Remark = employeeIncomingLetter.Remark,
                    OrgBranchName = employeeIncomingLetter.OrgBranchName,
                    DateCreated = employeeIncomingLetter.DateCreated,
                    LastModified = employeeIncomingLetter.LastModified,
                    CreatedBy = employeeIncomingLetter.CreatedBy,
                    ModifiedBy = employeeIncomingLetter.ModifiedBy
                };

                db.EmployeeIncommingLetter.Attach(entity);
                db.EmployeeIncommingLetter.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { employeeIncomingLetter }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult EmployeeLetterSender_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var employeeLetterSenders = db.EmployeeLetterSender.Where(x => x.EmployeeIncomingLetterId == id).ToList();
            DataSourceResult result = employeeLetterSenders.ToDataSourceResult(request, employeeLetterSender => new
            {
                LetterSenderId = employeeLetterSender.LetterSenderId,
                Date = employeeLetterSender.Date,
                Receiver = employeeLetterSender.Receiver,
                Description = employeeLetterSender.Description,
                Status = employeeLetterSender.Status,
                DateAmharic = employeeLetterSender.DateAmharic,
                IsTransfered = employeeLetterSender.IsTransfered
            });

            return Json(result);
        }
        public JsonResult UploadAnswerFile(HttpPostedFileBase FileAnswer, int Incoming_AnswerId)
        {
            var answer = db.EmployeeIncomingAnswers.Find(Incoming_AnswerId);
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

                    //answer.FileName = newFileName;
                    //answer.FilePath = filePath;
                    //answer.ShortPath = shortPath;
                    //answer.FileExtension = fileExtension;

                    db.Entry(answer).State = System.Data.Entity.EntityState.Modified;
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
        public JsonResult ApproveEmployeeIncomingAnswer(int id)
        {
            var incoming = db.EmployeeIncomingAnswers.Find(id);

            if (incoming != null)
            {
                incoming.IsAnswered = true;

                db.Entry(incoming).State = System.Data.Entity.EntityState.Modified;
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
