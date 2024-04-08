using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using System;
using Kendo.Mvc.UI;
using System.Web;
using ExceedERP.Core.Domain.Common;
using System.Linq;
using ExceedERP.Web.Models;
using System.IO;
using System.Data.Entity;
using ExceedERP.Web.Helpers;
using ExceedERP.Helpers.HR;
using Kendo.Mvc.Extensions;
using ExceedERP.Core.Domain.Common.HRM;
using System.Collections.Generic;
using ExceedERP.Core.Domain.ArchiveManagement;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Hubs;

namespace ExceedERP.Web.Areas.Archive.Controllers
{
    public class DepartmentSentLetterController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/DepartmentSentLetter
        public ActionResult Index()
        {
            ViewData["InstuType"] = db.InstitutionType.AsQueryable();

            ViewData["Departments"] = db.MainCompanyStructures.ToList();

            return View();
        }      
        public ActionResult DepartmentSentLetter_Read([DataSourceRequest]DataSourceRequest request)
        {
            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            int orgId = 0;
            var place = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
            if (place != null)
            {
                orgId = place.OrgStructureId;
            }
            var lists = db.LetterSender.Where(x => x.Receiver == orgId && x.IsTransfered).Select(x => x.InstitutionIncomingLetterId).ToList();
            var newincommingletter = db.InstitutionIncomingLetters.Where(x => lists.Contains(x.InstitutionIncomingLetterId)).ToList();

            DataSourceResult result = newincommingletter.ToDataSourceResult(request, incommingLetterRegistration => new
            {
                InstitutionIncomingLetterId = incommingLetterRegistration.InstitutionIncomingLetterId,
                BookNo = incommingLetterRegistration.BookNo,
                LetterDate = incommingLetterRegistration.LetterDate,
                LetterReferenceNo = incommingLetterRegistration.LetterReferenceNo,
                RecivedDate = incommingLetterRegistration.RecivedDate,
                SenderName = incommingLetterRegistration.SenderName,
                Subject = incommingLetterRegistration.Subject,
                Shelve = incommingLetterRegistration.Shelve,
                Row = incommingLetterRegistration.Row,
                FolderNo = incommingLetterRegistration.FolderNo,
                FileNo = incommingLetterRegistration.FileNo,
               // OrgStructureId = incommingLetterRegistration.OrgStructureId,
                Remark = incommingLetterRegistration.Remark,               
                InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
                VolumeNo = incommingLetterRegistration.VolumeNo,
                LetterDateAmh = incommingLetterRegistration.LetterDateAmh,
                RecivedDateAmh = incommingLetterRegistration.RecivedDateAmh,
                LastModified = incommingLetterRegistration.LastModified,
                DateCreated = incommingLetterRegistration.DateCreated,
                CreatedBy = incommingLetterRegistration.CreatedBy,
                ModifiedBy = incommingLetterRegistration.ModifiedBy,
                OrgBranchName = incommingLetterRegistration.OrgBranchName,
                //Status = incommingLetterRegistration.Status,
                RecivedTime = incommingLetterRegistration.RecivedTime,
                IsTransfered = incommingLetterRegistration.IsTransfered,
                Time = incommingLetterRegistration.RecivedTime

            });

            return Json(result);
        }        
        public ActionResult LetterSender_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var letterSender = db.LetterSender.Where(x => x.InstitutionIncomingLetterId == id).ToList();

            DataSourceResult result = letterSender.ToDataSourceResult(request, sender => new
            {
                LetterSenderId = sender.LetterSenderId,
                Date = sender.Date,
                Receiver = sender.Receiver,
                Description = sender.Description,
                InstitutionIncomingLetterId = sender.InstitutionIncomingLetterId,
                Status = sender.Status,
                DateAmharic = sender.DateAmharic,
                SenderDepartment = sender.SenderDepartment,
                IsTransfered = sender.IsTransfered,
                Time = sender.Time
            });

            return Json(result);
        }
        public JsonResult ApproveInstitutionIncomingAnswer(int id)
        {
            var incoming = db.InstitutionIncomingAnswers.Find(id);

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
        public JsonResult ReturenAssignment(int id)
        {
            var letterSender = db.LetterSender.Find(id);

            if (letterSender != null)
            {
                letterSender.IsTransfered = false;

                db.Entry(letterSender).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();

                var orgId = letterSender.SenderDepartment;
                var placement = db.Placements.FirstOrDefault(x => x.OrgStructureId == orgId && x.isCurrent && x.IsHead);
                if(placement != null)
                {
                    NotifModels notif = new NotifModels
                    {
                        UserId = UserInformationHelper.GetEmployeeUserId(placement.EmployeeId),
                        Title = "Leave Request",
                        Message = $" {UserInformationHelper.GetEmployeeFullName(User.Identity.Name)}  has returen the letter assignement",
                        URL = "/Archive/InstitutionIncomingLetters"
                    };

                    Notify.NotifyUser(notif);
                }
              
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
        public void SetTransfer(int id, int reciver, string desc)
        {
            var institutionLetter = db.InstitutionIncomingLetters.FirstOrDefault(x => x.InstitutionIncomingLetterId == id);
            if (institutionLetter != null)
            {
                var entity = db.LetterSender.FirstOrDefault(x => x.InstitutionIncomingLetterId == id);

                if (entity != null)
                {
                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
                    if (placment != null)
                    {
                        entity.SenderDepartment = placment.OrgStructureId;
                    }
                    entity.IsTransfered = true;
                    entity.Receiver = reciver;
                    entity.Date = DateTime.Now;
                    entity.DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    entity.Status = LettersStatus.TRANSFERED;
                    entity.Date = DateTime.Now;
                    entity.InstitutionIncomingLetterId = id;

                    db.Entry(entity).State = EntityState.Modified;

                    institutionLetter.IsTransfered = true;

                    db.Entry(institutionLetter).State = EntityState.Modified;

                    db.SaveChanges();
                }
            }

            RedirectToAction("Index");
        }
        public JsonResult UploadAnswerFile(HttpPostedFileBase FileAnswer, int Incoming_AnswerId)
        {
            var answer = db.InstitutionIncomingAnswers.Find(Incoming_AnswerId);
            if(answer != null)
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }      
    }
}