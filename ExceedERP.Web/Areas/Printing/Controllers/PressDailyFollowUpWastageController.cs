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
    public class PressDailyFollowUpWastageController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Printing/PrintingFollowUpDelayReason
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int detailId)
        {
            var entity = db.PressDailyFollowUpWastages.Where(x => x.PrintingProductionDailyFollowUpDetailId == detailId);
            return Json(entity.ToDataSourceResult(request));
        }
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, PressDailyFollowUpWastage model, int detailId)
        {
            if (ModelState.IsValid)
            {
                var entity = new PressDailyFollowUpWastage
                {
                   CreatedBy = User.Identity.Name,
                   DateCreated = DateTime.Now,
                   Amount = model.Amount,
                   UOM = model.UOM,
                   PrintingProductionDailyFollowUpDetailId = detailId,
                   
                };
                db.PressDailyFollowUpWastages.Add(entity);               
                db.SaveChanges();
                model.CreatedBy = entity.CreatedBy;
                model.DateCreated = entity.DateCreated;
                model.PressDailyFollowUpWastageId = entity.PressDailyFollowUpWastageId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PressDailyFollowUpWastage model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Now;
                db.PressDailyFollowUpWastages.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PressDailyFollowUpWastage model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.PressDailyFollowUpWastages.Find(model.PressDailyFollowUpWastageId);
                db.PressDailyFollowUpWastages.Remove(entity);
                
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}