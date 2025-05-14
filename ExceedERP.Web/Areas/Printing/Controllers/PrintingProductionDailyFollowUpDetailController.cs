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
    public class PrintingProductionDailyFollowUpDetailController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int id) 
        {
            var entity = db.PrintingProductionDailyFollowUpDetails.Where(x => x.PrintingProductionFollowUpTaskId == id);
            return Json(entity.ToDataSourceResult(request));
        }
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, PrintingProductionDailyFollowUpDetail model, int id)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductionDailyFollowUpDetail
                {
                    ActualOutput = model.ActualOutput,
                    CreatedBy = User.Identity.Name,
                    DateCreated = DateTime.Today,
                    PrintingProductionFollowUpTaskId = id,
                    Remark = model.Remark,
                    TimeSpentHours = model.TimeSpentHours,
                    OperatorId = model.OperatorId,
                    ProductionEndTime = model.ProductionEndTime,
                    ProductionStartTime = model.ProductionStartTime,
                    UoM = model.UoM
                };
                db.PrintingProductionDailyFollowUpDetails.Add(entity);
                db.SaveChanges();
                model.PrintingProductionDailyFollowUpDetailId = entity.PrintingProductionDailyFollowUpDetailId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PrintingProductionDailyFollowUpDetail model)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingProductionDailyFollowUpDetail
                {
                    ActualOutput = model.ActualOutput,
                    CreatedBy = model.CreatedBy,
                    DateCreated = model.DateCreated,
                    OperatorId = model.OperatorId,
                    TimeSpentHours = model.TimeSpentHours,
                    Remark = model.Remark,
                    PrintingProductionFollowUpTaskId = model.PrintingProductionFollowUpTaskId,
                    ProductionEndTime = model.ProductionEndTime,
                    ProductionStartTime = model.ProductionStartTime,
                    UoM = model.UoM,
                    PrintingProductionDailyFollowUpDetailId = model.PrintingProductionDailyFollowUpDetailId,
                    LastModified = DateTime.Today,
                    ModifiedBy = User.Identity.Name
                };
                db.Entry(entity).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingProductionDailyFollowUpDetail model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.PrintingProductionDailyFollowUpDetails.Find(model.PrintingProductionDailyFollowUpDetailId);
                db.PrintingProductionDailyFollowUpDetails.Remove(entity);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}