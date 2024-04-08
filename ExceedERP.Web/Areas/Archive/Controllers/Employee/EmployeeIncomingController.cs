using System;
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
using ExceedERP.Core.Domain.Common;
using ExceedERP.Web.Areas.Common.Controllers;
using System.IO;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Models;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Core.Domain.ArchiveManagement;

namespace ExceedERP.Web.Areas.Archive.Controllers
{

    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class EmployeeIncomingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            //var context = new ApplicationDbContext();

            //var allUsers = context.Users.ToList();
            //ViewData["Users"] = allUsers;
            ViewData["InstuType"] = db.InstitutionType.AsQueryable();

            ViewData["Employee"] = db.Person_Employee.ToList();
            ViewData["Departments"] = db.MainCompanyStructures.AsQueryable();

            return View();
        }
        public ActionResult EmployeeIncoming_Read([DataSourceRequest]DataSourceRequest request)
        {
            var newincommingletter = db.EmployeeIncommingLetter.ToList();

            DataSourceResult result = newincommingletter.ToDataSourceResult(request, incommingLetterRegistration => new
            {
                EmployeeIncomingLetterId = incommingLetterRegistration.EmployeeIncomingLetterId,
                BookNo = incommingLetterRegistration.BookNo,
                LetterDate = incommingLetterRegistration.LetterDate,
                LetterReferenceNo = incommingLetterRegistration.LetterReferenceNo,
                RecivedDate = incommingLetterRegistration.RecivedDate,
                Sender = incommingLetterRegistration.Sender,
                Subject = incommingLetterRegistration.Subject,
                Shelve = incommingLetterRegistration.Shelve,
                Row = incommingLetterRegistration.Row,
                FolderNo = incommingLetterRegistration.FolderNo,
                FileNo = incommingLetterRegistration.FileNo,
                //RefersTo = incommingLetterRegistration.Receiver,
                Remark = incommingLetterRegistration.Remark,               
                VolumeNo = incommingLetterRegistration.VolumeNo,
                LetterAmharic = incommingLetterRegistration.LetterAmharic,
                RecivedDateAmharic = incommingLetterRegistration.RecivedDateAmharic,
                LastModified = incommingLetterRegistration.LastModified,
                DateCreated = incommingLetterRegistration.DateCreated,
                CreatedBy = incommingLetterRegistration.CreatedBy,
                ModifiedBy = incommingLetterRegistration.ModifiedBy,
                OrgBranchName = incommingLetterRegistration.OrgBranchName,
                //Status = CheckStatus(incommingLetterRegistration.BookNo),
                RecivedTime = incommingLetterRegistration.RecivedTime,
                RoomNo = incommingLetterRegistration.RoomNo,
                IsTransfered = incommingLetterRegistration.IsTransfered
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeIncoming_Create([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter incomingLetterRegistration)
        {
            ModelState.Clear();

            var checkexistance = db.EmployeeIncommingLetter.FirstOrDefault(x => x.BookNo == incomingLetterRegistration.BookNo);
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This Book No is already exist!!");
            }
            if (incomingLetterRegistration.LetterAmharic != null)
            {
                var splitDate = incomingLetterRegistration.LetterAmharic.Split('/');
                incomingLetterRegistration.LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Incoming Date values are mandatory!!");
            }

            if (incomingLetterRegistration.RecivedDateAmharic != null)
            {
                var splitDate = incomingLetterRegistration.RecivedDateAmharic.Split('/');
                incomingLetterRegistration.RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Incoming Date values are mandatory!!");
            }

            if (incomingLetterRegistration.LetterDate.Date > incomingLetterRegistration.RecivedDate.Date)
            {
                ModelState.AddModelError("", "Letter date sholud be less or equal to receiving date!!");
            }
            if (incomingLetterRegistration.LetterDate.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Letter date sholud be less or equal to current date!!");
            }

            var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            //var refersTo = incomingLetterRegistration.Receiver;//UserInformationHelper.GetEmployeeId(incomingLetterRegistration.RefersTo);
            if (empId == 0)
            {
                ModelState.AddModelError("", "This user is not mapped with employee Id!!");
            }
            //if (refersTo == 0)
            //{
            //    ModelState.AddModelError("", "Ther receiver user is not mapped with employee Id!!");
            //}

            var folder = db.PersonalArchiveNo.Where(x => x.EmployeeId == incomingLetterRegistration.Sender).OrderByDescending(x => x.PersonalArchiveNoId).FirstOrDefault();
            if(folder != null)
            {
                incomingLetterRegistration.RoomNo = folder.RoomNo;
                incomingLetterRegistration.Shelve = folder.Shelve;
                incomingLetterRegistration.Row = folder.Row;
                incomingLetterRegistration.FolderNo = folder.Folder;
                incomingLetterRegistration.VolumeNo = folder.Volume;
                incomingLetterRegistration.FileNo = folder.FileNo;
            }
            if (ModelState.IsValid)
            {
                var entity = new EmployeeIncomingLetter
                {
                    BookNo = incomingLetterRegistration.BookNo,
                    RecivedTime = incomingLetterRegistration.RecivedTime,
                    LetterDate = incomingLetterRegistration.LetterDate,
                    LetterReferenceNo = incomingLetterRegistration.LetterReferenceNo,
                    RecivedDate = incomingLetterRegistration.RecivedDate,
                    Sender = incomingLetterRegistration.Sender,
                    Subject = incomingLetterRegistration.Subject,
                    Shelve = incomingLetterRegistration.Shelve,
                    RoomNo = incomingLetterRegistration.RoomNo,
                    Row = incomingLetterRegistration.Row,
                    FolderNo = incomingLetterRegistration.FolderNo,
                    FileNo = incomingLetterRegistration.FileNo,
                    //Receiver = incomingLetterRegistration.Receiver,
                    Remark = incomingLetterRegistration.Remark,                   
                    VolumeNo = incomingLetterRegistration.VolumeNo,
                    LetterAmharic = incomingLetterRegistration.LetterAmharic,
                    RecivedDateAmharic = incomingLetterRegistration.RecivedDateAmharic,
                    LastModified = incomingLetterRegistration.LastModified,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = incomingLetterRegistration.ModifiedBy,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                };

                db.EmployeeIncommingLetter.Add(entity);
                db.SaveChanges();
                incomingLetterRegistration.EmployeeIncomingLetterId = entity.EmployeeIncomingLetterId;
                

                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Employee Incoming Letter", "Success", User.Identity.Name, User.Identity.Name + " add new employee incoming letter of book no " + entity.BookNo + " at date " + DateTime.Today, Modules.ArchiveAndLetters);
                #endregion
            }

            return Json(new[] { incomingLetterRegistration }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeIncoming_Update([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter incomingLetterRegistration)
        {
            ModelState.Clear();

            if (incomingLetterRegistration.LetterAmharic != null)
            {
                var splitDate = incomingLetterRegistration.LetterAmharic.Split('/');
                incomingLetterRegistration.LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Incoming Date values are mandatory!!");
            }

            if (incomingLetterRegistration.RecivedDateAmharic != null)
            {
                var splitDate = incomingLetterRegistration.RecivedDateAmharic.Split('/');
                incomingLetterRegistration.RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Incoming Date values are mandatory!!");
            }

            var folder = db.PersonalArchiveNo.Where(x => x.EmployeeId == incomingLetterRegistration.Sender).OrderByDescending(x => x.PersonalArchiveNoId).FirstOrDefault();
            if (folder != null)
            {
                incomingLetterRegistration.RoomNo = folder.RoomNo;
                incomingLetterRegistration.Shelve = folder.Shelve;
                incomingLetterRegistration.Row = folder.Row;
                incomingLetterRegistration.FolderNo = folder.Folder;
                incomingLetterRegistration.VolumeNo = folder.Volume;
                incomingLetterRegistration.FileNo = folder.FileNo;
            }

            if (ModelState.IsValid)
            {
                var entity = new EmployeeIncomingLetter
                {
                    EmployeeIncomingLetterId = incomingLetterRegistration.EmployeeIncomingLetterId,
                    BookNo = incomingLetterRegistration.BookNo,
                    LetterDate = incomingLetterRegistration.LetterDate,
                    LetterReferenceNo = incomingLetterRegistration.LetterReferenceNo,
                    RecivedDate = incomingLetterRegistration.RecivedDate,
                    Sender = incomingLetterRegistration.Sender,
                    Subject = incomingLetterRegistration.Subject,
                    Shelve = incomingLetterRegistration.Shelve,
                    RoomNo = incomingLetterRegistration.RoomNo,
                    Row = incomingLetterRegistration.Row,
                    FolderNo = incomingLetterRegistration.FolderNo,
                    FileNo = incomingLetterRegistration.FileNo,
                    //Receiver = incomingLetterRegistration.Receiver,
                    Remark = incomingLetterRegistration.Remark,                 
                    VolumeNo = incomingLetterRegistration.VolumeNo,
                    LetterAmharic = incomingLetterRegistration.LetterAmharic,
                    RecivedDateAmharic = incomingLetterRegistration.RecivedDateAmharic,
                    LastModified = DateTime.Now,
                    DateCreated = incomingLetterRegistration.DateCreated,
                    CreatedBy = incomingLetterRegistration.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    RecivedTime = incomingLetterRegistration.RecivedTime,
                };

                db.EmployeeIncommingLetter.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();

                #region operation log
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Employee Incoming Letter", "Success", User.Identity.Name, User.Identity.Name + " updated employee incoming letter of book no " + entity.BookNo + " at date " + DateTime.Today, Modules.ArchiveAndLetters);
                #endregion
            }

            return Json(new[] { incomingLetterRegistration }.ToDataSourceResult(request, ModelState));
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EmployeeIncoming_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter incomingLetterRegistration)
        {
            ModelState.Clear();

            if (ModelState.IsValid)
            {
                var entity = new EmployeeIncomingLetter
                {
                    BookNo = incomingLetterRegistration.BookNo,
                    LetterDate = incomingLetterRegistration.LetterDate,
                    LetterReferenceNo = incomingLetterRegistration.LetterReferenceNo,
                    RecivedDate = incomingLetterRegistration.RecivedDate,
                    Sender = incomingLetterRegistration.Sender,
                    Subject = incomingLetterRegistration.Subject,
                    RoomNo = incomingLetterRegistration.RoomNo,
                    Shelve = incomingLetterRegistration.Shelve,
                    Row = incomingLetterRegistration.Row,
                    FolderNo = incomingLetterRegistration.FolderNo,
                    FileNo = incomingLetterRegistration.FileNo,
                    //Receiver = incomingLetterRegistration.Receiver,
                    Remark = incomingLetterRegistration.Remark,                   
                    VolumeNo = incomingLetterRegistration.VolumeNo,
                    LetterAmharic = incomingLetterRegistration.LetterAmharic,
                    RecivedDateAmharic = incomingLetterRegistration.RecivedDateAmharic,
                    RecivedTime = incomingLetterRegistration.RecivedTime,
                };
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted incomming letter of book no" + incomingLetterRegistration.BookNo);
                #endregion
               
                db.EmployeeIncommingLetter.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { incomingLetterRegistration }.ToDataSourceResult(request, ModelState));
        }
       
        public ActionResult LetterSender_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var LetterSender = db.EmployeeLetterSender.Where(x => x.EmployeeIncomingLetterId == id).ToList();

            DataSourceResult result = LetterSender.ToDataSourceResult(request, letterSender => new
            {
                LetterSenderId = letterSender.LetterSenderId,
                Date = letterSender.Date,
                DateAmharic = letterSender.DateAmharic,
                Receiver = letterSender.Receiver,
                Description = letterSender.Description,
                Status = letterSender.Status,
                StatusName = letterSender.StatusName,
                IsTransfered = letterSender.IsTransfered
            });

            return Json(result);
        }
        public JsonResult GetData(int empId)
        {
            var archive = db.PersonalArchiveNo.Where(x => x.EmployeeId == empId).OrderByDescending(x => x.PersonalArchiveNoId).FirstOrDefault();

            return Json(new
            {
                Shelve = archive.Shelve,
                Row = archive.Row,
                FileNo = archive.FileNo,
                Folder = archive.Folder,
                Volume = archive.Volume,
                RoomNo = archive.RoomNo
            });
        }
        public JsonResult GetShelve()
        {
            var personalArchives = db.PersonalArchiveNo.ToList();
            return Json(personalArchives.Select(p => new
            {
                Shelve = p.Shelve
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRows()
        {
            var personalArchives = db.PersonalArchiveNo.ToList();
            return Json(personalArchives.Select(p => new
            {
                Row = p.Row
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFolder()
        {
            var personalArchives = db.PersonalArchiveNo.ToList();
            return Json(personalArchives.Select(p => new
            {
                Folder = p.Folder
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFile()
        {
            var personalArchives = db.PersonalArchiveNo.ToList();
            return Json(personalArchives.Select(p => new
            {
                FileNo = p.FileNo
            }), JsonRequestBehavior.AllowGet);
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
