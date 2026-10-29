using System;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.Core.Domain.Planning;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;

namespace ExceedERP.Web.Areas.Planning.Controllers
{
    [AuthorizeRoles(PlanningRoles.Planning_Admin, PlanningRoles.Planning_User)]
    public class PastPlanAttachmentsController : Controller
    {
        private const int MaximumFileSize = 25 * 1024 * 1024;
        private static readonly string[] AllowedExtensions = { ".doc", ".docx", ".pdf", ".xls", ".xlsx", ".jpg", ".jpeg", ".png" };
        private readonly ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            ViewBag.FiscalYears = db.GLFiscalYears
                .OrderByDescending(x => x.DateFrom)
                .Select(x => new SelectListItem
                {
                    Value = x.GlFiscalYearId.ToString(),
                    Text = x.Name
                })
                .ToList();

            var attachments = db.PastPlanAttachments
                .OrderByDescending(x => x.PlanDate)
                .ThenByDescending(x => x.PastPlanAttachmentId)
                .ToList();

            return View(attachments);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(PastPlanUploadModel pastPlan, HttpPostedFileBase file)
        {
            ValidateUpload(pastPlan, file);

            if (ModelState.IsValid)
            {
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var storedFileName = Guid.NewGuid().ToString("N") + extension;
                var folder = Server.MapPath("~/App_Data/PastPlans");
                Directory.CreateDirectory(folder);
                file.SaveAs(Path.Combine(folder, storedFileName));

                var attachment = new PastPlanAttachment
                {
                    GlFiscalYearId = pastPlan.GlFiscalYearId,
                    PlanDate = pastPlan.PlanDate,
                    Title = pastPlan.Title,
                    Description = pastPlan.Description,
                    FileName = Path.GetFileName(file.FileName),
                    StoredFileName = storedFileName,
                    ContentType = file.ContentType,
                    FileSize = file.ContentLength,
                    UploadedAt = DateTime.Now,
                    UploadedBy = User.Identity.Name
                };

                db.PastPlanAttachments.Add(attachment);
                db.SaveChanges();
                TempData["Success"] = "Past plan uploaded successfully.";
                return RedirectToAction("Index");
            }

            ViewBag.FiscalYears = db.GLFiscalYears
                .OrderByDescending(x => x.DateFrom)
                .Select(x => new SelectListItem
                {
                    Value = x.GlFiscalYearId.ToString(),
                    Text = x.Name
                })
                .ToList();
            return View("Index", db.PastPlanAttachments.OrderByDescending(x => x.PlanDate).ToList());
        }

        [HttpGet]
        public ActionResult Download(int id)
        {
            var attachment = db.PastPlanAttachments.Find(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }

            var path = Server.MapPath("~/App_Data/PastPlans/" + attachment.StoredFileName);
            if (!System.IO.File.Exists(path))
            {
                return HttpNotFound("The attached file is no longer available.");
            }

            return File(path, attachment.ContentType ?? "application/octet-stream", attachment.FileName);
        }

        [HttpGet]
        public ActionResult Open(int id)
        {
            var attachment = db.PastPlanAttachments.Find(id);
            if (attachment == null)
            {
                return HttpNotFound();
            }

            var path = Server.MapPath("~/App_Data/PastPlans/" + attachment.StoredFileName);
            if (!System.IO.File.Exists(path))
            {
                return HttpNotFound("The attached file is no longer available.");
            }

            return File(path, MimeMapping.GetMimeMapping(attachment.StoredFileName));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id)
        {
            var attachment = db.PastPlanAttachments.Find(id);
            if (attachment != null)
            {
                var path = Server.MapPath("~/App_Data/PastPlans/" + attachment.StoredFileName);
                if (System.IO.File.Exists(path))
                {
                    System.IO.File.Delete(path);
                }

                db.PastPlanAttachments.Remove(attachment);
                db.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        private void ValidateUpload(PastPlanUploadModel pastPlan, HttpPostedFileBase file)
        {
            if (pastPlan == null || pastPlan.GlFiscalYearId <= 0 || !db.GLFiscalYears.Any(x => x.GlFiscalYearId == pastPlan.GlFiscalYearId))
            {
                ModelState.AddModelError("GlFiscalYearId", "Please select a valid fiscal year.");
            }

            if (file == null || file.ContentLength == 0)
            {
                ModelState.AddModelError("file", "Please select a file.");
                return;
            }

            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!AllowedExtensions.Contains(extension))
            {
                ModelState.AddModelError("file", "Allowed files: PDF, Word, Excel, JPG, JPEG, and PNG.");
            }
            else if (file.ContentLength > MaximumFileSize)
            {
                ModelState.AddModelError("file", "The file must be 25 MB or smaller.");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }

            base.Dispose(disposing);
        }
    }

    public class PastPlanUploadModel
    {
        [Required]
        [Display(Name = "Fiscal Year")]
        public int GlFiscalYearId { get; set; }

        [Required]
        [Display(Name = "Plan Date")]
        [DataType(DataType.Date)]
        public DateTime PlanDate { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; }

        [StringLength(2000)]
        public string Description { get; set; }
    }
}
