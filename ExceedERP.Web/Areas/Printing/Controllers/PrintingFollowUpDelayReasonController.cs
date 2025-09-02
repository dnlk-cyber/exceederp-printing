using ExceedERP.Core.Domain.Printing.PrintingProduction;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Printing.Controllers
{
    public class PrintingFollowUpDelayReasonController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Printing/PrintingFollowUpDelayReason
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int detailId)
        {
            var entity = db.PrintingTaskIssues.Where(x => x.PrintingProductionDailyFollowUpDetailId == detailId);
            return Json(entity.ToDataSourceResult(request));
        }
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, PrintingTaskIssue model, int detailId)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingTaskIssue
                {
                   CreatedBy = User.Identity.Name,
                   DateCreated = DateTime.Now,
                   Description = model.Description,
                   PrintingProductionDailyFollowUpDetailId = detailId,
                   IssueType = model.IssueType,
                   ReportedTime = model.ReportedTime,
                   Resolution = model.Resolution,
                   ResolvedTime = model.ResolvedTime,
                   TimeSpentInMinute = model.TimeSpentInMinute,
                   
                };
                db.PrintingTaskIssues.Add(entity);               
                db.SaveChanges();
                model.CreatedBy = entity.CreatedBy;
                model.DateCreated = entity.DateCreated;
                model.PrintingTaskIssueId = entity.PrintingTaskIssueId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PrintingTaskIssue model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Now;
                db.PrintingTaskIssues.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingTaskIssue model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.PrintingTaskIssues.Find(model.PrintingTaskIssueId);
                db.PrintingTaskIssues.Remove(entity);
                
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}