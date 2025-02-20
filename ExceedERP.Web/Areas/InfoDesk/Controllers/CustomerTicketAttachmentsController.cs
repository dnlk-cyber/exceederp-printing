using System;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using System.IO;
using ExceedERP.Core.Domain.InfoDesk.Support;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    public class CustomerTicketAttachmentsController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult CustomerTicketAttachments_Read([DataSourceRequest]DataSourceRequest request, int eACSId)
        {
            var attachments = db.CustomerTicketAttachments.Where(x => x.CustomerTicket_HelpDeskId == eACSId).ToList();
            DataSourceResult result = attachments.ToDataSourceResult(request, attachment => new
            {
                CustomerTicketAttachmentId = attachment.CustomerTicketAttachmentId,
                CustomerTicket_HelpDeskId = attachment.CustomerTicket_HelpDeskId,
                FilePath = attachment.FilePath,
                FileName = attachment.FileName,
                ShortPath = attachment.ShortPath,
                FileExtension = attachment.FileExtension,
                Description = attachment.Description
            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTicketAttachments_Create([DataSourceRequest]DataSourceRequest request, CustomerTicketAttachment attachment, int eACSId)
        {
            if (ModelState.IsValid)
            {
                attachment.CustomerTicket_HelpDeskId = eACSId;
                var entity = GetMappedObject(attachment);
                db.CustomerTicketAttachments.Add(entity);
                db.SaveChanges();
                attachment.CustomerTicketAttachmentId = entity.CustomerTicketAttachmentId;
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTicketAttachments_Update([DataSourceRequest]DataSourceRequest request, CustomerTicketAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(attachment);
                db.CustomerTicketAttachments.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult CustomerTicketAttachments_Destroy([DataSourceRequest]DataSourceRequest request, CustomerTicketAttachment attachment)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(attachment);
                db.CustomerTicketAttachments.Attach(entity);
                db.CustomerTicketAttachments.Remove(entity);
                db.SaveChanges();
                RemoveFiles(attachment.FilePath);
            }

            return Json(new[] { attachment }.ToDataSourceResult(request, ModelState));
        }
        private CustomerTicketAttachment GetMappedObject(CustomerTicketAttachment attachment)
        {
            var entity = new CustomerTicketAttachment
            {
                CustomerTicketAttachmentId = attachment.CustomerTicketAttachmentId,
                CustomerTicket_HelpDeskId = attachment.CustomerTicket_HelpDeskId,
                FilePath = attachment.FilePath,
                FileName = attachment.FileName,
                ShortPath = attachment.ShortPath,
                FileExtension = attachment.FileExtension,
                Description = attachment.Description

            };
            return entity;
        }
        public ActionResult RemoveFiles(string fileName)
        {
            if (fileName != null)
            {
                var physicalPath = Path.Combine(Server.MapPath("/Content/InfoDesk/InfoDeskFiles/"), fileName);

                if (System.IO.File.Exists(physicalPath))
                {
                    // Deleting the File from the Location
                    System.IO.File.Delete(physicalPath);
                }
            }

            return Content("");
        }
     
        public ActionResult UploadSingleFiles(int masterId, HttpPostedFileBase file, string Description)
        {
            if (file != null && file.ContentLength > 0)
            {
                string path = HttpContext.Server.MapPath("~/Content/InfoDesk/InfoDeskFiles/");
                string shortPath = "/Content/InfoDesk/InfoDeskFiles/";
                if (!Directory.Exists(path))
                {
                    Directory.CreateDirectory(path);
                }
                string newFileName = Path.GetExtension(file.FileName);
                var fileExtension = Path.GetExtension(file.FileName);
                string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                newFileName = name + newFileName;
                string filePath = path + newFileName;

                file.SaveAs(filePath);

                var entity = new CustomerTicketAttachment
                {
                    CustomerTicket_HelpDeskId = masterId,
                    FilePath = filePath,
                    FileName = newFileName,
                    Description = Description,
                    ShortPath = shortPath,
                    FileExtension = fileExtension
                };

                db.CustomerTicketAttachments.Add(entity);
                db.SaveChanges();
            }

            return RedirectToAction("Index", "CustomerTicket");
        }
        public ActionResult Upload_MultipleFiles(int masterId,  HttpPostedFileBase[] letter, string description)
        {
            for (int i = 0; i < letter.Count(); i++)
            {
                HttpPostedFileBase oneFile = letter[i];
                if (oneFile != null && oneFile.ContentLength > 0)
                {
                    string path = HttpContext.Server.MapPath("~/Content/InfoDesk/InfoDeskFiles/");
                    string shortPath = "/Content/InfoDesk/InfoDeskFiles/";
                    if (!Directory.Exists(path))
                    {
                        Directory.CreateDirectory(path);
                    }
                    string newFileName = Path.GetFileNameWithoutExtension(oneFile.FileName);
                    var fileExtension = Path.GetExtension(oneFile.FileName);
                    string name = "" + DateTime.Now.Year + DateTime.Now.Month + DateTime.Now.Day + DateTime.Now.Hour + DateTime.Now.Minute + DateTime.Now.Second;
                    newFileName = newFileName + name + fileExtension;
                    string filePath = path + newFileName;

                    oneFile.SaveAs(filePath);

                    var entity = new CustomerTicketAttachment
                    {
                        CustomerTicket_HelpDeskId = masterId,
                        FilePath = filePath,
                        FileName = newFileName,
                        Description = description,
                        ShortPath = shortPath,
                        FileExtension = fileExtension
                    };
                    db.CustomerTicketAttachments.Add(entity);
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Index");
        }
        public ActionResult DownloadAttachment(int attachmentId)
        {
            
            var record = db.CustomerTicketAttachments.Find(attachmentId);
            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
                   
        }
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
