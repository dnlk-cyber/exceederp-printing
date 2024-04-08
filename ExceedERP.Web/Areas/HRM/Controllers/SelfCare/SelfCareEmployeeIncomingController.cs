using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Helpers;
using ExceedERP.Web.Models;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Archive;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    [AuthorizeRoles(HrmConstants.HREmployeeSelfService, HrmConstants.HRAdmin)]

    public class SelfCareEmployeeIncomingController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public static DateTime LetterDate { get; set; }
        public static int RecivedLettersId { get; set; }
        public static LettersStatus LettersStatus { get; set; }
        public static string LetterAmharic { get; set; }
        public static DateTime RecivedDate { get; set; }
        public static string RecivedDateAmharic { get; set; }
        public static DateTime OutgoingLetterDate { get; set; }
        public static string OutgoingLetterDateAm { get; set; }

        public ActionResult Index()
        {
            var context = new ApplicationDbContext();

            var allUsers = context.Users.ToList();
            ViewData["Users"] = allUsers;
            //ViewData["InstuType"] = db.InstitutionType.AsQueryable();
            GetDepartments();
            return View();
        }
        public void GetDepartments()
        {
            List<MainCompanyStructure> allDepartment = new List<MainCompanyStructure>();
            var departments = db.MainCompanyStructures.ToList();

            ViewData["Departments"] = departments;
        }
        public void SetDate(string name, string value)
        {
            if (name == "RecivedDateAmharic")
            {
                RecivedDateAmharic = value;
                var splitDate = RecivedDateAmharic.Split('/');
                RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "OutgoingLetterDateAm")
            {
                OutgoingLetterDateAm = value;
                var splitDate = OutgoingLetterDateAm.Split('/');
                OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else if (name == "LetterAmharic")
            {
                LetterAmharic = value;
                var splitDate = LetterAmharic.Split('/');
                LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }



        }

        public ActionResult NewIncommingLetter_Read([DataSourceRequest]DataSourceRequest request)
        {
            //var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
            // var lists = db.EmployeeLetterSender.Where(x => x.Receiver == User.Identity.Name).OrderBy(x => x.Date).Select(x => x.BookNo);
            IQueryable<EmployeeIncomingLetter> newincommingletter = db.EmployeeIncommingLetter;//.Where(x => lists.Contains(x.BookNo) && x.RefernceType == "Employee");
            DataSourceResult result = newincommingletter.ToDataSourceResult(request, incommingLetterRegistration => new
            {
                BookNo = incommingLetterRegistration.BookNo,
                LetterDate = incommingLetterRegistration.LetterDate,
                LetterReferenceNo = incommingLetterRegistration.LetterReferenceNo,
                RecivedDate = incommingLetterRegistration.RecivedDate,
                Subject = incommingLetterRegistration.Subject,
                Shelve = incommingLetterRegistration.Shelve,
                Row = incommingLetterRegistration.Row,
                FolderNo = incommingLetterRegistration.FolderNo,
                FileNo = incommingLetterRegistration.FileNo,
                Remark = incommingLetterRegistration.Remark,
                // InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
                VolumeNo = incommingLetterRegistration.VolumeNo,
                LetterAmharic = incommingLetterRegistration.LetterAmharic,
                RecivedDateAmharic = incommingLetterRegistration.RecivedDateAmharic,
                LastModified = incommingLetterRegistration.LastModified,
                DateCreated = incommingLetterRegistration.DateCreated,
                CreatedBy = incommingLetterRegistration.CreatedBy,
                ModifiedBy = incommingLetterRegistration.ModifiedBy,
                OrgBranchName = incommingLetterRegistration.OrgBranchName,
                Status = CheckStatus(incommingLetterRegistration.BookNo)
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult NewIncommingLetter_Create([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter incomingLetterRegistration)
        {
            ModelState.Clear();
            var checkexistance = db.EmployeeIncommingLetter.Where(x => x.BookNo == incomingLetterRegistration.BookNo).FirstOrDefault();
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This Book No is already exist!!");
            }
            else if (LetterAmharic == null || RecivedDateAmharic == null)
            {
                ModelState.AddModelError("", "Incoming Date values are mandatory!!");
            }
            else if (LetterDate.Date > RecivedDate.Date)
            {
                ModelState.AddModelError("", "Letter date sholud be less or equal to recived date!!");
            }
            else if (LetterDate.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Letter date sholud be less or equal to current date!!");
            }
            else if (RecivedDate.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Recived date sholud be less or equal to current date!!");
            }
            if (OutgoingLetterDateAm != null)
            {
                if (OutgoingLetterDate.Date > DateTime.Now.Date)
                {
                    ModelState.AddModelError("", "Outgoing Letter date sholud be less or equal to current date!!");
                }
            }
            else
            {
                OutgoingLetterDateAm = RecivedDateAmharic;
                OutgoingLetterDate = RecivedDate;
            }
            if (ModelState.IsValid)
            {


                var entity = new EmployeeIncomingLetter
                {
                    BookNo = incomingLetterRegistration.BookNo,
                    LetterDate = LetterDate,
                    LetterReferenceNo = incomingLetterRegistration.LetterReferenceNo,
                    RecivedDate = RecivedDate,
                    Subject = incomingLetterRegistration.Subject,
                    Shelve = incomingLetterRegistration.Shelve,
                    Row = incomingLetterRegistration.Row,
                    FolderNo = incomingLetterRegistration.FolderNo,
                    FileNo = incomingLetterRegistration.FileNo,
                    Remark = incomingLetterRegistration.Remark,
                    VolumeNo = incomingLetterRegistration.VolumeNo,
                    LetterAmharic = LetterAmharic,
                    RecivedDateAmharic = RecivedDateAmharic,
                    LastModified = incomingLetterRegistration.LastModified,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = incomingLetterRegistration.ModifiedBy,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),

                };

                db.EmployeeIncommingLetter.Add(entity);
                db.SaveChanges();
                incomingLetterRegistration.BookNo = entity.BookNo;
                db = new ExceedDbContext();
                var entityy = new EmployeeLetterSender
                {
                    Date = DateTime.Now,
                    DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year),
                    Description = incomingLetterRegistration.Subject,
             
                };

                db.EmployeeLetterSender.Add(entityy);
                db.SaveChanges();

            }



            return Json(new[] { incomingLetterRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult NewIncommingLetter_Update([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter incomingLetterRegistration)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (RecivedDateAmharic == null)
                {
                    RecivedDateAmharic = incomingLetterRegistration.RecivedDateAmharic;
                    var splitDate = RecivedDateAmharic.Split('/');
                    RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (OutgoingLetterDateAm == null)
                {
                    var splitDate = OutgoingLetterDateAm.Split('/');
                    OutgoingLetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                if (LetterAmharic == null)
                {
                    LetterAmharic = incomingLetterRegistration.LetterAmharic;
                    var splitDate = LetterAmharic.Split('/');
                    LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }

                var entity = new EmployeeIncomingLetter
                {
                    BookNo = incomingLetterRegistration.BookNo,
                    LetterDate = LetterDate,
                    LetterReferenceNo = incomingLetterRegistration.LetterReferenceNo,
                    RecivedDate = RecivedDate,
                    Subject = incomingLetterRegistration.Subject,
                    Shelve = incomingLetterRegistration.Shelve,
                    Row = incomingLetterRegistration.Row,
                    FolderNo = incomingLetterRegistration.FolderNo,
                    FileNo = incomingLetterRegistration.FileNo,                   
                    VolumeNo = incomingLetterRegistration.VolumeNo,
                    LetterAmharic = incomingLetterRegistration.LetterAmharic,
                    RecivedDateAmharic = incomingLetterRegistration.RecivedDateAmharic,
                    LastModified = DateTime.Now,
                    DateCreated = incomingLetterRegistration.DateCreated,
                    CreatedBy = incomingLetterRegistration.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                };

                db.EmployeeIncommingLetter.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { incomingLetterRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]

        public ActionResult NewIncommingLetter_Destroy([DataSourceRequest]DataSourceRequest request, EmployeeIncomingLetter incomingLetterRegistration)
        {
            ModelState.Clear();
            var newincommingletter = db.EmployeeIncommingLetter.FirstOrDefault();//.Where(x => x.RefernceType == incomingLetterRegistration.BookNo).FirstOrDefault();
            if (ModelState.IsValid)
            {
                var entity = new EmployeeIncomingLetter
                {
                    BookNo = incomingLetterRegistration.BookNo,
                    LetterDate = incomingLetterRegistration.LetterDate,
                    LetterReferenceNo = incomingLetterRegistration.LetterReferenceNo,
                    RecivedDate = incomingLetterRegistration.RecivedDate,
                    Subject = incomingLetterRegistration.Subject,
                    Shelve = incomingLetterRegistration.Shelve,
                    Row = incomingLetterRegistration.Row,
                    FolderNo = incomingLetterRegistration.FolderNo,
                    FileNo = incomingLetterRegistration.FileNo,
                    Remark = incomingLetterRegistration.Remark,
                    VolumeNo = incomingLetterRegistration.VolumeNo,
                    LetterAmharic = incomingLetterRegistration.LetterAmharic,
                    RecivedDateAmharic = incomingLetterRegistration.RecivedDateAmharic
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
        public ActionResult UpdateStatus(string BookNo)
        {
            var incommingLetterRegistration = db.EmployeeIncommingLetter.Where(x => x.BookNo == BookNo).FirstOrDefault();
            if (incommingLetterRegistration != null)
            {

                var entity = new EmployeeIncomingLetter
                {
                    BookNo = incommingLetterRegistration.BookNo,
                    LetterDate = incommingLetterRegistration.LetterDate,
                    LetterReferenceNo = incommingLetterRegistration.LetterReferenceNo,
                    RecivedDate = incommingLetterRegistration.RecivedDate,
                    Subject = incommingLetterRegistration.Subject,
                    Shelve = incommingLetterRegistration.Shelve,
                    Row = incommingLetterRegistration.Row,
                    FolderNo = incommingLetterRegistration.FolderNo,
                    FileNo = incommingLetterRegistration.FileNo,
                    Remark = incommingLetterRegistration.Remark,
                    VolumeNo = incommingLetterRegistration.VolumeNo,
                    LetterAmharic = incommingLetterRegistration.LetterAmharic,
                    RecivedDateAmharic = incommingLetterRegistration.RecivedDateAmharic,

                    
                    LastModified = DateTime.Now,
                    DateCreated = incommingLetterRegistration.DateCreated,
                    CreatedBy = incommingLetterRegistration.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                };
               
                db = new ExceedDbContext();
                db.EmployeeIncommingLetter.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();


            }

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
     
        //public ActionResult LetterSender_Read([DataSourceRequest]DataSourceRequest request, string bookId)
        //{
        //    IQueryable<EmployeeLetterSender> LetterSender = db.EmployeeLetterSender.Where(x => xb == bookId);
        //    DataSourceResult result = LetterSender.ToDataSourceResult(request, letterSender => new
        //    {
        //        LetterSenderId = letterSender.LetterSenderId,
        //        Date = letterSender.Date,
        //        DateAmharic = letterSender.DateAmharic,
        //        Receiver = letterSender.Receiver,
        //        Description = letterSender.Description,
        //        Status = letterSender.Status,
        //        StatusName = letterSender.StatusName,
        //    });

        //    return Json(result);
        //}



        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        public int CheckStatus(string id)
        {
            int result = 0;
            //var book = db.EmployeeIncommingLetter.FirstOrDefault(x => x.BookNo == id);

            //if (book != null)
            //{
            //    var newstat =
            //        db.EmployeeLetterSender.Where(x => x.BookNo == id);

            //    if (newstat.Any())
            //    {
            //        var answer = newstat.FirstOrDefault(x => x.Status == LettersStatus.ANSWERED);
            //        if (answer != null)
            //        {
            //            result = 1;
            //        }
            //        var transfer = newstat.FirstOrDefault(x => x.Status == LettersStatus.TRANSFERED);
            //        if (transfer != null)
            //        {
            //            result = 2;
            //        }
            //    }
            //}
            return result;
        }
    }
}
