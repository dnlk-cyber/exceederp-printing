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

namespace ExceedERP.Web.Areas.InfoDesk.Controllers
{
    public class InfoDeskKnowledgeBasesController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult InfoDeskKnowledgeBases_Read([DataSourceRequest]DataSourceRequest request)
        {
            var knowledgeBases = db.InfoDeskKnowledgeBases.ToList();
            DataSourceResult result = knowledgeBases.ToDataSourceResult(request, knowledgeBase => new
            {
                InfoDeskKnowledgeBaseId = knowledgeBase.InfoDeskKnowledgeBaseId,
                Problem = knowledgeBase.Problem,
                Solution = knowledgeBase.Solution,
                DateCreated = knowledgeBase.DateCreated,
                CreatedBy = knowledgeBase.CreatedBy,
                ModifiedBy = knowledgeBase.ModifiedBy,
                LastModified = knowledgeBase.LastModified

            });

            return Json(result);
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InfoDeskKnowledgeBases_Create([DataSourceRequest]DataSourceRequest request, InfoDeskKnowledgeBase knowledgeBase)
        {
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
        public ActionResult InfoDeskKnowledgeBases_Update([DataSourceRequest]DataSourceRequest request, InfoDeskKnowledgeBase knowledgeBase)
        {
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
        public ActionResult InfoDeskKnowledgeBases_Destroy([DataSourceRequest]DataSourceRequest request, InfoDeskKnowledgeBase knowledgeBase)
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
                DateCreated = knowledgeBase.DateCreated,
                CreatedBy = knowledgeBase.CreatedBy,
                ModifiedBy = knowledgeBase.ModifiedBy,
                LastModified = knowledgeBase.LastModified

            };
            return entity;
        }
       
        
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}
