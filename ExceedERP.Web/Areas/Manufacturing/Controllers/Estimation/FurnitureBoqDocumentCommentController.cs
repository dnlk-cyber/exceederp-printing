using ExceedERP.Core.Domain.Manufacturing.FurnitureEstimation;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Manufacturing.Controllers.Estimation
{
    public class FurnitureBoqDocumentCommentController : Controller
    {
        private readonly ExceedDbContext db = new ExceedDbContext();
        // GET: Manufacturing/FurnitureBoqDocumentComment
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            var comments = db.FurnitureBoqDocumentComments.Where(x => x.FurnitureBoqRequestId == id);
            return Json(comments.ToDataSourceResult(request));
        }
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, FurnitureBoqDocumentComment model, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new FurnitureBoqDocumentComment
                {
                    Comment = model.Comment,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    FurnitureBoqRequestId = id
                };
                db.FurnitureBoqDocumentComments.Add(entity);
                db.SaveChanges();
                model.FurnitureBoqDocumentCommentId = entity.FurnitureBoqDocumentCommentId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, FurnitureBoqDocumentComment model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, FurnitureBoqDocumentComment model)
        {
            if (ModelState.IsValid)
            {
                var comment = db.FurnitureBoqDocumentComments.Find(model.FurnitureBoqDocumentCommentId);
                db.FurnitureBoqDocumentComments.Remove(comment);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}