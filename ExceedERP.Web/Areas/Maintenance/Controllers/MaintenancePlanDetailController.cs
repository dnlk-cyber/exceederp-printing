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
                var fetchDetail = db.MaintenanceDailyPlan.Where(x => x.MaintenancePlanDetailId == model.MaintenancePlanDetailId).ToList();
                if (!fetchDetail.Any())
                {
                    GenerateMonthlyPlanDetail(model.MaintenancePlanDetailId);
                }
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, MaintenancePlanDetail model)
        {
            if (ModelState.IsValid)
            {

                var annualPlan = db.MaintenancePlanDetails.Find(model.MaintenancePlanDetailId);
                var detail=db.MaintenanceDailyPlan.Where(x => x.MaintenancePlanDetailId == model.MaintenancePlanDetailId).ToList();
                if (detail.Any())
                {
                    db.MaintenanceDailyPlan.RemoveRange(detail);
                    db.SaveChanges();
                }
                db.MaintenancePlanDetails.Remove(annualPlan);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult DailyRead([DataSourceRequest] DataSourceRequest request, int id)
        {
            var detail = db.MaintenanceDailyPlan.Where(x => x.MaintenancePlanDetailId == id).ToList();

            return Json(detail.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DailyUpdate([DataSourceRequest] DataSourceRequest request, MaintenanceDailyPlan model)
        {
            var parent = db.MaintenancePlanDetails.FirstOrDefault(x => x.MaintenancePlanDetailId == model.MaintenancePlanDetailId);
            var preDetails = db.MaintenanceDailyPlan.Where(x => x.MaintenancePlanDetailId == model.MaintenancePlanDetailId).ToList();
            if(preDetails.Any())
            {
                decimal cmCost = preDetails.Sum(x => x.CMCost);
                decimal pmCost = preDetails.Sum(x => x.PMCost);
                decimal cmQty = preDetails.Sum(x => x.CMQuantity);
                decimal pmQty = preDetails.Sum(x => x.PMQuantity);
                if((parent.CMCost<(cmCost+model.CMCost))|| (parent.PMCost < (pmCost + model.PMCost))||(parent.CMQuantity < (cmQty + model.CMQuantity))||(parent.PMQuantity < (pmQty + model.PMQuantity)))
                {
                    ModelState.AddModelError("exceeds", "Sum of daily plans exceeds Monthly plan");
                }
                
            }
            if (ModelState.IsValid)
            {
                var existing = db.MaintenanceDailyPlan.FirstOrDefault(x => x.MaintenanceDailyPlanId == model.MaintenanceDailyPlanId);
                existing.CMLaborCost = model.CMLaborCost;
                existing.CMMaterialCost = model.CMMaterialCost;
                existing.PMLaborCost = model.PMLaborCost;
                existing.PMMaterialCost = model.PMMaterialCost;
                existing.PMQuantity = model.PMQuantity;
                existing.CMQuantity = model.CMQuantity;
                existing.CMCost = model.CMLaborCost + model.CMMaterialCost;
                existing.PMCost = model.PMMaterialCost + model.PMLaborCost;
                existing.ModifiedBy = User.Identity.Name;
                existing.LastModified = DateTime.Today;
                
                db.Entry(existing).State = EntityState.Modified;
                db.SaveChanges();
               

            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult DailyDelete([DataSourceRequest] DataSourceRequest request, MaintenanceDailyPlan model)
        {
            if (ModelState.IsValid)
            {

                var dailyPlan = db.MaintenanceDailyPlan.Find(model.MaintenanceDailyPlanId);
                db.MaintenanceDailyPlan.Remove(dailyPlan);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void GenerateMonthlyPlanDetail(int planId)
        {
            var plan = db.MaintenancePlanDetails.Find(planId);
            var glPeriod = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == plan.GlPeriodId);
            var daysDif = (glPeriod.DateTo - glPeriod.DateFrom).Days;
            for(int i=1;i<=daysDif;i++)
            {
                var entity = new MaintenanceDailyPlan
                {
                    MaintenancePlanDetailId = planId,
                    Date = glPeriod.DateFrom.AddDays(i - 1),
                };
                db.MaintenanceDailyPlan.Add(entity);
            }
            db.SaveChanges();
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}