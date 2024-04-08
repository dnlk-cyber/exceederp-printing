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
using System.IO;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.Core.Domain.Archive;
using ExceedERP.Web.Areas.Common.Controllers;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Controllers;
using ExceedERP.Web.Models;
using ExceedERP.Helpers.Common;
using ExceedERP.Helpers.HR;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain;
using ExceedERP.Web.Areas.HRM.Controllers;
using ExceedERP.Core.Domain.ArchiveManagement;

namespace ExceedERP.Web.Areas.Archive.Controllers
{

    [AuthorizeRoles(HrmConstants.HRArchiveAdmin, HrmConstants.HRArchiveUser)]
    public class InstitutionIncomingLettersController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            ViewData["SubjectType"] = db.SubjectTypes.AsQueryable();
            ViewData["InstuType"] = db.InstitutionType.AsQueryable();
            var context = new ApplicationDbContext();

            var allUsers = context.Users.ToList();
            ViewData["Users"] = allUsers;

            ViewData["Departments"] = db.MainCompanyStructures.ToList();

            return View();
        }
        public ActionResult InstitutionIncomingLetters_Read([DataSourceRequest]DataSourceRequest request)
        {
            var newincommingletter = db.InstitutionIncomingLetters.ToList();
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
                //OrgStructureId = incommingLetterRegistration.OrgStructureId,
               // RefernceType = incommingLetterRegistration.RefernceType,
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
                //Status = CheckStatus(incommingLetterRegistration.InstitutionIncomingLetterId),
                RecivedTime = incommingLetterRegistration.RecivedTime,
                SubjectTypeId = incommingLetterRegistration.SubjectTypeId,
                IsTransfered = incommingLetterRegistration.IsTransfered,
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionIncomingLetters_Create([DataSourceRequest] DataSourceRequest request, InstitutionIncomingLetter incommingLetterRegistration)
        {
            ModelState.Clear();

            var checkexistance = db.InstitutionIncomingLetters.FirstOrDefault(x => x.BookNo == incommingLetterRegistration.BookNo);
            if (checkexistance != null)
            {
                ModelState.AddModelError("", "This Book No is already exist!!");
            }
            if (incommingLetterRegistration.LetterDateAmh != null)
            {
                var splitDate = incommingLetterRegistration.LetterDateAmh.Split('/');
                incommingLetterRegistration.LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Incoming Letter Date value is mandatory!!");
            }

            if (incommingLetterRegistration.RecivedDateAmh != null)
            {
                var splitDate = incommingLetterRegistration.RecivedDateAmh.Split('/');
                incommingLetterRegistration.RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
            }
            else
            {
                ModelState.AddModelError("", "Incoming Received Date value is mandatory!!");
            }

            if (incommingLetterRegistration.LetterDate.Date > incommingLetterRegistration.RecivedDate.Date)
            {
                ModelState.AddModelError("", "Letter date sholud be less or equal to recived date!!");
            }
            if (incommingLetterRegistration.LetterDate.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Letter date sholud be less or equal to current date!!");
            }

            if (incommingLetterRegistration.RecivedDate.Date > DateTime.Now.Date)
            {
                ModelState.AddModelError("", "Recived date sholud be less or equal to current date!!");
            }

            //if (incommingLetterRegistration.OutgoingLetterDate != null && incommingLetterRegistration.OutgoingLetterDate.Value.Date > DateTime.Now.Date)
            //{
            //    ModelState.AddModelError("", "Outgoing Letter date sholud be less or equal to current date!!");
            //}

            var archive = db.InstitutionArchiveNo.Where(x => x.SubjectTypeId == incommingLetterRegistration.SubjectTypeId).OrderByDescending(x => x.InstiArchiveNoId).FirstOrDefault();
            if (archive != null)
            {
                incommingLetterRegistration.FileNo = archive.FileNo;
                incommingLetterRegistration.Row = archive.Row;
                incommingLetterRegistration.Shelve = archive.Shelve;
                incommingLetterRegistration.FolderNo = archive.Folder;
            }
            if (ModelState.IsValid)
            {
                var entity = new InstitutionIncomingLetter
                {
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
                   // RefernceType = ReferenceType.Institution.ToString(),
                    Remark = incommingLetterRegistration.Remark,                  
                    InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
                    VolumeNo = incommingLetterRegistration.VolumeNo,
                    LetterDateAmh = incommingLetterRegistration.LetterDateAmh,
                    RecivedDateAmh = incommingLetterRegistration.RecivedDateAmh,
                    LastModified = incommingLetterRegistration.LastModified,
                    DateCreated = DateTime.Now,
                    CreatedBy = User.Identity.Name,
                    ModifiedBy = incommingLetterRegistration.ModifiedBy,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    RecivedTime = incommingLetterRegistration.RecivedTime,
                    SubjectTypeId = incommingLetterRegistration.SubjectTypeId,
                };

                db.InstitutionIncomingLetters.Add(entity);
                db.SaveChanges();
                incommingLetterRegistration.InstitutionIncomingLetterId = entity.InstitutionIncomingLetterId;
                              

                #region operation log
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Institution Incoming Letter", "Success", User.Identity.Name, User.Identity.Name + " add new institution Incoming Letter for " + entity.SenderName + " at date " + DateTime.Today, Modules.ArchiveAndLetters);
                #endregion
            }

            return Json(new[] { incommingLetterRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionIncomingLetters_Update([DataSourceRequest]DataSourceRequest request, InstitutionIncomingLetter incommingLetterRegistration)
        {
            ModelState.Clear();
            if (ModelState.IsValid)
            {
                if (incommingLetterRegistration.RecivedDateAmh != null)
                {
                    var splitDate = incommingLetterRegistration.RecivedDateAmh.Split('/');
                    incommingLetterRegistration.RecivedDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }
                
                if (incommingLetterRegistration.LetterDateAmh != null)
                {
                    var splitDate = incommingLetterRegistration.LetterDateAmh.Split('/');
                    incommingLetterRegistration.LetterDate = XAPI.EthiopicDateTime.GetGregorianDate(Convert.ToInt32(splitDate[0]), Convert.ToInt32(splitDate[1]), Convert.ToInt32(splitDate[2]));
                }

                var archive = db.InstitutionArchiveNo.Where(x => x.SubjectTypeId == incommingLetterRegistration.SubjectTypeId).OrderByDescending(x => x.InstiArchiveNoId).ToList().FirstOrDefault();
                if (archive != null)
                {
                    incommingLetterRegistration.FileNo = archive.FileNo;
                    incommingLetterRegistration.Row = archive.Row;
                    incommingLetterRegistration.Shelve = archive.Shelve;
                    incommingLetterRegistration.FolderNo = archive.Folder;
                }

                var entity = new InstitutionIncomingLetter
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
                   // RefernceType = ReferenceType.Institution.ToString(),
                    Remark = incommingLetterRegistration.Remark,                   
                    InstitutionTypeId = incommingLetterRegistration.InstitutionTypeId,
                    VolumeNo = incommingLetterRegistration.VolumeNo,
                    LetterDateAmh = incommingLetterRegistration.LetterDateAmh,
                    RecivedDateAmh = incommingLetterRegistration.RecivedDateAmh,
                    LastModified = DateTime.Now,
                    DateCreated = incommingLetterRegistration.DateCreated,
                    CreatedBy = incommingLetterRegistration.CreatedBy,
                    ModifiedBy = User.Identity.Name,
                    OrgBranchName = UserInformationHelper.GetUserDefaultBranch(User.Identity.Name),
                    RecivedTime = incommingLetterRegistration.RecivedTime,
                    SubjectTypeId = incommingLetterRegistration.SubjectTypeId,
                };

                db.InstitutionIncomingLetters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;              
                db.SaveChanges();

                #region operation log
                var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                UserHelper.OperationLog(Request.UserHostAddress, "Institution Incoming Letter", "Success", User.Identity.Name, User.Identity.Name + " Updated institution Incoming Letter of " + entity.SenderName + " at date " + DateTime.Today, Modules.ArchiveAndLetters);
                #endregion
            }

            return Json(new[] { incommingLetterRegistration }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InstitutionIncomingLetters_Destroy([DataSourceRequest]DataSourceRequest request, InstitutionIncomingLetter institionIncommingLetter)
        {
            ModelState.Clear();
            var sender = db.LetterSender.FirstOrDefault(x => x.InstitutionIncomingLetterId == institionIncommingLetter.InstitutionIncomingLetterId && x.IsTransfered);
            if (sender != null)
            {
                ModelState.AddModelError("", "You can't delete this data it is already transfered");
            }
            if (ModelState.IsValid)
            {
                var entity = new InstitutionIncomingLetter
                {
                    InstitutionIncomingLetterId = institionIncommingLetter.InstitutionIncomingLetterId,
                    BookNo = institionIncommingLetter.BookNo,
                    LetterDate = institionIncommingLetter.LetterDate,
                    LetterReferenceNo = institionIncommingLetter.LetterReferenceNo,
                    RecivedDate = institionIncommingLetter.RecivedDate,
                    SenderName = institionIncommingLetter.SenderName,
                    Subject = institionIncommingLetter.Subject,
                    Shelve = institionIncommingLetter.Shelve,
                    Row = institionIncommingLetter.Row,
                    FolderNo = institionIncommingLetter.FolderNo,
                    FileNo = institionIncommingLetter.FileNo,
                    //OrgStructureId = institionIncommingLetter.OrgStructureId,
                    //RefernceType = institionIncommingLetter.RefernceType,
                    Remark = institionIncommingLetter.Remark,                   
                    InstitutionTypeId = institionIncommingLetter.InstitutionTypeId,
                    VolumeNo = institionIncommingLetter.VolumeNo,
                    LetterDateAmh = institionIncommingLetter.LetterDateAmh,
                    RecivedDateAmh = institionIncommingLetter.RecivedDateAmh,
                    RecivedTime = institionIncommingLetter.RecivedTime,
                    SubjectTypeId = institionIncommingLetter.SubjectTypeId,
                };

            
                #region delete log
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted institution incomming letter of book no" + institionIncommingLetter.BookNo);
                #endregion
                db.InstitutionIncomingLetters.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;

                var deleteSenderAlso = db.LetterSender.FirstOrDefault(x => x.InstitutionIncomingLetterId == institionIncommingLetter.InstitutionIncomingLetterId);
                if (deleteSenderAlso != null)
                {
                    db.Entry(deleteSenderAlso).State = EntityState.Deleted;
                }

                db.SaveChanges();
            }

            return Json(new[] { institionIncommingLetter }.ToDataSourceResult(request, ModelState));
        }      
        public JsonResult GetShelve()
        {
            var vacancies = db.InstitutionArchiveNo.ToList();
            return Json(vacancies.Select(p => new
            {
                Shelve = p.Shelve
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetRows()
        {
            var vacancies = db.InstitutionArchiveNo.ToList();
            return Json(vacancies.Select(p => new
            {
                Row = p.Row
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFolder()
        {
            var vacancies = db.InstitutionArchiveNo.ToList();
            return Json(vacancies.Select(p => new
            {
                Folder = p.Folder
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetFile()
        {
            var vacancies = db.InstitutionArchiveNo.ToList();
            return Json(vacancies.Select(p => new
            {
                FileNo = p.FileNo
            }), JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetData(int subType)
        {
            var archive = db.InstitutionArchiveNo.Where(x => x.SubjectTypeId == subType).OrderByDescending(x => x.InstiArchiveNoId).FirstOrDefault();

            if (archive != null)
            {
                return Json(new
                {
                    Shelve = archive.Shelve,
                    Row = archive.Row,
                    FileNo = archive.FileNo,
                    Folder = archive.Folder
                });
            }

            return Json(new
            {
                Shelve = "",
                Row = "",
                FileNo = "",
                Folder = ""
            });
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
        public ActionResult UploadDocument(HttpPostedFileBase file, string url, string txtValue, string txtDecsion, int id)
        {
            try
            {
                SetAnswer(id, txtDecsion, file);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                ViewBag.Message = "Upload failed";
                return RedirectToAction("Upload");
            }
        }
        public void SetAnswer(int id, string description, HttpPostedFileBase file)
        {
            var recivedLetters = db.InstitutionIncomingLetters.FirstOrDefault(x => x.InstitutionIncomingLetterId == id);
            if (recivedLetters != null)
            {
                var status = db.LetterSender.FirstOrDefault(x => x.InstitutionIncomingLetterId == id);

                if (status != null)
                {
                    var filePath = "";
                    var newFileName = "";
                    if (file.ContentLength > 0)
                    {
                        string path = HttpContext.Server.MapPath("~/Content/Files/Archive/");
                        string shortPath = "/Content/Files/Archive/";
                        if (!Directory.Exists(path))
                        {
                            Directory.CreateDirectory(path);
                        }
                        newFileName = Path.GetExtension(file.FileName);
                        var fileExtension = System.IO.Path.GetExtension(file.FileName);
                        string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                        newFileName = name + newFileName;
                        filePath = path + newFileName;

                        file.SaveAs(filePath);

                        status.ShortPath = shortPath;
                        status.FileExtension = fileExtension;
                    }

                    var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
                    var placment = db.Placements.FirstOrDefault(x => x.EmployeeId == empId && x.isCurrent);
                    if (placment != null)
                    {
                        status.SenderDepartment = placment.OrgStructureId;
                    }
                    status.Description = description;
                    status.Date = DateTime.Now;
                    status.DateAmharic = XAPI.EthiopicDateTime.GetEthiopicDate(DateTime.Now.Day, DateTime.Now.Month, DateTime.Now.Year);
                    status.Status = LettersStatus.ANSWERED;
                    status.Date = DateTime.Now;
                    status.InstitutionIncomingLetterId = id;          

                    db.Entry(status).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }

            RedirectToAction("Index");
        }
        public ActionResult DownloadArchiveAttachment(int ArchiveAttachementId)
        {
            try
            {
                var record = db.ArchiveAttachements.Find(ArchiveAttachementId);

                if (record != null)
                {
                    string filePath = record.FilePath;

                    if (System.IO.File.Exists(filePath))
                    {
                        return File(filePath, "application/force-download", Path.GetFileName(filePath));
                    }
                    else
                    {
                        // Handle the case where the file does not exist
                        return HttpNotFound("File not found");
                    }
                }
                else
                {
                    // Handle the case where the record with the given ID is not found
                    return HttpNotFound("Record not found");
                }
            }
            catch (Exception ex)
            {
                // Handle other exceptions that might occur
                return Content($"An error occurred: {ex.Message}");
            }
        }

        //public ActionResult DownloadArchiveAttachment(int ArchiveAttachementId)
        //{


        //    var record = db.ArchiveAttachements.Find(ArchiveAttachementId);

        //    return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        //}
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
                    entity.Description = desc;
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
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
