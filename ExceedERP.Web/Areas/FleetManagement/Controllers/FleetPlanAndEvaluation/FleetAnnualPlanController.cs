using ExceedERP.Core.Domain.FleetManagement.EquipmentPlanandEvaluation;
using ExceedERP.DataAccess.Context;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.FleetPlanAndEvaluation
{
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
            return View();
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
                db.FleetAnnualPlans.Remove(annualPlan);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
    }
}