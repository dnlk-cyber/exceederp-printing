using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using ExceedERP.DataAccess.Context;
using System.IO;
using ExceedERP.Core.Domain.InfoDesk;
using ExceedERP.Web.Controllers;

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    [Authorize]
    public class KnowledgeBaseResolutionsController : BaseController
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request)
        {
            var knowledgeBases = db.InfoDeskKnowledgeBases.Where(x=>x.DocumentType==InfoDocumentType.DocumentResolution).ToList();
            DataSourceResult result = knowledgeBases.ToDataSourceResult(request, knowledgeBase => new
            {
                InfoDeskKnowledgeBaseId = knowledgeBase.InfoDeskKnowledgeBaseId,
                Problem = knowledgeBase.Problem,
                Solution = knowledgeBase.Solution,
                DocumentType = knowledgeBase.DocumentType,
                IsOnlineApproved = knowledgeBase.IsOnlineApproved,
                OnlineApprovedBy = knowledgeBase.OnlineApprovedBy,
                OnlineApprovedTime = knowledgeBase.OnlineApprovedTime,
                DateCreated = knowledgeBase.DateCreated,
                CreatedBy = knowledgeBase.CreatedBy,
                ModifiedBy = knowledgeBase.ModifiedBy,
                LastModified = knowledgeBase.LastModified

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, InfoDeskKnowledgeBase knowledgeBase)
        {
            if (knowledgeBase.DocumentType == InfoDocumentType.KnowledgeBase)
            {
                ModelState.AddModelError("exist", "you can't add Knowledge base Document type ");
                return Json(new[] { knowledgeBase }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                knowledgeBase.DateCreated = DateTime.Now;
                knowledgeBase.CreatedBy = User.Identity.Name;
                var entity = GetMappedObject(knowledgeBase);
                db.InfoDeskKnowledgeBases.Add(entity);
                db.SaveChanges();
                knowledgeBase.InfoDeskKnowledgeBaseId = entity.InfoDeskKnowledgeBaseId;
            }

            return Json(new[] { knowledgeBase }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, InfoDeskKnowledgeBase knowledgeBase)
        {
            if (knowledgeBase.DocumentType == InfoDocumentType.KnowledgeBase)
            {
                ModelState.AddModelError("exist", "you can't add Knowledge base Document type ");
                return Json(new[] { knowledgeBase }.ToDataSourceResult(request, ModelState));

            }
            if (ModelState.IsValid)
            {
                knowledgeBase.LastModified = DateTime.Now;
                knowledgeBase.ModifiedBy = User.Identity.Name;
                var entity = GetMappedObject(knowledgeBase);
                db.InfoDeskKnowledgeBases.Attach(entity);
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { knowledgeBase }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Destroy([DataSourceRequest]DataSourceRequest request, InfoDeskKnowledgeBase knowledgeBase)
        {
            if (ModelState.IsValid)
            {
                var entity = GetMappedObject(knowledgeBase);
                db.InfoDeskKnowledgeBases.Attach(entity);
                db.InfoDeskKnowledgeBases.Remove(entity);
                db.SaveChanges();
            }

            return Json(new[] { knowledgeBase }.ToDataSourceResult(request, ModelState));
        }
        private InfoDeskKnowledgeBase GetMappedObject(InfoDeskKnowledgeBase knowledgeBase)
        {
            var entity = new InfoDeskKnowledgeBase
            {
                InfoDeskKnowledgeBaseId = knowledgeBase.InfoDeskKnowledgeBaseId,
                Problem = knowledgeBase.Problem,
                Solution = knowledgeBase.Solution,
                DocumentType = knowledgeBase.DocumentType,
                IsOnlineApproved = knowledgeBase.IsOnlineApproved,
                OnlineApprovedBy = knowledgeBase.OnlineApprovedBy,
                OnlineApprovedTime = knowledgeBase.OnlineApprovedTime,
                DateCreated = knowledgeBase.DateCreated,
                CreatedBy = knowledgeBase.CreatedBy,
                ModifiedBy = knowledgeBase.ModifiedBy,
                LastModified = knowledgeBase.LastModified

            };
            return entity;
        }
        public JsonResult ApproveKnowledgeBases(int id, string reason)
        {
            var knowledgeBase = db.InfoDeskKnowledgeBases.Find(id);
            if (knowledgeBase == null)
            {
                return Json(new { Message = "Record not found.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            if (knowledgeBase.IsOnlineApproved)
            {
                return Json(new { Message = "already aproved.", Status = "Not-Ok" }, JsonRequestBehavior.AllowGet);
            }

            // Update ticket 
            if (!string.IsNullOrWhiteSpace(reason))
            {
                if (string.IsNullOrWhiteSpace(knowledgeBase.Solution))
                    knowledgeBase.Solution = reason;
                else
                    knowledgeBase.Solution += ", " + reason;
            }
            knowledgeBase.IsOnlineApproved = true;
            knowledgeBase.OnlineApprovedBy = User.Identity.Name;
            knowledgeBase.OnlineApprovedTime = DateTime.Now;
            // Mark entities as modified
            db.Entry(knowledgeBase).State = EntityState.Modified;
            db.SaveChanges();


            return Json(new { Message = "Successfully Approved ", Status = "Ok" }, JsonRequestBehavior.AllowGet);

        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
