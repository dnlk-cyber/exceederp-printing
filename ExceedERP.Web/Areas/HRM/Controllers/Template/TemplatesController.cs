using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ExceedERP.DataAccess.Context;
using ExceedERP.Reporting.HRM.HRM.Letters;
using ExceedERP.Web.Controllers;
using PagedList;
using ExceedERP.Core.Domain.Common;

namespace ExceedERP.Web.Areas.HRM.Controllers.Template
{

    public class TemplatesController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: HRM/Templates
        public ActionResult Index(int? page, Core.Domain.HRM.TemplateTypes? templateType, int? templateTypeSettingId, string search)
        {
            int pageSize = 1;
            int pageNumber = (page ?? 1);

            var query = db.Templates.AsQueryable();
            if (templateType.HasValue)
            {
                query = query.Where(x => x.TemplateType == templateType.Value);
            }
            if (templateTypeSettingId.HasValue)
            {
                query = query.Where(x => x.TemplateTypeSettingId == templateTypeSettingId.Value);
            }
            if (!string.IsNullOrWhiteSpace(search))
            {
                query = query.Where(x => x.Title.Contains(search));
            }

            var templates = query.ToList()
                .Select(x=>new Core.Domain.HRM.Templates.Template {
                                    TemplateId= x.TemplateId,
                                    TemplateType= x.TemplateType,
                                    TempOne=x.TempOne,
                                    Title= x.Title,
                                    TemplateTypeSettingId=x.TemplateTypeSettingId,
                                    Document= x.Document,
                                    CreatedBy=    x.CreatedBy,
                                    DateCreated = x.DateCreated,
                                    OrgBranchName = x.OrgBranchName,
                                    LastModified = x.LastModified,
                                    ModifiedBy= x.ModifiedBy,
                                    TemplateTypes = GetTeplateTypeName(x.TemplateTypeSettingId??0),
                        }).OrderBy(t => t.TemplateId)
                        .ToPagedList(pageNumber, pageSize);
            // Fetch active employees for the dropdown
            ViewBag.Employees = new SelectList(
                db.Person_Employee.Select(e => new
                {
                    EmployeeId = e.EmployeeId,
                    FullName = e.FirstNameEnglish + " " + e.MiddleNameEnglish + " " + e.LastNameEnglish
                }).ToList(),
                "EmployeeId",
                "FullName"
            );

            // Options for the type/category search filters
            ViewBag.TemplateTypeSettings = new SelectList(db.TemplateTypeSettings.OrderBy(t => t.TemplateType).ToList(), "TemplateTypeSettingId", "TemplateType", templateTypeSettingId);
            ViewBag.SelectedTemplateType = templateType;
            ViewBag.SelectedTemplateTypeSettingId = templateTypeSettingId;
            ViewBag.SearchTerm = search;

            return View(templates);
        }

        public ActionResult TemplatePrint(int id, string employeeId = null)
        {
            using (var ds = new TemplateLettersObjectDataSource())
            {
                // Pass employeeId as headId or employee ID based on your DataSource implementation
                var letters = ds.GetTemplateLetters(
                    empId: employeeId,
                    id: id,
                    header: true,
                    includeExt: false,
                    language: "english",
                    type: null,
                    from: null,
                    to: null,
                    templatetype: id.ToString(), // Pass the templateId here
                    headId: employeeId
                );

                var letter = letters.FirstOrDefault();
                if (letter == null) return HttpNotFound();

                ViewBag.WaterMarkUrl = GetWaterMarkUrl();
                ViewBag.HeaderImageUrl = GetTemplateHeaderImageUrl();
                ViewBag.IsoLogoSrc = GetIsoLogoSrc();
                return View("TemplatePrint", letter);
            }
        }
        private string GetTeplateTypeName(int id)
        {
            return db.TemplateTypeSettings.FirstOrDefault(x => x.TemplateTypeSettingId == id)?.TemplateType;
        }

        private string GetTemplateHeaderImageUrl()
        {
            //var header = db.TemplateHeaderImages.FirstOrDefault(x => x.IsNeened);
            //return header != null && !string.IsNullOrEmpty(header.HeaderImageFileName)
            //    ? Url.Content("~/Content/Image/WaterMark/" + header.HeaderImageFileName)
            //    : null;
            return null;
        }


        private string GetIsoLogoSrc()
        {
            var isoLogo = db.Attachments.FirstOrDefault(a => a.ReferenceType == AttachmentType.ISO_LOGO);
            return isoLogo?.BinaryFile != null && isoLogo.BinaryFile.Length > 0
                ? "data:image/png;base64," + Convert.ToBase64String(isoLogo.BinaryFile)
                : null;
        }

        public ActionResult Details(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var template = db.Templates.Find(id);

            if (template == null)
                return HttpNotFound();

            //ViewBag.RecordTypeList = new SelectList(Enum.GetValues(typeof(RecordKeyType)));
            ViewBag.WaterMarkUrl = GetWaterMarkUrl();
            return View(template);

        }

        public ActionResult Create()
        {
            var model = new Core.Domain.HRM.Templates.Template(); // ✅ initializes the model
            ViewBag.WaterMarkUrl = GetWaterMarkUrl();
            return View(model);
        }

        private string GetWaterMarkUrl()
        {
            var waterMark = db.WaterMarks.FirstOrDefault(x => x.IsNeened);
            return waterMark != null && !string.IsNullOrEmpty(waterMark.WaterMarkFileName)
                ? Url.Content("~/Content/Image/WaterMark/" + waterMark.WaterMarkFileName)
                : null;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "TemplateId,Title,TempOne,TemplateType,TemplateTypeSettingId,Document")] Core.Domain.HRM.Templates.Template template)
        {
            template.CreatedBy = User.Identity.Name;
            template.DateCreated = DateTime.Now;
            var exist = db.Templates.Any(x => x.TemplateType == template.TemplateType && x.TemplateTypeSettingId == template.TemplateTypeSettingId);
            if (exist)
            {
                ModelState.AddModelError("exist", "this Template type is aready Registerd chage the Template type");
                return RedirectToAction("Index");

            }
            if (ModelState.IsValid)
            {
                db.Templates.Add(template);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(template);
        }
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

            var template = db.Templates.Find(id);
            if (template == null)
                return HttpNotFound();

            // Ensure Document contains raw HTML for Kendo Editor
            var model = new Core.Domain.HRM.Templates.Template
            {
                TemplateId = template.TemplateId,
                Title = template.Title,
                TemplateTypeSettingId = template.TemplateTypeSettingId,
                TemplateType = template.TemplateType,
                TemplateTypes = GetTeplateTypeName(template.TemplateTypeSettingId ?? 0),
                Document = HttpUtility.HtmlDecode(HttpUtility.HtmlDecode(template.Document)), // Decode if stored encoded
                ModifiedBy = template.ModifiedBy,
                LastModified = template.LastModified,
                CreatedBy = template.CreatedBy,
                DateCreated = template.DateCreated
            };

            ViewBag.WaterMarkUrl = GetWaterMarkUrl();
            return View(model);
        }

        //public ActionResult Edit(int? id)
        //{
        //    if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    var template = db.Templates.Find(id);
        //    if (template == null) return HttpNotFound();
        //    return View(template);
        //}

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "TemplateId,Title,TempOne,TemplateType,TemplateTypeSettingId,Document")] Core.Domain.HRM.Templates.Template template)
        {
            template.ModifiedBy = User.Identity.Name;
            template.LastModified = DateTime.Now;
            var exist = db.Templates.Any(x => x.TemplateType == template.TemplateType 
                                            && x.TemplateTypeSettingId == template.TemplateTypeSettingId 
                                            && x.TemplateId != template.TemplateId);
            if (exist)
            {
                ModelState.AddModelError("exist", "this Template type is aready Registerd chage the Template type");
                return RedirectToAction("Index");

            }
            if (ModelState.IsValid)
            {
                db.Entry(template).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(template);
        }

        public ActionResult Delete(int? id)
        {
            if (id == null) return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            var template = db.Templates.Find(id);
            if (template == null) return HttpNotFound();
            ViewBag.WaterMarkUrl = GetWaterMarkUrl();
            return View(template);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var template = db.Templates.Find(id);
            db.Templates.Remove(template);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public JsonResult GetTeplateType()
        {
            return Json(db.TemplateTypeSettings.Select(x => new { x.TemplateTypeSettingId, x.TemplateType }), JsonRequestBehavior.AllowGet);
        }
      
        protected override void Dispose(bool disposing)
        {
            if (disposing) db.Dispose();
            base.Dispose(disposing);
        }
        #region Template  Commented

        //// GET: HRM/Templates
        //public ActionResult Index()
        //{
        //    return View(db.Templates.ToList());
        //}

        //// GET: HRM/Templates/Details/5
        //public ActionResult Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    Core.Domain.HRM.Templates.Template template = db.Templates.Find(id);

        //    if (template == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(template);
        //}

        //// GET: HRM/Templates/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: HRM/Templates/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Create([Bind(Include = "TemplateId,TempOne,TemplateType,Document")] Core.Domain.HRM.Templates.Template template)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Templates.Add(template);
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }

        //    return View(template);
        //}

        //// GET: HRM/Templates/Edit/5
        //public ActionResult Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Core.Domain.HRM.Templates.Template template = db.Templates.Find(id);
        //    if (template == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(template);
        //}

        //// POST: HRM/Templates/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public ActionResult Edit([Bind(Include = "TemplateId,TempOne,TemplateType,Document")] Core.Domain.HRM.Templates.Template template)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(template).State = System.Data.Entity.EntityState.Modified;
        //        db.SaveChanges();
        //        return RedirectToAction("Index");
        //    }
        //    return View(template);
        //}

        //// GET: HRM/Templates/Delete/5
        //public ActionResult Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Core.Domain.HRM.Templates.Template template = db.Templates.Find(id);
        //    if (template == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(template);
        //}

        //// POST: HRM/Templates/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public ActionResult DeleteConfirmed(int id)
        //{
        //    Core.Domain.HRM.Templates.Template template = db.Templates.Find(id);
        //    db.Templates.Remove(template);
        //    db.SaveChanges();
        //    return RedirectToAction("Index");
        //}

        //protected override void Dispose(bool disposing)
        //{
        //    if (disposing)
        //    {
        //        db.Dispose();
        //    }
        //    base.Dispose(disposing);
        //}
        #endregion 
    }
}
