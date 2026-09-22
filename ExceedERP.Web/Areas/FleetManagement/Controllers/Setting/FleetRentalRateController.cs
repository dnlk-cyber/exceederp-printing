using ExceedERP.Core.Domain.FleetManagement;
using ExceedERP.DataAccess.Context;
using ExceedERP.Helpers.Common;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ExceedERP.Web.Areas.FleetManagement.Controllers.Setting
{
    public class FleetRentalRateController : Controller
    {
        ExceedDbContext db = new ExceedDbContext();
        private readonly CacheData CacheData = new CacheData();
        // GET: FleetManagement/FleetRentalRate
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
            ViewData["Branch"] = ViewData["branch"] = CacheData.GetBranchCaches().Select(
       i => new
       {
           Name = i.Name,
           BranchId = i.Id
       }).AsQueryable();
            ViewData["GlFiscalYear"] = db.GLFiscalYears.Select(s => new
            {
                Value = s.GlFiscalYearId,
                Text = s.Name
            }).ToList();
            return View();
        }

        public ActionResult RentalRate_Read([DataSourceRequest] DataSourceRequest request)
        {
            var rate = db.FleetRentalRates;

            return Json(rate.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetRentalRate_Create([DataSourceRequest] DataSourceRequest request, FleetRentalRate model)
        {
            if (ModelState.IsValid)
            {
                var glFiscalYear = db.GLFiscalYears.FirstOrDefault(x => x.GlFiscalYearId == model.GlFiscalYearId);

                db.FleetRentalRates.Add(model);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetRentalRate_Update([DataSourceRequest] DataSourceRequest request, FleetRentalRate model)
        {
            if (ModelState.IsValid)
            {
                db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        public ActionResult FleetRentalRate_Delete([DataSourceRequest] DataSourceRequest request, FleetRentalRate model)
        {
            if (ModelState.IsValid)
            {
                var fleetRentalRate = db.FleetRentalRates.Find(model.FleetRentalRateId);
                db.FleetRentalRates.Remove(fleetRentalRate);
                db.SaveChanges();
            }
            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}