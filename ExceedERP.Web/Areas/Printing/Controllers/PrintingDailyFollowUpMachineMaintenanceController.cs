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
    public class PrintingDailyFollowUpMachineMaintenanceController : Controller
    {
        private ExceedDbContext db = new ExceedDbContext();
        // GET: Printing/PrintingFollowUpDelayReason
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Read([DataSourceRequest]DataSourceRequest request, int detailId)
        {
            var entity = db.PrintingDailyFollowUpMachineMaintenances.Where(x => x.PrintingProductionDailyFollowUpDetailId == detailId);
            return Json(entity.ToDataSourceResult(request));
        }
        public ActionResult Create([DataSourceRequest]DataSourceRequest request, PrintingDailyFollowUpMachineMaintenance model, int detailId)
        {
            if (ModelState.IsValid)
            {
                var entity = new PrintingDailyFollowUpMachineMaintenance
                {
                   CreatedBy = User.Identity.Name,
                   DateCreated = DateTime.Now,
                   IssueReportedTime = model.IssueReportedTime,
                   MaintenanceEndTime = model.MaintenanceEndTime,
                   MaintenanceStartTime = model.MaintenanceStartTime,
                   TechnicianId = model.TechnicianId,
                   MaintenanceTime = model.MaintenanceTime,
                   Reason = model.Reason,
                   Remark = model.Remark,
                   PrintingProductionDailyFollowUpDetailId = detailId,
                   
                };
                db.PrintingDailyFollowUpMachineMaintenances.Add(entity);               
                db.SaveChanges();
                model.CreatedBy = entity.CreatedBy;
                model.DateCreated = entity.DateCreated;
                model.PrintingDailyFollowUpMachineMaintenanceId = entity.PrintingDailyFollowUpMachineMaintenanceId;
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update([DataSourceRequest]DataSourceRequest request, PrintingDailyFollowUpMachineMaintenance model)
        {
            if (ModelState.IsValid)
            {
                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Now;
                db.PrintingDailyFollowUpMachineMaintenances.Attach(model);
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult Delete([DataSourceRequest]DataSourceRequest request, PrintingDailyFollowUpMachineMaintenance model)
        {
            if (ModelState.IsValid)
            {
                var entity = db.PrintingDailyFollowUpMachineMaintenances.Find(model.PrintingDailyFollowUpMachineMaintenanceId);
                db.PrintingDailyFollowUpMachineMaintenances.Remove(entity);
                
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}