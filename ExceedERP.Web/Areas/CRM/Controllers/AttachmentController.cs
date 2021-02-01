﻿using System; using ExceedERP.Web.Helpers;
using System.Collections.Generic;

using System.Data.Entity;
﻿using System.IO;
﻿using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;     
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
 using ExceedERP.Core;
  using ExceedERP.Core.Domain.Common;
 using ExceedERP.DataAccess;
﻿using ExceedERP.DataAccess.Context;
 using AttachmentType = ExceedERP.Core.Domain.CRM.AttachmentType;
using ExceedERP.Helpers.Common;  
using ExceedERP.Web.Areas.FleetManagement.Models;
using ExceedERP.Core.Domain.Budget;
 using ExceedERP.Core.Domain.Common.HRM;
 using ExceedERP.Core.Domain.CRM;

namespace ExceedERP.Web.Areas.CRM.Controllers
{


       [AuthorizeRoles( CRMRoles.CRMAdmin)] 
    public class AttachmentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Attachments_Read([DataSourceRequest]DataSourceRequest request, string attachmentReferenceType,string id)
        {
            AttachmentType value = AttachmentType.OTHER;
            switch (attachmentReferenceType.ToLower())
            {
                case "photo":
                    value = AttachmentType.PHOTO;
                    break;
                case "experience":
                    value = AttachmentType.EXPERIENCE;
                    break;
                case "education":
                    value = AttachmentType.EDUCATION;
                    break;
                case "training":
                    value = AttachmentType.TRAINING;
                    break;
                case "clearance":
                    value = AttachmentType.CLEARANCE;
                    break;
                case "commitment":
                    value = AttachmentType.COMMITMENT;
                    break;
                case "incomingLetter":
                    value = AttachmentType.INCOMING_LETTER;
                    break;
                case "outgoingLetter":
                    value = AttachmentType.OUTGOING_LETTER;
                    break;
                case "other":
                    value = AttachmentType.OTHER ;
                    break;
                case "logo":
                    value = AttachmentType.LOGO ;
                    break;
            }
            IQueryable<Attachment> attachments = db.Attachments.Where(a => a.Reference == id.ToString());
            DataSourceResult result = attachments.ToDataSourceResult(request, attachment => new {
                AttachmentId = attachment.AttachmentId,
                ReferenceType = attachment.ReferenceType,
                Reference = attachment.Reference,
                Description = attachment.Description,
                URL = attachment.URL,
                BinaryFile = attachment.BinaryFile,
                FileType = attachment.FileType,
                Index = attachment.Index,
                //CreatedBy = attachment.CreatedBy,
                //DateCreated = attachment.DateCreated,
                //LastModified = attachment.LastModified,
                //ModifiedBy = attachment.ModifiedBy,
                //OrgBranchName = attachment.OrgBranchName,
                Remark = attachment.Remark
            });
            CRMDataProviderController.EmployeeId = id;
            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attachments_Create([DataSourceRequest]DataSourceRequest request, Attachment attachment, string attachmentReferenceType, int id)
        {
            AttachmentType value = AttachmentType.OTHER;
            switch (attachmentReferenceType)
            {
                case "Photo":
                    value = AttachmentType.PHOTO;
                    break;
                case "Experience":
                    value = AttachmentType.EXPERIENCE;
                    break;
                case "Education":
                    value = AttachmentType.EDUCATION;
                    break;
                case "Training":
                    value = AttachmentType.TRAINING;
                    break;
                case "Clearance":
                    value = AttachmentType.CLEARANCE;
                    break;
                case "Commitment":
                    value = AttachmentType.COMMITMENT;
                    break;
                case "IncomingLetter":
                    value = AttachmentType.INCOMING_LETTER;
                    break;
                case "OutgoingLetter":
                    value = AttachmentType.OUTGOING_LETTER;
                    break;
                case "Other":
                    value = AttachmentType.OTHER;
                    break;
            }
            if (ModelState.IsValid)
            {
                var entity = new Attachment
                {
                  //  ReferenceType = Refer.OTHER,
                    Reference = id.ToString(),
                    Description = attachment.Description,
                    URL = attachment.URL,
                    BinaryFile = attachment.BinaryFile,
                    FileType = attachment.FileType,
                    Index = attachment.Index,
                    //CreatedBy = User.Identity.Name,
                    //DateCreated = DateTime.Now,
                    Remark = attachment.Remark
                };

                db.Attachments.Add(entity);
                db.SaveChanges();
                attachment.AttachmentId = entity.AttachmentId;
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Attachments_Update([DataSourceRequest]DataSourceRequest request, Attachment attachment)
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
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
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
                UserHelper.DeleteLog(Request.UserHostAddress, "Delete", "Success", User.Identity.Name, "Deleted Attachment number " + attachment.AttachmentId);
		
     
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Save(IEnumerable<HttpPostedFileBase> files)
        {
            // The Name of the Upload component is "files"
            int id = 0;
            try
            {
                int i = 0;

                if (files != null)
                {
                    foreach (var file in files)
                    {
                      
                        byte[] imgData;
                        using (Stream inputStream = file.InputStream)
                        {
                            MemoryStream memoryStream = inputStream as MemoryStream;
                            if (memoryStream == null)
                            {
                                memoryStream = new MemoryStream();
                                inputStream.CopyTo(memoryStream);
                            }

                            imgData = memoryStream.ToArray();
                        }

                        var fileName = Path.GetFileName(file.FileName);
                        string physicalPath = Server.MapPath("~/Content/Images/" + fileName);
                        file.SaveAs(physicalPath);
                        var entity = new Attachment
                        {

                            Reference = id.ToString(),
                            Description = file.FileName,
                            FileType = fileName.Split('.')[1],
                        //    ReferenceType = (AttachmentType)Enum.Parse(typeof(AttachmentType), fileName.Split('.')[1]),
                            Index = i,
                            BinaryFile = imgData,
                            URL = physicalPath

                        };
                        db.Attachments.Add(entity);

                        db.SaveChanges();
                        i++;
                    }

                    // Some browsers send file names with full path.
                    // We are only interested in the file name.

                }

            }
            catch (Exception)
            {

                // throw;
            }
            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Remove(string[] fileNames, int id)
        {
            // The parameter of the Remove action must be called "fileNames"
            var attachments = db.Attachments.Where(a => a.Reference == id.ToString());
            db = new ExceedDbContext();
            if (attachments.Count() > 0)
            {
                if (fileNames != null)
                {
                    foreach (var a in attachments)
                    {
                        foreach (var fullName in fileNames)
                        {

                            var fileName = Path.GetFileName(fullName);
                            if (a.Description == fileName)
                            {

                                if (System.IO.File.Exists(a.URL))
                                {
                                    // The files are not actually removed in this demo
                                    System.IO.File.Delete(a.URL);
                                }

                                db.Attachments.Attach(a);
                                db.Attachments.Remove(a);
                                db.SaveChanges();
                            }

                        }
                    }
                }

            }
            // Return an empty string to signify success
            return Content("");
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
