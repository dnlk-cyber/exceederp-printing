using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class SatisfactionNoteController : Controller
    {
        // GET: Maintenance/SatisfactionNote
        ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request,Guid Id)
        {
            IQueryable<SatisfactionNote> Note = db.SatisfactionNote.Where(x=>x.ServiceCenterJobId==Id);
            DataSourceResult Result = Note.ToDataSourceResult(request,x=>new {
                SatisfactionNoteId = x.SatisfactionNoteId,
                ServiceCenterJobId = x.ServiceCenterJobId,
                WorkDescription = x.WorkDescription,
                RefNo = x.RefNo,
                Remark = x.Remark,
                Quantity = x.Quantity,
                Unit = x.Unit,
                DateCreated = x.DateCreated,
                CreatedBy = x.CreatedBy,
                LastModified = x.LastModified,
                ModifiedBy = x.ModifiedBy,
            });
               
            return Json(Result,JsonRequestBehavior.AllowGet);
        }
        public ActionResult Create([DataSourceRequest] DataSourceRequest request,SatisfactionNote Note,Guid Id)
        {
            ModelState.Remove("RefNo");
            ModelState.Remove("ServiceCenterJobId");
            Note.CreatedBy = User.Identity.Name;
            Note.DateCreated = DateTime.Now;
            Note.ServiceCenterJobId = Id;
            if (ModelState.IsValid)
            {
                db.SatisfactionNote.Add(Note);
                db.SaveChanges();
            }

            return Json(new []{ Note}.ToDataSourceResult(request,ModelState));
        }

        public ActionResult Update([DataSourceRequest]DataSourceRequest request,SatisfactionNote Note)
        {
            ModelState.Remove("RefNo");
            Note.LastModified = DateTime.Now;
            Note.ModifiedBy = User.Identity.Name;
            var fetchNote = db.SatisfactionNote.FirstOrDefault(x => x.SatisfactionNoteId == Note.SatisfactionNoteId);
            fetchNote.WorkDescription = Note.WorkDescription;
            fetchNote.Quantity = Note.Quantity;
            fetchNote.Unit = Note.Unit;
            fetchNote.Remark = Note.Remark;
            if(ModelState.IsValid)
            {
                db.Entry(fetchNote).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { Note }.ToDataSourceResult(request, ModelState));

        }
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request,SatisfactionNote Note)
        {
            var fetchNote = db.SatisfactionNote.FirstOrDefault(x => x.SatisfactionNoteId == Note.SatisfactionNoteId);
            
            if(ModelState.IsValid)
            {
                db.SatisfactionNote.Remove(fetchNote);
                db.SaveChanges();
            }
            return Json(new[] { Note }.ToDataSourceResult(request, ModelState));

        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}