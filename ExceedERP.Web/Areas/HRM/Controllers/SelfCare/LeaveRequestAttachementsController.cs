using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using System.IO;
using ExceedERP.Helpers.Common;
using ExceedERP.Core.Domain.Global;
using ExceedERP.Web.Controllers;
using ExceedERP.Core.Domain.HRM.SelfCare;

namespace ExceedERP.Web.Areas.HRM.Controllers.SelfCare
{
    public class LeaveRequestAttachementsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult LeaveRequestAttachements_Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var attachements = db.LeaveRequestAttachements.Where(x => x.LeaveRequestId == id ).ToList();
            DataSourceResult result = attachements.ToDataSourceResult(request, attachement => new
            {
                LeaveRequestAttachementId = attachement.LeaveRequestAttachementId,
                LeaveRequestId = attachement.LeaveRequestId,
                FileExtension = attachement.FileExtension,
                Description = attachement.Description,
                FilePath = attachement.FilePath,
                FileName = attachement.FileName,
                ShortPath = attachement.ShortPath
            });

            return Json(result);
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequestAttachements_Create([DataSourceRequest] DataSourceRequest request, IEnumerable<HttpPostedFileBase> file, int id)
        {
            if (file != null)
            {
                string[] AllowedExtensions = { ".jpg", ".png", ".pdf" };
                int MaxFileSize = 50 * 1024 * 1024;

                string subPath = "~/Content/HRM/LeaveRequestFile/";
                string shortPath = "/Content/HRM/LeaveRequestFile/";
                bool exists = Directory.Exists(Server.MapPath("~/Content/HRM/LeaveRequestFile/"));

                if (!exists)
                    Directory.CreateDirectory(Server.MapPath(subPath));
                string fileName = "";

                foreach (var f in file)
                {

                    if (f != null)
                    {
                        #region file validation
                        var fileExtension = Path.GetExtension(f.FileName).ToLower();
                        if (!AllowedExtensions.Contains(fileExtension))
                        {
                            ModelState.AddModelError("File", "Invalid file type.");
                            return Json(new[] { file }.ToDataSourceResult(request, ModelState));

                        }

                        if (f.ContentLength > MaxFileSize)
                        {
                            ModelState.AddModelError("File", $"The uploaded file exceeds the maximum allowed size of {MaxFileSize / (1024 * 1024)} MB.");
                            return Json(new[] { file }.ToDataSourceResult(request, ModelState));

                        }
                        #endregion
                        if (ModelState.IsValid)
                        {
                            fileName = Path.GetFileName(f.FileName);
                            string physicalPath = Server.MapPath("~/Content/HRM/LeaveRequestFile/" + fileName);
                            f.SaveAs(physicalPath);
                            db.LeaveRequestAttachements.Add(new LeaveRequestAttachement()
                            {
                                FileName = Path.GetFileName(f.FileName),
                                LeaveRequestId = id,
                                ShortPath = shortPath,
                                FileExtension = fileExtension,
                                Description = "Attachment",
                                FilePath = physicalPath

                            });
                            db.SaveChanges();
                        }
                    }
                }
                #region operation log
                UserHelper.OperationLog(Request.UserHostAddress, "Attachment template", "Success", User.Identity.Name, $"uploaded  Attachment template {fileName} ", Modules.ArchiveAndLetters);
                #endregion
            }
            return Json(new[] { file }.ToDataSourceResult(request, ModelState));

        }


        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequestAttachements_Update([DataSourceRequest]DataSourceRequest request, LeaveRequestAttachement attachement)
        {
            if (ModelState.IsValid)
            {
                var entity = new LeaveRequestAttachement
                {
                    LeaveRequestAttachementId = attachement.LeaveRequestAttachementId,
                    LeaveRequestId = attachement.LeaveRequestId,
                    FileExtension = attachement.FileExtension,
                    Description = attachement.Description,
                    FilePath = attachement.FilePath,
                    FileName = attachement.FileName,
                    ShortPath = attachement.ShortPath
                };
                db.LeaveRequestAttachements.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { attachement }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult LeaveRequestAttachements_Destroy([DataSourceRequest]DataSourceRequest request, LeaveRequestAttachement attachement)
        {
            if (ModelState.IsValid)
            {
                var entity = new LeaveRequestAttachement
                {
                    LeaveRequestAttachementId = attachement.LeaveRequestAttachementId,
                    LeaveRequestId = attachement.LeaveRequestId,
                    FileExtension = attachement.FileExtension,
                    Description = attachement.Description,
                    FilePath = attachement.FilePath,
                    FileName = attachement.FileName,
                    ShortPath = attachement.ShortPath
                };

                db.LeaveRequestAttachements.Attach(entity);
                db.LeaveRequestAttachements.Remove(entity);
                db.SaveChanges();
                RemoveFiles(attachement.FileName);
            }

            return Json(new[] { attachement }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult RemoveFiles(string fileName)
        {
            if (fileName != null)
            {

                var physicalPath = Path.Combine(Server.MapPath("~/Content/HRM/LeaveRequestFile/"), fileName);

                if (System.IO.File.Exists(physicalPath))
                {
                    // Deleting the File from the Location
                    System.IO.File.Delete(physicalPath);
                }
            }

            return Content("");
        }
        public ActionResult DownloadAttachment(int attachmentId)
        {
            var record = db.LeaveRequestAttachements.Find(attachmentId);

            return File(record.FilePath, "application/force-download", Path.GetFileName(record.FilePath));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
