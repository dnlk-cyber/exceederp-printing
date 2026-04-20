using ExceedERP.Core.Domain.Printing;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingSummaryCommentController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Printing/PrintingSummaryComment
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id)
        {
            IQueryable<PrintingSummaryComment> estimationsummaries = db.PrintingSummaryComments.Where(x => x.PrintingEstimationSummaryId == id);
            return Json(estimationsummaries.ToDataSourceResult(request));
        }
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, PrintingSummaryComment model, int id)
        {
            if (ModelState.IsValid)
            {

                var entity = new PrintingSummaryComment
                {
                    Comment = model.Comment,
                    
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Now,
                    PrintingEstimationSummaryId = id
                };
                db.PrintingSummaryComments.Add(entity);
                db.SaveChanges();
                model.PrintingSummaryCommentId = entity.PrintingSummaryCommentId;
                model.CreatedBy = entity.CreatedBy;
                model.DateCreated = entity.DateCreated;
            }
            
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PrintingSummaryComment model)
        {
            if (ModelState.IsValid)
            {
                
                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Now;
                db.PrintingSummaryComments.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingSummaryComment model)
        {
            if (ModelState.IsValid)
            {
                var comment = db.PrintingSummaryComments.FirstOrDefault(x => x.PrintingSummaryCommentId == model.PrintingSummaryCommentId);
                
                db.PrintingSummaryComments.Remove(comment);
                db.SaveChanges();
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}