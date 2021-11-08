using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.Core.Domain.Common;
using ExceedERP.Core.Domain.Common.HRM;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using ExceedERP.Web.Helpers;
using ExceedERP.Core.Domain.Global;

namespace ExceedERP.Web.Areas.HRM.Controllers.Archive
{
    public class AttachmentArchiveController : Controller
    {
        // GET: HRM/AttachmentArchive
        //private ExceedDbContext db = new ExceedDbContext();

        //public ActionResult Index()
        //{
        //    return View();
        //}

        //public ActionResult AttachmentArchive_Read([DataSourceRequest]DataSourceRequest request, string attachmentReferenceType, int id, string fileOwner = "")
        //{
        //    AttachmentType value = AttachmentType.OTHER;
        //    if (attachmentReferenceType != null)
        //    {
        //        switch (attachmentReferenceType.ToLower())
        //        {
        //            case "photo":
        //                value = AttachmentType.PHOTO;
        //                break;
        //            case "experience":
        //                value = AttachmentType.EXPERIENCE;
        //                break;
        //            case "education":
        //                value = AttachmentType.EDUCATION;
        //                break;
        //            case "training":
        //                value = AttachmentType.TRAINING;
        //                break;
        //            case "clearance":
        //                value = AttachmentType.CLEARANCE;
        //                break;
        //            case "commitment":
        //                value = AttachmentType.COMMITMENT;
        //                break;
        //            case "incomingletter":
        //                value = AttachmentType.INCOMING_LETTER;
        //                break;
        //            case "outgoingletter":
        //                value = AttachmentType.OUTGOING_LETTER;
        //                break;
        //            case "other":
        //                value = AttachmentType.OTHER;
        //                break;
        //            case "logo":
        //                value = AttachmentType.LOGO;
        //                break;
        //            case "cosign":
        //                value = AttachmentType.COSIGN;
        //                HRMInputs.CosignId = id;
        //                break;
        //            case "letter":
        //                value = AttachmentType.LETTER;
        //                HRMInputs.LetterId = id.ToString();
        //                break;
        //            case "minute":
        //                value = AttachmentType.MINUTE;
        //                break;
        //            case "document":
        //                value = AttachmentType.DOCUMENT;
        //                break;
        //            case "project_document":
        //                value = AttachmentType.PROJECT_DOCUMENT;
        //                break;
        //        }
        //    }
        //    if (attachmentReferenceType == "TEMPLATE")
        //    {
        //        List<string> attachments = new List<string> { "Branch.xlsx", "Employee.xlsx", "JobTitle.xlsx", "Lookup.xlsx", "Rank.xlsx", "Step.xlsx", };

        //        var attachmentt = new AttachmentArchive();
        //        var list = new List<AttachmentArchive>();
        //        for (int i = 0; i < attachments.Count; i++)

        //        {
        //            attachmentt = new AttachmentArchive();
        //            attachmentt.AttachmentArchiveId = i;

        //            attachmentt.Description = attachments[i];

        //            attachmentt.FileType = "application/vnd.ms-excel";
        //            list.Add(attachmentt);
        //        }
        //        DataSourceResult result = list.ToDataSourceResult(request, attachment => new
        //        {
        //            AttachmentArchiveId = attachment.AttachmentArchiveId,
        //            ReferenceType = attachment.ReferenceType,
        //            Reference = attachment.Reference,
        //            Description = attachment.Description,
        //            URL = attachment.URL,
        //            BinaryFile = attachment.BinaryFile,
        //            FileType = attachment.FileType,
        //            Index = attachment.Index,
        //            Remark = attachment.Remark,
        //            FileName = attachment.FileName
        //        });

        //        return Json(result);
        //    }
        //    else
        //    {


        //        IQueryable<AttachmentArchive> attachments =
        //            db.AttachmentArchives.Where(
        //                a => a.ReferenceType == (value) && a.Reference == id.ToString() && a.Remark == fileOwner);
        //        DataSourceResult result = attachments.ToDataSourceResult(request, attachment => new
        //        {
        //            AttachmentArchiveId = attachment.AttachmentArchiveId,
        //            ReferenceType = attachment.ReferenceType,
        //            Reference = attachment.Reference,
        //            Description = attachment.Description,
        //            URL = attachment.URL,
        //            BinaryFile = attachment.BinaryFile,
        //            FileType = attachment.FileType,
        //            Index = attachment.Index,
        //            Remark = attachment.Remark,
        //            FileName = attachment.FileName
        //        });
        //        HRMInputs.EmployeeId = id;
        //        HRMInputs.Owner = fileOwner;
        //        return Json(result);
        //    }
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult AttachmentArchive_Create([DataSourceRequest]DataSourceRequest request, Attachment attachment, string attachmentReferenceType, int id, string fileOwner = "")
        //{
        //    AttachmentType value = AttachmentType.OTHER;
        //    switch (attachmentReferenceType)
        //    {
        //        case "Photo":
        //            value = AttachmentType.PHOTO;
        //            break;
        //        case "Experience":
        //            value = AttachmentType.EXPERIENCE;
        //            break;
        //        case "Education":
        //            value = AttachmentType.EDUCATION;
        //            break;
        //        case "Training":
        //            value = AttachmentType.TRAINING;
        //            break;
        //        case "Clearance":
        //            value = AttachmentType.CLEARANCE;
        //            break;
        //        case "Commitment":
        //            value = AttachmentType.COMMITMENT;
        //            break;
        //        case "IncomingLetter":
        //            value = AttachmentType.INCOMING_LETTER;
        //            break;
        //        case "OutgoingLetter":
        //            value = AttachmentType.OUTGOING_LETTER;
        //            break;
        //        case "Other":
        //            value = AttachmentType.OTHER;
        //            break;
        //    }
        //    if (ModelState.IsValid)
        //    {
          
        //        var entity = new AttachmentArchive
        //        {
        //            ReferenceType = value,
        //            Reference = id.ToString(),
        //            Description = attachment.Description,
        //            URL = attachment.URL,
        //            BinaryFile = attachment.BinaryFile,
        //            FileType = attachment.FileType,
        //            Index = attachment.Index,
        //            Remark = fileOwner,
        //            FileName = attachment.FileName
        //        };

        //        db.AttachmentArchives.Add(entity);
        //        db.SaveChanges();
        //        attachment.AttachmentId = entity.AttachmentArchiveId;


        //        #region operation log

        //        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
              
        //            UserHelper.OperationLog(Request.UserHostAddress, "Attachment Archive", "Success", User.Identity.Name, User.Identity.Name + " attach letter called " + entity.FileName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

        //        #endregion
        //    }

        //    return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult AttachmentArchive_Update([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new Attachment
        //        {
        //            AttachmentId = attachment.AttachmentId,
        //            ReferenceType = attachment.ReferenceType,
        //            Reference = attachment.Reference,
        //            Description = attachment.Description,
        //            URL = attachment.URL,
        //            BinaryFile = attachment.BinaryFile,
        //            FileType = attachment.FileType,
        //            Index = attachment.Index,
        //            Remark = attachment.Remark,
        //            FileName = attachment.FileName
        //        };

        //        db.Attachments.Attach(entity);
        //        db.Entry(entity).State = EntityState.Modified;
        //        db.SaveChanges();

        //        #region operation log

        //        var empId = UserInformationHelper.GetEmployeeId(User.Identity.Name);
        //        var name = db.Person_Employee.FirstOrDefault(x => x.EmployeeId == empId);
        //        if (name != null)
        //        {
        //            UserHelper.OperationLog(Request.UserHostAddress, "Attachment Archive", "Success", User.Identity.Name, User.Identity.Name + " updated attached letter called " + entity.FileName + " at date " + DateTime.Today,Modules.ArchiveAndLetters);

        //        }
        //        #endregion
        //    }

        //    return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        //}

        //[AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult AttachmentArchive_Destroy([DataSourceRequest]DataSourceRequest request, Attachment attachment)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var entity = new Attachment
        //        {
        //            AttachmentId = attachment.AttachmentId,
        //            ReferenceType = attachment.ReferenceType,
        //            Reference = attachment.Reference,
        //            Description = attachment.Description,
        //            URL = attachment.URL,
        //            BinaryFile = attachment.BinaryFile,
        //            FileType = attachment.FileType,
        //            Index = attachment.Index,
        //            Remark = attachment.Remark,
        //            FileName = attachment.FileName
        //        };
        //        #region delete log
        //        UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment" + attachment.ReferenceType.ToString() + " " + attachment.Description);
        //        #endregion
        //        db.Attachments.Attach(entity);
        //        db.Attachments.Remove(entity);
        //        db.SaveChanges(); RemoveFiles(attachment.Description);
        //    }

        //    return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        //}
        //public ActionResult RemoveFiles(string fileName)
        //{
        //    if (fileName != null)
        //    {

        //        var physicalPath = Path.Combine(Server.MapPath("~/Content/Images/0-DOCUMENT"), fileName);

        //        if (System.IO.File.Exists(physicalPath))
        //        {
        //            // Deleting the File from the Location
        //            System.IO.File.Delete(physicalPath);
        //        }
        //    }

        //    return Content("");
        //}
        //public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        //{
        //    // The Name of the Upload component is "files"
        //    int id = 0;
        //    try
        //    {
        //        int i = 0;

        //        if (files != null)
        //        {
        //            foreach (var file in files)
        //            {

        //                byte[] imgData;
        //                using (Stream inputStream = file.InputStream)
        //                {
        //                    MemoryStream memoryStream = inputStream as MemoryStream;
        //                    if (memoryStream == null)
        //                    {
        //                        memoryStream = new MemoryStream();
        //                        inputStream.CopyTo(memoryStream);
        //                    }

        //                    imgData = memoryStream.ToArray();
        //                }

        //                var fileName = Path.GetFileName(file.FileName);
        //                string physicalPath = Server.MapPath("~/Content/Images/" + fileName);
        //                file.SaveAs(physicalPath);
        //                var entity = new Attachment
        //                {

        //                    Reference = id.ToString(),
        //                    Description = file.FileName,
        //                    FileType = fileName.Split('.')[1],
        //                    ReferenceType = (AttachmentType)Enum.Parse(typeof(AttachmentType), fileName.Split('.')[1]),
        //                    Index = i,
        //                    BinaryFile = imgData,
        //                    URL = physicalPath

        //                };
        //                db.Attachments.Add(entity);

        //                db.SaveChanges();
        //                i++;
        //            }

        //            // Some browsers send file names with full path.
        //            // We are only interested in the file name.

        //        }

        //    }
        //    catch (Exception)
        //    {

        //        // throw;
        //    }
        //    // Return an empty string to signify success
        //    return Content("");
        //}

        //public ActionResult Remove(string[] fileNames, int id)
        //{
        //    // The parameter of the Remove action must be called "fileNames"
        //    var attachments = db.Attachments.Where(a => a.Reference == id.ToString());
        //    db = new ExceedDbContext();
        //    if (attachments.Count() > 0)
        //    {
        //        if (fileNames != null)
        //        {
        //            foreach (var a in attachments)
        //            {
        //                foreach (var fullName in fileNames)
        //                {

        //                    var fileName = Path.GetFileName(fullName);
        //                    if (a.Description == fileName)
        //                    {

        //                        if (System.IO.File.Exists(a.URL))
        //                        {
        //                            // The files are not actually removed in this demo
        //                            System.IO.File.Delete(a.URL);
        //                        }

        //                        db.Attachments.Attach(a);
        //                        db.Attachments.Remove(a);
        //                        db.SaveChanges();
        //                    }

        //                }
        //            }
        //        }

        //    }
        //    // Return an empty string to signify success
        //    return Content("");
        //}
        //protected override void Dispose(bool disposing)
        //{
        //    db.Dispose();
        //    base.Dispose(disposing);
        //}
    }
}
