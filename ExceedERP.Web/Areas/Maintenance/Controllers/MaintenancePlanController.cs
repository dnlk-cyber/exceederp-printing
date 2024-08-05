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
    public class MaintenancePlanController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: Maintenance/MaintenancePlan
        public ActionResult Index()
        {
            ViewData["Category"] = db.EquipmentIdentity.Select(s => new
            {
                Value = s.EquipmentIdentityID,
                Text = s.EquipmentCategoryName
            }).ToList();
            ViewData["Type"] = db.EquipmentType.Select(s => new
            {
                Value = s.EquipmentTypeID,
                Text = s.EquipmentTypeName
            }).ToList();

            ViewData["Name"] = db.EquipmentName.Select(s => new
            {
                Value = s.EquipmentNameID,
                Text = s.Name
            }).ToList();
            ViewData["Fleet"] = db.Fleets.Select(s => new
            {
                Value = s.FleetsID,
                Text = s.PlateNumber + " " + s.SerialNo
            }).ToList();

            ViewData["GlFiscalYear"] = db.GLFiscalYears.Select(s => new
            {
                Value = s.GlFiscalYearId,
                Text = s.Name
            }).ToList();
            ViewData["GlPeriod"] = db.GLPeriods.Select(s => new
            {
                Value = s.GLPeriodId,
                Text = s.Name
            }).ToList();
            return View();
        }

        public ActionResult Read([DataSourceRequest] DataSourceRequest request)
        {
            var annualPlans = db.MaintenancePlans;

            return Json(annualPlans.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Create([DataSourceRequest] DataSourceRequest request, MaintenancePlan model)
        {
            if (ModelState.IsValid)
            {

                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Today;
                db.MaintenancePlans.Add(model);
                db.SaveChanges();
                GenerateAnnualPlanDetail(model.MaintenancePlanId);
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Update([DataSourceRequest] DataSourceRequest request, MaintenancePlan model)
        {
            if (ModelState.IsValid)
            {

                model.ModifiedBy = User.Identity.Name;
                model.LastModified = DateTime.Today;
                db.Entry(model).State = EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult Delete([DataSourceRequest] DataSourceRequest request, MaintenancePlan model)
        {
            if (ModelState.IsValid)
            {

                var maintenancePlan = db.MaintenancePlans.Find(model.MaintenancePlanId);
                var detail = db.MaintenancePlanDetails.Where(x => x.MaintenancePlanId == maintenancePlan.MaintenancePlanId).ToList();
                if (detail.Any())
                {
                    db.MaintenancePlanDetails.RemoveRange(detail);
                    db.SaveChanges();
                }
                db.MaintenancePlans.Remove(maintenancePlan);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void GenerateAnnualPlanDetail(int planId)
        {
            var plan = db.MaintenancePlans.Find(planId);
            var glPeriod = db.GLPeriods.Where(x => x.GlFiscalYearId == plan.GlFiscalYearId).ToList();
            foreach (var item in glPeriod)
            {
                var entity = new MaintenancePlanDetail
                {
                    GlPeriodId = item.GLPeriodId,
                    MaintenancePlanId = plan.MaintenancePlanId
                };
                db.MaintenancePlanDetails.Add(entity);
            }
            db.SaveChanges();
        }
    }
}