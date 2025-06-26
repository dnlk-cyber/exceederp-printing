using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.DataAccess.Context;
using ExceedERP.Web.Helpers;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetPlanAndEvaluation
{
    [AuthorizeRoles(FleetManagementRoles.FleetAnnualPlanner, FleetManagementRoles.FleetAnnualPlanApprover)]
    public class FleetAnnualPlanController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        // GET: FleetManagement/FleetAnnualPlan
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
        private void GenerateDate()
        {
            var details = db.FleetAnnualPlanDetails.ToList();
            foreach (var item in details)
            {
                item.DateFrom = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == item.GlPeriodId).DateFrom;
                item.DateTo = db.GLPeriods.FirstOrDefault(x => x.GLPeriodId == item.GlPeriodId).DateTo;
                db.Entry(item).State = EntityState.Modified;
            }
            db.SaveChanges();
        }
        public ActionResult FleetAnnualPlan_Read([DataSourceRequest] DataSourceRequest request)
        {
            var annualPlans = db.FleetAnnualPlans;
            
            return Json(annualPlans.ToDataSourceResult(request, ModelState));
        }

        public ActionResult FleetAnnualPlan_Create([DataSourceRequest] DataSourceRequest request, FleetAnnualPlan model)
        {
            if (ModelState.IsValid)
            {

                model.CreatedBy = User.Identity.Name;
                model.DateCreated = DateTime.Today;
                db.FleetAnnualPlans.Add(model);
                db.SaveChanges();
                GenerateAnnualPlanDetail(model.FleetAnnualPlanId);
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetAnnualPlan_Update([DataSourceRequest] DataSourceRequest request, FleetAnnualPlan model)
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
        public ActionResult FleetAnnualPlan_Delete([DataSourceRequest] DataSourceRequest request, FleetAnnualPlan model)
        {
            if (ModelState.IsValid)
            {

                var annualPlan = db.FleetAnnualPlans.Find(model.FleetAnnualPlanId);
                var fleetAnnualPlanDetails = db.FleetAnnualPlanDetails.Where(x => x.FleetAnnualPlanId == annualPlan.FleetAnnualPlanId).ToList();
                if (fleetAnnualPlanDetails.Any())
                {
                    db.FleetAnnualPlanDetails.RemoveRange(fleetAnnualPlanDetails);
                    db.SaveChanges();
                }
                db.FleetAnnualPlans.Remove(annualPlan);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        private void GenerateAnnualPlanDetail(int fleetAnnualPlanId)
        {
            var annualPlan = db.FleetAnnualPlans.Find(fleetAnnualPlanId);
            var glPeriod = db.GLPeriods.Where(x => x.GlFiscalYearId == annualPlan.GlFiscalYearId).ToList();
            foreach (var item in glPeriod)
            {
                var entity = new FleetAnnualPlanDetail
                {
                    GlPeriodId = item.GLPeriodId,
                    FleetAnnualPlanId = annualPlan.FleetAnnualPlanId,
                    DateFrom = item.DateFrom,
                    DateTo = item.DateTo
                };
                db.FleetAnnualPlanDetails.Add(entity);
            }
            db.SaveChanges();
        }
    }
}