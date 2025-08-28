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
    public class PrintingDailyFollowUpLogBookController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Printing/PrintingFollowUpDelayReason
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int detailId)
        {
            var entity = db.PrintingDailyFollowUpLogBooks.Where(x => x.PrintingProductionDailyFollowUpDetailId == detailId);
            return Json(entity.ToDataSourceResult(request));
        }
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, PrintingDailyFollowUpLogBook model, int detailId)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingDailyFollowUpLogBook
                {
                   CreatedBy = User.Identity.Name,
                   DateCreated = DateTime.Now,
                   ActivityType = model.ActivityType,
                   TimeSpent = model.TimeSpent,
                   PrintingProductionDailyFollowUpDetailId = detailId,
                   
                };
                db.PrintingDailyFollowUpLogBooks.Add(entity);               
                db.SaveChanges();
                model.CreatedBy = entity.CreatedBy;
                model.DateCreated = entity.DateCreated;
                model.PrintingDailyFollowUpLogBookId = entity.PrintingDailyFollowUpLogBookId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PrintingDailyFollowUpLogBook model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Now;
                db.PrintingDailyFollowUpLogBooks.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingDailyFollowUpLogBook model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.PrintingDailyFollowUpLogBooks.Find(model.PrintingDailyFollowUpLogBookId);
                db.PrintingDailyFollowUpLogBooks.Remove(entity);
                
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}