using ExceedERP.Core.Domain.Maintenance;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.Maintenance.Controllers
{
    public class MaintenancePlanDetailController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Maintenance/MaintenancePlanDetail
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request, int id)
        {
            var detail = db.MaintenancePlanDetails.Where(x => x.MaintenancePlanId == id).ToList();

            return Json(detail.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update([DataSourceRequest] DataSourceRequest request, MaintenancePlanDetail model)
        {
           
            if (ModelState.IsValid)
            {
                model.CMCost = model.CMLaborCost + model.CMMaterialCost;
                model.PMCost = model.PMMaterialCost + model.PMLaborCost;
                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Today;
                
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();

            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, MaintenancePlanDetail model)
        {
            if (ModelState.IsValid)
            {

                var annualPlan = db.MaintenancePlanDetails.Find(model.MaintenancePlanDetailId);
                db.MaintenancePlanDetails.Remove(annualPlan);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}